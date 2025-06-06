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

            // Simular proceso de registro
            await Task.Delay(2000);

            await DisplayAlert("Éxito", "¡Cuenta creada exitosamente!", "OK");
            await Shell.Current.GoToAsync("//LoginPage");

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

            if (UniversityPicker.SelectedIndex == -1)
            {
                DisplayAlert("Error", "Por favor selecciona tu universidad", "OK");
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
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            RegisterButton.IsEnabled = true;
        }
    }
}