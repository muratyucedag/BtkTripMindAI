using BtkTripMindAI.Agent.Extractors;
using BtkTripMindAI.Agent.Intents;
using BtkTripMindAI.Agent.Responses;
using BtkTripMindAI.Models.HotelModels;
using BtkTripMindAI.Models.ImageModels;
using BtkTripMindAI.Services.BudgetServices;
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
        public AgentService(IIntentDetector intentDetector, ICityExtractor cityExtractor, IWeatherService weatherService, ICityImageService cityImageService, IRouteService routeService, IRestaurantService restaurantService, IHotelService hotelService, IBudgetService budgetService, ITravelDetailExtractor travelDetailExtractor)
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
        }

        public async Task<AgentResult> ProcessMessageAsync(string prompt)
        {
            var intent = await _intentDetector.DetectAsync(prompt);

            //  var cityResult = await _cityExtractor.ExtractAsync(prompt);

            var travelDetails = await _travelDetailExtractor.ExtractAsync(prompt);

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

    }
}
