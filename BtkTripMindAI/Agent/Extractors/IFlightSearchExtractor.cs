using BtkTripMindAI.Models.FlightModels;

namespace BtkTripMindAI.Agent.Extractors
{
    public interface IFlightSearchExtractor
    {
        Task<FlightSearchExtractionResult> ExtractAsync(string message);
    }
}
