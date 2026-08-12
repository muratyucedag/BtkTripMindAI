using BtkTripMindAI.Models.AirportModels;

namespace BtkTripMindAI.Services.AirportServices
{
    public interface IAirportService
    {
        Task<AirportSearchResult> SearchAirportsAsync(string query);
    }
}
