using RodriguezCalvaRualesMAUIUniWay.ViewModels;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class MyRidesPage : ContentPage
    {
        public MyRidesPage()
        {
            InitializeComponent();
            BindingContext = new MyRidesViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Refrescar datos al aparecer
            if (BindingContext is MyRidesViewModel viewModel)
            {
                if (viewModel.RefreshCommand.CanExecute(null))
                {
                    viewModel.RefreshCommand.Execute(null);
                }
            }
        }

        private async void OnSearchRidesClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SearchRidePage");
        }

        private async void OnCreateRideClicked(object sender, EventArgs e)
        {
            if (BindingContext is MyRidesViewModel viewModel)
            {
                if (viewModel.CreateRideCommand.CanExecute(null))
                {
                    viewModel.CreateRideCommand.Execute(null);
                }
            }
        }

        private async void OnRideItemTapped(object sender, EventArgs e)
        {
            try
            {
                if (sender is Frame frame && frame.BindingContext != null)
                {
                    // Aquí se manejaría la navegación a detalles del viaje
                    await DisplayAlert("Detalle de Viaje",
                        "Vista de detalles estará disponible próximamente", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error mostrando detalles: {ex.Message}", "OK");
            }
        }

        private async void OnReserveRideClicked(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button button && button.BindingContext != null)
                {
                    var confirm = await DisplayAlert("Confirmar Reserva",
                        "¿Quieres reservar un lugar en este viaje?",
                        "Reservar", "Cancelar");

                    if (confirm)
                    {
                        await DisplayAlert("Reserva Confirmada",
                            "¡Tu reserva ha sido confirmada! El conductor se pondrá en contacto contigo.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error realizando reserva: {ex.Message}", "OK");
            }
        }

        private async void OnCancelRideClicked(object sender, EventArgs e)
        {
            try
            {
                var confirm = await DisplayAlert("Cancelar Viaje",
                    "¿Estás seguro de que quieres cancelar este viaje?",
                    "Cancelar Viaje", "No");

                if (confirm)
                {
                    await DisplayAlert("Viaje Cancelado",
                        "El viaje ha sido cancelado. Se notificará a los pasajeros.", "OK");

                    // Refrescar lista
                    if (BindingContext is MyRidesViewModel viewModel)
                    {
                        if (viewModel.RefreshCommand.CanExecute(null))
                        {
                            viewModel.RefreshCommand.Execute(null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error cancelando viaje: {ex.Message}", "OK");
            }
        }

        private async void OnContactDriverClicked(object sender, EventArgs e)
        {
            try
            {
                var options = await DisplayActionSheet("Contactar Conductor",
                    "Cancelar", null, "WhatsApp", "Llamar", "Email");

                switch (options)
                {
                    case "WhatsApp":
                        await DisplayAlert("WhatsApp",
                            "Abriendo WhatsApp...", "OK");
                        break;
                    case "Llamar":
                        await DisplayAlert("Llamar",
                            "Iniciando llamada...", "OK");
                        break;
                    case "Email":
                        await DisplayAlert("Email",
                            "Abriendo email...", "OK");
                        break;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error contactando conductor: {ex.Message}", "OK");
            }
        }

        private async void OnRideHistoryClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Historial",
                "Historial de viajes estará disponible próximamente", "OK");
        }

        private async void OnEarningsClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Ganancias",
                "Resumen de ganancias estará disponible próximamente", "OK");
        }
    }
}