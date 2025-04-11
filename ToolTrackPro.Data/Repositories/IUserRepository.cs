using ToolTrackPro.Models.Models;

namespace ToolTrackPro.Data.Repositories
{
    public interface IUserRepository
    {
        Task<User> LoginAsync(User user);
        Task<bool> RegisterAsync(User user);
        Task<string> GetEmailAsync(int userId);
    }
}
