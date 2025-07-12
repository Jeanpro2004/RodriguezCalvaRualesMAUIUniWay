namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    public static class AppConstants
    {
        // API Configuration
        public const string ApiBaseUrl = "http://localhost:5113/";
        public const int ApiTimeoutSeconds = 30;

        // File Management
        public const string UserDataFolder = "UserData";
        public const string BackupFolder = "Backups";
        public const string LogsFolder = "Logs";
        public const int MaxLogEntries = 1000;
        public const int LogRetentionDays = 30;

        // Session Management
        public const int SessionTimeoutDays = 30;
        public const string UserSessionFile = "user_session.json";
        public const string UserPreferencesFile = "user_preferences.json";

        // Security
        public const int MinPasswordLength = 8;
        public const int MaxLoginAttempts = 5;
        public const int LoginLockoutMinutes = 15;

        // UI Constants
        public const string PrimaryColor = "#8B0000";
        public const string SecondaryColor = "#2C3E50";
        public const string AccentColor = "#3498DB";
        public const string ErrorColor = "#E74C3C";
        public const string SuccessColor = "#27AE60";
        public const string WarningColor = "#F39C12";

        // File Extensions
        public const string JsonExtension = ".json";
        public const string LogExtension = ".log";
        public const string BackupExtension = ".backup";

        // Default Values
        public const string DefaultLanguage = "es";
        public const bool DefaultNotificationsEnabled = true;
        public const bool DefaultDarkModeEnabled = false;
        public const bool DefaultLocationServicesEnabled = true;
    }
}