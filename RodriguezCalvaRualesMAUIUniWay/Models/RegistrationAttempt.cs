namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    public class RegistrationAttempt
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string IdBanner { get; set; }
        public bool IsDriver { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; } 
        public string ErrorMessage { get; set; }
        public string DeviceInfo { get; set; }
    }
}
