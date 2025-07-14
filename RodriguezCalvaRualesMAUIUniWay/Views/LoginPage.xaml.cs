using RodriguezCalvaRualesMAUIUniWay.API;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly UsuarioService _usuarioService = new UsuarioService();

        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            LoginButton.IsEnabled = false;

            // Basic validations
            if (string.IsNullOrWhiteSpace(EmailEntry.Text))
            {
                await DisplayAlert("Error", "Por favor ingresa tu email", "OK");
                ResetLoadingState();
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                await DisplayAlert("Error", "Por favor ingresa tu contraseña", "OK");
                ResetLoadingState();
                return;
            }

            // Hardcoded test user
            var testEmail = "mathias.david@udla.edu.ec";  // Replace with your known email
            var testPassword = "1234567890";        // Replace with your known password

            // Compare trimmed strings to avoid whitespace issues
            if (EmailEntry.Text.Trim().Equals(testEmail, StringComparison.OrdinalIgnoreCase) &&
                PasswordEntry.Text.Trim() == testPassword)
            {
                // Simulate login success
                await DisplayAlert("Éxito", $"¡Bienvenido, usuario de prueba!", "OK");
                // TODO: Navigate to home page or main app page
                await Shell.Current.GoToAsync("//HomePage");
            }
            else
            {
                await DisplayAlert("Error", "Correo o contraseña incorrectos", "OK");
            }

            ResetLoadingState();
        }


        private void ResetLoadingState()
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            LoginButton.IsEnabled = true;
        }

        private void OnForgotPasswordTapped(object sender, EventArgs e)
        {
            DisplayAlert("Recuperación", "Función de recuperación de contraseña próximamente", "OK");
        }

        private async void OnRegisterTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }
    }
}
