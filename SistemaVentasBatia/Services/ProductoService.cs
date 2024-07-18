using AutoMapper;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using SistemaVentasBatia.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;
using System.Runtime.ConstrainedExecution;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace SistemaVentasBatia.Services
{
    public interface IProductoService
    {
        Task CreateMaterial(MaterialPuestoDTO mat);
        Task CreateUniforme(MaterialPuestoDTO uni);
        Task CreateHerramienta(MaterialPuestoDTO her);
        Task CreateEquipo(MaterialPuestoDTO equi);
        Task<bool> DeleteMaterial(int id);
        Task<bool> DeleteUniforme(int id);
        Task<bool> EliminarServicio(int id);
        Task<bool> EliminarIndustria(int id);
        Task<bool> AgregarServicio(string servicio, int idPersonal);
        Task<bool> AgregarIndustria(string industria, int idPersonal);
        Task<bool> DeleteHerramienta(int id);
        Task<bool> DeleteEquipo(int id);
        Task<IEnumerable<ProductoItemDTO>> GetMaterialPuesto(int idPuesto);
        Task<IEnumerable<ProductoItemDTO>> GetHerramientaPuesto(int idPuesto);
        Task<IEnumerable<ProductoItemDTO>> GetUniformePuesto(int idPuesto);
        Task<IEnumerable<ProductoItemDTO>> GetEquipoPuesto(int idPuesto);
        Task<ActionResult<MaterialPuestoDTO>> ObtenerProductoDefault(int idProdcuto, int tipo, int idPuesto);
        Task ActualizarMaterial(MaterialPuestoDTO producto);
        Task ActualizarHerramienta(MaterialPuestoDTO producto);
        Task ActualizarEquipo(MaterialPuestoDTO producto);
        Task ActualizarUniforme(MaterialPuestoDTO producto);
        Task<ActionResult<ListaProductoDTO>> GetProductoProveedorByIdEstado(ListaProductoDTO listaProducto, int idEstado, int idFamilia);
        Task<bool> AgregarProductosGeneral( ProductosGeneralDTO productos);
        Task<bool> EliminarProductosGeneral( ProductosGeneralDTO productos);
    }

    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository repo;
        private readonly IMapper mapper;
        private readonly IMaterialService materialService;

        public ProductoService(IProductoRepository repository, IMapper imapper, IMaterialService imaterialService)
        {
            repo = repository;
            mapper = imapper;
            materialService = imaterialService;
        }

        public async Task CreateEquipo(MaterialPuestoDTO equi)
        {
            MaterialPuesto model = mapper.Map<MaterialPuesto>(equi);
            model.FechaAlta = DateTime.Now;
            await repo.AgregarEquipoPuesto(model);
        }

        public async Task CreateHerramienta(MaterialPuestoDTO her)
        {
            MaterialPuesto model = mapper.Map<MaterialPuesto>(her);
            model.FechaAlta = DateTime.Now;
            await repo.AgregarHerramientaPuesto(model);
        }

        public async Task CreateMaterial(MaterialPuestoDTO mat)
        {
            MaterialPuesto model = mapper.Map<MaterialPuesto>(mat);
            model.FechaAlta = DateTime.Now;
            await repo.AgregarMaterialPuesto(model);
        }

        public async Task CreateUniforme(MaterialPuestoDTO uni)
        {
            MaterialPuesto model = mapper.Map<MaterialPuesto>(uni);
            model.FechaAlta = DateTime.Now;
            await repo.AgregarUniformePuesto(model);
        }

        public async Task<bool> DeleteEquipo(int id)
        {
            return await repo.EliminarEquipoPuesto(id);
        }

        public async Task<bool> DeleteHerramienta(int id)
        {
            return await repo.EliminarHerramientaPuesto(id);
        }

        public async Task<bool> DeleteMaterial(int id)
        {
            return await repo.EliminarMaterialPuesto(id);
        }

        public async Task<bool> DeleteUniforme(int id)
        {
            return await repo.EliminarUniformePuesto(id);
        }

        public async Task<bool> EliminarServicio(int id)
        {
            bool result = await repo.VerificarServiciosExistentes(id);
            if(result == true)
            {
                return false;
            }
            return await repo.EliminarServicio(id);
        }

        public async Task<bool> EliminarIndustria(int id)
        {
            bool result = await repo.VerificarIndustriasExistentes(id);
            if (result == true)
            {
                return false;
            }
            return await repo.EliminarIndustria(id);
        }

        public async Task<bool> AgregarServicio(string servicio, int idPersonal)
        {
            return await repo.AgregarServicio(servicio, idPersonal);
        }
        
        public async Task<bool> AgregarIndustria(string industria, int idPersonal)
        {
            return await repo.AgregarIndustria(industria, idPersonal);
        }

        public async Task<IEnumerable<ProductoItemDTO>> GetEquipoPuesto(int idPuesto)
        {
            return mapper.Map<IEnumerable<ProductoItemDTO>>(await repo.ObtenerEquipoPuesto(idPuesto));
        }

        public async Task<IEnumerable<ProductoItemDTO>> GetHerramientaPuesto(int idPuesto)
        {
            return mapper.Map<IEnumerable<ProductoItemDTO>>(await repo.ObtenerHerramientaPuesto(idPuesto));
        }

        public async Task<IEnumerable<ProductoItemDTO>> GetMaterialPuesto(int idPuesto)
        {
            return mapper.Map<IEnumerable<ProductoItemDTO>>(await repo.ObtenerMaterialPuesto(idPuesto));
        }

        public async Task<IEnumerable<ProductoItemDTO>> GetUniformePuesto(int idPuesto)
        {
            return mapper.Map<IEnumerable<ProductoItemDTO>>(await repo.ObtenerUniformePuesto(idPuesto));
        }

        public async Task<ActionResult<MaterialPuestoDTO>> ObtenerProductoDefault(int idProdcuto, int tipo, int idPuesto)
        {
            var producto = await repo.ObtenerProductoDefault(idProdcuto, tipo, idPuesto);
            MaterialPuestoDTO model = mapper.Map<MaterialPuestoDTO>(producto);
            return model;
        }

        public async Task ActualizarMaterial(MaterialPuestoDTO producto)
        {
            MaterialPuesto model = mapper.Map<MaterialPuesto>(producto);

            await repo.ActualizarMaterialPuesto(model);
        }

        public async Task ActualizarHerramienta(MaterialPuestoDTO producto)
        {
            MaterialPuesto model = mapper.Map<MaterialPuesto>(producto);
            await repo.ActualizarHerramientaPuesto(model);
        }

        public async Task ActualizarEquipo(MaterialPuestoDTO producto)
        {
            MaterialPuesto model = mapper.Map<MaterialPuesto>(producto);
            await repo.ActualizarEquipoPuesto(model);
        }

        public async Task ActualizarUniforme(MaterialPuestoDTO producto)
        {
            MaterialPuesto model = mapper.Map<MaterialPuesto>(producto);
            await repo.ActualizarUniformePuesto(model);
        }

        public async Task<ActionResult<ListaProductoDTO>> GetProductoProveedorByIdEstado(ListaProductoDTO listaProducto, int idEstado, int idFamilia)
        {
            listaProducto.Rows = await repo.CountProductoProveedorByIdEstado(idEstado,idFamilia);
            listaProducto.Proveedor = await repo.GetProveedorByIdEstado(idEstado);
            listaProducto.IdProveedor = await repo.GetIdProveedorByIdEstado(idEstado);
            var familias = await repo.GetFamiliasByIdEstado(idEstado);
            listaProducto.Familias = mapper.Map<List<ProductoFamiliaDTO>>(familias);
            if(listaProducto.Rows > 0)
            {
                listaProducto.NumPaginas = (listaProducto.Rows / 40);
                if (listaProducto.Rows % 40 > 0)
                {
                    listaProducto.NumPaginas++;
                }
                var lista = await repo.GetProductoProveedorByIdEstado(idEstado, listaProducto.Pagina, idFamilia);
                listaProducto.Productos = mapper.Map<List<ProductoPrecioEstadoDTO>>(lista);
            }
            else
            {
                listaProducto.Productos = new List<ProductoPrecioEstadoDTO>();
            }
            return listaProducto;
        }

        public async Task<bool> AgregarProductosGeneral(ProductosGeneralDTO productos)
        {
            var lista = await repo.ObtenerPlantillasCotizacion(productos.IdCotizacion);
            foreach(var dir in lista)
            {
                foreach(var mat in productos.Material)
                {
                    mat.IdDireccionCotizacion = dir.IdDireccionCotizacion;
                    mat.IdPuestoDireccionCotizacion = dir.IdPuestoDireccionCotizacion;
                    await materialService.AgregarMaterialOperario(mat);
                }
                foreach (var uni in productos.Uniforme)
                {
                    uni.IdDireccionCotizacion = dir.IdDireccionCotizacion;
                    uni.IdPuestoDireccionCotizacion = dir.IdPuestoDireccionCotizacion;
                    await materialService.AgregarUniformeOperario(uni);
                }
                foreach (var equ in productos.Equipo)
                {
                    equ.IdDireccionCotizacion = dir.IdDireccionCotizacion;
                    equ.IdPuestoDireccionCotizacion = dir.IdPuestoDireccionCotizacion;
                    await materialService.AgregarEquipoOperario(equ);
                }
                foreach (var herr in productos.Herramienta)
                {
                    herr.IdDireccionCotizacion = dir.IdDireccionCotizacion;
                    herr.IdPuestoDireccionCotizacion = dir.IdPuestoDireccionCotizacion;
                    await materialService.AgregarHerramientaOperario(herr);
                }
            }
            return true;
        }

        public async Task<bool> EliminarProductosGeneral(ProductosGeneralDTO productos)
        {
            foreach (var mat in productos.Material)
            {
                await materialService.EliminarProductoPlantillas(mat.ClaveProducto, productos.IdCotizacion, "material");
            }
            foreach (var uni in productos.Uniforme)
            {
                await materialService.EliminarProductoPlantillas(uni.ClaveProducto, productos.IdCotizacion, "uniforme");
            }
            foreach (var equ in productos.Equipo)
            {
                await materialService.EliminarProductoPlantillas(equ.ClaveProducto, productos.IdCotizacion, "equipo");
            }
            foreach (var herr in productos.Herramienta)
            {
                await materialService.EliminarProductoPlantillas(herr.ClaveProducto, productos.IdCotizacion, "herramienta");
            }
            return true;
        }
    }
}