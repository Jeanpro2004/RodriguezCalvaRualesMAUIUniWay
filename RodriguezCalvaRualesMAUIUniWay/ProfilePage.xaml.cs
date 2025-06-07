using RodriguezCalvaRualesMAUIUniWay.API;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class ProfilePage : ContentPage
    {
        private readonly UsuarioService _usuarioService;
        private Usuario _usuario;
        private int _userId = 2; 

        public ProfilePage()
        {
            InitializeComponent();
            _usuarioService = new UsuarioService();
            LoadUserData();
        }

        private async void LoadUserData()
        {
            try
            {
                _usuario = await _usuarioService.GetUsuarioByIdAsync(_userId);
                NameEntry.Text = _usuario.Nombre;
                EmailEntry.Text = _usuario.Correo;
                PhoneEntry.Text = _usuario.Telefono.Replace("+593", ""); // Optional formatting
                PasswordEntry.Text = _usuario.Contrasena; // Consider security implications
                IdBannerEntry.Text = _usuario.IdBanner;
                PassengerRadio.IsChecked = _usuario.EsConductor;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo cargar el perfil: {ex.Message}", "OK");
            }
        }

        private async void OnUpdateClicked(object sender, EventArgs e)
        {
            try
            {
                var update = new Usuario
                {
                    Id = _userId,
                    Nombre = NameEntry.Text,
                    Correo = EmailEntry.Text,
                    Telefono = "+593" + PhoneEntry.Text,
                    IdBanner = IdBannerEntry.Text,
                    Contrasena = PasswordEntry.Text,
                    EsConductor = DriverRadio.IsChecked
                };

                await _usuarioService.UpdateUsuarioAsync(_userId, update);
                await DisplayAlert("Éxito", "Perfil actualizado correctamente", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo actualizar: {ex.Message}", "OK");
            }
        }


        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var confirm = await DisplayAlert("Confirmar", "¿Estás seguro de que deseas eliminar tu cuenta?", "Sí", "Cancelar");

            if (!confirm) return;

            try
            {
                await _usuarioService.DeleteUsuarioAsync(_userId);
                await DisplayAlert("Cuenta eliminada", "Tu cuenta ha sido eliminada", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo eliminar: {ex.Message}", "OK");
            }
        }
    }
}
