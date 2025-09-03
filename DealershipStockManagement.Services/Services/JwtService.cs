using DealershipStockManagement.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DealershipStockManagement.Application.Services
{
    public class JwtService 
    {
        private readonly IConfiguration _config;
        private readonly byte[] _key;

        public JwtService(IConfiguration config)
        {
            _config = config;
            _key = Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "dev_key_change_me");
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim("id", user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role ?? "User")
        };

            var creds = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiryMinutes"] ?? "240")),
                signingCredentials: creds
            );

            try
            {
                var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                return jwtToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
