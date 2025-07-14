using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RodriguezCalvaRualesMAUIUniWay.API
{
    public class Reserva
    {
        public int Id { get; set; }
        public string Estado { get; set; }
        public string MetodoPago { get; set; }
        public int ViajeId { get; set; }
        public int PasajeroId { get; set; }
    }
}
