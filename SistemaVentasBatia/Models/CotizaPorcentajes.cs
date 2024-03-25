using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.Models
{
    public class CotizaPorcentajes
    {
        public int IdPersonal { get; set; }
        public string Personal { get; set; }
        public decimal CostoIndirecto { get; set; }
        public decimal Utilidad { get; set; }
        public decimal ComisionSobreVenta { get; set; }
        public decimal ComisionExterna { get; set; }
        public string FechaAplica { get; set; }
        public DateTime FechaAlta { get; set; }
    }
}