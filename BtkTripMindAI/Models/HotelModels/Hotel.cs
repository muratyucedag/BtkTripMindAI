namespace BtkTripMindAI.Models.HotelModels
{
    public class Hotel
    {
        public string PlaceId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int UserRatingCount { get; set; }
        public string? GoogleMapsUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? PhotoName { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
