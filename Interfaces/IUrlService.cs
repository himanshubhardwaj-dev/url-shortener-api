using URLShortner.API.DTOs;
using URLShortner.API.Models;

namespace URLShortner.API.Interfaces
{
    public interface IUrlService
    {
        Task<CreateShortUrlResponseDto> CreateAsync(CreateShortUrlRequestDto request);
        Task<Url?> ResolveShortCodeAsync(string shortCode);

        Task<bool> DeleteAsync(string shortCode);

        Task<PagedResponseDto<UrlListItemDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<Url?> GetOwnedByShortAsync(string shortCode);
        Task<UrlAnalyticsDto?> GetAnalyticsAsync(string shortCode);
        Task<bool> UpdateAsync(string shortCode, UpdateShortUrlRequestDto request);
    }
}
