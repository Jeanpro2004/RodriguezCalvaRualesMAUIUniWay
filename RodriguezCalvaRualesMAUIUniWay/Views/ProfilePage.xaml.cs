using RodriguezCalvaRualesMAUIUniWay.ViewModels;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class ProfilePage : ContentPage
    {
        private ProfileViewModel _viewModel;

        public ProfilePage(ProfileViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadUserProfile();
        }
    }
}