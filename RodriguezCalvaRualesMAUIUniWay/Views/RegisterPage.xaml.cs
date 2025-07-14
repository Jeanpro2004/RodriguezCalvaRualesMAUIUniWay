using System.Net.Http.Json;
using RodriguezCalvaRualesMAUIUniWay.API;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class RegisterPage : ContentPage
    {
        private readonly UsuarioService _usuarioService = new UsuarioService();
        private readonly VehiculoService _vehiculoService = new VehiculoService();

        public RegisterPage()
        {
            InitializeComponent();
            DriverRadio.CheckedChanged += OnDriverCheckedChanged;

        }

        private void OnDriverCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            VehicleSection.IsVisible = e.Value; 
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            RegisterButton.IsEnabled = false;

            if (!ValidateForm())
            {
                ResetLoadingState();
                return;
            }

            var user = new Usuario
            {
                IdBanner = IdBannerEntry.Text,
                Nombre = NameEntry.Text,
                Correo = EmailEntry.Text,
                Telefono = "+593" + PhoneEntry.Text.Trim(),
                Contrasena = PasswordEntry.Text,
                EsConductor = DriverRadio.IsChecked
            };

            try
            {
                // Create user
                var createdUser = await _usuarioService.CreateUsuarioAsync(user);

                // If user is driver, create vehicle using the created user's Id
                if (DriverRadio.IsChecked)
                {
                    var vehiculo = new Vehiculo
                    {
                        Marca = MarcaEntry.Text,
                        Modelo = ModeloEntry.Text,
                        Color = ColorEntry.Text ?? string.Empty,
                        Placa = PlacaEntry.Text,
                        ConductorId = createdUser.Id
                    };

                    await _vehiculoService.CreateVehiculoAsync(vehiculo);
                }

                await DisplayAlert("Éxito", "¡Cuenta creada exitosamente!", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch (HttpRequestException httpEx)
            {
                await DisplayAlert("Error", $"Error en la petición HTTP: {httpEx.Message}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }

            ResetLoadingState();
        }

        private async void OnLoginTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text))
            {
                DisplayAlert("Error", "Por favor ingresa tu nombre completo", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(EmailEntry.Text))
            {
                DisplayAlert("Error", "Por favor ingresa tu email universitario", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(PhoneEntry.Text))
            {
                DisplayAlert("Error", "Por favor ingresa tu número de teléfono", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(IdBannerEntry.Text))
            {
                DisplayAlert("Error", "Por favor ingresa tu ID Banner", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(PasswordEntry.Text) || PasswordEntry.Text.Length < 8)
            {
                DisplayAlert("Error", "La contraseña debe tener al menos 8 caracteres", "OK");
                return false;
            }

            if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                DisplayAlert("Error", "Las contraseñas no coinciden", "OK");
                return false;
            }

            if (!TermsCheckBox.IsChecked)
            {
                DisplayAlert("Error", "Debes aceptar los términos y condiciones", "OK");
                return false;
            }

            if (DriverRadio.IsChecked)
            {
                if (string.IsNullOrWhiteSpace(MarcaEntry.Text))
                {
                    DisplayAlert("Error", "Por favor ingresa la marca del vehículo", "OK");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(ModeloEntry.Text))
                {
                    DisplayAlert("Error", "Por favor ingresa el modelo del vehículo", "OK");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(PlacaEntry.Text))
                {
                    DisplayAlert("Error", "Por favor ingresa la placa del vehículo", "OK");
                    return false;
                }
                if (string.IsNullOrEmpty(ColorEntry.Text))
                {
                    DisplayAlert("Error", "Por favor ingresa el color del vehículo", "OK");
                    return false;
                }
            }

            return true;
        }

        private void ResetLoadingState()
        {
            NameEntry.Text = string.Empty;
            EmailEntry.Text = string.Empty;
            PhoneEntry.Text = string.Empty;
            IdBannerEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            ConfirmPasswordEntry.Text = string.Empty;
            DriverRadio.IsChecked = false;
            PassengerRadio.IsChecked = true;
            TermsCheckBox.IsChecked = false;

            MarcaEntry.Text = string.Empty;
            ModeloEntry.Text = string.Empty;
            ColorEntry.Text = string.Empty;
            PlacaEntry.Text = string.Empty;

            VehicleSection.IsVisible = false;

            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            RegisterButton.IsEnabled = true;
        }

    }
}