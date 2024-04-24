using System.Security.Policy;

namespace SistemaVentasBatia.Models
{
    public class ClienteCotizacion
    {
        public int IdClienteCotizacion { get; set; }
        public int IdEmpresa { get; set; }
        public int IdCotizacion { get; set; }
        public int IdCliente { get; set; }
        public string RazonSocial { get; set; }
        public string RepresentanteLegal { get; set; }
        public string PoderRepresentanteLegal { get; set; }
        public string ActaConstitutiva { get; set; }
        public string RFC { get; set; }
        public string DomicilioFiscal { get; set; }
        public string RegistroPatronal { get; set; }
    }
}
