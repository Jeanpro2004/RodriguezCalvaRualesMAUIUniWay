using RodriguezCalvaRualesMAUIUniWay.API;
using RodriguezCalvaRualesMAUIUniWay.Models;
using RodriguezCalvaRualesMAUIUniWay.Repositories;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly UsuarioService _usuarioService;
        private readonly LoginAttemptRepository _loginRepo;

        public LoginPage()
        {
            InitializeComponent();
            _usuarioService = new UsuarioService();
            _loginRepo = new LoginAttemptRepository();

            System.Diagnostics.Debug.WriteLine($"📁 Archivo login_attempts.txt se guardará en: {_loginRepo.ObtenerRutaArchivo()}");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Limpiar intentos antiguos
            try
            {
                await _loginRepo.LimpiarIntentosAntiguos(30);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error limpiando intentos antiguos: {ex.Message}");
            }
        }

        // BOTÓN DEBUG PARA VER RUTA Y PROBAR GUARDADO
        private async void OnDebugFileLocationClicked(object sender, EventArgs e)
        {
            try
            {
                var rutaArchivo = _loginRepo.ObtenerRutaArchivo();
                var infoArchivo = await _loginRepo.ObtenerInformacionDelArchivo();

                string debugInfo = $"📁 Ruta del archivo:\n{rutaArchivo}\n\n";

                if (infoArchivo != null)
                {
                    debugInfo += $"📄 Archivo existe: SÍ\n";
                    debugInfo += $"📊 Tamaño: {infoArchivo.Length} bytes\n";
                    debugInfo += $"📅 Creado: {infoArchivo.CreationTime:dd/MM/yyyy HH:mm}\n";
                    debugInfo += $"🔄 Modificado: {infoArchivo.LastWriteTime:dd/MM/yyyy HH:mm}";
                }
                else
                {
                    debugInfo += $"📄 Archivo existe: NO";
                }

                await DisplayAlert("Debug - Información del Archivo", debugInfo, "OK");

                // Mostrar contenido del archivo
                var intentos = await _loginRepo.ObtenerTodosLosIntentos();
                if (intentos.Any())
                {
                    var ultimosIntentos = intentos.Take(3).ToList();
                    var contenido = string.Join("\n", ultimosIntentos.Select(i =>
                        $"{i.GetFormattedDateTime()} - {i.Email} - {i.GetStatusText()}"));

                    await DisplayAlert("Últimos 3 Intentos", contenido, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error Debug", $"Error: {ex.Message}", "OK");
            }
        }

        // BOTÓN PARA PROBAR GUARDADO
        private async void OnTestFileServiceClicked(object sender, EventArgs e)
        {
            try
            {
                // Crear un intento de prueba
                var intentoPrueba = new LoginAttempt("test@udla.edu.ec", false, "Prueba de guardado desde debug");

                bool guardado = await _loginRepo.GuardarIntentoLogin(intentoPrueba);

                await DisplayAlert("Test Guardado",
                    $"¿Guardado exitoso? {guardado}\n" +
                    $"Archivo: {_loginRepo.ObtenerRutaArchivo()}", "OK");

                // Verificar leyendo los intentos
                var intentos = await _loginRepo.ObtenerTodosLosIntentos();
                await DisplayAlert("Verificación", $"Intentos en archivo: {intentos.Count}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error Test", ex.Message, "OK");
            }
        }

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

            // Verificar bloqueo de cuenta
            try
            {
                var recentFailedAttempts = await _loginRepo.ContarIntentosFallidos(email, DateTime.Now.AddMinutes(-15));
                if (recentFailedAttempts >= 5)
                {
                    await DisplayAlert("Cuenta Bloqueada",
                        "Tu cuenta ha sido bloqueada temporalmente debido a múltiples intentos fallidos. " +
                        "Intenta nuevamente en 15 minutos.", "OK");

                    var blockedAttempt = new LoginAttempt(email, false, "Cuenta bloqueada por múltiples intentos fallidos");
                    await _loginRepo.GuardarIntentoLogin(blockedAttempt);
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error verificando bloqueo: {ex.Message}");
            }

            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            LoginButton.IsEnabled = false;

            try
            {
                var usuarios = await _usuarioService.GetUsuariosAsync();
                var usuario = usuarios?.FirstOrDefault(u =>
                    string.Equals(u.Correo, email, StringComparison.OrdinalIgnoreCase) &&
                    u.Contrasena == password);

                if (usuario != null)
                {
                    // ✅ LOGIN EXITOSO
                    SessionService.CurrentUserId = usuario.Id;

                    // Guardar intento exitoso
                    var successAttempt = new LoginAttempt(email, true, "Login exitoso");
                    bool guardadoExitoso = await _loginRepo.GuardarIntentoLogin(successAttempt);

                    System.Diagnostics.Debug.WriteLine($"✅ Login exitoso - Guardado: {guardadoExitoso}");

                    await DisplayAlert("Éxito", $"¡Bienvenido, {usuario.Nombre}!", "OK");

                    EmailEntry.Text = "";
                    PasswordEntry.Text = "";

                    await Shell.Current.GoToAsync("//SearchRidePage");
                }
                else
                {
                    // ❌ LOGIN FALLIDO
                    string errorMessage = "Email o contraseña incorrectos.";

                    // Guardar intento fallido
                    var failedAttempt = new LoginAttempt(email, false, errorMessage);
                    bool guardadoFallido = await _loginRepo.GuardarIntentoLogin(failedAttempt);

                    System.Diagnostics.Debug.WriteLine($"❌ Login fallido - Guardado: {guardadoFallido}");

                    // Verificar cuántos intentos fallidos recientes tiene
                    var recentFailedCount = await _loginRepo.ContarIntentosFallidos(email, DateTime.Now.AddMinutes(-15));

                    if (recentFailedCount >= 3)
                    {
                        errorMessage = $"Credenciales incorrectas. Te quedan {5 - recentFailedCount} intentos " +
                                     $"antes de que tu cuenta sea bloqueada temporalmente.";
                    }

                    await DisplayAlert("Error de Login", errorMessage, "OK");
                }
            }
            catch (HttpRequestException)
            {
                string connectionError = "No se pudo conectar al servidor. Verifica tu conexión a internet.";

                var connectionAttempt = new LoginAttempt(email, false, connectionError);
                await _loginRepo.GuardarIntentoLogin(connectionAttempt);

                await DisplayAlert("Error de Conexión", connectionError, "OK");
            }
            catch (Exception ex)
            {
                string generalError = $"Error inesperado: {ex.Message}";

                var errorAttempt = new LoginAttempt(email, false, generalError);
                await _loginRepo.GuardarIntentoLogin(errorAttempt);

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
    }
}