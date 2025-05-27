using System.Collections.Generic;

namespace SistemaVentasBatia.DTOs
{
    public class ListaMaterialesCotizacionLimpiezaDTO
    {
        public List<MaterialCotizacionMinDTO> MaterialesCotizacion { get; set; }
        public int IdCotizacion { get; set; }
        public int IdDireccionCotizacion { get; set; }
        public int IdPuestoDireccionCotizacion { get; set; }
        public string Keywords { get; set; }
        public int Pagina { get; set; }
        public int Rows { get; set; }
        public int NumPaginas { get; set; }

    }
}
