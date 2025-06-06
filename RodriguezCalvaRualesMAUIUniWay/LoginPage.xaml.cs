namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            LoginButton.IsEnabled = false;

            // Validaciones básicas
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

            // Simular proceso de login
            await Task.Delay(2000);

            // Aquí iría la lógica de autenticación real
            await DisplayAlert("Éxito", "¡Bienvenido a UniWay!", "OK");

            ResetLoadingState();
        }

        private void OnForgotPasswordTapped(object sender, EventArgs e)
        {
            DisplayAlert("Recuperación", "Función de recuperación de contraseña próximamente", "OK");
        }

        private async void OnRegisterTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }

        private void ResetLoadingState()
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            LoginButton.IsEnabled = true;
        }
    }
}