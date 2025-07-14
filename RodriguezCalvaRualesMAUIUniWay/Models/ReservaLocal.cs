using System;
using SQLite;

namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    [Table("Reservas")]
    public class ReservaLocal
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public int ReservaRemotaId { get; set; }

        [NotNull]
        public int ViajeId { get; set; }

        [NotNull]
        public int UsuarioId { get; set; }

        [MaxLength(20), NotNull]
        public string Estado { get; set; } = "Pendiente";

        [MaxLength(20), NotNull]
        public string MetodoPago { get; set; }

        [NotNull]
        public DateTime FechaReserva { get; set; }

        [NotNull]
        public DateTime FechaViaje { get; set; }

        [MaxLength(100)]
        public string Origen { get; set; }

        [MaxLength(100)]
        public string Destino { get; set; }

        public decimal Precio { get; set; }

        public int NumeroAsientos { get; set; } = 1;

        [MaxLength(500)]
        public string Observaciones { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaActualizacion { get; set; }

        // Propiedades calculadas
        [Ignore]
        public bool EsPendiente => Estado == "Pendiente";

        [Ignore]
        public bool EsConfirmada => Estado == "Confirmada";

        [Ignore]
        public string RutaCompleta => $"{Origen} → {Destino}";
    }

    // Enum para estados de reserva
    public static class EstadosReserva
    {
        public const string Pendiente = "Pendiente";
        public const string Confirmada = "Confirmada";
        public const string Cancelada = "Cancelada";
        public const string Completada = "Completada";
    }
}