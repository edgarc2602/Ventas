using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.Models
{
    public class Cotizacionupd
    {
        public int IdCotizacion { get; set; }
        public string Indirecto { get; set; }
        public string Utilidad { get; set; }
        public string ComisionSV { get; set; }
        public string ComisionExt { get; set; }
    }
}
