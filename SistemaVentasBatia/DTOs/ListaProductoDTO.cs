using System.Collections.Generic;

namespace SistemaVentasBatia.DTOs
{
    public class ListaProductoDTO
    {
        public List<ProductoPrecioEstadoDTO> Productos { get; set; }
        public List<ProductoFamiliaDTO> Familias { get; set; }
        public int Pagina { get; set; }
        public int NumPaginas { get; set; }
        public int Rows { get; set; }
        public int IdProveedor { get; set; }
        public string Proveedor { get; set; }
    }
}
