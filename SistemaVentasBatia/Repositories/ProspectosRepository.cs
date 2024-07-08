using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Connections;
using System.Collections;
using System.Data.SqlTypes;
using System.Data;

namespace SistemaVentasBatia.Repositories
{
    public interface IProspectosRepository
    {
        //PROSPECTO
        Task InsertarProspecto(Prospecto prospecto);
        Task ActualizarProspecto(Prospecto prospecto);
        Task InactivarProspecto(int registroAEliminar);
        Task<bool> ActivarProspecto(int idProspecto);
        Task<bool> DesactivarProspecto(int idProspecto);
        Task<int> ObtenerIdProspectoPorCotizacion(int idCotizacion);
        Task<int> ContarProspectos(EstatusProspecto idEstatusProspecto, string keywords, int idPersonal, int autorizacion);
        Task<Prospecto> ObtenerProspectoPorId(int idProspecto);
        Task<Prospecto> ObtenerProspectoPorCotizacion(int idCotizacion);
        Task<List<Prospecto>> ObtenerProspectos(int pagina, EstatusProspecto idEstatusProspecto, string keywords, int autorizacion, int idPersonal);
        Task<List<Prospecto>> ObtenerCatalogoProspectos(int autorizacion, int idPersonal);
        Task<List<Prospecto>> ObtenerCoincidenciasProspecto(string nombreComercial, string rfc);
        Task EliminarTotalPolizasByIdProspecto(int idProspecto);

        //DIRECCION
        Task InsertarDireccion(Direccion direccion);
        Task ActualizarDireccion(Direccion direccion);
        Task<Direccion> ObtenerDireccionPorId(int id);
        Task<PuestoDireccionCotizacion> ObtenerPuestoDireccionCotizacionPorId(int id);

        Task<int> GetIdEstadoByEstado(string estado);
        Task<int> GetIdMunucipioByMunicipio(int idEstado, string municipio);
        Task<bool> GetFronteraPorIdMunicipio(int idMunicipio);
        Task<List<Direccion>> ObtenerDireccionesPorProspecto(int idProspecto, int pagina);
        Task<Prospecto> ObtenerDatosProspecto(int idProspecto);
    }

