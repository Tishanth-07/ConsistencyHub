using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ConsistencyHub.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }

        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public required string Email { get; set; }

        // hashed password (BCrypt)
        public required string PasswordHash { get; set; }

        // Whether email verified
        public bool EmailVerified { get; set; } = false;

        // When user registered
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Optional: roles or claims
        public List<string> Roles { get; set; } = ["User"];

        // For users created via Google, this may be true and PasswordHash can be null
        public bool IsGoogleUser { get; set; } = false;
    }
}
