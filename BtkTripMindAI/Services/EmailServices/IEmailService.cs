using BtkTripMindAI.Dtos.EmailDtos;

namespace BtkTripMindAI.Services.EmailServices
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(SendEmailRequestDto request);
    }
}
