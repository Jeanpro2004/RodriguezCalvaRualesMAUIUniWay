// Models/ReservaLocal.cs
using SQLite;
using System;
using RodriguezCalvaRualesMAUIUniWay.API;
using System.Diagnostics.CodeAnalysis;

namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    [Table("Reservas")]
    public class ReservaLocal
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int ViajeId { get; set; }
        public int PasajeroId { get; set; }
        public Estado Estado { get; set; } = Estado.Pendiente;
        public MetodoPago MetodoPago { get; set; }
        public DateTime FechaReserva { get; set; } = DateTime.Now;
        public DateTime FechaViaje { get; set; }

        [MaxLength(100)]
        public string Origen { get; set; }

        [MaxLength(100)]
        public string Destino { get; set; }

        public decimal Precio { get; set; }

        public int NumeroAsientos { get; set; } = 1;

        [MaxLength(500)]
        public string Observaciones { get; set; }

        // Propiedades calculadas (no se guardan en BD)
        [Ignore]
        public string RutaCompleta => $"{Origen} → {Destino}";

        [Ignore]
        public string EstadoTexto => Estado.ToString();

        [Ignore]
        public string MetodoPagoTexto => MetodoPago.ToString();
    }
}