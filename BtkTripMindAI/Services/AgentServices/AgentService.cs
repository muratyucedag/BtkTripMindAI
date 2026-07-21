using BtkTripMindAI.Agent.Extractors;
using BtkTripMindAI.Agent.Intents;
using BtkTripMindAI.Agent.Responses;

namespace BtkTripMindAI.Services.AgentServices
{
    public class AgentService : IAgentService
    {
        private readonly IIntentDetector _intentDetector;
        private readonly ICityExtractor _cityExtractor;
        public AgentService(IIntentDetector intentDetector, ICityExtractor cityExtractor)
        {
            _intentDetector = intentDetector;
            _cityExtractor = cityExtractor;
        }

        public async Task<AgentResult> ProcessMessageAsync(string message)
        {

            if (string.IsNullOrWhiteSpace(message))
            {
                return new AgentResult
                {
                    IsSuccess = false,
                    Intent = TravelIntent.Unknown,
                    Message = "Lütfen geçerli bir mesaj giriniz."
                };
            }

            TravelIntent intent = await _intentDetector.DetectAsync(message);

            var cityResult = await _cityExtractor.ExtractAsync(message);

            string responseMessage = CreateResponse(intent, cityResult.City);

            return new AgentResult
            {
                IsSuccess = true,
                Intent = intent,
                Message = responseMessage,
                ToolName = GetToolName(intent),
                City = cityResult.City,
                Country = cityResult.Country
            };
        }

        private static string CreateResponse(TravelIntent intent, string? city)
        {
            string cityText = string.IsNullOrWhiteSpace(city) ? "hangi şehir" : city;

            return intent switch
            {
                TravelIntent.Weather when city is not null =>
                    $"{city} için hava durumu bilgilerini kontrol etmeye hazırım.",

                TravelIntent.Weather =>
                    "Hava durumunu kontrol edebilirim. Hangi şehir için bilgi almak istiyorsunuz?",

                TravelIntent.Hotel when city is not null =>
                    $"{city} için uygun otelleri araştırabilirim. Tarih aralığını ve kişi sayısını paylaşır mısınız?",

                TravelIntent.Hotel =>
                    "Size uygun otelleri listeleyebilirim. Şehir, tarih aralığı ve kişi sayısını paylaşır mısınız?",

                TravelIntent.Restaurant when city is not null =>
                    $"{city} için restoran önerileri hazırlayabilirim. Tercih ettiğiniz mutfak türü var mı?",

                TravelIntent.Restaurant =>
                    "Restoran önerileri hazırlayabilirim. Hangi şehirde restoran arıyorsunuz?",

                TravelIntent.Route when city is not null =>
                    $"{city} için seyahat rotası oluşturabilirim. Kaç günlük bir plan istiyorsunuz?",

                TravelIntent.Route =>
                    "Seyahat rotası oluşturabilirim. Şehir ve gün sayısını paylaşır mısınız?",

                TravelIntent.Attraction when city is not null =>
                    $"{city} için görülmesi gereken yerleri listeleyebilirim.",

                TravelIntent.Attraction =>
                    "Gezilecek yerleri listeleyebilirim. Hangi şehir için öneri almak istiyorsunuz?",

                _ when city is not null =>
                    $"{cityText} ile ilgili seyahat talebinizi aldım.",

                _ =>
                    "Talebinizi aldım. Gideceğiniz şehri ve seyahat tercihlerinizi paylaşabilirsiniz."
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
