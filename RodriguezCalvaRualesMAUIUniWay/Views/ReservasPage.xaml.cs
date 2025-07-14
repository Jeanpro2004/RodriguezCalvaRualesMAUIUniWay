// Views/ReservasPage.xaml.cs
using RodriguezCalvaRualesMAUIUniWay.ViewModels;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class ReservasPage : ContentPage
    {
        public ReservasPage(ReservasViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is ReservasViewModel viewModel)
            {
                await viewModel.LoadReservasAsync();
            }
        }
    }
}