using System.Collections.Generic;

namespace SistemaVentasBatia.DTOs {
    public class ListaCatalogoProductosDTO {
        public List<CatalogoDTO> Productos { get; set; }
        public string Keywords { get; set; }

        public int Pagina { get; set; }

        public int NumPaginas { get; set; }

        public int Rows { get; set; }
    }
}
