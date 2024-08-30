using Dapper;
using Microsoft.AspNetCore.Connections;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario> Login(Acceso acceso);
        Task<bool> InsertarUsuario(UsuarioRegistro usuario);
        Task<bool> ActualizarUsuario(AgregarUsuario usuario);
        Task<bool> AgregarUsuario(AgregarUsuario usuario);
        Task<int> ConsultarUsuario(AgregarUsuario usuario);
        Task<bool> ConsultarUsuario(int idPersonal, string Nombres);
        Task<bool> ActivarUsuario(int idPersonal);
        Task<bool> DesactivarUsuario(int idPersonal);
        Task<bool> EliminarUsuario(int idPersonal);
        Task<List<UsuarioGrafica>> ObtenerCotizacionesUsuarios();
        Task<List<UsuarioGraficaMensual>> ObtenerCotizacionesMensuales();
        Task<List<AgregarUsuario>> ObtenerUsuarios();
        Task<List<AgregarUsuario>> ObtenerVendedores();
        Task<AgregarUsuario> ObtenerUsuarioPorIdPersonal(int idPersonal);
    }

    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DapperContext _ctx;
        public UsuarioRepository(DapperContext context)
        {
            _ctx = context;
        }
        public async Task<Usuario> Login(Acceso acceso)
        {
            Usuario usu;
            string query = @"SELECT 
per_usuario Identificador, 
per_nombre Nombre, 
idpersonal as IdPersonal,
per_interno as IdInterno, 
per_estatusventas Estatus, 
id_empleado as IdEmpleado,
per_autorizaventas IdAutoriza,
per_revisaventas IdSupervisa
FROM personal 
where 
per_usuario = @Usuario and 
per_password = @Contrasena AND 
per_cotizadorventas = 1 AND
Per_Interno = 0";

            using (var connection = _ctx.CreateConnection())
            {
                usu = (await connection.QueryFirstOrDefaultAsync<Usuario>(query, acceso));
            }
            return usu;
        }
        public async Task<bool> InsertarUsuario(UsuarioRegistro usuario)
        {
            var base64Data = usuario.Firma;

            if (base64Data.StartsWith("data:image/jpeg;base64,"))
            {
                base64Data = base64Data.Substring("data:image/jpeg;base64,".Length);
            }
            if (base64Data.StartsWith("data:image/png;base64,"))
            {
                base64Data = base64Data.Substring("data:image/png;base64,".Length);
            }
            usuario.Firma = base64Data;

            var query = @"
INSERT INTO  tb_autorizacion_ventas 
(
IdPersonal,
per_autoriza,
per_nombre,
per_puesto,
per_telefono,
per_telefono_extension,
per_telefonomovil,
per_email,
per_firma,
per_revisa
)
VALUES
(
@IdPersonal,
@Autoriza,
@Nombres + ' ' + @Apellidos,
@Puesto,
@Telefono,
@TelefonoExtension,
@TelefonoMovil,
@Email,
@Firma,
@Revisa
)
";
            bool result = false;
            int rowsAffected = 0;
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    rowsAffected = await connection.ExecuteAsync(query, usuario);
                    if (rowsAffected > 0)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public async Task<bool> ActualizarUsuario(AgregarUsuario usuario)
        {
            string query = @"
UPDATE Personal
SET
per_telefono = @Telefono,
per_telefonoextension = @TelefonoExtension,
per_telefonomovil = @TelefonoMovil,
per_firma = @Firma,
per_autorizaventas = @AutorizaVentas,
per_revisaventas = @RevisaVentas
WHERE IdPersonal = @IdPersonal
";
            bool result;
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    int rowsaffected = await connection.ExecuteAsync(query, usuario);
                    result = rowsaffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public async Task<bool> AgregarUsuario(AgregarUsuario usuario)
        {
            string query = @"
UPDATE Personal
SET
per_telefono = @Telefono,
per_telefonoextension = @TelefonoExtension,
per_telefonomovil = @TelefonoMovil,
per_firma = @Firma,
per_autorizaventas = @AutorizaVentas,
per_estatusventas = 1,
per_cotizadorventas = 1,
per_revisaventas = @RevisaVentas
WHERE IdPersonal = @IdPersonal
";
            bool result;
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    int rowsaffected = await connection.ExecuteAsync(query, usuario);
                    result = rowsaffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public async Task<int> ConsultarUsuario(AgregarUsuario usuario)
        {
            if (string.IsNullOrEmpty(usuario.Nombres) && string.IsNullOrEmpty(usuario.ApellidoPaterno) && string.IsNullOrEmpty(usuario.ApellidoMaterno))
            {
                int idPersonal = 0;
                return idPersonal;
            }
            else
            {
                string nombres = "%" + usuario.Nombres + "%";
                string apellidoPaterno = "%" + usuario.ApellidoPaterno + "%";
                string apellidoMaterno = "%" + usuario.ApellidoMaterno + "%";
                string query = @"
SELECT
IdPersonal
FROM Personal WHERE Per_Nombre LIKE @nombres 
AND Per_Paterno LIKE @apellidoPaterno 
AND Per_Materno LIKE @apellidoMaterno
";
                int idPersonal;
                using (var connection = _ctx.CreateConnection())
                {
                    idPersonal = await connection.QueryFirstOrDefaultAsync<int>(query, new { nombres, apellidoPaterno, apellidoMaterno });
                }
                return idPersonal;
            }
        }
        public async Task<bool> ConsultarUsuario(int idPersonal, string Nombres)
        {
            var query = @"
SELECT CASE
           WHEN COUNT(*) > 0 THEN 'true'
           ELSE 'false'
       END AS Resultado
FROM Personal
WHERE IdPersonal = @idPersonal AND Per_Nombre LIKE @Nombres;
";
            bool result = false;
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    result = await connection.QueryFirstOrDefaultAsync<bool>(query, new { idPersonal, Nombres = "%" + Nombres + "%" });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public async Task<bool> ActivarUsuario(int idPersonal)
        {
            string query = @"
UPDATE Personal 
SET
per_estatusventas = 1
WHERE IdPersonal = @idPersonal
";
            bool result;
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    int rowsaffected = await connection.ExecuteAsync(query, new { idPersonal });
                    result = rowsaffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public async Task<bool> DesactivarUsuario(int idPersonal)
        {
            string query = @"
UPDATE Personal 
SET
per_estatusventas = 0
WHERE IdPersonal = @idPersonal
";
            bool result;
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    int rowsaffected = await connection.ExecuteAsync(query, new { idPersonal });
                    result = rowsaffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public async Task<bool> EliminarUsuario(int idPersonal)
        {
            string query = @"
UPDATE Personal
SET
per_autorizaventas = 0,
per_estatusventas = 0,
per_cotizadorventas = 0,
per_revisaventas = 0
WHERE IdPersonal = @IdPersonal
";
            bool result;
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    int rowsaffected = await connection.ExecuteAsync(query, new { idPersonal });
                    result = rowsaffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public async Task<List<UsuarioGrafica>> ObtenerCotizacionesUsuarios()
        {
            var query = @"
SELECT
p.Per_Nombre + ' ' + p.Per_Paterno AS Nombre,
p.IdPersonal AS IdPersonal,
(SELECT COUNT(id_personal) FROM tb_cotizacion c WHERE c.id_personal = p.IdPersonal) AS Cotizaciones,
(SELECT COUNT(id_personal) FROM tb_prospecto pros WHERE pros.id_personal = p.IdPersonal ) AS Prospectos
FROM Personal p
WHERE p.per_cotizadorventas = 1 AND  p.per_revisaventas = 0
";
            var usuarios = new List<UsuarioGrafica>();
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    usuarios = (await connection.QueryAsync<UsuarioGrafica>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return usuarios;
        }
        public async Task<List<UsuarioGraficaMensual>> ObtenerCotizacionesMensuales()
        {
            var query = @"
WITH MesesDeReferencia AS (
    SELECT 1 AS Mes
    UNION SELECT 2
    UNION SELECT 3
    UNION SELECT 4
    UNION SELECT 5
    UNION SELECT 6
    UNION SELECT 7
    UNION SELECT 8
    UNION SELECT 9
    UNION SELECT 10
    UNION SELECT 11
    UNION SELECT 12
)
SELECT
    p.Per_Nombre +' '+ p.Per_Paterno AS Nombre,
    p.IdPersonal AS IdPersonal,
    m.Mes AS Mes,
    COALESCE(SUM(CASE WHEN MONTH(c.fecha_alta) = m.Mes THEN 1 ELSE 0 END), 0) AS CotizacionesPorMes
FROM Personal p
CROSS JOIN
    MesesDeReferencia m
LEFT JOIN
    tb_cotizacion c ON p.IdPersonal = c.id_personal
        AND MONTH(c.fecha_alta) = m.Mes
WHERE
    p.per_revisaventas = 0 AND p.per_cotizadorventas = 1
GROUP BY
    p.IdPersonal,
    p.Per_Nombre,
    p.Per_Paterno,
    m.Mes
ORDER BY
    p.IdPersonal,
    Mes;
";
            var usuarios = new List<UsuarioGraficaMensual>();
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    usuarios = (await connection.QueryAsync<UsuarioGraficaMensual>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return usuarios;
        }
        public async Task<List<AgregarUsuario>> ObtenerUsuarios()
        {
            string query = @"
SELECT 
	IdPersonal IdPersonal,
	CAST(per_autorizaventas AS BIT) AS AutorizaVentas,
    CAST(per_estatusventas AS BIT) AS EstatusVentas,
    CAST(per_cotizadorventas AS BIT) AS CotizadorVentas,
    CAST(per_revisaventas AS BIT) AS RevisaVentas,
	Per_Nombre Nombres,
	Per_Paterno ApellidoPaterno,
	Per_Materno ApellidoMaterno,
	per_Puesto Puesto,
	per_telefono Telefono,
	per_telefonoextension TelefonoExtension,
	per_telefonomovil TelefonoMovil,
	per_Email Email,
	per_firma Firma,
	per_usuario Usuario,
	per_password Password
	FROM Personal WHERE per_cotizadorventas = 1 ORDER BY Per_Nombre
";
            var usuarios = new List<AgregarUsuario>();
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    usuarios = (await connection.QueryAsync<AgregarUsuario>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return usuarios;
        }
        public async Task<List<AgregarUsuario>> ObtenerVendedores()
        {
            string query = @"
SELECT 
	IdPersonal IdPersonal,
	CAST(per_autorizaventas AS BIT) AS AutorizaVentas,
    CAST(per_estatusventas AS BIT) AS EstatusVentas,
    CAST(per_cotizadorventas AS BIT) AS CotizadorVentas,
    CAST(per_revisaventas AS BIT) AS RevisaVentas,
	Per_Nombre Nombres,
	Per_Paterno ApellidoPaterno,
	Per_Materno ApellidoMaterno,
	per_Puesto Puesto,
	per_telefono Telefono,
	per_telefonoextension TelefonoExtension,
	per_telefonomovil TelefonoMovil,
	per_Email Email,
	per_firma Firma,
	per_usuario Usuario,
	per_password Password
	FROM Personal WHERE per_cotizadorventas = 1 AND per_revisaventas = 0 ORDER BY Per_Nombre
";
            var usuarios = new List<AgregarUsuario>();
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    usuarios = (await connection.QueryAsync<AgregarUsuario>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return usuarios;
        }
        public async Task<AgregarUsuario> ObtenerUsuarioPorIdPersonal(int idPersonal)
        {
            var query = @"
SELECT 
	IdPersonal IdPersonal,
	per_autorizaventas AutorizaVentas,
	per_estatusventas EstatusVentas,
	per_cotizadorventas CotizadorVentas,
    per_revisaventas RevisaVentas,
	Per_Nombre Nombres,
	Per_Paterno ApellidoPaterno,
	Per_Materno ApellidoMaterno,
	per_Puesto Puesto,
	per_telefono Telefono,
	per_telefonoextension TelefonoExtension,
	per_telefonomovil TelefonoMovil,
	per_Email Email,
	per_firma Firma
	FROM Personal
	WHERE IdPersonal = @idPersonal
";
            var usuario = new AgregarUsuario();
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    usuario = await connection.QueryFirstAsync<AgregarUsuario>(query, new { idPersonal });
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return usuario;
        }
    }
}