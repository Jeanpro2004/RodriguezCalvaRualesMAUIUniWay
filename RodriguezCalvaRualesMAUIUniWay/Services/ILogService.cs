namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Debug
    }

    public interface ILogService
    {
        Task LogAsync(LogLevel level, string operation, string message, string userEmail = null, string errorDetails = null);
        Task<List<LogEntry>> GetLogsAsync();
    }

    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Operation { get; set; }
        public string Message { get; set; }
        public string UserEmail { get; set; }
        public string ErrorDetails { get; set; }
    }
}