using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace SistemaVentasBatia.Repositories
{
    public interface IMaterialRepository
    {
        //MATERIAL
        Task InsertarMaterialesCotizacion(List<MaterialCotizacion> material);
        Task ActualizarMaterialCotizacion(MaterialCotizacion material);
        Task AgregarMaterialCotizacion(MaterialCotizacion material);
        Task<bool> EliminarMaterialCotizacion(int idMaterialCotizacion);
        Task<int> ObtieneIdCotizacionPorMaterial(int idMaterialCotizacion);
        Task<int> ObtieneIdDireccionCotizacionPorMaterial(int idMaterialCotizacion);
        Task<int> ObtieneIdPuestoDireccionCotizacionPorMaterial(int idMaterialCotizacion);
        Task<int> ContarMaterialesCotizacion(int idCotizacion, int idDireccionCotizacion, int idPuestoDireccionCotizacion, string keywords);
        Task<MaterialCotizacion> ObtenerMaterialCotizacionPorId(int id);
        Task<List<MaterialCotizacion>> ObtieneMaterialesPorIdCotizacion(int idCotizacion);
        Task<List<MaterialCotizacion>> ObtenerMaterialCotizacionOperario(int id, int idCotizacion);
        Task<List<MaterialCotizacion>> ObtenerMaterialesCotizacion(int idCotizacion, int idDireccionCotizacion, int idPuestoDireccionCotizacion, string keywords, int pagina);
        Task<List<MaterialCotizacion>> ObtenerMaterialExtraCotizacion(int idCotizacion);
        Task<List<MaterialCotizacion>> ObtenerMaterialExtraCotizacionDireccion(int idCotizacion, int idDireccionCotizacion);

        //UNIFORME
        Task InsertarUniformeCotizacion(List<MaterialCotizacion> material);
        Task ActualizarUniformeCotizacion(MaterialCotizacion uniforme);
        Task AgregarUniformeCotizacion(MaterialCotizacion uniforme);
        Task<bool> EliminarUniformeCotizacion(int idUniformeCotizacion);
        Task<int> ContarUniformeCotizacion(int idCotizacion, int idDireccionCotizacion, int idPuestoDireccionCotizacion, string keywords);
        Task<MaterialCotizacion> ObtenerUniformeCotizacionPorId(int id);
        Task<List<MaterialCotizacion>> ObtenerUniformeCotizacionOperario(int id, int idCotizacion);
        Task<List<MaterialCotizacion>> ObtenerUniformeCotizacion(int idCotizacion, int idDireccionCotizacion, int idPuestoDireccionCotizacion, string keywords, int pagina);
        Task<List<UniformeCotizacion>> ObtieneUniformesPorIdCotizacion(int idCotizacion);
        Task<List<MaterialCotizacion>> ObtenerUniformeExtraCotizacion(int idCotizacion);
        Task<List<MaterialCotizacion>> ObtenerUniformeExtraCotizacionDireccion(int idCotizacion, int idDireccionCotizacion);

        //EQUIPO
        Task InsertarEquipoCotizacion(List<MaterialCotizacion> material);
        Task ActualizarEquipoCotizacion(MaterialCotizacion equipo);
        Task AgregarEquipoCotizacion(MaterialCotizacion equipo);
        Task<bool> EliminarEquipoCotizacion(int idEquipoCotizacion);
        Task<int> ContarEquipoCotizacion(int idCotizacion, int idDireccionCotizacion, int idPuestoDireccionCotizacion, string keywords);
        Task<MaterialCotizacion> ObtenerEquipoCotizacionPorId(int id);
        Task<List<MaterialCotizacion>> ObtenerEquipoCotizacionOperario(int id, int idCotizacion);
        Task<List<MaterialCotizacion>> ObtenerEquipoCotizacion(int idCotizacion, int idDireccionCotizacion, int idPuestoDireccionCotizacion, string keywords, int pagina);
        Task<List<EquipoCotizacion>> ObtieneEquiposPorIdCotizacion(int idCotizacion);
        Task<List<MaterialCotizacion>> ObtenerEquipoExtraCotizacion(int idCotizacion);
        Task<List<MaterialCotizacion>> ObtenerEquipoExtraCotizacionDireccion(int idCotizacion, int idDireccionCotizacion);

        //HERRAMIENTA
        Task InsertarHerramientaCotizacion(List<MaterialCotizacion> material);
        Task ActualizarHerramientaCotizacion(MaterialCotizacion herramienta);
        Task AgregarHerramientaCotizacion(MaterialCotizacion herramienta);
        Task<bool> EliminarHerramientaCotizacion(int idHerramientaCotizacion);
        Task<int> ContarHerramientaCotizacion(int idCotizacion, int idDireccionCotizacion, int idPuestoDireccionCotizacion, string keywords);
        Task<MaterialCotizacion> ObtenerHerramientaCotizacionPorId(int id);
        Task<List<MaterialCotizacion>> ObtenerHerramientaCotizacionOperario(int id, int idCotizacion);
        Task<List<MaterialCotizacion>> ObtenerHerramientaCotizacion(int idCotizacion, int idDireccionCotizacion, int idPuestoDireccionCotizacion, string keywords, int pagina);
        Task<List<HerramientaCotizacion>> ObtieneHerramientasPorIdCotizacion(int idCotizacion);
        Task<List<MaterialCotizacion>> ObtenerHerramientaExtraCotizacion(int idCotizacion);
        Task<List<MaterialCotizacion>> ObtenerHerramientaExtraCotizacionDireccion(int idCotizacion, int idDireccionCotizacion);

        //SERVICIO
        Task InsertarServicioCotizacion(ServicioCotizacion servicio);
        Task ActualizarServicioCotizacion(ServicioCotizacion servicio);
        Task EliminarServicioCotizacion(int id);
        Task<ServicioCotizacion> ServicioGetById(int id);
        Task<List<ServicioCotizacion>> ObtieneServiciosPorIdCotizacion(int idCotizacion);
        Task<List<ServicioCotizacion>> ObtenerServicioExtraCotizacion(int idCotizacion);

        //PRODUCTO
        Task<decimal> ObtenerPrecioProductoBase(string clave);
        Task<decimal> ObtenerPrecioProductoProveedor(string claveproducto, int idProveedor);
        Task<List<ProductoPrecio>> ObtenerPreciosBaseProductos(string listaClaves);
        Task<List<ProductoPrecio>> ObtenerPreciosProductosPorEstado(string listaClaves, int idEstado);

        Task<int> ObtenerIdEstadoPorIdDireccionCotizacion(int idDireccionCotizacion);
        Task<int> ObtenerIdProveedorPorIdEstado(int idEstado);
    }

    public class MaterialRepository : IMaterialRepository
    {
        private readonly DapperContext _ctx;
        public MaterialRepository(DapperContext context)
        {
            _ctx = context;
        }
        public async Task InsertarMaterialesCotizacion(List<MaterialCotizacion> materialCotizacion)
        {
            var query = "insert into tb_cotiza_material (clave_producto, id_cotizacion, id_direccion_cotizacion, id_puesto_direccioncotizacion, precio_unitario, cantidad, total, importemensual, fecha_alta, id_personal, id_frecuencia) values";
            var row = "('{0}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, '{8}', {9}, {10}),";

            foreach (var material in materialCotizacion)
            {
                query += string.Format(row, material.ClaveProducto, material.IdCotizacion, material.IdDireccionCotizacion, material.IdPuestoDireccionCotizacion,
                    material.PrecioUnitario, material.Cantidad, material.Total, material.ImporteMensual, DateTime.Now.ToString("yyyy/MM/dd"), material.IdPersonal, (int)material.IdFrecuencia);
            }

            query = query.Remove(query.Length - 1);

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<ProductoPrecio>> ObtenerPreciosProductosPorEstado(string listaClaves, int idEstado)
        {
            var query = $@"SELECT 
pp.clave Clave,
pp.id_proveedor IdProveedor,
pp.precio Precio
FROM tb_productoprecio pp
LEFT OUTER JOIN tb_proveedor p ON pp.id_proveedor = p.id_proveedor
LEFT OUTER JOIN tb_estado e ON e.id_estado = {idEstado}
WHERE pp.clave IN ({listaClaves})
AND e.id_estado = {idEstado}
AND p.id_proveedor = e.id_proveedor";

            var preciosProductos = new List<ProductoPrecio>();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    preciosProductos = (await connection.QueryAsync<ProductoPrecio>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return preciosProductos;
        }
        public async Task<List<ProductoPrecio>> ObtenerPreciosBaseProductos(string listaClaves)
        {
            var query = $@" SELECT clave Clave, preciobase Precio
                            FROM tb_producto
                            WHERE clave IN ({listaClaves})";

            var preciosProductos = new List<ProductoPrecio>();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    preciosProductos = (await connection.QueryAsync<ProductoPrecio>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return preciosProductos;
        }
        public async Task<MaterialCotizacion> ObtenerMaterialCotizacionPorId(int id)
        {
            var query = $@"SELECT mc.id_material_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
                                  mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
                                  mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal
                            FROM tb_cotiza_material mc
                            WHERE id_material_cotizacion = @id;";

            var naterialCotizacion = new MaterialCotizacion();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    naterialCotizacion = await connection.QueryFirstAsync<MaterialCotizacion>(query, new { id });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return naterialCotizacion;
        }
        public async Task<decimal> ObtenerPrecioProductoBase(string clave)
        {
            var query = $@"SELECT preciobase
                            FROM tb_producto
                            WHERE clave = @clave";

            decimal precio = 0;

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    precio = await connection.QueryFirstAsync<decimal>(query, new { clave });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return precio;
        }
        public async Task<int> ObtenerIdEstadoPorIdDireccionCotizacion(int idDireccionCotizacion)
        {
            var query = $@"SELECT d.id_estado FROM tb_direccion_cotizacion dc
                        INNER JOIN tb_direccion d ON dc.id_direccion = d.id_direccion
                        WHERE id_direccion_cotizacion = @idDireccionCotizacion";
            int idEstado;
            try
            {
                using (var connecion = _ctx.CreateConnection())
                {
                    idEstado = await connecion.QueryFirstAsync<int>(query, new { idDireccionCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return idEstado;
        }
        public async Task<int> ObtenerIdProveedorPorIdEstado(int idEstado)
        {
            var query = $@"SELECT id_proveedor FROM tb_estado
                           WHERE id_estado = @idEstado";
            int idProveedor;
            try
            {
                using (var connecion = _ctx.CreateConnection())
                {
                    idProveedor = await connecion.QueryFirstAsync<int>(query, new { idEstado });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return idProveedor;
        }
        public async Task<decimal> ObtenerPrecioProductoProveedor(string claveproducto, int idProveedor)
        {
            var query = $@"SELECT precio FROM tb_productoprecio WHERE clave = @claveproducto AND id_proveedor = @idProveedor";
            decimal precio = 0;
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    precio = await connection.QueryFirstOrDefaultAsync<decimal>(query, new { claveproducto, idProveedor });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return precio;
        }
        public async Task<int> ContarMaterialesCotizacion(int idCotizacion, int idDireccionCotizacion, int idPuestoDireccionCotizacion, string keywords)
        {
            var query = @"SELECT count(mc.id_material_cotizacion) Rows
                        FROM tb_cotiza_material mc
						JOIN tb_producto p on p.clave = mc.clave_producto
                        JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = mc.id_direccion_cotizacion
                        left outer JOIN tb_puesto_direccion_cotizacion pdc on pdc.id_puesto_direccioncotizacion = mc.id_puesto_direccioncotizacion
                        WHERE 
                            mc.id_cotizacion = @idCotizacion AND
							ISNULL(NULLIF(@idDireccionCotizacion, 0), dc.id_direccion_cotizacion) = dc.id_direccion_cotizacion AND
							mc.id_puesto_direccioncotizacion = @IdPuestoDireccionCotizacion AND
							p.descripcion LIKE '%' + ISNULL(@keywords, '') + '%';";

            var rows = 0;

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    rows = await connection.QuerySingleAsync<int>(query, new { idCotizacion, idDireccionCotizacion, idPuestoDireccionCotizacion, keywords });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return rows;
        }
        public async Task<List<MaterialCotizacion>> ObtenerMaterialesCotizacion(int idCotizacion, int idDireccionCotizacion, int idPuestoDireccionCotizacion, string keywords, int pagina)
        {
            var query = @"select mc.id_material_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
                            mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
                            mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal, p.descripcion DescripcionMaterial,
                            d.nombre_sucursal NombreSucursal, pu.descripcion DescripcionPuesto
                        FROM tb_cotiza_material mc
                        JOIN tb_producto p on p.clave = mc.clave_producto
                        JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = mc.id_direccion_cotizacion
                        JOIN tb_direccion d on d.id_direccion = dc.id_direccion
                        left outer JOIN tb_puesto_direccion_cotizacion pdc on pdc.id_puesto_direccioncotizacion = mc.id_puesto_direccioncotizacion
                        left outer JOIN tb_puesto pu on pu.id_puesto = pdc.id_puesto
                        WHERE mc.id_cotizacion = @idCotizacion AND
						ISNULL(NULLIF(@idDireccionCotizacion, 0), dc.id_direccion_cotizacion) = dc.id_direccion_cotizacion AND
						mc.id_puesto_direccioncotizacion = @idPuestoDireccionCotizacion AND
						p.descripcion LIKE '%' + ISNULL(@keywords, '') + '%'
                        order by p.descripcion
                        OFFSET ((@pagina - 1) * 10) rows
                        fetch next 10 rows only;";

            var materialesCotizacion = new List<MaterialCotizacion>();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    materialesCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new
                    {
                        idCotizacion,
                        pagina,
                        idDireccionCotizacion,
                        idPuestoDireccionCotizacion,
                        keywords
                    })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return materialesCotizacion;
        }
        public async Task<List<MaterialCotizacion>> ObtieneMaterialesPorIdCotizacion(int idCotizacion)
        {
            var query = @"SELECT
mc.id_material_cotizacion IdMaterialCotizacion, 
mc.clave_producto ClaveProducto, 
mc.id_cotizacion IdCotizacion, 
mc.id_direccion_cotizacion IdDireccionCotizacion,
mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion,
dc.id_direccion IdDireccionCotizacionDireccion, 
pdc.id_puesto IdPuesto,
mc.precio_unitario PrecioUnitario, 
mc.id_frecuencia IdFrecuencia,
mc.cantidad Cantidad, 
mc.total Total, 
mc.importemensual ImporteMensual,
mc.fecha_alta FechaAlta, 
mc.id_personal IdPersonal
FROM tb_cotiza_material mc
INNER JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = mc.id_direccion_cotizacion
LEFT JOIN tb_puesto_direccion_cotizacion pdc on pdc.id_puesto_direccioncotizacion = mc.id_puesto_direccioncotizacion
WHERE mc.id_cotizacion = @idCotizacion
ORDER BY mc.fecha_alta";

            var materialesCotizacion = new List<MaterialCotizacion>();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    materialesCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new { idCotizacion, })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return materialesCotizacion;

        }
        public async Task<List<UniformeCotizacion>> ObtieneUniformesPorIdCotizacion(int idCotizacion)
        {
            var query = @"SELECT
uc.id_uniforme_cotizacion IdUniformeCotizacion, 
uc.clave_producto ClaveProducto, 
uc.id_cotizacion IdCotizacion, 
uc.id_direccion_cotizacion IdDireccionCotizacion,
uc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion,
dc.id_direccion IdDireccionCotizacionDireccion, 
pdc.id_puesto IdPuesto,
uc.precio_unitario PrecioUnitario, 
uc.id_frecuencia IdFrecuencia,
uc.cantidad Cantidad, 
uc.total Total, 
uc.importemensual ImporteMensual,
uc.fecha_alta FechaAlta, 
uc.id_personal IdPersonal
FROM tb_cotiza_uniforme uc
INNER JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = uc.id_direccion_cotizacion
LEFT JOIN tb_puesto_direccion_cotizacion pdc on pdc.id_puesto_direccioncotizacion = uc.id_puesto_direccioncotizacion
WHERE uc.id_cotizacion = @idCotizacion
ORDER BY uc.fecha_alta";

            var uniformesCotizacion = new List<UniformeCotizacion>();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    uniformesCotizacion = (await connection.QueryAsync<UniformeCotizacion>(query, new { idCotizacion, })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return uniformesCotizacion;

        }
        public async Task<List<EquipoCotizacion>> ObtieneEquiposPorIdCotizacion(int idCotizacion)
        {
            var query = @"SELECT
ec.id_equipo_cotizacion IdEquipoCotizacion, 
ec.clave_producto ClaveProducto, 
ec.id_cotizacion IdCotizacion, 
ec.id_direccion_cotizacion IdDireccionCotizacion,
ec.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion,
dc.id_direccion IdDireccionCotizacionDireccion, 
pdc.id_puesto IdPuesto,
ec.precio_unitario PrecioUnitario, 
ec.id_frecuencia IdFrecuencia,
ec.cantidad Cantidad, 
ec.total Total, 
ec.importemensual ImporteMensual,
ec.fecha_alta FechaAlta, 
ec.id_personal IdPersonal
FROM tb_cotiza_equipo ec
INNER JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = ec.id_direccion_cotizacion
LEFT JOIN tb_puesto_direccion_cotizacion pdc on pdc.id_puesto_direccioncotizacion = ec.id_puesto_direccioncotizacion
WHERE ec.id_cotizacion = @idCotizacion
ORDER BY ec.fecha_alta";

            var equiposCotizacion = new List<EquipoCotizacion>();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    equiposCotizacion = (await connection.QueryAsync<EquipoCotizacion>(query, new { idCotizacion, })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return equiposCotizacion;

        }
        public async Task<List<HerramientaCotizacion>> ObtieneHerramientasPorIdCotizacion(int idCotizacion)
        {
            var query = @"SELECT
hc.id_herramienta_cotizacion IdHerramientaCotizacion, 
hc.clave_producto ClaveProducto, 
hc.id_cotizacion IdCotizacion, 
hc.id_direccion_cotizacion IdDireccionCotizacion,
hc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion,
dc.id_direccion IdDireccionCotizacionDireccion,
pdc.id_puesto IdPuesto,
hc.precio_unitario PrecioUnitario, 
hc.id_frecuencia IdFrecuencia,
hc.cantidad Cantidad, 
hc.total Total, 
hc.importemensual ImporteMensual,
hc.fecha_alta FechaAlta, 
hc.id_personal IdPersonal
FROM tb_cotiza_herramienta hc
INNER JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = hc.id_direccion_cotizacion
LEFT JOIN tb_puesto_direccion_cotizacion pdc on pdc.id_puesto_direccioncotizacion = hc.id_puesto_direccioncotizacion
WHERE hc.id_cotizacion = @idCotizacion
ORDER BY hc.fecha_alta";

            var herramientasCotizacion = new List<HerramientaCotizacion>();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    herramientasCotizacion = (await connection.QueryAsync<HerramientaCotizacion>(query, new { idCotizacion, })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return herramientasCotizacion;

        }
        public async Task<List<ServicioCotizacion>> ObtieneServiciosPorIdCotizacion(int idCotizacion)
        {
            var query = @"
SELECT
cs.id_servicioextra_cotizacion IdServicioCotizacion, 
cs.id_servicioextra IdServicioExtra, 
cs.id_cotizacion IdCotizacion, 
ISNULL(cs.id_direccion_cotizacion,0)  IdDireccionCotizacion,
cs.precio_unitario PrecioUnitario, 
cs.id_frecuencia IdFrecuencia,
cs.cantidad Cantidad, 
cs.total Total, 
cs.importemensual ImporteMensual,
cs.fecha_alta FechaAlta, 
cs.id_personal IdPersonal
FROM tb_cotiza_servicioextra cs
LEFT OUTER JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = cs.id_direccion_cotizacion
WHERE cs.id_cotizacion = @idCotizacion
ORDER BY cs.fecha_alta
";
            var serviciosCotizacion = new List<ServicioCotizacion>();
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    serviciosCotizacion = (await connection.QueryAsync<ServicioCotizacion>(query, new { idCotizacion, })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return serviciosCotizacion;
        }
        public async Task AgregarHerramientaCotizacion(MaterialCotizacion herramienta)
        {
            var query = @"INSERT INTO tb_cotiza_herramienta (clave_producto, id_cotizacion, id_direccion_cotizacion, id_puesto_direccioncotizacion, precio_unitario, cantidad, total, importemensual, id_frecuencia, fecha_alta, id_personal)
                        VALUES(@ClaveProducto, @IdCotizacion, @IdDireccionCotizacion, @IdPuestoDireccionCotizacion, @PrecioUnitario, @Cantidad, @Total, @ImporteMensual, @IdFrecuencia, @FechaAlta, @IdPersonal );";

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, herramienta);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task AgregarMaterialCotizacion(MaterialCotizacion material)
        {
            var query = @"INSERT INTO tb_cotiza_material (clave_producto, id_cotizacion, id_direccion_cotizacion, id_puesto_direccioncotizacion,
                            precio_unitario, cantidad, total, importemensual, fecha_alta, id_personal, id_frecuencia)
                        VALUES(@ClaveProducto, @IdCotizacion, @IdDireccionCotizacion, @IdPuestoDireccionCotizacion, 
                          @PrecioUnitario, @Cantidad, @Total, @ImporteMensual, @FechaAlta, @IdPersonal, @IdFrecuencia);";

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, material);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task ActualizarMaterialCotizacion(MaterialCotizacion material)
        {
            var query = @"UPDATE tb_cotiza_material 
                        SET 
                        precio_unitario = @PrecioUnitario,
                        cantidad = @Cantidad,
                        total = @Total, 
                        importemensual = @ImporteMensual,
                        id_frecuencia = @IdFrecuencia,
                        clave_producto = @ClaveProducto,
                        id_direccion_cotizacion = @IdDireccionCotizacion
                        WHERE id_material_cotizacion = @IdMaterialCotizacion";

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, material);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> ObtieneIdCotizacionPorMaterial(int idMaterialCotizacion)
        {
            var idCotizacion = 0;

            var query = @"SELECT id_cotizacion IdCotizacion 
                          FROM tb_cotiza_material
                          WHERE id_material_cotizacion = @idMaterialCotizacion";

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    idCotizacion = await connection.ExecuteScalarAsync<int>(query, new { idMaterialCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return idCotizacion;
        }
        public async Task<int> ObtieneIdPuestoDireccionCotizacionPorMaterial(int idMaterialCotizacion)
        {
            var idPuestoDireccionCotizacion = 0;

            var query = @"SELECT id_puesto_direccioncotizacion IdPuestoDireccionCotizacion
                          FROM tb_cotiza_material
                          WHERE id_material_cotizacion = @idMaterialCotizacion";

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    idPuestoDireccionCotizacion = await connection.ExecuteScalarAsync<int>(query, new { idMaterialCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return idPuestoDireccionCotizacion;
        }
        public async Task<int> ObtieneIdDireccionCotizacionPorMaterial(int idMaterialCotizacion)
        {
            var idDireccionCotizacion = 0;

            var query = @"SELECT id_direccion_cotizacion IdDireccionCotizacion 
                          FROM tb_cotiza_material
                          WHERE id_material_cotizacion = @idMaterialCotizacion";

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    idDireccionCotizacion = await connection.ExecuteScalarAsync<int>(query, new { idMaterialCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return idDireccionCotizacion;
        }
        public async Task<bool> EliminarMaterialCotizacion(int idMaterialCotizacion)
        {
            bool reg = false;
            var query = @"DELETE 
                          FROM tb_cotiza_material
                          WHERE id_material_cotizacion = @idMaterialCotizacion";

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new { idMaterialCotizacion });
                }
                reg = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return reg;
        }
        public async Task AgregarEquipoCotizacion(MaterialCotizacion equipo)
        {
            string query = @"INSERT INTO tb_cotiza_equipo(id_cotizacion, id_direccion_cotizacion, id_puesto_direccioncotizacion, clave_producto, precio_unitario,
                            cantidad, id_frecuencia, total, importemensual, fecha_alta, id_personal)
                        VALUES(@IdCotizacion, @IdDireccionCotizacion, @IdPuestoDireccionCotizacion, @ClaveProducto, @PrecioUnitario,
                            @Cantidad, @IdFrecuencia, @Total, @ImporteMensual, @FechaAlta, @IdPersonal);";

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, equipo);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task AgregarUniformeCotizacion(MaterialCotizacion uniforme)
        {
            string query = @"INSERT INTO tb_cotiza_uniforme(id_cotizacion, id_direccion_cotizacion, id_puesto_direccioncotizacion, clave_producto, precio_unitario,
                            cantidad, id_frecuencia, total, importemensual, fecha_alta, id_personal)
                        VALUES(@IdCotizacion, @IdDireccionCotizacion, @IdPuestoDireccionCotizacion, @ClaveProducto, @PrecioUnitario,
                            @Cantidad, @IdFrecuencia, @Total, @ImporteMensual, @FechaAlta, @IdPersonal);";

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, uniforme);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task ActualizarEquipoCotizacion(MaterialCotizacion equipo)
        {
            string query = @"UPDATE tb_cotiza_equipo
                        SET precio_unitario = @PrecioUnitario,
                        cantidad = @Cantidad,
                        total = @Total, 
                        importemensual = @ImporteMensual,
                        id_frecuencia = @IdFrecuencia,
                        clave_producto = @ClaveProducto,
                        id_direccion_cotizacion = @IdDireccionCotizacion
                        WHERE id_equipo_cotizacion = @IdMaterialCotizacion";

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, equipo);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task ActualizarHerramientaCotizacion(MaterialCotizacion herramienta)
        {
            string query = @"UPDATE tb_cotiza_herramienta
                        SET precio_unitario = @PrecioUnitario,
                        cantidad = @Cantidad,
                        total = @Total, 
                        importemensual = @ImporteMensual,
                        id_frecuencia = @IdFrecuencia,
                        clave_producto = @ClaveProducto,
                        id_direccion_cotizacion = @IdDireccionCotizacion
                        WHERE id_herramienta_cotizacion = @IdMaterialCotizacion";

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, herramienta);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task ActualizarUniformeCotizacion(MaterialCotizacion uniforme)
        {
            string query = @"UPDATE tb_cotiza_uniforme
                        SET precio_unitario = @PrecioUnitario,
                        cantidad = @Cantidad,
                        total = @Total, 
                        importemensual = @ImporteMensual,
                        id_frecuencia = @IdFrecuencia,
                        clave_producto = @ClaveProducto,
                        id_direccion_cotizacion = @IdDireccionCotizacion
                        WHERE id_uniforme_cotizacion = @IdMaterialCotizacion";

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, uniforme);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task InsertarServicioCotizacion(ServicioCotizacion servicio)
        {
            string query = @"INSERT INTO tb_cotiza_servicioextra
(
id_servicioextra,
id_cotizacion,
id_direccion_cotizacion,
precio_unitario,
cantidad,
total,
importemensual,
id_frecuencia,
fecha_alta,
id_personal
)
VALUES
(
@IdServicioExtra,
@IdCotizacion,
@IdDireccionCotizacion,
@PrecioUnitario,
@Cantidad,
@Total,
@ImporteMensual,
@IdFrecuencia,
@FechaAlta,
@IdPersonal
)";
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, servicio);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task ActualizarServicioCotizacion(ServicioCotizacion servicio)
        {
            string query = @"
UPDATE tb_cotiza_servicioextra
SET 
id_servicioextra = @IdServicioExtra,
id_direccion_cotizacion = @IdDireccionCotizacion,
precio_unitario = @PrecioUnitario,
cantidad = @Cantidad,
total = @Total, 
importemensual = @ImporteMensual,
id_frecuencia = @IdFrecuencia
WHERE id_servicioextra_cotizacion = @IdServicioExtraCotizacion";
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, servicio);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public async Task EliminarServicioCotizacion(int id)
        {
            string query = @"DELETE FROM tb_cotiza_servicioextra WHERE id_servicioextra_cotizacion = @id";

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new { id });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> EliminarEquipoCotizacion(int idEquipoCotizacion)
        {
            bool reg = false;
            string query = @"DELETE
                        FROM tb_cotiza_equipo
                        WHERE id_equipo_cotizacion = @IdEquipoCotizacion";

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new { idEquipoCotizacion });
                }
                reg = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return reg;
        }
        public async Task<bool> EliminarHerramientaCotizacion(int idHerramientaCotizacion)
        {
            bool reg = false;
            string query = @"DELETE
                        FROM tb_cotiza_herramienta
                        WHERE id_herramienta_cotizacion = @IdHerramientaCotizacion";

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new { idHerramientaCotizacion });
                }
                reg = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return reg;
        }
        public async Task<bool> EliminarUniformeCotizacion(int idUniformeCotizacion)
        {
            bool reg = false;
            string query = @"DELETE
                        FROM tb_cotiza_uniforme
                        WHERE id_uniforme_cotizacion = @IdUniformeCotizacion";

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new { idUniformeCotizacion });
                }
                reg = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return reg;
        }
        public async Task<int> ContarEquipoCotizacion(int idCotizacion, int idDireccionCotizacion, int idPuestoDireccionCotizacion, string keywords)
        {
            var query = @"SELECT count(mc.id_equipo_cotizacion) Rows
                        FROM tb_cotiza_equipo mc
						JOIN tb_producto p on p.clave = mc.clave_producto
                        JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = mc.id_direccion_cotizacion
                        left outer JOIN tb_puesto_direccion_cotizacion pdc on pdc.id_puesto_direccioncotizacion = mc.id_puesto_direccioncotizacion
                        WHERE 
                            mc.id_cotizacion = @idCotizacion AND
							ISNULL(NULLIF(@idDireccionCotizacion, 0), dc.id_direccion_cotizacion) = dc.id_direccion_cotizacion AND
							mc.id_puesto_direccioncotizacion = @IdPuestoDireccionCotizacion AND
							p.descripcion LIKE '%' + ISNULL(@keywords, '') + '%';";

            var rows = 0;

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    rows = await connection.QuerySingleAsync<int>(query, new { idCotizacion, idDireccionCotizacion, idPuestoDireccionCotizacion, keywords });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return rows;
        }
        public async Task<int> ContarHerramientaCotizacion(int idCotizacion, int idDireccionCotizacion, int idPuestoDireccionCotizacion, string keywords)
        {
            var query = @"SELECT count(mc.id_herramienta_cotizacion) Rows
                        FROM tb_cotiza_herramienta mc
						JOIN tb_producto p on p.clave = mc.clave_producto
                        JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = mc.id_direccion_cotizacion
                        left outer JOIN tb_puesto_direccion_cotizacion pdc on pdc.id_puesto_direccioncotizacion = mc.id_puesto_direccioncotizacion
                        WHERE 
                            mc.id_cotizacion = @idCotizacion AND
							ISNULL(NULLIF(@idDireccionCotizacion, 0), dc.id_direccion_cotizacion) = dc.id_direccion_cotizacion AND
							mc.id_puesto_direccioncotizacion = @IdPuestoDireccionCotizacion AND
							p.descripcion LIKE '%' + ISNULL(@keywords, '') + '%';";

            var rows = 0;

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    rows = await connection.QuerySingleAsync<int>(query, new { idCotizacion, idDireccionCotizacion, idPuestoDireccionCotizacion, keywords });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return rows;
        }
        public async Task<int> ContarUniformeCotizacion(int idCotizacion, int idDireccionCotizacion, int idPuestoDireccionCotizacion, string keywords)
        {
            var query = @"SELECT count(mc.id_uniforme_cotizacion) Rows
                        FROM tb_cotiza_uniforme mc
						JOIN tb_producto p on p.clave = mc.clave_producto
                        JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = mc.id_direccion_cotizacion
                        left outer JOIN tb_puesto_direccion_cotizacion pdc on pdc.id_puesto_direccioncotizacion = mc.id_puesto_direccioncotizacion
                        WHERE 
                            mc.id_cotizacion = @idCotizacion AND
							ISNULL(NULLIF(@idDireccionCotizacion, 0), dc.id_direccion_cotizacion) = dc.id_direccion_cotizacion AND
							mc.id_puesto_direccioncotizacion = @IdPuestoDireccionCotizacion AND
							p.descripcion LIKE '%' + ISNULL(@keywords, '') + '%';";

            var rows = 0;

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    rows = await connection.QuerySingleAsync<int>(query, new { idCotizacion, idDireccionCotizacion, idPuestoDireccionCotizacion, keywords });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return rows;
        }
        public async Task<List<MaterialCotizacion>> ObtenerEquipoCotizacion(int idCotizacion, int idDireccionCotizacion, int idPuestoDireccionCotizacion, string keywords, int pagina)
        {
            var query = @"select mc.id_equipo_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
                            mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
                            mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal, p.descripcion DescripcionMaterial,
                            d.nombre_sucursal NombreSucursal, pu.descripcion DescripcionPuesto
                        FROM tb_cotiza_equipo mc
                        JOIN tb_producto p on p.clave = mc.clave_producto
                        JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = mc.id_direccion_cotizacion
                        JOIN tb_direccion d on d.id_direccion = dc.id_direccion
                        left outer JOIN tb_puesto_direccion_cotizacion pdc on pdc.id_puesto_direccioncotizacion = mc.id_puesto_direccioncotizacion
                        left outer JOIN tb_puesto pu on pu.id_puesto = pdc.id_puesto
                        WHERE mc.id_cotizacion = @idCotizacion AND
						ISNULL(NULLIF(@idDireccionCotizacion, 0), dc.id_direccion_cotizacion) = dc.id_direccion_cotizacion AND
						mc.id_puesto_direccioncotizacion = @idPuestoDireccionCotizacion AND
						p.descripcion LIKE '%' + ISNULL(@keywords, '') + '%'
                        order by p.descripcion
                        OFFSET ((@pagina - 1) * 10) rows
                        fetch next 10 rows only;";

            var materialesCotizacion = new List<MaterialCotizacion>();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    materialesCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new
                    {
                        idCotizacion,
                        pagina,
                        idDireccionCotizacion,
                        idPuestoDireccionCotizacion,
                        keywords
                    })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return materialesCotizacion;
        }
        public async Task<List<MaterialCotizacion>> ObtenerHerramientaCotizacion(int idCotizacion, int idDireccionCotizacion, int idPuestoDireccionCotizacion, string keywords, int pagina)
        {
            var query = @"select mc.id_herramienta_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
                            mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
                            mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal, p.descripcion DescripcionMaterial,
                            d.nombre_sucursal NombreSucursal, pu.descripcion DescripcionPuesto
                        FROM tb_cotiza_herramienta mc
                        JOIN tb_producto p on p.clave = mc.clave_producto
                        JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = mc.id_direccion_cotizacion
                        JOIN tb_direccion d on d.id_direccion = dc.id_direccion
                        left outer JOIN tb_puesto_direccion_cotizacion pdc on pdc.id_puesto_direccioncotizacion = mc.id_puesto_direccioncotizacion
                        left outer JOIN tb_puesto pu on pu.id_puesto = pdc.id_puesto
                        WHERE mc.id_cotizacion = @idCotizacion AND
						ISNULL(NULLIF(@idDireccionCotizacion, 0), dc.id_direccion_cotizacion) = dc.id_direccion_cotizacion AND
						mc.id_puesto_direccioncotizacion = @idPuestoDireccionCotizacion AND
						p.descripcion LIKE '%' + ISNULL(@keywords, '') + '%'
                        order by p.descripcion
                        OFFSET ((@pagina - 1) * 10) rows
                        fetch next 10 rows only;";

            var materialesCotizacion = new List<MaterialCotizacion>();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    materialesCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new
                    {
                        idCotizacion,
                        pagina,
                        idDireccionCotizacion,
                        idPuestoDireccionCotizacion,
                        keywords
                    })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return materialesCotizacion;
        }
        public async Task<List<MaterialCotizacion>> ObtenerUniformeCotizacion(int idCotizacion, int idDireccionCotizacion, int idPuestoDireccionCotizacion, string keywords, int pagina)
        {
            var query = @"select mc.id_uniforme_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
                            mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
                            mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal, p.descripcion DescripcionMaterial,
                            d.nombre_sucursal NombreSucursal, pu.descripcion DescripcionPuesto
                        FROM tb_cotiza_uniforme mc
                        JOIN tb_producto p on p.clave = mc.clave_producto
                        JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = mc.id_direccion_cotizacion
                        JOIN tb_direccion d on d.id_direccion = dc.id_direccion
                        left outer JOIN tb_puesto_direccion_cotizacion pdc on pdc.id_puesto_direccioncotizacion = mc.id_puesto_direccioncotizacion
                        left outer JOIN tb_puesto pu on pu.id_puesto = pdc.id_puesto
                        WHERE mc.id_cotizacion = @idCotizacion AND
						ISNULL(NULLIF(@idDireccionCotizacion, 0), dc.id_direccion_cotizacion) = dc.id_direccion_cotizacion AND
						mc.id_puesto_direccioncotizacion = @idPuestoDireccionCotizacion AND
						p.descripcion LIKE '%' + ISNULL(@keywords, '') + '%'
                        order by p.descripcion
                        OFFSET ((@pagina - 1) * 10) rows
                        fetch next 10 rows only;";

            var materialesCotizacion = new List<MaterialCotizacion>();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    materialesCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new
                    {
                        idCotizacion,
                        pagina,
                        idDireccionCotizacion,
                        idPuestoDireccionCotizacion,
                        keywords
                    })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return materialesCotizacion;
        }
        public async Task<List<MaterialCotizacion>> ObtenerEquipoCotizacionOperario(int id, int idCotizacion)
        {
            if (id != 0)
            {
                string query = @"SELECT mc.id_equipo_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
                                mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
                                mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal, p.descripcion DescripcionMaterial
                            FROM tb_cotiza_equipo mc
                            JOIN tb_producto p on p.clave = mc.clave_producto
                            WHERE id_puesto_direccioncotizacion = @id
ORDER BY p.descripcion";

                var equipoCotizacion = new List<MaterialCotizacion>();

                try
                {
                    using (var connection = _ctx.CreateConnection())
                    {
                        equipoCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new { id })).ToList();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return equipoCotizacion;
            }
            else
            {
                string query = @"SELECT mc.id_equipo_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
                                mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
                                mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal, p.descripcion DescripcionMaterial
                            FROM tb_cotiza_equipo mc
                            JOIN tb_producto p on p.clave = mc.clave_producto
                            WHERE id_cotizacion = @idCotizacion
ORDER BY p.descripcion";

                var equipoCotizacion = new List<MaterialCotizacion>();

                try
                {
                    using (var connection = _ctx.CreateConnection())
                    {
                        equipoCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new { idCotizacion })).ToList();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return equipoCotizacion;
            }

        }
        public async Task<List<MaterialCotizacion>> ObtenerHerramientaCotizacionOperario(int id, int idCotizacion)
        {
            if (id != 0)
            {
                string query = @"SELECT mc.id_herramienta_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
                                mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
                                mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal, p.descripcion DescripcionMaterial
                            FROM tb_cotiza_herramienta mc
                            JOIN tb_producto p on p.clave = mc.clave_producto
                            WHERE id_puesto_direccioncotizacion = @id
ORDER BY p.descripcion";

                var equipoCotizacion = new List<MaterialCotizacion>();

                try
                {
                    using (var connection = _ctx.CreateConnection())
                    {
                        equipoCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new { id })).ToList();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return equipoCotizacion;
            }
            else
            {
                string query = @"SELECT mc.id_herramienta_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
                                mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
                                mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal, p.descripcion DescripcionMaterial
                            FROM tb_cotiza_herramienta mc
                            JOIN tb_producto p on p.clave = mc.clave_producto
                            WHERE id_cotizacion = @idCotizacion
ORDER BY p.descripcion";

                var equipoCotizacion = new List<MaterialCotizacion>();

                try
                {
                    using (var connection = _ctx.CreateConnection())
                    {
                        equipoCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new { idCotizacion })).ToList();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return equipoCotizacion;
            }

        }
        public async Task<List<MaterialCotizacion>> ObtenerUniformeCotizacionOperario(int id, int idCotizacion)
        {
            if (id != 0)
            {
                string query = @"SELECT mc.id_uniforme_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
                                mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
                                mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal, p.descripcion DescripcionMaterial
                            FROM tb_cotiza_uniforme mc
                            JOIN tb_producto p on p.clave = mc.clave_producto
                            WHERE id_puesto_direccioncotizacion = @id
                            ORDER BY p.descripcion";

                var equipoCotizacion = new List<MaterialCotizacion>();

                try
                {
                    using (var connection = _ctx.CreateConnection())
                    {
                        equipoCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new { id })).ToList();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return equipoCotizacion;
            }
            else
            {
                string query = @"SELECT mc.id_uniforme_cotizacion IdMaterialCotizacion, 
mc.clave_producto ClaveProducto, 
mc.id_cotizacion IdCotizacion, 
mc.id_direccion_cotizacion IdDireccionCotizacion,
mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, 
mc.precio_unitario PrecioUnitario, 
id_frecuencia IdFrecuencia,
mc.cantidad Cantidad, 
mc.total Total, 
mc.fecha_alta FechaAlta, 
mc.id_personal IdPersonal, 
p.descripcion DescripcionMaterial
                            FROM tb_cotiza_uniforme mc
                            JOIN tb_producto p on p.clave = mc.clave_producto
                            WHERE id_cotizacion = @idCotizacion
ORDER BY p.descripcion";

                var equipoCotizacion = new List<MaterialCotizacion>();

                try
                {
                    using (var connection = _ctx.CreateConnection())
                    {
                        equipoCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new { idCotizacion })).ToList();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return equipoCotizacion;
            }

        }
        public async Task<List<MaterialCotizacion>> ObtenerMaterialCotizacionOperario(int id, int idCotizacion)
        {
            if (id != 0)
            {
                var query = $@"SELECT mc.id_material_cotizacion IdMaterialCotizacion,
                            mc.clave_producto ClaveProducto,
                            mc.id_cotizacion IdCotizacion,
                            mc.id_direccion_cotizacion IdDireccionCotizacion,
                            mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion,
                            mc.precio_unitario PrecioUnitario,
                            id_frecuencia IdFrecuencia,
                            mc.cantidad Cantidad,
                            mc.total Total,
                            mc.fecha_alta FechaAlta,
                            mc.id_personal IdPersonal,
                            p.descripcion DescripcionMaterial
                            FROM tb_cotiza_material mc
                            JOIN tb_producto p on p.clave = mc.clave_producto
                            WHERE id_puesto_direccioncotizacion = @id
                            ORDER BY p.descripcion";

                var naterialCotizacion = new List<MaterialCotizacion>();

                try
                {
                    using (var connection = _ctx.CreateConnection())
                    {
                        naterialCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new { id })).ToList();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return naterialCotizacion;
            }
            else
            {
                var query = $@"SELECT mc.id_material_cotizacion IdMaterialCotizacion, 
                            mc.clave_producto ClaveProducto, 
                            mc.id_cotizacion IdCotizacion, 
                            mc.id_direccion_cotizacion IdDireccionCotizacion,
                            mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, 
                            mc.precio_unitario PrecioUnitario, 
                            id_frecuencia IdFrecuencia,
                            mc.cantidad Cantidad, 
                            mc.total Total, 
                            mc.fecha_alta FechaAlta, 
                            mc.id_personal IdPersonal, 
                            p.descripcion DescripcionMaterial
                            FROM tb_cotiza_material mc
                            JOIN tb_producto p on p.clave = mc.clave_producto
                            WHERE id_cotizacion = @idCotizacion
ORDER BY p.descripcion";

                var naterialCotizacion = new List<MaterialCotizacion>();

                try
                {
                    using (var connection = _ctx.CreateConnection())
                    {
                        naterialCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new { idCotizacion })).ToList();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return naterialCotizacion;
            }

        }
        public async Task<MaterialCotizacion> ObtenerEquipoCotizacionPorId(int id)
        {
            var query = @"SELECT mc.id_equipo_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
                                  mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
                                  mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal
                            FROM tb_cotiza_equipo mc
                            WHERE id_equipo_cotizacion = @id";

            var naterialCotizacion = new MaterialCotizacion();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    naterialCotizacion = await connection.QueryFirstAsync<MaterialCotizacion>(query, new { id });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return naterialCotizacion;
        }
        public async Task<MaterialCotizacion> ObtenerHerramientaCotizacionPorId(int id)
        {
            var query = @"SELECT mc.id_herramienta_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
                                  mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
                                  mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal
                            FROM tb_cotiza_herramienta mc
                            WHERE id_herramienta_cotizacion = @id";

            var naterialCotizacion = new MaterialCotizacion();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    naterialCotizacion = await connection.QueryFirstAsync<MaterialCotizacion>(query, new { id });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return naterialCotizacion;
        }
        public async Task<MaterialCotizacion> ObtenerUniformeCotizacionPorId(int id)
        {
            var query = @"SELECT mc.id_uniforme_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
                                  mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
                                  mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal
                            FROM tb_cotiza_uniforme mc
                            WHERE id_uniforme_cotizacion = @id;";

            var naterialCotizacion = new MaterialCotizacion();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    naterialCotizacion = await connection.QueryFirstAsync<MaterialCotizacion>(query, new { id });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return naterialCotizacion;
        }
        public async Task<ServicioCotizacion> ServicioGetById(int id)
        {
            var query = @"SELECT 
cse.id_servicioextra_cotizacion IdServicioExtraCotizacion,
cse.id_servicioextra IdServicioExtra,
se.descripcion ServicioExtra,
cse.id_cotizacion IdCotizacion,
ISNULL(cse.id_direccion_cotizacion,0) IdDireccionCotizacion,
ISNULL(d.nombre_sucursal,'General') Direccion,
cse.precio_unitario PrecioUnitario,
cse.cantidad Cantidad,
cse.total Total,
cse.importemensual ImporteMensual,
cse.id_frecuencia IdFrecuencia,
cse.fecha_alta FechaAlta,
cse.id_personal IdPersonal
FROM tb_cotiza_servicioextra cse
INNER JOIN tb_servicioextra se ON cse.id_servicioextra =  se.id_servicioextra
LEFT JOIN tb_direccion_cotizacion dc ON dc.id_direccion_cotizacion = cse.id_direccion_cotizacion
LEFT JOIN tb_direccion d ON d.id_direccion = dc.id_direccion
WHERE cse.id_servicioextra_cotizacion = @id";

            var servicio = new ServicioCotizacion();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    servicio = await connection.QueryFirstAsync<ServicioCotizacion>(query, new { id });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return servicio;
        }
        public async Task InsertarEquipoCotizacion(List<MaterialCotizacion> materialList)
        {
            var query = "insert into tb_cotiza_equipo (clave_producto, id_cotizacion, id_direccion_cotizacion, id_puesto_direccioncotizacion, precio_unitario, cantidad, total, importemensual, fecha_alta, id_personal, id_frecuencia) values";
            var row = "('{0}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, '{8}', {9}, {10}),";

            foreach (var material in materialList)
            {
                query += string.Format(row, material.ClaveProducto, material.IdCotizacion, material.IdDireccionCotizacion, material.IdPuestoDireccionCotizacion,
                    material.PrecioUnitario, material.Cantidad, material.Total, material.ImporteMensual, DateTime.Now.ToString("yyyy/MM/dd"), material.IdPersonal, (int)material.IdFrecuencia);
            }

            query = query.Remove(query.Length - 1);

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task InsertarHerramientaCotizacion(List<MaterialCotizacion> materialList)
        {
            var query = "insert into tb_cotiza_herramienta (clave_producto, id_cotizacion, id_direccion_cotizacion, id_puesto_direccioncotizacion, precio_unitario, cantidad, total, importemensual, fecha_alta, id_personal, id_frecuencia) values";
            var row = "('{0}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, '{8}', {9}, {10}),";

            foreach (var material in materialList)
            {
                query += string.Format(row, material.ClaveProducto, material.IdCotizacion, material.IdDireccionCotizacion, material.IdPuestoDireccionCotizacion,
                    material.PrecioUnitario, material.Cantidad, material.Total, material.ImporteMensual, DateTime.Now.ToString("yyyy/MM/dd"), material.IdPersonal, (int)material.IdFrecuencia);
            }

            query = query.Remove(query.Length - 1);

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task InsertarUniformeCotizacion(List<MaterialCotizacion> materialList)
        {
            var query = "insert into tb_cotiza_uniforme (clave_producto, id_cotizacion, id_direccion_cotizacion, id_puesto_direccioncotizacion, precio_unitario, cantidad, total, importemensual, fecha_alta, id_personal, id_frecuencia) values";
            var row = "('{0}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, '{8}', {9}, {10}),";

            foreach (var material in materialList)
            {
                query += string.Format(row, material.ClaveProducto, material.IdCotizacion, material.IdDireccionCotizacion, material.IdPuestoDireccionCotizacion,
                    material.PrecioUnitario, material.Cantidad, material.Total, material.ImporteMensual, DateTime.Now.ToString("yyyy/MM/dd"), material.IdPersonal, (int)material.IdFrecuencia);
            }

            query = query.Remove(query.Length - 1);

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<MaterialCotizacion>> ObtenerMaterialExtraCotizacion(int idCotizacion)
        {
            var query = @"select mc.id_material_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
                            mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
                            mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal, p.descripcion DescripcionMaterial,
                            d.nombre_sucursal NombreSucursal, pu.descripcion DescripcionPuesto
                        FROM tb_cotiza_material mc
                        JOIN tb_producto p on p.clave = mc.clave_producto
                        JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = mc.id_direccion_cotizacion
                        JOIN tb_direccion d on d.id_direccion = dc.id_direccion
                        left outer JOIN tb_puesto_direccion_cotizacion pdc on pdc.id_puesto_direccioncotizacion = mc.id_puesto_direccioncotizacion
                        left outer JOIN tb_puesto pu on pu.id_puesto = pdc.id_puesto
                        WHERE mc.id_cotizacion = @idCotizacion AND
                        mc.id_puesto_direccioncotizacion = 0
                        order by p.descripcion";

            var materialesCotizacion = new List<MaterialCotizacion>();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    materialesCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new
                    {
                        idCotizacion
                    })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return materialesCotizacion;
        }
        public async Task<List<MaterialCotizacion>> ObtenerHerramientaExtraCotizacion(int idCotizacion)
        {
            var query = @"select mc.id_herramienta_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
                            mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
                            mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal, p.descripcion DescripcionMaterial,
                            d.nombre_sucursal NombreSucursal, pu.descripcion DescripcionPuesto
                        FROM tb_cotiza_herramienta mc
                        JOIN tb_producto p on p.clave = mc.clave_producto
                        JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = mc.id_direccion_cotizacion
                        JOIN tb_direccion d on d.id_direccion = dc.id_direccion
                        left outer JOIN tb_puesto_direccion_cotizacion pdc on pdc.id_puesto_direccioncotizacion = mc.id_puesto_direccioncotizacion
                        left outer JOIN tb_puesto pu on pu.id_puesto = pdc.id_puesto
                        WHERE mc.id_cotizacion = @idCotizacion AND
                        mc.id_puesto_direccioncotizacion = 0
                        order by p.descripcion";

            var herramientaCotizacion = new List<MaterialCotizacion>();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    herramientaCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new
                    {
                        idCotizacion
                    })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return herramientaCotizacion;
        }
        public async Task<List<MaterialCotizacion>> ObtenerEquipoExtraCotizacion(int idCotizacion)
        {
            var query = @"select mc.id_equipo_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
                            mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
                            mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal, p.descripcion DescripcionMaterial,
                            d.nombre_sucursal NombreSucursal, pu.descripcion DescripcionPuesto
                        FROM tb_cotiza_equipo mc
                        JOIN tb_producto p on p.clave = mc.clave_producto
                        JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = mc.id_direccion_cotizacion
                        JOIN tb_direccion d on d.id_direccion = dc.id_direccion
                        left outer JOIN tb_puesto_direccion_cotizacion pdc on pdc.id_puesto_direccioncotizacion = mc.id_puesto_direccioncotizacion
                        left outer JOIN tb_puesto pu on pu.id_puesto = pdc.id_puesto
                        WHERE mc.id_cotizacion = @idCotizacion AND
                        mc.id_puesto_direccioncotizacion = 0
                        order by p.descripcion";

            var equipoCotizacion = new List<MaterialCotizacion>();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    equipoCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new
                    {
                        idCotizacion
                    })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return equipoCotizacion;
        }
        public async Task<List<MaterialCotizacion>> ObtenerUniformeExtraCotizacion(int idCotizacion)
        {
            var query = @"select mc.id_uniforme_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
                            mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
                            mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal, p.descripcion DescripcionMaterial,
                            d.nombre_sucursal NombreSucursal, pu.descripcion DescripcionPuesto
                        FROM tb_cotiza_uniforme mc
                        JOIN tb_producto p on p.clave = mc.clave_producto
                        JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = mc.id_direccion_cotizacion
                        JOIN tb_direccion d on d.id_direccion = dc.id_direccion
                        left outer JOIN tb_puesto_direccion_cotizacion pdc on pdc.id_puesto_direccioncotizacion = mc.id_puesto_direccioncotizacion
                        left outer JOIN tb_puesto pu on pu.id_puesto = pdc.id_puesto
                        WHERE mc.id_cotizacion = @idCotizacion AND
                        mc.id_puesto_direccioncotizacion = 0
                        order by p.descripcion";

            var uniformeCotizacion = new List<MaterialCotizacion>();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    uniformeCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new
                    {
                        idCotizacion
                    })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return uniformeCotizacion;
        }
        public async Task<List<ServicioCotizacion>> ObtenerServicioExtraCotizacion(int idCotizacion)
        {
            var servicioscotizacion = new List<ServicioCotizacion>();
            var query = @"SELECT 
                            cse.id_servicioextra_cotizacion IdServicioExtraCotizacion,
                            cse.id_servicioextra IdServicioExtra,
                            se.descripcion ServicioExtra,
                            cse.id_cotizacion IdCotizacion,
                            ISNULL(cse.id_direccion_cotizacion,0) IdDireccionCotizacion,
                            ISNULL(d.nombre_sucursal,'General') Direccion,
                            cse.precio_unitario PrecioUnitario,
                            cse.cantidad Cantidad,
                            cse.total Total,
                            cse.importemensual ImporteMensual,
                            cse.id_frecuencia IdFrecuencia,
                            cse.fecha_alta FechaAlta,
                            cse.id_personal IdPersonal
                        FROM tb_cotiza_servicioextra cse
                        INNER JOIN tb_servicioextra se ON cse.id_servicioextra = se.id_servicioextra
                        LEFT JOIN tb_direccion_cotizacion dc ON dc.id_direccion_cotizacion = cse.id_direccion_cotizacion
                        LEFT JOIN tb_direccion d ON d.id_direccion = dc.id_direccion
                        WHERE cse.id_cotizacion = @idCotizacion";

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    servicioscotizacion = (await connection.QueryAsync<ServicioCotizacion>(query, new { idCotizacion })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return servicioscotizacion;
        }

        public async Task<List<MaterialCotizacion>> ObtenerMaterialExtraCotizacionDireccion(int idCotizacion, int idDireccionCotizacion)
        {
            var query = @"select mc.id_material_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
                            mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
                            mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal, p.descripcion DescripcionMaterial,
                            d.nombre_sucursal NombreSucursal, pu.descripcion DescripcionPuesto
                        FROM tb_cotiza_material mc
                        JOIN tb_producto p on p.clave = mc.clave_producto
                        JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = mc.id_direccion_cotizacion
                        JOIN tb_direccion d on d.id_direccion = dc.id_direccion
                        left outer JOIN tb_puesto_direccion_cotizacion pdc on pdc.id_puesto_direccioncotizacion = mc.id_puesto_direccioncotizacion
                        left outer JOIN tb_puesto pu on pu.id_puesto = pdc.id_puesto
                        WHERE mc.id_cotizacion = @idCotizacion AND
                        mc.id_direccion_cotizacion = @idDireccionCotizacion AND
						mc.id_puesto_direccioncotizacion = 0
                        order by mc.clave_producto";

            var materialesCotizacion = new List<MaterialCotizacion>();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    materialesCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new
                    {
                        idCotizacion,
                        idDireccionCotizacion
                    })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return materialesCotizacion;
        }
        public async Task<List<MaterialCotizacion>> ObtenerHerramientaExtraCotizacionDireccion(int idCotizacion, int idDireccionCotizacion)
        {
            var query = @"select mc.id_herramienta_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
                            mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
                            mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal, p.descripcion DescripcionMaterial,
                            d.nombre_sucursal NombreSucursal, pu.descripcion DescripcionPuesto
                        FROM tb_cotiza_herramienta mc
                        JOIN tb_producto p on p.clave = mc.clave_producto
                        JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = mc.id_direccion_cotizacion
                        JOIN tb_direccion d on d.id_direccion = dc.id_direccion
                        left outer JOIN tb_puesto_direccion_cotizacion pdc on pdc.id_puesto_direccioncotizacion = mc.id_puesto_direccioncotizacion
                        left outer JOIN tb_puesto pu on pu.id_puesto = pdc.id_puesto
                        WHERE mc.id_cotizacion = @idCotizacion AND
                        mc.id_direccion_cotizacion = @idDireccionCotizacion AND
                        mc.id_puesto_direccioncotizacion = 0
                        order by mc.clave_producto";

            var herramientaCotizacion = new List<MaterialCotizacion>();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    herramientaCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new
                    {
                        idCotizacion,
                        idDireccionCotizacion
                    })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return herramientaCotizacion;
        }
        public async Task<List<MaterialCotizacion>> ObtenerEquipoExtraCotizacionDireccion(int idCotizacion, int idDireccionCotizacion)
        {
            var query = @"select mc.id_equipo_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
                            mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
                            mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal, p.descripcion DescripcionMaterial,
                            d.nombre_sucursal NombreSucursal, pu.descripcion DescripcionPuesto
                        FROM tb_cotiza_equipo mc
                        JOIN tb_producto p on p.clave = mc.clave_producto
                        JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = mc.id_direccion_cotizacion
                        JOIN tb_direccion d on d.id_direccion = dc.id_direccion
                        left outer JOIN tb_puesto_direccion_cotizacion pdc on pdc.id_puesto_direccioncotizacion = mc.id_puesto_direccioncotizacion
                        left outer JOIN tb_puesto pu on pu.id_puesto = pdc.id_puesto
                        WHERE mc.id_cotizacion = @idCotizacion AND
                        mc.id_direccion_cotizacion = @idDireccionCotizacion AND
                        mc.id_puesto_direccioncotizacion = 0
                        order by mc.clave_producto";

            var equipoCotizacion = new List<MaterialCotizacion>();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    equipoCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new
                    {
                        idCotizacion,
                        idDireccionCotizacion
                    })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return equipoCotizacion;
        }
        public async Task<List<MaterialCotizacion>> ObtenerUniformeExtraCotizacionDireccion(int idCotizacion, int idDireccionCotizacion)
        {
            var query = @"select mc.id_uniforme_cotizacion IdMaterialCotizacion, mc.clave_producto ClaveProducto, mc.id_cotizacion IdCotizacion, mc.id_direccion_cotizacion IdDireccionCotizacion,
                            mc.id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, mc.precio_unitario PrecioUnitario, id_frecuencia IdFrecuencia,
                            mc.cantidad Cantidad, mc.total Total, mc.fecha_alta FechaAlta, mc.id_personal IdPersonal, p.descripcion DescripcionMaterial,
                            d.nombre_sucursal NombreSucursal, pu.descripcion DescripcionPuesto
                        FROM tb_cotiza_uniforme mc
                        JOIN tb_producto p on p.clave = mc.clave_producto
                        JOIN tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = mc.id_direccion_cotizacion
                        JOIN tb_direccion d on d.id_direccion = dc.id_direccion
                        left outer JOIN tb_puesto_direccion_cotizacion pdc on pdc.id_puesto_direccioncotizacion = mc.id_puesto_direccioncotizacion
                        left outer JOIN tb_puesto pu on pu.id_puesto = pdc.id_puesto
                        WHERE mc.id_cotizacion = @idCotizacion AND
                        mc.id_direccion_cotizacion = @idDireccionCotizacion AND
                        mc.id_puesto_direccioncotizacion = 0
                        order by mc.clave_producto";

            var uniformeCotizacion = new List<MaterialCotizacion>();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    uniformeCotizacion = (await connection.QueryAsync<MaterialCotizacion>(query, new
                    {
                        idCotizacion,
                        idDireccionCotizacion
                    })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return uniformeCotizacion;
        }
        public async Task<List<ServicioCotizacion>> ObtenerServicioExtraCotizacionDireccion(int idCotizacion, int idDireccionCotizacion)
        {
            var servicioscotizacion = new List<ServicioCotizacion>();
            var query = @"SELECT 
                            cse.id_servicioextra_cotizacion IdServicioExtraCotizacion,
                            cse.id_servicioextra IdServicioExtra,
                            se.descripcion ServicioExtra,
                            cse.id_cotizacion IdCotizacion,
                            ISNULL(cse.id_direccion_cotizacion,0) IdDireccionCotizacion,
                            ISNULL(d.nombre_sucursal,'General') Direccion,
                            cse.precio_unitario PrecioUnitario,
                            cse.cantidad Cantidad,
                            cse.total Total,
                            cse.importemensual ImporteMensual,
                            cse.id_frecuencia IdFrecuencia,
                            cse.fecha_alta FechaAlta,
                            cse.id_personal IdPersonal
                        FROM tb_cotiza_servicioextra cse
                        INNER JOIN tb_servicioextra se ON cse.id_servicioextra = se.id_servicioextra
                        LEFT JOIN tb_direccion_cotizacion dc ON dc.id_direccion_cotizacion = cse.id_direccion_cotizacion
                        LEFT JOIN tb_direccion d ON d.id_direccion = dc.id_direccion
                        WHERE cse.id_cotizacion = @idCotizacion";

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    servicioscotizacion = (await connection.QueryAsync<ServicioCotizacion>(query, new { idCotizacion })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return servicioscotizacion;
        }
    }
}
