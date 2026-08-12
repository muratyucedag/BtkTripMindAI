namespace BtkTripMindAI.Models.FlightModels
{
    public class FlightSegment
    {
        public string DepartureAirportName { get; set; } = string.Empty;
        public string DepartureAirportCode { get; set; } = string.Empty;
        public string ArrivalAirportName { get; set; } = string.Empty;
        public string ArrivalAirportCode { get; set; } = string.Empty;
        public DateTime? DepartureDateTime { get; set; }
        public DateTime? ArrivalDateTime { get; set; }
        public string Airline { get; set; } = string.Empty;
        public string? AirlineLogoUrl { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string? Aircraft { get; set; }
        public int DurationMinutes { get; set; }
        public string DurationText { get; set; } = string.Empty;
    }
}
