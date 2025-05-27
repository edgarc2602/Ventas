using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SalarioController : ControllerBase
    {
        private readonly ISalarioService _logic;
        private readonly IUsuarioService logicuse;
        public SalarioController(ISalarioService logic, IUsuarioService _usuarioService)
        {
            _logic = logic;
            logicuse = _usuarioService;
        }

        [HttpGet("{idtab}/{idpue}/{idtur}")]
        public async Task<SalarioMinDTO> GetFind(int idtab, int idpue, int idtur)
        {
            var token = logicuse.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await _logic.GetFind(idtab, idpue, idtur);
        }

        [HttpGet("{idPuesto?}/{idClase?}/{idTabulador?}/{idTurno?}/{jornada?}")]
        public async Task<decimal> GetSueldo(int? idPuesto, int? idClase, int? idTabulador, int? idTurno, int? jornada)
        {
            var token = logicuse.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            decimal result;
            result = await _logic.GetSueldo(idPuesto, idClase, idTabulador, idTurno, jornada);

            return result;
        }

        [HttpGet("[action]/{idDireccionCotizacion}")]
        public async Task<int> GetZonaDefault(int idDireccionCotizacion)
        {
            var token = logicuse.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await _logic.GetZonaDefault(idDireccionCotizacion);
        }

        [HttpGet("[action]/{idD}/{idCliente}/{idSucursal}")]
        public async Task<IEnumerable<CatalogoSueldoJornaleroDTO>> ObtenerSueldoJornal(int idD, int idCliente, int idSucursal)
        {
            var token = logicuse.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await _logic.ObtenerSueldoJornal(idD, idCliente, idSucursal);
        }

        [HttpGet("[action]/{idD}")]
        public async Task<int> GetEstadoDireccion(int idD)
        {
            var token = logicuse.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await _logic.GetEstadoDireccion(idD);
        }
    }
}
