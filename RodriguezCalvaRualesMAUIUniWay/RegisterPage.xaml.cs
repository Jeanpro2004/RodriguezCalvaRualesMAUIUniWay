using System.Net.Http.Json;
using RodriguezCalvaRualesMAUIUniWay.API;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            RegisterButton.IsEnabled = false;

            // Validaciones
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
                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("http://localhost:5113/"); 

                var response = await httpClient.PostAsJsonAsync("api/Usuarios", user);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Éxito", "¡Cuenta creada exitosamente!", "OK");
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Error", $"Error al crear cuenta: {error}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo conectar al servidor: {ex.Message}", "OK");
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
            TermsCheckBox.IsChecked = false;
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            RegisterButton.IsEnabled = true;
        }
    }
}