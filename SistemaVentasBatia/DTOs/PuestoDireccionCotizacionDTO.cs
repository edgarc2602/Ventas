using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.DTOs
{
    public class PuestoDireccionCotizacionDTO
    {
        public int IdPuestoDireccionCotizacion { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Puesto es obligatorio")]
        public int IdPuesto { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Dirección es obligatorio")]
        public int IdDireccionCotizacion { get; set; }

        [Required(ErrorMessage = "Cantidad es obligatorio")]
        public int? Cantidad { get; set; }

        [Required(ErrorMessage = "Jornada es obligatorio")]
        [Range(1, 50, ErrorMessage = "Jornada es obligatorio")]
        public decimal? Jornada { get; set; }
        public string JornadaDesc { get; set; }
        [Required(ErrorMessage = "Turno es obligatorio")]
        public Turno IdTurno { get; set; }
        public int IdSalario { get; set; }
        [Required(ErrorMessage = "Hora inicio es obligatorio")]
        public TimeSpan HrInicio { get; set; }

        public TimeSpan HrFin { get; set; }
        [Required(ErrorMessage = "Día inicio es obligatorio")]
        public DiaSemana? DiaInicio { get; set; }
        [Required(ErrorMessage = "Día fin es obligatorio")]
        public DiaSemana? DiaFin { get; set; }

        //[Required(ErrorMessage = "Este campo es obligatorio")]
        //[Range(1, double.MaxValue, ErrorMessage = "Sueldo es obligatorio")]
        public decimal Sueldo { get; set; }
        public DateTime FechaAlta { get; set; }
        public int IdPersonal { get; set; }
        public decimal Vacaciones { get; set; }
        public decimal PrimaVacacional { get; set; }
        public decimal IMSS { get; set; }
        public decimal ISN { get; set; }
        public decimal Aguinaldo { get; set; }
        public decimal Total { get; set; }

        //custom
        public string DescripcionTurno { get; set; }
        public string DescripcionPuesto { get; set; }
        public int IdCotizacion { get; set; }
        public int IdTabulador { get; set; }
        public int IdClase { get; set; }

        public int IdZona { get; set; }
        public bool DiaFestivo { get; set; }
        public decimal Festivo { get; set; }
        public decimal Bonos { get; set; }
        public decimal Vales { get; set; }
        public bool DiaDomingo { get; set; }
        public decimal Domingo { get; set; }

        public bool DiaCubreDescanso { get; set; }
        public decimal CubreDescanso { get; set; }
        public TimeSpan HrInicioFin { get; set; }
        public TimeSpan HrFinFin { get; set; }
        public DiaSemana? DiaInicioFin { get; set; }
        public DiaSemana? DiaFinFin { get; set; }
        public DiaSemana? DiaDescanso { get; set; }
    }
}
