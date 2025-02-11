using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Services;
using SistemaVentasBatia.Enums;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVentasBatia.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogoController : ControllerBase
    {
        private readonly ICatalogosService logic;
        private readonly IUsuarioService _logic;

        public CatalogoController(ICatalogosService service, IUsuarioService _usuarioService)
        {
            logic = service;
            _logic = _usuarioService;
        }

        [HttpGet("[action]/{idEstado}")]
        public async Task<IEnumerable<CatalogoDTO>> GetMunicipio(int idEstado)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.ObtenerMunicipios(idEstado);
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> GetEstado()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.ObtenerEstados();
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> GetServicio()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.ObtenerServicios();
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> GetInmuebleTipo()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.ObtenerTiposInmueble();
        }

        [HttpGet("[action]/{id}")]
        public async Task<IEnumerable<CatalogoDTO>> GetSucursalByCot(int id)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.ObtenerCatalogoSucursalesCotizacion(id);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IEnumerable<CatalogoDTO>> GetPuestoByCot(int id)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.ObtenerCatalogoPuestosCotizacion(id);
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> GetTurno()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.ObtenerCatalogoTurnos();
        }

        [HttpGet("[action]/{idServicio}")]
        public async Task<IEnumerable<CatalogoDTO>> GetPuesto(int idServicio)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.ObtenerCatalogoPuestos(idServicio);
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> GetTipoServicio()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.ObtenerCatalogoServicios();
        }

        [HttpGet("[action]/{servicio}")]
        public async Task<IEnumerable<CatalogoDTO>> GetProductoBySer(Servicio servicio)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.ObtenerCatalogoProductos(servicio);
        }

        [HttpGet("[action]/{servicio}/{grupo}")]
        public async Task<IEnumerable<CatalogoDTO>> GetProductoByGrupo(Servicio servicio, string grupo)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.ObtenerCatalogoProductosGrupo(servicio, grupo);
        }

        [HttpGet("[action]/{grupo}/{idCotizacion}")]
        public async Task<IEnumerable<CatalogoDTO>> GetProductoByGrupoElimina(string grupo, int idCotizacion)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.ObtenerCatalogoProductosGrupoElimina(grupo, idCotizacion);
        }

        [HttpGet("[action]")]
        public IEnumerable<Item<int>> GetDia()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            List<Item<int>> ls = Enum.GetValues(typeof(DiaSemana))
                .Cast<DiaSemana>().Select(d => new Item<int>
                {
                    Id = (int)d,
                    Nom = d.ToString(),
                    Act = false
                }).ToList();

            return ls;
        }

        [HttpGet("[action]")]
        public IEnumerable<Item<int>> GetFrecuencia()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            List<Item<int>> ls = Enum.GetValues(typeof(Frecuencia))
                .Cast<Frecuencia>().Select(d => new Item<int>
                {
                    Id = (int)d,
                    Nom = d.ToString(),
                    Act = false
                }).ToList();

            return ls;
        }

        [HttpGet("[action]")]
        public IEnumerable<string> GetHorario()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            List<string> ls = Enumerable.Range(0, 24)
                .Select(d => DateTime.Today.AddHours(d).ToString("HH:mm"))
                .ToList();

            return ls;
        }

        [HttpGet("[action]/{idServicio}")]
        public async Task<IEnumerable<CatalogoDTO>> GetJornada(int idServicio)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.ObtenerCatalogoJornada(idServicio);
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> GetClase()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.ObtenerCatalogoClase();
        }

        [HttpGet("[action]/{idServicio}")]
        public async Task<IEnumerable<CatalogoDTO>> ObtenerCatalogoFamiliasPorIdServicio(int idServicio)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.ObtenerCatalogoFamiliasPorIdServicio(idServicio);
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> ObtenerEmpresas()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.ObtenerCatalogoEmpresas();
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> ObtenerCatalogoVendedores()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.ObtenerCatalogoVendedores();
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> ObtenerCatalogoEjecutivos()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.ObtenerCatalogoEjecutivos();
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> ObtenerCatalogoGerentesLimpieza()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.ObtenerCatalogoGerentesLimpieza();
        }
        
        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> ObtenerCatalogoTiposdeIndustria()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.ObtenerCatalogoTiposdeIndustria();
        }
        
        [HttpGet("[action]/{idEstado}")]
        public async Task<IEnumerable<CatalogoDTO>> GetCatalogoClientes(int idEstado)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.GetCatalogoClientes(idEstado);
        }
        [HttpGet("[action]/{idEstado}/{idCliente}")]
        public async Task<IEnumerable<CatalogoDTO>> GetCatalogoSucursalesCliente(int idEstado, int idCliente)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");
            return await logic.GetCatalogoSucursalesCliente(idEstado, idCliente);
        }

    }
}