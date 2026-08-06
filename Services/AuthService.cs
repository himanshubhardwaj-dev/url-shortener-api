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
        public AuthService(UrlShortnerDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
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
    }
}
