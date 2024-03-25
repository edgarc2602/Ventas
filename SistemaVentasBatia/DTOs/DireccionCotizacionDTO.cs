using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.DTOs
{
    public class DireccionCotizacionDTO
    {
        public int IdDireccionCotizacion { get; set; }
        public int IdDireccion { get; set; }
        public int IdCotizacion { get; set; }
        public string NombreSucursal { get; set; }
    }
}
