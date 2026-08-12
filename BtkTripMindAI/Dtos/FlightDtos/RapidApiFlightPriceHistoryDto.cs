using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.FlightDtos
{
    public class RapidApiFlightPriceHistoryDto
    {
        [JsonPropertyName("summary")]
        public RapidApiFlightPriceSummaryDto? Summary { get; set; }
    }
}
