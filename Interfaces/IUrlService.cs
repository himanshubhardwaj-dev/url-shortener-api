using URLShortner.API.DTOs;
using URLShortner.API.Models;

namespace URLShortner.API.Interfaces
{
    public interface IUrlService
    {
        Task<CreateShortUrlResponseDto> CreateAsync(CreateShortUrlRequestDto request);
        Task<Url?> ResolveShortCodeAsync(string shortCode);
    }
}
