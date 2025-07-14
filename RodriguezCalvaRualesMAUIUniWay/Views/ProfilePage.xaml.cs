using RodriguezCalvaRualesMAUIUniWay.API;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class ProfilePage : ContentPage
    {
        private readonly UsuarioService _usuarioService;
        private readonly VehiculoService _vehiculoService;
        private Usuario _usuario;
        private int _userId = SessionService.CurrentUserId;  

        public ProfilePage()
        {
            InitializeComponent();
            _usuarioService = new UsuarioService();
            _vehiculoService = new VehiculoService();
            LoadUserData();
        }

        private bool _isEditing = false;

        private void SetInputsEnabled(bool enabled)
        {
            NameEntry.IsEnabled = enabled;
            EmailEntry.IsEnabled = enabled;
            PhoneEntry.IsEnabled = enabled;
            IdBannerEntry.IsEnabled = enabled;
            PasswordEntry.IsEnabled = enabled;
            ConfirmPasswordEntry.IsEnabled = enabled;
            PassengerRadio.IsEnabled = enabled;
            DriverRadio.IsEnabled = enabled;

            VehicleBrandEntry.IsEnabled = enabled;
            VehicleModelEntry.IsEnabled = enabled;
            VehiclePlateEntry.IsEnabled = enabled;
        }

        private async void LoadUserData()
        {
            try
            {
                _usuario = await _usuarioService.GetUsuarioByIdAsync(_userId);

                NameEntry.Text = _usuario.Nombre;
                EmailEntry.Text = _usuario.Correo;
                PhoneEntry.Text = _usuario.Telefono.Replace("+593", "");
                PasswordEntry.Text = _usuario.Contrasena;
                ConfirmPasswordEntry.Text = _usuario.Contrasena;
                IdBannerEntry.Text = _usuario.IdBanner;

                PassengerRadio.IsChecked = !_usuario.EsConductor;
                DriverRadio.IsChecked = _usuario.EsConductor;

                var allCars = await _vehiculoService.GetVehiculosAsync();  
                var vehiculo = allCars.FirstOrDefault(v => v.ConductorId == _userId);

                if (vehiculo != null)
                {
                    VehicleBrandEntry.Text = vehiculo.Marca;
                    VehicleModelEntry.Text = vehiculo.Modelo;
                    VehicleColorEntry.Text = vehiculo.Color;
                    VehiclePlateEntry.Text = vehiculo.Placa;
                    VehicleSection.IsVisible = true;          
                }
                else
                {
                    VehicleBrandEntry.Text = "";
                    VehicleModelEntry.Text = "";
                    VehicleColorEntry.Text = "";
                    VehiclePlateEntry.Text = "";
                    VehicleSection.IsVisible = false;        
                }

                SetInputsEnabled(false);
                EditButton.IsVisible = true;
                UpdateButton.IsVisible = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo cargar el perfil: {ex.Message}", "OK");
            }
        }

        private void OnEditClicked(object sender, EventArgs e)
        {
            _isEditing = true;
            SetInputsEnabled(true);

            EditButton.IsVisible = false;
            UpdateButton.IsVisible = true;
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
