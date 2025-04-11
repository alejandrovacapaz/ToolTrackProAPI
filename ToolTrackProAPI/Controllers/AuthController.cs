using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolTrackPro.Models.Dtos;
using ToolTrackPro.Models.Models;
using ToolTrackPro.Services;

namespace ToolTrackProAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly ITokenService tokenService;

        public AuthController(IAuthService authService, ITokenService tokenService)
        {
            this.authService = authService;
            this.tokenService = tokenService;
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] ResgisterDto userToRegister)
        {
            try
            {
                var result = await this.authService.RegisterAsync(new User
                {
                    Name = userToRegister.Name,
                    Email = userToRegister.Email,
                    PasswordHash = userToRegister.Password
                });

                return result ? Ok(result) : BadRequest("Error creating the user, review data");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto login)
        {
            try
            {
                var resultUser = await this.authService.LoginAsync(new User
                {                   
                    Email = login.Email,
                    PasswordHash = login.Password
                });

                if (resultUser == null || resultUser.Id <= 0)
                    return StatusCode(401, new { message = "An unexpected error occurred.", details = "Invalid Credencials" });

                var token = this.tokenService.GenerateToken(resultUser);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }           
        }

    }
}
