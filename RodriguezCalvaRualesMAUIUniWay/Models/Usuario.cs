
using System.ComponentModel.DataAnnotations;

namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string IdBanner { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public bool EsConductor { get; set; }
    }
}