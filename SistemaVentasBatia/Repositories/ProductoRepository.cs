using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;
using SistemaVentasBatia.DTOs;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;

namespace SistemaVentasBatia.Repositories
{
    public interface IProductoRepository
    {
        //MATERIAL
        Task AgregarMaterialPuesto(MaterialPuesto mat);
        Task ActualizarMaterialPuesto(MaterialPuesto mat);
        Task<bool> EliminarMaterialPuesto(int id);
        Task<IEnumerable<ProductoItem>> ObtenerMaterialPuesto(int id);

        //UNIFORME
        Task AgregarUniformePuesto(MaterialPuesto uni);
        Task ActualizarUniformePuesto(MaterialPuesto uni);
        Task<bool> EliminarUniformePuesto(int id);
        Task<IEnumerable<ProductoItem>> ObtenerUniformePuesto(int id);

        //EQUIPO
        Task AgregarEquipoPuesto(MaterialPuesto equi);
        Task ActualizarEquipoPuesto(MaterialPuesto equi);
        Task<bool> EliminarEquipoPuesto(int id);
        Task<IEnumerable<ProductoItem>> ObtenerEquipoPuesto(int id);

        //HERRAMIENTA
        Task AgregarHerramientaPuesto(MaterialPuesto her);
        Task ActualizarHerramientaPuesto(MaterialPuesto her);
        Task<bool> EliminarHerramientaPuesto(int id);
        Task<IEnumerable<ProductoItem>> ObtenerHerramientaPuesto(int id);

        //SERVICIO
        Task<bool> VerificarServiciosExistentes(int id);
        Task<bool> EliminarServicio(int id);
        Task<bool> AgregarServicio(string servicio, int idPersonal);

        //INDUSTRIA
        Task<bool> VerificarIndustriasExistentes(int id);
        Task<bool> EliminarIndustria(int id);
        Task<bool> AgregarIndustria(string industria, int idPersonal);

        //PRODUCTO
        Task<MaterialPuesto> ObtenerProductoDefault(int idProdcuto, int tipo, int idPuesto);
        Task<int> CountProductoProveedorByIdEstado(int idEstado, int idFamilia);
        Task<List<ProductoPrecioEstado>> GetProductoProveedorByIdEstado(int idEstado, int pagina, int idFamilia);

        Task<string> GetProveedorByIdEstado(int idEstado);
        Task<int> GetIdProveedorByIdEstado(int idEstado);
        Task<List<ProductoFamilia>> GetFamiliasByIdEstado(int idEstado);
    }

