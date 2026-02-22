using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MailKit.Security;
using Microsoft.Extensions.Configuration;

namespace Lappy.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage(); // (MimeKit builds the object)
            message.From.Add(new MailboxAddress(_configuration["SMTP:MailerName"], _configuration["SMTP:FromMail"]));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody =  htmlMessage;

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(_configuration["SMTP:SMTPServer"], int.Parse(_configuration["SMTP:SMTPPort"]) , SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_configuration["SMTP:UserID"], _configuration["SMTP:Pass"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
