using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RodriguezCalvaRualesMAUIUniWay.Models;

namespace RodriguezCalvaRualesMAUIUniWay.Repositories
{
    public class LoginAttemptRepository
    {
        private string path = Path.Combine(FileSystem.AppDataDirectory, "login_attempts.txt");

        public async Task<bool> GuardarIntentoLogin(LoginAttempt intento)
        {
            try
            {
                // Leer contenido existente
                string contenidoExistente = string.Empty;
                if (File.Exists(path))
                {
                    contenidoExistente = await File.ReadAllTextAsync(path, Encoding.UTF8);
                }

                // Agregar fecha y hora al registro 
                string registroCompleto = $"{intento.ToString()}\n";

                // Combinar contenido existente + nuevo registro
                string contenidoFinal = contenidoExistente + registroCompleto;

                // Guardar todo junto
                await File.WriteAllTextAsync(path, contenidoFinal, Encoding.UTF8);

                // Verificar que se guardó correctamente (como en tu ManejoArchivosRepository)
                bool guardadoCorrectamente = File.Exists(path) &&
                                           !string.IsNullOrEmpty(await File.ReadAllTextAsync(path));

                if (guardadoCorrectamente)
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Intento de login guardado: {intento.Email} - {intento.GetStatusText()}");
                }

                return guardadoCorrectamente;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al guardar intento de login: {ex.Message}");
                return false;
            }
        }

        public async Task<List<LoginAttempt>> ObtenerTodosLosIntentos()
        {
            try
            {
                if (File.Exists(path))
                {
                    string contenido = await File.ReadAllTextAsync(path, Encoding.UTF8);

                    if (string.IsNullOrWhiteSpace(contenido))
                    {
                        return new List<LoginAttempt>();
                    }

                    var lineas = contenido.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    var intentos = new List<LoginAttempt>();

                    foreach (var linea in lineas)
                    {
                        if (!string.IsNullOrWhiteSpace(linea))
                        {
                            var intento = LoginAttempt.FromString(linea);
                            if (intento != null)
                            {
                                intentos.Add(intento);
                            }
                        }
                    }

                    return intentos.OrderByDescending(i => i.AttemptDateTime).ToList();
                }

                return new List<LoginAttempt>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al leer intentos de login: {ex.Message}");
                return new List<LoginAttempt>();
            }
        }

        public async Task<List<LoginAttempt>> ObtenerIntentosPorEmail(string email)
        {
            try
            {
                var todosLosIntentos = await ObtenerTodosLosIntentos();
                return todosLosIntentos
                    .Where(i => string.Equals(i.Email, email, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(i => i.AttemptDateTime)
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al obtener intentos por email: {ex.Message}");
                return new List<LoginAttempt>();
            }
        }

        public async Task<int> ContarIntentosFallidos(string email, DateTime desde)
        {
            try
            {
                var intentos = await ObtenerIntentosPorEmail(email);
                return intentos
                    .Where(i => !i.IsSuccessful && i.AttemptDateTime >= desde)
                    .Count();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al contar intentos fallidos: {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> EliminarArchivo()
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    return !File.Exists(path);
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al eliminar archivo de intentos: {ex.Message}");
                return false;
            }
        }

        public async Task<FileInfo?> ObtenerInformacionDelArchivo()
        {
            try
            {
                if (File.Exists(path))
                {
                    return new FileInfo(path);
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al obtener información del archivo: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> LimpiarIntentosAntiguos(int diasAMantener = 30)
        {
            try
            {
                var todosLosIntentos = await ObtenerTodosLosIntentos();
                var fechaCorte = DateTime.Now.AddDays(-diasAMantener);

                var intentosAMantener = todosLosIntentos
                    .Where(i => i.AttemptDateTime >= fechaCorte)
                    .ToList();

                // Reescribir el archivo solo con los intentos recientes
                var lineasAMantener = intentosAMantener.Select(i => i.ToString()).ToList();
                string contenidoFinal = string.Join("\n", lineasAMantener) + "\n";

                await File.WriteAllTextAsync(path, contenidoFinal, Encoding.UTF8);

                System.Diagnostics.Debug.WriteLine($"✅ Archivo limpiado: mantenidos {intentosAMantener.Count} de {todosLosIntentos.Count} intentos");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al limpiar intentos antiguos: {ex.Message}");
                return false;
            }
        }

        // Método para debug - mostrar ruta del archivo
        public string ObtenerRutaArchivo()
        {
            return path;
        }
    }
}