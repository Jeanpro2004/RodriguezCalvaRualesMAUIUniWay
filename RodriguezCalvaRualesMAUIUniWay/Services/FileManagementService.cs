using RodriguezCalvaRualesMAUIUniWay.Models;
using System.Text.Json;

namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public class FileManagementService : IFileManagementService
    {
        private readonly ILogService _logService;
        private readonly string _userDataFolder = "UserData";
        private readonly string _backupFolder = "Backups";

        public FileManagementService(ILogService logService)
        {
            _logService = logService;
            EnsureDirectoriesExist();
        }

        private void EnsureDirectoriesExist()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var userDataPath = Path.Combine(appDataPath, _userDataFolder);
            var backupPath = Path.Combine(appDataPath, _backupFolder);

            if (!Directory.Exists(userDataPath))
                Directory.CreateDirectory(userDataPath);

            if (!Directory.Exists(backupPath))
                Directory.CreateDirectory(backupPath);
        }

        public async Task<string> SaveUserDataAsync(string fileName, object data)
        {
            try
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var filePath = Path.Combine(appDataPath, _userDataFolder, fileName);

                var jsonData = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await File.WriteAllTextAsync(filePath, jsonData);

                await _logService.LogAsync(LogLevel.Info, "FILE_SAVE", $"Archivo guardado: {fileName}", null, null, new { FileSize = jsonData.Length });

                return filePath;
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "FILE_SAVE_ERROR", $"Error al guardar archivo: {fileName}", null, ex.ToString());
                throw;
            }
        }

        public async Task<T?> LoadUserDataAsync<T>(string fileName) where T : class
        {
            try
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var filePath = Path.Combine(appDataPath, _userDataFolder, fileName);

                if (!File.Exists(filePath))
                {
                    await _logService.LogAsync(LogLevel.Warning, "FILE_NOT_FOUND", $"Archivo no encontrado: {fileName}");
                    return null;
                }

                var jsonData = await File.ReadAllTextAsync(filePath);
                var data = JsonSerializer.Deserialize<T>(jsonData, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await _logService.LogAsync(LogLevel.Info, "FILE_LOAD", $"Archivo cargado: {fileName}");

                return data;
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "FILE_LOAD_ERROR", $"Error al cargar archivo: {fileName}", null, ex.ToString());
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            try
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var filePath = Path.Combine(appDataPath, _userDataFolder, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    await _logService.LogAsync(LogLevel.Info, "FILE_DELETE", $"Archivo eliminado: {fileName}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "FILE_DELETE_ERROR", $"Error al eliminar archivo: {fileName}", null, ex.ToString());
                return false;
            }
        }

        public async Task<List<string>> GetUserFilesAsync()
        {
            try
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var userDataPath = Path.Combine(appDataPath, _userDataFolder);

                if (!Directory.Exists(userDataPath))
                    return new List<string>();

                var files = Directory.GetFiles(userDataPath)
                    .Select(Path.GetFileName)
                    .Where(f => !string.IsNullOrEmpty(f))
                    .ToList();

                await _logService.LogAsync(LogLevel.Info, "GET_USER_FILES", $"Archivos encontrados: {files.Count}");

                return files!;
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "GET_USER_FILES_ERROR", "Error al obtener lista de archivos", null, ex.ToString());
                return new List<string>();
            }
        }

        public async Task<string> ExportLogsToFileAsync()
        {
            try
            {
                var logs = await _logService.GetLogsAsync();
                var fileName = $"uniway_logs_export_{DateTime.Now:yyyyMMdd_HHmmss}.json";

                var exportData = new
                {
                    ExportDate = DateTime.Now,
                    TotalLogs = logs.Count,
                    Logs = logs
                };

                var filePath = await SaveUserDataAsync(fileName, exportData);

                await _logService.LogAsync(LogLevel.Info, "LOGS_EXPORT", $"Logs exportados a: {fileName}", null, null, new { LogCount = logs.Count });

                return filePath;
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "LOGS_EXPORT_ERROR", "Error al exportar logs", null, ex.ToString());
                throw;
            }
        }

        public async Task<bool> ImportLogsFromFileAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                var jsonData = await File.ReadAllTextAsync(filePath);
                var importData = JsonSerializer.Deserialize<dynamic>(jsonData);

                // Implementar lógica de importación según sea necesario
                await _logService.LogAsync(LogLevel.Info, "LOGS_IMPORT", $"Logs importados desde: {Path.GetFileName(filePath)}");

                return true;
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "LOGS_IMPORT_ERROR", $"Error al importar logs desde: {filePath}", null, ex.ToString());
                return false;
            }
        }

        public async Task<long> GetFileSizeAsync(string fileName)
        {
            try
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var filePath = Path.Combine(appDataPath, _userDataFolder, fileName);

                if (File.Exists(filePath))
                {
                    var fileInfo = new FileInfo(filePath);
                    return fileInfo.Length;
                }

                return 0;
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "GET_FILE_SIZE_ERROR", $"Error al obtener tamaño del archivo: {fileName}", null, ex.ToString());
                return 0;
            }
        }

        public async Task<DateTime> GetFileLastModifiedAsync(string fileName)
        {
            try
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var filePath = Path.Combine(appDataPath, _userDataFolder, fileName);

                if (File.Exists(filePath))
                {
                    return File.GetLastWriteTime(filePath);
                }

                return DateTime.MinValue;
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "GET_FILE_MODIFIED_ERROR", $"Error al obtener fecha de modificación: {fileName}", null, ex.ToString());
                return DateTime.MinValue;
            }
        }

        public async Task<bool> BackupUserDataAsync()
        {
            try
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var userDataPath = Path.Combine(appDataPath, _userDataFolder);
                var backupPath = Path.Combine(appDataPath, _backupFolder);

                var backupFileName = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                var backupFilePath = Path.Combine(backupPath, backupFileName);

                // Crear backup de todos los archivos del usuario
                var files = await GetUserFilesAsync();
                var backupData = new Dictionary<string, object>();

                foreach (var file in files)
                {
                    try
                    {
                        var fileContent = await LoadUserDataAsync<object>(file);
                        if (fileContent != null)
                            backupData[file] = fileContent;
                    }
                    catch
                    {
                        // Continuar con el siguiente archivo si hay error
                    }
                }

                // Incluir logs en el backup
                var logs = await _logService.GetLogsAsync();
                backupData["logs.json"] = logs;

                var jsonBackup = JsonSerializer.Serialize(backupData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await File.WriteAllTextAsync(backupFilePath, jsonBackup);

                await _logService.LogAsync(LogLevel.Info, "BACKUP_SUCCESS", $"Backup creado: {backupFileName}", null, null, new { FileCount = files.Count });

                return true;
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "BACKUP_ERROR", "Error al crear backup", null, ex.ToString());
                return false;
            }
        }

        public async Task<bool> RestoreUserDataAsync(string backupFileName)
        {
            try
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var backupPath = Path.Combine(appDataPath, _backupFolder, backupFileName);

                if (!File.Exists(backupPath))
                    return false;

                var jsonBackup = await File.ReadAllTextAsync(backupPath);
                var backupData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBackup);

                if (backupData == null)
                    return false;

                foreach (var kvp in backupData)
                {
                    if (kvp.Key == "logs.json")
                        continue; // Los logs se manejan por separado

                    await SaveUserDataAsync(kvp.Key, kvp.Value);
                }

                await _logService.LogAsync(LogLevel.Info, "RESTORE_SUCCESS", $"Datos restaurados desde: {backupFileName}");

                return true;
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "RESTORE_ERROR", $"Error al restaurar desde: {backupFileName}", null, ex.ToString());
                return false;
            }
        }
    }
}