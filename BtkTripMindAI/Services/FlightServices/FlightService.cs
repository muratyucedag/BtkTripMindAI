using BtkTripMindAI.Dtos.FlightDtos;
using BtkTripMindAI.Models.FlightModels;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BtkTripMindAI.Services.FlightServices
{
    public class FlightService : IFlightService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FlightService> _logger;
        public FlightService(HttpClient httpClient, IConfiguration configuration, ILogger<FlightService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<FlightResult> SearchFlightsAsync(string departureAirportCode, string arrivalAirportCode, DateTime departureDate, int adultCount, int childCount)
        {
            if (string.IsNullOrWhiteSpace(departureAirportCode))
            {
                return CreateFailureResult("Kalkış havalimanı kodu gereklidir.");
            }

            if (string.IsNullOrWhiteSpace(arrivalAirportCode))
            {
                return CreateFailureResult("Varış havalimanı kodu gereklidir.");
            }

            if (departureDate.Date < DateTime.Today)
            {
                return CreateFailureResult("Geçmiş bir tarih için uçuş aranamaz.");
            }

            if (adultCount <= 0)
            {
                adultCount = 1;
            }

            if (childCount < 0)
            {
                childCount = 0;
            }

            string? apiKey = _configuration["RapidApiFlight:ApiKey"];

            string host = _configuration["RapidApiFlight:Host"] ?? "google-flights2.p.rapidapi.com";

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return CreateFailureResult("RapidAPI Flight API anahtarı bulunamadı.");
            }

            string departureCode = departureAirportCode.Trim().ToUpperInvariant();
            string arrivalCode = arrivalAirportCode.Trim().ToUpperInvariant();
            string formattedDate = departureDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            string requestUrl =
                $"https://{host}/api/v1/searchFlights" +
                $"?departure_id={Uri.EscapeDataString(departureCode)}" +
                $"&arrival_id={Uri.EscapeDataString(arrivalCode)}" +
                $"&outbound_date={formattedDate}" +
                $"&travel_class=ECONOMY" +
                $"&adults={adultCount}" +
                $"&show_hidden=1" +
                $"&currency=TRY" +
                $"&language_code=en-US" +
                $"&country_code=US" +
                $"&search_type=best";

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Add("x-rapidapi-key", apiKey);
                request.Headers.Add("x-rapidapi-host", host);

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using HttpResponseMessage response = await _httpClient.SendAsync(request);

                string responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Flight API başarısız cevap verdi. " + "StatusCode: {StatusCode}, Response: {Response}", response.StatusCode, responseContent);

                    return CreateFailureResult("Uçuş bilgileri alınamadı.");
                }

                var apiResponse = JsonSerializer.Deserialize<RapidApiFlightResponseDto>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiResponse is null || !apiResponse.Status || apiResponse.Data?.Itineraries is null)
                {
                    return CreateFailureResult("Uçuş servisinden geçerli veri alınamadı.");
                }

                List<FlightOption> topFlights = apiResponse.Data.Itineraries.TopFlights.Select(MapFlightOption).Where(x => x.Segments.Count > 0).ToList();

                List<FlightOption> otherFlights = apiResponse.Data.Itineraries.OtherFlights.Select(MapFlightOption).Where(x => x.Segments.Count > 0).ToList();

                if (topFlights.Count == 0 && otherFlights.Count == 0)
                {
                    return CreateFailureResult($"{departureCode} ile {arrivalCode} arasında " + "uygun uçuş bulunamadı.");
                }

                return new FlightResult
                {
                    IsSuccess = true,
                    DepartureAirportCode = departureCode,
                    ArrivalAirportCode = arrivalCode,
                    DepartureDate = departureDate.Date,
                    Currency = "TRY",
                    CurrentPrice = apiResponse.Data.PriceHistory?.Summary?.Current,
                    TopFlights = topFlights,
                    OtherFlights = otherFlights
                };
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Uçuş arama isteği zaman aşımına uğradı. " + "Departure: {Departure}, Arrival: {Arrival}", departureCode,
                    arrivalCode);

                return CreateFailureResult("Uçuş arama servisi zaman aşımına uğradı.");
            }
            catch (JsonException exception)
            {
                _logger.LogError(exception, "Uçuş API JSON cevabı ayrıştırılamadı.");

                return CreateFailureResult("Uçuş verileri işlenemedi.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Uçuş bilgileri alınırken hata oluştu.");
                return CreateFailureResult("Uçuş bilgileri alınırken bir hata oluştu.");
            }
        }

        private static FlightOption MapFlightOption(RapidApiFlightOptionDto option)
        {
            return new FlightOption
            {
                DepartureDateTime = ParseDateTime(option.DepartureTime),
                ArrivalDateTime = ParseDateTime(option.ArrivalTime),
                DurationMinutes = option.Duration?.Raw ?? 0,
                DurationText = option.Duration?.Text ?? string.Empty,
                Price = GetPrice(option.Price),
                Currency = "TRY",
                Stops = option.Stops,
                SelfTransfer = option.SelfTransfer,
                CarryOnBagCount = option.Bags?.CarryOn,
                CheckedBagCount = option.Bags?.Checked,
                AirlineLogoUrl = option.AirlineLogo,
                BookingToken = option.BookingToken,
                Segments = option.Flights.Select(MapFlightSegment).ToList()
            };
        }
        private static FlightSegment MapFlightSegment(RapidApiFlightSegmentDto segment)
        {
            return new FlightSegment
            {
                DepartureAirportName = segment.DepartureAirport?.AirportName ?? string.Empty,
                DepartureAirportCode = segment.DepartureAirport?.AirportCode ?? string.Empty,
                ArrivalAirportName = segment.ArrivalAirport?.AirportName ?? string.Empty,
                ArrivalAirportCode = segment.ArrivalAirport?.AirportCode ?? string.Empty,
                DepartureDateTime = ParseDateTime(segment.DepartureAirport?.Time),
                ArrivalDateTime = ParseDateTime(segment.ArrivalAirport?.Time),
                Airline = segment.Airline ?? string.Empty,
                AirlineLogoUrl = segment.AirlineLogo,
                FlightNumber = segment.FlightNumber ?? string.Empty,
                Aircraft = segment.Aircraft,
                DurationMinutes = segment.Duration?.Raw ?? 0,
                DurationText = segment.Duration?.Text ?? string.Empty
            };
        }
        private static DateTime? ParseDateTime(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            string[] formats =
            {
                "dd-MM-yyyy hh:mm tt",
                "d-M-yyyy hh:mm tt",
                "yyyy-M-d HH:mm",
                "yyyy-MM-dd HH:mm"
            };

            if (DateTime.TryParseExact(
                    value.Trim(),
                    formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime parsedDate))
            {
                return parsedDate;
            }

            if (DateTime.TryParse(
                    value,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out parsedDate))
            {
                return parsedDate;
            }

            return null;
        }

        private static FlightResult CreateFailureResult(string errorMessage)
        {
            return new FlightResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                Currency = "TRY"
            };
        }

        private static decimal GetPrice(JsonElement priceElement)
        {
            if (priceElement.ValueKind == JsonValueKind.Number)
            {
                if (priceElement.TryGetDecimal(out decimal decimalValue))
                {
                    return decimalValue;
                }

                if (priceElement.TryGetDouble(out double doubleValue))
                {
                    if (doubleValue >= 0 &&
                        doubleValue <= (double)decimal.MaxValue)
                    {
                        return Convert.ToDecimal(doubleValue);
                    }
                }
            }

            if (priceElement.ValueKind == JsonValueKind.String)
            {
                string? value = priceElement.GetString();

                if (decimal.TryParse(
                        value,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out decimal parsedValue))
                {
                    return parsedValue;
                }
            }

            return 0;
        }
    }
}