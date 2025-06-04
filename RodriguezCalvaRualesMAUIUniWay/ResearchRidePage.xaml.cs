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
            try
            {
                
                if (string.IsNullOrWhiteSpace(OriginEntry.Text) ||
                    string.IsNullOrWhiteSpace(DestinationEntry.Text))
                {
                    await DisplayAlert("Error", "Por favor completa origen y destino", "OK");
                    return;
                }

                
                await DisplayAlert("Búsqueda",
                    $"Buscando viajes de {OriginEntry.Text} a {DestinationEntry.Text}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error en la búsqueda: {ex.Message}", "OK");
            }
        }
    }
}

