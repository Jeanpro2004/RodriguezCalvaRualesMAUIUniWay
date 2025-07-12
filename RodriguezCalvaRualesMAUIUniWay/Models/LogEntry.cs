namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? UserEmail { get; set; }
        public string? ErrorDetails { get; set; }
        public string? AdditionalData { get; set; }
    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Debug
    }
}