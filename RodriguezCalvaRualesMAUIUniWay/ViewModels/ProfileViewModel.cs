using RodriguezCalvaRualesMAUIUniWay.API;
using RodriguezCalvaRualesMAUIUniWay.Models;
using RodriguezCalvaRualesMAUIUniWay.Services;
using System.Windows.Input;

namespace RodriguezCalvaRualesMAUIUniWay.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogService _logService;

        private Usuario? _currentUser;
        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private string _idBanner = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private bool _isDriver;
        private bool _isLoading;

        public ProfileViewModel(IUsuarioService usuarioService, ILogService logService)
        {
            _usuarioService = usuarioService;
            _logService = logService;
            Title = "Mi Perfil";

            UpdateProfileCommand = new Command(async () => await ExecuteUpdateProfileCommand(), () => CanExecuteUpdate());
            DeleteAccountCommand = new Command(async () => await ExecuteDeleteAccountCommand());
            ViewLogsCommand = new Command(async () => await ExecuteViewLogsCommand());
        }

        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
                ((Command)UpdateProfileCommand).ChangeCanExecute();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                SetProperty(ref _email, value);
                ((Command)UpdateProfileCommand).ChangeCanExecute();
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                SetProperty(ref _phone, value);
                ((Command)UpdateProfileCommand).ChangeCanExecute();
            }
        }

        public string IdBanner
        {
            get => _idBanner;
            set
            {
                SetProperty(ref _idBanner, value);
                ((Command)UpdateProfileCommand).ChangeCanExecute();
            }
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public bool IsDriver
        {
            get => _isDriver;
            set => SetProperty(ref _isDriver, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand UpdateProfileCommand { get; }
        public ICommand DeleteAccountCommand { get; }
        public ICommand ViewLogsCommand { get; }

        private bool CanExecuteUpdate()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Phone) &&
                   !string.IsNullOrWhiteSpace(IdBanner) &&
                   !IsLoading &&
                   _currentUser != null;
        }

        public async Task LoadUserProfile()
        {
            try
            {
                IsLoading = true;

                var userIdString = await SecureStorage.GetAsync("user_id");
                if (int.TryParse(userIdString, out int userId))
                {
                    var result = await _usuarioService.GetUsuarioByIdAsync(userId);
                    if (result.IsSuccess && result.Data != null)
                    {
                        _currentUser = result.Data;
                        Name = _currentUser.Nombre;
                        Email = _currentUser.Correo;
                        Phone = _currentUser.Telefono;
                        IdBanner = _currentUser.IdBanner;
                        IsDriver = _currentUser.EsConductor;
                    }
                }
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "LOAD_PROFILE_ERROR", "Error al cargar perfil", null, ex.ToString());
                await Shell.Current.DisplayAlert("Error", "No se pudo cargar el perfil", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ExecuteUpdateProfileCommand()
        {
            if (IsLoading || _currentUser == null) return;

            try
            {
                IsLoading = true;

                // Validar contraseñas si se están cambiando
                if (!string.IsNullOrEmpty(Password))
                {
                    if (Password != ConfirmPassword)
                    {
                        await Shell.Current.DisplayAlert("Error", "Las contraseñas no coinciden", "OK");
                        return;
                    }

                    if (Password.Length < 8)
                    {
                        await Shell.Current.DisplayAlert("Error", "La contraseña debe tener al menos 8 caracteres", "OK");
                        return;
                    }
                }

                var updatedUser = new Usuario
                {
                    Id = _currentUser.Id,
                    Nombre = Name.Trim(),
                    Correo = Email.Trim().ToLower(),
                    Telefono = Phone.Trim(),
                    IdBanner = IdBanner.Trim(),
                    Contrasena = string.IsNullOrEmpty(Password) ? _currentUser.Contrasena : Password,
                    EsConductor = IsDriver
                };

                var result = await _usuarioService.UpdateUsuarioAsync(_currentUser.Id, updatedUser);

                if (result.IsSuccess)
                {
                    _currentUser = updatedUser;
                    Password = string.Empty;
                    ConfirmPassword = string.Empty;

                    // Actualizar datos en SecureStorage
                    await SecureStorage.SetAsync("user_email", updatedUser.Correo);
                    await SecureStorage.SetAsync("is_driver", updatedUser.EsConductor.ToString());

                    await Shell.Current.DisplayAlert("Éxito", "Perfil actualizado correctamente", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", result.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "UPDATE_PROFILE_ERROR", "Error al actualizar perfil", Email, ex.ToString());
                await Shell.Current.DisplayAlert("Error", "Ocurrió un error inesperado", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ExecuteDeleteAccountCommand()
        {
            if (_currentUser == null) return;

            var confirm = await Shell.Current.DisplayAlert("Confirmar",
                "¿Está seguro que desea eliminar su cuenta? Esta acción no se puede deshacer.",
                "Eliminar", "Cancelar");

            if (!confirm) return;

            try
            {
                IsLoading = true;

                var result = await _usuarioService.DeleteUsuarioAsync(_currentUser.Id);

                if (result.IsSuccess)
                {
                    // Limpiar datos almacenados
                    SecureStorage.RemoveAll();

                    await Shell.Current.DisplayAlert("Cuenta Eliminada", "Su cuenta ha sido eliminada exitosamente", "OK");

                    // Navegar al login
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", result.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "DELETE_ACCOUNT_ERROR", "Error al eliminar cuenta", Email, ex.ToString());
                await Shell.Current.DisplayAlert("Error", "Ocurrió un error inesperado", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ExecuteViewLogsCommand()
        {
            try
            {
                var logs = await _logService.GetLogsAsync();
                var userEmail = await SecureStorage.GetAsync("user_email");

                // Filtrar logs del usuario actual
                var userLogs = logs.Where(l => l.UserEmail == userEmail).Take(50).ToList();

                if (userLogs.Any())
                {
                    var logText = string.Join("\n\n", userLogs.Select(l =>
                        $"[{l.Timestamp:yyyy-MM-dd HH:mm:ss}] {l.Level}\n" +
                        $"Operación: {l.Operation}\n" +
                        $"Mensaje: {l.Message}" +
                        (string.IsNullOrEmpty(l.ErrorDetails) ? "" : $"\nError: {l.ErrorDetails}")
                    ));

                    await Shell.Current.DisplayAlert("Mis Logs", logText, "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Logs", "No hay logs disponibles para este usuario", "OK");
                }
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "VIEW_LOGS_ERROR", "Error al ver logs", null, ex.ToString());
                await Shell.Current.DisplayAlert("Error", "No se pudieron cargar los logs", "OK");
            }
        }
    }
}