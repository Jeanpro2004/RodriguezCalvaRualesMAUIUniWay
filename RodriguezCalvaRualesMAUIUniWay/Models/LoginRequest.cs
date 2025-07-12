namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    public class LoginRequest
    {
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public bool RecordarContrasena { get; set; }
    }
}