namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public interface IFileManagementService
    {
        Task<T> LoadUserDataAsync<T>(string fileName) where T : class;
        Task SaveUserDataAsync<T>(string fileName, T data);
        Task<bool> DeleteFileAsync(string fileName);
        Task<string> ExportLogsToFileAsync();
    }
}