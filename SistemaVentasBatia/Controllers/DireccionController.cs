using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Services;

namespace SistemaVentasBatia.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DireccionController : ControllerBase
    {
        private readonly ILogger<DireccionController> _logger;
        private readonly IProspectosService _prospectosSvc;
        private readonly ICotizacionesService _cotizaSvc;
        private readonly IUsuarioService _logic;
        public DireccionController(
            ILogger<DireccionController> logger, IProspectosService prospectosSvc
          , ICotizacionesService cotizaSvc, IUsuarioService _usuarioService)
        {
            _logger = logger;
            _prospectosSvc = prospectosSvc;
            _cotizaSvc = cotizaSvc;
            _logic = _usuarioService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ListaDireccionDTO>> Directorio(int id)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            var listaDireccionesVM = new ListaDireccionDTO()
            {
                IdProspecto = id,
                Pagina = 1
            };

            await _prospectosSvc.ObtenerListaDirecciones(listaDireccionesVM);

            return listaDireccionesVM;
        }

        [HttpGet("[action]/{id}")]
        public async Task<IEnumerable<Item<int>>> GetCatalogo(int id)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            IEnumerable<Item<int>> ls = (await _cotizaSvc.ObtenerCatalogoDireccionesPorProspecto(id)).Select(x => new Item<int> { Id = x.IdDireccion, Nom = x.NombreSucursal });

            return ls;
        }

        [HttpPost]
        public async Task<ActionResult<DireccionDTO>> AgregarDireccion([FromBody] DireccionDTO direccionVM)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _prospectosSvc.CrearDireccion(direccionVM);

            return direccionVM;
        }

        [HttpGet("[action]/{idD}/{idP}")]
        public async Task<ActionResult<DireccionDTO>> ObtenerDireccion(int idD = 0, int idP = 0)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            var direccion = await _prospectosSvc.ObtenerDireccionPorId(idD);

            direccion.IdProspecto = idP;

            // ViewBag.Estados = new List<SelectListItem>((await catalogosSvc.ObtenerEstados()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Descripcion }));

            // ViewBag.TiposInmueble = new List<SelectListItem>((await catalogosSvc.ObtenerTiposInmueble()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Descripcion }));

            // return PartialView("_ModalEditarDireccion", direccion);
            return direccion;
        }

        [HttpPut]
        public async Task<ActionResult<DireccionDTO>> EditarDireccion([FromBody] DireccionDTO direccionVM)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _prospectosSvc.ActualizarDireccion(direccionVM);

            return direccionVM;
        }

        [HttpGet("[action]/{cp}")]
        public async Task <ActionResult<DireccionResponseAPIDTO>> GetDireccionAPI(string cp = "")
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await _prospectosSvc.GetDireccionAPI(cp);
        }
    }
}