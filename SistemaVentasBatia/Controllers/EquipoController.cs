using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Services;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EquipoController : ControllerBase
    {
        private readonly IMaterialService _logic;
        private readonly IUsuarioService logic;
        public EquipoController(IMaterialService service, IUsuarioService _usuarioService)
        {
            _logic = service;
            logic = _usuarioService;
        }

        [HttpGet("{id}/{pagina}")]
        public async Task<ActionResult<ListaMaterialesCotizacionLimpiezaDTO>> Get(int idDir, int idPues, string keywords, int id, int pagina = 1)
        {
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            ListaMaterialesCotizacionLimpiezaDTO listaMaterialesVM = new ListaMaterialesCotizacionLimpiezaDTO()
            {
                Pagina = pagina,
                IdCotizacion = id,
                IdDireccionCotizacion = idDir,
                IdPuestoDireccionCotizacion = idPues,
                Keywords = keywords
            };
            await _logic.ObtenerListaEquipoCotizacion(listaMaterialesVM);

            return listaMaterialesVM;
        }

        [HttpGet("[action]/{idPuestoDireccion}")]
        public async Task<ActionResult<ListaMaterialesCotizacionLimpiezaDTO>> GetByPuesto(int idPuestoDireccion)
        {
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            var equipoCotizacion = await _logic.ObtenerListaEquipoOperario(idPuestoDireccion);
            return equipoCotizacion;
        }

        [HttpGet("[action]/{idEquipoCotizacion}")]
        public async Task<ActionResult<MaterialCotizacionDTO>> GetById(int idEquipoCotizacion)
        {
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await _logic.ObtenerEquipoCotizacionPorId(idEquipoCotizacion);
        }

        [HttpPost]
        public async Task<ActionResult<MaterialCotizacionDTO>> Create([FromBody] MaterialCotizacionDTO equipo)
        {
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            await _logic.AgregarEquipoOperario(equipo);
            return equipo;
        }

        [HttpPut]
        public async Task<ActionResult<MaterialCotizacionDTO>> Update([FromBody] MaterialCotizacionDTO equipo)
        {
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            await _logic.ActualizarEquipoCotizacion(equipo);
            return equipo;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            await _logic.EliminarEquipoCotizacion(id);
            return true;
        }
    }
}
