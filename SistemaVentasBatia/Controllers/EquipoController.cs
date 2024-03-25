using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Services;

namespace SistemaVentasBatia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipoController : ControllerBase
    {
        private readonly IMaterialService _logic;

        public EquipoController(IMaterialService service)
        {
            _logic = service;
        }

        [HttpGet("{id}/{pagina}")]
        public async Task<ActionResult<ListaMaterialesCotizacionLimpiezaDTO>> Get(int idDir, int idPues, string keywords, int id, int pagina = 1)
        {
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
            var equipoCotizacion = await _logic.ObtenerListaEquipoOperario(idPuestoDireccion);
            return equipoCotizacion;
        }

        [HttpGet("[action]/{idEquipoCotizacion}")]
        public async Task<ActionResult<MaterialCotizacionDTO>> GetById(int idEquipoCotizacion)
        {
            return await _logic.ObtenerEquipoCotizacionPorId(idEquipoCotizacion);
        }

        [HttpPost]
        public async Task<ActionResult<MaterialCotizacionDTO>> Create([FromBody] MaterialCotizacionDTO equipo)
        {
            await _logic.AgregarEquipoOperario(equipo);
            return equipo;
        }

        [HttpPut]
        public async Task<ActionResult<MaterialCotizacionDTO>> Update([FromBody] MaterialCotizacionDTO equipo)
        {
            await _logic.ActualizarEquipoCotizacion(equipo);
            return equipo;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            await _logic.EliminarEquipoCotizacion(id);
            return true;
        }
    }
}
