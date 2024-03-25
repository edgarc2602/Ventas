using SistemaVentasBatia.Enums;
using System;

namespace SistemaVentasBatia.DTOs
{
    public class CotizacionMinDTO
    {
        public int IdCotizacion { get; set; }
        public int IdProspecto { get; set; }
        public string Servicio { get; set; }
        public decimal Total { get; set; }
        public DateTime FechaAlta { get; set; }
        public int IdCotizacionOriginal { get; set; }
        public string NombreComercial { get; set; }
        public string IdAlta { get; set; }
        public EstatusCotizacion IdEstatusCotizacion { get; set; }
    }
}