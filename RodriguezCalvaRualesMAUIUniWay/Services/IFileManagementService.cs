namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public interface IFileManagementService
    {
        Task<string> SaveUserDataAsync(string fileName, object data);
        Task<T?> LoadUserDataAsync<T>(string fileName) where T : class;
        Task<bool> DeleteFileAsync(string fileName);
        Task<List<string>> GetUserFilesAsync();
        Task<string> ExportLogsToFileAsync();
        Task<bool> ImportLogsFromFileAsync(string filePath);
        Task<long> GetFileSizeAsync(string fileName);
        Task<DateTime> GetFileLastModifiedAsync(string fileName);
        Task<bool> BackupUserDataAsync();
        Task<bool> RestoreUserDataAsync(string backupFileName);
    }
}