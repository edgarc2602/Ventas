using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.Models
{
    public class Direccion
    {
        public int IdDireccion { get; set; }

        public int IdProspecto { get; set; }

        public int IdTipoInmueble { get; set; }

        public string NombreSucursal { get; set; }

        public int IdEstado { get; set; }
        public int IdTabulador { get; set; }

        public string Municipio { get; set; }

        public int IdMunicipio { get; set; }

        public string Ciudad { get; set; }

        public string Colonia { get; set; }

        public string Domicilio { get; set; }

        public string Referencia { get; set; }

        public string CodigoPostal { get; set; }

        public string Contacto { get; set; }

        public string TelefonoContacto { get; set; }

        public EstatusDireccion IdEstatusDireccion { get; set; }

        public DateTime FechaAlta { get; set; }

        //Custom
        public string Estado { get; set; }
        public string TipoInmueble { get; set; }

        public int IdDireccionCotizacion { get; set; }
        public bool Frontera { get; set; }
    }

}
