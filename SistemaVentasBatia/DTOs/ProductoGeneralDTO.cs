using SistemaVentasBatia.Models;
using System.Collections.Generic;

namespace SistemaVentasBatia.DTOs
{
    public class ProductosGeneralDTO
    {
        public List<MaterialCotizacionDTO> Material { get; set; }
        public List<MaterialCotizacionDTO> Uniforme { get; set; }
        public List<MaterialCotizacionDTO> Equipo { get; set; }
        public List<MaterialCotizacionDTO> Herramienta { get; set; }
        public int IdCotizacion { get; set; }
    }

}
