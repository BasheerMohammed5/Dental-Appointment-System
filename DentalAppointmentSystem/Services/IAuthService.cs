using System.Threading.Tasks;
using YourProjectNamespace.Models;

namespace YourProjectNamespace.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}