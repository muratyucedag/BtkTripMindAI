namespace BtkTripMindAI.Models.WeatherModels
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }
        public string DayName { get; set; } = string.Empty;
        public double MinTemperature { get; set; }
        public double MaxTemperature { get; set; }
        public double AverageTemperature { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public int ChanceOfRain { get; set; }
    }
}