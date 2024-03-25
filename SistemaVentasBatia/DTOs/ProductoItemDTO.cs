using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.DTOs
{
    public class ProductoItemDTO
    {
        public int IdMaterialPuesto { get; set; }
        public string ClaveProducto { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public decimal Cantidad { get; set; }
        public string IdFrecuencia { get; set; }
    }
}