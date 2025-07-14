using System;
using System.Collections.Generic;
namespace RodriguezCalvaRualesMAUIUniWay.API
{
    public class Usuario
    {
        public int Id { get; set; }
        public string IdBanner { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Contrasena { get; set; }
        public bool EsConductor { get; set; }
    }
}
