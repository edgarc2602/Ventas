using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Services;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HerramientaController : ControllerBase
    {
        private readonly IMaterialService _logic;
        private readonly IUsuarioService logic;
        public HerramientaController(IMaterialService service,IUsuarioService _usuarioService)
        {
            _logic = service;
            logic = _usuarioService;
        }

        [HttpGet("{id}/{pagina}")]
        public async Task<ActionResult<ListaMaterialesCotizacionLimpiezaDTO>> Get(int idDir, int idPues, string keywords, int id, int pagina = 1)
        {
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

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
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            var herramientaCotizacion = await _logic.ObtenerListaHerramientaOperario(idPuestoDireccion);
            return herramientaCotizacion;
        }

        [HttpGet("[action]/{idHerramientaCotizacion}")]
        public async Task<ActionResult<MaterialCotizacionDTO>> GetById(int idHerramientaCotizacion)
        {
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await _logic.ObtenerHerramientaCotizacionPorId(idHerramientaCotizacion);
        }

        [HttpPost]
        public async Task<ActionResult<MaterialCotizacionDTO>> Create([FromBody] MaterialCotizacionDTO herramienta)
        {
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            await _logic.AgregarHerramientaOperario(herramienta);
            return herramienta;
        }

        [HttpPut]
        public async Task<ActionResult<MaterialCotizacionDTO>> Update([FromBody] MaterialCotizacionDTO herramienta)
        {
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            await _logic.ActualizarHerramientaOperario(herramienta);
            return herramienta;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            await _logic.EliminarHerramientaCotizacion(id);
            return true;
        }
    }
}
