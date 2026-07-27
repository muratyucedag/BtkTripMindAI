namespace BtkTripMindAI.Models.RestaurantModels
{
    public class RestaurantResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public List<Restaurant> Restaurants { get; set; } = new();
    }
}
