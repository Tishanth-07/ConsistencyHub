using ConsistencyHub.Models;
using System.Threading.Tasks;

namespace ConsistencyHub.Services
{
    public interface IUserService
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task EnsureIndexesAsync();
    }
}
