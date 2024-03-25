using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.DTOs
{
    public class ListaServiciosCotizacionLimpiezaDTO
    {
        public List<ServicioCotizacionMinDTO> ServiciosCotizacion { get; set; }
        public int IdCotizacion { get; set; }
        public int IdDireccionCotizacion { get; set; }
        public string Keywords { get; set; }
        public int Pagina { get; set; }
        public int Rows { get; set; }
        public int NumPaginas { get; set; }

    }
}
