using System;
using System.Text.Json;
using RodriguezCalvaRualesMAUIUniWay.API;
using RodriguezCalvaRualesMAUIUniWay.Models;

namespace RodriguezCalvaRualesMAUIUniWay.Repositorios
{
    public class ManejoArchivosRepository
    {
        private readonly string _appDataPath;
        private readonly string _credencialesFile;
        private readonly string _usuarioActualFile;
        private readonly string _usuariosLocalesFile;
        private readonly string _viajesLocalesFile;
        private readonly string _reservasLocalesFile;
        private readonly string _notificacionesFile;
        private readonly string _notasFile;
        private readonly string _configFile;
        private readonly string _favoritosFile;

        public ManejoArchivosRepository()
        {
            _appDataPath = FileSystem.AppDataDirectory;
            _credencialesFile = Path.Combine(_appDataPath, "credenciales.txt");
            _usuarioActualFile = Path.Combine(_appDataPath, "usuario_actual.json");
            _usuariosLocalesFile = Path.Combine(_appDataPath, "usuarios_locales.json");
            _viajesLocalesFile = Path.Combine(_appDataPath, "viajes_locales.json");
            _reservasLocalesFile = Path.Combine(_appDataPath, "reservas_locales.json");
            _notificacionesFile = Path.Combine(_appDataPath, "notificaciones.json");
            _notasFile = Path.Combine(_appDataPath, "notas_uniway.txt");
            _configFile = Path.Combine(_appDataPath, "config.json");
            _favoritosFile = Path.Combine(_appDataPath, "rutas_favoritas.json");
        }

        #region Manejo de Credenciales

        public async Task<bool> GuardarCredenciales(string email, string password)
        {
            try
            {
                var contenido = $"{email}\n{password}";
                await File.WriteAllTextAsync(_credencialesFile, contenido);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error guardando credenciales: {ex.Message}");
                return false;
            }
        }

