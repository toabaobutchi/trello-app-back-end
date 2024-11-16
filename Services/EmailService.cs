using backend_apis.Utils;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace backend_apis.Services
{
    public class EmailService
    {
        private readonly IEmailSender _emailSender;
        public EmailService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        public Task SendEmailAsync(Email email)
        {
            return Task.Run(async () =>
            {
                await _emailSender.SendEmailAsync(
                    email.ToEmail,
                    email.Subject,
                    email.Body
                );
            });
        }
    }
}