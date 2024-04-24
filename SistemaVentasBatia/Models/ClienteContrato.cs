using System;

namespace SistemaVentasBatia.Models
{
    public class ClienteContrato
    {
        public int IdClienteContrato { get; set; }
        public int IdEmpresa { get; set; }
        public int IdProspecto { get; set; }
        public int IdCotizacion { get; set; }
        //cliente datos
        public string ConstitutivaEscrituraPublica { get; set; }
        public string ConstitutivaFecha { get; set; }
        public string ConstitutivaLicenciado { get; set; }
        public string ConstitutivaNumeroNotario { get; set; }
        public string ConstitutivaFolioMercantil { get; set; }
        public int ConstitutivaIdEstado { get; set; }
        public string PoderEscrituraPublica { get; set; }
        public string PoderFecha { get; set; }
        public string PoderLicenciado { get; set; }
        public string PoderNumeroNotario { get; set; }
        public int PoderIdEstado { get; set; }
        public string ClienteRegistroPatronal { get; set; }
        //poliza
        public decimal PolizaMonto { get; set; }
        public string PolizaMontoLetra { get; set; }
        public string PolizaEmpresa { get; set; }
        public string PolizaNumero { get; set; }
        //vigencia
        public string ContratoVigencia { get; set; }
        //contrato empresa
        public string EmpresaContactoNombre { get; set; }
        public string EmpresaContactoCorreo { get; set; }
        public string EmpresaContactoTelefono { get; set; }
        //domicilio fiscal
        public string ClienteDireccion { get; set; }
        public int ClienteColonia { get; set; }
        public string ClienteColoniaDescripcion { get; set; }
        public int ClienteMunicipio { get; set; }
        public string ClienteMunicipioDescripcion { get; set; }
        public int ClienteEstado { get; set; }
        public string CP { get; set; }
        public string ClienteRazonSocial { get; set; }
        public string ClienteRfc { get; set; }
        public string ClienteEmail { get; set; }
        public string ClienteContactoNombre { get; set; }
        public string ClienteContactoTelefono { get; set; }
        public string ClienteRepresentante { get; set; }
    }
}
