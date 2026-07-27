using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.ImageDtos
{
    public class PexelsPhotoSourceDto
    {
        [JsonPropertyName("landscape")]
        public string? Landscape { get; set; }

        [JsonPropertyName("large2x")]
        public string? Large2X { get; set; }
    }
}
