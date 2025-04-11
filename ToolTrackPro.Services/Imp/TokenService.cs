using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToolTrackPro.Models.Models;

namespace ToolTrackPro.Services.Imp
{
    public class TokenService : ITokenService
    {
        private readonly string secretKey;
        private readonly string issuer;
        private readonly string audience;

        public TokenService(IConfiguration config)
        {
            secretKey = config["Jwt:SecretKey"] ?? string.Empty;
            issuer = config["Jwt:Issuer"] ?? string.Empty;
            audience = config["Jwt:Audience"] ?? string.Empty;
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),                
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(                
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //public ClaimsPrincipal GetPrincipalFromToken(string token)
        //{
        //    try
        //    {
        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        //        var handler = new JwtSecurityTokenHandler();

        //        var validationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuer = true,
        //            ValidateAudience = true,
        //            ValidateLifetime = true,
        //            ValidIssuer = issuer,
        //            ValidAudience = audience,
        //            IssuerSigningKey = key
        //        };

        //        var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);
        //        return principal;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}    
    }
}
