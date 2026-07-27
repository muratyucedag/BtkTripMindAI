using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.ImageDtos
{
    public class GooglePlacePhotoResponseDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("photoUri")]
        public string? PhotoUri { get; set; }
    }
}
