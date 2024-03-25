using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.Models
{
    public class ProductoItem
    {
        public int IdMaterialPuesto { get; set; }
        public string ClaveProducto { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public decimal Cantidad { get; set; }
        public Frecuencia IdFrecuencia { get; set; }
    }
}