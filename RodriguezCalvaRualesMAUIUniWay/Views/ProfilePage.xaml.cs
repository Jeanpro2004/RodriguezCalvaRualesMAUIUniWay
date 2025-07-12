using RodriguezCalvaRualesMAUIUniWay.ViewModels;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage(ProfileViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is ProfileViewModel viewModel)
            {
                await viewModel.LoadUserProfile();
            }
        }

        private async void OnUpdateClicked(object sender, EventArgs e)
        {
            if (BindingContext is ProfileViewModel viewModel)
            {
                await (Task)viewModel.UpdateProfileCommand.Execute(null);
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (BindingContext is ProfileViewModel viewModel)
            {
                await (Task)viewModel.DeleteAccountCommand.Execute(null);
            }
        }

        private async void OnViewLogsClicked(object sender, EventArgs e)
        {
            if (BindingContext is ProfileViewModel viewModel)
            {
                await (Task)viewModel.ViewLogsCommand.Execute(null);
            }
        }
    }
}