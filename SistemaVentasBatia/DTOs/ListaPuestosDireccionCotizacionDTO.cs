using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.DTOs
{
    public class ListaPuestosDireccionCotizacionDTO
    {
        public List<PuestoDireccionMinDTO> PuestosDireccionesCotizacion { get; set; }

        public List<DireccionDTO> DireccionesCotizacion { get; set; }

        public int IdCotizacion { get; set; }

        public int IdDireccionCotizacion { get; set; }
        
        public int IdPuestoDireccionCotizacion { get; set; }
        public int Empleados { get; set; }

    }
}
