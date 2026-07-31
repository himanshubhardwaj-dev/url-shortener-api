namespace URLShortner.API.DTOs
{
    public class UrlAnalyticsDto
    {
        public string OriginalUrl { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;
        public int ClickCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpireAt { get; set; }
        public bool isDeleted { get; set; }
    }
}
