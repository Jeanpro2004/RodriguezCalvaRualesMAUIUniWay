using RodriguezCalvaRualesMAUIUniWay.Services;
using System.Windows.Input;

namespace RodriguezCalvaRualesMAUIUniWay.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly AuthenticationService _authService;
        private bool _isUserLoggedIn = false;
        private string _welcomeMessage = "¡Bienvenido a UniWay!";

        public HomeViewModel()
        {
            Title = "UniWay";
            StartAsPassengerCommand = new Command(async () => await StartAsPassenger());
            StartAsDriverCommand = new Command(async () => await StartAsDriver());
        }

        public bool IsUserLoggedIn
        {
            get => _isUserLoggedIn;
            set => SetProperty(ref _isUserLoggedIn, value);
        }

        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }

        public ICommand StartAsPassengerCommand { get; }
        public ICommand StartAsDriverCommand { get; }

        public async Task CheckUserStatus()
        {
            try
            {
                IsBusy = true;
                // Verificar si hay un archivo de sesión
                var appDataPath = FileSystem.AppDataDirectory;
                var sessionFile = Path.Combine(appDataPath, "usuario_actual.json");

                IsUserLoggedIn = File.Exists(sessionFile);

                if (IsUserLoggedIn)
                {
                    WelcomeMessage = "¡Bienvenido de nuevo!";
                }
                else
                {
                    WelcomeMessage = "¡Bienvenido a UniWay!";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking user status: {ex.Message}");
                IsUserLoggedIn = false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task StartAsPassenger()
        {
            if (IsUserLoggedIn)
            {
                await Shell.Current.GoToAsync("//SearchRidePage");
            }
            else
            {
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }

        private async Task StartAsDriver()
        {
            if (IsUserLoggedIn)
            {
                await Shell.Current.GoToAsync("//MyRidesPage");
            }
            else
            {
                await Shell.Current.GoToAsync("//RegisterPage");
            }
        }
    }
}
