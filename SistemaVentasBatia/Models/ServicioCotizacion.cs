using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.Models
{
    public class ServicioCotizacion
    {
        public int IdServicioExtraCotizacion { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Servicio es necesario")]

        public int IdServicioExtra { get; set; }
        public string ServicioExtra { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Cotización es necesaria")]
        public int IdCotizacion { get; set; }
        public int IdDireccionCotizacion { get; set; }
        public string Direccion { get; set; }
        [Required(ErrorMessage = "Costo es necesario")]
        [Range(1, int.MaxValue, ErrorMessage = "Costo debe ser mayor que 0")]
        public decimal PrecioUnitario { get; set; }
        [Required(ErrorMessage = "Cantidad es necesario")]
        [Range(1, int.MaxValue, ErrorMessage = "Cantidad debe ser mayor que 0")]
        public decimal Cantidad { get; set; }

        public decimal Total { get; set; }

        public decimal ImporteMensual { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Frecuencia es necesaria")]
        public Frecuencia IdFrecuencia { get; set; }
        public DateTime FechaAlta { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Usuario es necesario")]
        public int IdPersonal { get; set; }
    }
}
