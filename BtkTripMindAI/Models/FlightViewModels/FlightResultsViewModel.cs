using BtkTripMindAI.Models.FlightModels;

namespace BtkTripMindAI.Models.FlightViewModels
{
    public class FlightResultsViewModel
    {
        public string DepartureCity { get; set; } = string.Empty;
        public string DepartureAirportCode { get; set; } = string.Empty;
        public string ArrivalCity { get; set; } = string.Empty;
        public string ArrivalAirportCode { get; set; } = string.Empty;
        public DateTime DepartureDate { get; set; }
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public string TravelClass { get; set; } = "ECONOMY";
        public decimal? LowestPrice { get; set; }
        public int TotalFlightCount { get; set; }
        public List<FlightOption> TopFlights { get; set; } = new();
        public List<FlightOption> OtherFlights { get; set; } = new();
    }
}
