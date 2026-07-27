using System.Text.Json.Serialization;

namespace BtkTripMindAI.Dtos.WeatherDtos
{
    public class RapidApiWeatherResponseDto
    {

        [JsonPropertyName("location")]
        public RapidApiWeatherLocation? Location { get; set; }

        [JsonPropertyName("current")]
        public RapidApiCurrentWeather? Current { get; set; }

        [JsonPropertyName("forecast")]
        public RapidApiForecastContainer? Forecast { get; set; }
    }

    public class RapidApiWeatherLocation
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }
    }

    public class RapidApiCurrentWeather
    {
        [JsonPropertyName("temp_c")]
        public double TemperatureCelsius { get; set; }

        [JsonPropertyName("feelslike_c")]
        public double FeelsLikeCelsius { get; set; }

        [JsonPropertyName("condition")]
        public RapidApiWeatherCondition? Condition { get; set; }
    }

    public class RapidApiForecastContainer
    {
        [JsonPropertyName("forecastday")]
        public List<RapidApiForecastDay> ForecastDays { get; set; } = new();
    }

    public class RapidApiForecastDay
    {
        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("day")]
        public RapidApiDayDetail? Day { get; set; }
    }

    public class RapidApiDayDetail
    {
        [JsonPropertyName("maxtemp_c")]
        public double MaxTemperatureCelsius { get; set; }

        [JsonPropertyName("mintemp_c")]
        public double MinTemperatureCelsius { get; set; }

        [JsonPropertyName("avgtemp_c")]
        public double AverageTemperatureCelsius { get; set; }

        [JsonPropertyName("daily_chance_of_rain")]
        public int ChanceOfRain { get; set; }

        [JsonPropertyName("condition")]
        public RapidApiWeatherCondition? Condition { get; set; }
    }

    public class RapidApiWeatherCondition
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }
    }
}
