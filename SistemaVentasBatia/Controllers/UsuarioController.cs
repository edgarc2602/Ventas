using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _logic;

        public UsuarioController(IUsuarioService logic)
        {
            _logic = logic;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<UsuarioDTO>> Login(AccesoDTO dto)
        {
            return await _logic.Login(dto);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<bool>> AgregarUsuario([FromBody] UsuarioRegistro usuario)
        {
            bool result = false;
            result = await _logic.InsertarUsuario(usuario);
            return true;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<List<UsuarioGrafica>>> ObtenerCotizacionesUsuarios()
        {
            return await _logic.ObtenerCotizacionesUsuarios();
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<List<UsuarioGraficaMensual>>> ObtenerCotizacionesMensuales()
        {
            return await _logic.ObtenerCotizacionesMensuales();
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<List<AgregarUsuario>>> ObtenerUsuarios()
        {
            return await _logic.ObtenerUsuarios();
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<List<AgregarUsuario>>> ObtenerVendedores()
        {
            return await _logic.ObtenerVendedores();
        }

        [HttpGet("[action]/{idPersonal}")]
        public async Task<ActionResult<AgregarUsuario>> ObtenerUsuarioPorIdPersonal(int idPersonal)
        {
            return await _logic.ObtenerUsuarioPorIdPersonal(idPersonal);
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<bool>> ActualizarUsuario([FromBody] AgregarUsuario agregarusuario)
        {
            return await _logic.ActualizarUsuario(agregarusuario);
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<bool>> ActivarUsuario([FromBody]int idPersonal)
        {
            return await _logic.ActivarUsuario(idPersonal);
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<bool>> DesactivarUsuario([FromBody] int idPersonal)
        {
            return await _logic.DesactivarUsuario(idPersonal);
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<bool>> AgregarUsuario([FromBody] AgregarUsuario agregarusuario)
        {
            return await _logic.AgregarUsuario(agregarusuario);
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<bool>> EliminarUsuario([FromBody] int idPersonal)
        {
            return await _logic.EliminarUsuario(idPersonal);
        }
    }
}