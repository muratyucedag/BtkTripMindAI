using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.FlightDtos
{
    public class RapidApiFlightDurationDto
    {
        [JsonPropertyName("raw")]
        public int Raw { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
