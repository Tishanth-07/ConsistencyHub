using ConsistencyHub.Models;
using System.Threading.Tasks;

namespace ConsistencyHub.Services
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);
    }
}
