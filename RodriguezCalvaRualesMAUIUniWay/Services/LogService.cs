using System.Text.Json;

namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public class LogService : ILogService
    {
        private readonly string _logsFile;

        public LogService()
        {
            var appDataPath = FileSystem.AppDataDirectory;
            _logsFile = Path.Combine(appDataPath, "app_logs.json");
        }

        public async Task LogAsync(LogLevel level, string operation, string message, string userEmail = null, string errorDetails = null)
        {
            try
            {
                var logEntry = new LogEntry
                {
                    Timestamp = DateTime.Now,
                    Level = level,
                    Operation = operation,
                    Message = message,
                    UserEmail = userEmail,
                    ErrorDetails = errorDetails
                };

                var logs = await GetLogsAsync();
                logs.Insert(0, logEntry);

                // Mantener solo los últimos 1000 logs
                if (logs.Count > 1000)
                {
                    logs = logs.Take(1000).ToList();
                }

                var json = JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_logsFile, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error logging: {ex.Message}");
            }
        }

        public async Task<List<LogEntry>> GetLogsAsync()
        {
            try
            {
                if (File.Exists(_logsFile))
                {
                    var json = await File.ReadAllTextAsync(_logsFile);
                    return JsonSerializer.Deserialize<List<LogEntry>>(json) ?? new List<LogEntry>();
                }
                return new List<LogEntry>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting logs: {ex.Message}");
                return new List<LogEntry>();
            }
        }
    }
}