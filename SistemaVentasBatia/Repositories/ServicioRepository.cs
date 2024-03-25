using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace SistemaVentasBatia.Repositories
{
    public interface IServicioRepository
    {
        Task<List<ServicioCotizacion>> ObtenerListaServiciosCotizacion(int idCotizacion, int direccioncotizacion);
    }

    public class ServicioRepository : IServicioRepository
    {
        private readonly DapperContext _ctx;
        public ServicioRepository(DapperContext context)
        {
            _ctx = context;
        }
        public async Task<List<ServicioCotizacion>> ObtenerListaServiciosCotizacion(int idCotizacion, int direccioncotizacion)
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
                    servicioscotizacion = (await connection.QueryAsync<ServicioCotizacion>(query, new{ idCotizacion})).ToList();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return servicioscotizacion;
        }
    }
}
