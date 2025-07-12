using RodriguezCalvaRualesMAUIUniWay.ViewModels;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(EnhancedLoginViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is EnhancedLoginViewModel viewModel)
            {
                await viewModel.LoadSavedCredentials();
            }
        }

        private async void OnForgotPasswordTapped(object sender, EventArgs e)
        {
            if (BindingContext is EnhancedLoginViewModel viewModel && viewModel.ForgotPasswordCommand.CanExecute(null))
            {
                await (Task)viewModel.ForgotPasswordCommand.Execute(null);
            }
        }

        private async void OnRegisterTapped(object sender, EventArgs e)
        {
            if (BindingContext is EnhancedLoginViewModel viewModel && viewModel.NavigateToRegisterCommand.CanExecute(null))
            {
                await (Task)viewModel.NavigateToRegisterCommand.Execute(null);
            }
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            if (BindingContext is EnhancedLoginViewModel viewModel && viewModel.LoginCommand.CanExecute(null))
            {
                await (Task)viewModel.LoginCommand.Execute(null);
            }
        }

        private async void OnExportLogsClicked(object sender, EventArgs e)
        {
            if (BindingContext is EnhancedLoginViewModel viewModel && viewModel.ExportLogsCommand.CanExecute(null))
            {
                await (Task)viewModel.ExportLogsCommand.Execute(null);
            }
        }

        private async void OnViewLogsClicked(object sender, EventArgs e)
        {
            if (BindingContext is EnhancedLoginViewModel viewModel && viewModel.ViewLogsCommand.CanExecute(null))
            {
                await (Task)viewModel.ViewLogsCommand.Execute(null);
            }
        }
    }
}