using SistemaVentasBatia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Models
{
    public class EquipoCotizacion
    {
        public int IdEquipoCotizacion { get; set; }

        public string ClaveProducto { get; set; }

        public int IdCotizacion { get; set; }

        public int IdDireccionCotizacion { get; set; }

        public int IdPuestoDireccionCotizacion { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Cantidad { get; set; }

        public Frecuencia IdFrecuencia { get; set; }

        public decimal Total { get; set; }

        public decimal ImporteMensual { get; set; }

        public DateTime FechaAlta { get; set; }

        public int IdPersonal { get; set; }

        public string DescripcionMaterial { get; set; }

        public string NombreSucursal { get; set; }

        public string DescripcionPuesto { get; set; }





        public int IdDireccionCotizacionDireccion { get; set; }
        public int IdPuesto { get; set; }
    }
}
