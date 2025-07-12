using RodriguezCalvaRualesMAUIUniWay.Models;

namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public interface ILogService
    {
        Task LogAsync(LogLevel level, string operation, string message, string? userEmail = null, string? errorDetails = null, object? additionalData = null);
        Task<List<LogEntry>> GetLogsAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task ClearLogsAsync();
        Task<string> GetLogFilePathAsync();
    }
}