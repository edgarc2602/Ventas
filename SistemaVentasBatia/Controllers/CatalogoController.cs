using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Services;
using SistemaVentasBatia.Enums;
using System.Diagnostics.Eventing.Reader;

namespace SistemaVentasBatia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogoController : ControllerBase
    {
        private readonly ICatalogosService logic;

        public CatalogoController(ICatalogosService service)
        {
            logic = service;
        }

        [HttpGet("[action]/{idEstado}")]
        public async Task<IEnumerable<CatalogoDTO>> GetMunicipio(int idEstado)
        {
            return await logic.ObtenerMunicipios(idEstado);
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> GetEstado()
        {
            return await logic.ObtenerEstados();
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> GetServicio()
        {
            return await logic.ObtenerServicios();
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> GetInmuebleTipo()
        {
            return await logic.ObtenerTiposInmueble();
        }

        [HttpGet("[action]/{id}")]
        public async Task<IEnumerable<CatalogoDTO>> GetSucursalByCot(int id)
        {
            return await logic.ObtenerCatalogoSucursalesCotizacion(id);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IEnumerable<CatalogoDTO>> GetPuestoByCot(int id)
        {
            return await logic.ObtenerCatalogoPuestosCotizacion(id);
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> GetTurno()
        {
            return await logic.ObtenerCatalogoTurnos();
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> GetPuesto()
        {
            return await logic.ObtenerCatalogoPuestos();
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> GetTipoServicio()
        {
            return await logic.ObtenerCatalogoServicios();
        }

        [HttpGet("[action]/{servicio}")]
        public async Task<IEnumerable<CatalogoDTO>> GetProductoBySer(Servicio servicio)
        {
            return await logic.ObtenerCatalogoProductos(servicio);
        }

        [HttpGet("[action]/{servicio}/{grupo}")]
        public async Task<IEnumerable<CatalogoDTO>> GetProductoByGrupo(Servicio servicio, string grupo)
        {
            return await logic.ObtenerCatalogoProductosGrupo(servicio, grupo);
        }

        [HttpGet("[action]/{grupo}/{idCotizacion}")]
        public async Task<IEnumerable<CatalogoDTO>> GetProductoByGrupoElimina(string grupo, int idCotizacion)
        {
            return await logic.ObtenerCatalogoProductosGrupoElimina(grupo, idCotizacion);
        }

        [HttpGet("[action]")]
        public IEnumerable<Item<int>> GetDia()
        {
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
            List<string> ls = Enumerable.Range(0, 24)
                .Select(d => DateTime.Today.AddHours(d).ToString("HH:mm"))
                .ToList();

            return ls;
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> GetJornada()
        {
            return await logic.ObtenerCatalogoJornada();
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> GetClase()
        {
            return await logic.ObtenerCatalogoClase();
        }

        [HttpGet("[action]/{idServicio}")]
        public async Task<IEnumerable<CatalogoDTO>> ObtenerCatalogoFamiliasPorIdServicio(int idServicio)
        {
            return await logic.ObtenerCatalogoFamiliasPorIdServicio(idServicio);
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> ObtenerEmpresas()
        {
            return await logic.ObtenerCatalogoEmpresas();
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> ObtenerCatalogoVendedores()
        {
            return await logic.ObtenerCatalogoVendedores();
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> ObtenerCatalogoEjecutivos()
        {
            return await logic.ObtenerCatalogoEjecutivos();
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> ObtenerCatalogoGerentesLimpieza()
        {
            return await logic.ObtenerCatalogoGerentesLimpieza();
        }
        
        [HttpGet("[action]")]
        public async Task<IEnumerable<CatalogoDTO>> ObtenerCatalogoTiposdeIndustria()
        {
            return await logic.ObtenerCatalogoTiposdeIndustria();
        }
        
        [HttpGet("[action]/{idEstado}")]
        public async Task<IEnumerable<CatalogoDTO>> GetCatalogoClientes(int idEstado)
        {
            return await logic.GetCatalogoClientes(idEstado);
        }
        [HttpGet("[action]/{idEstado}/{idCliente}")]
        public async Task<IEnumerable<CatalogoDTO>> GetCatalogoSucursalesCliente(int idEstado, int idCliente)
        {
            return await logic.GetCatalogoSucursalesCliente(idEstado, idCliente);
        }

    }
}