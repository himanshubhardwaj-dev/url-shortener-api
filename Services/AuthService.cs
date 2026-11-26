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
        private readonly ICurrentUserService _currentUserService;

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

            if (user == null)
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
        public async Task<UserProfileDto> GetProfileAsync()
        {
            var user = await GetCurrentUserAsync();
            return MapProfile(user);
        }

        public async Task<UserProfileDto> UpdateProfileAsync(UpdateProfileRequestDto request)
        {
            var user = await GetCurrentUserAsync();
            var email = request.Email.Trim().ToLowerInvariant();

            bool emailInUse = await _context.Users.AnyAsync(u =>
                u.Id != user.Id && u.Email == email);

            if (emailInUse)
                throw new ArgumentException("This email address is already in use.");

            user.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();
            user.Email = email;

            await _context.SaveChangesAsync();
            return MapProfile(user);
        }

        public async Task ChangePasswordAsync(ChangePasswordRequestDto request)
        {
            var user = await GetCurrentUserAsync();

            if (!_passwordHasher.verify(request.CurrentPassword, user.PasswordHash))
                throw new ArgumentException("Your current password is incorrect.");

            if (_passwordHasher.verify(request.NewPassword, user.PasswordHash))
                throw new ArgumentException("Your new password must be different from your current password.");

            user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
            await _context.SaveChangesAsync();
        }

        private async Task<User> GetCurrentUserAsync()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Id == _currentUserService.UserId);

            return user ?? throw new KeyNotFoundException("The account could not be found.");
        }

        private static UserProfileDto MapProfile(User user)
        {
            return new UserProfileDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.isActive,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
