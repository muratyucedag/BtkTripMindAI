using BtkTripMindAI.Models.FlightModels;

namespace BtkTripMindAI.Services.FlightServices
{
    public interface IFlightService
    {
        Task<FlightResult> SearchFlightsAsync(string departureAirportCode, string arrivalAirportCode, DateTime departureDate, int adultCount, int childCount);
    }
}