    public class ProductoRepository : IProductoRepository
    {
        private readonly DapperContext ctx;
        public ProductoRepository(DapperContext context)
        {
            ctx = context;
        }
        public async Task ActualizarEquipoPuesto(MaterialPuesto equi)
        {
            var query = @"update tb_equipo_puesto set clave = @ClaveProducto,
					        id_puesto = @IdPuesto, id_frecuencia = @IdFrecuencia, cantidad = @Cantidad
					    where id_equipo_puesto = @IdMaterialPuesto;";
            try
            {
                using var connection = ctx.CreateConnection();
                await connection.ExecuteScalarAsync<int>(query, equi);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task ActualizarHerramientaPuesto(MaterialPuesto her)
        {
            var query = @"update tb_herramienta_puesto set clave = @ClaveProducto,
					        id_puesto = @IdPuesto, id_frecuencia = @IdFrecuencia, cantidad = @Cantidad
					    where id_herramienta_puesto = @IdMaterialPuesto AND id_puesto = @IdPuesto;";
            try
            {
                using var connection = ctx.CreateConnection();
                await connection.ExecuteScalarAsync<int>(query, her);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task ActualizarMaterialPuesto(MaterialPuesto mat)
        {
            var query = @"update tb_material_puesto set clave_producto = @ClaveProducto,
					        id_puesto = @IdPuesto, id_frecuencia = @IdFrecuencia, cantidad = @Cantidad
					    where id_material_puesto = @IdMaterialPuesto;";
            try
            {
                using var connection = ctx.CreateConnection();
                await connection.ExecuteScalarAsync<int>(query, mat);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task ActualizarUniformePuesto(MaterialPuesto uni)
        {
            var query = @"update tb_uniforme_puesto set clave = @ClaveProducto,
					        id_puesto = @IdPuesto, id_frecuencia = @IdFrecuencia, cantidad = @Cantidad
					    where id_uniforme_puesto = @IdMaterialPuesto;";
            try
            {
                using var connection = ctx.CreateConnection();
                await connection.ExecuteScalarAsync<int>(query, uni);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task AgregarEquipoPuesto(MaterialPuesto equi)
        {
            var query = @"insert into tb_equipo_puesto(clave, id_puesto, id_frecuencia, cantidad, id_personal, fecha_alta)
                        values(@ClaveProducto, @IdPuesto, @IdFrecuencia, @Cantidad, @IdPersonal, @FechaAlta);";
            try
            {
                using var connection = ctx.CreateConnection();
                await connection.ExecuteScalarAsync<int>(query, equi);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task AgregarHerramientaPuesto(MaterialPuesto her)
        {
            var query = @"insert into tb_herramienta_puesto(clave, id_puesto, id_frecuencia, cantidad, id_personal, fecha_alta)
                        values(@ClaveProducto, @IdPuesto, @IdFrecuencia, @Cantidad, @IdPersonal, @FechaAlta);";
            try
            {
                using var connection = ctx.CreateConnection();
                await connection.ExecuteScalarAsync<int>(query, her);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task AgregarMaterialPuesto(MaterialPuesto mat)
        {
            var query = @"insert into tb_material_puesto (clave_producto, id_puesto, id_frecuencia, cantidad, id_personal, fecha_alta)
                        values(@ClaveProducto, @IdPuesto, @IdFrecuencia, @Cantidad, @IdPersonal, @FechaAlta);";
            try
            {
                using var connection = ctx.CreateConnection();
                await connection.ExecuteScalarAsync<int>(query, mat);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task AgregarUniformePuesto(MaterialPuesto uni)
        {
            var query = @"insert into tb_uniforme_puesto (id_puesto, clave, cantidad, id_frecuencia, id_personal, fecha_alta)
                        values(@IdPuesto, @ClaveProducto, @Cantidad, @IdFrecuencia, @IdPersonal, @FechaAlta);";
            try
            {
                using var connection = ctx.CreateConnection();
                await connection.ExecuteScalarAsync<int>(query, uni);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> EliminarEquipoPuesto(int id)
        {
            var query = @"delete from tb_equipo_puesto where id_equipo_puesto = @id;";
            bool reg;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, new { id });
                }
                reg = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reg;
        }
        public async Task<bool> EliminarHerramientaPuesto(int id)
        {
            var query = @"delete from tb_herramienta_puesto where id_herramienta_puesto = @id;";
            bool reg;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, new { id });
                }
                reg = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reg;
        }
        public async Task<bool> EliminarMaterialPuesto(int id)
        {
            var query = @"delete from tb_material_puesto where id_material_puesto = @id;";
            bool reg;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, new { id });
                }
                reg = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reg;
        }
        public async Task<bool> EliminarUniformePuesto(int id)
        {
            var query = @"delete from tb_uniforme_puesto where id_uniforme_puesto = @id;";
            bool reg;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, new { id });
                }
                reg = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reg;
        }
        public async Task<bool> VerificarServiciosExistentes(int id)
        {
            var query = @"SELECT COUNT (*)
                        FROM (
                        SELECT id_servicioextra_cotizacion
                        FROM tb_cotiza_servicioextra a
                        INNER JOIN tb_servicioextra b ON b.id_servicioextra = a.id_servicioextra
                        WHERE a.id_servicioextra = @id
                        ) AS CantidadServiciosExistentes";
            bool result;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    int rows = await connection.ExecuteScalarAsync<int>(query, new { id });
                    result = (rows > 0) ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
        public async Task<bool> EliminarServicio(int id)
        {
            var query = @"UPDATE tb_servicioextra SET id_estatus = 2 WHERE id_servicioextra = @id";
            bool reg;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, new { id });
                }
                reg = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reg;
        }
        
        public async Task<bool> VerificarIndustriasExistentes(int id)

        {
            var query = @"SELECT COUNT (*)
                        FROM (
                        SELECT a.id_prospecto
                        FROM tb_prospecto a
                        INNER JOIN tb_industria_tipo b ON b.id_tipoindustria = a.id_tipoindustria
                        WHERE a.id_tipoindustria = @id
                        ) AS CantidadIndustriasExistentes";
            bool result;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    int rows = await connection.ExecuteScalarAsync<int>(query, new { id });
                    result = (rows > 0) ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
        public async Task<bool> EliminarIndustria(int id)
        {
            var query = @"UPDATE tb_industria_tipo SET id_estatus = 2 WHERE id_tipoindustria = @id";
            bool reg;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, new { id });
                }
                reg = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reg;
        }
        public async Task<bool> AgregarServicio(string servicio, int idPersonal)
        {
            var query = @"INSERT INTO tb_servicioextra
(
descripcion,
fecha_alta,
id_personal
)
VALUES 
(
@servicio,
GETDATE(),
@idPersonal
)";
            bool reg;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, new { servicio, idPersonal });
                }
                reg = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reg;
        }
        
        public async Task<bool> AgregarIndustria(string industria, int idPersonal)
        {
            var query = @"INSERT INTO tb_industria_tipo (descripcion,id_estatus,fecha_alta, id_personal) VALUES (@industria,1,GETDATE(), @idPersonal)";
            bool reg;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteScalarAsync<int>(query, new { industria, idPersonal });
                }
                reg = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reg;
        }
        public async Task<IEnumerable<ProductoItem>> ObtenerEquipoPuesto(int id)
        {
            IEnumerable<ProductoItem> ls;
            var query = @"SELECT a.id_equipo_puesto IdMaterialPuesto, a.clave ClaveProducto,
                            b.descripcion Descripcion, 0 Precio, id_frecuencia IdFrecuencia,
						    a.cantidad Cantidad
                        FROM tb_equipo_puesto a
                        JOIN tb_producto b on a.clave = b.clave
                        WHERE a.id_puesto = @id
                        ORDER BY b.descripcion";
            try
            {
                using var connection = ctx.CreateConnection();
                ls = (await connection.QueryAsync<ProductoItem>(query, new { id })).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ls;
        }
        public async Task<IEnumerable<ProductoItem>> ObtenerHerramientaPuesto(int id)
        {
            IEnumerable<ProductoItem> ls;
            var query = @"SELECT a.id_herramienta_puesto IdMaterialPuesto, a.clave ClaveProducto,
                            b.descripcion Descripcion, 0 Precio, id_frecuencia IdFrecuencia,
						    a.cantidad Cantidad
                        FROM tb_herramienta_puesto a
                        JOIN tb_producto b on a.clave = b.clave
                        WHERE a.id_puesto = @id
                        ORDER BY b.descripcion";
            try
            {
                using var connection = ctx.CreateConnection();
                ls = (await connection.QueryAsync<ProductoItem>(query, new { id })).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ls;
        }
        public async Task<IEnumerable<ProductoItem>> ObtenerMaterialPuesto(int id)
        {
            IEnumerable<ProductoItem> ls;
            var query = @"SELECT a.id_material_puesto IdMaterialPuesto, a.clave_producto ClaveProducto,
                            b.descripcion Descripcion, 0 Precio, id_frecuencia IdFrecuencia,
						    a.cantidad Cantidad
                        FROM tb_material_puesto a
                        JOIN tb_producto b on a.clave_producto = b.clave
                        WHERE a.id_puesto = @id
                        ORDER BY b.descripcion";
            try
            {
                using var connection = ctx.CreateConnection();
                ls = (await connection.QueryAsync<ProductoItem>(query, new { id })).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ls;
        }
        public async Task<IEnumerable<ProductoItem>> ObtenerUniformePuesto(int id)
        {
            IEnumerable<ProductoItem> ls;
            var query = @"SELECT a.id_uniforme_puesto IdMaterialPuesto, a.clave ClaveProducto,
                            b.descripcion Descripcion, 0 Precio, id_frecuencia IdFrecuencia,
						    a.cantidad Cantidad
                        FROM tb_uniforme_puesto a
                        JOIN tb_producto b on a.clave = b.clave
                        WHERE a.id_puesto = @id
                        ORDER BY b.descripcion";
            try
            {
                using var connection = ctx.CreateConnection();
                ls = (await connection.QueryAsync<ProductoItem>(query, new { id })).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ls;
        }
        public async Task<MaterialPuesto> ObtenerProductoDefault(int idProducto, int tipo, int idPuesto)
        {
            string query = @"";
            if (tipo == 1)
            {
                query = @"
SELECT
id_material_puesto IdMaterialPuesto,
clave_producto ClaveProducto,
id_puesto IdPuesto,
id_frecuencia IdFrecuencia,
cantidad Cantidad,
fecha_alta FechaAlta,
id_personal IdPersonal
FROM tb_material_puesto
WHERE id_material_puesto = @idProducto AND id_puesto = @idPuesto
";
            }
            if (tipo == 2)
            {
                query = @"
SELECT
id_uniforme_puesto IdMaterialPuesto,
clave ClaveProducto,
id_puesto IdPuesto,
id_frecuencia IdFrecuencia,
cantidad Cantidad,
fecha_alta FechaAlta,
id_personal IdPersonal
FROM tb_uniforme_puesto
WHERE id_uniforme_puesto = @idProducto AND id_puesto = @idPuesto
";
            }
            if (tipo == 3)
            {
                query = @"
SELECT
id_equipo_puesto IdMaterialPuesto,
clave ClaveProducto,
id_puesto IdPuesto,
id_frecuencia IdFrecuencia,
cantidad Cantidad,
fecha_alta FechaAlta,
id_personal IdPersonal
FROM tb_equipo_puesto
WHERE id_equipo_puesto = @idProducto AND id_puesto = @idPuesto
";
            }
            if (tipo == 4)
            {
                query = @"
SELECT
id_herramienta_puesto IdMaterialPuesto,
clave ClaveProducto,
id_puesto IdPuesto,
id_frecuencia IdFrecuencia,
cantidad Cantidad,
fecha_alta FechaAlta,
id_personal IdPersonal
FROM tb_herramienta_puesto
WHERE id_herramienta_puesto = @idProducto AND id_puesto = @idPuesto
";
            }
            var producto = new MaterialPuesto();
            try
            {
                using var connection = ctx.CreateConnection();
                producto = await connection.QueryFirstOrDefaultAsync<MaterialPuesto>(query, new { idProducto, idPuesto });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return producto;
        }
        public async Task<int> CountProductoProveedorByIdEstado(int idEstado, int idFamilia)
        {
            var query = @"
SELECT 
count(a.clave) Rows
FROM tb_productoprecio a
INNER JOIN tb_producto b ON b.clave = a.clave 
INNER JOIN tb_estado c ON c.id_proveedor = a.id_proveedor
WHERE c.id_estado = @idEstado AND
b.id_status = 1 AND
ISNULL(NULLIF(@idFamilia,0), b.id_familia) = b.id_familia
";
            var numrows = 0;
            try
            {
                using var connection = ctx.CreateConnection();
                numrows = await connection.QuerySingleAsync<int>(query, new {idEstado, idFamilia});
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return numrows;
        }
        public async Task<List<ProductoPrecioEstado>> GetProductoProveedorByIdEstado(int idEstado, int pagina, int idFamilia)
        {
            string query = @"
SELECT  *   
FROM (
SELECT 
ROW_NUMBER() OVER ( ORDER BY b.descripcion ) AS RowNum,
a.clave Clave, 
b.id_familia Familia, 
b.descripcion Descripcion, 
a.id_proveedor IdProveedor, 
precio PrecioProveedor, 
b.preciobase PrecioBase 
FROM tb_productoprecio a
INNER JOIN tb_producto b ON b.clave = a.clave 
INNER JOIN tb_estado c ON c.id_proveedor = a.id_proveedor
WHERE c.id_estado = @idEstado AND
b.id_status = 1 AND
ISNULL(NULLIF(@idFamilia,0), b.id_familia) = b.id_familia
) AS Supervisiones
WHERE   RowNum >= ((@pagina - 1) * 40) + 1
AND RowNum <= (@pagina * 40)
ORDER BY RowNum
";
            var productos = new List<ProductoPrecioEstado>();
            try
            {
                using var connection = ctx.CreateConnection();
                productos = (await connection.QueryAsync<ProductoPrecioEstado>(query, new { idEstado, pagina, idFamilia })).ToList();
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return productos;
        }
        public async Task<string> GetProveedorByIdEstado(int idEstado)
        {
            var query = @"
SELECT b.nombre FROM tb_estado a 
INNER JOIN tb_proveedor b ON b.id_proveedor = a.id_proveedor
WHERE a.id_estado = @idEstado";
            string proveedor;
            try
            {
                using var connection = ctx.CreateConnection();
                proveedor = await connection.QuerySingleAsync<string>(query, new { idEstado});

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return proveedor;
        }
        public async Task<int> GetIdProveedorByIdEstado(int idEstado)
        {
            var query = @"
SELECT a.id_proveedor FROM tb_estado a WHERE a.id_estado = @idEstado
";
            int  idProveedor;
            try
            {
                using var connection = ctx.CreateConnection();
                idProveedor = await connection.QueryFirstAsync<int>(query, new { idEstado });

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return idProveedor;
        }
        public async Task<List<ProductoFamilia>> GetFamiliasByIdEstado(int idEstado)
        {
            string query = @"
SELECT DISTINCT 
    b.id_familia AS idFamilia,
    d.descripcion AS Descripcion
FROM 
    tb_productoprecio a
    INNER JOIN tb_producto b ON b.clave = a.clave 
    INNER JOIN tb_estado c ON c.id_proveedor = a.id_proveedor
    INNER JOIN tb_familia d ON d.id_familia = b.id_familia
WHERE 
    c.id_estado = @idEstado
AND b.id_status = 1
ORDER BY 
    d.descripcion
";
            var familias = new List<ProductoFamilia>();
            try
            {
                using var connection = ctx.CreateConnection();
                familias = (await connection.QueryAsync<ProductoFamilia>(query, new { idEstado })).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return familias;
        }
    }
}