        public async Task<string> ObtenerCredencialesGuardadas()
        {
            try
            {
                if (File.Exists(_credencialesFile))
                {
                    return await File.ReadAllTextAsync(_credencialesFile);
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo credenciales: {ex.Message}");
                return string.Empty;
            }
        }

        public async Task<(string Email, string Password)> ObtenerCredencialesDesglosadas()
        {
            try
            {
                var contenido = await ObtenerCredencialesGuardadas();
                if (!string.IsNullOrEmpty(contenido))
                {
                    var lines = contenido.Split('\n');
                    if (lines.Length >= 2)
                    {
                        return (lines[0], lines[1]);
                    }
                }
                return (string.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo credenciales desglosadas: {ex.Message}");
                return (string.Empty, string.Empty);
            }
        }

        public async Task<bool> LimpiarCredenciales()
        {
            try
            {
                if (File.Exists(_credencialesFile))
                {
                    File.Delete(_credencialesFile);
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error limpiando credenciales: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Manejo de Usuario Actual

        public async Task<bool> GuardarUsuarioActual(Usuario usuario)
        {
            try
            {
                var usuarioExtendido = new UsuarioExtendedModel
                {
                    Id = usuario.Id,
                    IdBanner = usuario.IdBanner,
                    Nombre = usuario.Nombre,
                    Correo = usuario.Correo,
                    Telefono = usuario.Telefono,
                    Contrasena = usuario.Contrasena,
                    EsConductor = usuario.EsConductor,
                    UltimoAcceso = DateTime.Now,
                    FechaRegistro = DateTime.Now
                };

                var json = JsonSerializer.Serialize(usuarioExtendido, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_usuarioActualFile, json);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error guardando usuario actual: {ex.Message}");
                return false;
            }
        }

        public async Task<Usuario> ObtenerUsuarioActual()
        {
            try
            {
                if (File.Exists(_usuarioActualFile))
                {
                    var json = await File.ReadAllTextAsync(_usuarioActualFile);
                    var usuario = JsonSerializer.Deserialize<UsuarioExtendedModel>(json);

                    // Actualizar último acceso
                    if (usuario != null)
                    {
                        usuario.UltimoAcceso = DateTime.Now;
                        await GuardarUsuarioActual(usuario);
                    }

                    return usuario;
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo usuario actual: {ex.Message}");
                return null;
            }
        }

        public async Task<UsuarioExtendedModel> ObtenerUsuarioActualExtendido()
        {
            try
            {
                if (File.Exists(_usuarioActualFile))
                {
                    var json = await File.ReadAllTextAsync(_usuarioActualFile);
                    return JsonSerializer.Deserialize<UsuarioExtendedModel>(json);
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo usuario actual extendido: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CerrarSesion()
        {
            try
            {
                if (File.Exists(_usuarioActualFile))
                {
                    File.Delete(_usuarioActualFile);
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cerrando sesión: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Manejo de Usuarios Locales

        public async Task<bool> GuardarUsuarioLocal(Usuario usuario)
        {
            try
            {
                var usuarios = await ObtenerUsuariosLocales();

                var usuarioExistente = usuarios.FirstOrDefault(u => u.Correo == usuario.Correo || u.IdBanner == usuario.IdBanner);
                if (usuarioExistente != null)
                {
                    var index = usuarios.IndexOf(usuarioExistente);
                    usuarios[index] = usuario;
                }
                else
                {
                    usuarios.Add(usuario);
                }

                var json = JsonSerializer.Serialize(usuarios, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_usuariosLocalesFile, json);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error guardando usuario local: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Usuario>> ObtenerUsuariosLocales()
        {
            try
            {
                if (File.Exists(_usuariosLocalesFile))
                {
                    var json = await File.ReadAllTextAsync(_usuariosLocalesFile);
                    return JsonSerializer.Deserialize<List<Usuario>>(json) ?? new List<Usuario>();
                }
                return new List<Usuario>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo usuarios locales: {ex.Message}");
                return new List<Usuario>();
            }
        }

        public async Task<Usuario> ValidarCredencialesLocales(string email, string password)
        {
            try
            {
                var usuarios = await ObtenerUsuariosLocales();
                return usuarios.FirstOrDefault(u => u.Correo.Equals(email, StringComparison.OrdinalIgnoreCase) && u.Contrasena == password);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error validando credenciales locales: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> ExisteUsuarioLocal(string email, string idBanner)
        {
            try
            {
                var usuarios = await ObtenerUsuariosLocales();
                return usuarios.Any(u =>
                    u.Correo.Equals(email, StringComparison.OrdinalIgnoreCase) ||
                    u.IdBanner.Equals(idBanner, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error verificando existencia de usuario: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Manejo de Viajes Locales

        public async Task<bool> GuardarViajeLocal(ViajeModel viaje)
        {
            try
            {
                var viajes = await ObtenerViajesLocales();

                var viajeExistente = viajes.FirstOrDefault(v => v.Id == viaje.Id);
                if (viajeExistente != null)
                {
                    var index = viajes.IndexOf(viajeExistente);
                    viajes[index] = viaje;
                }
                else
                {
                    viajes.Add(viaje);
                }

                var json = JsonSerializer.Serialize(viajes, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_viajesLocalesFile, json);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error guardando viaje local: {ex.Message}");
                return false;
            }
        }

        public async Task<List<ViajeModel>> ObtenerViajesLocales()
        {
            try
            {
                if (File.Exists(_viajesLocalesFile))
                {
                    var json = await File.ReadAllTextAsync(_viajesLocalesFile);
                    return JsonSerializer.Deserialize<List<ViajeModel>>(json) ?? new List<ViajeModel>();
                }
                return new List<ViajeModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo viajes locales: {ex.Message}");
                return new List<ViajeModel>();
            }
        }

        public async Task<List<ViajeModel>> ObtenerViajesPorConductor(int conductorId)
        {
            try
            {
                var viajes = await ObtenerViajesLocales();
                return viajes.Where(v => v.ConductorId == conductorId).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo viajes por conductor: {ex.Message}");
                return new List<ViajeModel>();
            }
        }

        #endregion

        #region Manejo de Reservas Locales

        public async Task<bool> GuardarReservaLocal(ReservaModel reserva)
        {
            try
            {
                var reservas = await ObtenerReservasLocales();

                var reservaExistente = reservas.FirstOrDefault(r => r.Id == reserva.Id);
                if (reservaExistente != null)
                {
                    var index = reservas.IndexOf(reservaExistente);
                    reservas[index] = reserva;
                }
                else
                {
                    reservas.Add(reserva);
                }

                var json = JsonSerializer.Serialize(reservas, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_reservasLocalesFile, json);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error guardando reserva local: {ex.Message}");
                return false;
            }
        }

        public async Task<List<ReservaModel>> ObtenerReservasLocales()
        {
            try
            {
                if (File.Exists(_reservasLocalesFile))
                {
                    var json = await File.ReadAllTextAsync(_reservasLocalesFile);
                    return JsonSerializer.Deserialize<List<ReservaModel>>(json) ?? new List<ReservaModel>();
                }
                return new List<ReservaModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo reservas locales: {ex.Message}");
                return new List<ReservaModel>();
            }
        }

        public async Task<List<ReservaModel>> ObtenerReservasPorUsuario(int usuarioId)
        {
            try
            {
                var reservas = await ObtenerReservasLocales();
                return reservas.Where(r => r.UsuarioId == usuarioId).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo reservas por usuario: {ex.Message}");
                return new List<ReservaModel>();
            }
        }

        #endregion

        #region Manejo de Rutas Favoritas

        public async Task<bool> GuardarRutaFavorita(RutaModel ruta)
        {
            try
            {
                var favoritas = await ObtenerRutasFavoritas();

                if (!favoritas.Any(r => r.Origen == ruta.Origen && r.Destino == ruta.Destino))
                {
                    favoritas.Add(ruta);
                    var json = JsonSerializer.Serialize(favoritas, new JsonSerializerOptions { WriteIndented = true });
                    await File.WriteAllTextAsync(_favoritosFile, json);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error guardando ruta favorita: {ex.Message}");
                return false;
            }
        }

        public async Task<List<RutaModel>> ObtenerRutasFavoritas()
        {
            try
            {
                if (File.Exists(_favoritosFile))
                {
                    var json = await File.ReadAllTextAsync(_favoritosFile);
                    return JsonSerializer.Deserialize<List<RutaModel>>(json) ?? new List<RutaModel>();
                }
                return new List<RutaModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo rutas favoritas: {ex.Message}");
                return new List<RutaModel>();
            }
        }

        #endregion

        #region Manejo de Notas (Similar al proyecto original)

        public async Task<bool> GuardarArchivo(string contenido)
        {
            try
            {
                if (File.Exists(_notasFile))
                {
                    var contenidoExistente = await File.ReadAllTextAsync(_notasFile);
                    contenido = contenidoExistente + contenido;
                }

                await File.WriteAllTextAsync(_notasFile, contenido);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error guardando archivo: {ex.Message}");
                return false;
            }
        }

        public async Task<string> ObtenerInformacionArchivo()
        {
            try
            {
                if (File.Exists(_notasFile))
                {
                    var contenido = await File.ReadAllTextAsync(_notasFile);
                    return string.IsNullOrEmpty(contenido) ? "No encontré nada en el archivo" : contenido;
                }
                return "No encontré nada en el archivo";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo información del archivo: {ex.Message}");
                return "Error al leer el archivo";
            }
        }

        public async Task<bool> LimpiarArchivo()
        {
            try
            {
                if (File.Exists(_notasFile))
                {
                    File.Delete(_notasFile);
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error limpiando archivo: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Configuración de la App

        public async Task<bool> GuardarConfiguracion(Dictionary<string, object> config)
        {
            try
            {
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_configFile, json);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error guardando configuración: {ex.Message}");
                return false;
            }
        }

        public async Task<Dictionary<string, object>> ObtenerConfiguracion()
        {
            try
            {
                if (File.Exists(_configFile))
                {
                    var json = await File.ReadAllTextAsync(_configFile);
                    return JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();
                }
                return new Dictionary<string, object>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo configuración: {ex.Message}");
                return new Dictionary<string, object>();
            }
        }

        #endregion

        #region Métodos de utilidad

        public async Task<bool> ExportarDatos(string rutaDestino)
        {
            try
            {
                var datos = new
                {
                    UsuarioActual = await ObtenerUsuarioActual(),
                    UsuariosLocales = await ObtenerUsuariosLocales(),
                    ViajesLocales = await ObtenerViajesLocales(),
                    ReservasLocales = await ObtenerReservasLocales(),
                    RutasFavoritas = await ObtenerRutasFavoritas(),
                    Notas = await ObtenerInformacionArchivo(),
                    Configuracion = await ObtenerConfiguracion(),
                    FechaExportacion = DateTime.Now
                };

                var json = JsonSerializer.Serialize(datos, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(rutaDestino, json);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exportando datos: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> LimpiarTodosLosDatos()
        {
            try
            {
                var archivos = new[] {
                    _credencialesFile, _usuarioActualFile, _usuariosLocalesFile,
                    _viajesLocalesFile, _reservasLocalesFile, _notificacionesFile,
                    _notasFile, _configFile, _favoritosFile
                };

                foreach (var archivo in archivos)
                {
                    if (File.Exists(archivo))
                    {
                        File.Delete(archivo);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error limpiando todos los datos: {ex.Message}");
                return false;
            }
        }

        public async Task<long> ObtenerTamañoTotalDatos()
        {
            try
            {
                long tamaño = 0;
                var archivos = new[] {
                    _credencialesFile, _usuarioActualFile, _usuariosLocalesFile,
                    _viajesLocalesFile, _reservasLocalesFile, _notificacionesFile,
                    _notasFile, _configFile, _favoritosFile
                };

                foreach (var archivo in archivos)
                {
                    if (File.Exists(archivo))
                    {
                        var info = new FileInfo(archivo);
                        tamaño += info.Length;
                    }
                }

                return tamaño;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo tamaño de datos: {ex.Message}");
                return 0;
            }
        }

        public string ObtenerRutaDirectorioDatos()
        {
            return _appDataPath;
        }

        #endregion
    }
}