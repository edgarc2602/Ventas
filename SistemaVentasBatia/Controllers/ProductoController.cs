﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Mozilla;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Services;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService logic;

        public ProductoController(IProductoService service)
        {
            logic = service;
        }

        [HttpGet("[action]/{id}")]
        public async Task<IEnumerable<ProductoItemDTO>> GetMaterial(int id)
        {
            return await logic.GetMaterialPuesto(id);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<MaterialPuestoDTO>> PostMaterial(MaterialPuestoDTO dto)
        {
            if (dto.IdMaterialPuesto == 0)
            {
                await logic.CreateMaterial(dto);
            }
            else
            {
                await logic.ActualizarMaterial(dto);
            }
            return dto;
        }

        [HttpGet("[action]/{id}")]
        public async Task<IEnumerable<ProductoItemDTO>> GetHerramienta(int id)
        {
            return await logic.GetHerramientaPuesto(id);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<MaterialPuestoDTO>> PostHerramienta(MaterialPuestoDTO dto)
        {
            if (dto.IdMaterialPuesto == 0)
            {
                await logic.CreateHerramienta(dto);
            }
            else
            {
                await logic.ActualizarHerramienta(dto);
            }
            return dto;
        }

        [HttpGet("[action]/{id}")]
        public async Task<IEnumerable<ProductoItemDTO>> GetEquipo(int id)
        {
            return await logic.GetEquipoPuesto(id);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<MaterialPuestoDTO>> PostEquipo(MaterialPuestoDTO dto)
        {
            if (dto.IdMaterialPuesto == 0)
            {
                await logic.CreateEquipo(dto);
            }
            else
            {
                await logic.ActualizarEquipo(dto);
            }
            return dto;
        }

        [HttpGet("[action]/{id}")]
        public async Task<IEnumerable<ProductoItemDTO>> GetUniforme(int id)
        {
            return await logic.GetUniformePuesto(id);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<MaterialPuestoDTO>> PostUniforme(MaterialPuestoDTO dto)
        {
            if (dto.IdMaterialPuesto == 0)
            {
                await logic.CreateUniforme(dto);
            }
            else
            {
                await logic.ActualizarUniforme(dto);
            }
            return dto;
        }

        [HttpDelete("[action]/{id}")]
        public async Task<ActionResult<bool>> DelMaterial(int id)
        {
            return await logic.DeleteMaterial(id);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<ActionResult<bool>> DelHerramienta(int id)
        {
            return await logic.DeleteHerramienta(id);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<ActionResult<bool>> DelEquipo(int id)
        {
            return await logic.DeleteEquipo(id);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<ActionResult<bool>> DelUniforme(int id)
        {
            return await logic.DeleteUniforme(id);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<ActionResult<bool>> EliminarServicio(int id)
        {
            return await logic.EliminarServicio(id);
        }
        
        [HttpDelete("[action]/{id}")]
        public async Task<ActionResult<bool>> EliminarIndustria(int id)
        {
            return await logic.EliminarIndustria(id);
        }

        [HttpGet("[action]/{servicio}/{idPersonal}")]
        public async Task<ActionResult<bool>> AgregarServicio(string servicio = "", int idPersonal = 0)
        {
            return await logic.AgregarServicio(servicio, idPersonal);
        }
        
        [HttpGet("[action]/{industria}/{idPersonal}")]
        public async Task<ActionResult<bool>> AgregarIndustria(string industria = "", int idPersonal = 0)
        {
            return await logic.AgregarIndustria(industria, idPersonal);
        }

        [HttpGet("[action]/{idProducto}/{tipo}/{idPuesto}")]
        public async Task<ActionResult<MaterialPuestoDTO>> ObtenerProductoDefault(int idProducto, int tipo, int idPuesto)
        {
            return await logic.ObtenerProductoDefault(idProducto, tipo, idPuesto);
        }

        [HttpGet("[action]/{idEstado}/{pagina}/{idFamilia}")]
        public async Task<ActionResult<ListaProductoDTO>> GetProductoProveedorByIdEstado(int idEstado, int pagina, int idFamilia = 0)
        {
            var listaproducto = new ListaProductoDTO()
            {
                Pagina = pagina
            };
            return await logic.GetProductoProveedorByIdEstado(listaproducto, idEstado, idFamilia);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<bool>> AgregarProductosGeneral(ProductosGeneralDTO data)
        {
            return await logic.AgregarProductosGeneral(data);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<bool>> EliminarProductosGeneral(ProductosGeneralDTO data)
        {
            return await logic.EliminarProductosGeneral(data);
        }

        [HttpGet("[action]")]

        public async Task<List<EstadoProveedorDTO>> EstadoProveedor()
        {
            return await logic.EstadoProveedor();
        }
    }
}