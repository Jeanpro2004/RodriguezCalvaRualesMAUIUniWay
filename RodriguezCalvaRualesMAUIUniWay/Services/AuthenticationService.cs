using System.Text.Json;
using RodriguezCalvaRualesMAUIUniWay.API;
using RodriguezCalvaRualesMAUIUniWay.Repositorios;

namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public class AuthenticationService
    {
        private readonly UsuarioService _usuarioService;
        private readonly ManejoArchivosRepository _archivoRepository;

        public AuthenticationService(UsuarioService usuarioService, ManejoArchivosRepository archivoRepository)
        {
            _usuarioService = usuarioService;
            _archivoRepository = archivoRepository;
        }

        public async Task<(bool Success, Usuario Usuario, string Message)> LoginAsync(string email, string password)
        {
            try
            {
                // Intentar login con API primero
                try
                {
                    var usuarios = await _usuarioService.GetUsuariosAsync();
                    var usuarioApi = usuarios?.FirstOrDefault(u =>
                        u.Correo.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                        u.Contrasena == password);

                    if (usuarioApi != null)
                    {
                        await _archivoRepository.GuardarUsuarioActual(usuarioApi);
                        return (true, usuarioApi, "Login exitoso");
                    }
                }
                catch (Exception apiEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Error API Login: {apiEx.Message}");
                }

                // Fallback a usuarios locales
                var usuarioLocal = await _archivoRepository.ValidarCredencialesLocales(email, password);
                if (usuarioLocal != null)
                {
                    await _archivoRepository.GuardarUsuarioActual(usuarioLocal);
                    return (true, usuarioLocal, "Login exitoso (modo offline)");
                }

                return (false, null, "Email o contraseña incorrectos");
            }
            catch (Exception ex)
            {
                return (false, null, $"Error en el login: {ex.Message}");
            }
        }

        public async Task<(bool Success, Usuario Usuario, string Message)> RegisterAsync(Usuario usuario)
        {
            try
            {
                // Verificar si el usuario ya existe localmente
                var usuariosLocales = await _archivoRepository.ObtenerUsuariosLocales();
                var existeLocal = usuariosLocales.Any(u =>
                    u.Correo.Equals(usuario.Correo, StringComparison.OrdinalIgnoreCase) ||
                    u.IdBanner.Equals(usuario.IdBanner, StringComparison.OrdinalIgnoreCase));

                if (existeLocal)
                {
                    return (false, null, "Ya existe un usuario con ese email o ID Banner");
                }

                // Intentar crear en API
                try
                {
                    var usuarioCreado = await _usuarioService.CreateUsuarioAsync(usuario);
                    await _archivoRepository.GuardarUsuarioLocal(usuarioCreado);
                    return (true, usuarioCreado, "Usuario registrado exitosamente");
                }
                catch (Exception apiEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Error API Register: {apiEx.Message}");

                    // Guardar solo localmente si falla la API
                    usuario.Id = new Random().Next(1000, 9999);
                    await _archivoRepository.GuardarUsuarioLocal(usuario);
                    return (true, usuario, "Usuario registrado exitosamente (modo offline)");
                }
            }
            catch (Exception ex)
            {
                return (false, null, $"Error en el registro: {ex.Message}");
            }
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                return await _archivoRepository.CerrarSesion();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error Logout: {ex.Message}");
                return false;
            }
        }

        public async Task<Usuario> GetCurrentUserAsync()
        {
            return await _archivoRepository.ObtenerUsuarioActual();
        }

        public async Task<bool> IsUserLoggedInAsync()
        {
            var usuario = await GetCurrentUserAsync();
            return usuario != null;
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            try
            {
                var usuario = await GetCurrentUserAsync();
                if (usuario == null)
                {
                    return (false, "No hay usuario logueado");
                }

                if (usuario.Contrasena != currentPassword)
                {
                    return (false, "Contraseña actual incorrecta");
                }

                usuario.Contrasena = newPassword;

                // Intentar actualizar en API
                try
                {
                    await _usuarioService.UpdateUsuarioAsync(usuario.Id, usuario);
                }
                catch (Exception apiEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Error API ChangePassword: {apiEx.Message}");
                }

                // Actualizar localmente
                await _archivoRepository.GuardarUsuarioActual(usuario);
                await _archivoRepository.GuardarUsuarioLocal(usuario);

                return (true, "Contraseña actualizada exitosamente");
            }
            catch (Exception ex)
            {
                return (false, $"Error cambiando contraseña: {ex.Message}");
            }
        }
    }
}