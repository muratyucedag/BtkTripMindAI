namespace BtkTripMindAI.Models.FlightModels
{
    public class FlightOption
    {
        public DateTime? DepartureDateTime { get; set; }
        public DateTime? ArrivalDateTime { get; set; }
        public int DurationMinutes { get; set; }
        public string DurationText { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = "TRY";
        public int Stops { get; set; }
        public bool IsDirect => Stops == 0;
        public bool SelfTransfer { get; set; }
        public int? CarryOnBagCount { get; set; }
        public int? CheckedBagCount { get; set; }
        public string? AirlineLogoUrl { get; set; }
        public string? BookingToken { get; set; }
        public List<FlightSegment> Segments { get; set; } = new();
    }
}
