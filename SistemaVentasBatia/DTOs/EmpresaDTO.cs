using Microsoft.VisualBasic;
using System;

namespace SistemaVentasBatia.DTOs
{
    public class EmpresaDTO
    {
        public int IdEmpresa { get; set; }
        public string Nombre { get; set; }
        public int Tipo { get; set; }
        public string Registro { get; set; }
        public string RazonSocial { get; set; }
        public string Rfc { get; set; }
        public string CalleNumero { get; set; }
        public string Colonia { get; set; }
        public string CP { get; set; }
        public string Municipio { get; set;}
        public int IdEstado { get; set; }
        public int IdEstatus { get; set; }
        public int IdClase { get; set; }
        public string Representante { get; set; }
        public string FolioVigente { get; set; }
        public string ConstitutivaEscrituraPublica { get; set;  }
        public DateTime ConstitutivaFecha { get; set; }
        public string ConstitutivaLicenciado { get; set; }
        public string ConstitutivaNumeroNotario { get; set; }
        public string ConstitutivaFolioMercantil { get; set; }
        public int ConstitutivaIdEstado { get; set; }
        public string PoderEscrituraPublica { get; set; }
        public DateTime PoderFecha { get; set; }
        public string PoderLicenciado { get; set; }
        public string PoderNumeroNotario { get; set; }
        public int PoderIdEstado { get; set; }

    }
}
