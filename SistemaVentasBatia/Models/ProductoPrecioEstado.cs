namespace SistemaVentasBatia.Models
{
    public class ProductoPrecioEstado
    {
        public int RowNum { get; set; }
        public string Clave { get; set; }
        public int Familia { get; set; }
        public string Descripcion { get; set; }
        public int IdProveedor { get; set; }
        public decimal PrecioProveedor { get; set; }
        public decimal PrecioBase { get; set; }
    }
}
