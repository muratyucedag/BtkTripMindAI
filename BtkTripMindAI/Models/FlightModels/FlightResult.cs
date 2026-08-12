namespace BtkTripMindAI.Models.FlightModels
{
    public class FlightResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string DepartureAirportCode { get; set; } = string.Empty;
        public string ArrivalAirportCode { get; set; } = string.Empty;
        public DateTime DepartureDate { get; set; }
        public string Currency { get; set; } = "TRY";
        public decimal? CurrentPrice { get; set; }
        public List<FlightOption> TopFlights { get; set; } = new();
        public List<FlightOption> OtherFlights { get; set; } = new();
    }
}
