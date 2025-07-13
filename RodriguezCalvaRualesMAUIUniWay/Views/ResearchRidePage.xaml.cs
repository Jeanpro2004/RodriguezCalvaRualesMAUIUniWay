using RodriguezCalvaRualesMAUIUniWay.ViewModels;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class SearchRidePage : ContentPage
    {
        public SearchRidePage()
        {
            InitializeComponent();
            BindingContext = new SearchRideViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Configurar fecha mínima al día actual
            if (BindingContext is SearchRideViewModel viewModel)
            {
                if (viewModel.TravelDate < DateTime.Today)
                {
                    viewModel.TravelDate = DateTime.Today;
                }
            }
        }

        private async void OnEconomicFilterTapped(object sender, EventArgs e)
        {
            await ApplyFilter("económico");
        }

        private async void OnTopRatedFilterTapped(object sender, EventArgs e)
        {
            await ApplyFilter("mejor valorado");
        }

        private async void OnSpaciousFilterTapped(object sender, EventArgs e)
        {
            await ApplyFilter("espacioso");
        }

        private async void OnEarlyDepartureFilterTapped(object sender, EventArgs e)
        {
            await ApplyFilter("salida temprana");
        }

        private async Task ApplyFilter(string filterType)
        {
            try
            {
                await DisplayAlert("Filtro Aplicado",
                    $"Buscando viajes {filterType}...", "OK");

                // Aquí se implementaría la lógica de filtrado
                if (BindingContext is SearchRideViewModel viewModel)
                {
                    if (viewModel.SearchCommand.CanExecute(null))
                    {
                        viewModel.SearchCommand.Execute(null);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error aplicando filtro: {ex.Message}", "OK");
            }
        }

        private async void OnAdvancedSearchClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Búsqueda Avanzada",
                "Opciones avanzadas de búsqueda estarán disponibles próximamente", "OK");
        }
    }
}
