using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.DTOs
{
    public class ListaProspectoDTO
    {
        public List<ProspectoDTO> Prospectos { get; set; }

        public EstatusProspecto IdEstatusProspecto { get; set; }

        public string Keywords { get; set; }

        public int Pagina { get; set; }

        public int Rows { get; set; }
        public int NumPaginas { get; internal set; }
    }
}
