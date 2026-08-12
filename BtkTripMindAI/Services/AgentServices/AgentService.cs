using BtkTripMindAI.Agent.Email;
using BtkTripMindAI.Agent.Extractors;
using BtkTripMindAI.Agent.Intents;
using BtkTripMindAI.Agent.Responses;
using BtkTripMindAI.Dtos.CalendarDtos;
using BtkTripMindAI.Dtos.EmailDtos;
using BtkTripMindAI.Models.EmailModels;
using BtkTripMindAI.Models.HotelModels;
using BtkTripMindAI.Models.ImageModels;
using BtkTripMindAI.Services.AirportServices;
using BtkTripMindAI.Services.BudgetServices;
using BtkTripMindAI.Services.CalendarServices;
using BtkTripMindAI.Services.EmailServices;
using BtkTripMindAI.Services.FlightServices;
using BtkTripMindAI.Services.HotelServices;
using BtkTripMindAI.Services.ImageServices;
using BtkTripMindAI.Services.RestaurantServices;
using BtkTripMindAI.Services.RouteServices;
using BtkTripMindAI.Services.TravelDetailServices;
using BtkTripMindAI.Services.WeatherServices;

namespace BtkTripMindAI.Services.AgentServices
{
    public class AgentService : IAgentService
    {
        private readonly IIntentDetector _intentDetector;
        private readonly ICityExtractor _cityExtractor;
        private readonly IWeatherService _weatherService;
        private readonly ICityImageService _cityImageService;
        private readonly IRouteService _routeService;
        private readonly IRestaurantService _restaurantService;
        private readonly IHotelService _hotelService;
        private readonly IBudgetService _budgetService;
        private readonly ITravelDetailExtractor _travelDetailExtractor;
        private readonly IEmailExtractor _emailExtractor;
        private readonly IEmailService _emailService;
        private readonly ICalendarEventExtractor _calendarEventExtractor;
        private readonly ICalendarService _calendarService;
        private readonly IFlightSearchExtractor _flightSearchExtractor;
        private readonly IAirportService _airportService;
        private readonly IFlightService _flightService;
        public AgentService(IIntentDetector intentDetector, ICityExtractor cityExtractor, IWeatherService weatherService, ICityImageService cityImageService, IRouteService routeService, IRestaurantService restaurantService, IHotelService hotelService, IBudgetService budgetService, ITravelDetailExtractor travelDetailExtractor, IEmailExtractor emailExtractor, IEmailService emailService, ICalendarEventExtractor calendarEventExtractor, ICalendarService calendarService, IFlightSearchExtractor flightSearchExtractor, IAirportService airportService, IFlightService flightService)
        {
            _intentDetector = intentDetector;
            _cityExtractor = cityExtractor;
            _weatherService = weatherService;
            _cityImageService = cityImageService;
            _routeService = routeService;
            _restaurantService = restaurantService;
            _hotelService = hotelService;
            _budgetService = budgetService;
            _travelDetailExtractor = travelDetailExtractor;
            _emailExtractor = emailExtractor;
            _emailService = emailService;
            _calendarEventExtractor = calendarEventExtractor;
            _calendarService = calendarService;
            _flightSearchExtractor = flightSearchExtractor;
            _airportService = airportService;
            _flightService = flightService;
        }

