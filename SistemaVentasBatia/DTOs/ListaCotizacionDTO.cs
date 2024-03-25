using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.DTOs
{
    public class ListaCotizacionDTO
    {
        public List<CotizacionMinDTO> Cotizaciones { get; set; }

        public EstatusCotizacion IdEstatusCotizacion { get; set; }

        public int IdProspecto { get; set; }

        public int IdServicio { get; set; }

        public int Pagina { get; set; }

        public int NumPaginas { get; set; }

        public int Rows { get; set; }

        public string IdAlta { get; set; }
    }
}
