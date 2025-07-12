namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    public class UserPreferences
    {
        public bool RememberPassword { get; set; }
        public bool NotificationsEnabled { get; set; } = true;
        public string PreferredLanguage { get; set; } = "es";
        public bool DarkModeEnabled { get; set; }
        public bool LocationServicesEnabled { get; set; } = true;
        public int LogRetentionDays { get; set; } = 30;
    }
}