using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.DTOs
{
    public class CotizacionDTO
    {
        public int IdCotizacion { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Prospecto es requerido")]
        public int IdProspecto { get; set; }
        public Servicio IdServicio { get; set; }
        public decimal? Total { get; set; }
        public DateTime FechaAlta { get; set; }
        public int IdCotizacionOriginal { get; set; }
        public string NombreComercial { get; set; }
        public ICollection<Item<int>> ListaServicios { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Usuario es requerido")]
        public int IdPersonal { get; set; }
        public SalarioTipo SalTipo { get; set; }
        public ICollection<Item<int>> ListaTipoSalarios { get; set; }
    }
}
