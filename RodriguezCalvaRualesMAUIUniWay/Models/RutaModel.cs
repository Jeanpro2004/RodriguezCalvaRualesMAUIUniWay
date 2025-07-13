namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    public class RutaModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public double DistanciaKm { get; set; }
        public int TiempoEstimadoMinutos { get; set; }
        public decimal PrecioSugerido { get; set; }
        public bool EsPopular { get; set; }
        public List<string> PuntosIntermedios { get; set; } = new List<string>();

        // Propiedades computadas
        public string RutaCompleta => $"{Origen} → {Destino}";
        public string DistanciaFormateada => $"{DistanciaKm:F1} km";
        public string TiempoFormateado => $"{TiempoEstimadoMinutos} min";
        public string PrecioSugeridoFormateado => $"${PrecioSugerido:F2}";
        public string IconoPopular => EsPopular ? "⭐" : "";
    }
}