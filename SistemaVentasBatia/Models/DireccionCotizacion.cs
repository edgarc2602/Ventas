using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Models
{
    public class DireccionCotizacion
    {
        public int IdDireccionCotizacion { get; set; }

        public int IdDireccion { get; set; }

        public int IdCotizacion { get; set; }

        //custom

        public string NombreSucursal { get; set; }

    }
}
