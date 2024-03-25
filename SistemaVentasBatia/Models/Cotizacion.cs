using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.Models
{
    public class Cotizacion
    {


        public int IdCotizacion { get; set; }
        public int IdProspecto { get; set; }
        public string Nombre { get; set; }
        public string NombreComercial { get; set; }
        public decimal CostoIndirecto { get; set; }
        public decimal Utilidad { get; set; }
        public decimal ComisionSV { get; set; }
        public decimal ComisionExt { get; set; }
        public decimal Total { get; set; }
        public Servicio IdServicio { get; set; }
        public EstatusCotizacion IdEstatusCotizacion { get; set; }
        public DateTime FechaAlta { get; set; }
        public int IdPersonal { get; set; }
        public int IdCotizacionOriginal { get; set; }
        public int IdPorcentaje { get; set; }
        public string IdAlta { get; set; }
        public SalarioTipo SalTipo { get; set; }


    }
}
