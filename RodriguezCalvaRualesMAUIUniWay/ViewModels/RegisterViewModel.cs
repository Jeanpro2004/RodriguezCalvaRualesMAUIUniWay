using RodriguezCalvaRualesMAUIUniWay.API;
using RodriguezCalvaRualesMAUIUniWay.Services;
using System.Windows.Input;

namespace RodriguezCalvaRualesMAUIUniWay.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly AuthenticationService _authService;
        private string _nombre = string.Empty;
        private string _email = string.Empty;
        private string _telefono = string.Empty;
        private string _idBanner = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private bool _isDriver = false;
        private bool _acceptTerms = false;

        public RegisterViewModel(AuthenticationService authService)
        {
            _authService = authService;
            Title = "Crear Cuenta";
            RegisterCommand = new Command(async () => await ExecuteRegisterCommand(), () => CanExecuteRegister());
            LoginCommand = new Command(async () => await Shell.Current.GoToAsync("//LoginPage"));
        }

        public string Nombre
        {
            get => _nombre;
            set => SetProperty(ref _nombre, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Telefono
        {
            get => _telefono;
            set => SetProperty(ref _telefono, value);
        }

        public string IdBanner
        {
            get => _idBanner;
            set => SetProperty(ref _idBanner, value);
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

        public bool AcceptTerms
        {
            get => _acceptTerms;
            set => SetProperty(ref _acceptTerms, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand LoginCommand { get; }

        private bool CanExecuteRegister()
        {
            return !string.IsNullOrWhiteSpace(Nombre) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Telefono) &&
                   !string.IsNullOrWhiteSpace(IdBanner) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                   AcceptTerms &&
                   !IsBusy;
        }

        private async Task ExecuteRegisterCommand()
        {
            try
            {
                IsBusy = true;

                if (Password != ConfirmPassword)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Las contraseñas no coinciden", "OK");
                    return;
                }

                var usuario = new Usuario
                {
                    Nombre = Nombre,
                    Correo = Email,
                    Telefono = Telefono,
                    IdBanner = IdBanner,
                    Contrasena = Password,
                    EsConductor = IsDriver
                };

                var result = await _authService.RegisterAsync(usuario);

                if (result.Success)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", "Cuenta creada exitosamente", "OK");
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", result.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}