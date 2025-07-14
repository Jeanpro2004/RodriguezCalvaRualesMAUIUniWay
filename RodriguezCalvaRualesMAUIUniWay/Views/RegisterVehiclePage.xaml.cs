using RodriguezCalvaRualesMAUIUniWay.ViewModels;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class RegisterVehiclePage : ContentPage
    {
        public RegisterVehiclePage(RegisterVehicleViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}