﻿using Microsoft.AspNetCore.Authorization;
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
    public class UniformeController : ControllerBase
    {
        private readonly IMaterialService _logic;
        private readonly IUsuarioService _logicS;

        public UniformeController(IMaterialService service, IUsuarioService logicS)
        {
            _logic = service;
            _logicS = logicS;
        }

        [HttpGet("{id}/{pagina}")]
        public async Task<ActionResult<ListaMaterialesCotizacionLimpiezaDTO>> Get(int idDir, int idPues, string keywords, int id, int pagina = 1)
        {
            ListaMaterialesCotizacionLimpiezaDTO listaMaterialesVM = new ListaMaterialesCotizacionLimpiezaDTO()
            {
                Pagina = pagina,
                IdCotizacion = id,
                IdDireccionCotizacion = idDir,
                IdPuestoDireccionCotizacion = idPues,
                Keywords = keywords
            };
            await _logic.ObtenerListaUniformeCotizacion(listaMaterialesVM);

            return listaMaterialesVM;
        }

        [HttpGet("[action]/{idPuestoDireccion}")]
        public async Task<ActionResult<ListaMaterialesCotizacionLimpiezaDTO>> GetByPuesto(int idPuestoDireccion)
        {
            var uniformeCotizacion = await _logic.ObtenerListaUniformeOperario(idPuestoDireccion);
            return uniformeCotizacion;
        }

        [HttpGet("[action]/{idUniformeCotizacion}")]
        public async Task<ActionResult<MaterialCotizacionDTO>> GetById(int idUniformeCotizacion)
        {
            var token = _logicS.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await _logic.ObtenerUniformeCotizacionPorId(idUniformeCotizacion);
        }

        [HttpPost]
        public async Task<ActionResult<MaterialCotizacionDTO>> Create([FromBody] MaterialCotizacionDTO uniforme)
        {
            await _logic.AgregarUniformeOperario(uniforme);
            return uniforme;
        }

        [HttpPut]
        public async Task<ActionResult<MaterialCotizacionDTO>> Update([FromBody] MaterialCotizacionDTO uniforme)
        {
            await _logic.ActualizarUniformeCotizacion(uniforme);
            return uniforme;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            await _logic.EliminarUniformeCotizacion(id);
            return true;
        }
    }
}
