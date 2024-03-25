using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.DTOs
{
    public class MaterialCotizacionMinDTO
    {
        public int IdMaterialCotizacion { get; set; }
        public string ClaveProducto { get; set; }
        public int IdCotizacion { get; set; }
        public int IdDireccionCotizacion { get; set; }
        public int IdPuestoDireccionCotizacion { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Cantidad { get; set; }
        public string IdFrecuencia { get; set; }
        public decimal Total { get; set; }
        public decimal ImporteMensual { get; set; }
        public DateTime FechaAlta { get; set; }
        public int IdUsuarioAlta { get; set; }
        public string DescripcionMaterial { get; set; }
        public string NombreSucursal { get; set; }
        public string DescripcionPuesto { get; set; }
    }
}
