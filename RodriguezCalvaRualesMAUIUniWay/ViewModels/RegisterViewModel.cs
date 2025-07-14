using RodriguezCalvaRualesMAUIUniWay.API;
using RodriguezCalvaRualesMAUIUniWay.Services;
using System.Windows.Input;

namespace RodriguezCalvaRualesMAUIUniWay.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly AuthenticationService _authService;
        private readonly VehiculoService _vehiculoService;

        private string _nombre = string.Empty;
        private string _email = string.Empty;
        private string _telefono = string.Empty;
        private string _idBanner = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private bool _isDriver = false;
        private bool _acceptTerms = false;

        private string _marca = string.Empty;
        private string _modelo = string.Empty;
        private string _color = string.Empty;
        private string _placa = string.Empty;

        private bool _isFormValid = false;

        public RegisterViewModel(AuthenticationService authService, VehiculoService vehiculoService)
        {
            _authService = authService;
            _vehiculoService = vehiculoService;
            Title = "Crear Cuenta";

            RegisterCommand = new Command(async () => await ExecuteRegisterCommand());
            LoginCommand = new Command(async () => await Shell.Current.GoToAsync("//LoginPage"));
        }

        public string Nombre
        {
            get => _nombre;
            set
            {
                SetProperty(ref _nombre, value);
                EvaluateForm();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                SetProperty(ref _email, value);
                EvaluateForm();
            }
        }

        public string Telefono
        {
            get => _telefono;
            set
            {
                SetProperty(ref _telefono, value);
                EvaluateForm();
            }
        }

        public string IdBanner
        {
            get => _idBanner;
            set
            {
                SetProperty(ref _idBanner, value);
                EvaluateForm();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                EvaluateForm();
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                SetProperty(ref _confirmPassword, value);
                EvaluateForm();
            }
        }

        public bool IsDriver
        {
            get => _isDriver;
            set
            {
                SetProperty(ref _isDriver, value);
                EvaluateForm();
            }
        }

        public bool AcceptTerms
        {
            get => _acceptTerms;
            set
            {
                SetProperty(ref _acceptTerms, value);
                EvaluateForm();
            }
        }

        public string Marca
        {
            get => _marca;
            set
            {
                SetProperty(ref _marca, value);
                EvaluateForm();
            }
        }

        public string Modelo
        {
            get => _modelo;
            set
            {
                SetProperty(ref _modelo, value);
                EvaluateForm();
            }
        }

        public string Color
        {
            get => _color;
            set
            {
                SetProperty(ref _color, value);
                EvaluateForm();
            }
        }

        public string Placa
        {
            get => _placa;
            set
            {
                SetProperty(ref _placa, value);
                EvaluateForm();
            }
        }

        public bool IsFormValid
        {
            get => _isFormValid;
            private set => SetProperty(ref _isFormValid, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand LoginCommand { get; }

        private void EvaluateForm()
        {
            bool baseValid =
                !string.IsNullOrWhiteSpace(Nombre) &&
                !string.IsNullOrWhiteSpace(Email) &&
                !string.IsNullOrWhiteSpace(Telefono) &&
                !string.IsNullOrWhiteSpace(IdBanner) &&
                !string.IsNullOrWhiteSpace(Password) &&
                !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                AcceptTerms;

            if (IsDriver)
            {
                IsFormValid = baseValid &&
                              !string.IsNullOrWhiteSpace(Marca) &&
                              !string.IsNullOrWhiteSpace(Modelo) &&
                              !string.IsNullOrWhiteSpace(Color) &&
                              !string.IsNullOrWhiteSpace(Placa);
            }
            else
            {
                IsFormValid = baseValid;
            }
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
                    if (IsDriver)
                    {
                        var vehiculo = new Vehiculo
                        {
                            Marca = Marca,
                            Modelo = Modelo,
                            Color = Color,
                            Placa = Placa,
                            ConductorId = result.Usuario.Id
                        };

                        await _vehiculoService.CreateVehiculoAsync(vehiculo);
                    }

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
