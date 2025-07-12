using RodriguezCalvaRualesMAUIUniWay.Services;
using RodriguezCalvaRualesMAUIUniWay.Models;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class HomePage : ContentPage
    {
        private readonly IUserSessionService _sessionService;
        private readonly ILogService _logService;

        public HomePage(IUserSessionService sessionService, ILogService logService)
        {
            InitializeComponent();
            _sessionService = sessionService;
            _logService = logService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CheckUserSession();
        }

        private async Task CheckUserSession()
        {
            try
            {
                var currentUser = await _sessionService.GetCurrentUserAsync();
                if (currentUser != null)
                {
                    // Mostrar información personalizada si hay sesión activa
                    await _logService.LogAsync(LogLevel.Info, "HOME_VISIT", $"Usuario activo visitó home: {currentUser.Correo}", currentUser.Correo);

                    // Aquí podrías personalizar la UI basada en el usuario
                    if (currentUser.EsConductor)
                    {
                        // Mostrar opciones específicas para conductores
                    }
                }
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "HOME_SESSION_CHECK_ERROR", "Error al verificar sesión en home", null, ex.ToString());
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            // Navegar como pasajero
            await Shell.Current.GoToAsync("//SearchRidePage");
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            // Registrarse como conductor
            await Shell.Current.GoToAsync("//RegisterPage");
        }
    }
}