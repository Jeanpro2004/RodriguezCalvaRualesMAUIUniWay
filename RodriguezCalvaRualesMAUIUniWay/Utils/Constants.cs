namespace RodriguezCalvaRualesMAUIUniWay.Utils
{
    public static class Constants
    {
        // API Configuration
        public const string ApiBaseUrl = "http://localhost:5113/";
        public const int ApiTimeoutSeconds = 30;

        // File Names
        public const string CredencialesFileName = "credenciales.txt";
        public const string UsuarioActualFileName = "usuario_actual.json";
        public const string UsuariosLocalesFileName = "usuarios_locales.json";
        public const string ViajesLocalesFileName = "viajes_locales.json";
        public const string NotificacionesFileName = "notificaciones.json";
        public const string NotasFileName = "notas_uniway.txt";
        public const string ConfigFileName = "config.json";

        // App Settings
        public const int MaxNotificaciones = 100;
        public const int MaxDiasHistorial = 30;
        public const double DefaultLatitud = -0.1807; // Quito
        public const double DefaultLongitud = -78.4678; // Quito

        // University Campuses
        public static readonly List<string> CampusesUDLA = new()
        {
            "UDLA PARK",
            "UDLA ARENA",
            "UDLA GRANADOS",
            "UDLA COLON"
        };

        // Vehicle Types
        public static readonly List<string> TiposVehiculo = new()
        {
            "Sedan",
            "SUV",
            "Hatchback",
            "Camioneta",
            "Otro"
        };

        // Trip States
        public static readonly List<string> EstadosViaje = new()
        {
            "Activo",
            "Completo",
            "En curso",
            "Finalizado",
            "Cancelado"
        };

        // Reservation States
        public static readonly List<string> EstadosReserva = new()
        {
            "Pendiente",
            "Confirmada",
            "Cancelada",
            "Completada"
        };

        // Colors
        public const string PrimaryColor = "#8B0000";
        public const string SecondaryColor = "#2C3E50";
        public const string SuccessColor = "#27AE60";
        public const string WarningColor = "#F39C12";
        public const string ErrorColor = "#E74C3C";
        public const string InfoColor = "#3498DB";
    }
}