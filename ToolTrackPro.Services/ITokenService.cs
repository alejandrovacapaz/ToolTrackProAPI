using System.Security.Claims;
using ToolTrackPro.Models.Models;

namespace ToolTrackPro.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        //ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}
