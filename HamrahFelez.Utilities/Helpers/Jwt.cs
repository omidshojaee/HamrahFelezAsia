using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HamrahFelez.Utilities
{
    public sealed class Jwt
    {
        private readonly IConfiguration _configuration;

        public Jwt(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(long pkUser, int role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var keyString = jwtSettings["Key"]
                            ?? throw new InvalidOperationException("Jwt Key is not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            var expiresInDays = Convert.ToDouble(jwtSettings["ExpiresInDays"]);
            var expires = now.AddDays(expiresInDays);

            var roleName = Enum.GetName(typeof(Enums.ApiRoles), role);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, pkUser.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                          new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
                          ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.Role, roleName)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
