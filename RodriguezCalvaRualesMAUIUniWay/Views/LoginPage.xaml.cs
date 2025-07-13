using RodriguezCalvaRualesMAUIUniWay.ViewModels;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Cargar credenciales guardadas si existen
            if (BindingContext is LoginViewModel viewModel)
            {
                // Las credenciales se cargan automáticamente en el constructor del ViewModel
            }
        }

        protected override bool OnBackButtonPressed()
        {
            // Permitir salir de la app desde login
            return false;
        }
    }
}