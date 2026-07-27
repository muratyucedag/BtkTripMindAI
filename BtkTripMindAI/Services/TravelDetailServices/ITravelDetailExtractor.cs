using BtkTripMindAI.Models.TravelModels;

namespace BtkTripMindAI.Services.TravelDetailServices
{
    public interface ITravelDetailExtractor
    {
        Task<TravelRequestDetails> ExtractAsync(string userMessage);
    }
}
