using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.FlightDtos
{
    public class RapidApiFlightSegmentDto
    {
        [JsonPropertyName("departure_airport")]
        public RapidApiAirportDto? DepartureAirport { get; set; }

        [JsonPropertyName("arrival_airport")]
        public RapidApiAirportDto? ArrivalAirport { get; set; }

        [JsonPropertyName("duration")]
        public RapidApiFlightDurationDto? Duration { get; set; }

        [JsonPropertyName("airline")]
        public string? Airline { get; set; }

        [JsonPropertyName("airline_logo")]
        public string? AirlineLogo { get; set; }

        [JsonPropertyName("flight_number")]
        public string? FlightNumber { get; set; }

        [JsonPropertyName("aircraft")]
        public string? Aircraft { get; set; }

        [JsonPropertyName("seat")]
        public string? Seat { get; set; }

        [JsonPropertyName("legroom")]
        public string? Legroom { get; set; }
    }
}
