using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _logic;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsuarioController(IUsuarioService logic, IHttpContextAccessor httpContextAccessor)
        {
            _logic = logic;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpPost("[action]")]
        public async Task<ActionResult<UsuarioDTO>> Login(AccesoDTO dto)
        {
            var usu = new UsuarioDTO();
            var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
            usu.DireccionIP = ipAddress.ToString();
            var usuario = await _logic.Login(dto, usu);

            if (usuario == null)
            {
                return Unauthorized(new { message = "Credenciales incorrectas" });
            }

            // Generar el token JWT
            var key = Encoding.ASCII.GetBytes("S1ng4*2025_crm_key_ClaveSeguraParaSingaCRM.@");
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, usuario.Nombre) }),
                Expires = DateTime.UtcNow.AddMinutes(50),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Agregar el token en los headers de la respuesta
            Response.Headers.Add("Authorization", $"Bearer {tokenString}");

            return Ok(usuario); // Devuelve solo el objeto usuario
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<ActionResult<bool>> AgregarUsuario([FromBody] UsuarioRegistro usuario)
        {
            bool result = false;
            result = await _logic.InsertarUsuario(usuario);
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return true;
        }

        [Authorize]
        [HttpGet("[action]")]
        public async Task<ActionResult<List<UsuarioGrafica>>> ObtenerCotizacionesUsuarios()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await _logic.ObtenerCotizacionesUsuarios();
        }

        [Authorize]
        [HttpGet("[action]")]
        public async Task<ActionResult<List<UsuarioGraficaMensual>>> ObtenerCotizacionesMensuales()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await _logic.ObtenerCotizacionesMensuales();
        }

        [Authorize]
        [HttpGet("[action]")]
        public async Task<ActionResult<List<AgregarUsuario>>> ObtenerUsuarios()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await _logic.ObtenerUsuarios();
        }

        [Authorize]
        [HttpGet("[action]")]
        public async Task<ActionResult<List<AgregarUsuario>>> ObtenerVendedores()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await _logic.ObtenerVendedores();
        }

        [Authorize]
        [HttpGet("[action]/{idPersonal}")]
        public async Task<ActionResult<AgregarUsuario>> ObtenerUsuarioPorIdPersonal(int idPersonal)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await _logic.ObtenerUsuarioPorIdPersonal(idPersonal);
        }

        [Authorize]
        [HttpPut("[action]")]
        public async Task<ActionResult<bool>> ActualizarUsuario([FromBody] AgregarUsuario agregarusuario)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await _logic.ActualizarUsuario(agregarusuario);
        }

        [Authorize]
        [HttpPut("[action]")]
        public async Task<ActionResult<bool>> ActivarUsuario([FromBody] int idPersonal)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await _logic.ActivarUsuario(idPersonal);
        }

        [Authorize]
        [HttpPut("[action]")]
        public async Task<ActionResult<bool>> DesactivarUsuario([FromBody] int idPersonal)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await _logic.DesactivarUsuario(idPersonal);
        }

        [Authorize]
        [HttpPut("[action]")]
        public async Task<ActionResult<bool>> AgregarUsuario([FromBody] AgregarUsuario agregarusuario)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await _logic.AgregarUsuario(agregarusuario);
        }

        [Authorize]
        [HttpPut("[action]")]
        public async Task<ActionResult<bool>> EliminarUsuario([FromBody] int idPersonal)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await _logic.EliminarUsuario(idPersonal);
        }
    }
}