using System;
using System.ComponentModel.DataAnnotations;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.DTOs
{
    public class UniformeDTO
    {
        public int IdUniforme { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Puesto es necesario")]
        public int IdPuesto { get; set; }
        public Prenda Prendas { get; set; }
        public decimal Costo { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Frecuencia es necesario")]
        public int IdFrecuencia { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int IdUsuarioCreacion { get; set; }
    }
}
