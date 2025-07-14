using RodriguezCalvaRualesMAUIUniWay.ViewModels;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class SearchRidePage : ContentPage
    {
        private readonly SearchRideViewModel _viewModel;

        public SearchRidePage(SearchRideViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarTodosLosViajesAsync();
        }

        // Métodos para los filtros rápidos (si prefieres usar eventos en lugar de comandos)
        private void OnEconomicFilterTapped(object sender, EventArgs e)
        {
            _viewModel.FiltroActivo = "Económicos";
        }

        private void OnTopRatedFilterTapped(object sender, EventArgs e)
        {
            // Implementar filtro por valoración si tienes esa información
        }

        private void OnSpaciousFilterTapped(object sender, EventArgs e)
        {
            _viewModel.FiltroActivo = "Espaciosos";
        }

        private void OnEarlyDepartureFilterTapped(object sender, EventArgs e)
        {
            _viewModel.FiltroActivo = "Salida Temprana";
        }
    }
}