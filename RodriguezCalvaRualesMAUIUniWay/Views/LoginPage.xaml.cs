using RodriguezCalvaRualesMAUIUniWay.API;
using RodriguezCalvaRualesMAUIUniWay.Interfaces; 
using RodriguezCalvaRualesMAUIUniWay.Models; 

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly UsuarioService _usuarioService;
        private readonly ILoginAttemptService _loginAttemptService; 

        
        public LoginPage()
        {
            InitializeComponent();
            _usuarioService = new UsuarioService();

            // Obtener el servicio desde el contenedor DI
            _loginAttemptService = Handler?.MauiContext?.Services?.GetService<ILoginAttemptService>();
        }

        // MÉTODO OnLoginClicked 
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            if (LoadingIndicator.IsRunning)
                return;

            var email = EmailEntry.Text?.Trim();
            var password = PasswordEntry.Text?.Trim();

            // Validaciones básicas
            if (string.IsNullOrEmpty(email))
            {
                await DisplayAlert("Error", "Por favor ingresa tu email.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Por favor ingresa tu contraseña.", "OK");
                return;
            }

            // Verificar si la cuenta está bloqueada
            if (_loginAttemptService != null)
            {
                try
                {
                    var isLocked = await _loginAttemptService.IsAccountLockedAsync(email, 5, 15);
                    if (isLocked)
                    {
                        await DisplayAlert("Cuenta Bloqueada",
                            "Tu cuenta ha sido bloqueada temporalmente debido a múltiples intentos fallidos. " +
                            "Intenta nuevamente en 15 minutos.", "OK");

                        // Guardar intento bloqueado
                        var blockedAttempt = new LoginAttempt(email, false, "Cuenta bloqueada por múltiples intentos fallidos");
                        await _loginAttemptService.SaveLoginAttemptAsync(blockedAttempt);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    // Log error pero continuar con el login
                    System.Diagnostics.Debug.WriteLine($"Error verificando bloqueo: {ex.Message}");
                }
            }

            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            LoginButton.IsEnabled = false;

            try
            {
                // Verificar credenciales
                var usuarios = await _usuarioService.GetUsuariosAsync();
                var usuario = usuarios?.FirstOrDefault(u =>
                    string.Equals(u.Correo, email, StringComparison.OrdinalIgnoreCase) &&
                    u.Contrasena == password);

                if (usuario != null)
                {
                    // Login exitoso
                    SessionService.CurrentUserId = usuario.Id;

                    // Guardar intento exitoso
                    if (_loginAttemptService != null)
                    {
                        try
                        {
                            var successAttempt = new LoginAttempt(email, true, "Login exitoso");
                            await _loginAttemptService.SaveLoginAttemptAsync(successAttempt);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error guardando intento exitoso: {ex.Message}");
                        }
                    }

                    await DisplayAlert("Éxito", $"¡Bienvenido, {usuario.Nombre}!", "OK");

                    // Limpiar campos
                    EmailEntry.Text = "";
                    PasswordEntry.Text = "";

                    // Navegar a página principal o actualizar UI
                    await Shell.Current.GoToAsync("//SearchRidePage");
                }
                else
                {
                    // Login fallido
                    string errorMessage = "Email o contraseña incorrectos.";

                    // Guardar intento fallido
                    if (_loginAttemptService != null)
                    {
                        try
                        {
                            var failedAttempt = new LoginAttempt(email, false, errorMessage);
                            await _loginAttemptService.SaveLoginAttemptAsync(failedAttempt);

                            // Verificar cuántos intentos fallidos recientes tiene
                            var recentFailedAttempts = await _loginAttemptService.GetFailedAttemptsCountAsync(
                                email, DateTime.Now.AddMinutes(-15));

                            if (recentFailedAttempts >= 3)
                            {
                                errorMessage = $"Credenciales incorrectas. Te quedan {5 - recentFailedAttempts} intentos " +
                                             $"antes de que tu cuenta sea bloqueada temporalmente.";
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error guardando intento fallido: {ex.Message}");
                        }
                    }

                    await DisplayAlert("Error de Login", errorMessage, "OK");
                }
            }
            catch (HttpRequestException)
            {
                // Error de conexión
                string connectionError = "No se pudo conectar al servidor. Verifica tu conexión a internet.";

                // Guardar error de conexión
                if (_loginAttemptService != null)
                {
                    try
                    {
                        var connectionAttempt = new LoginAttempt(email, false, connectionError);
                        await _loginAttemptService.SaveLoginAttemptAsync(connectionAttempt);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error guardando intento de conexión: {ex.Message}");
                    }
                }

                await DisplayAlert("Error de Conexión", connectionError, "OK");
            }
            catch (Exception ex)
            {
                // Error general
                string generalError = $"Error inesperado: {ex.Message}";

                // Guardar error general
                if (_loginAttemptService != null)
                {
                    try
                    {
                        var errorAttempt = new LoginAttempt(email, false, generalError);
                        await _loginAttemptService.SaveLoginAttemptAsync(errorAttempt);
                    }
                    catch (Exception saveEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error guardando intento con error: {saveEx.Message}");
                    }
                }

                await DisplayAlert("Error", generalError, "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                LoginButton.IsEnabled = true;
            }
        }

        private async void OnForgotPasswordTapped(object sender, TappedEventArgs e)
        {
            await DisplayAlert("Recuperar Contraseña",
                "Funcionalidad en desarrollo. Contacta al administrador del sistema.", "OK");
        }

        private async void OnRegisterTapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }

        // Limpiar intentos antiguos (opcional, puede llamarse periódicamente)
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Limpiar intentos antiguos al aparecer la página
            if (_loginAttemptService != null)
            {
                try
                {
                    // Limpiar intentos mayores a 30 días
                    await _loginAttemptService.ClearOldAttemptsAsync(30);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error limpiando intentos antiguos: {ex.Message}");
                }
            }
        }

        // Ver historial de intentos (opcional, para debugging o admin)
        private async void OnViewLoginHistoryTapped(object sender, TappedEventArgs e)
        {
            if (_loginAttemptService == null)
            {
                await DisplayAlert("Error", "Servicio de intentos no disponible.", "OK");
                return;
            }

            try
            {
                var email = EmailEntry.Text?.Trim();
                if (string.IsNullOrEmpty(email))
                {
                    await DisplayAlert("Error", "Ingresa un email para ver el historial.", "OK");
                    return;
                }

                var attempts = await _loginAttemptService.GetLoginAttemptsByEmailAsync(email);
                var recentAttempts = attempts.Take(5).ToList();

                if (!recentAttempts.Any())
                {
                    await DisplayAlert("Historial", "No hay intentos registrados para este email.", "OK");
                    return;
                }

                var historyText = string.Join("\n", recentAttempts.Select(a =>
                    $"{a.GetFormattedDateTime()} - {a.GetStatusText()}" +
                    (!string.IsNullOrEmpty(a.ErrorMessage) ? $" ({a.ErrorMessage})" : "")));

                await DisplayAlert("Últimos 5 Intentos", historyText, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error obteniendo historial: {ex.Message}", "OK");
            }
        }
    }
}