namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    public class RegistrationAttempt
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string IdBanner { get; set; } = string.Empty;
        public bool IsDriver { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; } = string.Empty; // ATTEMPT, SUCCESS, FAILED, ERROR
        public string? ErrorMessage { get; set; }
        public string DeviceInfo { get; set; } = string.Empty;
    }
}
