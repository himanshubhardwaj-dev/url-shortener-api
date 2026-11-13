namespace URLShortner.API.DTOs
{
    public class UrlListItemDto
    {
        public string OriginalUrl { get; set; } = null!;
        public string ShortCode { get; set; } = null!;
        public int ClickCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
