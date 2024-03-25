using System.Collections.Generic;

namespace SistemaVentasBatia.DTOs
{
    public class DireccionAPIDTO
    {
        public string Estado { get; set; }
        public int IdEstado { get; set; }
        public string EstadoAbreviatura { get; set; }
        public string Municipio { get; set; }
        public int IdMunicipio { get; set; }
        public string CentroReparto { get; set; }
        public string CodigoPostal { get; set; }
        public List<string> Colonias { get; set; }
    }
}
