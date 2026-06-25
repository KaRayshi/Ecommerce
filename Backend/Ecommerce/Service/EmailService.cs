using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Ecommerce.Service
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendOtpEmailAsync(string toEmail, string otpCode)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(
                _config["EmailSettings:SenderName"],
                _config["EmailSettings:SenderEmail"]));

            email.To.Add(MailboxAddress.Parse(toEmail));

            email.Subject = "Your OTP Request Code";

            var builder = new BodyBuilder
            {
                HtmlBody = $"<h2>OTP Request</h2>" +
                           $"<p>Your secure One-Time Password (OTP) is: <strong style='font-size:24px;'>{otpCode}</strong></p>" +
                           $"<p>This code will expire in 10 minutes.</p>" +
                           $"<p>If you did not request this, please ignore this email.</p>"
            };
            email.Body = builder.ToMessageBody();

            // 5. Send it through the SMTP Server
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:SmtpPort"]), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config["EmailSettings:SenderEmail"], _config["EmailSettings:SenderPassword"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}