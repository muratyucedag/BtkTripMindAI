namespace BtkTripMindAI.Models.AirportModels
{
    public class AirportSearchResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string Query { get; set; } = string.Empty;
        public List<AirportOption> Airports { get; set; } = new();
        public AirportOption? BestMatch { get; set; }
    }
}
