using BtkTripMindAI.Agent.Intents;
using BtkTripMindAI.Dtos.EmailDtos;
using BtkTripMindAI.Dtos.MessageDtos;
using BtkTripMindAI.Models.FlightViewModels;
using BtkTripMindAI.Services.AgentServices;
using BtkTripMindAI.Services.EmailServices;
using Microsoft.AspNetCore.Mvc;

namespace BtkTripMindAI.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IAgentService _agentService;
        private readonly IEmailService _emailService;
        public DashboardController(IAgentService agentService, IEmailService emailService)
        {
            _agentService = agentService;
            _emailService = emailService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FlightResults()
        {
            var model = new FlightResultsViewModel();
            return View(model);
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
                Country = result.Country,
                Weather = result.Weather,
                CoverImageUrl = result.CoverImageUrl,
                Route = result.Route,
                Restaurant = result.Restaurant,
                Hotel = result.Hotel,
                DayCount = result.DayCount,
                AdultCount = result.AdultCount,
                ChildCount = result.ChildCount,
                BudgetLevel = result.BudgetLevel,
                Budget = result.Budget,
                EmailAddress = result.EmailAddress,
                EmailSent = result.EmailSent,
                Flight = result.Flight

            };

            return result.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpGet]
        public async Task<IActionResult> TestEmail()
        {
            var emailRequest = new SendEmailRequestDto
            {
                ToEmail = "testprojekurs1@gmail.com",
                Subject = "BtkTripMindAI MailKit Testi",
                Body =
                """
            <div style="font-family:Arial;padding:24px">
                <h2 style="color:#286887">
                    BtkTripMindAI
                </h2>

                <p>
                    MailKit e-posta servisi başarıyla çalışıyor.
                </p>

                <p>
                    Bu mesaj ASP.NET Core 9 uygulamasından gönderildi.
                </p>
            </div>
            """,
                IsHtml = true
            };

            bool isSent = await _emailService.SendEmailAsync(emailRequest);

            if (!isSent)
            {
                return BadRequest("E-posta gönderilemedi. Log kayıtlarını kontrol edin.");
            }

            return Ok("E-posta başarıyla gönderildi.");
        }
    }
}
