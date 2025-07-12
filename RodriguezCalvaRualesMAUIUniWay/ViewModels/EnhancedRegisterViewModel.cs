using RodriguezCalvaRualesMAUIUniWay.Models;
using RodriguezCalvaRualesMAUIUniWay.Services;
using System.Windows.Input;

namespace RodriguezCalvaRualesMAUIUniWay.ViewModels
{
    public class EnhancedRegisterViewModel : BaseViewModel
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogService _logService;
        private readonly IFileManagementService _fileService;

        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private string _idBanner = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private bool _isDriver;
        private bool _acceptsTerms;
        private bool _isLoading;
        private string _statusMessage = string.Empty;
        private double _formProgress;

        public EnhancedRegisterViewModel(
            IUsuarioService usuarioService,
            ILogService logService,
            IFileManagementService fileService)
        {
            _usuarioService = usuarioService;
            _logService = logService;
            _fileService = fileService;
            Title = "Crear Cuenta";

            RegisterCommand = new Command(async () => await ExecuteRegisterCommand(), () => CanExecuteRegister());
            NavigateToLoginCommand = new Command(async () => await ExecuteNavigateToLoginCommand());
            SaveDraftCommand = new Command(async () => await ExecuteSaveDraftCommand());
            LoadDraftCommand = new Command(async () => await ExecuteLoadDraftCommand());

            // Calcular progreso del formulario cuando cambian los campos
            PropertyChanged += (s, e) => UpdateFormProgress();
        }

        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
                ((Command)RegisterCommand).ChangeCanExecute();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                SetProperty(ref _email, value);
                ((Command)RegisterCommand).ChangeCanExecute();
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                SetProperty(ref _phone, value);
                ((Command)RegisterCommand).ChangeCanExecute();
            }
        }

        public string IdBanner
        {
            get => _idBanner;
            set
            {
                SetProperty(ref _idBanner, value);
                ((Command)RegisterCommand).ChangeCanExecute();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                ((Command)RegisterCommand).ChangeCanExecute();
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                SetProperty(ref _confirmPassword, value);
                ((Command)RegisterCommand).ChangeCanExecute();
            }
        }

        public bool IsDriver
        {
            get => _isDriver;
            set => SetProperty(ref _isDriver, value);
        }

        public bool AcceptsTerms
        {
            get => _acceptsTerms;
            set
            {
                SetProperty(ref _acceptsTerms, value);
                ((Command)RegisterCommand).ChangeCanExecute();
            }
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

        public double FormProgress
        {
            get => _formProgress;
            set => SetProperty(ref _formProgress, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand NavigateToLoginCommand { get; }
        public ICommand SaveDraftCommand { get; }
        public ICommand LoadDraftCommand { get; }

        private void UpdateFormProgress()
        {
            var fields = new[]
            {
                !string.IsNullOrWhiteSpace(Name),
                !string.IsNullOrWhiteSpace(Email),
                !string.IsNullOrWhiteSpace(Phone),
                !string.IsNullOrWhiteSpace(IdBanner),
                !string.IsNullOrWhiteSpace(Password),
                !string.IsNullOrWhiteSpace(ConfirmPassword),
                AcceptsTerms
            };

            FormProgress = (double)fields.Count(f => f) / fields.Length;
        }

        private bool CanExecuteRegister()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Phone) &&
                   !string.IsNullOrWhiteSpace(IdBanner) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                   AcceptsTerms &&
                   !IsLoading;
        }

        private async Task ExecuteRegisterCommand()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                IsBusy = true;
                StatusMessage = "Creando cuenta...";

                // Guardar intento de registro
                await SaveRegistrationAttempt("ATTEMPT");

                // Validaciones adicionales
                if (Password != ConfirmPassword)
                {
                    StatusMessage = "Las contraseñas no coinciden";
                    await Shell.Current.DisplayAlert("Error", "Las contraseñas no coinciden", "OK");
                    await SaveRegistrationAttempt("FAILED", "Las contraseñas no coinciden");
                    return;
                }

                if (Password.Length < 8)
                {
                    StatusMessage = "La contraseña debe tener al menos 8 caracteres";
                    await Shell.Current.DisplayAlert("Error", "La contraseña debe tener al menos 8 caracteres", "OK");
                    await SaveRegistrationAttempt("FAILED", "Contraseña muy corta");
                    return;
                }

                if (!IsValidEmail(Email))
                {
                    StatusMessage = "Por favor ingrese un email válido";
                    await Shell.Current.DisplayAlert("Error", "Por favor ingrese un email válido", "OK");
                    await SaveRegistrationAttempt("FAILED", "Email inválido");
                    return;
                }

                var registerRequest = new RegisterRequest
                {
                    Nombre = Name.Trim(),
                    Correo = Email.Trim().ToLower(),
                    Telefono = Phone.Trim(),
                    IdBanner = IdBanner.Trim(),
                    Contrasena = Password,
                    ConfirmarContrasena = ConfirmPassword,
                    EsConductor = IsDriver,
                    AceptaTerminos = AcceptsTerms
                };

                var result = await _usuarioService.RegisterAsync(registerRequest);

                if (result.IsSuccess && result.Data != null)
                {
                    await SaveRegistrationAttempt("SUCCESS");

                    // Limpiar borrador guardado
                    await _fileService.DeleteFileAsync("registration_draft.json");

                    StatusMessage = "¡Cuenta creada exitosamente!";
                    await Shell.Current.DisplayAlert("Éxito", "Cuenta creada exitosamente", "OK");

                    // Limpiar campos
                    ClearForm();

                    // Navegar al login
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                else
                {
                    await SaveRegistrationAttempt("FAILED", result.Message);
                    StatusMessage = result.Message;
                    await Shell.Current.DisplayAlert("Error", result.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                await SaveRegistrationAttempt("ERROR", ex.Message);
                await _logService.LogAsync(LogLevel.Error, "REGISTER_VM_ERROR", "Error en RegisterViewModel", Email, ex.ToString());
                StatusMessage = "Ocurrió un error inesperado";
                await Shell.Current.DisplayAlert("Error", "Ocurrió un error inesperado", "OK");
            }
            finally
            {
                IsLoading = false;
                IsBusy = false;
            }
        }

        private async Task SaveRegistrationAttempt(string status, string? error = null)
        {
            try
            {
                var attempt = new RegistrationAttempt
                {
                    Email = Email,
                    Name = Name,
                    IdBanner = IdBanner,
                    IsDriver = IsDriver,
                    Timestamp = DateTime.Now,
                    Status = status,
                    ErrorMessage = error,
                    DeviceInfo = $"{DeviceInfo.Platform} {DeviceInfo.VersionString}"
                };

                var fileName = $"registration_attempts_{DateTime.Now:yyyyMM}.json";
                var existingAttempts = await _fileService.LoadUserDataAsync<List<RegistrationAttempt>>(fileName) ?? new List<RegistrationAttempt>();

                existingAttempts.Add(attempt);
                await _fileService.SaveUserDataAsync(fileName, existingAttempts);
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "SAVE_REGISTRATION_ATTEMPT_ERROR", "Error al guardar intento de registro", Email, ex.ToString());
            }
        }

        private async Task ExecuteSaveDraftCommand()
        {
            try
            {
                var draft = new RegistrationDraft
                {
                    Name = Name,
                    Email = Email,
                    Phone = Phone,
                    IdBanner = IdBanner,
                    IsDriver = IsDriver,
                    SavedAt = DateTime.Now
                };

                await _fileService.SaveUserDataAsync("registration_draft.json", draft);
                StatusMessage = "Borrador guardado";
                await Task.Delay(2000);
                StatusMessage = string.Empty;
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "SAVE_DRAFT_ERROR", "Error al guardar borrador", null, ex.ToString());
            }
        }

        private async Task ExecuteLoadDraftCommand()
        {
            try
            {
                var draft = await _fileService.LoadUserDataAsync<RegistrationDraft>("registration_draft.json");
                if (draft != null)
                {
                    Name = draft.Name;
                    Email = draft.Email;
                    Phone = draft.Phone;
                    IdBanner = draft.IdBanner;
                    IsDriver = draft.IsDriver;

                    StatusMessage = "Borrador cargado";
                    await Task.Delay(2000);
                    StatusMessage = string.Empty;
                }
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "LOAD_DRAFT_ERROR", "Error al cargar borrador", null, ex.ToString());
            }
        }

        private async Task ExecuteNavigateToLoginCommand()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void ClearForm()
        {
            Name = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            IdBanner = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            IsDriver = false;
            AcceptsTerms = false;
            FormProgress = 0;
        }

        public async Task InitializeAsync()
        {
            // Cargar borrador automáticamente si existe
            await ExecuteLoadDraftCommand();
        }
    }
}