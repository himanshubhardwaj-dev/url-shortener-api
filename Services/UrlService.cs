using Microsoft.EntityFrameworkCore;
using URLShortner.API.Data;
using URLShortner.API.DTOs;
using URLShortner.API.Interfaces;
using URLShortner.API.Models;

namespace URLShortner.API.Services
{
    public class UrlService : IUrlService
    {

        private readonly UrlShortnerDbContext _context;
        private readonly IShortCodeGenerator _shortCodeGenerator;
        public UrlService(UrlShortnerDbContext context, IShortCodeGenerator shortCodeGenerator)
        {
            _context = context;
            _shortCodeGenerator = shortCodeGenerator;
        }

        public async Task<CreateShortUrlResponseDto> CreateAsync(CreateShortUrlRequestDto request)
        {
            string shortCode = _shortCodeGenerator.Generate();
            bool exists = await _context.Urls.AnyAsync(u => u.ShortCode == shortCode);
            while (exists)
            {
                shortCode = _shortCodeGenerator.Generate();
                exists = await _context.Urls.AnyAsync(u => u.ShortCode == shortCode);
            }
            var url = new Url
            {
                OriginalUrl = request.OriginalUrl,
                ShortCode = shortCode,
                CreatedAt = DateTime.UtcNow,
                ClickCount = 0
            };
            _context.Urls.Add(url);
            await _context.SaveChangesAsync();
            return new CreateShortUrlResponseDto 
            {
                OriginalUrl = url.OriginalUrl,
                ShortCode = url.ShortCode,
                ShortUrl = $"https://localhost:7236/{url.ShortCode}",
                CreatedAt = url.CreatedAt
            };

        }
    }
}
