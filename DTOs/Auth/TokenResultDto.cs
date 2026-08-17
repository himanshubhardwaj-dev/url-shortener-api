namespace URLShortner.API.DTOs.Auth
{
    public class TokenResultDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpireAt { get; set; }
    }
}
