using RodriguezCalvaRualesMAUIUniWay.ViewModels;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage(EnhancedRegisterViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is EnhancedRegisterViewModel viewModel)
            {
                await viewModel.InitializeAsync();
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            if (BindingContext is EnhancedRegisterViewModel viewModel && viewModel.RegisterCommand.CanExecute(null))
            {
                await (Task)viewModel.RegisterCommand.Execute(null);
            }
        }

        private async void OnLoginTapped(object sender, EventArgs e)
        {
            if (BindingContext is EnhancedRegisterViewModel viewModel && viewModel.NavigateToLoginCommand.CanExecute(null))
            {
                await (Task)viewModel.NavigateToLoginCommand.Execute(null);
            }
        }

        private async void OnSaveDraftClicked(object sender, EventArgs e)
        {
            if (BindingContext is EnhancedRegisterViewModel viewModel && viewModel.SaveDraftCommand.CanExecute(null))
            {
                await (Task)viewModel.SaveDraftCommand.Execute(null);
            }
        }

        private async void OnLoadDraftClicked(object sender, EventArgs e)
        {
            if (BindingContext is EnhancedRegisterViewModel viewModel && viewModel.LoadDraftCommand.CanExecute(null))
            {
                await (Task)viewModel.LoadDraftCommand.Execute(null);
            }
        }
    }
}