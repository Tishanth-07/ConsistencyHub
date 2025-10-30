using System.Threading.Tasks;

namespace ConsistencyHub.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string bodyHtml);
    }
}
