using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.ImageDtos
{
    public class PexelsPhotoDto
    {
        [JsonPropertyName("photographer")]
        public string? Photographer { get; set; }

        [JsonPropertyName("src")]
        public PexelsPhotoSourceDto? Source { get; set; }
    }
}