    public class ProspectosRepository : IProspectosRepository
    {
        private readonly DapperContext ctx;
        public ProspectosRepository(DapperContext ctx)
        {
            this.ctx = ctx;
        }
        public async Task InsertarProspecto(Prospecto prospecto)
        {
            var query = @"insert into tb_prospecto (nombre_comercial, razon_social, rfc, domicilio_fiscal, telefono, representante_legal, documentacion,
                            id_estatus_prospecto, fecha_alta, id_personal, nombre_contacto, email_contacto, numero_contacto, ext_contacto, id_tipoindustria)
                        values(@NombreComercial, @RazonSocial, @Rfc, @DomicilioFiscal, @Telefono, @RepresentanteLegal, @Documentacion, 
                            @IdEstatusProspecto, @FechaAlta, @IdPersonal, @NombreContacto, @EmailContacto, @NumeroContacto, @ExtContacto, @IdTipoIndustria)
                          select scope_identity()";
            //,poder_representante_legal, acta_constitutiva, registro_patronal, empresa_venta
            //,@PoderRepresentanteLegal, @ActaConstitutiva, @RegistroPatronal, @EmpresaVenta
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    prospecto.IdProspecto = await connection.ExecuteScalarAsync<int>(query, prospecto);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> ContarProspectos(EstatusProspecto idEstatusProspecto, string keywords, int idPersonal, int autorizacion)
        {
            var queryuser = @"SELECT count(id_prospecto) Rows 
                        FROM tb_prospecto
                        WHERE
                            id_personal = @idPersonal AND
                            ISNULL(NULLIF(@idEstatusProspecto,0), id_estatus_prospecto) = id_estatus_prospecto
                            AND nombre_comercial like '%' + @keywords + '%';";
            var queryadmin = @"SELECT count(id_prospecto) Rows 
                        FROM tb_prospecto
                        WHERE
                            ISNULL(NULLIF(@idEstatusProspecto,0), id_estatus_prospecto) = id_estatus_prospecto
                            AND nombre_comercial like '%' + @keywords + '%';";

            var numrows = 0;

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    if (autorizacion == 0)
                    {
                        numrows = await connection.QuerySingleAsync<int>(queryuser, new { idEstatusProspecto, keywords = keywords ?? "", idPersonal });
                    }
                    else
                    {
                        numrows = await connection.QuerySingleAsync<int>(queryadmin, new { idEstatusProspecto, keywords = keywords ?? "" });
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return numrows;
        }
        public async Task<List<Prospecto>> ObtenerProspectos(int pagina, EstatusProspecto idEstatusProspecto, string keywords, int autorizacion, int idPersonal)
        {
            var queryadmin = @"SELECT ROW_NUMBER() OVER ( ORDER BY id_prospecto desc ) AS RowNum, id_prospecto IdProspecto, nombre_comercial NombreComercial , razon_social RazonSocial, rfc Rfc, 
				                domicilio_fiscal DomicilioFiscal, telefono Telefono,numero_contacto NumeroContacto, p.Per_Nombre + ' ' + p.Per_Paterno +' ' + p.Per_Materno RepresentanteLegal , documentacion Documentacion, 
				                id_estatus_prospecto IdEstatusProspecto, tb_prospecto.fecha_alta FechaAlta, id_personal IdPersonal
                        FROM tb_prospecto
						INNER JOIN dbo.Personal p on tb_prospecto.id_personal = p.IdPersonal
                        WHERE
                            ISNULL(NULLIF(@idEstatusProspecto,0), id_estatus_prospecto) = id_estatus_prospecto AND
                            nombre_comercial like '%' + @keywords + '%' 
                        ORDER BY nombre_comercial
                        OFFSET ((@pagina - 1) * 40) ROWS
                        FETCH NEXT 40 ROWS ONLY;";
            var queryuser = @"SELECT ROW_NUMBER() OVER ( ORDER BY id_prospecto desc ) AS RowNum, id_prospecto IdProspecto, nombre_comercial NombreComercial , razon_social RazonSocial, rfc Rfc, 
				                domicilio_fiscal DomicilioFiscal, telefono Telefono,numero_contacto NumeroContacto, p.Per_Nombre + ' ' + p.Per_Paterno +' ' + p.Per_Materno RepresentanteLegal , documentacion Documentacion, 
				                id_estatus_prospecto IdEstatusProspecto, tb_prospecto.fecha_alta FechaAlta, id_personal IdPersonal
                        FROM tb_prospecto
						INNER JOIN dbo.Personal p on tb_prospecto.id_personal = p.IdPersonal
                        WHERE
                            ISNULL(NULLIF(@idEstatusProspecto,0), id_estatus_prospecto) = id_estatus_prospecto AND
                            nombre_comercial like '%' + @keywords + '%'  AND
                            tb_prospecto.id_personal = @idPersonal
                        ORDER BY nombre_comercial
                        OFFSET ((@pagina - 1) * 40) ROWS
                        FETCH NEXT 40 ROWS ONLY;";
            var prospectos = new List<Prospecto>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    if (autorizacion == 1)
                    {
                        prospectos = (await connection.QueryAsync<Prospecto>(queryadmin, new { pagina, idEstatusProspecto, keywords = keywords ?? "" })).ToList();

                    }
                    else if (autorizacion == 0)
                    {
                        prospectos = (await connection.QueryAsync<Prospecto>(queryuser, new { pagina, idEstatusProspecto, keywords = keywords ?? "", idPersonal })).ToList();

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return prospectos;
        }
        public async Task<Prospecto> ObtenerProspectoPorId(int idProspecto)
        {
            var query = @"SELECT id_prospecto IdProspecto, nombre_comercial NombreComercial , razon_social RazonSocial, rfc Rfc, 
				                           domicilio_fiscal DomicilioFiscal, telefono Telefono, representante_legal RepresentanteLegal , documentacion Documentacion, 
				                           id_estatus_prospecto IdEstatusProspecto, fecha_alta FechaAlta, id_personal IdPersonal, 
                                           nombre_contacto NombreContacto, numero_contacto NumeroContacto, ext_contacto ExtContacto, email_contacto EmailContacto, id_tipoindustria IdTipoIndustria
                                           
                          FROM tb_prospecto
                          WHERE id_prospecto = @idProspecto";
            //,poder_representante_legal PoderRepresentanteLegal, acta_constitutiva ActaConstitutiva, registro_patronal RegistroPatronal, empresa_venta EmpresaVenta
            var prospecto = new Prospecto();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    prospecto = await connection.QueryFirstAsync<Prospecto>(query, new { idProspecto });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return prospecto;
        }
        public async Task ActualizarProspecto(Prospecto prospecto)
        {
            var query = @"update tb_prospecto set nombre_comercial = @NombreComercial, razon_social = @RazonSocial, rfc = @Rfc, domicilio_fiscal = @DomicilioFiscal, telefono = @Telefono, representante_legal = @RepresentanteLegal, documentacion = @Documentacion, 
                            nombre_contacto = @NombreContacto, email_contacto = @EmailContacto, numero_contacto = @NumeroContacto,ext_contacto = @ExtContacto, id_tipoindustria = @IdTipoIndustria
	                      where id_prospecto = @IdProspecto";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, prospecto);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Direccion>> ObtenerDireccionesPorProspecto(int idProspecto, int pagina)
        {
            var query = @"SELECT ROW_NUMBER() OVER ( ORDER BY id_direccion desc ) AS RowNum, id_direccion IdDireccion, nombre_sucursal NombreSucursal, id_tipo_inmueble IdTipoInmueble,
                                d.id_estado IdEstado, d.id_tabulador as IdTabulador, municipio Municipio, ciudad Ciudad, colonia Colonia, domicilio Domicilio, referencia Referencia, codigo_postal CodigoPostal,
                                contacto Contacto, telefono_contacto TelefonoContacto, id_estatus_direccion IdEstatusDireccion, fecha_alta FechaAlta, e.descripcion Estado, ti.descripcion TipoInmueble
                        FROM tb_direccion d
                        JOIN tb_estado e on d.id_estado = e.id_estado
                        JOIN tb_tipoinmueble ti on d.id_tipo_inmueble = ti.id_tipoinmueble
                        WHERE id_prospecto = @idProspecto and id_estatus_direccion = @idEstatusDireccion
                        ORDER BY id_direccion
                        OFFSET ((@pagina - 1) * 10) ROWS
                        FETCH NEXT 10 ROWS ONLY;";

            var direcciones = new List<Direccion>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direcciones = (await connection.QueryAsync<Direccion>(query, new { idProspecto, pagina, idEstatusDireccion = (int)EstatusDireccion.Activo })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return direcciones;
        }
        public async Task InsertarDireccion(Direccion direccion)
        {
            direccion.IdTabulador = 1;
            var query = @"insert into tb_direccion (id_prospecto, nombre_sucursal, id_tipo_inmueble, id_estado, id_tabulador, municipio, ciudad, colonia,
                            domicilio, referencia, codigo_postal, contacto, telefono_contacto, id_estatus_direccion, fecha_alta,frontera)
                        values(@IdProspecto, @NombreSucursal, @IdTipoInmueble, @IdEstado, @IdTabulador, @Municipio, @Ciudad, @Colonia, 
                            @Domicilio, @Referencia, @CodigoPostal, @Contacto, @TelefonoContacto, @IdEstatusDireccion, @FechaAlta, @Frontera)
                        select scope_identity()";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direccion.IdDireccion = await connection.ExecuteScalarAsync<int>(query, direccion);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Prospecto>> ObtenerCatalogoProspectos(int autorizacion, int idPersonal)
        {
            var queryadmin = @"SELECT id_prospecto IdProspecto, nombre_comercial NombreComercial
                          FROM tb_prospecto WHERE id_estatus_prospecto IN (1,4) ORDER BY nombre_comercial";
            var queryuser = @"SELECT id_prospecto IdProspecto, nombre_comercial NombreComercial
                          FROM tb_prospecto WHERE id_estatus_prospecto IN (1,4) AND id_personal = @idPersonal ORDER BY nombre_comercial";

            var prospectos = new List<Prospecto>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    if (autorizacion == 1)
                    {
                        prospectos = (await connection.QueryAsync<Prospecto>(queryadmin)).ToList();
                    }
                    else if (autorizacion == 0)
                    {
                        prospectos = (await connection.QueryAsync<Prospecto>(queryuser, new { idPersonal })).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return prospectos;
        }
        public async Task<int> ObtenerIdProspectoPorCotizacion(int idCotizacion)
        {
            var query = @"SELECT id_prospecto IdProspecto
                          FROM tb_cotizacion
                          WHERE id_cotizacion = @idCotizacion";

            var idProspecto = 0;

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    idProspecto = await connection.ExecuteScalarAsync<int>(query, new { idCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return idProspecto;
        }
        public async Task<Prospecto> ObtenerProspectoPorCotizacion(int idCotizacion)
        {
            var query = @"DECLARE @idProspecto int = (select id_prospecto from tb_cotizacion where id_cotizacion = @idCotizacion) 
                          SELECT id_prospecto IdProspecto, nombre_comercial NombreComercial , razon_social RazonSocial, rfc Rfc, 
				                           domicilio_fiscal DomicilioFiscal, telefono Telefono, representante_legal RepresentanteLegal , documentacion Documentacion, 
				                           id_estatus_prospecto IdEstatusProspecto, fecha_alta FechaAlta, id_personal IdPersonal, 
                                           nombre_contacto NombreContacto, numero_contacto NumeroContacto, ext_contacto ExtContacto, email_contacto EmailContacto
                          FROM tb_prospecto
                          WHERE id_prospecto = @idProspecto";

            var prospecto = new Prospecto();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    prospecto = await connection.QueryFirstAsync<Prospecto>(query, new { idCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return prospecto;
        }
        public async Task InactivarProspecto(int registroAEliminar)
        {
            var query = @"UPDATE tb_prospecto set id_estatus_prospecto = @idEstatusProspecto where id_prospecto = @registroAEliminar 
                          UPDATE tb_cotizacion set id_estatus_cotizacion = @idEstatusCotizacion where id_prospecto = @registroAEliminar
                          UPDATE tb_direccion set id_estatus_direccion = @idEstatusDireccion where id_prospecto = @registroAEliminar";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new { idEstatusProspecto = EstatusProspecto.Inactivo, idEstatusCotizacion = EstatusCotizacion.Inactivo, idEstatusDireccion = EstatusDireccion.Inactivo, registroAEliminar });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Prospecto>> ObtenerCoincidenciasProspecto(string nombreComercial, string rfc)
        {
            var query = $@"SELECT id_prospecto IdProspecto, nombre_comercial NombreComercial, rfc Rfc, fecha_alta FechaAlta 
                          FROM tb_prospecto                         
                            {(string.IsNullOrEmpty(nombreComercial) ? "" : $" WHERE nombre_comercial like '%' + '{nombreComercial}' + '%'")}
                            {(string.IsNullOrEmpty(rfc) ? "" : $" WHERE rfc = '{rfc}'")}   
                          ORDER BY id_prospecto desc";

            var coincidencias = new List<Prospecto>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    coincidencias = (await connection.QueryAsync<Prospecto>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return coincidencias;
        }
        public async Task<Direccion> ObtenerDireccionPorId(int id)
        {
            var query = $@"SELECT id_direccion IdDireccion, nombre_sucursal NombreSucursal, id_tipo_inmueble IdTipoInmueble,
                                  d.id_estado IdEstado, d.id_tabulador IdTabulador, municipio Municipio, ciudad Ciudad, colonia Colonia, domicilio Domicilio, referencia Referencia, codigo_postal CodigoPostal,
                                  contacto Contacto, telefono_contacto TelefonoContacto, id_estatus_direccion IdEstatusDireccion, fecha_alta FechaAlta
                           FROM tb_direccion  d
                           WHERE id_direccion = @id
                                ";

            var direccion = new Direccion();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direccion = await connection.QueryFirstAsync<Direccion>(query, new { id });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return direccion;
        }
        public async Task ActualizarDireccion(Direccion direccion)
        {
            var query = @"update tb_direccion 
	                      set nombre_sucursal = @NombreSucursal, id_tipo_inmueble = @IdTipoInmueble, id_estado = @IdEstado, municipio = @Municipio, ciudad = @Ciudad, 
		                      colonia = @Colonia, domicilio = @Domicilio, codigo_postal = @CodigoPostal, id_tabulador = @IdTabulador, frontera = @Frontera
	                      where id_direccion = @IdDireccion";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, direccion);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<PuestoDireccionCotizacion> ObtenerPuestoDireccionCotizacionPorId(int id)
        {
            var query = $@"SELECT id_puesto_direccioncotizacion IdPuestoDireccionCotizacion, id_puesto IdPuesto, id_direccion_cotizacion IdDireccionCotizacion,
                            jornada Jornada,j.descripcion JornadaDesc, id_turno IdTurno, cantidad Cantidad, hr_inicio HrInicio, hr_fin HrFin, dia_inicio DiaInicio, dia_Fin DiaFin,
                            fecha_alta FechaAlta, sueldo Sueldo , id_tabulador IdTabulador, id_clase IdClase, dia_festivo DiaFestivo, festivo Festivo, bonos Bonos, vales Vales, dia_domingo DiaDomingo, domingo Domingo,
                            dia_cubredescanso DiaCubreDescanso, cubredescanso CubreDescanso, hr_inicio_fin HrInicioFin, hr_fin_fin HrFinFin, dia_inicio_fin DiaInicioFin, dia_fin_fin DiaFinFin, dia_descanso DiaDescanso,incluyematerial IncluyeMaterial
                        FROM tb_puesto_direccion_cotizacion
						INNER JOIN tb_jornada j ON jornada = j.id_jornada
                        WHERE id_puesto_direccioncotizacion = @id";

            var puestoDireccionCotizacion = new PuestoDireccionCotizacion();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    puestoDireccionCotizacion = await connection.QueryFirstAsync<PuestoDireccionCotizacion>(query, new { id });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return puestoDireccionCotizacion;
        }
        public async Task<bool> ActivarProspecto(int idProspecto)
        {
            string query = @"
UPDATE tb_prospecto
SET id_estatus_prospecto = 1
WHERE id_prospecto = @idProspecto
";
            bool result;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    result = await connection.ExecuteScalarAsync<bool>(query, new { idProspecto });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public async Task<bool> DesactivarProspecto(int idProspecto)
        {
            string query = @"
UPDATE tb_prospecto
SET id_estatus_prospecto = 2
WHERE id_prospecto = @idProspecto
";
            bool result;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    result = await connection.ExecuteScalarAsync<bool>(query, new { idProspecto });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public async Task<int> GetIdEstadoByEstado(string estado)
        {
            if (estado == "MEXICO")
            {
                return 11;
            }
            else
            {
                var query = @"
SELECT id_estado FROM tb_estado WHERE descripcion COLLATE Latin1_General_CI_AI LIKE @estado COLLATE Latin1_General_CI_AI
";
                int idEstado;
                try
                {
                    using (var connection = ctx.CreateConnection())
                    {
                        idEstado = await connection.ExecuteScalarAsync<int>(query, new { estado = "%" + estado + "%" });
                    }

                }
                catch (Exception ex)
                {
                    throw ex;

                }
                return idEstado;
            }
        }
        public async Task<int> GetIdMunucipioByMunicipio(int idEstado, string municipio)
        {
            string query = @"
SELECT a.id_municipio 
FROM tb_estado_municipio a
INNER JOIN tb_municipio b ON b.Id_Municipio = a.id_municipio
WHERE a.id_estado = @idEstado AND b.Municipio COLLATE Latin1_General_CI_AI LIKE @municipio COLLATE Latin1_General_CI_AI

";
            int idMunicipio;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    idMunicipio = await connection.ExecuteScalarAsync<int>(query, new { idEstado, municipio = "%" + municipio + "%" });
                }

            }
            catch (Exception ex)
            {
                throw ex;

            }
            return idMunicipio;
        }
        public async Task<bool> GetFronteraPorIdMunicipio(int idMunicipio)
        {
            string query = @"
SELECT frontera FROM tb_estado_municipio WHERE id_municipio = @idMunicipio
";
            bool result;
            try
            {
                using (var connection = ctx.CreateConnection())
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
        public async Task EliminarTotalPolizasByIdProspecto(int idProspecto)
        {
            string query = @"
UPDATE tb_cotizacion SET total_poliza = 0 WHERE id_prospecto = @idProspecto";
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new { idProspecto });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<Prospecto> ObtenerDatosProspecto(int idProspecto)
        {
            string query = "SELECT nombre_comercial NombreComercial, razon_social RazonSocial, rfc Rfc FROM tb_prospecto WHERE id_prospecto = @idProspecto";
            var prospecto = new Prospecto();
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    prospecto = await connection.QueryFirstAsync<Prospecto>(query, new { idProspecto });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return prospecto;
        }
    }
}