        public async Task<AgentResult> ProcessMessageAsync(string prompt)
        {
            var intent = await _intentDetector.DetectAsync(prompt);

            //  var cityResult = await _cityExtractor.ExtractAsync(prompt);

            var travelDetails = await _travelDetailExtractor.ExtractAsync(prompt);

            var calendarEvent = await _calendarEventExtractor.ExtractAsync(prompt);

            var city = travelDetails.City;
            var country = travelDetails.Country;

            CityImageResult? cityImageResult = null;

            if (!string.IsNullOrWhiteSpace(city))
            {
                cityImageResult = await _cityImageService.GetCityImageAsync(city, country);
            }

            if (intent == TravelIntent.Weather)
            {
                if (string.IsNullOrWhiteSpace(city))
                {
                    return new AgentResult
                    {
                        IsSuccess = true,
                        Intent = TravelIntent.Weather,
                        City = null,
                        Country = null,
                        Message =
                            "Hava durumunu kontrol edebilirim. " +
                            "Hangi şehir için bilgi almak istiyorsunuz?"
                    };
                }

                var weatherResult =
                    await _weatherService.GetWeatherAsync(
                        city);

                if (!weatherResult.IsSuccess)
                {
                    return new AgentResult
                    {
                        IsSuccess = false,
                        Intent = TravelIntent.Weather,
                        ToolName = nameof(RapidApiWeatherService),
                        City = city,
                        Country = country,
                        Weather = weatherResult,
                        Message = weatherResult.ErrorMessage ?? "Hava durumu bilgisi alınamadı."
                    };
                }

                return new AgentResult
                {
                    IsSuccess = true,
                    Intent = TravelIntent.Weather,
                    ToolName = nameof(RapidApiWeatherService),

                    City = weatherResult.City,
                    Country = weatherResult.Country,

                    Weather = weatherResult,

                    CoverImageUrl = cityImageResult?.IsSuccess == true ? cityImageResult.ImageUrl : null,

                    Message =
                        $"{weatherResult.City} için hava sıcaklığı " +
                        $"{weatherResult.Temperature:0.#}°C. " +
                        $"Hava durumu: {weatherResult.Condition}. " +
                        $"Hissedilen sıcaklık " +
                        $"{weatherResult.FeelsLike:0.#}°C."
                };
            }

            if (intent == TravelIntent.Route)
            {
                if (string.IsNullOrWhiteSpace(city))
                {
                    return new AgentResult
                    {
                        IsSuccess = true,
                        Intent = TravelIntent.Route,
                        City = null,
                        Country = null,
                        Message = "Seyahat rotası oluşturabilirim. Hangi şehir için rota hazırlamamı istersiniz?"
                    };
                }

                //const int dayCount = 3;

                //var routeResult = await _routeService.CreateRouteAsync(city, country, dayCount);

                var routeResult = await _routeService.CreateRouteAsync(city, country, travelDetails.DayCount);

                if (!routeResult.IsSuccess)
                {
                    return new AgentResult
                    {
                        IsSuccess = false,
                        Intent = TravelIntent.Route,
                        ToolName = nameof(RouteService),
                        City = city,
                        Country = country,
                        Route = routeResult,
                        Message = routeResult.ErrorMessage ?? "Seyahat rotası oluşturulamadı."
                    };
                }

                return new AgentResult
                {
                    IsSuccess = true,
                    Intent = TravelIntent.Route,
                    ToolName = nameof(RouteService),
                    City = routeResult.City,
                    Country = routeResult.Country,
                    Route = routeResult,
                    Message = $"{routeResult.City} için " + $"{routeResult.DayCount} günlük seyahat rotası hazırlandı."
                };
            }

            if (intent == TravelIntent.Restaurant)
            {
                if (string.IsNullOrWhiteSpace(city))
                {
                    return new AgentResult
                    {
                        IsSuccess = true,
                        Intent = TravelIntent.Restaurant,
                        Message =
                            "Restoran önerileri hazırlayabilirim. " +
                            "Hangi şehirde restoran arıyorsunuz?"
                    };
                }

                var restaurantResult =
                    await _restaurantService.GetRestaurantsAsync(
                        city,
                        country);

                if (!restaurantResult.IsSuccess)
                {
                    return new AgentResult
                    {
                        IsSuccess = false,
                        Intent = TravelIntent.Restaurant,
                        ToolName = nameof(RestaurantService),
                        City = city,
                        Country = country,
                        Restaurant = restaurantResult,
                        Message =
                            restaurantResult.ErrorMessage
                            ?? "Restoran bilgileri alınamadı."
                    };
                }

                return new AgentResult
                {
                    IsSuccess = true,
                    Intent = TravelIntent.Restaurant,
                    ToolName = nameof(RestaurantService),
                    City = restaurantResult.City,
                    Country = restaurantResult.Country,
                    Restaurant = restaurantResult,
                    Message =
                        $"{restaurantResult.City} için " +
                        $"{restaurantResult.Restaurants.Count} restoran bulundu."
                };
            }

            if (intent == TravelIntent.Hotel)
            {
                if (string.IsNullOrWhiteSpace(city))
                {
                    return new AgentResult
                    {
                        IsSuccess = true,
                        Intent = TravelIntent.Hotel,
                        Message =
                            "Otel önerileri hazırlayabilirim. " + "Hangi şehirde otel arıyorsunuz?"
                    };
                }

                var hotelResult = await _hotelService.GetHotelsAsync(city, country);

                if (!hotelResult.IsSuccess)
                {
                    return new AgentResult
                    {
                        IsSuccess = false,
                        Intent = TravelIntent.Hotel,
                        ToolName = nameof(HotelService),
                        City = city,
                        Country = country,
                        Hotel = hotelResult,
                        Message = hotelResult.ErrorMessage ?? "Otel bilgileri alınamadı."
                    };
                }

                Hotel? firstHotel = hotelResult.Hotels.FirstOrDefault();

                return new AgentResult
                {
                    IsSuccess = true,
                    Intent = TravelIntent.Hotel,
                    ToolName = nameof(HotelService),
                    City = hotelResult.City,
                    Country = hotelResult.Country,
                    Hotel = hotelResult,

                    CoverImageUrl = firstHotel?.PhotoUrl,

                    Message = $"{hotelResult.City} için " + $"{hotelResult.Hotels.Count} otel bulundu."
                };
            }

            if (intent == TravelIntent.Budget)
            {
                if (string.IsNullOrWhiteSpace(city))
                {
                    return new AgentResult
                    {
                        IsSuccess = true,
                        Intent = TravelIntent.Budget,

                        DayCount = travelDetails.DayCount,
                        AdultCount = travelDetails.AdultCount,
                        ChildCount = travelDetails.ChildCount,
                        BudgetLevel = travelDetails.BudgetLevel,

                        Message = "Seyahat bütçesi hesaplayabilirim. " + "Hangi şehir için bütçe oluşturmamı istersiniz?"
                    };
                }

                var budgetResult = await _budgetService.CalculateBudgetAsync(
                        city,
                        country,
                        travelDetails.DayCount,
                        travelDetails.AdultCount,
                        travelDetails.ChildCount);

                if (!budgetResult.IsSuccess)
                {
                    return new AgentResult
                    {
                        IsSuccess = false,
                        Intent = TravelIntent.Budget,
                        ToolName = nameof(BudgetService),

                        City = city,
                        Country = country,

                        DayCount = travelDetails.DayCount,
                        AdultCount = travelDetails.AdultCount,
                        ChildCount = travelDetails.ChildCount,
                        BudgetLevel = travelDetails.BudgetLevel,

                        Budget = budgetResult,

                        Message = budgetResult.Description ?? "Seyahat bütçesi hesaplanamadı."
                    };
                }

                return new AgentResult
                {
                    IsSuccess = true,
                    Intent = TravelIntent.Budget,
                    ToolName = nameof(BudgetService),

                    City = city,
                    Country = country,

                    DayCount = travelDetails.DayCount,
                    AdultCount = travelDetails.AdultCount,
                    ChildCount = travelDetails.ChildCount,
                    BudgetLevel = travelDetails.BudgetLevel,

                    Budget = budgetResult,

                    Message =
                        $"{city} için {travelDetails.DayCount} günlük, " +
                        $"{travelDetails.AdultCount} yetişkin ve " +
                        $"{travelDetails.ChildCount} çocuk için " +
                        $"tahmini seyahat bütçesi hazırlandı. " +
                        $"Toplam: {budgetResult.TotalAmount:0.##} " +
                        $"{budgetResult.Currency}."
                };
            }

            if (intent == TravelIntent.Email)
            {
                var emailResult =
                    await _emailExtractor.ExtractAsync(prompt);

                if (!emailResult.Found ||
                    string.IsNullOrWhiteSpace(emailResult.Email))
                {
                    return new AgentResult
                    {
                        IsSuccess = true,
                        Intent = TravelIntent.Email,
                        ToolName = nameof(OpenAIEmailExtractor),
                        City = city,
                        Country = country,
                        Message =
                            "E-posta gönderebilirim. " +
                            "Lütfen gönderilecek e-posta adresini mesajınızda belirtin."
                    };
                }

                string subject =
                    string.IsNullOrWhiteSpace(city)
                        ? "BtkTripMindAI Seyahat Bilgilendirmesi"
                        : $"{city} Seyahat Bilgilendirmesi";

                string body = CreateEmailBody(
                    city,
                    country,
                    travelDetails.DayCount,
                    travelDetails.AdultCount,
                    travelDetails.ChildCount,
                    travelDetails.BudgetLevel);

                var emailRequest =
                    new SendEmailRequestDto
                    {
                        ToEmail = emailResult.Email,
                        Subject = subject,
                        Body = body,
                        IsHtml = true
                    };

                bool isSent =
                    await _emailService.SendEmailAsync(
                        emailRequest);

                if (!isSent)
                {
                    return new AgentResult
                    {
                        IsSuccess = false,
                        Intent = TravelIntent.Email,
                        ToolName = nameof(EmailService),
                        City = city,
                        Country = country,
                        Message =
                            "E-posta gönderilemedi. " +
                            "Lütfen e-posta ayarlarını ve SMTP bağlantısını kontrol edin."
                    };
                }

                return new AgentResult
                {
                    IsSuccess = true,
                    Intent = TravelIntent.Email,
                    ToolName = nameof(EmailService),
                    City = city,
                    Country = country,
                    Message =
                        $"Seyahat bilgileri {emailResult.Email} " +
                        "adresine başarıyla gönderildi."
                };
            }

            if (intent == TravelIntent.Calendar)
            {
                if (!calendarEvent.Found)
                {
                    return new AgentResult
                    {
                        IsSuccess = false,
                        Intent = TravelIntent.Calendar,
                        Message =
                            "Takvime eklenecek tarih bilgisi bulunamadı."
                    };
                }

                var calendarResult =
                    await _calendarService.CreateEventAsync(
                        new CreateCalendarEventDto
                        {
                            Title = calendarEvent.Title!,
                            Description = calendarEvent.Description,
                            Location = calendarEvent.Location,
                            StartDateTime = calendarEvent.StartDateTime!.Value,
                            EndDateTime = calendarEvent.EndDateTime!.Value,
                            TimeZone = calendarEvent.TimeZone
                        });

                if (!calendarResult.IsSuccess)
                {
                    return new AgentResult
                    {
                        IsSuccess = false,
                        Intent = TravelIntent.Calendar,
                        ToolName = nameof(GoogleCalendarService),
                        Message = calendarResult.Message
                    };
                }

                return new AgentResult
                {
                    IsSuccess = true,
                    Intent = TravelIntent.Calendar,
                    ToolName = nameof(GoogleCalendarService),
                    Message =
                        "Seyahat planınız başarıyla Google Calendar'a eklendi."
                };
            }

            if (intent == TravelIntent.Flight)
            {
                var flightDetails =
                    await _flightSearchExtractor.ExtractAsync(prompt);

                if (!flightDetails.Found ||
                    string.IsNullOrWhiteSpace(flightDetails.DepartureLocation) ||
                    string.IsNullOrWhiteSpace(flightDetails.ArrivalLocation) ||
                    flightDetails.DepartureDate is null)
                {
                    return new AgentResult
                    {
                        IsSuccess = true,
                        Intent = TravelIntent.Flight,
                        ToolName = nameof(OpenAIFlightSearchExtractor),
                        Message =
                            "Uçuş arayabilmem için kalkış noktası, varış noktası " +
                            "ve gidiş tarihini belirtmelisiniz."
                    };
                }

                var departureAirportResult =
                    await _airportService.SearchAirportsAsync(
                        flightDetails.DepartureLocation);

                if (!departureAirportResult.IsSuccess ||
                    departureAirportResult.BestMatch is null)
                {
                    return new AgentResult
                    {
                        IsSuccess = false,
                        Intent = TravelIntent.Flight,
                        ToolName = nameof(AirportService),
                        Message =
                            $"{flightDetails.DepartureLocation} için uygun " +
                            "kalkış havalimanı bulunamadı."
                    };
                }

                var arrivalAirportResult =
                    await _airportService.SearchAirportsAsync(
                        flightDetails.ArrivalLocation);

                if (!arrivalAirportResult.IsSuccess ||
                    arrivalAirportResult.BestMatch is null)
                {
                    return new AgentResult
                    {
                        IsSuccess = false,
                        Intent = TravelIntent.Flight,
                        ToolName = nameof(AirportService),
                        Message =
                            $"{flightDetails.ArrivalLocation} için uygun " +
                            "varış havalimanı bulunamadı."
                    };
                }

                string departureAirportCode =
                    departureAirportResult.BestMatch.IataCode;

                string arrivalAirportCode =
                    arrivalAirportResult.BestMatch.IataCode;

                var flightResult =
                    await _flightService.SearchFlightsAsync(
                        departureAirportCode,
                        arrivalAirportCode,
                        flightDetails.DepartureDate.Value,
                        flightDetails.AdultCount,
                        flightDetails.ChildCount);

                if (!flightResult.IsSuccess)
                {
                    return new AgentResult
                    {
                        IsSuccess = false,
                        Intent = TravelIntent.Flight,
                        ToolName = nameof(FlightService),

                        Flight = flightResult,

                        Message =
             flightResult.ErrorMessage
             ?? "Uçuş bilgileri alınamadı."
                    };
                }

                int totalFlightCount =
                    flightResult.TopFlights.Count +
                    flightResult.OtherFlights.Count;

                return new AgentResult
                {
                    IsSuccess = true,
                    Intent = TravelIntent.Flight,
                    ToolName = nameof(FlightService),

                    Flight = flightResult,

                    Message =
         $"{departureAirportCode} ile {arrivalAirportCode} arasında " +
         $"{flightDetails.DepartureDate.Value:dd.MM.yyyy} tarihi için " +
         $"{totalFlightCount} uçuş seçeneği bulundu."
                };
            }

            return new AgentResult
            {
                IsSuccess = true,
                Intent = intent,
                City = city,
                Country = country,
                Message = CreateResponse(intent, city)
            };
        }

