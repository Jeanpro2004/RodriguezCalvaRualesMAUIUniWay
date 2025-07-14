using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RodriguezCalvaRualesMAUIUniWay.Interfaces;
using RodriguezCalvaRualesMAUIUniWay.Models;

namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public class LoginAttemptService : ILoginAttemptService
    {
        private readonly IFileService _fileService;
        private const string LoginAttemptsFileName = "login_attempts.txt";

        public LoginAttemptService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task SaveLoginAttemptAsync(LoginAttempt attempt)
        {
            try
            {
                if (attempt == null)
                    throw new ArgumentNullException(nameof(attempt));

                // Si es el primer intento del día para este email, obtener el número de intento
                if (attempt.AttemptNumber == 0)
                {
                    var todayAttempts = await GetLoginAttemptsByEmailAsync(attempt.Email);
                    var todayFailedAttempts = todayAttempts
                        .Where(a => a.AttemptDateTime.Date == DateTime.Today && !a.IsSuccessful)
                        .Count();

                    attempt.AttemptNumber = todayFailedAttempts + 1;
                }

                var line = attempt.ToString() + Environment.NewLine;
                await _fileService.AppendTextAsync(LoginAttemptsFileName, line);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar intento de login: {ex.Message}", ex);
            }
        }

        public async Task<List<LoginAttempt>> GetAllLoginAttemptsAsync()
        {
            try
            {
                var lines = await _fileService.ReadLinesAsync(LoginAttemptsFileName);
                var attempts = new List<LoginAttempt>();

                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var attempt = LoginAttempt.FromString(line);
                        if (attempt != null)
                        {
                            attempts.Add(attempt);
                        }
                    }
                }

                return attempts.OrderByDescending(a => a.AttemptDateTime).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener intentos de login: {ex.Message}", ex);
            }
        }

        public async Task<List<LoginAttempt>> GetLoginAttemptsByEmailAsync(string email)
        {
            try
            {
                var allAttempts = await GetAllLoginAttemptsAsync();
                return allAttempts
                    .Where(a => string.Equals(a.Email, email, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(a => a.AttemptDateTime)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener intentos de login por email: {ex.Message}", ex);
            }
        }

        public async Task<List<LoginAttempt>> GetLoginAttemptsByDateAsync(DateTime date)
        {
            try
            {
                var allAttempts = await GetAllLoginAttemptsAsync();
                return allAttempts
                    .Where(a => a.AttemptDateTime.Date == date.Date)
                    .OrderByDescending(a => a.AttemptDateTime)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener intentos de login por fecha: {ex.Message}", ex);
            }
        }

        public async Task<List<LoginAttempt>> GetFailedLoginAttemptsAsync()
        {
            try
            {
                var allAttempts = await GetAllLoginAttemptsAsync();
                return allAttempts
                    .Where(a => !a.IsSuccessful)
                    .OrderByDescending(a => a.AttemptDateTime)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener intentos fallidos: {ex.Message}", ex);
            }
        }

        public async Task<List<LoginAttempt>> GetSuccessfulLoginAttemptsAsync()
        {
            try
            {
                var allAttempts = await GetAllLoginAttemptsAsync();
                return allAttempts
                    .Where(a => a.IsSuccessful)
                    .OrderByDescending(a => a.AttemptDateTime)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener intentos exitosos: {ex.Message}", ex);
            }
        }

        public async Task<int> GetFailedAttemptsCountAsync(string email, DateTime fromDate)
        {
            try
            {
                var attempts = await GetLoginAttemptsByEmailAsync(email);
                return attempts
                    .Where(a => !a.IsSuccessful && a.AttemptDateTime >= fromDate)
                    .Count();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al contar intentos fallidos: {ex.Message}", ex);
            }
        }

        public async Task ClearOldAttemptsAsync(int daysToKeep = 30)
        {
            try
            {
                var allAttempts = await GetAllLoginAttemptsAsync();
                var cutoffDate = DateTime.Now.AddDays(-daysToKeep);

                var attemptsToKeep = allAttempts
                    .Where(a => a.AttemptDateTime >= cutoffDate)
                    .ToList();

                var lines = attemptsToKeep.Select(a => a.ToString()).ToList();
                await _fileService.WriteLinesAsync(LoginAttemptsFileName, lines);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al limpiar intentos antiguos: {ex.Message}", ex);
            }
        }

        public async Task<bool> IsAccountLockedAsync(string email, int maxAttempts = 5, int lockoutMinutes = 15)
        {
            try
            {
                var fromDate = DateTime.Now.AddMinutes(-lockoutMinutes);
                var recentFailedAttempts = await GetFailedAttemptsCountAsync(email, fromDate);

                return recentFailedAttempts >= maxAttempts;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar bloqueo de cuenta: {ex.Message}", ex);
            }
        }

        public async Task<LoginAttempt> GetLastLoginAttemptAsync(string email)
        {
            try
            {
                var attempts = await GetLoginAttemptsByEmailAsync(email);
                return attempts.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener último intento: {ex.Message}", ex);
            }
        }
    }
}