using RodriguezCalvaRualesMAUIUniWay.Models;
using RodriguezCalvaRualesMAUIUniWay.Services;
using System.Windows.Input;

namespace RodriguezCalvaRualesMAUIUniWay.ViewModels
{
    public class EnhancedLoginViewModel : BaseViewModel
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogService _logService;
        private readonly IUserSessionService _sessionService;
        private readonly IFileManagementService _fileService;

        private string _email = string.Empty;
        private string _password = string.Empty;
        private bool _rememberPassword;
        private bool _isLoading;
        private string _statusMessage = string.Empty;

        public EnhancedLoginViewModel(
            IUsuarioService usuarioService,
            ILogService logService,
            IUserSessionService sessionService,
            IFileManagementService fileService)
        {
            _usuarioService = usuarioService;
            _logService = logService;
            _sessionService = sessionService;
            _fileService = fileService;
            Title = "Iniciar Sesión";

            LoginCommand = new Command(async () => await ExecuteLoginCommand(), () => CanExecuteLogin());
            ForgotPasswordCommand = new Command(async () => await ExecuteForgotPasswordCommand());
            NavigateToRegisterCommand = new Command(async () => await ExecuteNavigateToRegisterCommand());
            ExportLogsCommand = new Command(async () => await ExecuteExportLogsCommand());
            ViewLogsCommand = new Command(async () => await ExecuteViewLogsCommand());
        }

        public string Email
        {
            get => _email;
            set
            {
                SetProperty(ref _email, value);
                ((Command)LoginCommand).ChangeCanExecute();
                StatusMessage = string.Empty;
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                ((Command)LoginCommand).ChangeCanExecute();
                StatusMessage = string.Empty;
            }
        }

        public bool RememberPassword
        {
            get => _rememberPassword;
            set => SetProperty(ref _rememberPassword, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }
        public ICommand ExportLogsCommand { get; }
        public ICommand ViewLogsCommand { get; }

        private bool CanExecuteLogin()
        {
            return !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !IsLoading;
        }

        private async Task ExecuteLoginCommand()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                IsBusy = true;
                StatusMessage = "Iniciando sesión...";

                var loginRequest = new LoginRequest
                {
                    Correo = Email.Trim(),
                    Contrasena = Password,
                    RecordarContrasena = RememberPassword
                };

                // Guardar intento de login en archivo
                await SaveLoginAttempt(Email, DateTime.Now, "ATTEMPT");

                var result = await _usuarioService.LoginAsync(loginRequest);

                if (result.IsSuccess && result.Data != null)
                {
                    // Guardar sesión del usuario
                    await _sessionService.SaveUserSessionAsync(result.Data);

                    // Guardar preferencias si recuerda contraseña
                    if (RememberPassword)
                    {
                        var preferences = await _sessionService.GetUserPreferencesAsync();
                        preferences.RememberPassword = true;
                        await _sessionService.SaveUserPreferencesAsync(preferences);

                        await SecureStorage.SetAsync("saved_email", Email);
                    }
                    else
                    {
                        SecureStorage.Remove("saved_email");
                    }

                    // Guardar login exitoso
                    await SaveLoginAttempt(Email, DateTime.Now, "SUCCESS");

                    StatusMessage = "¡Inicio de sesión exitoso!";
                    await Task.Delay(1000); // Mostrar mensaje brevemente

                    // Navegar a la página principal
                    await Shell.Current.GoToAsync("//HomePage");
                }
                else
                {
                    await SaveLoginAttempt(Email, DateTime.Now, "FAILED", result.Message);
                    StatusMessage = result.Message;
                    await Shell.Current.DisplayAlert("Error", result.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                await SaveLoginAttempt(Email, DateTime.Now, "ERROR", ex.Message);
                await _logService.LogAsync(LogLevel.Error, "LOGIN_VM_ERROR", "Error en LoginViewModel", Email, ex.ToString());
                StatusMessage = "Ocurrió un error inesperado";
                await Shell.Current.DisplayAlert("Error", "Ocurrió un error inesperado", "OK");
            }
            finally
            {
                IsLoading = false;
                IsBusy = false;
            }
        }

        private async Task SaveLoginAttempt(string email, DateTime timestamp, string status, string? error = null)
        {
            try
            {
                var loginAttempt = new LoginAttempt
                {
                    Email = email,
                    Timestamp = timestamp,
                    Status = status,
                    ErrorMessage = error,
                    IpAddress = await GetDeviceIpAddress(),
                    DeviceInfo = GetDeviceInfo()
                };

                var fileName = $"login_attempts_{DateTime.Now:yyyyMM}.json";
                var existingAttempts = await _fileService.LoadUserDataAsync<List<LoginAttempt>>(fileName) ?? new List<LoginAttempt>();

                existingAttempts.Add(loginAttempt);

                // Mantener solo los últimos 100 intentos por mes
                if (existingAttempts.Count > 100)
                {
                    existingAttempts = existingAttempts.Skip(existingAttempts.Count - 100).ToList();
                }

                await _fileService.SaveUserDataAsync(fileName, existingAttempts);
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "SAVE_LOGIN_ATTEMPT_ERROR", "Error al guardar intento de login", email, ex.ToString());
            }
        }

