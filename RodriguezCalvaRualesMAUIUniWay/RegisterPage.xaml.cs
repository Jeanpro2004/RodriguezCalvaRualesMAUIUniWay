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

            try
            {
                
                if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
                    string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                    string.IsNullOrWhiteSpace(PhoneEntry.Text) ||
                    string.IsNullOrWhiteSpace(PasswordEntry.Text) ||
                    string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text) ||
                    UniversityPicker.SelectedIndex == -1)
                {
                    await DisplayAlert("Error", "Por favor completa todos los campos", "OK");
                    return;
                }

                
                if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
                {
                    await DisplayAlert("Error", "Las contraseñas no coinciden", "OK");
                    return;
                }

                
                if (!TermsCheckBox.IsChecked)
                {
                    await DisplayAlert("Error", "Debes aceptar los términos y condiciones", "OK");
                    return;
                }

                
                if (!EmailEntry.Text.Contains("@") || !EmailEntry.Text.EndsWith(".edu.ec"))
                {
                    await DisplayAlert("Error", "Ingresa un email universitario válido", "OK");
                    return;
                }

                
                await Task.Delay(2000);

                await DisplayAlert("Éxito", "Cuenta creada exitosamente", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al crear la cuenta: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                RegisterButton.IsEnabled = true;
            }
        }

        private async void OnLoginTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}