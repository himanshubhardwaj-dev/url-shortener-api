namespace URLShortner.API.DTOs
{
    public class CreateShortUrlResponseDto
    {
        public string OriginalUrl { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } 
        public DateTime? ExpiresAt { get; set; }
    }
}