        private async Task<string> GetDeviceIpAddress()
        {
            try
            {
                // Implementación simplificada - en producción usar librerías específicas
                return "127.0.0.1";
            }
            catch
            {
                return "Unknown";
            }
        }

        private string GetDeviceInfo()
        {
            try
            {
                return $"{DeviceInfo.Platform} {DeviceInfo.VersionString} - {DeviceInfo.Model}";
            }
            catch
            {
                return "Unknown Device";
            }
        }

        private async Task ExecuteForgotPasswordCommand()
        {
            await Shell.Current.DisplayAlert("Recuperar Contraseña", "Funcionalidad próximamente disponible", "OK");
        }

        private async Task ExecuteNavigateToRegisterCommand()
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }

        private async Task ExecuteExportLogsCommand()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Exportando logs...";

                var exportPath = await _fileService.ExportLogsToFileAsync();

                StatusMessage = "Logs exportados exitosamente";
                await Shell.Current.DisplayAlert("Exportar Logs", $"Logs exportados a: {Path.GetFileName(exportPath)}", "OK");
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "EXPORT_LOGS_ERROR", "Error al exportar logs", null, ex.ToString());
                StatusMessage = "Error al exportar logs";
                await Shell.Current.DisplayAlert("Error", "No se pudieron exportar los logs", "OK");
            }
            finally
            {
                IsLoading = false;
                StatusMessage = string.Empty;
            }
        }

        private async Task ExecuteViewLogsCommand()
        {
            try
            {
                var logs = await _logService.GetLogsAsync();
                var loginLogs = logs.Where(l => l.Operation.Contains("LOGIN")).Take(20).ToList();

                if (loginLogs.Any())
                {
                    var logText = string.Join("\n\n", loginLogs.Select(l =>
                        $"[{l.Timestamp:yyyy-MM-dd HH:mm:ss}] {l.Level}\n" +
                        $"Usuario: {l.UserEmail}\n" +
                        $"Mensaje: {l.Message}" +
                        (string.IsNullOrEmpty(l.ErrorDetails) ? "" : $"\nError: {l.ErrorDetails}")
                    ));

                    await Shell.Current.DisplayAlert("Logs de Login", logText, "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Logs", "No hay logs de login disponibles", "OK");
                }
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "VIEW_LOGS_ERROR", "Error al ver logs", null, ex.ToString());
                await Shell.Current.DisplayAlert("Error", "No se pudieron cargar los logs", "OK");
            }
        }

        public async Task LoadSavedCredentials()
        {
            try
            {
                var preferences = await _sessionService.GetUserPreferencesAsync();
                if (preferences.RememberPassword)
                {
                    var savedEmail = await SecureStorage.GetAsync("saved_email");
                    if (!string.IsNullOrEmpty(savedEmail))
                    {
                        Email = savedEmail;
                        RememberPassword = true;
                    }
                }

                // Verificar si hay una sesión activa
                var currentUser = await _sessionService.GetCurrentUserAsync();
                if (currentUser != null)
                {
                    StatusMessage = $"Sesión activa para {currentUser.Correo}";
                }
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "LOAD_CREDENTIALS_ERROR", "Error al cargar credenciales guardadas", null, ex.ToString());
            }
        }
    }
}