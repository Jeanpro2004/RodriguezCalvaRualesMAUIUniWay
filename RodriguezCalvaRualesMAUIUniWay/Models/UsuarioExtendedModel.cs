using RodriguezCalvaRualesMAUIUniWay.API;

namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    public class UsuarioExtendedModel : Usuario
    {
        public DateTime FechaRegistro { get; set; }
        public DateTime UltimoAcceso { get; set; }
        public string FotoPerfil { get; set; }
        public double CalificacionPromedio { get; set; }
        public int TotalViajes { get; set; }
        public bool PerfilVerificado { get; set; }
        public VehiculoModel Vehiculo { get; set; }
        public List<string> PreferenciasViaje { get; set; } = new List<string>();

        // Propiedades computadas
        public string FechaRegistroFormateada => FechaRegistro.ToString("dd/MM/yyyy");
        public string UltimoAccesoFormateado => UltimoAcceso.ToString("dd/MM/yyyy HH:mm");
        public string CalificacionFormateada => $"⭐ {CalificacionPromedio:F1}";
        public string TipoUsuarioTexto => EsConductor ? "Conductor" : "Pasajero";
        public string EstadoVerificacion => PerfilVerificado ? "✓ Verificado" : "⚠️ Pendiente";
    }
}