    using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Services;
using System.Threading.Tasks;

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

        [HttpGet("{idPuesto?}/{idClase?}/{idTabulador?}/{idTurno?}")]
        public async Task<decimal> GetSueldo(int? idPuesto, int? idClase, int? idTabulador, int? idTurno)
        {
            decimal result;
            result =  await _logic.GetSueldo(idPuesto, idClase, idTabulador, idTurno);

            return result;
        }

        [HttpGet("[action]/{idDireccionCotizacion}")]
        public async Task<int> GetZonaDefault(int idDireccionCotizacion)
        {
            return await _logic.GetZonaDefault(idDireccionCotizacion);
        }

    }
}
