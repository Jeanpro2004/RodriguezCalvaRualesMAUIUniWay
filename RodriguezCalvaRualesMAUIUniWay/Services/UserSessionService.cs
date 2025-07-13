using RodriguezCalvaRualesMAUIUniWay.API;
using RodriguezCalvaRualesMAUIUniWay.Models;

namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public class UserSessionService : IUserSessionService
    {
        private readonly IFileManagementService _fileManagementService;
        private readonly ILogService _logService;
        private const string UserSessionFile = "user_session.json";
        private const string UserPreferencesFile = "user_preferences.json";

        public UserSessionService(IFileManagementService fileManagementService, ILogService logService)
        {
            _fileManagementService = fileManagementService;
            _logService = logService;
        }

        public async Task SaveUserSessionAsync(Usuario usuario)
        {
            try
            {
                var sessionData = new UserSession
                {
                    UserId = usuario.Id,
                    Email = usuario.Correo,
                    Name = usuario.Nombre,
                    IsDriver = usuario.EsConductor,
                    LoginTime = DateTime.Now,
                    LastActivity = DateTime.Now
                };

                await _fileManagementService.SaveUserDataAsync(UserSessionFile, sessionData);
                await _logService.LogAsync(LogLevel.Info, "SESSION_SAVE", $"Sesión guardada para usuario: {usuario.Correo}", usuario.Correo);
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "SESSION_SAVE_ERROR", "Error al guardar sesión", usuario.Correo, ex.ToString());
                throw;
            }
        }

        public async Task<Usuario?> GetCurrentUserAsync()
        {
            try
            {
                var sessionData = await _fileManagementService.LoadUserDataAsync<UserSession>(UserSessionFile);
                if (sessionData == null)
                    return null;

                // Verificar si la sesión no ha expirado (ejemplo: 30 días)
                if (DateTime.Now.Subtract(sessionData.LastActivity).TotalDays > 30)
                {
                    await ClearUserSessionAsync();
                    return null;
                }

                // Actualizar última actividad
                sessionData.LastActivity = DateTime.Now;
                await _fileManagementService.SaveUserDataAsync(UserSessionFile, sessionData);

                return new Usuario
                {
                    Id = sessionData.UserId,
                    Correo = sessionData.Email,
                    Nombre = sessionData.Name,
                    EsConductor = sessionData.IsDriver
                };
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "SESSION_GET_ERROR", "Error al obtener sesión actual", null, ex.ToString());
                return null;
            }
        }

        public async Task ClearUserSessionAsync()
        {
            try
            {
                await _fileManagementService.DeleteFileAsync(UserSessionFile);
                await _logService.LogAsync(LogLevel.Info, "SESSION_CLEAR", "Sesión eliminada");
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "SESSION_CLEAR_ERROR", "Error al limpiar sesión", null, ex.ToString());
                throw;
            }
        }

        public async Task<bool> IsUserLoggedInAsync()
        {
            var currentUser = await GetCurrentUserAsync();
            return currentUser != null;
        }

        public async Task SaveUserPreferencesAsync(UserPreferences preferences)
        {
            try
            {
                await _fileManagementService.SaveUserDataAsync(UserPreferencesFile, preferences);
                await _logService.LogAsync(LogLevel.Info, "PREFERENCES_SAVE", "Preferencias de usuario guardadas");
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "PREFERENCES_SAVE_ERROR", "Error al guardar preferencias", null, ex.ToString());
                throw;
            }
        }

        public async Task<UserPreferences> GetUserPreferencesAsync()
        {
            try
            {
                var preferences = await _fileManagementService.LoadUserDataAsync<UserPreferences>(UserPreferencesFile);
                return preferences ?? new UserPreferences();
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "PREFERENCES_GET_ERROR", "Error al obtener preferencias", null, ex.ToString());
                return new UserPreferences();
            }
        }
    }
}