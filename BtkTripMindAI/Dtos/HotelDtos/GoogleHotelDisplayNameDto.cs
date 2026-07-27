using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.HotelDtos
{
    public class GoogleHotelDisplayNameDto
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("languageCode")]
        public string? LanguageCode { get; set; }
    }
}
