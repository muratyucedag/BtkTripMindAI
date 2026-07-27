namespace BtkTripMindAI.Models.RouteModels
{
    public class RouteResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int DayCount { get; set; }
        public List<RouteDay> Days { get; set; } = new();
    }
}
