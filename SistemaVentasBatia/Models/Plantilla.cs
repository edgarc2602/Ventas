using System;
using System.Security.Policy;

namespace SistemaVentasBatia.Models
{
    public class Plantilla
    {
        public int IdPuestoDireccionCotizacion { get; set; }
        public int IdPuesto { get; set; }
        public string Puesto { get; set; }
        public int IdDireccionCotizacion { get; set; }
        public int IdDireccion { get; set; }
        public string Direccion { get; set; }
        public int Cantidad { get; set; }
        public int IdZona { get; set; }
        public string Zona { get; set; }
        public int IdClase { get; set; }
        public string Clase { get; set; }
        public decimal Sueldo { get; set; }
        public decimal Aguinaldo { get; set; }
        public decimal Vacaciones { get; set; }
        public decimal PrimaVacacional { get; set; }
        public decimal ISN { get; set; }
        public decimal IMSS { get; set; }
        public decimal Bonos { get; set; }
        public decimal Vales { get; set; }
        public decimal Festivos { get; set; }
        public decimal Domingos { get; set; }
        public bool IdCubredescansos { get; set; }
        public decimal CubreDescansos { get; set; }
        public decimal Total { get; set; }
        public int IdJornada { get; set; }
        public string Jornada { get; set; }
        public int IdTurno { get; set; }
        public string Turno { get; set; }
        public string Horario { get; set; }
        public int IdDiaDescanso { get; set; }
        public string DiaDescanso { get; set; }
        public bool IdTieneMaterial { get; set; }
        public string TieneMaterial { get; set; }
        public DateTime FechaAlta { get; set; }
    }
}
