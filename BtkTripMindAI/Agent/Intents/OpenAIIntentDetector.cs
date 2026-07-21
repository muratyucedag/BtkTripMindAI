using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BtkTripMindAI.Agent.Intents
{
    public class OpenAIIntentDetector : IIntentDetector
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OpenAIIntentDetector> _logger;
        public OpenAIIntentDetector(HttpClient httpClient, IConfiguration configuration, ILogger<OpenAIIntentDetector> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<TravelIntent> DetectAsync(string message, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return TravelIntent.Unknown;
            }

            string? apiKey = _configuration["OpenAI:ApiKey"];
            string model = _configuration["OpenAI:Model"] ?? "gpt-5-mini";

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("OpenAI API anahtarı bulunamadı.");
            }

            var requestBody = new
            {
                model,
                instructions =
                    """
                    Sen BtkTripMindAI isimli bir seyahat asistanının
                    intent sınıflandırma bileşenisin.

                    Kullanıcının mesajını analiz et ve yalnızca aşağıdaki
                    intent değerlerinden birini seç:

                    - Unknown
                    - CityRecommendation
                    - Weather
                    - Hotel
                    - Restaurant
                    - Route
                    - Currency
                    - Flight
                    - Transportation
                    - Attraction

                    Intent açıklamaları:

                    Weather:
                    Hava durumu, sıcaklık, yağmur, kar, rüzgar veya iklim
                    soruları.

                    Hotel:
                    Otel, konaklama, pansiyon, apart, resort veya kalacak
                    yer soruları.

                    Restaurant:
                    Restoran, yemek, kahvaltı, kafe, lokanta veya mutfak
                    önerileri.

                    Route:
                    Gezi planı, günlük program, seyahat rotası veya
                    kaç günlük plan oluşturma talepleri.

                    Currency:
                    Döviz kuru, para çevirme, euro, dolar, sterlin veya
                    para birimi hesaplama soruları.

                    Flight:
                    Uçuş, uçak bileti, havayolu veya sefer araştırma
                    talepleri.

                    Transportation:
                    Havalimanı transferi, metro, tren, otobüs, taksi veya
                    bir noktadan başka bir noktaya ulaşım soruları.

                    Attraction:
                    Gezilecek yerler, müzeler, aktiviteler veya görülmesi
                    gereken noktalar.

                    CityRecommendation:
                    Kullanıcının hangi şehre veya ülkeye gitmesi gerektiği
                    konusunda öneri istemesi.

                    Unknown:
                    Yukarıdaki kategorilerden hiçbirine uymayan mesajlar.

                    Kurallar:
                    - Yalnızca tek bir intent seç.
                    - Açıklama yazma.
                    - Markdown kullanma.
                    - Kullanıcının mesajında birden fazla konu varsa ana
                      isteği seç.
                    - Sonucu yalnızca JSON olarak döndür.

                    JSON biçimi:

                    {
                      "intent": "Weather"
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
                using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);

                string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("OpenAI intent isteği başarısız. " + "StatusCode: {StatusCode}, Response: {Response}", response.StatusCode, responseContent);

                    return TravelIntent.Unknown;
                }

                string? outputText = GetOutputText(responseContent);

                if (string.IsNullOrWhiteSpace(outputText))
                {
                    return TravelIntent.Unknown;
                }

                IntentExtractionResult? result = JsonSerializer.Deserialize<IntentExtractionResult>(outputText,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (result == null || string.IsNullOrWhiteSpace(result.Intent))
                {
                    return TravelIntent.Unknown;
                }

                bool isParsed = Enum.TryParse(result.Intent, ignoreCase: true, out TravelIntent parsedIntent);

                return isParsed ? parsedIntent : TravelIntent.Unknown;
            }
            catch (TaskCanceledException)
                when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogError("OpenAI intent tespit isteği zaman aşımına uğradı.");

                return TravelIntent.Unknown;
            }
            catch (JsonException exception)
            {
                _logger.LogError(exception, "OpenAI intent cevabı JSON formatına çevrilemedi.");

                return TravelIntent.Unknown;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Intent tespit edilirken beklenmeyen bir hata oluştu.");

                return TravelIntent.Unknown;
            }
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

        private sealed class IntentExtractionResult
        {
            public string? Intent { get; set; }
        }
    }
}
