namespace BtkTripMindAI.Models.HotelModels
{
    public class HotelResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public List<Hotel> Hotels { get; set; } = new();
    }
}