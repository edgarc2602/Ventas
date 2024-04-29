using System.Collections.Generic;

namespace SistemaVentasBatia.DTOs
{
    public class CotizacionVendedorDetalleDTO
    {
        public int IdVendedor { get; set; }
        public List<ResumenCotizacionLimpiezaDTO> CotizacionDetalle { get; set; }
    }
}
