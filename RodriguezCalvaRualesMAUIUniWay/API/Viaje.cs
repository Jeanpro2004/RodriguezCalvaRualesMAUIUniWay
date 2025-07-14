using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RodriguezCalvaRualesMAUIUniWay.API
{
    public class Viaje
    {
        public int Id { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public DateTime FechaHoraSalida { get; set; }
        public decimal Precio { get; set; }
        public int AsientosDisponibles { get; set; }
        public int ConductorId { get; set; }
    }
}
