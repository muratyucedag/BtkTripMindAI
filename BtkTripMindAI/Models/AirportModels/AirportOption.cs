namespace BtkTripMindAI.Models.AirportModels
{
    public class AirportOption
    {
        public string IataCode { get; set; } = string.Empty;
        public string AirportName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? CountryInfo { get; set; }
        public string? Distance { get; set; }
    }
}
