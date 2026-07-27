using BtkTripMindAI.Models.RestaurantModels;

namespace BtkTripMindAI.Services.RestaurantServices
{
    public interface IRestaurantService
    {
        Task<RestaurantResult> GetRestaurantsAsync(string city, string? country);
    }
}
