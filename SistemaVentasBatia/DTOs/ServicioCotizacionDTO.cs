using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.DTOs
{
    public class ServicioCotizacionDTO
    {
        public int IdServicioExtraCotizacion { get; set; }
        [Required(ErrorMessage = "Producto es necesario")]
        public int IdServicioExtra { get; set; }
        [MinLength(7, ErrorMessage = "Clave debe ser 7 carateres mínimo")]
        public string ServicioExtra { get; set; }
        //public string ClaveProducto { get; set; }
        //[Range(1, int.MaxValue, ErrorMessage = "Cotización es necesaria")]
        public int IdCotizacion { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Dirección es necesario")]
        public int IdDireccionCotizacion { get; set; }
        public string Direccion { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Total { get; set; }
        public decimal ImporteMensual { get; set; }
        public Frecuencia IdFrecuencia { get; set; }
        public DateTime FechaAlta { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Usuario es necesario")]
        public int IdPersonal { get; set; }
    }
}
