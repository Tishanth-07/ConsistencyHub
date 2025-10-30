using ConsistencyHub.Models;
using System.Threading.Tasks;

namespace ConsistencyHub.Services
{
    public interface IVerificationService
    {
        Task<VerificationCode> CreateCodeAsync(string email, CodePurpose purpose, int expirySeconds = 120);
        Task<VerificationCode> GetLatestCodeAsync(string email, CodePurpose purpose);
        Task<bool> ValidateCodeAsync(string email, string code, CodePurpose purpose);
        Task DeleteCodeAsync(string id);
    }
}
