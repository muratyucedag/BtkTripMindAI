namespace BtkTripMindAI.Agent.Intents
{
    public interface IIntentDetector
    {
        Task<TravelIntent> DetectAsync(string message, CancellationToken cancellationToken = default);
    }
}