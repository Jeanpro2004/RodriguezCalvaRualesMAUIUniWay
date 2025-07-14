namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    public class LoginAttempt
    {
        public string Email { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; } // ATTEMPT, SUCCESS, FAILED, ERROR
        public string ErrorMessage { get; set; }
        public string IpAddress { get; set; }
        public string DeviceInfo { get; set; }
    }
}