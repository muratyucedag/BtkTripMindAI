using BtkTripMindAI.Agent.Responses;

namespace BtkTripMindAI.Services.AgentServices
{
    public interface IAgentService
    {
        Task<AgentResult> ProcessMessageAsync(string message);
    }
}
