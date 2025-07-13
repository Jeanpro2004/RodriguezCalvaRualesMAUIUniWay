using RodriguezCalvaRualesMAUIUniWay.ViewModels;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = new ProfileViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Recargar datos del usuario al aparecer
            if (BindingContext is ProfileViewModel viewModel)
            {
                await viewModel.LoadUserData();
            }
        }

        private async void OnExportDataClicked(object sender, EventArgs e)
        {
            try
            {
                if (BindingContext is ProfileViewModel viewModel)
                {
                    var confirm = await DisplayAlert("Exportar Datos",
                        "¿Quieres exportar todos tus datos de UniWay?",
                        "Exportar", "Cancelar");

                    if (confirm)
                    {
                        // Aquí se implementaría la exportación
                        await DisplayAlert("Exportación",
                            "Funcionalidad de exportación estará disponible próximamente", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error exportando datos: {ex.Message}", "OK");
            }
        }

        private async void OnViewStatsClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Estadísticas",
                "Panel de estadísticas estará disponible próximamente", "OK");
        }
    }
}