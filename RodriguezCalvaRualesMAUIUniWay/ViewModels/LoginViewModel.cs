using RodriguezCalvaRualesMAUIUniWay.API;
using RodriguezCalvaRualesMAUIUniWay.Services;
using System.Windows.Input;

namespace RodriguezCalvaRualesMAUIUniWay.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthenticationService _authService;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private bool _rememberPassword = false;

        public LoginViewModel(AuthenticationService authService)
        {
            _authService = authService;
            Title = "Iniciar Sesión";
            LoginCommand = new Command(async () => await ExecuteLoginCommand(), () => CanExecuteLogin());
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public bool RememberPassword
        {
            get => _rememberPassword;
            set => SetProperty(ref _rememberPassword, value);
        }

        public ICommand LoginCommand { get; }

        private bool CanExecuteLogin()
        {
            return !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password) && !IsBusy;
        }

        private async Task ExecuteLoginCommand()
        {
            try
            {
                IsBusy = true;
                var result = await _authService.LoginAsync(Email, Password);

                if (result.Success && result.Usuario != null)
                {
                    await Shell.Current.GoToAsync("//HomePage");
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