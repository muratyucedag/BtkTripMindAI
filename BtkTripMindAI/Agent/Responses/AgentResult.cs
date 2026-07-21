using BtkTripMindAI.Agent.Intents;

namespace BtkTripMindAI.Agent.Responses
{
    public class AgentResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public TravelIntent Intent { get; set; }
        public string? ToolName { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}