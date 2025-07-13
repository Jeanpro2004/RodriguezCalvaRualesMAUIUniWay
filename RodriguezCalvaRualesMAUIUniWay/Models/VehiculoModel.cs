namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    public class VehiculoModel
    {
        public int Id { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Color { get; set; }
        public string Placa { get; set; }
        public int Año { get; set; }
        public int Capacidad { get; set; }
        public string TipoVehiculo { get; set; } // Auto, SUV, Camioneta, etc.

        public string VehiculoCompleto => $"{Marca} {Modelo} {Año}";
        public string PlacaFormateada => Placa?.ToUpper();
    }
}