using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.FlightDtos
{
    public class RapidApiFlightLayoverDto
    {
        [JsonPropertyName("airport_name")]
        public string? AirportName { get; set; }

        [JsonPropertyName("airport_code")]
        public string? AirportCode { get; set; }

        [JsonPropertyName("duration")]
        public int? Duration { get; set; }
    }
}
