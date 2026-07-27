using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.RouteDtos
{
    public class OpenAIRouteDayDto
    {
        [JsonPropertyName("dayNumber")]
        public int DayNumber { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
