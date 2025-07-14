using RodriguezCalvaRualesMAUIUniWay.API;
using RodriguezCalvaRualesMAUIUniWay.Services;
using RodriguezCalvaRualesMAUIUniWay.Repositorios;
using System.Windows.Input;

namespace RodriguezCalvaRualesMAUIUniWay.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly AuthenticationService _authService;
        private readonly ManejoArchivosRepository _archivoRepository;

        private Usuario? _usuarioActual;
        private string _nombre = string.Empty;
        private string _email = string.Empty;
        private string _telefono = string.Empty;
        private string _idBanner = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private bool _isDriver;
        private bool _isLoading;

        public ProfileViewModel(AuthenticationService authService, ManejoArchivosRepository archivoRepository)
        {
            _authService = authService;
            _archivoRepository = archivoRepository;
            Title = "Mi Perfil";

            UpdateCommand = new Command(async () => await ExecuteUpdateCommand(), () => CanExecuteUpdate());
            LogoutCommand = new Command(async () => await ExecuteLogoutCommand());
            DeleteCommand = new Command(async () => await ExecuteDeleteCommand());
        }

        public Usuario UsuarioActual
        {
            get => _usuarioActual;
            set => SetProperty(ref _usuarioActual, value);
        }

        public string Nombre
        {
            get => _nombre;
            set
            {
                SetProperty(ref _nombre, value);
                ((Command)UpdateCommand).ChangeCanExecute();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                SetProperty(ref _email, value);
                ((Command)UpdateCommand).ChangeCanExecute();
            }
        }

        public string Telefono
        {
            get => _telefono;
            set
            {
                SetProperty(ref _telefono, value);
                ((Command)UpdateCommand).ChangeCanExecute();
            }
        }

        public string IdBanner
        {
            get => _idBanner;
            set
            {
                SetProperty(ref _idBanner, value);
                ((Command)UpdateCommand).ChangeCanExecute();
            }
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public bool IsDriver
        {
            get => _isDriver;
            set => SetProperty(ref _isDriver, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand UpdateCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand DeleteCommand { get; }

        private bool CanExecuteUpdate()
        {
            return !string.IsNullOrWhiteSpace(Nombre) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Telefono) &&
                   !string.IsNullOrWhiteSpace(IdBanner) &&
                   !IsLoading &&
                   UsuarioActual != null;
        }

        public async Task LoadUserProfile()
        {
            try
            {
                IsLoading = true;
                UsuarioActual = await _archivoRepository.ObtenerUsuarioActual();

                if (UsuarioActual != null)
                {
                    Nombre = UsuarioActual.Nombre;
                    Email = UsuarioActual.Correo;
                    Telefono = UsuarioActual.Telefono;
                    IdBanner = UsuarioActual.IdBanner;
                    IsDriver = UsuarioActual.EsConductor;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading profile: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo cargar el perfil", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ExecuteUpdateCommand()
        {
            if (IsLoading || UsuarioActual == null) return;

            try
            {
                IsLoading = true;

                // Validar contraseñas si se están cambiando
                if (!string.IsNullOrEmpty(Password))
                {
                    if (Password != ConfirmPassword)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Las contraseñas no coinciden", "OK");
                        return;
                    }

                    if (Password.Length < 8)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "La contraseña debe tener al menos 8 caracteres", "OK");
                        return;
                    }
                }

                var updatedUser = new Usuario
                {
                    Id = UsuarioActual.Id,
                    Nombre = Nombre.Trim(),
                    Correo = Email.Trim().ToLower(),
                    Telefono = Telefono.Trim(),
                    IdBanner = IdBanner.Trim(),
                    Contrasena = string.IsNullOrEmpty(Password) ? UsuarioActual.Contrasena : Password,
                    EsConductor = IsDriver
                };

                // Guardar en archivo local
                await _archivoRepository.GuardarUsuarioActual(updatedUser);
                await _archivoRepository.GuardarUsuarioLocal(updatedUser);

                UsuarioActual = updatedUser;
                Password = string.Empty;
                ConfirmPassword = string.Empty;

                await Application.Current.MainPage.DisplayAlert("Éxito", "Perfil actualizado correctamente", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating profile: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Ocurrió un error inesperado", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ExecuteLogoutCommand()
        {
            try
            {
                await _authService.LogoutAsync();
                await Application.Current.MainPage.DisplayAlert("Sesión Cerrada", "Has cerrado sesión exitosamente", "OK");
                await Shell.Current.GoToAsync("//HomePage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error logging out: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Error al cerrar sesión", "OK");
            }
        }

        private async Task ExecuteDeleteCommand()
        {
            var confirm = await Application.Current.MainPage.DisplayAlert("Confirmar",
                "¿Está seguro que desea eliminar su cuenta? Esta acción no se puede deshacer.",
                "Eliminar", "Cancelar");

            if (!confirm) return;

            try
            {
                IsLoading = true;
                await _archivoRepository.LimpiarTodosLosDatos();
                await Application.Current.MainPage.DisplayAlert("Cuenta Eliminada", "Su cuenta ha sido eliminada exitosamente", "OK");
                await Shell.Current.GoToAsync("//HomePage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting account: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Ocurrió un error inesperado", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}