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

            // Validaciones b�sicas
            if (string.IsNullOrWhiteSpace(EmailEntry.Text))
            {
                await DisplayAlert("Error", "Por favor ingresa tu email", "OK");
                ResetLoadingState();
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                await DisplayAlert("Error", "Por favor ingresa tu contrase�a", "OK");
                ResetLoadingState();
                return;
            }

            // Simular proceso de login
            await Task.Delay(2000);

            // Aqu� ir�a la l�gica de autenticaci�n real
            await DisplayAlert("�xito", "�Bienvenido a UniWay!", "OK");

            ResetLoadingState();
        }

        private void OnForgotPasswordTapped(object sender, EventArgs e)
        {
            DisplayAlert("Recuperaci�n", "Funci�n de recuperaci�n de contrase�a pr�ximamente", "OK");
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