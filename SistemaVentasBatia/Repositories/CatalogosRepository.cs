using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;
using SistemaVentasBatia.DTOs;

namespace SistemaVentasBatia.Repositories
{
    public interface ICatalogosRepository
    {
        Task<List<Catalogo>> ObtenerEstados();
        Task<List<Catalogo>> ObtenerServicios();
        Task<List<Catalogo>> ObtenerMunicipios(int idMunicipio);
        Task<List<Catalogo>> ObtenerTiposInmueble();
        Task<List<Catalogo>> ObtenerCatalogoPuestos();
        Task<List<Catalogo>> ObtenerCatalogoServicios();
        Task<List<Catalogo>> ObtenerCatalogoTurnos();
        Task<List<Catalogo>> ObtenerCatalogoJornada();
        Task<List<Catalogo>> ObtenerCatalogoClase();
        Task<List<Catalogo>> ObtenerCatalogoDireccionesCotizacion(int idCotizacion);
        Task<List<Catalogo>> ObtenerCatalogoPuestosCotizacion(int idCotizacion);
        Task<List<Catalogo>> ObtenerCatalogoProductos(Servicio idServicio);
        Task<List<Catalogo>> ObtenerCatalogoEmpresas();
        Task<List<MaterialPuesto>> ObtenerMaterialDefaultPorPuesto(int idPuesto);
        Task<IEnumerable<Catalogo>> ObtenerCatalogoProductosByFamilia(Servicio idServicio, int[] familia);
        Task<IEnumerable<Catalogo>> ObtenerCatalogoProductosByGrupoElimina(string grupo, int idCotizacion);
        Task<IEnumerable<MaterialPuesto>> ObtenerHerramientaDefaultPorPuesto(int idPuesto);
        Task<IEnumerable<MaterialPuesto>> ObtenerEquipoDefaultPorPuesto(int idPuesto);
        Task<IEnumerable<MaterialPuesto>> ObtenerUniformeDefaultPorPuesto(int idPuesto);
        Task<List<Catalogo>> ObtenerCatalogoFamiliasPorIdServicio(int idServicio);
        Task<List<Catalogo>> ObtenerCatalogoVendedores();
        Task<List<Catalogo>> ObtenerCatalogoEjecutivos();
        Task<List<Catalogo>> ObtenerCatalogoGerentesLimpieza();
        Task<List<Catalogo>> ObtenerCatalogoTiposdeIndustria();

    }

