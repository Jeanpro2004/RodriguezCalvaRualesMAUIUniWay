using RodriguezCalvaRualesMAUIUniWay.Models;
using System.Text.Json;

namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public class LogService : ILogService
    {
        private readonly string _logFileName = "uniway_logs.json";
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public async Task LogAsync(LogLevel level, string operation, string message, string? userEmail = null, string? errorDetails = null, object? additionalData = null)
        {
            var logEntry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = level.ToString(),
                Operation = operation,
                Message = message,
                UserEmail = userEmail,
                ErrorDetails = errorDetails,
                AdditionalData = additionalData?.ToString()
            };

            await _semaphore.WaitAsync();
            try
            {
                var logFilePath = await GetLogFilePathAsync();
                var logs = await GetExistingLogsAsync();

                logs.Add(logEntry);

                // Mantener solo los últimos 1000 logs
                if (logs.Count > 1000)
                {
                    logs = logs.Skip(logs.Count - 1000).ToList();
                }

                var jsonContent = JsonSerializer.Serialize(logs, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await File.WriteAllTextAsync(logFilePath, jsonContent);
            }
            catch (Exception ex)
            {
                // Si falla el logging, intentar escribir en Debug
                System.Diagnostics.Debug.WriteLine($"Error writing log: {ex.Message}");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<List<LogEntry>> GetLogsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var logs = await GetExistingLogsAsync();

            if (fromDate.HasValue)
                logs = logs.Where(l => l.Timestamp >= fromDate.Value).ToList();

            if (toDate.HasValue)
                logs = logs.Where(l => l.Timestamp <= toDate.Value).ToList();

            return logs.OrderByDescending(l => l.Timestamp).ToList();
        }

        public async Task ClearLogsAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                var logFilePath = await GetLogFilePathAsync();
                if (File.Exists(logFilePath))
                    File.Delete(logFilePath);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<string> GetLogFilePathAsync()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(appDataPath, _logFileName);
        }

        private async Task<List<LogEntry>> GetExistingLogsAsync()
        {
            try
            {
                var logFilePath = await GetLogFilePathAsync();
                if (!File.Exists(logFilePath))
                    return new List<LogEntry>();

                var jsonContent = await File.ReadAllTextAsync(logFilePath);
                if (string.IsNullOrEmpty(jsonContent))
                    return new List<LogEntry>();

                return JsonSerializer.Deserialize<List<LogEntry>>(jsonContent) ?? new List<LogEntry>();
            }
            catch
            {
                return new List<LogEntry>();
            }
        }
    }
}