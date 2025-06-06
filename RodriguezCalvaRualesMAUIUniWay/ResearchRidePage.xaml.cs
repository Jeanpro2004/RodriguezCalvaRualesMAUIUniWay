namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class SearchRidePage : ContentPage
    {
        public SearchRidePage()
        {
            InitializeComponent();
            TravelDatePicker.Date = DateTime.Today;
        }

        private async void OnSearchClicked(object sender, EventArgs e)
        {
            if (!ValidateSearchForm())
                return;

            SearchButton.IsEnabled = false;
            SearchButton.Text = "Buscando...";

            // Simular búsqueda
            await Task.Delay(1500);

            await DisplayAlert("Búsqueda",
                $"Buscando viajes de {OriginEntry.Text} a {DestinationEntry.Text} para {TravelDatePicker.Date:dd/MM/yyyy}",
                "OK");

            SearchButton.IsEnabled = true;
            SearchButton.Text = "?? Buscar Viajes";
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
