using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.RestaurantDtos
{
    public class GooglePlacesTextSearchResponseDto
    {
        [JsonPropertyName("places")]
        public List<GooglePlaceDto> Places { get; set; } = new();
    }
}
