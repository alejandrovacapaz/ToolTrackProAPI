using ToolTrackPro.Models.Models;

namespace ToolTrackPro.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(User user);
        Task<User> LoginAsync(User user);
    }
}