        private static string CreateResponse(
     TravelIntent intent,
     string? city)
        {
            return intent switch
            {
                TravelIntent.Hotel when city is not null =>
                    $"{city} için uygun otelleri araştırabilirim. " +
                    "Tarih aralığını ve kişi sayısını paylaşır mısınız?",

                TravelIntent.Hotel =>
                    "Size uygun otelleri listeleyebilirim. " +
                    "Şehir, tarih aralığı ve kişi sayısını paylaşır mısınız?",

                TravelIntent.Restaurant when city is not null =>
                    $"{city} için restoran önerileri hazırlayabilirim. " +
                    "Tercih ettiğiniz mutfak türü var mı?",

                TravelIntent.Restaurant =>
                    "Restoran önerileri hazırlayabilirim. " +
                    "Hangi şehirde restoran arıyorsunuz?",

                TravelIntent.Route when city is not null =>
                    $"{city} için seyahat rotası oluşturabilirim. " +
                    "Kaç günlük bir plan istiyorsunuz?",

                TravelIntent.Route =>
                    "Seyahat rotası oluşturabilirim. " +
                    "Şehir ve gün sayısını paylaşır mısınız?",

                TravelIntent.Attraction when city is not null =>
                    $"{city} için görülmesi gereken yerleri listeleyebilirim.",

                TravelIntent.Attraction =>
                    "Gezilecek yerleri listeleyebilirim. " +
                    "Hangi şehir için öneri almak istiyorsunuz?",

                _ when city is not null =>
                    $"{city} ile ilgili seyahat talebinizi aldım.",

                _ =>
                    "Talebinizi aldım. Gideceğiniz şehri ve " +
                    "seyahat tercihlerinizi paylaşabilirsiniz."
            };
        }

