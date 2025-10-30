using ConsistencyHub.Models;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace ConsistencyHub.Services
{
    public class VerificationService : IVerificationService
    {
        private readonly IMongoCollection<VerificationCode> _codes;

        public VerificationService(IMongoContext context)
        {
            _codes = context.Database.GetCollection<VerificationCode>("verification_codes");
        }

        private string Generate6DigitCode()
        {
            var r = new Random();
            return r.Next(100000, 999999).ToString();
        }

        public async Task<VerificationCode> CreateCodeAsync(string email, CodePurpose purpose, int expirySeconds = 120)
        {
            // create new code and insert
            var code = new VerificationCode
            {
                Id = Guid.NewGuid().ToString(),
                Email = email.ToLower(),
                Code = Generate6DigitCode(),
                Purpose = purpose,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddSeconds(expirySeconds)
            };

            await _codes.InsertOneAsync(code);
            return code;
        }

        public async Task<VerificationCode> GetLatestCodeAsync(string email, CodePurpose purpose)
        {
            return await _codes.Find(c => c.Email == email.ToLower() && c.Purpose == purpose)
                               .SortByDescending(c => c.CreatedAt)
                               .Limit(1)
                               .FirstOrDefaultAsync();
        }

        public async Task<bool> ValidateCodeAsync(string email, string code, CodePurpose purpose)
        {
            var doc = await GetLatestCodeAsync(email, purpose);
            if (doc == null) return false;
            if (doc.Code != code) return false;
            if (DateTime.UtcNow > doc.ExpiresAt) return false;
            // valid — delete the code after successful validation
            await DeleteCodeAsync(doc.Id);
            return true;
        }

        public async Task DeleteCodeAsync(string id)
        {
            await _codes.DeleteOneAsync(c => c.Id == id);
        }
    }
}
