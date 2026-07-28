using System.ComponentModel.DataAnnotations;

namespace URLShortner.API.DTOs
{
    public class CreateShortUrlRequestDto
    {
        [Required]
        [Url]
        public string OriginalUrl { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
    }
}
