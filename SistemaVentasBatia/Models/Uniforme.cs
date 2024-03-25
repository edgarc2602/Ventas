using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Models
{
    public class Uniforme
    {
        public int IdUniforme { get; set; }
        public int IdPuesto { get; set; }
        public Prenda Prendas { get; set; }
        public decimal Costo { get; set; }
        public int IdFrecuencia { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int IdUsuarioCreacion { get; set; }
    }
}
