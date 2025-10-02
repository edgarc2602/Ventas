using SistemaVentasBatia.Enums;
using System.Collections.Generic;

namespace SistemaVentasBatia.DTOs {
    public class ListaProductoSumcoDTO {
        public List<ProductoSumcoDTO> Productos { get; set; }

        public string Keywords { get; set; }

        public int Pagina { get; set; }

        public int NumPaginas { get; set; }

        public int Rows { get; set; }

    }
}
