using BtkTripMindAI.Agent.Models;

namespace BtkTripMindAI.Agent.Extractors
{
    public interface ICityExtractor
    {
        Task<CityExtractionResult> ExtractAsync(string message, CancellationToken cancellationToken = default);
    }
}