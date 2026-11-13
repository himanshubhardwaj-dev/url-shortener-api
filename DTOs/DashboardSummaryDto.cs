namespace URLShortner.API.DTOs
{
    public class DashboardSummaryDto
    {
        public int TotalUrls { get; set; }

        public int TotalClicks { get; set; }

        public int ActiveUrls { get; set; }

        public int ExpiredUrls { get; set; }
    }
}
