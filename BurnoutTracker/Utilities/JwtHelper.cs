using BurnoutTracker.Domain.Models.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BurnoutTracker.Utilities
{
    public class JwtHelper
    {
        private readonly IConfiguration _config;
        public JwtHelper(IConfiguration config) => _config = config;

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"]!));

            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                _config["Jwt:Issuer"], _config["Jwt:Audience"], claims, expires: expires, signingCredentials: creds
            ));
        }
    }

}
