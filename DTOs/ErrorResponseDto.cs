namespace URLShortner.API.DTOs
{
    public class ErrorResponseDto
    {
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; }
    }
}
