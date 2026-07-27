using BtkTripMindAI.Dtos.ImageDtos;
using System.Text.Json;

namespace BtkTripMindAI.Services.ImageServices
{
    public class GooglePlacePhotoService : IGooglePlacePhotoService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GooglePlacePhotoService> _logger;
        public GooglePlacePhotoService(HttpClient httpClient, IConfiguration configuration, ILogger<GooglePlacePhotoService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string?> GetPhotoUrlAsync(string? photoName, int maxWidth = 1200)
        {
            if (string.IsNullOrWhiteSpace(photoName))
            {
                return null;
            }

            string? apiKey = _configuration["GooglePlaces:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Google Places API anahtarı bulunamadı.");
            }

            if (maxWidth < 1)
            {
                maxWidth = 1200;
            }

            if (maxWidth > 4800)
            {
                maxWidth = 4800;
            }

            string encodedApiKey = Uri.EscapeDataString(apiKey);

            string requestUrl = $"https://places.googleapis.com/v1/{photoName}/media" + $"?maxWidthPx={maxWidth}" + $"&skipHttpRedirect=true" + $"&key={encodedApiKey}";

            try
            {
                using HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Google Place fotoğraf isteği başarısız. " + "StatusCode: {StatusCode}, Response: {Response}", response.StatusCode, responseContent);
                    return null;
                }

                GooglePlacePhotoResponseDto? photoResponse = JsonSerializer.Deserialize<GooglePlacePhotoResponseDto>(
                            responseContent,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });
                return photoResponse?.PhotoUri;
            }
            catch (JsonException exception)
            {
                _logger.LogError(exception, "Google Place fotoğraf cevabı çözümlenemedi.");
                return null;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Google Place fotoğrafı alınırken hata oluştu.");
                return null;
            }
        }
    }
}