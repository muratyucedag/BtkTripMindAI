using System.Text.Json;
using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.FlightDtos
{
    public class RapidApiFlightOptionDto
    {
        [JsonPropertyName("departure_time")]
        public string? DepartureTime { get; set; }

        [JsonPropertyName("arrival_time")]
        public string? ArrivalTime { get; set; }

        [JsonPropertyName("duration")]
        public RapidApiFlightDurationDto? Duration { get; set; }

        [JsonPropertyName("flights")]
        public List<RapidApiFlightSegmentDto> Flights { get; set; } = new();

        [JsonPropertyName("self_transfer")]
        public bool SelfTransfer { get; set; }

        [JsonPropertyName("layovers")]
        public List<RapidApiFlightLayoverDto>? Layovers { get; set; }

        [JsonPropertyName("bags")]
        public RapidApiFlightBagsDto? Bags { get; set; }

        [JsonPropertyName("price")]
        public JsonElement Price { get; set; }

        [JsonPropertyName("stops")]
        public int Stops { get; set; }

        [JsonPropertyName("airline_logo")]
        public string? AirlineLogo { get; set; }

        [JsonPropertyName("booking_token")]
        public string? BookingToken { get; set; }
    }
}
