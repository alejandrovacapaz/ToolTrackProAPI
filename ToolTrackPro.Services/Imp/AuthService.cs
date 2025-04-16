using System.Text.RegularExpressions;
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
           if (this.ValidateUserData(user, true)) {
                return await this.userRepository.RegisterAsync(user);
           }
           else
           {
                throw new Exception("Invalid Data, Failed to Register User");
           }
        }
        public async Task<User> LoginAsync(User user)
        {
            if (this.ValidateUserData(user))
            {
                return await this.userRepository.LoginAsync(user);
            }
            else
            {
                throw new Exception("Invalid Data, Failed to Login");
            }
        }

        private bool ValidateUserData(User user, bool requireName = false)
        {
            if (user == null)
                return false;

            if (requireName && string.IsNullOrWhiteSpace(user.Name))
                return false;

            if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.PasswordHash))
                return false;

            return ValidateEmail(user.Email);
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email.Trim(), pattern, RegexOptions.IgnoreCase);
        }
    }
}
