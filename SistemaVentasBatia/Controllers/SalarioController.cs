using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Services;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;
using System.Collections;
using System.Collections.Generic;

namespace SistemaVentasBatia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalarioController : ControllerBase
    {
        private readonly ISalarioService _logic;

        public SalarioController(ISalarioService logic)
        {
            _logic = logic;
        }

        [HttpGet("{idtab}/{idpue}/{idtur}")]
        public async Task<SalarioMinDTO> GetFind(int idtab, int idpue, int idtur)
        {
            return await _logic.GetFind(idtab, idpue, idtur);
        }

        [HttpGet("{idPuesto?}/{idClase?}/{idTabulador?}/{idTurno?}/{jornada?}")]
        public async Task<decimal> GetSueldo(int? idPuesto, int? idClase, int? idTabulador, int? idTurno, int? jornada)
        {
            decimal result;
            result =  await _logic.GetSueldo(idPuesto, idClase, idTabulador, idTurno, jornada);

            return result;
        }

        [HttpGet("[action]/{idDireccionCotizacion}")]
        public async Task<int> GetZonaDefault(int idDireccionCotizacion)
        {
            return await _logic.GetZonaDefault(idDireccionCotizacion);
        }

        [HttpGet("[action]/{idD}/{idCliente}/{idSucursal}")]
        public async Task<IEnumerable<CatalogoSueldoJornaleroDTO>> ObtenerSueldoJornal(int idD, int idCliente, int idSucursal)
        {
            return await _logic.ObtenerSueldoJornal(idD,idCliente, idSucursal);
        }

        [HttpGet("[action]/{idD}")]
        public async Task<int> GetEstadoDireccion(int idD)
        {
            return await _logic.GetEstadoDireccion(idD);
        }
    }
}
