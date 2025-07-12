namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    public class LoginAttempt
    {
        public string Email { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Status { get; set; } = string.Empty; // ATTEMPT, SUCCESS, FAILED, ERROR
        public string? ErrorMessage { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string DeviceInfo { get; set; } = string.Empty;
    }
}
