using System.ComponentModel.DataAnnotations;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.DTOs
{
    public class MaterialPuestoDTO
    {
        public int IdMaterialPuesto { get; set; }
        [Required(ErrorMessage = "Clave de producto es necesario")]
        public string ClaveProducto { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Puesto es necesario")]
        public int IdPuesto { get; set; }
        public decimal Precio { get; set; }
        [Required(ErrorMessage = "Cantidad es necesario")]
        [Range(1, int.MaxValue, ErrorMessage = "Cantidad debe ser mayor que 0")]
        public decimal Cantidad { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Frecuencia es necesario")]
        public Frecuencia IdFrecuencia { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Usuario es necesario")]
        public int IdPersonal { get; set; }
    }
}
