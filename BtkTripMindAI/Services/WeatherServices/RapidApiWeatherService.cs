using BtkTripMindAI.Dtos.WeatherDtos;
using BtkTripMindAI.Models.WeatherModels;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BtkTripMindAI.Services.WeatherServices
{
    public class RapidApiWeatherService : IWeatherService
    {
        private const string RapidApiKey = "630ce9cc86msh271c60cffe62d5ep1b514djsn0fe292593744";

        private const string RapidApiHost = "weatherapi-com.p.rapidapi.com";

        private readonly HttpClient _httpClient;
        private readonly ILogger<RapidApiWeatherService> _logger;

        public RapidApiWeatherService(
            HttpClient httpClient,
            ILogger<RapidApiWeatherService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<WeatherResult> GetWeatherAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return new WeatherResult
                {
                    IsSuccess = false,
                    ErrorMessage =
                        "Hava durumu bilgisi için şehir adı gereklidir."
                };
            }

            try
            {
                var encodedCity = Uri.EscapeDataString(city.Trim());

                var requestUrl =
                    $"https://{RapidApiHost}/forecast.json" +
                    $"?q={encodedCity}&days=3";

                using var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    requestUrl);

                request.Headers.Add(
                    "x-rapidapi-key",
                    RapidApiKey);

                request.Headers.Add(
                    "x-rapidapi-host",
                    RapidApiHost);

                request.Headers.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(
                        "application/json"));

                using var response = await _httpClient.SendAsync(
                    request
                    );

                var responseContent =
                    await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "Weather API başarısız cevap verdi. " +
                        "StatusCode: {StatusCode}, City: {City}, Response: {Response}",
                        response.StatusCode,
                        city,
                        responseContent);

                    return new WeatherResult
                    {
                        IsSuccess = false,
                        ErrorMessage =
                            $"{city} için hava durumu bilgisi alınamadı."
                    };
                }

                var apiResponse =
                    JsonSerializer.Deserialize<RapidApiWeatherResponseDto>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (apiResponse?.Location is null ||
                    apiResponse.Current is null)
                {
                    return new WeatherResult
                    {
                        IsSuccess = false,
                        ErrorMessage =
                            "Hava durumu servisinden geçerli veri alınamadı."
                    };
                }

                var turkishCulture =
                    CultureInfo.GetCultureInfo("tr-TR");

                var forecastList =
                    apiResponse.Forecast?.ForecastDays
                        .Take(3)
                        .Select(forecastDay =>
                        {
                            DateTime.TryParse(
                                forecastDay.Date,
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None,
                                out var forecastDate);

                            return new WeatherForecast
                            {
                                Date = forecastDate,

                                DayName = forecastDate == DateTime.Today
                                    ? "Bugün"
                                    : turkishCulture.DateTimeFormat
                                        .GetAbbreviatedDayName(
                                            forecastDate.DayOfWeek),

                                MinTemperature =
                                    forecastDay.Day?
                                        .MinTemperatureCelsius ?? 0,

                                MaxTemperature =
                                    forecastDay.Day?
                                        .MaxTemperatureCelsius ?? 0,

                                AverageTemperature =
                                    forecastDay.Day?
                                        .AverageTemperatureCelsius ?? 0,

                                Condition =
                                    forecastDay.Day?
                                        .Condition?
                                        .Text ?? string.Empty,

                                IconUrl = BuildIconUrl(
                                    forecastDay.Day?
                                        .Condition?
                                        .Icon),

                                ChanceOfRain =
                                    forecastDay.Day?
                                        .ChanceOfRain ?? 0
                            };
                        })
                        .ToList()
                    ?? new List<WeatherForecast>();

                return new WeatherResult
                {
                    IsSuccess = true,

                    City =
                        apiResponse.Location.Name
                        ?? city,

                    Country =
                        apiResponse.Location.Country
                        ?? string.Empty,

                    Temperature =
                        apiResponse.Current.TemperatureCelsius,

                    FeelsLike =
                        apiResponse.Current.FeelsLikeCelsius,

                    Condition =
                        apiResponse.Current.Condition?
                            .Text ?? string.Empty,

                    IconUrl = BuildIconUrl(
                        apiResponse.Current.Condition?.Icon),

                    Forecasts = forecastList
                };
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "Hava durumu isteği zaman aşımına uğradı. City: {City}",
                    city);

                return new WeatherResult
                {
                    IsSuccess = false,
                    ErrorMessage =
                        "Hava durumu servisi zaman aşımına uğradı."
                };
            }
            catch (JsonException exception)
            {
                _logger.LogError(
                    exception,
                    "Hava durumu JSON cevabı çözümlenemedi. City: {City}",
                    city);

                return new WeatherResult
                {
                    IsSuccess = false,
                    ErrorMessage =
                        "Hava durumu verileri işlenemedi."
                };
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Hava durumu alınırken hata oluştu. City: {City}",
                    city);

                return new WeatherResult
                {
                    IsSuccess = false,
                    ErrorMessage =
                        "Hava durumu bilgisi alınırken bir hata oluştu."
                };
            }
        }

        private static string? BuildIconUrl(string? iconUrl)
        {
            if (string.IsNullOrWhiteSpace(iconUrl))
            {
                return null;
            }

            if (iconUrl.StartsWith("//"))
            {
                return $"https:{iconUrl}";
            }

            return iconUrl;
        }
    }
}
