using Microsoft.EntityFrameworkCore;
using URLShortner.API.Data;
using URLShortner.API.DTOs.Auth;
using URLShortner.API.Interfaces;
using URLShortner.API.Models;

namespace URLShortner.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UrlShortnerDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenservice;
        public AuthService(UrlShortnerDbContext context, IPasswordHasher passwordHasher, ITokenService tokenservice)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenservice = tokenservice;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            request.Email = request.Email.Trim().ToLowerInvariant();
            bool emailExist = await _context.Users.AnyAsync(e => e.Email == request.Email);
            if (emailExist)
            {
                throw new ArgumentException("Email already exist.");
            }
            string passwordHash = _passwordHasher.Hash(request.Password);

            var user = new User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new RegisterResponseDto
            {
                Message = "User registered successfully."
            };

        }
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            request.Email = request.Email.Trim().ToLowerInvariant();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if(user == null)
            {
                throw new ArgumentException("Invalid Email or Password");
            }
            bool passwordHash = _passwordHasher.verify(request.Password, user.PasswordHash);
            if (!passwordHash)
            {
                throw new ArgumentException("Invalid Email or Password");
            }
            if (!user.isActive)
            {
                throw new ArgumentException("Invalid Email or Password");
            }
            var tokenResult = _tokenservice.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = tokenResult.Token,
                ExpireAt = tokenResult.ExpireAt,
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role
            };
        }

    }
}
