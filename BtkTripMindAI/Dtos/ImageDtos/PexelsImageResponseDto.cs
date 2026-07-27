using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.ImageDtos
{
    public class PexelsImageResponseDto
    {
        [JsonPropertyName("photos")]
        public List<PexelsPhotoDto> Photos { get; set; } = new();
    }
}
