using BtkTripMindAI.Models.EmailModels;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BtkTripMindAI.Agent.Email
{
    public class OpenAIEmailExtractor : IEmailExtractor
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OpenAIEmailExtractor> _logger;
        public OpenAIEmailExtractor(HttpClient httpClient, IConfiguration configuration, ILogger<OpenAIEmailExtractor> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<EmailExtractionResult> ExtractAsync(string message)
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

            var requestBody = new
            {
                model,

                instructions =
                    """
                    Sen bir seyahat uygulamasında kullanıcının mesajından
                    e-posta adresini çıkaran veri ayrıştırma aracısın.

                    Kurallar:
                    - Mesajda açıkça yazılmış geçerli bir e-posta adresini bul.
                    - E-posta adresi yoksa tahmin yapma.
                    - Kullanıcının kayıtlı veya varsayılan e-posta adresini
                      bildiğini varsayma.
                    - Birden fazla e-posta adresi varsa gönderim yapılması
                      istenen ilk adresi döndür.
                    - Sonuç yalnızca geçerli JSON olmalıdır.
                    - Açıklama veya markdown ekleme.

                    E-posta bulunduğunda JSON biçimi:

                    {
                      "found": true,
                      "email": "murat@example.com"
                    }

                    E-posta bulunamadığında:

                    {
                      "found": false,
                      "email": null
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
                    _logger.LogError("OpenAI e-posta çıkarma isteği başarısız. " + "StatusCode: {StatusCode}, Response: {Response}", response.StatusCode, responseContent);

                    return EmptyResult();
                }

                string? outputText = GetOutputText(responseContent);

                if (string.IsNullOrWhiteSpace(outputText))
                {
                    return EmptyResult();
                }

                EmailExtractionResult? result = JsonSerializer.Deserialize<EmailExtractionResult>(outputText,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (result is null || !result.Found || string.IsNullOrWhiteSpace(result.Email))
                {
                    return EmptyResult();
                }

                result.Email = result.Email.Trim();

                return result;
            }
            catch (JsonException exception)
            {
                _logger.LogError(exception, "E-posta çıkarma JSON cevabı ayrıştırılamadı.");

                return EmptyResult();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "E-posta adresi çıkarılırken hata oluştu.");

                return EmptyResult();
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
        private static EmailExtractionResult EmptyResult()
        {
            return new EmailExtractionResult
            {
                Found = false,
                Email = null
            };
        }
    }
}