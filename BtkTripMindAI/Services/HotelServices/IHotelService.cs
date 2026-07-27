using BtkTripMindAI.Models.HotelModels;

namespace BtkTripMindAI.Services.HotelServices
{
    public interface IHotelService
    {
        Task<HotelResult> GetHotelsAsync(string city, string? country);
    }
}
