using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Services;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HerramientaController : ControllerBase
    {
        private readonly IMaterialService _logic;

        public HerramientaController(IMaterialService service)
        {
            _logic = service;
        }

        [HttpGet("{id}/{pagina}")]
        public async Task<ActionResult<ListaMaterialesCotizacionLimpiezaDTO>> Get(int idDir, int idPues, string keywords, int id, int pagina = 1)
        {
            var listaMaterialesVM = new ListaMaterialesCotizacionLimpiezaDTO()
            {
                Pagina = pagina,
                IdCotizacion = id,
                IdDireccionCotizacion = idDir,
                IdPuestoDireccionCotizacion = idPues,
                Keywords = keywords
            };

            await _logic.ObtenerListaHerramientaCotizacion(listaMaterialesVM);

            return listaMaterialesVM;
        }

        [HttpGet("[action]/{idPuestoDireccion}")]
        public async Task<ActionResult<ListaMaterialesCotizacionLimpiezaDTO>> GetByPuesto(int idPuestoDireccion)
        {
            var herramientaCotizacion = await _logic.ObtenerListaHerramientaOperario(idPuestoDireccion);
            return herramientaCotizacion;
        }

        [HttpGet("[action]/{idHerramientaCotizacion}")]
        public async Task<ActionResult<MaterialCotizacionDTO>> GetById(int idHerramientaCotizacion)
        {
            return await _logic.ObtenerHerramientaCotizacionPorId(idHerramientaCotizacion);
        }

        [HttpPost]
        public async Task<ActionResult<MaterialCotizacionDTO>> Create([FromBody] MaterialCotizacionDTO herramienta)
        {
            await _logic.AgregarHerramientaOperario(herramienta);
            return herramienta;
        }

        [HttpPut]
        public async Task<ActionResult<MaterialCotizacionDTO>> Update([FromBody] MaterialCotizacionDTO herramienta)
        {
            await _logic.ActualizarHerramientaOperario(herramienta);
            return herramienta;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            await _logic.EliminarHerramientaCotizacion(id);
            return true;
        }
    }
}
