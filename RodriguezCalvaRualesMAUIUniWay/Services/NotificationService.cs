using System.Text.Json;
using RodriguezCalvaRualesMAUIUniWay.Models;

namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public class NotificationService
    {
        private readonly string _notificacionesFile;

        public NotificationService()
        {
            var appDataPath = FileSystem.AppDataDirectory;
            _notificacionesFile = Path.Combine(appDataPath, "notificaciones.json");
        }

        public async Task<bool> GuardarNotificacionAsync(NotificacionModel notificacion)
        {
            try
            {
                var notificaciones = await GetNotificacionesAsync();
                notificacion.Id = notificaciones.Count > 0 ? notificaciones.Max(n => n.Id) + 1 : 1;
                notificacion.Fecha = DateTime.Now;

                notificaciones.Insert(0, notificacion); // Agregar al inicio

                // Mantener solo las últimas 100 notificaciones
                if (notificaciones.Count > 100)
                {
                    notificaciones = notificaciones.Take(100).ToList();
                }

                var json = JsonSerializer.Serialize(notificaciones, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_notificacionesFile, json);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error guardando notificación: {ex.Message}");
                return false;
            }
        }

        public async Task<List<NotificacionModel>> GetNotificacionesAsync(int usuarioId = 0)
        {
            try
            {
                if (File.Exists(_notificacionesFile))
                {
                    var json = await File.ReadAllTextAsync(_notificacionesFile);
                    var notificaciones = JsonSerializer.Deserialize<List<NotificacionModel>>(json) ?? new List<NotificacionModel>();

                    if (usuarioId > 0)
                    {
                        return notificaciones.Where(n => n.UsuarioId == usuarioId || n.UsuarioId == 0).ToList();
                    }

                    return notificaciones;
                }
                return new List<NotificacionModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo notificaciones: {ex.Message}");
                return new List<NotificacionModel>();
            }
        }

        public async Task<bool> MarcarComoLeidaAsync(int notificacionId)
        {
            try
            {
                var notificaciones = await GetNotificacionesAsync();
                var notificacion = notificaciones.FirstOrDefault(n => n.Id == notificacionId);

                if (notificacion != null)
                {
                    notificacion.Leida = true;
                    var json = JsonSerializer.Serialize(notificaciones, new JsonSerializerOptions { WriteIndented = true });
                    await File.WriteAllTextAsync(_notificacionesFile, json);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error marcando notificación como leída: {ex.Message}");
                return false;
            }
        }

        public async Task<int> GetNotificacionesNoLeidasCountAsync(int usuarioId)
        {
            var notificaciones = await GetNotificacionesAsync(usuarioId);
            return notificaciones.Count(n => !n.Leida);
        }

        public async Task MostrarNotificacionLocalAsync(string titulo, string mensaje, int usuarioId = 0, string tipo = "Info")
        {
            var notificacion = new NotificacionModel
            {
                Titulo = titulo,
                Mensaje = mensaje,
                UsuarioId = usuarioId,
                Tipo = tipo,
                Icono = GetIconoParaTipo(tipo),
                Leida = false
            };

            await GuardarNotificacionAsync(notificacion);
            await Application.Current.MainPage.DisplayAlert(titulo, mensaje, "OK");
        }

        public async Task<bool> EnviarNotificacionViajeAsync(string tipo, ViajeModel viaje, int usuarioId)
        {
            var (titulo, mensaje, icono) = tipo switch
            {
                "ViajeCreado" => ("🚗 Viaje Creado", $"Tu viaje {viaje.RutaCompleta} ha sido creado exitosamente", "🚗"),
                "ViajeReservado" => ("✅ Reserva Confirmada", $"Has reservado un lugar en el viaje {viaje.RutaCompleta}", "✅"),
                "ViajeCancelado" => ("❌ Viaje Cancelado", $"El viaje {viaje.RutaCompleta} ha sido cancelado", "❌"),
                "ViajeProximo" => ("⏰ Viaje Próximo", $"Tu viaje {viaje.RutaCompleta} es en 1 hora", "⏰"),
                _ => ("📱 Notificación", "Nueva notificación de UniWay", "📱")
            };

            var notificacion = new NotificacionModel
            {
                Titulo = titulo,
                Mensaje = mensaje,
                UsuarioId = usuarioId,
                Tipo = tipo,
                Icono = icono,
                Leida = false
            };

            return await GuardarNotificacionAsync(notificacion);
        }

        private string GetIconoParaTipo(string tipo)
        {
            return tipo switch
            {
                "Info" => "ℹ️",
                "Success" => "✅",
                "Warning" => "⚠️",
                "Error" => "❌",
                "Viaje" => "🚗",
                "Reserva" => "📝",
                _ => "📱"
            };
        }

        public async Task<bool> LimpiarNotificacionesAsync()
        {
            try
            {
                if (File.Exists(_notificacionesFile))
                {
                    File.Delete(_notificacionesFile);
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error limpiando notificaciones: {ex.Message}");
                return false;
            }
        }
    }
}