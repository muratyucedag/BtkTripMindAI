using BtkTripMindAI.Models.RouteModels;

namespace BtkTripMindAI.Services.RouteServices
{
    public interface IRouteService
    {
        Task<RouteResult> CreateRouteAsync(string city, string? country, int dayCount);
    }
}
