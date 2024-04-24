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

namespace SistemaVentasBatia.Repositories
{
    public interface IReportRepository
    {
        Task<Prospecto> ObtenerProspecto(int idCotizacion);
        Task<decimal> ObtenerTotalCotizacion(int idCotizacion);
        Task<Empresa> ObtenerEmpresa(int idEmpresa);
        Task<string> ObtenerEstadoByIdEstado(int idEstado);
        Task<string> ObtenerMunicipioByIdMunicipio(int idMunicipio);
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
    }
}
