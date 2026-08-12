using BtkTripMindAI.Models.CalendarModels;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BtkTripMindAI.Agent.Extractors
{
    public class OpenAICalendarEventExtractor : ICalendarEventExtractor
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OpenAICalendarEventExtractor> _logger;
        public OpenAICalendarEventExtractor(HttpClient httpClient, IConfiguration configuration, ILogger<OpenAICalendarEventExtractor> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<CalendarEventExtractionResult> ExtractAsync(string message)
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
                    Sen BtkTripMindAI seyahat uygulamasında kullanıcının
                    mesajından Google Calendar etkinlik bilgilerini çıkaran
                    veri ayrıştırma aracısın.

                    Bugünün tarihi:
                    {{currentDate}}

                    Kullanıcının mesajından şu alanları çıkar:

                    - Etkinlik başlığı
                    - Açıklama
                    - Konum
                    - Başlangıç tarihi ve saati
                    - Bitiş tarihi ve saati
                    - IANA saat dilimi

                    Kurallar:

                    - Sonuç yalnızca geçerli JSON olmalıdır.
                    - Markdown veya açıklama ekleme.
                    - Kullanıcı şehir belirttiyse başlık
                      "[Şehir] Seyahati" biçiminde olabilir.
                    - Konum şehir ve ülke biçiminde olmalıdır.
                    - Tarihleri yyyy-MM-ddTHH:mm:ss biçiminde döndür.
                    - Saat dilimini IANA biçiminde döndür.
                      Örnek: Europe/Rome, Europe/Istanbul.
                    - Kullanıcı yıl belirtmediyse bugünden sonraki
                      en yakın uygun tarihi kullan.
                    - Kullanıcı başlangıç saati belirtmediyse 09:00 kullan.
                    - Kullanıcı bitiş saati belirtmediyse 20:00 kullan.
                    - Kullanıcı yalnızca tek tarih belirttiyse etkinliği
                      aynı gün 09:00 ile 20:00 arasında oluştur.
                    - Başlangıç veya bitiş tarihi güvenilir şekilde
                      belirlenemiyorsa found false döndür.
                    - Tahmin edilmemesi gereken kritik tarih bilgilerinde
                      rastgele değer üretme.
                    - Açıklama kısa ve seyahatle ilgili olmalıdır.

                    Başarılı JSON örneği:

                    {
                      "found": true,
                      "title": "Roma Seyahati",
                      "description": "BtkTripMindAI tarafından oluşturulan Roma seyahat planı.",
                      "location": "Rome, Italy",
                      "startDateTime": "2026-09-10T09:00:00",
                      "endDateTime": "2026-09-14T20:00:00",
                      "timeZone": "Europe/Rome"
                    }

                    Tarih bilgisi bulunamadığında:

                    {
                      "found": false,
                      "title": null,
                      "description": null,
                      "location": null,
                      "startDateTime": null,
                      "endDateTime": null,
                      "timeZone": "Europe/Istanbul"
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
                    _logger.LogError("OpenAI takvim bilgisi çıkarma isteği başarısız. " + "StatusCode: {StatusCode}, Response: {Response}", response.StatusCode, responseContent);
                    return EmptyResult();
                }

                string? outputText = GetOutputText(responseContent);

                if (string.IsNullOrWhiteSpace(outputText))
                {
                    return EmptyResult();
                }

                CalendarEventExtractionResult? result = JsonSerializer.Deserialize<CalendarEventExtractionResult>(
                        outputText,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                return NormalizeResult(result);
            }
            catch (JsonException exception)
            {
                _logger.LogError(exception, "Takvim bilgisi JSON cevabı ayrıştırılamadı.");

                return EmptyResult();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Takvim etkinliği bilgileri çıkarılırken hata oluştu.");

                return EmptyResult();
            }
        }
        private static CalendarEventExtractionResult NormalizeResult(CalendarEventExtractionResult? result)
        {
            if (result is null || !result.Found || result.StartDateTime is null || result.EndDateTime is null)
            {
                return EmptyResult();
            }

            if (result.EndDateTime <= result.StartDateTime)
            {
                return EmptyResult();
            }

            result.Title = string.IsNullOrWhiteSpace(result.Title) ? "Seyahat Planı" : result.Title.Trim();
            result.Description = string.IsNullOrWhiteSpace(result.Description) ? "BtkTripMindAI tarafından oluşturulan seyahat planı." : result.Description.Trim();
            result.Location = string.IsNullOrWhiteSpace(result.Location) ? null : result.Location.Trim();
            result.TimeZone = string.IsNullOrWhiteSpace(result.TimeZone) ? "Europe/Istanbul" : result.TimeZone.Trim();
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

        private static CalendarEventExtractionResult EmptyResult()
        {
            return new CalendarEventExtractionResult
            {
                Found = false,
                Title = null,
                Description = null,
                Location = null,
                StartDateTime = null,
                EndDateTime = null,
                TimeZone = "Europe/Istanbul"
            };
        }
    }
}