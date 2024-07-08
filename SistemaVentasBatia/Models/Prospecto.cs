using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.Models
{
    public class Prospecto
    {        
        public int IdProspecto { get; set; }
        public string NombreComercial { get; set; } 
        public string RazonSocial { get; set; }
        public string Rfc { get; set; }
        public string DomicilioFiscal { get; set; }
        public string RepresentanteLegal { get; set; }
        public string Telefono { get; set; }
        public Documento Documentacion { get; set; }
        public EstatusProspecto IdEstatusProspecto { get; set; }
        public DateTime FechaAlta { get; set; }
        public int IdPersonal { get; set; }
        public string NombreContacto { get; set; }
        public string EmailContacto { get; set; }
        public string NumeroContacto { get; set; }
        public string  ExtContacto { get; set; }
        public List<Cotizacion> Cotizaciones { get; set; }
        public int IdTipoIndustria { get; set; }
    }
}
