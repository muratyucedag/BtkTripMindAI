using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.FlightDtos
{
    public class RapidApiFlightPriceSummaryDto
    {
        [JsonPropertyName("current")]
        public decimal Current { get; set; }
    }
}
