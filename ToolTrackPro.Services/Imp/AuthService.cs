using ToolTrackPro.Data.Repositories;
using ToolTrackPro.Models.Models;

namespace ToolTrackPro.Services.Imp
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository userRepository;

        public AuthService(IUserRepository userRepository)
        {            
            this.userRepository = userRepository;
        }

        public async Task<bool> RegisterAsync(User user)
        {            
           return await this.userRepository.RegisterAsync(user);            
        }
        public async Task<User> LoginAsync(User user)
        {
            return await this.userRepository.LoginAsync(user);
        }        
    }
}
