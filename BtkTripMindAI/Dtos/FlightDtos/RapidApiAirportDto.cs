using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.FlightDtos
{
    public class RapidApiAirportDto
    {
        [JsonPropertyName("airport_name")]
        public string? AirportName { get; set; }

        [JsonPropertyName("airport_code")]
        public string? AirportCode { get; set; }

        [JsonPropertyName("time")]
        public string? Time { get; set; }
    }
}
