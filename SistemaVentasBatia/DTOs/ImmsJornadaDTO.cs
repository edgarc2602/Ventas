using System;

namespace SistemaVentasBatia.DTOs
{
    public class ImmsJornadaDTO
    {
        public int IdImmsJornadaCotizador { get; set; }
        public decimal Normal2 { get; set; }
        public decimal Normal4 { get; set; }
        public decimal Normal8 { get; set; }
        public decimal Normal12 { get; set; }
        public decimal Frontera2 { get; set; }
        public decimal Frontera4 { get; set; }
        public decimal Frontera8 { get; set; }
        public decimal Frontera12 { get; set; }
        public DateTime FechaAlta { get; set; }
        public int IdPersonal { get; set; }
        public string Usuario { get; set; }

    }
}
