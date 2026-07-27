namespace BtkTripMindAI.Models.WeatherModels
{
    public class WeatherResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public List<WeatherForecast> Forecasts { get; set; } = new();
    }
}
