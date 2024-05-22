using AutoMapper;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using SistemaVentasBatia.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;
using SistemaVentasBatia.Options;
using Microsoft.Extensions.Options;

namespace SistemaVentasBatia.Services
{
    public interface ICatalogosService
    {
        Task<List<CatalogoDTO>> ObtenerEstados();
        Task<List<CatalogoDTO>> ObtenerServicios();
        Task<List<CatalogoDTO>> ObtenerMunicipios(int idEstado);
        Task<List<CatalogoDTO>> ObtenerTiposInmueble();
        Task<List<CatalogoDTO>> ObtenerCatalogoPuestos();
        Task<List<CatalogoDTO>> ObtenerCatalogoServicios();
        Task<List<CatalogoDTO>> ObtenerCatalogoTurnos();
        Task<List<CatalogoDTO>> ObtenerCatalogoSucursalesCotizacion(int idCotizacion);
        Task<List<CatalogoDTO>> ObtenerCatalogoPuestosCotizacion(int idCotizacion);
        Task<List<CatalogoDTO>> ObtenerCatalogoProductos(Servicio servicio);
        Task<List<CatalogoDTO>> ObtenerCatalogoJornada();
        Task<List<CatalogoDTO>> ObtenerCatalogoClase();
        Task<IEnumerable<CatalogoDTO>> ObtenerCatalogoProductosGrupo(Servicio servicio, string grupo);
        Task<List<CatalogoDTO>> ObtenerCatalogoFamiliasPorIdServicio(int idServicio);
        Task<List<CatalogoDTO>> ObtenerCatalogoEmpresas();
        Task<List<CatalogoDTO>> ObtenerCatalogoVendedores();
        Task<List<CatalogoDTO>> ObtenerCatalogoEjecutivos();
        Task<List<CatalogoDTO>> ObtenerCatalogoGerentesLimpieza();
    }

    public class CatalogosService : ICatalogosService
    {
        private readonly ICatalogosRepository catalogosRepo;
        private readonly IMapper mapper;
        private readonly ProductoOption _option;

        public CatalogosService(ICatalogosRepository catalogosRepo, IMapper mapper, IOptions<ProductoOption> options)
        {
            this.catalogosRepo = catalogosRepo;
            this.mapper = mapper;
            _option = options.Value;
        }

        public async Task<List<CatalogoDTO>> ObtenerEstados()
        {
            var estados = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerEstados());

            return estados;
        }

        public async Task<List<CatalogoDTO>> ObtenerServicios()
        {
            var servicios = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerServicios());

            return servicios;
        }

        public async Task<List<CatalogoDTO>> ObtenerMunicipios(int idEstado)
        {
            var municipios = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerMunicipios(idEstado));

            return municipios;
        }

        public async Task<List<CatalogoDTO>> ObtenerTiposInmueble()
        {
            var tiposInmueble = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerTiposInmueble());

            return tiposInmueble;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoPuestos()
        {
            var puestos = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoPuestos());

            return puestos;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoServicios()
        {
            var servicios = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoServicios());

            return servicios;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoTurnos()
        {
            var turnos = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoTurnos());

            return turnos;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoJornada()
        {
            var turnos = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoJornada());

            return turnos;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoClase()
        {
            var turnos = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoClase());

            return turnos;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoSucursalesCotizacion(int idCotizacion)
        {
            var sucursales = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoDireccionesCotizacion(idCotizacion));

            return sucursales;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoPuestosCotizacion(int idCotizacion)
        {
            var puestos = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoPuestosCotizacion(idCotizacion));

            return puestos;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoProductos(Servicio servicio)
        {
            var productos = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoProductos(servicio));

            return productos;
        }

        public async Task<IEnumerable<CatalogoDTO>> ObtenerCatalogoProductosGrupo(Servicio servicio, string grupo)
        {
            int[] fams;
            switch (grupo.ToLower())
            {
                case "material":
                    fams = _option.Material;
                    break;
                case "uniforme":
                    fams = _option.Uniforme;
                    break;
                case "equipo":
                    fams = _option.Equipo;
                    break;
                case "herramienta":
                    fams = _option.Herramienta;
                    break;
                case "servicio":
                    fams = _option.Servicio;
                    break;
                case "materialope":
                    fams = _option.MaterialOpe;
                    break;
                case "uniformeope":
                    fams = _option.UniformeOpe;
                    break;
                case "equipoope":
                    fams = _option.EquipoOpe;
                    break;
                case "herramientaope":
                    fams = _option.HerramientaOpe;
                    break;
                case "servicioope":
                    fams = _option.ServicioOpe;
                    break;
                default:
                    fams = new int[] { };
                    break;
            }
            return mapper.Map<IEnumerable<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoProductosByFamilia(servicio, fams));
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoFamiliasPorIdServicio(int idServicio)
        {
            var familias = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoFamiliasPorIdServicio(idServicio));
            return familias;
        }
        public async Task<List<CatalogoDTO>> ObtenerCatalogoEmpresas()
        {
            var empresas = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoEmpresas());

            return empresas;
        }
        
        public async Task<List<CatalogoDTO>> ObtenerCatalogoVendedores()
        {
            var empresas = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoVendedores());

            return empresas;
        }
        
        public async Task<List<CatalogoDTO>> ObtenerCatalogoEjecutivos()
        {
            var empresas = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoEjecutivos());

            return empresas;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoGerentesLimpieza()
        {
            var empresas = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoGerentesLimpieza());

            return empresas;
        }
    }
}
