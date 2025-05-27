using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Services;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _logic;
        private readonly IUsuarioService logic;
        public MaterialController(IMaterialService service, IUsuarioService _usuarioService)
        {
            _logic = service;
            logic = _usuarioService;
        }

        [HttpGet("{id}/{pagina}")]
        public async Task<ActionResult<ListaMaterialesCotizacionLimpiezaDTO>> LimpiezaMaterial(int idDir, int idPues, string keywords, int id, int pagina = 1)
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

            await _logic.ObtenerListaMaterialesCotizacion(listaMaterialesVM);

            return listaMaterialesVM;
        }

        [HttpGet("[action]/{idCotizacion}/{idDireccionCotizacion}")]
        public async Task<ActionResult<ListaServiciosCotizacionLimpiezaDTO>> ObtenerListaServiciosCotizacion(int idCotizacion = 0, int idDireccionCotizacion = 0)
        {
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            var listaServiciosVM = new ListaServiciosCotizacionLimpiezaDTO()
            {
                IdCotizacion = idCotizacion,
            };

            listaServiciosVM = await _logic.ObtenerListaServiciosCotizacion(idCotizacion, idDireccionCotizacion);

            return listaServiciosVM;
        }

        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<ServicioCotizacionDTO>> ServicioGetById(int id)
        {
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            var servicio = new ServicioCotizacionDTO();

            servicio = await _logic.ServicioGetById(id);
            return servicio;
        }

        [HttpPost("[action]")]
        public async Task InsertarServicioCotizacion(ServicioCotizacion servicio)
        {
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            await _logic.InsertarServicioCotizacion(servicio);

        }

        [HttpPost("[action]")]
        public async Task ActualizarServicioCotizacion(ServicioCotizacion servicio)
        {
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            await _logic.ActualizarServicioCotizacion(servicio);

        }

        [HttpDelete("[action]/{id}")]
        public async Task EliminarServicioCotizacion(int id)
        {
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            await _logic.EliminarServicioCotizacion(id);

        }

        [HttpGet("[action]/{idPuestoDireccionCotizacion}")]
        public async Task<ActionResult<ListaMaterialesCotizacionLimpiezaDTO>> GetByPuesto(int idPuestoDireccionCotizacion)
        {
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            var materialCotizacion = await _logic.ObtenerListaMaterialesOperarioLimpieza(idPuestoDireccionCotizacion);

            return materialCotizacion;
        }

        [HttpGet("[action]/{idMaterialCotizacion}")]
        public async Task<ActionResult<MaterialCotizacionDTO>> GetById(int idMaterialCotizacion)
        {
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await _logic.ObtenerMaterialCotizacionPorId(idMaterialCotizacion);
        }

        [HttpPost]
        public async Task<ActionResult<MaterialCotizacionDTO>> AgregarMaterialOperario([FromBody] MaterialCotizacionDTO materialVM)
        {
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            await _logic.AgregarMaterialOperario(materialVM);

            return materialVM;
        }

        [HttpPut]
        public async Task<ActionResult<MaterialCotizacionDTO>> EditarMaterialOperario([FromBody] MaterialCotizacionDTO materialVM)
        {
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            await _logic.ActualizarMaterialCotizacion(materialVM);

            return materialVM;
        }

        [HttpDelete("{registroAEliminar}")]
        public async Task<ActionResult<bool>> EliminarMaterialOperario(int registroAEliminar)
        {
            var token = logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            await _logic.EliminarMaterialDeCotizacion(registroAEliminar);

            return true;
        }
    }
}