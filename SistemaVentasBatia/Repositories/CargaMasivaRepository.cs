using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using System.ComponentModel;

namespace SistemaVentasBatia.Repositories
{
    public interface ICargaMasivaRepository
    {
        Task<bool> ObtenerFronteraPorIdMunicipio(int idMunicipio);
        bool InsertarDireccionesExcel(List<Direccion> direcciones, int idCotizacion);
        Task<List<int>> ObtenerIdDireccionesInsertadas(int idProspecto, int cantidadDireccionesInsertadas);
        Task InsertarDireccionCotizacion(int idDireccion, int idCotizacion);
        Task<List<Direccion>> ObtenerSucursalesCotizacion(int idCotizacion);
        Task<ResumenCotizacionLimpieza> ObtenerResumenCotizacionLimpieza(int idCotizacion);
        Task<decimal> ObtenerPolizaCotizacion(int idCotizacion);
        Task<Prospecto> ObtenerProspecto(int idCotizacion);
        Task<List<Direccion>> ObtenerDirecciones(int idCotizacion);
        Task<Cotizacion> ObtenerCotizacion(int id);
    }

    public class CargaMasivaRepository : ICargaMasivaRepository
    {
        private readonly DapperContext _ctx;
        public CargaMasivaRepository(DapperContext context)
        {
            _ctx = context;
        }

