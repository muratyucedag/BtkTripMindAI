using BtkTripMindAI.Models.FlightModels;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BtkTripMindAI.Agent.Extractors
{
    public class OpenAIFlightSearchExtractor : IFlightSearchExtractor
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OpenAIFlightSearchExtractor> _logger;
        public OpenAIFlightSearchExtractor(HttpClient httpClient, IConfiguration configuration, ILogger<OpenAIFlightSearchExtractor> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<FlightSearchExtractionResult> ExtractAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return EmptyResult();
            }

            string? apiKey = _configuration["OpenAI:ApiKey"];
            string model = _configuration["OpenAI:Model"] ?? "gpt-5-mini";

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("OpenAI API anahtarı bulunamadı.");
            }

            string currentDate = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            var requestBody = new
            {
                model,

                instructions =
                    $$"""
                    Sen BtkTripMindAI isimli seyahat uygulamasında
                    kullanıcının mesajından uçuş arama bilgilerini
                    çıkaran veri ayrıştırma aracısın.

                    Bugünün tarihi:
                    {{currentDate}}

                    Kullanıcının mesajından şu alanları çıkar:

                    - Kalkış şehri veya havalimanı
                    - Varış şehri veya havalimanı
                    - Gidiş tarihi
                    - Dönüş tarihi
                    - Yetişkin sayısı
                    - Çocuk sayısı
                    - Uçuş sınıfı
                    - Tek yön veya gidiş-dönüş bilgisi

                    Kurallar:

                    - Sonuç yalnızca geçerli JSON olmalıdır.
                    - Açıklama veya markdown ekleme.
                    - Kalkış ve varış noktalarını kullanıcının ifade
                      ettiği biçime yakın şekilde döndür.
                    - IATA kodu üretme veya tahmin etme.
                    - Kullanıcı havalimanı adı verdiyse havalimanı adını,
                      şehir verdiyse şehir adını döndür.
                    - Tarihleri yyyy-MM-dd biçiminde döndür.
                    - Kullanıcı yıl belirtmediyse bugünden sonraki en
                      yakın uygun tarihi kullan.
                    - Kullanıcı yetişkin sayısını belirtmediyse 1 döndür.
                    - Kullanıcı çocuk belirtmediyse 0 döndür.
                    - "Eşimle" veya "eşim ve ben" ifadesi 2 yetişkin
                      anlamına gelir.
                    - Uçuş sınıfı yalnızca şu değerlerden biri olmalıdır:
                      ECONOMY, PREMIUM_ECONOMY, BUSINESS, FIRST.
                    - Ekonomik veya ekonomi ifadelerinde ECONOMY döndür.
                    - Business veya iş sınıfında BUSINESS döndür.
                    - Birinci sınıf ifadelerinde FIRST döndür.
                    - Dönüş tarihi varsa isRoundTrip true olmalıdır.
                    - Dönüş tarihi yoksa isRoundTrip false olmalıdır.
                    - Kalkış, varış veya gidiş tarihi güvenilir şekilde
                      belirlenemiyorsa found false döndür.
                    - Rastgele şehir, havalimanı veya tarih üretme.

                    Başarılı JSON örneği:

                    {
                      "found": true,
                      "departureLocation": "Sabiha Gökçen",
                      "arrivalLocation": "Esenboğa",
                      "departureDate": "2026-08-05",
                      "returnDate": null,
                      "adultCount": 2,
                      "childCount": 0,
                      "travelClass": "ECONOMY",
                      "isRoundTrip": false
                    }

                    Gidiş-dönüş örneği:

                    {
                      "found": true,
                      "departureLocation": "İstanbul",
                      "arrivalLocation": "Roma",
                      "departureDate": "2026-09-10",
                      "returnDate": "2026-09-14",
                      "adultCount": 2,
                      "childCount": 1,
                      "travelClass": "ECONOMY",
                      "isRoundTrip": true
                    }

                    Bilgi eksikse:

                    {
                      "found": false,
                      "departureLocation": null,
                      "arrivalLocation": null,
                      "departureDate": null,
                      "returnDate": null,
                      "adultCount": 1,
                      "childCount": 0,
                      "travelClass": "ECONOMY",
                      "isRoundTrip": false
                    }
                    """,

                input = message
            };

            string requestJson = JsonSerializer.Serialize(requestBody);

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            try
            {
                using HttpResponseMessage response = await _httpClient.SendAsync(request);

                string responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("OpenAI uçuş arama bilgisi çıkarma isteği başarısız. " + "StatusCode: {StatusCode}, Response: {Response}", response.StatusCode, responseContent);

                    return EmptyResult();
                }

                string? outputText = GetOutputText(responseContent);

                if (string.IsNullOrWhiteSpace(outputText))
                {
                    return EmptyResult();
                }

                FlightSearchExtractionResult? result = JsonSerializer.Deserialize<FlightSearchExtractionResult>(outputText,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                return NormalizeResult(result);
            }
            catch (JsonException exception)
            {
                _logger.LogError(exception, "Uçuş arama bilgisi JSON cevabı ayrıştırılamadı.");

                return EmptyResult();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Uçuş arama bilgileri çıkarılırken hata oluştu.");

                return EmptyResult();
            }
        }

        private static FlightSearchExtractionResult NormalizeResult(FlightSearchExtractionResult? result)
        {
            if (result is null || !result.Found || string.IsNullOrWhiteSpace(result.DepartureLocation) || string.IsNullOrWhiteSpace(result.ArrivalLocation) || result.DepartureDate is null)
            {
                return EmptyResult();
            }

            result.DepartureLocation = result.DepartureLocation.Trim();

            result.ArrivalLocation = result.ArrivalLocation.Trim();

            if (result.AdultCount <= 0)
            {
                result.AdultCount = 1;
            }

            if (result.ChildCount < 0)
            {
                result.ChildCount = 0;
            }

            result.TravelClass = result.TravelClass?.Trim().ToUpperInvariant() switch
            {
                "PREMIUM_ECONOMY" => "PREMIUM_ECONOMY",
                "BUSINESS" => "BUSINESS",
                "FIRST" => "FIRST",
                _ => "ECONOMY"
            };

            if (result.ReturnDate is not null && result.ReturnDate <= result.DepartureDate)
            {
                result.ReturnDate = null; result.IsRoundTrip = false;
            }
            else
            {
                result.IsRoundTrip = result.ReturnDate is not null;
            }

            return result;
        }

        private static string? GetOutputText(string responseContent)
        {
            using JsonDocument document = JsonDocument.Parse(responseContent);

            JsonElement root = document.RootElement;

            if (root.TryGetProperty("output_text", out JsonElement outputTextElement))
            {
                return outputTextElement.GetString();
            }

            if (!root.TryGetProperty("output", out JsonElement outputElement) || outputElement.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            foreach (JsonElement outputItem in outputElement.EnumerateArray())
            {
                if (!outputItem.TryGetProperty("content", out JsonElement contentElement) || contentElement.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                foreach (JsonElement contentItem in contentElement.EnumerateArray())
                {
                    if (contentItem.TryGetProperty("text", out JsonElement textElement))
                    {
                        return textElement.GetString();
                    }
                }
            }

            return null;
        }

        private static FlightSearchExtractionResult EmptyResult()
        {
            return new FlightSearchExtractionResult
            {
                Found = false,
                DepartureLocation = null,
                ArrivalLocation = null,
                DepartureDate = null,
                ReturnDate = null,
                AdultCount = 1,
                ChildCount = 0,
                TravelClass = "ECONOMY",
                IsRoundTrip = false
            };
        }
    }
}
