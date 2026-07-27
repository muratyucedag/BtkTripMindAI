using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.RestaurantDtos
{
    public class GooglePlacePhotoDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