        public async Task<bool> ObtenerFronteraPorIdMunicipio(int idMunicipio)
        {
            string query = @"
                SELECT frontera FROM tb_estado_municipio WHERE id_municipio = @idMunicipio
                ";
            bool result;
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    result = await connection.ExecuteScalarAsync<bool>(query, new { idMunicipio });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public bool InsertarDireccionesExcel(List<Direccion> direcciones, int idCotizacion)
        {
            List<int> idsGenerados = new List<int>();

            using (var connection = _ctx.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var dataTable = new DataTable();
                        dataTable.Columns.Add("id_prospecto", typeof(int));
                        dataTable.Columns.Add("nombre_sucursal", typeof(string));
                        dataTable.Columns.Add("id_tipo_inmueble", typeof(int));
                        dataTable.Columns.Add("id_estado", typeof(int));
                        dataTable.Columns.Add("id_tabulador", typeof(int));
                        dataTable.Columns.Add("municipio", typeof(string));
                        dataTable.Columns.Add("ciudad", typeof(string));
                        dataTable.Columns.Add("colonia", typeof(string));
                        dataTable.Columns.Add("domicilio", typeof(string));
                        dataTable.Columns.Add("referencia", typeof(string));
                        dataTable.Columns.Add("codigo_postal", typeof(string));
                        dataTable.Columns.Add("contacto", typeof(string));
                        dataTable.Columns.Add("telefono_contacto", typeof(string));
                        dataTable.Columns.Add("id_estatus_direccion", typeof(int));
                        dataTable.Columns.Add("fecha_alta", typeof(DateTime));
                        dataTable.Columns.Add("id_tiposalario", typeof(int));
                        dataTable.Columns.Add("frontera", typeof(bool));

                        foreach (var direccion in direcciones)
                        {
                            dataTable.Rows.Add(
                                direccion.IdProspecto,
                                direccion.NombreSucursal,
                                direccion.IdTipoInmueble,
                                direccion.IdEstado,
                                direccion.IdTabulador,
                                direccion.Municipio,
                                direccion.Ciudad,
                                direccion.Colonia,
                                direccion.Domicilio,
                                direccion.Referencia,
                                direccion.CodigoPostal,
                                direccion.Contacto,
                                direccion.TelefonoContacto,
                                (int)direccion.IdEstatusDireccion,
                                direccion.FechaAlta,
                                0,
                                direccion.Frontera
                            );
                        }
                        using (var bulkCopy = new SqlBulkCopy((SqlConnection)connection, SqlBulkCopyOptions.Default, (SqlTransaction)transaction))
                        {
                            bulkCopy.DestinationTableName = "tb_direccion";
                            foreach (DataColumn column in dataTable.Columns)
                            {
                                // Si la columna no es la columna de identidad, entonces agrega el mapeo
                                if (column.ColumnName != "id_direccion")
                                {
                                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                                }
                            }

                            // Ejecutar la inserción de datos
                            bulkCopy.WriteToServer(dataTable);
                        }

                        // Confirmar la transacción
                        transaction.Commit();

                        // Consultar los IDs generados después de la transacción
                        //using (var cmd = new SqlCommand("SELECT id_direccion FROM tb_direccion WHERE id_cotizacion = @idCotizacion", (SqlConnection)connection))
                        //{
                        //    cmd.Parameters.AddWithValue("@idCotizacion", idCotizacion);

                        //    using (var reader = await cmd.ExecuteReaderAsync())
                        //    {
                        //        while (reader.Read())
                        //        {
                        //            idsGenerados.Add(reader.GetInt32(0));
                        //        }
                        //    }
                        //}
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        // Manejar el error
                        return false;
                    }
                }
            }
        }
        public async Task<List<int>> ObtenerIdDireccionesInsertadas(int idProspecto, int cantidadDireccionesInsertadas)
        {
            string query = @" SELECT TOP (@cantidadDireccionesInsertadas) id_direccion  FROM tb_direccion WHERE id_prospecto = @idProspecto ORDER BY id_direccion DESC";

            var idInsertados = new List<int>();
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    idInsertados = (await connection.QueryAsync<int>(query, new { idProspecto, cantidadDireccionesInsertadas })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return idInsertados;
        }
        public async Task InsertarDireccionCotizacion(int idDireccion, int idCotizacion)
        {
            string query = @"insert into tb_direccion_cotizacion values(@idDireccion, @idCotizacion)";
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                     await connection.ExecuteScalarAsync<int>(query, new {idDireccion, idCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task <List<Direccion>> ObtenerSucursalesCotizacion(int idCotizacion)
        {
            string query = @"
            SELECT b.nombre_sucursal NombreSucursal, a.id_direccion_cotizacion IdDireccionCotizacion FROM tb_direccion_cotizacion a 
            INNER JOIN tb_direccion b ON b.id_direccion = a.id_direccion 
            WHERE a.id_cotizacion = @idCotizacion ORDER BY b.nombre_sucursal
            ";
            var sucursales = new List<Direccion>();
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    sucursales = (await connection.QueryAsync<Direccion>(query, new { idCotizacion })).ToList();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return sucursales;
        }
        public async Task<ResumenCotizacionLimpieza> ObtenerResumenCotizacionLimpieza(int idCotizacion)
        {
            var query = @"SELECT * FROM fn_resumencotizacion(@idCotizacion)";
            var queryserv = @"SELECT ISNULL(SUM(ISNULL(importemensual,0)),0) AS Servicio
                            FROM tb_cotiza_servicioextra
                            WHERE id_cotizacion = @idCotizacion";

            var resumen = new ResumenCotizacionLimpieza();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    resumen = await connection.QueryFirstAsync<ResumenCotizacionLimpieza>(query, new { idCotizacion });

                    resumen.Servicio = await connection.QueryFirstAsync<decimal>(queryserv, new { idCotizacion });

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return resumen;
        }
        public async Task<decimal> ObtenerPolizaCotizacion(int idCotizacion)
        {
            var query = @"SELECT total_poliza FROM tb_cotizacion WHERE id_cotizacion = @idCotizacion";

            object result;
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    result = await connection.QueryFirstOrDefaultAsync<decimal?>(query, new { idCotizacion });
                }

                if (result == null)
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return (decimal)result;
        }
        public async Task<Prospecto> ObtenerProspecto(int idCotizacion)
        {
            string query = @"
            SELECT
            b.id_prospecto IdProspecto,
            b.nombre_comercial NombreComercial,
            b.razon_social RazonSocial,
            b.rfc Rfc,
            b.domicilio_fiscal DomicilioFiscal,
            b.nombre_contacto NombreContacto,
            b.email_contacto EmailContacto,
            b.numero_contacto NumeroContacto,
            b.ext_contacto ExtContacto
            FROM tb_cotizacion a
            INNER JOIN tb_prospecto b ON b.id_prospecto = a.id_prospecto
            WHERE a.id_cotizacion = @idCotizacion";

            var prospecto = new Prospecto();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    prospecto = await connection.QueryFirstAsync<Prospecto>(query, new {idCotizacion});
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return prospecto;
        }
        public async Task<List<Direccion>> ObtenerDirecciones(int idCotizacion)
        {
            string query = @"
                SELECT
                a.id_direccion IdDireccion,
                a.nombre_sucursal NombreSucursal,
                a.id_tipo_inmueble IdTipoInmueble,
                c.descripcion TipoInmueble,
                a.id_estado IdEstado,
                b.descripcion Estado,
                a.id_tabulador IdTabulador,
                a.municipio Municipio,
                a.ciudad Ciudad,
                a.colonia Colonia,
                a.domicilio Domicilio,
                a.codigo_postal CodigoPostal
                FROM tb_direccion a
                INNER JOIN tb_estado b ON b.id_estado = a.id_estado
                INNER JOIN tb_tipoinmueble c ON c.id_tipoinmueble = a.id_tipo_inmueble
                INNER JOIN tb_direccion_cotizacion d ON a.id_direccion = d.id_direccion
                WHERE d.id_cotizacion = @idCotizacion
                ORDER BY a.nombre_sucursal";
            var direcciones = new List<Direccion>();
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    direcciones = (await connection.QueryAsync<Direccion>(query, new { idCotizacion})).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return direcciones;
        }
        public async Task<Cotizacion> ObtenerCotizacion(int id)
        {
            var cotizacion = new Cotizacion();

            var query = @"SELECT id_cotizacion IdCotizacion,
                costo_indirecto CostoIndirecto,
                utilidad Utilidad,
                comision_venta ComisionSV,
                comision_externa ComisionExt
                FROM tb_cotizacion  WHERE id_cotizacion = @id ";
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    cotizacion = await connection.QueryFirstAsync<Cotizacion>(query, new { id });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return cotizacion;
        }
    }
}
