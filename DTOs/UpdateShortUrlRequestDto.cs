namespace URLShortner.API.DTOs
{
    public class UpdateShortUrlRequestDto
    {
        public string OriginalUrl { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
    }
}
