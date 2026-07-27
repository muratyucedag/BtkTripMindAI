using BtkTripMindAI.Dtos.HotelDtos;
using BtkTripMindAI.Models.HotelModels;
using BtkTripMindAI.Services.ImageServices;
using System.Text;
using System.Text.Json;

namespace BtkTripMindAI.Services.HotelServices
{
    public class HotelService : IHotelService
    {
        private const string SearchEndpoint = "https://places.googleapis.com/v1/places:searchText";

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IGooglePlacePhotoService _photoService;
        private readonly ILogger<HotelService> _logger;
        public HotelService(HttpClient httpClient, IConfiguration configuration, IGooglePlacePhotoService photoService, ILogger<HotelService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _photoService = photoService;
            _logger = logger;
        }

        public async Task<HotelResult> GetHotelsAsync(string city, string? country)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return new HotelResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Otel aramak için şehir bilgisi gereklidir."
                };
            }

            string? apiKey = _configuration["GooglePlaces:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Google Places API anahtarı bulunamadı.");
            }

            string destination = string.IsNullOrWhiteSpace(country) ? city.Trim() : $"{city.Trim()}, {country.Trim()}";

            var requestBody = new
            {
                textQuery = $"{destination} hotels",
                pageSize = 5,
                languageCode = "tr"
            };

            string requestJson = JsonSerializer.Serialize(requestBody);
            using var request = new HttpRequestMessage(HttpMethod.Post, SearchEndpoint);
            request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");
            request.Headers.Add("X-Goog-Api-Key", apiKey);

            request.Headers.Add(
                "X-Goog-FieldMask",
                string.Join(",",
                    "places.id",
                    "places.displayName",
                    "places.formattedAddress",
                    "places.rating",
                    "places.userRatingCount",
                    "places.googleMapsUri",
                    "places.websiteUri",
                    "places.photos"));

            try
            {
                using HttpResponseMessage response = await _httpClient.SendAsync(request);

                string responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Google Places otel isteği başarısız. " + "StatusCode: {StatusCode}, City: {City}, Response: {Response}", response.StatusCode, city, responseContent);

                    return new HotelResult
                    {
                        IsSuccess = false,
                        City = city,
                        Country = country ?? string.Empty,
                        ErrorMessage = $"{city} için otel bilgileri alınamadı."
                    };
                }

                GooglePlacesHotelResponseDto? apiResponse = JsonSerializer.Deserialize<GooglePlacesHotelResponseDto>(
                            responseContent, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                if (apiResponse is null || apiResponse.Places.Count == 0)
                {
                    return new HotelResult
                    {
                        IsSuccess = false,
                        City = city,
                        Country = country ?? string.Empty,
                        ErrorMessage = $"{city} için otel bulunamadı."
                    };
                }

                List<Hotel> hotels = apiResponse.Places.Take(5)
                        .Select(place => new Hotel
                        {
                            PlaceId = place.Id ?? string.Empty,
                            Name = place.DisplayName?.Text ?? "Otel",
                            Address = place.FormattedAddress ?? string.Empty,
                            Rating = place.Rating,
                            UserRatingCount = place.UserRatingCount,
                            GoogleMapsUrl = place.GoogleMapsUri,
                            WebsiteUrl = place.WebsiteUri,
                            PhotoName = place.Photos.FirstOrDefault()?.Name
                        }).ToList();

                //foreach (Hotel hotel in hotels)
                //{
                //    hotel.PhotoUrl = await _photoService.GetPhotoUrlAsync(hotel.PhotoName, 1200);
                //}

                //Hotel? firstHotel = hotels.FirstOrDefault();

                //if (firstHotel is not null && !string.IsNullOrWhiteSpace(firstHotel.PhotoName))
                //{
                //    firstHotel.PhotoUrl = await _photoService.GetPhotoUrlAsync(firstHotel.PhotoName, 1200);
                //}

                var photoTasks = hotels.Select(async hotel =>
                {
                    if (string.IsNullOrWhiteSpace(hotel.PhotoName))
                    {
                        return;
                    }

                    try
                    {
                        hotel.PhotoUrl =
                            await _photoService.GetPhotoUrlAsync(
                                hotel.PhotoName,
                                1200);
                    }
                    catch
                    {
                        hotel.PhotoUrl = null;
                    }
                });

                await Task.WhenAll(photoTasks);

                return new HotelResult
                {
                    IsSuccess = true,
                    City = city,
                    Country = country ?? string.Empty,
                    Hotels = hotels
                };
            }
            catch (JsonException exception)
            {
                _logger.LogError(exception, "Google Places otel JSON cevabı çözümlenemedi. City: {City}", city);

                return new HotelResult
                {
                    IsSuccess = false,
                    City = city,
                    Country = country ?? string.Empty,
                    ErrorMessage = "Otel verileri işlenemedi."
                };
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Otel bilgileri alınırken hata oluştu. City: {City}", city);

                return new HotelResult
                {
                    IsSuccess = false,
                    City = city,
                    Country = country ?? string.Empty,
                    ErrorMessage = "Otel bilgileri alınırken bir hata oluştu."
                };
            }
        }
    }
}