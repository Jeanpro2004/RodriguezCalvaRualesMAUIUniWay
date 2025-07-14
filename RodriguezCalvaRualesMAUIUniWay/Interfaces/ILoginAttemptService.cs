using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RodriguezCalvaRualesMAUIUniWay.Models;

namespace RodriguezCalvaRualesMAUIUniWay.Interfaces
{
    public interface ILoginAttemptService
    {
        Task SaveLoginAttemptAsync(LoginAttempt attempt);
        Task<List<LoginAttempt>> GetAllLoginAttemptsAsync();
        Task<List<LoginAttempt>> GetLoginAttemptsByEmailAsync(string email);
        Task<List<LoginAttempt>> GetLoginAttemptsByDateAsync(DateTime date);
        Task<List<LoginAttempt>> GetFailedLoginAttemptsAsync();
        Task<List<LoginAttempt>> GetSuccessfulLoginAttemptsAsync();
        Task<int> GetFailedAttemptsCountAsync(string email, DateTime fromDate);
        Task ClearOldAttemptsAsync(int daysToKeep = 30);
        Task<bool> IsAccountLockedAsync(string email, int maxAttempts = 5, int lockoutMinutes = 15);
        Task<LoginAttempt> GetLastLoginAttemptAsync(string email);
    }
}