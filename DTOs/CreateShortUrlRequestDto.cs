using System.ComponentModel.DataAnnotations;

namespace URLShortner.API.DTOs
{
    public class CreateShortUrlRequestDto
    {
        [Required]
        [Url]
        public string OriginalUrl { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }

        [MaxLength(20)]
        [MinLength(3)]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$",ErrorMessage = "Custom short code can contain only letters,number,hyphens (-), and underscores(_).")]
        public string? CustomShortCode { get; set; }
    }
}
