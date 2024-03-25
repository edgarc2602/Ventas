using System;

namespace SistemaVentasBatia.Models
{
    public class Porcentaje
    {
        public int IdPorcentaje { get; set; }
        public decimal CostoIndirecto { get; set; }
        public decimal Utilidad { get; set; }
        public DateTime FechaAplica { get; set; }
        public bool Activo { get; set; }
        public int IdPersonal { get; set; }
    }
}
