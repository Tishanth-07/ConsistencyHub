using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ConsistencyHub.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;
        private readonly string _fromEmail;
        private readonly string _fromName;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public SmtpEmailService(IConfiguration config)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        {
            _config = config;
#pragma warning disable CS8601 // Possible null reference assignment.
            _host = _config.GetSection("Smtp:Host").Value;
#pragma warning restore CS8601 // Possible null reference assignment.
            _port = int.Parse(_config.GetSection("Smtp:Port").Value ?? "587");
#pragma warning disable CS8601 // Possible null reference assignment.
            _username = _config.GetSection("Smtp:Username").Value;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
            _password = _config.GetSection("Smtp:Password").Value;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
            _fromEmail = _config.GetSection("Smtp:FromEmail").Value;
#pragma warning restore CS8601 // Possible null reference assignment.
            _fromName = _config.GetSection("Smtp:FromName").Value ?? "NoReply";
        }

        public async Task SendEmailAsync(string toEmail, string subject, string bodyHtml)
        {
            using var client = new SmtpClient(_host, _port)
            {
                Credentials = new NetworkCredential(_username, _password),
                EnableSsl = true
            };

            var msg = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName)
            };
            msg.To.Add(new MailAddress(toEmail));
            msg.Subject = subject;
            msg.Body = bodyHtml;
            msg.IsBodyHtml = true;
            await client.SendMailAsync(msg);
        }
    }
}
