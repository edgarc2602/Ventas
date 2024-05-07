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
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Enums;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Net;
using System.Net.NetworkInformation;

namespace SistemaVentasBatia.Repositories
{
    public interface IReportRepository
    {
        Task<Prospecto> ObtenerProspecto(int idCotizacion);
        Task<decimal> ObtenerTotalCotizacion(int idCotizacion);
        Task<Empresa> ObtenerEmpresa(int idEmpresa);
        Task<string> ObtenerEstadoByIdEstado(int idEstado);
        Task<string> ObtenerMunicipioByIdMunicipio(int idMunicipio);
        Task<List<Prospecto>> ObtenerListaProspectos(int idEstatus, DateTime Finicio, DateTime Ffin);
        Task<List<Cotizacion>> ObtenerCotizacionesPorIdProspecto(int idEstatus, int idProspecto, DateTime Finicio, DateTime Ffin);
    }

    public class ReportRepository : IReportRepository
    {
        private readonly DapperContext _ctx;
        public ReportRepository(DapperContext context)
        {
            _ctx = context;
        }

        public async Task<Prospecto> ObtenerProspecto(int idCotizacion)
        {
            string query = @"
                DECLARE @idProspecto int = (select id_prospecto from tb_cotizacion where id_cotizacion = @idCotizacion) 
                          SELECT id_prospecto IdProspecto, nombre_comercial NombreComercial , razon_social RazonSocial, rfc Rfc, 
				                           domicilio_fiscal DomicilioFiscal, telefono Telefono, representante_legal RepresentanteLegal , documentacion Documentacion, 
				                           id_estatus_prospecto IdEstatusProspecto, fecha_alta FechaAlta, id_personal IdPersonal, 
                                           nombre_contacto NombreContacto, numero_contacto NumeroContacto, ext_contacto ExtContacto, email_contacto EmailContacto, poliza_cumplimiento PolizaCumplimiento
                          FROM tb_prospecto
                          WHERE id_prospecto = @idProspecto
                ";
            var prospecto = new Prospecto();
            try
            {
                using (var connection = _ctx.CreateConnection())
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
        public async Task<decimal> ObtenerTotalCotizacion(int idCotizacion)
        {
            string query = @"SELECT total FROM tb_cotizacion WHERE id_cotizacion = @idCotizacion";
            decimal total;
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    total = await connection.QueryFirstAsync<decimal>(query, new { idCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return total;
        }
        public async Task<Empresa> ObtenerEmpresa(int idEmpresa)
        {
            string query = @"
                        SELECT
                        id_empresa IdEmpresa,
                        nombre Nombre,
                        tipo Tipo,
                        registro Registro,
                        razonsocial RazonSocial,
                        rfc Rfc,
                        callenum CalleNumero,
                        colonia Colonia,
                        cp CP,
                        municipio Municipio,
                        id_estado IdEstado,
                        id_estatus IdEstatus,
                        Clase IdClase,
                        representante Representante,
                        foliovigente FolioVigente,
                        constitutiva_escriturapublica ConstitutivaEscrituraPublica,
                        constitutiva_fecha ConstitutivaFecha,
                        constitutiva_licenciado ConstitutivaLicenciado,
                        constitutiva_numeronotario ConstitutivaNumeroNotario,
                        constitutiva_foliomercantil ConstitutivaFolioMercantil,
                        constitutiva_idestado ConstitutivaIdEstado,
                        poder_escriturapublica PoderEscrituraPublica,
                        poder_fecha PoderFecha,
                        poder_licenciado PoderLicenciado,
                        poder_numeronotario PoderNumeroNotario,
                        poder_idestado PoderIdEstado
                        FROM tb_empresa WHERE id_empresa = @idEmpresa
                    ";
            var empresa = new Empresa();
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    empresa = await connection.QueryFirstAsync<Empresa>(query, new { idEmpresa });
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return empresa;
        }
        public async Task<string> ObtenerEstadoByIdEstado(int idEstado)
        {
            string query = @"SELECT descripcion FROM tb_estado WHERE id_estado = @idEstado";
            string estadoNombre = "";
            try
            {
                using(var connection = _ctx.CreateConnection())
                {
                    estadoNombre = await connection.QueryFirstAsync<string>(query, new { idEstado });
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return estadoNombre;
        }
        public async Task<string> ObtenerMunicipioByIdMunicipio(int idMunicipio)
        {
            string query = @"SELECT Municipio FROM tb_municipio  WHERE Id_Municipio = @idMunicipio";
            string municipioNombre = "";
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    municipioNombre = await connection.QueryFirstAsync<string>(query, new { idMunicipio });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return municipioNombre;
        }
        public async Task<List<Prospecto>> ObtenerListaProspectos(int idEstatus, DateTime Finicio, DateTime Ffin)
        {
            if(idEstatus == 2)
            {
                idEstatus = 3;
            }
            string query = @"
                 SELECT DISTINCT 
                 b.id_prospecto IdProspecto, 
                 b.nombre_comercial NombreComercial , 
                 b.razon_social RazonSocial, 
                 b.rfc Rfc, 
                 b.domicilio_fiscal DomicilioFiscal,
                 b.telefono Telefono,
                 b.numero_contacto NumeroContacto,
                 p.Per_Nombre + ' ' + p.Per_Paterno RepresentanteLegal , 
                 b.documentacion Documentacion, 
                 b.id_estatus_prospecto IdEstatusProspecto,
                 b.fecha_alta FechaAlta, 
                 b.id_personal IdPersonal,
                 p.Per_Nombre
                 FROM tb_cotizacion a
                 INNER JOIN tb_prospecto b ON a.id_prospecto = b.id_prospecto
                 INNER JOIN dbo.Personal p on b.id_personal = p.IdPersonal
                 WHERE 
                 ISNULL(NULLIF(@idEstatus,0), a.id_estatus_cotizacion) = a.id_estatus_cotizacion AND 
                 a.fecha_alta  BETWEEN @Finicio AND @Ffin
                 ORDER by p.Per_Nombre,b.nombre_comercial
                ";
            var prospectos = new List<Prospecto>();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                        prospectos = (await connection.QueryAsync<Prospecto>(query, new {idEstatus,Finicio, Ffin })).ToList();
                    

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return prospectos;
        }
        public async Task<List<Cotizacion>> ObtenerCotizacionesPorIdProspecto(int idEstatus,int idProspecto, DateTime Finicio, DateTime Ffin)
        {
            if(idEstatus == 2)
            {
                idEstatus = 3;
            }
            string query = @"
                    SELECT 
                    id_cotizacion IdCotizacion, 
                    id_servicio IdServicio, 
                    id_estatus_Cotizacion IdEstatusCotizacion, 
                    c.fecha_alta FechaAlta, 
                    c.id_personal IdPersonal, 
                    c.total Total, 
                    c.nombre Nombre, per.Per_Nombre + ' ' + per.Per_Paterno AS IdAlta, 
                    ISNULL(c.poliza_cumplimiento, 0) AS PolizaCumplimiento, 
                    ISNULL(c.dias_vigencia, 0) AS DiasVigencia
                    FROM tb_cotizacion c
                    JOIN tb_prospecto p on c.id_prospecto = p.id_prospecto
                    INNER JOIN dbo.Personal per ON c.id_personal = per.IdPersonal 
                    WHERE 
                    c.id_prospecto = @idProspecto AND
                    c.fecha_alta BETWEEN @Finicio AND @Ffin AND ";
            if(idEstatus == 0)
            {
                query += "c.id_estatus_cotizacion IN (1,3)";
            }
            else
            {
                query += "c.id_estatus_cotizacion = @idEstatus";
            }
            var cotizacion = new List<Cotizacion>();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    cotizacion = (await connection.QueryAsync<Cotizacion>(query, new { idEstatus, idProspecto, Finicio, Ffin })).ToList();


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
