using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ConsistencyHub.Models
{
    public enum CodePurpose
    {
        EmailVerification,
        PasswordReset
    }

    public class VerificationCode
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string Email { get; set; }
        public required string Code { get; set; }
        public CodePurpose Purpose { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
