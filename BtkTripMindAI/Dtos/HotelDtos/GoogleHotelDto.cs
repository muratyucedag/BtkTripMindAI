using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.HotelDtos
{
    public class GoogleHotelDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("displayName")]
        public GoogleHotelDisplayNameDto? DisplayName { get; set; }

        [JsonPropertyName("formattedAddress")]
        public string? FormattedAddress { get; set; }

        [JsonPropertyName("rating")]
        public double Rating { get; set; }

        [JsonPropertyName("userRatingCount")]
        public int UserRatingCount { get; set; }

        [JsonPropertyName("googleMapsUri")]
        public string? GoogleMapsUri { get; set; }

        [JsonPropertyName("websiteUri")]
        public string? WebsiteUri { get; set; }

        [JsonPropertyName("photos")]
        public List<GoogleHotelPhotoDto> Photos { get; set; } = new();
    }
}
