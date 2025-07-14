using RodriguezCalvaRualesMAUIUniWay.API;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class SearchRidePage : ContentPage
    {
        private readonly ViajeService _viajeService = new ViajeService();

        public SearchRidePage()
        {
            InitializeComponent();
            TravelDatePicker.Date = DateTime.Today;
        }

        private async void OnSearchClicked(object sender, EventArgs e)
        {
            if (!ValidateSearchForm())
                return;

            // feedback UI
            SearchButton.IsEnabled = false;
            SearchButton.Text = "Buscando…";
            EmptyLabel.IsVisible = false;
            ResultsCollection.IsVisible = false;

            try
            {
                // 1. Traer todos los viajes (o tu endpoint filtrado si lo tienes)
                var viajes = await _viajeService.GetViajesAsync();

                // 2. Normalizar criterios
                string origen = OriginEntry.Text.Trim().ToLowerInvariant();
                string destino = DestinationEntry.Text.Trim().ToLowerInvariant();
                DateTime fecha = TravelDatePicker.Date.Date;
                int pasajeros = PassengersPicker.SelectedIndex + 1; // 0=1 pasajero, etc.

                // 3. Filtrar
                var resultados = viajes
                    .Where(v =>
                        v.Origen.ToLowerInvariant().Contains(origen) &&
                        v.Destino.ToLowerInvariant().Contains(destino) &&
                        v.FechaHoraSalida.Date == fecha &&
                        v.AsientosDisponibles >= pasajeros)
                    .OrderBy(v => v.FechaHoraSalida)
                    .ToList();

                // 4. Mostrar resultados
                ResultsCollection.ItemsSource = resultados;
                ResultsCollection.IsVisible = resultados.Any();
                EmptyLabel.IsVisible = !resultados.Any();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo obtener la lista de viajes: {ex.Message}", "OK");
            }
            finally
            {
                SearchButton.IsEnabled = true;
                SearchButton.Text = "?? Buscar Viajes";
            }
        }

        private bool ValidateSearchForm()
        {
            if (string.IsNullOrWhiteSpace(OriginEntry.Text))
            {
                DisplayAlert("Error", "Por favor ingresa el punto de origen", "OK");
                return false;
            }
            if (string.IsNullOrWhiteSpace(DestinationEntry.Text))
            {
                DisplayAlert("Error", "Por favor ingresa el destino", "OK");
                return false;
            }
            if (TravelDatePicker.Date < DateTime.Today)
            {
                DisplayAlert("Error", "La fecha del viaje debe ser hoy o posterior", "OK");
                return false;
            }
            if (PassengersPicker.SelectedIndex == -1)
            {
                DisplayAlert("Error", "Por favor selecciona el número de pasajeros", "OK");
                return false;
            }
            return true;
        }
    }
}
