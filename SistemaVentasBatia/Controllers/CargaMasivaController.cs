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
using Microsoft.AspNetCore.Authorization;

namespace SistemaVentasBatia.Controllers

{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CargaMasivaController : ControllerBase
    {

        private readonly ICargaMasivaService logic;
        private readonly IUsuarioService _logic;
        public CargaMasivaController(ICargaMasivaService service, IUsuarioService _usuarioService)
        {
            logic = service;
            _logic = _usuarioService;
        }

        [HttpPost("[action]")]
        public IActionResult DescargarLayoutDirectorio()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            string rutaArchivo = Path.Combine("Layouts", "LayoutDirectorios.xlsx");

            byte[] fileContents = System.IO.File.ReadAllBytes(rutaArchivo);

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "archivo.xlsx");
        }

        [HttpPost("[action]/{idCotizacion}/{idProspecto}")]
        public async Task<bool> CargarDirecciones(int idCotizacion, int idProspecto, IFormFile file)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            await logic.CargarDirecciones(idCotizacion, idProspecto, file);
            return true;
        }

        [HttpPost("[action]/{idCotizacion}")]
        public async Task <FileContentResult> DescargarLayoutPlantilla(int idCotizacion)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            byte[] fileContents = await logic.ObtenerSucursalesLayout(idCotizacion);

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "archivo.xlsx");
        }
        
        [HttpPost("[action]/{idCotizacion}")]
        public async Task <bool> CargarPlantilla( IFormFile file, int idCotizacion)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await logic.CargarPlantilla(file, idCotizacion);
        }

        [HttpPost("[action]/{idCotizacion}")]
        public async Task<FileContentResult> DescargarLayoutProductoExtra(int idCotizacion)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            byte[] fileContents = await logic.ObtenerSucursalesLayoutProductoExtra(idCotizacion);

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "archivo.xlsx");
        }

        [HttpPost("[action]/{idCotizacion}/{tipo}/{idPersonal}")]
        public async Task<bool> CargaLayoutProductoExtra(IFormFile file, int idCotizacion, string tipo, int idPersonal)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await logic.CargaProductoExtra(file, idCotizacion, tipo, idPersonal);
        }

        [HttpPost("[action]/{idCotizacion}")]
        public async Task<FileContentResult> DescargarDatosCotizacion(int idCotizacion)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            byte[] fileContents = await logic.ObtenerDatosCotizacion(idCotizacion);

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "archivo.xlsx");

        }
    }
}