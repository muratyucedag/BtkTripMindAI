namespace BtkTripMindAI.Dtos.MessageDtos
{
    public class AgentMessageResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Intent { get; set; } = string.Empty;
        public string? ToolName { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}
