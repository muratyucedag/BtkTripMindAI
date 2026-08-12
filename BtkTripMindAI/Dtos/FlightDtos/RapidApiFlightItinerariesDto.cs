using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.FlightDtos
{
    public class RapidApiFlightItinerariesDto
    {
        [JsonPropertyName("topFlights")]
        public List<RapidApiFlightOptionDto> TopFlights { get; set; } = new();

        [JsonPropertyName("otherFlights")]
        public List<RapidApiFlightOptionDto> OtherFlights { get; set; } = new();
    }
}
