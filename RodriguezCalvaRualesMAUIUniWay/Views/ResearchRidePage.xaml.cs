using RodriguezCalvaRualesMAUIUniWay.ViewModels;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class SearchRidePage : ContentPage
    {
        public SearchRidePage(SearchRideViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private async void OnEconomicFilterTapped(object sender, EventArgs e)
        {
            await DisplayAlert("Filtro", "Mostrando viajes económicos", "OK");
        }

        private async void OnTopRatedFilterTapped(object sender, EventArgs e)
        {
            await DisplayAlert("Filtro", "Mostrando mejor valorados", "OK");
        }

        private async void OnSpaciousFilterTapped(object sender, EventArgs e)
        {
            await DisplayAlert("Filtro", "Mostrando viajes espaciosos", "OK");
        }

        private async void OnEarlyDepartureFilterTapped(object sender, EventArgs e)
        {
            await DisplayAlert("Filtro", "Mostrando salidas tempranas", "OK");
        }
    }
}
