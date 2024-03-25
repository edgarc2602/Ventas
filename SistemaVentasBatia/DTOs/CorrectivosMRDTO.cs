using System.Collections.Generic;

namespace SistemaVentasBatia.DTOs
{
    public class CorrectivosMRDTO
    {   
        public int IdClaveCM { get; set; }
        public string EntrevistaCliente { get; set; }
        public string TrabajosGeneral { get; set; }
        public string TecnicosUniforme { get; set; }
        public string TratoTecnicos { get; set; }
        public string TrabajosOrden { get; set; }
        public string MaterialesAdecuados { get; set; }
        public string Encuestado { get; set; }
        public List<CRMImagenesDTO> Imagenes { get; set; }
    }
}
