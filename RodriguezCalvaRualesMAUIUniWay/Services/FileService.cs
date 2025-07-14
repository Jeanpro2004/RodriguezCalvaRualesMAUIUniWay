using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RodriguezCalvaRualesMAUIUniWay.Interfaces;

namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public class FileService : IFileService
    {
        public string GetLocalFilePath(string fileName)
        {
            return Path.Combine(FileSystem.AppDataDirectory, fileName);
        }

        public async Task<string> ReadTextAsync(string fileName)
        {
            try
            {
                var filePath = GetLocalFilePath(fileName);
                if (File.Exists(filePath))
                {
                    return await File.ReadAllTextAsync(filePath);
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al leer archivo {fileName}: {ex.Message}", ex);
            }
        }

        public async Task WriteTextAsync(string fileName, string content)
        {
            try
            {
                var filePath = GetLocalFilePath(fileName);

                // Crear directorio si no existe
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllTextAsync(filePath, content);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al escribir archivo {fileName}: {ex.Message}", ex);
            }
        }

        public async Task AppendTextAsync(string fileName, string content)
        {
            try
            {
                var filePath = GetLocalFilePath(fileName);

                // Crear directorio si no existe
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.AppendAllTextAsync(filePath, content);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al agregar contenido al archivo {fileName}: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsAsync(string fileName)
        {
            try
            {
                var filePath = GetLocalFilePath(fileName);
                return await Task.FromResult(File.Exists(filePath));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task DeleteAsync(string fileName)
        {
            try
            {
                var filePath = GetLocalFilePath(fileName);
                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar archivo {fileName}: {ex.Message}", ex);
            }
        }

        public async Task<List<string>> ReadLinesAsync(string fileName)
        {
            try
            {
                var filePath = GetLocalFilePath(fileName);
                if (File.Exists(filePath))
                {
                    var lines = await File.ReadAllLinesAsync(filePath);
                    return lines.ToList();
                }
                return new List<string>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al leer líneas del archivo {fileName}: {ex.Message}", ex);
            }
        }

        public async Task WriteLinesAsync(string fileName, List<string> lines)
        {
            try
            {
                var filePath = GetLocalFilePath(fileName);

                // Crear directorio si no existe
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllLinesAsync(filePath, lines);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al escribir líneas en archivo {fileName}: {ex.Message}", ex);
            }
        }
    }
}