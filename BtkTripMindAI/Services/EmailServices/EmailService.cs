using BtkTripMindAI.Dtos.EmailDtos;
using BtkTripMindAI.Models.EmailModels;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BtkTripMindAI.Services.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        public EmailService(IOptions<EmailSettings> emailOptions)
        {
            _emailSettings = emailOptions.Value;
        }
        public async Task<bool> SendEmailAsync(SendEmailRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.ToEmail))
            {
                return false;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            message.To.Add(MailboxAddress.Parse(request.ToEmail));
            message.Subject = request.Subject;

            var bodyBuilder = new BodyBuilder();

            if (request.IsHtml)
            {
                bodyBuilder.HtmlBody = request.Body;
            }
            else
            {
                bodyBuilder.TextBody = request.Body;
            }

            message.Body = bodyBuilder.ToMessageBody();

            try
            {
                using var smtpClient = new SmtpClient();

                await smtpClient.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(_emailSettings.UserName, _emailSettings.Password);
                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
