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

            try
            {
                
                if (string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                    string.IsNullOrWhiteSpace(PasswordEntry.Text))
                {
                    await DisplayAlert("Error", "Por favor completa todos los campos", "OK");
                    return;
                }

               
                await Task.Delay(2000); 

                
                await DisplayAlert("�xito", "Inicio de sesi�n exitoso", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al iniciar sesi�n: {ex.Message}", "OK");
            }
            finally
            {
                
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                LoginButton.IsEnabled = true;
            }
        }

        private async void OnForgotPasswordTapped(object sender, EventArgs e)
        {
            await DisplayAlert("Recuperar Contrase�a",
                "Se ha enviado un enlace de recuperaci�n a tu email", "OK");
        }

        private async void OnRegisterTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }
    }
}