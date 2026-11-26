using URLShortner.API.DTOs.Auth;

namespace URLShortner.API.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<UserProfileDto> GetProfileAsync();
        Task<UserProfileDto> UpdateProfileAsync(UpdateProfileRequestDto request);
        Task ChangePasswordAsync(ChangePasswordRequestDto request);
    }
}
