using System;

namespace SistemaVentasBatia.Models
{
    public class SalarioMinimo
    {
        public int IdSalario { get; set; }
        public DateTime FechaAplica { get; set; }
        public decimal SalarioBase { get; set; }
        public int Zona { get; set; }
    }
}
