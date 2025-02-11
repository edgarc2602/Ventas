using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.DTOs;  
using SistemaVentasBatia.Services;

namespace SistemaVentasBatia.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PuestoController : ControllerBase
    {
        private readonly ICotizacionesService _logic;
        private readonly IProspectosService _logicPro;

        public PuestoController(ICotizacionesService service, IProspectosService servicePro)
        {
            _logic = service;
            _logicPro = servicePro;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PuestoDireccionCotizacionDTO>> EditarOperario(int id)
        {
            var operario = await _logicPro.ObtenerOperarioPorId(id);

            // TempData["Turnos"] = new List<SelectListItem>((await catalogosSvc.ObtenerCatalogoTurnos()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Descripcion }));

            // TempData["Puestos"] = new List<SelectListItem>((await catalogosSvc.ObtenerCatalogoPuestos()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Descripcion }));

            // return PartialView("ModalLimpiezaEditarOperario", operario);
            return operario;
        }

        [HttpPost("{idServicio}")]
        public async Task<ActionResult<PuestoDireccionCotizacionDTO>> AgregarOperario([FromBody] PuestoDireccionCotizacionDTO operarioVM, int idServicio)
        {
            await _logic.CrearPuestoDireccionCotizacion(operarioVM, idServicio);

            // TempData["DescripcionAlerta"] = "Se agregó correctamente al operario.";
            // TempData["IdTipoAlerta"] = TipoAlerta.Exito;

            return operarioVM;
        }

        [HttpPut("{incluyeMaterial}/{idServicio}")]
        public async Task<ActionResult<bool>> EditarOperario([FromBody] PuestoDireccionCotizacionDTO operarioVM, bool incluyeMaterial, int idServicio)
        {
            await _logic.ActualizarPuestoDireccionCotizacion(operarioVM, incluyeMaterial, idServicio);

            operarioVM.IdCotizacion = await _logic.ObtieneIdCotizacionPorOperario(operarioVM.IdPuestoDireccionCotizacion);
            // TempData["DescripcionAlerta"] = "Se guardó correctamente el operario.";
            // TempData["IdTipoAlerta"] = TipoAlerta.Exito;
            return true;
        }

        [HttpDelete("{registroAEliminar}")]
        public async Task<ActionResult<bool>> EliminarOperador(int registroAEliminar)
        {
            var idDireccionCotizacion = await _logic.ObtenerIdDireccionCotizacionPorOperario(registroAEliminar);

            var idCotizacion = await _logic.ObtenerIdCotizacionPorDireccion(idDireccionCotizacion);

            await _logic.EliminarOperario(registroAEliminar);

            //TempData["DescripcionAlerta"] = "Se quitó correctamente el operario de la plantilla";
            //TempData["IdTipoAlerta"] = TipoAlerta.Info;

            return true;
        }
    }
}