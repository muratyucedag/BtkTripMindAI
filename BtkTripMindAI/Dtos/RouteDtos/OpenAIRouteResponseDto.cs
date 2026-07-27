using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.RouteDtos
{
    public class OpenAIRouteResponseDto
    {
        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("dayCount")]
        public int DayCount { get; set; }

        [JsonPropertyName("days")]
        public List<OpenAIRouteDayDto> Days { get; set; } = new();
    }
}
