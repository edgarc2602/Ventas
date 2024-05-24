using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaVentasBatia.Services;
using SistemaVentasBatia.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;
using SistemaVentasBatia.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace SistemaVentasBatia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService clienteSvc;

        public ClienteController( IClienteService clienteSvc, ICatalogosService catalogosSvc, ICotizacionesService cotizacionesSvc)
        {
            this.clienteSvc = clienteSvc;
        }

        [HttpGet("[action]/{idProspecto}")]
        public async Task<ActionResult<ClienteContratoDTO>> ObtenerDatosExistentesClienteContrato(int idProspecto)
        {
            return await clienteSvc.ObtenerDatosClienteContrato(idProspecto);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<bool>> InsertarDatosClienteContrato(ClienteContratoDTO contrato)
        {
            return await clienteSvc.InsetarDatosClienteContrato(contrato);
        }

        [HttpPost("[action]/{direccionIP}")]
        public async Task<ActionResult<int>> ConvertirProspectoACliente(string direccionIP,ClienteDTO cliente, [FromServices] IHttpContextAccessor httpContextAccessor)
        {

            return await clienteSvc.ConvertirProspectoACliente(cliente, direccionIP);
        }

        [HttpPost("[action]")]
        public async Task<bool> InsertarContratoCliente([FromForm] int idClienteGenerado, [FromForm] string nombreComercial, [FromForm] IFormFile contratoSeleccionado)
        {
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
