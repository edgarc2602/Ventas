using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Repositories
{
    public interface ITabuladorRepository
    {
        Task<int> Agregar(Tabulador model);
        Task<bool> Actualizar(Tabulador model);
        Task<bool> Eliminar(int id);
        Task<Tabulador> Obtener(int id);
        Task<IEnumerable<Tabulador>> ObtenerPorEstado(int id);
        Task<PuestoTabulador> ObtenerTabuladorPuesto(int idPuesto, int idClase);
    }

    public class TabuladorRepository : ITabuladorRepository
    {
        private readonly DapperContext ctx;
        public TabuladorRepository(DapperContext ctx)
        {
            this.ctx = ctx;
        }
        public async Task<int> Agregar(Tabulador model)
        {
            int reg = 0;
            var query = @"insert into tb_cotiza_tabulador(nombre, id_estado)
                        values(@Nombre, @IdEstado);
                        select SCOPE_IDENTITY();";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    reg = await connection.ExecuteScalarAsync<int>(query, model);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reg;
        }
        public Task<bool> Actualizar(Tabulador model)
        {
            throw new System.NotImplementedException();
        }
        public Task<bool> Eliminar(int id)
        {
            throw new System.NotImplementedException();
        }
        public Task<Tabulador> Obtener(int id)
        {
            throw new System.NotImplementedException();
        }
        public async Task<IEnumerable<Tabulador>> ObtenerPorEstado(int id)
        {
            IEnumerable<Tabulador> ls;
            var query = @"SELECT id_tiposalario as IdTabulador, descripcion as Nombre
                        FROM tb_tiposalario a";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    ls = (await connection.QueryAsync<Tabulador>(query, new { id })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ls;
        }
        public async Task<PuestoTabulador> ObtenerTabuladorPuesto(int idPuesto, int idClase)
        {
            PuestoTabulador result = new PuestoTabulador();
            var query = @"

SELECT
  ISNULL(MAX(CASE WHEN id_zona = 1 THEN sueldo END), 0) AS Zona1,
  ISNULL(MAX(CASE WHEN id_zona = 2 THEN sueldo END), 0) AS Zona2,
  ISNULL(MAX(CASE WHEN id_zona = 3 THEN sueldo END), 0) AS Zona3,
  ISNULL(MAX(CASE WHEN id_zona = 4 THEN sueldo END), 0) AS Zona4,
  ISNULL(MAX(CASE WHEN id_zona = 5 THEN sueldo END), 0) AS Zona5
FROM tb_sueldozonaclase
WHERE id_puesto = @idPuesto AND id_clase = @idClase

";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    result = await connection.QueryFirstAsync<PuestoTabulador>(query, new { idPuesto, idClase });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
    }
}
