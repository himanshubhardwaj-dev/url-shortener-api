namespace URLShortner.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public bool isActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public ICollection<Url> Urls { get; set; } = new List<Url>();
    }
}
