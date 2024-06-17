using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace SistemaVentasBatia.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class CargaMasivaController : ControllerBase
    {

        private readonly ICargaMasivaService _logic;

        public CargaMasivaController(ICargaMasivaService logic)
        {
            _logic = logic;
        }

        [HttpPost("[action]")]
        public IActionResult DescargarLayoutDirectorio()
        {
            string rutaArchivo = Path.Combine("Layouts", "LayoutDirectorios.xlsx");

            byte[] fileContents = System.IO.File.ReadAllBytes(rutaArchivo);

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "archivo.xlsx");
        }

        [HttpPost("[action]/{idCotizacion}/{idProspecto}")]
        public async Task<bool> CargarDirecciones(int idCotizacion, int idProspecto, IFormFile file)
        {
            await _logic.CargarDirecciones(idCotizacion, idProspecto, file);
            return true;
        }

        [HttpPost("[action]/{idCotizacion}")]
        public async Task <FileContentResult> DescargarLayoutPlantilla(int idCotizacion)
        {
            byte[] fileContents = await _logic.ObtenerSucursalesLayout(idCotizacion);

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "archivo.xlsx");
        }
        
        [HttpPost("[action]/{idCotizacion}")]
        public async Task <bool> CargarPlantilla( IFormFile file, int idCotizacion)
        {
            return await _logic.CargarPlantilla(file, idCotizacion);
        }

        [HttpPost("[action]/{idCotizacion}")]
        public async Task<FileContentResult> DescargarLayoutProductoExtra(int idCotizacion)
        {
            byte[] fileContents = await _logic.ObtenerSucursalesLayoutProductoExtra(idCotizacion);

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "archivo.xlsx");
        }

        [HttpPost("[action]/{idCotizacion}/{tipo}/{idPersonal}")]
        public async Task<bool> CargaLayoutProductoExtra(IFormFile file, int idCotizacion, string tipo, int idPersonal)
        {
            return await _logic.CargaProductoExtra(file, idCotizacion, tipo, idPersonal);
        }

        [HttpPost("[action]/{idCotizacion}")]
        public async Task<FileContentResult> DescargarDatosCotizacion(int idCotizacion)
        {
            byte[] fileContents = await _logic.ObtenerDatosCotizacion(idCotizacion);

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "archivo.xlsx");

        }
    }
}