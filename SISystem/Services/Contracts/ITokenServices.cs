using SISystem.Models;
using System.Threading.Tasks;

namespace SISystem.Services
{
    public interface ITokenServices
    {
        Task<int> UserToken(TokenServiceGenerateToken token);
        Task<int> RevokeToken(Logout logout, string userName);
        Task<bool> CheckRefreshToken(CheckRefreshToken token);
    }
}
