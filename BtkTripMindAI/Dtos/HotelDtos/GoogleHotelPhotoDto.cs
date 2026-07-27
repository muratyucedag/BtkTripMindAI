using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.HotelDtos
{
    public class GoogleHotelPhotoDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
