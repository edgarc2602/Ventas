using System;

namespace SistemaVentasBatia.Models {
    public class ProductoSumco {

        public int IdProducto { get; set; }
        public string Clave { get; set; }
        public string Nombre { get; set; }
        public int IdUnidad { get; set; }
        public string Unidad { get; set; }
        public decimal Precio { get; set; }
        public decimal PrecioCompra { get; set; }
        public DateTime FechaAlta { get; set; }
        public int PersonalAlta { get; set; }
        public DateTime FechaMod { get; set; }
        public int PersonalMod { get; set; }
        public int IdEstatus { get; set; }
    }
}
