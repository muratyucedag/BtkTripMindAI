namespace BtkTripMindAI.Models.FlightModels
{
    public class FlightSearchExtractionResult
    {
        public bool Found { get; set; }
        public string? DepartureLocation { get; set; }
        public string? ArrivalLocation { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int AdultCount { get; set; } = 1;
        public int ChildCount { get; set; }
        public string TravelClass { get; set; } = "ECONOMY";
        public bool IsRoundTrip { get; set; }
    }
}
