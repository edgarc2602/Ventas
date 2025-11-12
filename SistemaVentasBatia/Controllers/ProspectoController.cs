using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Enums;
using SistemaVentasBatia.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProspectoController : ControllerBase
    {
        private readonly ILogger<ProspectoController> _logger;
        private readonly IProspectosService prospectosSvc;
        private readonly ICatalogosService catalogosSvc;
        private readonly ICotizacionesService cotizacionesSvc;
        private readonly IUsuarioService _logic;


        public ProspectoController(ILogger<ProspectoController> logger, IProspectosService prospectosSvc, ICatalogosService catalogosSvc, ICotizacionesService cotizacionesSvc, IUsuarioService _usuarioService)
        {
            _logger = logger;
            this.prospectosSvc = prospectosSvc;
            this.catalogosSvc = catalogosSvc;
            this.cotizacionesSvc = cotizacionesSvc;
            _logic = _usuarioService;
        }

        [HttpGet("{idPersonal?}/{pagina?}/{idEstatus?}/{idGrupoActivo?}")]
        public async Task<ActionResult<ListaProspectoDTO>> Index([FromQuery] string keywords, int idPersonal = 0, int pagina = 1, int idEstatus = 0, int idGrupoActivo = 0)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            ListaProspectoDTO listaProspectosVM = new ListaProspectoDTO()
            {
                IdEstatusProspecto = (EstatusProspecto)idEstatus,
                Pagina = pagina,
                Keywords = keywords ?? ""
            };

            int autorizacion = await cotizacionesSvc.ObtenerAutorizacion(idPersonal);
            await prospectosSvc.ObtenerListaProspectos(listaProspectosVM, autorizacion, idPersonal, idGrupoActivo);

            return listaProspectosVM;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProspectoDTO>> EditarProspecto(int id)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await prospectosSvc.ObtenerProspecto(id);
        }

        [HttpPost("[action]/{idGrupoActivo}")]
        public async Task<IEnumerable<ProspectoDTO>> GetCatalogo(int idGrupoActivo, [FromBody] int idPersonal = 0)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            int autorizacion = await cotizacionesSvc.ObtenerAutorizacion(idPersonal);
            return await prospectosSvc.ObtenerCatalogoProspectos(autorizacion, idPersonal, idGrupoActivo);
        }

        [HttpPut]
        public async Task<ActionResult<ProspectoDTO>> EditarProspecto(ProspectoDTO prospectoVM)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            var coincidencias = await prospectosSvc.ObtenerCoincidenciasProspecto(nombreComercial: null, rfc: prospectoVM.Rfc);
            if (coincidencias.Count != 0)
            {
                if (coincidencias[0].IdProspecto != prospectoVM.IdProspecto)
                {
                    if (coincidencias.Count() > 0)
                    {
                        ModelState.AddModelError("Rfc", "Este RFC ya se encuentra registrado");
                    }
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }
                }
            }

            await prospectosSvc.EditarProspecto(prospectoVM);

            return Ok(prospectoVM);
        }

        [HttpDelete("{registroAEliminar}")]
        public async Task<ActionResult<bool>> EliminarProspecto(int registroAEliminar)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            await prospectosSvc.EliminarProspecto(registroAEliminar);

            return true;
        }

        [HttpPost]
        public async Task<ActionResult<ProspectoDTO>> NuevoProspecto([FromBody] ProspectoDTO prospectoVM)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            var coincidencias = await prospectosSvc.ObtenerCoincidenciasProspecto(nombreComercial: null, rfc: prospectoVM.Rfc);

            if (coincidencias.Count() > 0)
            {
                ModelState.AddModelError("Rfc", "Este RFC ya se encuentra registrado");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await prospectosSvc.CrearProspecto(prospectoVM);
            return prospectoVM;
        }

        [HttpGet]
        public async Task<int> ObtenerNumeroCoincidenciasProspecto(string nombreComercial = null, string rfc = null)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            var coincidencias = await prospectosSvc.ObtenerNumeroCoincidenciasProspecto(nombreComercial, rfc);

            return coincidencias;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCoincidenciasProspecto(string nombreComercial = null, string rfc = null)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            var coincidenciasProspecto = await prospectosSvc.ObtenerCoincidenciasProspecto(nombreComercial, rfc);

            return Ok(coincidenciasProspecto);
        }

        [HttpGet("[action]")]
        public IEnumerable<Item<int>> GetDocumento()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            List<Item<int>> ls = Enum.GetValues(typeof(Documento))
                .Cast<Documento>().Select(d => new Item<int>
                {
                    Id = (int)d,
                    Nom = d.ToString(),
                    Act = false
                }).ToList();

            return ls;
        }

        [HttpGet("[action]")]
        public IEnumerable<Item<int>> GetEstatus()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            List<Item<int>> ls = Enum.GetValues(typeof(EstatusProspecto))
                .Cast<EstatusProspecto>().Select(d => new Item<int>
                {
                    Id = (int)d,
                    Nom = d.ToString(),
                    Act = false
                }).ToList();

            return ls;
        }

        [HttpGet("[action]")]
        public IEnumerable<Item<int>> GetServicio()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            List<Item<int>> ls = Enum.GetValues(typeof(Servicio))
                .Cast<Servicio>().Select(s => new Item<int>
                {
                    Id = (int)s,
                    Nom = s.ToString(),
                    Act = false
                }).ToList();

            return ls;
        }

        [HttpGet("[action]")]
        public IEnumerable<Item<int>> GetSalarioTipo()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            List<Item<int>> ls = Enum.GetValues(typeof(SalarioTipo))
                .Cast<SalarioTipo>().Select(s => new Item<int>
                {
                    Id = (int)s,
                    Nom = s.ToString(),
                    Act = false
                }).ToList();
            ls[0].Act = true;

            return ls;
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<bool>> ActivarProspecto([FromBody] int idProspecto)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await prospectosSvc.ActivarProspecto(idProspecto);
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<bool>> DesactivarProspecto([FromBody] int idProspecto)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            await cotizacionesSvc.DesactivarCotizaciones(idProspecto);
            return await prospectosSvc.DesactivarProspecto(idProspecto);
        }

        [HttpGet("[action]/{idProspecto}")]
        public async Task<ActionResult<ProspectoDTO>> ObtenerDatosExistentesProspecto(int idProspecto)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await prospectosSvc.ObtenerDatosProspecto(idProspecto);
        }

        [HttpGet("[action]")]
        public async Task<List<ProspectoDTO>> ValidarProspectoExistente(
            [FromQuery] string nombreComercial,
            [FromQuery] string? razonSocial = null,
            [FromQuery] string rfc = null) {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await prospectosSvc.ValidarProspectoExistente(nombreComercial, 0, razonSocial, rfc);
        }



    }
}
