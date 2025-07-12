namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    public class UserSession
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsDriver { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime LastActivity { get; set; }
    }
}