    public class CatalogosRepository : ICatalogosRepository
    {
        private readonly DapperContext ctx;
        public CatalogosRepository(DapperContext ctx)
        {
            this.ctx = ctx;
        }
        public async Task<List<Catalogo>> ObtenerEstados()
        {
            var query = @"SELECT id_Estado Id, descripcion Descripcion from tb_estado";

            var estados = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    estados = (await connection.QueryAsync<Catalogo>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return estados;
        }
        public async Task<List<Catalogo>> ObtenerServicios()
        {
            var query = @"SELECT id_servicioextra Id, descripcion Descripcion  FROM tb_servicioextra";


            var servicios = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    servicios = (await connection.QueryAsync<Catalogo>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return servicios;
        }
        public async Task<List<Catalogo>> ObtenerMunicipios(int idEstado)
        {
            var query = @"
SELECT es.id_municipio Id,
m.Municipio Descripcion FROM tb_estado_municipio es
INNER JOIN tb_municipio m ON m.Id_Municipio = es.id_municipio
WHERE es.id_estado = @idEstado ORDER BY m.Municipio";

            var municipios = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    municipios = (await connection.QueryAsync<Catalogo>(query, new { idEstado })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return municipios;
        }
        public async Task<List<Catalogo>> ObtenerTiposInmueble()
        {
            var query = @"SELECT id_tipoinmueble Id, descripcion Descripcion from tb_tipoinmueble where id_status = @idEstatus";

            var tiposInmueble = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    tiposInmueble = (await connection.QueryAsync<Catalogo>(query, new { idEstatus = EstatusTipoInmueble.Activo })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tiposInmueble;
        }
        public async Task<List<Catalogo>> ObtenerCatalogoPuestos()
        {
            var query = @"SELECT id_puesto Id, descripcion Descripcion
                          FROM tb_puesto d
                          WHERE id_status = 1 AND cotizador = 1 ORDER BY Descripcion";

            var puestos = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    puestos = (await connection.QueryAsync<Catalogo>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return puestos;
        }
        public async Task<List<Catalogo>> ObtenerCatalogoServicios()
        {
            var query = @"SELECT IdTpoServicio Id, TS_Descripcion Descripcion FROM Tbl_TipoServicio";

            var servicios = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    servicios = (await connection.QueryAsync<Catalogo>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return servicios;
        }
        public async Task<List<Catalogo>> ObtenerCatalogoTurnos()
        {
            var query = @"SELECT id_turno Id, descripcion Descripcion
                          FROM tb_turno 
                          WHERE id_status = 1 AND cotizador = 1 ORDER BY id_turno";

            var direcciones = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direcciones = (await connection.QueryAsync<Catalogo>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return direcciones;
        }
        public async Task<List<Catalogo>> ObtenerCatalogoJornada()
        {
            var query = @"SELECT 
id_jornada Id,
descripcion Descripcion
FROM tb_jornada";

            var direcciones = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direcciones = (await connection.QueryAsync<Catalogo>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return direcciones;
        }
        public async Task<List<Catalogo>> ObtenerCatalogoClase()
        {
            var query = @"SELECT 
id_clase Id,
descripcion Descripcion
FROM tb_clase";

            var direcciones = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direcciones = (await connection.QueryAsync<Catalogo>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return direcciones;
        }
        public async Task<List<Catalogo>> ObtenerCatalogoDireccionesCotizacion(int idCotizacion)
        {
            var query = @"SELECT dc.id_direccion_cotizacion Id, d.nombre_sucursal Descripcion
                          FROM tb_direccion_cotizacion dc
                          JOIN tb_direccion d on d.id_direccion = dc.id_direccion
                          WHERE dc.id_cotizacion = @idCotizacion";

            var direccionesCotizacion = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direccionesCotizacion = (await connection.QueryAsync<Catalogo>(query, new { idCotizacion })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return direccionesCotizacion;
        }
        public async Task<List<Catalogo>> ObtenerCatalogoPuestosCotizacion(int idCotizacion)
        {
            var query = @"SELECT distinct p.id_puesto Id, p.descripcion Descripcion
                          FROM tb_puesto_direccion_cotizacion pdc
                          JOIN tb_direccion_cotizacion dc ON dc.id_direccion_cotizacion = pdc.id_direccion_cotizacion
                          JOIN tb_puesto p on p.id_puesto = pdc.id_puesto
                          WHERE dc.id_cotizacion = @idCotizacion ORDER BY Descripcion";

            var puestosCotizacion = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    puestosCotizacion = (await connection.QueryAsync<Catalogo>(query, new { idCotizacion })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return puestosCotizacion;
        }
        public async Task<List<Catalogo>> ObtenerCatalogoProductos(Servicio idServicio)
        {
            var query = @"SELECT clave Clave, descripcion Descripcion
                          FROM tb_producto                          
                          WHERE id_servicio = @idServicio and id_status = 1 ORDER BY Descripcion;";

            var puestosCotizacion = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    puestosCotizacion = (await connection.QueryAsync<Catalogo>(query, new { idServicio })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return puestosCotizacion;
        }
        public async Task<List<MaterialPuesto>> ObtenerMaterialDefaultPorPuesto(int idPuesto)
        {
            var query = @"SELECT  id_material_puesto IdMaterialPuesto, clave_producto ClaveProducto, id_puesto IdPuesto, cantidad Cantidad, id_frecuencia IdFrecuencia
                          FROM tb_material_puesto
                          WHERE id_puesto = @idPuesto";

            var materialPuesto = new List<MaterialPuesto>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    materialPuesto = (await connection.QueryAsync<MaterialPuesto>(query, new { idPuesto })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return materialPuesto;
        }
        public async Task<IEnumerable<Catalogo>> ObtenerCatalogoProductosByFamilia(Servicio idServicio, int[] familia)
        {
            if ((int)idServicio == 4 || (int)idServicio == 5)
            {
                idServicio = Servicio.Limpieza;
            }
            var query = @"SELECT clave Clave, descripcion Descripcion
                          FROM tb_producto                          
                          WHERE id_servicio = @idServicio and id_familia in @familias and id_status = 1 ORDER BY Descripcion;";
            var listFamilia = familia.Select(x => x.ToString());

            var puestosCotizacion = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    puestosCotizacion = (await connection.QueryAsync<Catalogo>(query, new { idServicio, familias = listFamilia })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return puestosCotizacion;
        }

        public async Task<IEnumerable<Catalogo>> ObtenerCatalogoProductosByGrupoElimina(string grupo, int idCotizacion)
        {
            string table = @"";
            switch (grupo.ToLower())
            {
                case "material":
                    table = "tb_cotiza_material";
                    break;
                case "uniforme":
                    table = "tb_cotiza_uniforme";
                    break;
                case "equipo":
                    table = "tb_cotiza_equipo";
                    break;
                case "herramienta":
                    table = "tb_cotiza_herramienta";
                    break;
                default:
                    break;
            }
            var query = @"SELECT DISTINCT a.clave_producto Clave, b.descripcion Descripcion
                        FROM " + table + @" a
                        INNER JOIN tb_producto b ON b.clave = a.clave_producto
                        WHERE a.id_cotizacion = @idCotizacion AND a.id_puesto_direccioncotizacion != 0 
                        ORDER BY b.descripcion";
            var productos = new List<Catalogo>();
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    productos = (await connection.QueryAsync<Catalogo>(query, new { idCotizacion })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return productos;
        }

        public async Task<IEnumerable<MaterialPuesto>> ObtenerHerramientaDefaultPorPuesto(int idPuesto)
        {
            var query = @"SELECT id_herramienta_puesto IdMaterialPuesto, clave ClaveProducto, id_puesto IdPuesto, cantidad Cantidad, id_frecuencia IdFrecuencia
                        FROM tb_herramienta_puesto
                        WHERE id_puesto = @idPuesto";

            var materialPuesto = new List<MaterialPuesto>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    materialPuesto = (await connection.QueryAsync<MaterialPuesto>(query, new { idPuesto })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return materialPuesto;
        }
        public async Task<IEnumerable<MaterialPuesto>> ObtenerEquipoDefaultPorPuesto(int idPuesto)
        {
            var query = @"SELECT id_equipo_puesto IdMaterialPuesto, clave ClaveProducto, id_puesto IdPuesto, cantidad Cantidad, id_frecuencia IdFrecuencia
                        FROM tb_equipo_puesto
                        WHERE id_puesto = @idPuesto";

            var materialPuesto = new List<MaterialPuesto>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    materialPuesto = (await connection.QueryAsync<MaterialPuesto>(query, new { idPuesto })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return materialPuesto;
        }
        public async Task<IEnumerable<MaterialPuesto>> ObtenerUniformeDefaultPorPuesto(int idPuesto)
        {
            var query = @"SELECT id_uniforme_puesto IdMaterialPuesto, clave ClaveProducto, id_puesto IdPuesto, cantidad Cantidad, id_frecuencia IdFrecuencia
                        FROM tb_uniforme_puesto
                        WHERE id_puesto = @idPuesto";

            var materialPuesto = new List<MaterialPuesto>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    materialPuesto = (await connection.QueryAsync<MaterialPuesto>(query, new { idPuesto })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return materialPuesto;
        }
        public async Task<List<Catalogo>> ObtenerCatalogoFamiliasPorIdServicio(int idServicio)
        {
            string query = @"
                SELECT DISTINCT b.id_familia, b.descripcion
                FROM tb_producto a
                INNER JOIN tb_familia b ON a.id_familia = b.id_familia
                WHERE a.id_servicio = @idServicio and a.id_status = 1 ORDER BY Descripcion;
                ";
            var familias = new List<Catalogo>();
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    familias = (await connection.QueryAsync<Catalogo>(query, new { idServicio })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return familias;
        }

        public async Task<List<Catalogo>> ObtenerCatalogoEmpresas()
        {
            var query = @"SELECT id_empresa Id,
                        nombre Descripcion
                        FROM tb_empresa
                        WHERE id_estatus = 1 and tipo = 1
                        ORDER BY nombre";

            var empresas = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    empresas = (await connection.QueryAsync<Catalogo>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return empresas;
        }

        public async Task<List<Catalogo>> ObtenerCatalogoVendedores()
        {
            string query = @"SELECT IdPersonal Id, Per_Nombre + ' ' + Per_Paterno + ' ' + Per_Materno Descripcion
                            FROM Personal WHERE per_cotizadorventas = 1 AND per_revisaventas = 0 ORDER BY Per_Nombre";

            var vendedores = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    vendedores = (await connection.QueryAsync<Catalogo>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return vendedores;
        }

        public async Task<List<Catalogo>> ObtenerCatalogoEjecutivos()
        {
            string query = @"SELECT id_empleado Id, nombre + ' ' + paterno + ' ' + materno Descripcion FROM tb_empleado where id_status = 2 and esejecutivo = 1 ORDER BY nombre";

            var vendedores = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    vendedores = (await connection.QueryAsync<Catalogo>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return vendedores;
        }

        public async Task<List<Catalogo>> ObtenerCatalogoGerentesLimpieza()
        {
            string query = @"SELECT id_empleado Id, nombre + ' ' + paterno + ' ' + materno Descripcion FROM tb_empleado where id_status = 2 and esencargado = 1 ORDER BY nombre";

            var vendedores = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    vendedores = (await connection.QueryAsync<Catalogo>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return vendedores;
        }

        public async Task<List<Catalogo>> ObtenerCatalogoTiposdeIndustria()
        {
            string query = @"SELECT id_tipoindustria Id, descripcion Descripcion FROM tb_industria_tipo WHERE id_estatus = 1 ORDER BY descripcion";

            var industrias = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    industrias = (await connection.QueryAsync<Catalogo>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return industrias;
        }
    }
}
