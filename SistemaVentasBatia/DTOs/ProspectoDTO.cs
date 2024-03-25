using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.DTOs
{
    public class ProspectoDTO
    {
        public int IdProspecto { get; set; }

        [Required(ErrorMessage = "Nombre comercial es necesario")]
        [StringLength(500, ErrorMessage = "Máximo 500 posiciones")]
        public string NombreComercial { get; set; }

        //[Required(ErrorMessage = "Razón social es necesario")]
        [StringLength(500, ErrorMessage = "Máximo 500 posiciones")]
        public string RazonSocial { get; set; }

        //[Required(ErrorMessage = "RFC es necesario")]
        [StringLength(13, ErrorMessage = "El RFC no puede contener más de 13 caracteres")]
        public string Rfc { get; set; }

        //[Required(ErrorMessage = "Domicilio Fiscal es obligatorio.")]
        [StringLength(500, ErrorMessage = "Máximo 500 posiciones")]
        public string DomicilioFiscal { get; set; }

        public string RepresentanteLegal { get; set; }

        public string Telefono { get; set; }

        public DateTime FechaAlta { get; set; }

        [Required(ErrorMessage = "Contacto es obligatorio.")]
        [StringLength(200, ErrorMessage = "Máximo 200 posiciones")]
        public string NombreContacto { get; set; }

        [Required(ErrorMessage = "Email Contacto es obligatorio.")]
        [StringLength(100, ErrorMessage = "Máximo 100 posiciones")]
        public string EmailContacto { get; set; }

        [Required(ErrorMessage = "Numero contacto es obligatorio.")]
        [StringLength(15, ErrorMessage = "Máximo 15 posiciones")]
        public string NumeroContacto { get; set; }
        [StringLength(5, ErrorMessage = "Máximo 5 posiciones")]
        public string ExtContacto { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Usuario es requerido")]
        public int IdPersonal { get; set; }

        public ICollection<Item<int>> ListaDocumentos { get; set; }

        public int IdCotizacion { get; set; }
        public EstatusProspecto IdEstatusProspecto { get; set; }
        [Required(ErrorMessage = "Poliza de cumplimiento es necesario")]
        public bool PolizaCumplimiento { get; set; }
    }
}
