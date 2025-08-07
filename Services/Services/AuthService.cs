using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces;
using Services.Models;
using Services.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Services.Services
{
    public class AuthService(IOptions<JwtSettings> jwtOptions) : IAuthService
    {
        private readonly JwtSettings _jwtSettings = jwtOptions.Value;

        public string CreateToken(User user)
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            int tokenValidityMins = _jwtSettings.TokenValidityMins;

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(tokenValidityMins),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
