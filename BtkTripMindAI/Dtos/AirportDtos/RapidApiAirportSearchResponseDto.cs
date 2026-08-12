using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.AirportDtos
{
    public class RapidApiAirportSearchResponseDto
    {
        [JsonPropertyName("status")]
        public bool Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("data")]
        public List<RapidApiAirportSearchItemDto> Data { get; set; } = new();
    }
}