        private static string? GetToolName(TravelIntent intent)
        {
            return intent switch
            {
                TravelIntent.Weather => "WeatherTool",
                TravelIntent.Hotel => "HotelTool",
                TravelIntent.Restaurant => "RestaurantTool",
                TravelIntent.Route => "RoutePlannerTool",
                TravelIntent.Currency => "CurrencyTool",
                TravelIntent.Flight => "FlightTool",
                TravelIntent.Transportation => "TransportationTool",
                TravelIntent.Attraction => "AttractionTool",
                TravelIntent.CityRecommendation => "CityRecommendationTool",
                _ => null
            };
        }

        private static string CreateEmailBody(
    string? city,
    string? country,
    int dayCount,
    int adultCount,
    int childCount,
    string? budgetLevel)
        {
            string destination =
                string.IsNullOrWhiteSpace(city)
                    ? "Seyahat planınız"
                    : string.IsNullOrWhiteSpace(country)
                        ? city
                        : $"{city}, {country}";

            string budgetLevelText =
                budgetLevel switch
                {
                    "Economy" => "Ekonomik",
                    "Luxury" => "Lüks",
                    _ => "Orta Segment"
                };

            return $$"""
        <!DOCTYPE html>
        <html lang="tr">
        <head>
            <meta charset="UTF-8">
        </head>
        <body style="margin:0;background:#f6f8fb;font-family:Arial,sans-serif;color:#243447;">
            <div style="max-width:640px;margin:30px auto;background:#ffffff;border-radius:18px;overflow:hidden;border:1px solid #e5eaf0;">

                <div style="padding:28px;background:#dff1ff;">
                    <h1 style="margin:0;color:#286887;font-size:26px;">
                        BtkTripMindAI
                    </h1>

                    <p style="margin:8px 0 0;color:#526579;">
                        Yapay Zeka Destekli Seyahat Asistanı
                    </p>
                </div>

                <div style="padding:28px;">
                    <h2 style="margin-top:0;color:#243447;">
                        {{destination}} Seyahat Özeti
                    </h2>

                    <p style="line-height:1.7;color:#526579;">
                        Seyahat talebinize ait temel bilgiler aşağıda yer almaktadır.
                    </p>

                    <table style="width:100%;border-collapse:collapse;margin-top:20px;">
                        <tr>
                            <td style="padding:12px;border-bottom:1px solid #edf0f4;font-weight:bold;">
                                Destinasyon
                            </td>
                            <td style="padding:12px;border-bottom:1px solid #edf0f4;">
                                {{destination}}
                            </td>
                        </tr>

                        <tr>
                            <td style="padding:12px;border-bottom:1px solid #edf0f4;font-weight:bold;">
                                Süre
                            </td>
                            <td style="padding:12px;border-bottom:1px solid #edf0f4;">
                                {{dayCount}} gün
                            </td>
                        </tr>

                        <tr>
                            <td style="padding:12px;border-bottom:1px solid #edf0f4;font-weight:bold;">
                                Yolcular
                            </td>
                            <td style="padding:12px;border-bottom:1px solid #edf0f4;">
                                {{adultCount}} yetişkin, {{childCount}} çocuk
                            </td>
                        </tr>

                        <tr>
                            <td style="padding:12px;border-bottom:1px solid #edf0f4;font-weight:bold;">
                                Bütçe Seviyesi
                            </td>
                            <td style="padding:12px;border-bottom:1px solid #edf0f4;">
                                {{budgetLevelText}}
                            </td>
                        </tr>
                    </table>

                    <div style="margin-top:28px;padding:18px;background:#fff7d6;border-radius:14px;">
                        <p style="margin:0;color:#6e5d13;">
                            Bu e-posta BtkTripMindAI tarafından otomatik olarak hazırlanmıştır.
                        </p>
                    </div>
                </div>
            </div>
        </body>
        </html>
        """;
        }

    }
}
