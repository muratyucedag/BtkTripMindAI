using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.RestaurantDtos
{
    public class GooglePlaceDisplayNameDto
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("languageCode")]
        public string? LanguageCode { get; set; }
    }
}
