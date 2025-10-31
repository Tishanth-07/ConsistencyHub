using ConsistencyHub.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace ConsistencyHub.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IMongoContext context)
        {
            _users = context.Database.GetCollection<User>("users");
        }

        public async Task EnsureIndexesAsync()
        {
            var indexBuilder = Builders<User>.IndexKeys;
            var indexModel = new CreateIndexModel<User>(indexBuilder.Ascending(u => u.Email), new CreateIndexOptions { Unique = true });
            await _users.Indexes.CreateOneAsync(indexModel);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email.ToLower() == email.ToLower()).FirstOrDefaultAsync();
        }

        public async Task<User> CreateAsync(User user)
        {
            await _users.InsertOneAsync(user);
            return user;
        }

        public async Task UpdateAsync(User user)
        {
            await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
        }
    }
}
