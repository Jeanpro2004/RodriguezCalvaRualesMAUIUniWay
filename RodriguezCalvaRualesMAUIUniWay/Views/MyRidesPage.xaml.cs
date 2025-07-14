using RodriguezCalvaRualesMAUIUniWay.ViewModels;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class MyRidesPage : ContentPage
    {
        private MyRidesViewModel _viewModel;

        public MyRidesPage(MyRidesViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadDataAsync();
        }

        private async void OnSearchRidesClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SearchRidePage");
        }

        private async void OnContactDriverClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Contactar", "Funcionalidad próximamente disponible", "OK");
        }

        private async void OnReserveRideClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Reservar", "Funcionalidad próximamente disponible", "OK");
        }

        private async void OnCancelRideClicked(object sender, EventArgs e)
        {
            var result = await DisplayAlert("Cancelar Viaje", "¿Estás seguro de que quieres cancelar este viaje?", "Sí", "No");
            if (result)
            {
                await DisplayAlert("Cancelado", "Viaje cancelado exitosamente", "OK");
            }
        }

        private async void OnRideItemTapped(object sender, EventArgs e)
        {
            await DisplayAlert("Detalles", "Mostrar detalles del viaje", "OK");
        }

        private async void OnEarningsClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Ganancias", "Panel de ganancias próximamente", "OK");
        }

        private async void OnRideHistoryClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Historial", "Historial de viajes próximamente", "OK");
        }

        private async void OnRegisterVehicleClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//RegisterVehiclePage");
        }

    }
}
