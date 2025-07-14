using RodriguezCalvaRualesMAUIUniWay.API;

namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public interface IUserSessionService
    {
        Task SaveUserSessionAsync(Usuario usuario);
        Task<Usuario> GetCurrentUserAsync();
        Task<UserPreferences> GetUserPreferencesAsync();
        Task SaveUserPreferencesAsync(UserPreferences preferences);
        Task ClearSessionAsync();
    }

    public class UserPreferences
    {
        public bool RememberPassword { get; set; }
        public string DefaultOrigin { get; set; }
        public string DefaultDestination { get; set; }
        public bool NotificationsEnabled { get; set; }
        public string Theme { get; set; } = "Light";
    }
}
