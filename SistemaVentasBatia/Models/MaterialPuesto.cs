using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.Models
{
    public class MaterialPuesto
    {
        public int IdMaterialPuesto { get; set; }
        public string ClaveProducto { get; set; }
        public int IdPuesto { get; set; }
        public decimal Precio { get; set; }
        public decimal Cantidad { get; set; }
        public Frecuencia IdFrecuencia { get; set; }
        public DateTime FechaAlta { get; set; }
        public int IdPersonal { get; set; }
    }
}
