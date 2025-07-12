namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    public class RegistrationDraft
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string IdBanner { get; set; } = string.Empty;
        public bool IsDriver { get; set; }
        public DateTime SavedAt { get; set; }
    }
}