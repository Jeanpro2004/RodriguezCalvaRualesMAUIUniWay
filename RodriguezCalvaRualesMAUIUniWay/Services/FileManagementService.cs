using System.Text.Json;

namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public class FileManagementService : IFileManagementService
    {
        private readonly string _appDataPath;

        public FileManagementService()
        {
            _appDataPath = FileSystem.AppDataDirectory;
        }

        public async Task<T> LoadUserDataAsync<T>(string fileName) where T : class
        {
            try
            {
                var filePath = Path.Combine(_appDataPath, fileName);
                if (File.Exists(filePath))
                {
                    var json = await File.ReadAllTextAsync(filePath);
                    return JsonSerializer.Deserialize<T>(json);
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading {fileName}: {ex.Message}");
                return null;
            }
        }

        public async Task SaveUserDataAsync<T>(string fileName, T data)
        {
            try
            {
                var filePath = Path.Combine(_appDataPath, fileName);
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving {fileName}: {ex.Message}");
            }
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_appDataPath, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting {fileName}: {ex.Message}");
                return false;
            }
        }

        public async Task<string> ExportLogsToFileAsync()
        {
            try
            {
                var exportPath = Path.Combine(_appDataPath, $"logs_export_{DateTime.Now:yyyyMMdd_HHmmss}.json");

                var logService = new LogService();
                var logs = await logService.GetLogsAsync();

                var json = JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(exportPath, json);

                return exportPath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting logs: {ex.Message}");
                throw;
            }
        }
    }
}
