using BtkTripMindAI.Models.ImageModels;

namespace BtkTripMindAI.Services.ImageServices
{
    public interface ICityImageService
    {
        Task<CityImageResult> GetCityImageAsync(string city, string? country = null);
    }
}
