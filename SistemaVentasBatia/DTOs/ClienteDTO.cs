using System;

namespace SistemaVentasBatia.DTOs
{
    public class ClienteDTO
    {
        public int IdServicio { get; set; }
        public int IdCotizacion { get; set; }
        public int IdProspecto { get; set; }
        public int IdPersonal { get; set; }
        public int IdCliente { get; set; }
        public string Codigo { get; set; }
        public int IdTipo { get; set; }
        public string NombreComercial { get; set; }
        public string Contacto { get; set; }
        public string Departamento { get; set; }
        public string Puesto { get; set; }
        public string Email { get; set; }
        public string Telefonos { get; set; }
        public int IdEjecutivo { get; set; }
        public int IdGerenteLimpieza { get; set; }
        public int IdEmpresaPagadora { get; set; }
        public string Facturacion { get; set; }
        public string TipoFacturacion { get; set; }
        public int Credito { get; set; }
        public int DiasFacturacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public int Vigencia { get; set; }
        public DateTime FechaTermino { get; set; }
        public decimal PorcentajeMateriales { get; set; }
        public decimal PorcentajeIndirectos { get; set; }
        public int DiaLimiteFacturar { get; set; }
        public int TotalSucursales { get; set; }
        public int TotalEmpleados { get; set; }
        public bool IncluyeMaterial { get; set; }
        public bool IncluyeHerramienta { get; set; }
        public bool DeductivaMaterial { get; set; }
        public bool DeductivaServicio { get; set; }
        public bool DeductivaPlantilla { get; set; }
        public bool DeductivaPlazoEntrega { get; set; }
    }
}
