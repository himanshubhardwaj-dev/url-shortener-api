using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using URLShortner.API.Data;
using URLShortner.API.DTOs;
using URLShortner.API.Interfaces;
using URLShortner.API.Models;
using static System.Net.WebRequestMethods;
using URLShortner.API.Configuration;
using Microsoft.Extensions.Options;
using URLShortner.API.Exceptions;

namespace URLShortner.API.Services
{
    public class UrlService : IUrlService
    {

        private readonly UrlShortnerDbContext _context;
        private readonly IShortCodeGenerator _shortCodeGenerator;
        private readonly ShortCodeOptions _shortCodeOptions;
        private readonly ICurrentUserService _currentUser;
        public UrlService(UrlShortnerDbContext context, IShortCodeGenerator shortCodeGenerator, IOptions<ShortCodeOptions> shortCodeOptions, ICurrentUserService currentUser)
        {
            _context = context;
            _shortCodeGenerator = shortCodeGenerator;
            _shortCodeOptions = shortCodeOptions.Value;
            _currentUser = currentUser;
        }

        public async Task<CreateShortUrlResponseDto> CreateAsync(CreateShortUrlRequestDto request)
        {
            if (request.ExpiresAt.HasValue && request.ExpiresAt.Value <= DateTime.UtcNow)
            {
                throw new ArgumentException("Expiration Date must be in the future", nameof(request.ExpiresAt));
            }
            if (!string.IsNullOrWhiteSpace(request.CustomShortCode))
            {
                request.CustomShortCode = request.CustomShortCode.ToLowerInvariant();
            }
            if (!string.IsNullOrWhiteSpace(request.CustomShortCode) && _shortCodeOptions.ReservedShortCode.Any(code => code.Equals(request.CustomShortCode, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ReservedShortCodeException(request.CustomShortCode);
            }
            if (string.IsNullOrWhiteSpace(request.CustomShortCode))
            {
                var existingUrl = await _context.Urls.FirstOrDefaultAsync(ur => ur.UserId == _currentUser.UserId && ur.OriginalUrl == request.OriginalUrl && !ur.IsDeleted);
                if (existingUrl != null)
                {
                    return new CreateShortUrlResponseDto
                    {
                        OriginalUrl = existingUrl.OriginalUrl,
                        ShortCode = existingUrl.ShortCode,
                        CreatedAt = existingUrl.CreatedAt,
                        ExpiresAt = existingUrl.ExpiresAt
                    };

                }
            }

            string shortCode;
            if (!string.IsNullOrWhiteSpace(request.CustomShortCode))
            {
                shortCode = request.CustomShortCode;
                bool exists = await _context.Urls.AnyAsync(u => u.ShortCode == shortCode);
                if (exists)
                {
                    throw new DuplicateShortCodeException(shortCode);
                }
            }
            else
            {
                shortCode = _shortCodeGenerator.Generate();
                while (await _context.Urls.AnyAsync(u => u.ShortCode == shortCode))
                {
                    shortCode = _shortCodeGenerator.Generate();
                }
            }


            var url = new Url
            {
                OriginalUrl = request.OriginalUrl,
                ShortCode = shortCode,
                CreatedAt = DateTime.UtcNow,
                ClickCount = 0,
                ExpiresAt = request.ExpiresAt,
                UserId = _currentUser.UserId
            };
            _context.Urls.Add(url);
            await _context.SaveChangesAsync();
            return new CreateShortUrlResponseDto
            {
                OriginalUrl = url.OriginalUrl,
                ShortCode = url.ShortCode,
                CreatedAt = url.CreatedAt,
                ExpiresAt = url.ExpiresAt
            };

        }

        public async Task<bool> DeleteAsync(string shortCode)
        {
            var ExistUrl = await _context.Urls.FirstOrDefaultAsync(ur => ur.UserId == _currentUser.UserId && ur.ShortCode == shortCode && !ur.IsDeleted);

            if (ExistUrl == null)
                return false;

            ExistUrl.IsDeleted = true;
            ExistUrl.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<Url?> ResolveShortCodeAsync(string shortCode)
        {
            var url = await _context.Urls.SingleOrDefaultAsync(ur => ur.ShortCode == shortCode && !ur.IsDeleted);
            if (url == null)
                return null;
            if (url.ExpiresAt.HasValue && url.ExpiresAt.Value <= DateTime.UtcNow)
            {
                return null;
            }
            url.ClickCount++;

            await _context.SaveChangesAsync();

            return url;

        }
        public async Task<PagedResponseDto<UrlListItemDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Urls.AsNoTracking().Where(ur => ur.UserId == _currentUser.UserId && !ur.IsDeleted);
            var totalRecords = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            var item = await query.OrderByDescending(ur => ur.CreatedAt)
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .Select(u => new UrlListItemDto
                                 {
                                     OriginalUrl = u.OriginalUrl,
                                     ShortCode = u.ShortCode,
                                     ClickCount = u.ClickCount,
                                     CreatedAt = u.CreatedAt,
                                     ExpiresAt = u.ExpiresAt
                                 }).ToListAsync();
            return new PagedResponseDto<UrlListItemDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                Items = item
            };
        }

        public async Task<UrlAnalyticsDto?> GetAnalyticsAsync(string shortCode)
        {
            return await _context.Urls.Where(u => u.ShortCode == shortCode && u.UserId == _currentUser.UserId && !u.IsDeleted).Select(u => new UrlAnalyticsDto
            {
                OriginalUrl = u.OriginalUrl,
                ShortCode = u.ShortCode,
                ClickCount = u.ClickCount,
                CreatedAt = u.CreatedAt,
                ExpireAt = u.ExpiresAt,
                isDeleted = u.IsDeleted
            }).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(string shortCode, UpdateShortUrlRequestDto request)
        {
            var url = await _context.Urls.FirstOrDefaultAsync(u => u.ShortCode == shortCode && u.UserId == _currentUser.UserId && !u.IsDeleted);
            if (url == null)
                return false;
            if (request.ExpiresAt.HasValue && request.ExpiresAt.Value <= DateTime.UtcNow)
            {
                throw new ArgumentException("Expiration Date must be in future.", nameof(request.ExpiresAt));
            }
            url.OriginalUrl = request.OriginalUrl;
            url.ExpiresAt = request.ExpiresAt;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Url?> GetOwnedByShortAsync(string shortCode)
        {
            return await _context.Urls.FirstOrDefaultAsync(u =>
                u.ShortCode == shortCode &&
                u.UserId == _currentUser.UserId &&
                !u.IsDeleted &&
                (!u.ExpiresAt.HasValue ||
                 u.ExpiresAt > DateTime.UtcNow));
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
        {
            var query = _context.Urls.AsNoTracking().Where(url =>url.UserId == _currentUser.UserId &&!url.IsDeleted);

            var currentDate = DateTime.UtcNow;

            var totalUrls = await query.CountAsync();

            var totalClicks = await query.SumAsync(url => (int?)url.ClickCount) ?? 0;

            var activeUrls = await query.CountAsync(url => url.ExpiresAt == null || url.ExpiresAt > currentDate);

            var expiredUrls = await query.CountAsync(url => url.ExpiresAt != null && url.ExpiresAt <= currentDate);

            return new DashboardSummaryDto
            {
                TotalUrls = totalUrls,
                TotalClicks = totalClicks,
                ActiveUrls = activeUrls,
                ExpiredUrls = expiredUrls
            };
        }
    }
}
