using BtkTripMindAI.Models.WeatherModels;

namespace BtkTripMindAI.Services.WeatherServices
{
    public interface IWeatherService
    {
        Task<WeatherResult> GetWeatherAsync(string city);
    }
}
