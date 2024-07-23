using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.DTOs
{
    public class CatalogoSueldoJornaleroDTO
    {
        public int IdImporte { get; set; }
        public int IdCliente { get; set; }
        public string Cliente { get; set; }
        public int IdEstado { get; set; }
        public string Estado { get; set; }
        public int IdMunicipio { get; set; }
        public string Municipio { get; set; }
        public int IdJornada { get; set; }
        public string Jornada { get; set; }
        public decimal Importe { get; set; }
    }
}
