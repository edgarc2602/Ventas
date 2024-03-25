using System;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.DTOs
{
    public class PuestoDireccionMinDTO
    {
        public int IdPuestoDireccionCotizacion { get; set; }
        public int IdDireccionCotizacion { get; set; }
        public decimal Jornada { get; set; }
        public string JornadaDesc { get; set; }
        public TimeSpan HrInicio { get; set; }
        public TimeSpan HrFin { get; set; }
        public string DiaInicio { get; set; }
        public string DiaFin { get; set; }
        public decimal Sueldo { get; set; }
        public decimal Vacaciones { get; set; }
        public decimal PrimaVacacional { get; set; }
        public decimal IMSS { get; set; }
        public decimal ISN { get; set; }
        public decimal Aguinaldo { get; set; }
        public decimal Total { get; set; }
        public string Turno { get; set; }
        public string Puesto { get; set; }
        public int IdCotizacion { get; set; }

        public int Cantidad { get; set; }

        public decimal Festivo { get; set; }
        public decimal Bonos { get; set; }
        public decimal Vales { get; set; }
        public decimal Domingo { get; set; }
    }
}
