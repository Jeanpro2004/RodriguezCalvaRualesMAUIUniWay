// Views/SearchRidePage.xaml.cs
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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is SearchRideViewModel viewModel)
            {
                await viewModel.LoadViajesAsync();
            }
        }
    }
}