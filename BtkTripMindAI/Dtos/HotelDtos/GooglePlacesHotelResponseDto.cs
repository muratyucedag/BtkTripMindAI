using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.HotelDtos
{
    public class GooglePlacesHotelResponseDto
    {
        [JsonPropertyName("places")]
        public List<GoogleHotelDto> Places { get; set; } = new();
    }
}
