using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.Models
{
    public class TipoInmueble
    {
        public int IdTipoInmueble { get; set; }

        public string Descripcion { get; set; }

        public EstatusTipoInmueble IdEstatusInmueble { get; set; }
    }
}
