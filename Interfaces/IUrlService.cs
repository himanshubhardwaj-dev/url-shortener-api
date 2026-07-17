using URLShortner.API.DTOs;

namespace URLShortner.API.Interfaces
{
    public interface IUrlService
    {
        Task<CreateShortUrlResponseDto> CreateAsync(CreateShortUrlRequestDto request);
    }
}
