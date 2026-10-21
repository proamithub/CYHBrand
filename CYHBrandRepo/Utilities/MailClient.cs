using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace CYHBrandRepo.Utilities
{
    public class MailClient : IMailClient
    {
        private readonly IConfiguration _config;
        private readonly ILogger<MailClient> _logger;

        public MailClient(IConfiguration config, ILogger<MailClient> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<bool> SendMailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpHost    = _config["Email:SmtpHost"] ?? "localhost";
                var smtpPort    = int.Parse(_config["Email:SmtpPort"] ?? "587");
                var fromAddress = _config["Email:FromAddress"] ?? "noreply@cyhbrand.com";
                var fromName    = _config["Email:FromName"] ?? "CYHBrand";
                var username    = _config["Email:Username"] ?? "";
                var password    = _config["Email:Password"] ?? "";
                var useSsl      = bool.Parse(_config["Email:UseSsl"] ?? "false");

                using var client = new SmtpClient(smtpHost, smtpPort) { EnableSsl = useSsl };
                if (!string.IsNullOrEmpty(username))
                    client.Credentials = new NetworkCredential(username, password);

                var message = new MailMessage
                {
                    From       = new MailAddress(fromAddress, fromName),
                    Subject    = subject,
                    Body       = body,
                    IsBodyHtml = true
                };
                message.To.Add(toEmail);

                await client.SendMailAsync(message);
                _logger.LogInformation("Mail sent to {Email} | Subject: {Subject}", toEmail, subject);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send mail to {Email}", toEmail);
                return false;
            }
        }
    }
}
