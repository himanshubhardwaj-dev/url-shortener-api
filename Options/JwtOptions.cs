using System.ComponentModel.DataAnnotations;

namespace URLShortner.API.Options
{
    public class JwtOptions
    {
        [Required]
        public string Secretkey { get; set; } = string.Empty;
        [Required]
        public string Issuer { get; set; } = string.Empty;
        [Required]
        public string Audience { get; set; } = string.Empty;
        [Range(1,1440)]
        public int  ExpirationMinutes { get; set; }
    }
}
