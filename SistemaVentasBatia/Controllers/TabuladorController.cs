using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TabuladorController : ControllerBase
    {
        private readonly ITabuladorService _logic;
        private readonly IUsuarioService logicuse;
        public TabuladorController(ITabuladorService logic, IUsuarioService _usuarioService)
        {
            _logic = logic;
            logicuse = _usuarioService;
        }

        [HttpGet("[action]/{id}")]
        public async Task<IEnumerable<CatalogoDTO>> GetByEdo(int id)
        {
            var token = logicuse.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await _logic.GetPorEstado(id);
        }

        [HttpGet("[action]/{id}/{idClase}")]
        public async Task<PuestoTabulador> ObtenerTabuladorPuesto(int id, int idClase)
        {
            var token = logicuse.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            PuestoTabulador result = new PuestoTabulador();
            result = await _logic.ObtenerTabuladorPuesto(id, idClase);
            return result;
        }
    }
}
