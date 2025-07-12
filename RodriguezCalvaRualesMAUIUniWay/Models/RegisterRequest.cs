
namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    public class RegisterRequest
    {
        public string IdBanner { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public string ConfirmarContrasena { get; set; } = string.Empty;
        public bool EsConductor { get; set; }
        public bool AceptaTerminos { get; set; }
    }
}