using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.FlightDtos
{
    public class RapidApiFlightDataDto
    {
        [JsonPropertyName("itineraries")]
        public RapidApiFlightItinerariesDto? Itineraries { get; set; }

        [JsonPropertyName("priceHistory")]
        public RapidApiFlightPriceHistoryDto? PriceHistory { get; set; }
    }
}
