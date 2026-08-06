using URLShortner.API.DTOs.Auth;

namespace URLShortner.API.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponseDto>  RegisterAsync(RegisterRequestDto request);
    }
}
