using System;

namespace SistemaVentasBatia.DTOs
{
    public class ServicioCotizacionMinDTO
    {
        public int IdServicioExtraCotizacion { get; set; }
        public int idServicioExtra { get; set; }
        public string ServicioExtra { get; set; }
        public int IdCotizacion { get; set; }
        public int IdDireccionCotizacion { get; set; }
        public string Direccion { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Total { get; set; }
        public decimal ImporteMensual { get; set; }
        public string IdFrecuencia { get; set; }
        public DateTime FechaAlta { get; set; }
        public int IdPersonal { get; set; }

    }
}
