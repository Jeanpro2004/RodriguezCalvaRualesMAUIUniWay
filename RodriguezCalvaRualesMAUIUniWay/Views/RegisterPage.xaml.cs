using RodriguezCalvaRualesMAUIUniWay.ViewModels;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
            BindingContext = new RegisterViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Limpiar campos al aparecer (por si vuelve desde login)
            if (BindingContext is RegisterViewModel viewModel)
            {
                // Los campos se manejan automáticamente por el ViewModel
            }
        }
    }
}