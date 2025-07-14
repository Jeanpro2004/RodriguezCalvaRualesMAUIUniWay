using RodriguezCalvaRualesMAUIUniWay.ViewModels;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class LoginPage : ContentPage
    {
        private LoginViewModel _viewModel;

        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            _viewModel.Email = EmailEntry.Text ?? string.Empty;
            _viewModel.Password = PasswordEntry.Text ?? string.Empty;
            _viewModel.RememberPassword = RememberCheckBox.IsChecked;

            if (_viewModel.LoginCommand.CanExecute(null))
            {
                _viewModel.LoginCommand.Execute(null);
            }
        }

        private async void OnForgotPasswordTapped(object sender, EventArgs e)
        {
            await DisplayAlert("Recuperar Contraseña", "Funcionalidad próximamente disponible", "OK");
        }

        private async void OnRegisterTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }
    }
}