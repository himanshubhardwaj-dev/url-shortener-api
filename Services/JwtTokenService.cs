using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using URLShortner.API.DTOs.Auth;
using URLShortner.API.Interfaces;
using URLShortner.API.Models;
using URLShortner.API.Options;

namespace URLShortner.API.Services
{
    public class JwtTokenService : ITokenService
    {
        private readonly JwtOptions _JwtOptions;
        public JwtTokenService(IOptions<JwtOptions> jwtOptions)
        {
            _JwtOptions = jwtOptions.Value;
        }
        public TokenResultDto GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JwtOptions.Secretkey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expireAt = DateTime.UtcNow.AddMinutes(_JwtOptions.ExpirationMinutes);
            var token = new JwtSecurityToken(
                issuer: _JwtOptions.Issuer,
                audience: _JwtOptions.Audience,
                claims: claims,
                expires: expireAt,
                signingCredentials: credentials
                );
            return new TokenResultDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpireAt = expireAt
            };


        }
    }
}
