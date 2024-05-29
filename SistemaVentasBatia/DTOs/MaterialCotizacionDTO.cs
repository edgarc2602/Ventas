using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.DTOs
{
    public class MaterialCotizacionDTO
    {
        public int IdMaterialCotizacion { get; set; }
        [Required(ErrorMessage = "Producto es necesario")]
        //[MinLength(7, ErrorMessage = "Clave debe ser 7 carateres mínimo")]
        public string ClaveProducto { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Cotización es necesaria")]
        public int IdCotizacion { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Dirección es necesario")]
        public int IdDireccionCotizacion { get; set; }


        public int IdPuestoDireccionCotizacion { get; set; }
        public decimal PrecioUnitario { get; set; }
        [Required(ErrorMessage = "Cantidad es necesario")]
        [Range(1, int.MaxValue, ErrorMessage = "Cantidad debe ser mayor que 0")]
        public decimal Cantidad { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Frecuencia es necesaria")]
        public Frecuencia IdFrecuencia { get; set; }
        public decimal Total { get; set; }
        public DateTime FechaAlta { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Usuario es necesario")]
        public int IdPersonal { get; set; }
        public int edit { get; set; }
        public int DiasEvento { get; set; }
    }
}
