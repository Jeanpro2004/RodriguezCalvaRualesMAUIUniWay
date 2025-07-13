using RodriguezCalvaRualesMAUIUniWay.API;
using RodriguezCalvaRualesMAUIUniWay.Models;

namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public interface IUserSessionService
    {
        Task SaveUserSessionAsync(Usuario usuario);
        Task<Usuario?> GetCurrentUserAsync();
        Task ClearUserSessionAsync();
        Task<bool> IsUserLoggedInAsync();
        Task SaveUserPreferencesAsync(UserPreferences preferences);
        Task<UserPreferences> GetUserPreferencesAsync();
    }
}