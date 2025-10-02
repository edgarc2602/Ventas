using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaVentasBatia.DTOs {
    public class ProductoSingaDTO {
        public int IdProducto { get; set; }
        [StringLength(10, ErrorMessage = "Máximo 10 posiciones")]
        public string Clave { get; set; }
        [StringLength(200, ErrorMessage = "Máximo 200 caracteres")]
        public string Nombre { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Id de unidad es necesario")]
        public int IdUnidad { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Precio es necesario")]
        public string Unidad { get; set; }
        public decimal Precio { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Precio de compra es necesario")]
        public decimal PrecioCompra { get; set; }
        public DateTime FechaAlta { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Personal alta es necesario")]
        public int PersonalAlta { get; set; }
        public DateTime FechaMod { get; set; }
        public int PersonalMod { get; set; }
        public int IdEstatus { get; set; }
    }
}
