using AutoMapper;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Services
{
    public interface IMaterialService
    {
        Task AgregarMaterialOperario(MaterialCotizacionDTO materialVM);
        Task ActualizarMaterialCotizacion(MaterialCotizacionDTO materialVM);
        Task EliminarMaterialDeCotizacion(int registroAEliminar);
        Task AgregarEquipoOperario(MaterialCotizacionDTO dto);
        Task ActualizarEquipoCotizacion(MaterialCotizacionDTO dto);
        Task EliminarEquipoCotizacion(int idEquipo);
        Task AgregarHerramientaOperario(MaterialCotizacionDTO dto);
        Task ActualizarHerramientaOperario(MaterialCotizacionDTO dto);
        Task EliminarHerramientaCotizacion(int idHerramienta);
        Task AgregarUniformeOperario(MaterialCotizacionDTO dto);
        Task ActualizarUniformeCotizacion(MaterialCotizacionDTO dto);
        Task EliminarUniformeCotizacion(int idUniforme);
        Task ObtenerListaEquipoCotizacion(ListaMaterialesCotizacionLimpiezaDTO listaMateriales);
        Task ObtenerListaHerramientaCotizacion(ListaMaterialesCotizacionLimpiezaDTO listaMateriales);
        Task ObtenerListaMaterialesCotizacion(ListaMaterialesCotizacionLimpiezaDTO listaMateriales);
        Task ObtenerListaUniformeCotizacion(ListaMaterialesCotizacionLimpiezaDTO listaMateriales);
        Task<ListaMaterialesCotizacionLimpiezaDTO> ObtenerListaMaterialesOperarioLimpieza(int idPuestoDireccionCotizacion);
        Task<ListaMaterialesCotizacionLimpiezaDTO> ObtenerListaEquipoOperario(int idPuestoCotizacion);
        Task<ListaMaterialesCotizacionLimpiezaDTO> ObtenerListaHerramientaOperario(int idPuestoCotizacion);
        Task<ListaMaterialesCotizacionLimpiezaDTO> ObtenerListaUniformeOperario(int idPuestoCotizacion);
        Task<MaterialCotizacionDTO> ObtenerEquipoCotizacionPorId(int id);
        Task<MaterialCotizacionDTO> ObtenerHerramientaCotizacionPorId(int id);
        Task<MaterialCotizacionDTO> ObtenerMaterialCotizacionPorId(int id);
        Task<MaterialCotizacionDTO> ObtenerUniformeCotizacionPorId(int id);
        Task<ServicioCotizacionDTO> ServicioGetById(int id);
        Task InsertarServicioCotizacion(ServicioCotizacion servicio);
        Task ActualizarServicioCotizacion(ServicioCotizacion servicio);
        Task EliminarServicioCotizacion(int id);
        Task<ListaServiciosCotizacionLimpiezaDTO> ObtenerListaServiciosCotizacion(int idCotizacion, int idDireccionCotizacion);
        Task<bool> EliminarProductoPlantillas(string clave, int idCotizacion, string tipo);
    }

    public class MaterialService : IMaterialService
    {
        private readonly IMaterialRepository _materialRepo;
        private readonly ICotizacionesRepository _cotizaRepo;
        private readonly IMapper _mapper;
        private readonly IServicioRepository _servicioRepo;

        public MaterialService(IMaterialRepository materialRepo, ICotizacionesRepository cotizaRepo, IServicioRepository servicioRepo, IMapper mapper)
        {
            _materialRepo = materialRepo;
            _cotizaRepo = cotizaRepo;
            _mapper = mapper;
            _servicioRepo = servicioRepo;
        }

        public async Task AgregarMaterialOperario(MaterialCotizacionDTO materialVM)
        {
            int idEstadonew = await _materialRepo.ObtenerIdEstadoPorIdDireccionCotizacion(materialVM.IdDireccionCotizacion);
            int idProveedornew = await _materialRepo.ObtenerIdProveedorPorIdEstado(idEstadonew);
            decimal precionew = await _materialRepo.ObtenerPrecioProductoProveedor(materialVM.ClaveProducto, idProveedornew);
            if (precionew == 0)
            {
                precionew = await _materialRepo.ObtenerPrecioProductoBase(materialVM.ClaveProducto);
            }
            var material = _mapper.Map<MaterialCotizacion>(materialVM);
            material.PrecioUnitario = precionew;
            material.Cantidad = materialVM.Cantidad;
            if (materialVM.DiasEvento == 0)
            {
                material.Total = material.PrecioUnitario * material.Cantidad;
                material.ImporteMensual = material.Total / (int)material.IdFrecuencia;
            }
            else
            {
                material.Total = ((material.PrecioUnitario * material.Cantidad) / 30.4167M) * materialVM.DiasEvento;
                material.ImporteMensual = material.Total;
            }
            if (materialVM.edit == 1)
            {
                await _materialRepo.ActualizarMaterialCotizacion(material);
            }
            else
            {
                await _materialRepo.AgregarMaterialCotizacion(material);
            }

        }

        public async Task ActualizarMaterialCotizacion(MaterialCotizacionDTO materialVM)
        {
            var material = _mapper.Map<MaterialCotizacion>(materialVM);
            if (materialVM.DiasEvento == 0)
            {
                material.Total = (material.PrecioUnitario * material.Cantidad) / (int)material.IdFrecuencia;
            }
            else
            {
                material.Total = ((material.PrecioUnitario * material.Cantidad) / 30.4167M) * materialVM.DiasEvento;
            }

            await _materialRepo.ActualizarMaterialCotizacion(material);
        }

        public async Task EliminarMaterialDeCotizacion(int idMaterialCotizacion)
        {
            await _materialRepo.EliminarMaterialCotizacion(idMaterialCotizacion);
        }

        public async Task ObtenerListaMaterialesCotizacion(ListaMaterialesCotizacionLimpiezaDTO listaMaterialesVM)
        {
            listaMaterialesVM.Rows = await _materialRepo.ContarMaterialesCotizacion(
                listaMaterialesVM.IdCotizacion, listaMaterialesVM.IdDireccionCotizacion, listaMaterialesVM.IdPuestoDireccionCotizacion, listaMaterialesVM.Keywords);

            if (listaMaterialesVM.Rows > 0)
            {
                listaMaterialesVM.NumPaginas = (listaMaterialesVM.Rows / 10);
                if (listaMaterialesVM.Rows % 10 > 0)
                {
                    listaMaterialesVM.NumPaginas++;
                }
                listaMaterialesVM.MaterialesCotizacion = _mapper.Map<List<MaterialCotizacionMinDTO>>(await _materialRepo.ObtenerMaterialesCotizacion(
                    listaMaterialesVM.IdCotizacion, listaMaterialesVM.IdDireccionCotizacion, listaMaterialesVM.IdPuestoDireccionCotizacion, listaMaterialesVM.Keywords, listaMaterialesVM.Pagina));
            }
            else
            {
                listaMaterialesVM.MaterialesCotizacion = new List<MaterialCotizacionMinDTO>();
            }
        }

        public async Task<ListaMaterialesCotizacionLimpiezaDTO> ObtenerListaMaterialesOperarioLimpieza(int idPuestoDireccionCotizacion)
        {
            var materialCotizacionLimpieza = new ListaMaterialesCotizacionLimpiezaDTO { IdPuestoDireccionCotizacion = idPuestoDireccionCotizacion };

            materialCotizacionLimpieza.IdCotizacion = await _cotizaRepo.ObtieneIdCotizacionPorOperario(materialCotizacionLimpieza.IdPuestoDireccionCotizacion);

            materialCotizacionLimpieza.IdDireccionCotizacion = await _cotizaRepo.ObtieneIdDireccionCotizacionPorOperario(materialCotizacionLimpieza.IdPuestoDireccionCotizacion);

            materialCotizacionLimpieza.MaterialesCotizacion = _mapper.Map<List<MaterialCotizacionMinDTO>>(await _materialRepo.ObtenerMaterialCotizacionOperario(idPuestoDireccionCotizacion, materialCotizacionLimpieza.IdCotizacion));

            return materialCotizacionLimpieza;
        }

        public async Task<MaterialCotizacionDTO> ObtenerMaterialCotizacionPorId(int id)
        {
            var material = _mapper.Map<MaterialCotizacionDTO>(await _materialRepo.ObtenerMaterialCotizacionPorId(id));

            return material;
        }

        public async Task ObtenerListaEquipoCotizacion(ListaMaterialesCotizacionLimpiezaDTO listaMateriales)
        {
            listaMateriales.Rows = await _materialRepo.ContarEquipoCotizacion(
            listaMateriales.IdCotizacion, listaMateriales.IdDireccionCotizacion, listaMateriales.IdPuestoDireccionCotizacion, listaMateriales.Keywords);
            if (listaMateriales.Rows > 0)
            {
                listaMateriales.NumPaginas = (listaMateriales.Rows / 10);
                if (listaMateriales.Rows % 10 > 0)
                {
                    listaMateriales.NumPaginas++;
                }
                listaMateriales.MaterialesCotizacion = _mapper.Map<List<MaterialCotizacionMinDTO>>(await _materialRepo.ObtenerEquipoCotizacion(
                listaMateriales.IdCotizacion, listaMateriales.IdDireccionCotizacion, listaMateriales.IdPuestoDireccionCotizacion, listaMateriales.Keywords, listaMateriales.Pagina));
            }
            else
            {
                listaMateriales.MaterialesCotizacion = new List<MaterialCotizacionMinDTO>();
            }
        }

        public async Task ObtenerListaHerramientaCotizacion(ListaMaterialesCotizacionLimpiezaDTO listaMateriales)
        {
            listaMateriales.Rows = await _materialRepo.ContarHerramientaCotizacion(
            listaMateriales.IdCotizacion, listaMateriales.IdDireccionCotizacion, listaMateriales.IdPuestoDireccionCotizacion, listaMateriales.Keywords);
            if (listaMateriales.Rows > 0)
            {
                listaMateriales.NumPaginas = (listaMateriales.Rows / 10);
                if (listaMateriales.Rows % 10 > 0)
                {
                    listaMateriales.NumPaginas++;
                }
                listaMateriales.MaterialesCotizacion = _mapper.Map<List<MaterialCotizacionMinDTO>>(await _materialRepo.ObtenerHerramientaCotizacion(
                listaMateriales.IdCotizacion, listaMateriales.IdDireccionCotizacion, listaMateriales.IdPuestoDireccionCotizacion, listaMateriales.Keywords, listaMateriales.Pagina));
            }
            else
            {
                listaMateriales.MaterialesCotizacion = new List<MaterialCotizacionMinDTO>();
            }
        }

        public async Task ObtenerListaUniformeCotizacion(ListaMaterialesCotizacionLimpiezaDTO listaMateriales)
        {
            listaMateriales.Rows = await _materialRepo.ContarUniformeCotizacion(
            listaMateriales.IdCotizacion, listaMateriales.IdDireccionCotizacion, listaMateriales.IdPuestoDireccionCotizacion, listaMateriales.Keywords);
            if (listaMateriales.Rows > 0)
            {
                listaMateriales.NumPaginas = (listaMateriales.Rows / 10);
                if (listaMateriales.Rows % 10 > 0)
                {
                    listaMateriales.NumPaginas++;
                }
                listaMateriales.MaterialesCotizacion = _mapper.Map<List<MaterialCotizacionMinDTO>>(await _materialRepo.ObtenerUniformeCotizacion(
                listaMateriales.IdCotizacion, listaMateriales.IdDireccionCotizacion, listaMateriales.IdPuestoDireccionCotizacion, listaMateriales.Keywords, listaMateriales.Pagina));
            }
            else
            {
                listaMateriales.MaterialesCotizacion = new List<MaterialCotizacionMinDTO>();
            }
        }

        public async Task<ListaServiciosCotizacionLimpiezaDTO> ObtenerListaServiciosCotizacion(int idCotizacion, int idDireccioNCotizacion)
        {
            var servicioCotizacion = new ListaServiciosCotizacionLimpiezaDTO { IdCotizacion = idCotizacion };
            servicioCotizacion.ServiciosCotizacion = _mapper.Map<List<ServicioCotizacionMinDTO>>(await _servicioRepo.ObtenerListaServiciosCotizacion(idCotizacion, idDireccioNCotizacion));
            return servicioCotizacion;
        }

        public async Task<ListaMaterialesCotizacionLimpiezaDTO> ObtenerListaEquipoOperario(int idPuestoCotizacion)
        {
            var equipoCotizacion = new ListaMaterialesCotizacionLimpiezaDTO { IdPuestoDireccionCotizacion = idPuestoCotizacion };

            equipoCotizacion.IdCotizacion = await _cotizaRepo.ObtieneIdCotizacionPorOperario(equipoCotizacion.IdPuestoDireccionCotizacion);

            equipoCotizacion.IdDireccionCotizacion = await _cotizaRepo.ObtieneIdDireccionCotizacionPorOperario(equipoCotizacion.IdPuestoDireccionCotizacion);

            equipoCotizacion.MaterialesCotizacion = _mapper.Map<List<MaterialCotizacionMinDTO>>(await _materialRepo.ObtenerEquipoCotizacionOperario(idPuestoCotizacion, equipoCotizacion.IdCotizacion));

            return equipoCotizacion;
        }

        public async Task<ListaMaterialesCotizacionLimpiezaDTO> ObtenerListaHerramientaOperario(int idPuestoCotizacion)
        {
            var herramientaCotizacion = new ListaMaterialesCotizacionLimpiezaDTO { IdPuestoDireccionCotizacion = idPuestoCotizacion };

            herramientaCotizacion.IdCotizacion = await _cotizaRepo.ObtieneIdCotizacionPorOperario(herramientaCotizacion.IdPuestoDireccionCotizacion);

            herramientaCotizacion.IdDireccionCotizacion = await _cotizaRepo.ObtieneIdDireccionCotizacionPorOperario(herramientaCotizacion.IdPuestoDireccionCotizacion);

            herramientaCotizacion.MaterialesCotizacion = _mapper.Map<List<MaterialCotizacionMinDTO>>(await _materialRepo.ObtenerHerramientaCotizacionOperario(idPuestoCotizacion, herramientaCotizacion.IdCotizacion));

            return herramientaCotizacion;
        }

        public async Task<ListaMaterialesCotizacionLimpiezaDTO> ObtenerListaUniformeOperario(int idPuestoCotizacion)
        {
            var uniformeCotizacion = new ListaMaterialesCotizacionLimpiezaDTO { IdPuestoDireccionCotizacion = idPuestoCotizacion };

            uniformeCotizacion.IdCotizacion = await _cotizaRepo.ObtieneIdCotizacionPorOperario(uniformeCotizacion.IdPuestoDireccionCotizacion);

            uniformeCotizacion.IdDireccionCotizacion = await _cotizaRepo.ObtieneIdDireccionCotizacionPorOperario(uniformeCotizacion.IdPuestoDireccionCotizacion);

            uniformeCotizacion.MaterialesCotizacion = _mapper.Map<List<MaterialCotizacionMinDTO>>(await _materialRepo.ObtenerUniformeCotizacionOperario(idPuestoCotizacion, idPuestoCotizacion));

            return uniformeCotizacion;
        }

        public async Task<MaterialCotizacionDTO> ObtenerEquipoCotizacionPorId(int id)
        {
            var equipo = _mapper.Map<MaterialCotizacionDTO>(await _materialRepo.ObtenerEquipoCotizacionPorId(id));

            return equipo;
        }

        public async Task<MaterialCotizacionDTO> ObtenerHerramientaCotizacionPorId(int id)
        {
            var herramienta = _mapper.Map<MaterialCotizacionDTO>(await _materialRepo.ObtenerHerramientaCotizacionPorId(id));

            return herramienta;
        }

        public async Task<MaterialCotizacionDTO> ObtenerUniformeCotizacionPorId(int id)
        {
            var uniforme = _mapper.Map<MaterialCotizacionDTO>(await _materialRepo.ObtenerUniformeCotizacionPorId(id));

            return uniforme;
        }

        public async Task<ServicioCotizacionDTO> ServicioGetById(int id)
        {
            var servicio = new ServicioCotizacionDTO();

            servicio = _mapper.Map<ServicioCotizacionDTO>(await _materialRepo.ServicioGetById(id));
            return servicio;
        }

        public async Task AgregarEquipoOperario(MaterialCotizacionDTO dto)
        {
            int idEstadonew = await _materialRepo.ObtenerIdEstadoPorIdDireccionCotizacion(dto.IdDireccionCotizacion);
            int idProveedornew = await _materialRepo.ObtenerIdProveedorPorIdEstado(idEstadonew);
            decimal precionew = await _materialRepo.ObtenerPrecioProductoProveedor(dto.ClaveProducto, idProveedornew);
            if (precionew == 0)
            {
                precionew = await _materialRepo.ObtenerPrecioProductoBase(dto.ClaveProducto);

            }
            var material = _mapper.Map<MaterialCotizacion>(dto);
            material.PrecioUnitario = precionew;
            material.Cantidad = dto.Cantidad;
            if (dto.DiasEvento == 0)
            {
                material.Total = material.PrecioUnitario * material.Cantidad;
                material.ImporteMensual = material.Total / (int)material.IdFrecuencia;
            }
            else
            {
                material.Total = (((material.PrecioUnitario * material.Cantidad) / 12M) * 0.8M) * dto.DiasEvento;
                //material.Total = ((((material.PrecioUnitario / 12M) * 1.15M) * material.Cantidad) / 30.4167M) * dto.DiasEvento;
                material.ImporteMensual = material.Total;
            }
            if (dto.edit == 1)
            {
                await _materialRepo.ActualizarEquipoCotizacion(material);
            }
            else
            {
                await _materialRepo.AgregarEquipoCotizacion(material);
            }
        }

        public async Task ActualizarEquipoCotizacion(MaterialCotizacionDTO dto)
        {
            var equipo = _mapper.Map<MaterialCotizacion>(dto);
            if (dto.DiasEvento == 0)
            {
                equipo.Total = (equipo.PrecioUnitario * equipo.Cantidad);
                equipo.ImporteMensual = equipo.Total / (int)equipo.IdFrecuencia;
            }
            else
            {
                equipo.Total = ((((equipo.PrecioUnitario / 12M) * 1.15M) * equipo.Cantidad) / 30.4167M) * dto.DiasEvento;
                equipo.ImporteMensual = equipo.Total;
            }
            await _materialRepo.ActualizarEquipoCotizacion(equipo);
        }

        public async Task EliminarEquipoCotizacion(int idEquipo)
        {
            await _materialRepo.EliminarEquipoCotizacion(idEquipo);
        }

        public async Task AgregarUniformeOperario(MaterialCotizacionDTO dto)
        {
            int idEstadonew = await _materialRepo.ObtenerIdEstadoPorIdDireccionCotizacion(dto.IdDireccionCotizacion);
            int idProveedornew = await _materialRepo.ObtenerIdProveedorPorIdEstado(idEstadonew);
            decimal precionew = await _materialRepo.ObtenerPrecioProductoProveedor(dto.ClaveProducto, idProveedornew);
            if (precionew == 0)
            {
                precionew = await _materialRepo.ObtenerPrecioProductoBase(dto.ClaveProducto);
            }

            var material = _mapper.Map<MaterialCotizacion>(dto);
            material.PrecioUnitario = precionew;
            material.Cantidad = dto.Cantidad;
            if (dto.DiasEvento == 0)
            {
                material.Total = material.PrecioUnitario * material.Cantidad;
                material.ImporteMensual = material.Total / (int)material.IdFrecuencia;
            }
            else
            {
                material.Total = ((material.PrecioUnitario * material.Cantidad) / 30.4167M) * dto.DiasEvento;
                material.ImporteMensual = material.Total;
            }
            if (dto.edit == 1)
            {
                await _materialRepo.ActualizarUniformeCotizacion(material);
            }
            else
            {
                await _materialRepo.AgregarUniformeCotizacion(material);
            }


        }

        public async Task InsertarServicioCotizacion(ServicioCotizacion servicio)
        {
            servicio.Total = servicio.PrecioUnitario * servicio.Cantidad;
            servicio.ImporteMensual = servicio.Total / (int)servicio.IdFrecuencia;
            servicio.FechaAlta = DateTime.Now;
            await _materialRepo.InsertarServicioCotizacion(servicio);
        }

        public async Task ActualizarServicioCotizacion(ServicioCotizacion servicio)
        {
            servicio.Total = servicio.PrecioUnitario * servicio.Cantidad;
            servicio.ImporteMensual = servicio.Total / (int)servicio.IdFrecuencia;
            servicio.FechaAlta = DateTime.Now;
            await _materialRepo.ActualizarServicioCotizacion(servicio);
        }

        public async Task EliminarServicioCotizacion(int id)
        {
            await _materialRepo.EliminarServicioCotizacion(id);
        }

        public async Task ActualizarUniformeCotizacion(MaterialCotizacionDTO dto)
        {
            var uniforme = _mapper.Map<MaterialCotizacion>(dto);
            if (dto.DiasEvento == 0)
            {
                uniforme.Total = (uniforme.PrecioUnitario * uniforme.Cantidad);
                uniforme.ImporteMensual = uniforme.Total / (int)uniforme.IdFrecuencia;
            }
            else
            {
                uniforme.Total = ((uniforme.PrecioUnitario * uniforme.Cantidad) / 30.4167M) * dto.DiasEvento;
                uniforme.ImporteMensual = uniforme.Total;
            }
            await _materialRepo.ActualizarUniformeCotizacion(uniforme);
        }

        public async Task EliminarUniformeCotizacion(int idUniforme)
        {
            await _materialRepo.EliminarUniformeCotizacion(idUniforme);
        }

        public async Task AgregarHerramientaOperario(MaterialCotizacionDTO dto)
        {
            int idEstadonew = await _materialRepo.ObtenerIdEstadoPorIdDireccionCotizacion(dto.IdDireccionCotizacion);
            int idProveedornew = await _materialRepo.ObtenerIdProveedorPorIdEstado(idEstadonew);
            decimal precionew = await _materialRepo.ObtenerPrecioProductoProveedor(dto.ClaveProducto, idProveedornew);
            if (precionew == 0)
            {
                precionew = await _materialRepo.ObtenerPrecioProductoBase(dto.ClaveProducto);
            }
            var material = _mapper.Map<MaterialCotizacion>(dto);
            material.PrecioUnitario = precionew;
            material.Cantidad = dto.Cantidad;
            if (dto.DiasEvento == 0)
            {
                material.Total = material.PrecioUnitario * material.Cantidad;
                material.ImporteMensual = material.Total / (int)material.IdFrecuencia;
            }
            else
            {
                material.Total = ((material.PrecioUnitario * material.Cantidad) / 30.4167M) * dto.DiasEvento;
                material.ImporteMensual = material.Total;
            }

            if (dto.edit == 1)
            {
                await _materialRepo.ActualizarHerramientaCotizacion(material);
            }
            else
            {
                await _materialRepo.AgregarHerramientaCotizacion(material);
            }

        }

        public async Task ActualizarHerramientaOperario(MaterialCotizacionDTO dto)
        {
            var herramienta = _mapper.Map<MaterialCotizacion>(dto);
            if (dto.DiasEvento == 0)
            {
                herramienta.Total = (herramienta.PrecioUnitario * herramienta.Cantidad);
                herramienta.ImporteMensual = herramienta.Total / (int)herramienta.IdFrecuencia;
            }
            else
            {
                herramienta.Total = ((herramienta.PrecioUnitario * herramienta.Cantidad) / 30.4167M) * dto.DiasEvento;
                herramienta.ImporteMensual = herramienta.Total;
            }
            await _materialRepo.ActualizarHerramientaCotizacion(herramienta);
        }

        public async Task EliminarHerramientaCotizacion(int idHerramienta)
        {
            await _materialRepo.EliminarHerramientaCotizacion(idHerramienta);
        }

        public async Task<bool> EliminarProductoPlantillas(string clave, int idCotizacion, string tipo)
        {
            return await _materialRepo.EliminarProductoPlantillas(clave, idCotizacion, tipo);
        }
    }
}
