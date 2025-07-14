using RodriguezCalvaRualesMAUIUniWay.API;
using System.Text.Json;

namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public class UserSessionService : IUserSessionService
    {
        private readonly string _sessionFile;
        private readonly string _preferencesFile;

        public UserSessionService()
        {
            var appDataPath = FileSystem.AppDataDirectory;
            _sessionFile = Path.Combine(appDataPath, "user_session.json");
            _preferencesFile = Path.Combine(appDataPath, "user_preferences.json");
        }

        public async Task SaveUserSessionAsync(Usuario usuario)
        {
            try
            {
                var json = JsonSerializer.Serialize(usuario, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_sessionFile, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving session: {ex.Message}");
            }
        }

        public async Task<Usuario> GetCurrentUserAsync()
        {
            try
            {
                if (File.Exists(_sessionFile))
                {
                    var json = await File.ReadAllTextAsync(_sessionFile);
                    return JsonSerializer.Deserialize<Usuario>(json);
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting current user: {ex.Message}");
                return null;
            }
        }

        public async Task<UserPreferences> GetUserPreferencesAsync()
        {
            try
            {
                if (File.Exists(_preferencesFile))
                {
                    var json = await File.ReadAllTextAsync(_preferencesFile);
                    return JsonSerializer.Deserialize<UserPreferences>(json);
                }
                return new UserPreferences();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting preferences: {ex.Message}");
                return new UserPreferences();
            }
        }

        public async Task SaveUserPreferencesAsync(UserPreferences preferences)
        {
            try
            {
                var json = JsonSerializer.Serialize(preferences, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_preferencesFile, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving preferences: {ex.Message}");
            }
        }

        public async Task ClearSessionAsync()
        {
            try
            {
                if (File.Exists(_sessionFile))
                {
                    File.Delete(_sessionFile);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing session: {ex.Message}");
            }
        }
    }
}