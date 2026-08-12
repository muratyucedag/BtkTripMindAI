using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.FlightDtos
{
    public class RapidApiFlightBagsDto
    {
        [JsonPropertyName("carry_on")]
        public int? CarryOn { get; set; }

        [JsonPropertyName("checked")]
        public int? Checked { get; set; }
    }
}
