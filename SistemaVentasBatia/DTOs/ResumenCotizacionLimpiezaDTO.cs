using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.DTOs
{
    public class ResumenCotizacionLimpiezaDTO
    {
        public int IdCotizacion { get; set; }
        public decimal Salario { get; set; }
        public decimal CargaSocial { get; set; }
        public decimal Provisiones { get; set; }
        public decimal Prestaciones { get; set; }
        public decimal Material { get; set; }
        public decimal Uniforme { get; set; }
        public decimal Equipo { get; set; }
        public decimal Herramienta { get; set; }
        public decimal Servicio { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Indirecto { get; set; }
        public decimal Utilidad { get; set; }
        public decimal ComisionSV { get; set; }
        public decimal ComisionExt { get; set; }
        public decimal Total { get; set; }
        public int IdProspecto { get; set; }
        public Servicio IdServicio { get; set; }
        public int IdCotizacionOriginal { get; set; }


        public string NombreComercial { get; set; }
        public decimal UtilidadPor { get; set; }
        public decimal IndirectoPor { get; set; }
        public decimal CsvPor { get; set; }
        public decimal ComisionExtPor { get; set; }
        public bool PolizaCumplimiento { get; set; }
        public decimal  TotalPolizaCumplimiento { get; set; }
    }
}