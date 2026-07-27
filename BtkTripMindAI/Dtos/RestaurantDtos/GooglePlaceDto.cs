using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.RestaurantDtos
{
    public class GooglePlaceDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("displayName")]
        public GooglePlaceDisplayNameDto? DisplayName { get; set; }

        [JsonPropertyName("formattedAddress")]
        public string? FormattedAddress { get; set; }

        [JsonPropertyName("rating")]
        public double Rating { get; set; }

        [JsonPropertyName("userRatingCount")]
        public int UserRatingCount { get; set; }

        [JsonPropertyName("priceLevel")]
        public string? PriceLevel { get; set; }

        [JsonPropertyName("googleMapsUri")]
        public string? GoogleMapsUri { get; set; }

        [JsonPropertyName("photos")]
        public List<GooglePlacePhotoDto> Photos { get; set; } = new();
    }
}
