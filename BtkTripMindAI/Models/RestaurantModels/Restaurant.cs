namespace BtkTripMindAI.Models.RestaurantModels
{
    public class Restaurant
    {
        public string PlaceId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int UserRatingCount { get; set; }
        public string PriceLevel { get; set; } = string.Empty;
        public string? GoogleMapsUrl { get; set; }
        public string? PhotoName { get; set; }
    }
}
