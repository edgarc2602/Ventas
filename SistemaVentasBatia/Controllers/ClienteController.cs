using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Services;
using System.IO;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService clienteSvc;
        private readonly IUsuarioService _logic;
        public ClienteController(IClienteService clienteSvc, ICatalogosService catalogosSvc, ICotizacionesService cotizacionesSvc, IUsuarioService _usuarioService)
        {
            this.clienteSvc = clienteSvc;
            _logic = _usuarioService;
        }

        [HttpGet("[action]/{idProspecto}")]
        public async Task<ActionResult<ClienteContratoDTO>> ObtenerDatosExistentesClienteContrato(int idProspecto)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await clienteSvc.ObtenerDatosClienteContrato(idProspecto);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<bool>> InsertarDatosClienteContrato(ClienteContratoDTO contrato)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await clienteSvc.InsetarDatosClienteContrato(contrato);
        }

        [HttpPost("[action]/{direccionIP}")]
        public async Task<ActionResult<int>> ConvertirProspectoACliente(string direccionIP, ClienteDTO cliente, [FromServices] IHttpContextAccessor httpContextAccessor)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await clienteSvc.ConvertirProspectoACliente(cliente, direccionIP);
        }

        [HttpPost("[action]")]
        public async Task<bool> InsertarContratoCliente([FromForm] int idClienteGenerado, [FromForm] string nombreComercial, [FromForm] IFormFile contratoSeleccionado)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            byte[] contratoBytes;
            using (var memoryStream = new MemoryStream())
            {
                await contratoSeleccionado.CopyToAsync(memoryStream);
                contratoBytes = memoryStream.ToArray();
            }
            return await clienteSvc.InsertarContratoCliente(contratoBytes, idClienteGenerado, nombreComercial);
        }
    }
}
