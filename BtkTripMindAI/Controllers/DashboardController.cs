using BtkTripMindAI.Agent.Intents;
using BtkTripMindAI.Dtos.MessageDtos;
using BtkTripMindAI.Services.AgentServices;
using Microsoft.AspNetCore.Mvc;

namespace BtkTripMindAI.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IAgentService _agentService;
        public DashboardController(IAgentService agentService)
        {
            _agentService = agentService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] AgentMessageRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new AgentMessageResponseDto
                {
                    IsSuccess = false,
                    Message = "Lütfen geçerli bir mesaj giriniz.",
                    Intent = TravelIntent.Unknown.ToString()
                });
            }

            var result = await _agentService.ProcessMessageAsync(request.Message);

            var response = new AgentMessageResponseDto
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Intent = result.Intent.ToString(),
                ToolName = result.ToolName,
                City = result.City,
                Country = result.Country
            };

            return result.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}
