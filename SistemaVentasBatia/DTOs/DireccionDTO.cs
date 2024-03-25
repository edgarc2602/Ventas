using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.DTOs
{   
    public class DireccionDTO
    {
        public int IdDireccion { get; set; }

        public int IdProspecto { get; set; }

        public int IdCotizacion { get; set; }

        [Required(ErrorMessage = "Nombre sucursal es necesario")]
        [StringLength(300, ErrorMessage = "Máximo 300 posiciones")]
        public string NombreSucursal { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Tipo inmueble es necesario")]
        public int IdTipoInmueble { get; set; }

        [Range(1, 50, ErrorMessage = "Estado es necesario")]
        public int IdEstado { get; set; }
        //public int IdTabulador { get; set; }

        
        [StringLength(200, ErrorMessage = "Máximo 200 posiciones")]
        public string Municipio { get; set; }
        [Required(ErrorMessage = "Ciudad es necesario")]
        [StringLength(200, ErrorMessage = "Máximo 200 posiciones")]
        public string Ciudad { get; set; }

        [Required(ErrorMessage = "Colonia es necesario")]
        [StringLength(200, ErrorMessage = "Máximo 200 posiciones")]
        public string Colonia { get; set; }
        [Required(ErrorMessage = "Domicilio es necesario")]
        [StringLength(200, ErrorMessage = "Máximo 200 posiciones")]
        public string Domicilio { get; set; }
        [StringLength(300, ErrorMessage = "Máximo 300 posiciones")]
        public string Referencia { get; set; } 
        
        [Required(ErrorMessage = "Código postal es necesario")]
        [StringLength(5, ErrorMessage = "Máximo 5 posiciones")]
        public string CodigoPostal { get; set; }

        //Custom
        [Range(1, int.MaxValue, ErrorMessage = "Municipio es necesario")]
        public int IdMunicipio { get; set; }
        public string Estado { get; set; }
        public string TipoInmueble { get; set; }
        public string DomicilioCompleto { get { return (Domicilio ?? "") + ", " + (Colonia ?? "") + ", " + (Municipio ?? "") + ", " + (Ciudad ?? "") + ", " + (Estado ?? "") + ", CP " + (CodigoPostal); } set { } }
        public int IdDireccionCotizacion { get; set; }
        public bool Frontera { get; set; }

    }
}
