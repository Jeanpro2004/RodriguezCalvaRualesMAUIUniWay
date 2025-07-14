using RodriguezCalvaRualesMAUIUniWay.ViewModels;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            BindingContext = new HomeViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is HomeViewModel viewModel)
            {
                await viewModel.CheckUserStatus();
            }
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ProfilePage");
        }
    }
}
