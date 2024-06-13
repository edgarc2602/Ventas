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
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.DTOs;

namespace SistemaVentasBatia.Repositories
{
    public interface IClienteRepository
    {
        Task<ClienteContrato> ObtenerDatosClienteContrato(int idProspecto);
        Task<bool> InsertarDatosClienteContrato(ClienteContrato contrato);
        Task<bool> ActualizarDatosClienteContrato(ClienteContrato contrato);
        Task<bool> ConsultarContratoExistente(int idProspecto);

        Task<decimal> ObtenerTotalCotizacion(int idCotizacion);
        Task<List<Direccion>> ObtenerDireccionesCotizacion(int idCotizacion);
        Task<List<PuestoDireccionCotizacion>> ObtenerPuestosDireccionCotizacion(int idDireccionCotizacion);
        int InsertarClienteXML(string clienteXML, string logXML);
        bool InsertarXMLOficina(string oficinaXML);
        bool InsertarLineaNegocioXML(string lineaNegocioXML);
        int InsertarDireccionXML(string direccionXML, string logXML);
        int InsertarPlantillaXML(string plantillaXML);
        bool InsertarHorarioPlantillaXML(string horarioPlantillaXML);
        bool InsertarPlantillaPXML(int idPlantillaCreada, int cantidad);
        bool InsertarVacantePlantillaXML(int idPlantillaCreada, int idPersonal);
        Task<bool> InsertarMaterialAutorizado(MaterialCotizacion producto, int idPlantilla, int idClienteCreado);
        Task<bool> InsertarMaterialDireccion(MaterialCotizacion producto, int idPlantilla, int idClienteCreado);
        Task<int> ObtenerConceptoPresupuestoPorLineaNegocio(int idServicio);
        bool InsertarPresupuestoMaterialXML(string resupuestoPlantillaXML);
        bool InsertarPresupuestoEquipoHerramientaXML(string resupuestoPlantillaXML);
        bool InsertarEquipoHerramientaXML(string equipoHerramientaXML);
        int InsertarXMLAsuntoLegal(string asuntoLegalXML);
        Task<int> ObtenerIdAsuntoPasoContrato(int idAsuntoCreado);
        bool InsertarContratoClienteXML(string contratoClienteXML);
        Task<bool> ActualizarEstatusAsuntoPaso(int idAsuntoPaso);
        Task<decimal> ObtenerTotalDireccion(int idCotizacion, int idDireccionCotizacion);
        void InsertarIgualasXML(string IgualasXML);
        Task ActualizarPresupuestosSucursal(int idPuntoAtencion, decimal totalMateriales, decimal  totalHigienicos);
        Task<CotizaPorcentajes> ObtenerPorcentajesCotizacion(int idCotizacion);
        Task InsertarSubcontrato(int idClienteCreado, int idServicio, int idFrecuencia, int idServicioExtra, decimal total);
        Task<string> ObtenerDescripcionServicio(int idServicioExtra);
        Task InsertarCargaSocialPuesto(int idPlantillaCreada, decimal cargaSocial, decimal uniforme, decimal bonos, decimal primaDominical, decimal otrasComp);
        Task<bool> ConsultarPoliza(int idCotizacion);
    }

    public class ClienteRepository : IClienteRepository
    {
        private readonly DapperContext ctx;
        public ClienteRepository(DapperContext ctx)
        {
            this.ctx = ctx;
        }
        public async Task<ClienteContrato> ObtenerDatosClienteContrato(int idProspecto)
        {
            var contrato = new ClienteContrato();

            string query = @"
                        SELECT
                        id_clientecontrato IdClienteContrato, id_empresa IdEmpresa, id_prospecto IdProspecto, id_cotizacion IdCotizacion,
                        constitutiva_escriturapublica ConstitutivaEscrituraPublica, constitutiva_fecha ConstitutivaFecha,
                        constitutiva_licenciado ConstitutivaLicenciado, constitutiva_numeronotario ConstitutivaNumeroNotario,
                        constitutiva_foliomercantil ConstitutivaFolioMercantil, poder_escriturapublica PoderEscrituraPublica,
                        poder_fecha PoderFecha, poder_licenciado PoderLicenciado, poder_numeronotario PoderNumeroNotario,
                        cliente_registropatronal ClienteRegistroPatronal, poliza_monto PolizaMonto, poliza_montoletra PolizaMontoLetra, poliza_empresa PolizaEmpresa,
                        contrato_vigencia ContratoVigencia, empresa_contacto_nombre EmpresaContactoNombre, empresa_contacto_correo EmpresaContactoCorreo,
                        empresa_contacto_telefono EmpresaContactoTelefono, cliente_direccion ClienteDireccion, cliente_colonia ClienteColoniaDescripcion,
                        cliente_municipio ClienteMunicipio, cliente_estado ClienteEstado, cliente_cp CP, poliza_numero PolizaNumero, cliente_email ClienteEmail, cliente_representante ClienteRepresentante,
                        cliente_contacto_nombre ClienteContactoNombre, cliente_contacto_telefono ClienteContactoTelefono, constitutiva_idestado ConstitutivaIdEstado, poder_idestado PoderIdEstado
                        FROM tb_cliente_contrato
                        WHERE id_prospecto = @idProspecto";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    contrato = await connection.QueryFirstOrDefaultAsync<ClienteContrato>(query, new { idProspecto });
                    if (contrato == null)
                    {
                        var clientecontrato = new ClienteContrato();
                        return clientecontrato;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener datos del contrato: {ex.Message}");
            }
            return contrato;
        }
        public async Task<bool> InsertarDatosClienteContrato(ClienteContrato contrato)
        {
            string query = @"
                            INSERT INTO tb_cliente_contrato
                            (id_empresa , id_prospecto ,id_cotizacion, constitutiva_escriturapublica , constitutiva_fecha ,
                            constitutiva_licenciado , constitutiva_numeronotario ,constitutiva_foliomercantil , poder_escriturapublica ,
                            poder_fecha, poder_licenciado , poder_numeronotario ,cliente_registropatronal , poliza_monto , 
                            poliza_montoletra , poliza_empresa ,contrato_vigencia, empresa_contacto_nombre , empresa_contacto_correo,
                            empresa_contacto_telefono , cliente_direccion , cliente_colonia ,cliente_municipio, cliente_estado , cliente_cp, poliza_numero,
                            cliente_email, cliente_representante, cliente_contacto_nombre, cliente_contacto_telefono, constitutiva_idestado, poder_idestado)
                            VALUES
                            (
                            @IdEmpresa,  @IdProspecto, @IdCotizacion, @ConstitutivaEscrituraPublica, @ConstitutivaFecha,@ConstitutivaLicenciado, 
                            @ConstitutivaNumeroNotario,@ConstitutivaFolioMercantil, @PoderEscrituraPublica, @PoderFecha,  @PoderLicenciado,  
                            @PoderNumeroNotario, @ClienteRegistroPatronal,  @PolizaMonto,  @PolizaMontoLetra, @PolizaEmpresa,@ContratoVigencia,  @EmpresaContactoNombre,  
                            @EmpresaContactoCorreo,@EmpresaContactoTelefono, @ClienteDireccion, @ClienteColoniaDescripcion,@ClienteMunicipio, @ClienteEstado, @CP,
                            @PolizaNumero, @ClienteEmail, @ClienteRepresentante, @ClienteContactoNombre, @ClienteContactoTelefono, @ConstitutivaIdEstado, @PoderIdEstado)
                            ";
            bool result = false;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    var rowsAffected = await connection.ExecuteAsync(query, contrato);
                    if (rowsAffected > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
            }
            return result;
        }
        public async Task<bool> ActualizarDatosClienteContrato(ClienteContrato contrato)
        {
            string query = @"
        UPDATE tb_cliente_contrato
        SET 
            id_empresa = @IdEmpresa,
            constitutiva_escriturapublica = @ConstitutivaEscrituraPublica,
            constitutiva_fecha = @ConstitutivaFecha,
            constitutiva_licenciado = @ConstitutivaLicenciado,
            constitutiva_numeronotario = @ConstitutivaNumeroNotario,
            constitutiva_foliomercantil = @ConstitutivaFolioMercantil,
            poder_escriturapublica = @PoderEscrituraPublica,
            poder_fecha = @PoderFecha,
            poder_licenciado = @PoderLicenciado,
            poder_numeronotario = @PoderNumeroNotario,
            cliente_registropatronal = @ClienteRegistroPatronal,
            poliza_monto = @PolizaMonto,
            poliza_montoletra = @PolizaMontoLetra,
            poliza_empresa = @PolizaEmpresa,
            contrato_vigencia = @ContratoVigencia,
            empresa_contacto_nombre = @EmpresaContactoNombre,
            empresa_contacto_correo = @EmpresaContactoCorreo,
            empresa_contacto_telefono = @EmpresaContactoTelefono,
            cliente_direccion = @ClienteDireccion,
            cliente_colonia = @ClienteColoniaDescripcion,
            cliente_municipio = @ClienteMunicipio,
            cliente_estado = @ClienteEstado,
            cliente_cp = @CP,
            poliza_numero = @PolizaNumero,
            cliente_email = @ClienteEmail,
            cliente_representante = @ClienteRepresentante,
            cliente_contacto_nombre = @ClienteContactoNombre,
            cliente_contacto_telefono = @ClienteContactoTelefono,
            constitutiva_idestado = @ConstitutivaIdEstado,
            poder_idestado = @PoderIdEstado
        WHERE id_prospecto = @IdProspecto";

            bool result = false;

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    var rowsAffected = await connection.ExecuteAsync(query, contrato);

                    if (rowsAffected > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar datos en la tabla: {ex.Message}");
            }

            return result;
        }
        public async Task<bool> ConsultarContratoExistente(int idProspecto)
        {
            string query = "SELECT COUNT(*) FROM tb_cliente_contrato WHERE id_prospecto = @IdProspecto";

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    var count = await connection.ExecuteScalarAsync<int>(query, new { IdProspecto = idProspecto });

                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al consultar la existencia del contrato: {ex.Message}");
                return false;
            }
        }
        public async Task<decimal> ObtenerTotalCotizacion(int idCotizacion)
        {
            string query = @"SELECT ISNULL(total,0) Total FROM tb_cotizacion WHERE id_cotizacion = @idCotizacion";
            decimal total;
            try
            {
                using (var connecion = ctx.CreateConnection())
                {
                    total = await connecion.QueryFirstOrDefaultAsync<decimal>(query, new { idCotizacion });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return total;
        }
        public async Task<List<Direccion>> ObtenerDireccionesCotizacion(int idCotizacion)
        {
            var query = @"
                                SELECT dc.id_direccion_cotizacion IdDireccionCotizacion, d.id_direccion IdDireccion, nombre_sucursal NombreSucursal, id_tipo_inmueble IdTipoInmueble,
                                    d.id_estado IdEstado, d.id_tabulador IdTabulador, municipio Municipio, ciudad Ciudad, colonia Colonia, domicilio Domicilio, referencia Referencia, codigo_postal CodigoPostal,
                                    contacto Contacto, telefono_contacto TelefonoContacto, id_estatus_direccion IdEstatusDireccion, fecha_alta FechaAlta, e.descripcion Estado, ti.descripcion TipoInmueble
                                FROM tb_direccion d
                                JOIN tb_estado e on d.id_estado = e.id_estado
                                JOIN tb_tipoinmueble ti on d.id_tipo_inmueble = ti.id_tipoinmueble
                                JOIN tb_direccion_cotizacion dc on dc.Id_Direccion = d.Id_Direccion
                                WHERE dc.id_cotizacion = @idCotizacion and id_estatus_direccion = 1";

            var direcciones = new List<Direccion>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direcciones = (await connection.QueryAsync<Direccion>(query, new { idCotizacion })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return direcciones;
        }
        public async Task<List<PuestoDireccionCotizacion>> ObtenerPuestosDireccionCotizacion(int idDireccionCotizacion)
        {
            string query = @"
                    select 
                    p.id_puesto IdPuesto,
                    pdc.cantidad Cantidad, 
                    p.descripcion Puesto, 
                    jornada Jornada,
                    j.descripcion JornadaDesc, 
                    t.id_turno IdTurno,
                    t.descripcion Turno, 
                    hr_inicio HrInicio, 
                    hr_fin HrFin,
                    dia_inicio DiaInicio, 
                    dia_fin DiaFin,
                    fecha_alta FechaAlta,
                    sueldo Sueldo, 
                    dc.id_cotizacion IdCotizacion, 
                    pdc.id_direccion_cotizacion IdDireccionCotizacion, 
                    id_puesto_direccioncotizacion IdPuestoDireccionCotizacion,
                    aguinaldo Aguinaldo, 
                    vacaciones Vacaciones, 
                    prima_vacacional PrimaVacacional, 
                    isn ISN, imss IMSS,festivo Festivo , 
                    bonos Bonos, vales Vales, total Total, 
                    domingo Domingo, cubredescanso CubreDescanso,
                    hr_inicio_fin HrInicioFin,
                    hr_fin_fin HrFinFin,
                    dia_inicio_fin DiaInicioFin, 
                    dia_fin_fin DiaFinFin,
                    festivo Festivo,
                    bonos Bonos,
                    vales Vales,
                    domingo Domingo,
                    cubredescanso CubreDescanso,
                    horario_letra HorarioStr,
                    dia_descanso DiaDescanso
                    from tb_puesto_direccion_cotizacion pdc
                    join tb_puesto p on p.id_puesto = pdc.id_puesto
                    join tb_turno t on t.id_turno = pdc.id_turno
                    join tb_direccion_cotizacion dc on dc.id_direccion_cotizacion = pdc.id_direccion_cotizacion
                    join tb_jornada j on jornada = j.id_jornada
                    where dc.id_direccion_cotizacion = @idDireccionCotizacion order by p.descripcion
                    ";
            var direcciones = new List<PuestoDireccionCotizacion>();
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direcciones = (await connection.QueryAsync<PuestoDireccionCotizacion>(query, new { idDireccionCotizacion })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return direcciones;
        }
        public int InsertarClienteXML(string clienteXML, string logXML)
        {
            int result = 0;
            try
            {
                using var connection = ctx.CreateConnection();
                connection.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@Cabecero", new SqlXml(new System.Xml.XmlTextReader(clienteXML, System.Xml.XmlNodeType.Document, null)), DbType.Xml, ParameterDirection.Input);
                parameters.Add("@CabeceroLog", new SqlXml(new System.Xml.XmlTextReader(logXML, System.Xml.XmlNodeType.Document, null)), DbType.Xml, ParameterDirection.Input);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection.Execute("sp_cliente", parameters, commandType: CommandType.StoredProcedure);
                int idMov = parameters.Get<int>("@Id");
                result = idMov;
                Console.WriteLine("Cliente ID: " + idMov);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return result;
        }

        public bool InsertarXMLOficina(string oficinaXML)
        {
            bool result = false;
            try
            {
                using var connection = ctx.CreateConnection();
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@Cabecero", new SqlXml(new System.Xml.XmlTextReader(oficinaXML, System.Xml.XmlNodeType.Document, null)), DbType.Xml, ParameterDirection.Input);
                    connection.Execute("sp_clienteoficina", parameters, commandType: CommandType.StoredProcedure);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public bool InsertarLineaNegocioXML(string lineaNegocioXML)
        {
            bool result = false;
            try
            {
                using var connection = ctx.CreateConnection();
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Cabecero", new SqlXml(new System.Xml.XmlTextReader(lineaNegocioXML, System.Xml.XmlNodeType.Document, null)), DbType.Xml, ParameterDirection.Input);
                connection.Execute("sp_clientelinea", parameters, commandType: CommandType.StoredProcedure);
                result = true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return result;
        }
        public int InsertarDireccionXML(string direccionXML, string logXML)
        {
            int result = 0;
            try
            {
                using var connection = ctx.CreateConnection();
                connection.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@Cabecero", new SqlXml(new System.Xml.XmlTextReader(direccionXML, System.Xml.XmlNodeType.Document, null)), DbType.Xml, ParameterDirection.Input);
                parameters.Add("@CabeceroLog", new SqlXml(new System.Xml.XmlTextReader(logXML, System.Xml.XmlNodeType.Document, null)), DbType.Xml, ParameterDirection.Input);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection.Execute("sp_puntoatencion", parameters, commandType: CommandType.StoredProcedure);
                int idMov = parameters.Get<int>("@Id");
                result = idMov;
                Console.WriteLine("Cliente ID: " + idMov);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return result;
        }
        public int InsertarPlantillaXML(string plantillaXML)
        {
            int idplantilla = 0;
            try
            {
                using var connection = ctx.CreateConnection();
                connection.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@Cabecero", new SqlXml(new System.Xml.XmlTextReader(plantillaXML, System.Xml.XmlNodeType.Document, null)), DbType.Xml, ParameterDirection.Input);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection.Execute("sp_plantilla", parameters, commandType: CommandType.StoredProcedure);
                int idMov = parameters.Get<int>("@Id");
                idplantilla = idMov;
                Console.WriteLine("Cliente ID: " + idMov);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return idplantilla;
        }
        public bool InsertarHorarioPlantillaXML(string horarioPlantillaXML)
        {
            bool result = false;
            try
            {
                using var connection = ctx.CreateConnection();
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Cabecero", new SqlXml(new System.Xml.XmlTextReader(horarioPlantillaXML, System.Xml.XmlNodeType.Document, null)), DbType.Xml, ParameterDirection.Input);
                connection.Execute("sp_plantillahorario", parameters, commandType: CommandType.StoredProcedure);
                result = true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return result;
        }
        public bool InsertarPlantillaPXML(int idPlantillaCreada, int cantidad)
        {
            bool result = false;
            try
            {
                using var connection = ctx.CreateConnection();
                connection.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@pla", idPlantillaCreada, dbType: DbType.Int32, direction: ParameterDirection.Input);
                parameters.Add("@cant", cantidad, dbType: DbType.Int32, direction: ParameterDirection.Input);
                connection.Execute("sp_plantillaP", parameters, commandType: CommandType.StoredProcedure);
                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return result;
        }
        public bool InsertarVacantePlantillaXML(int idPlantillaCreada, int idPersonal)
        {
            bool result = false;
            try
            {
                using var connection = ctx.CreateConnection();
                connection.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@pla", idPlantillaCreada, dbType: DbType.Int32, direction: ParameterDirection.Input);
                parameters.Add("@usu", idPersonal, dbType: DbType.Int32, direction: ParameterDirection.Input);
                connection.Execute("sp_vacantexplantilla", parameters, commandType: CommandType.StoredProcedure);
                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return result;
        }
        public async Task<bool> InsertarMaterialAutorizado(MaterialCotizacion producto, int idPlantilla, int idClienteCreado)
        {
            int idFrecuencia = 1;
            switch ((int)producto.IdFrecuencia)
            {
                case 1: idFrecuencia = 1; break;
                case 2: idFrecuencia = 2; break;
                case 3: idFrecuencia = 3; break;
                case 4: idFrecuencia = 4; break;
                case 6: idFrecuencia = 5; break;
                case 12: idFrecuencia = 6; break;
                case 18: idFrecuencia = 6; break;
                case 24: idFrecuencia = 6; break;
            }
            string query = @"DECLARE @Existe INT
                            SET @Existe = (
                                SELECT CASE 
                                    WHEN EXISTS (
                                        SELECT 1 
                                        FROM tb_cliente_listaautorizada 
                                        WHERE id_cliente = @idClienteCreado AND clave = @ClaveProducto AND id_frecuencia = @IdFrecuencia
                                    ) THEN 1
                                    ELSE 0
                                END
                            )
                            IF @Existe = 0
	                            BEGIN
		                            insert into tb_cliente_listaautorizada (id_cliente, clave, id_frecuencia) values (@idClienteCreado, @ClaveProducto, @IdFrecuencia)
	                            END";
            bool result = false;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    var rowsAffected = await connection.ExecuteAsync(query, new { idClienteCreado, producto.ClaveProducto, idFrecuencia });
                    if (rowsAffected > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
            }
            return result;
        }
        public async Task<bool> InsertarMaterialDireccion(MaterialCotizacion producto, int idPlantilla, int idClienteCreado)
        {
            string query = @"DECLARE @Existe INT
                            SET @Existe = (
		                            SELECT CASE 
			                            WHEN EXISTS (
				                            SELECT 1 
				                            FROM tb_cliente_listatipo
				                            WHERE id_inmueble = @idPlantilla AND clave = @ClaveProducto AND id_frecuencia = @IdFrecuencia
			                            ) THEN 1
			                            ELSE 0
		                            END
	                            )
	                            IF @Existe = 1
		                            BEGIN
			                            UPDATE tb_cliente_listatipo SET Cantidad = Cantidad + @Cantidad WHERE id_inmueble = @IdPlantilla AND clave = @ClaveProducto AND id_frecuencia = @IdFrecuencia
		                            END
	                            ELSE
		                            BEGIN 
			                            insert into tb_cliente_listatipo (id_inmueble, clave, id_frecuencia, cantidad) values (@idPlantilla, @ClaveProducto, @IdFrecuencia, @Cantidad)
		                            END";
            bool result = false;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    var rowsAffected = await connection.ExecuteAsync(query, new { idPlantilla, producto.ClaveProducto, producto.IdFrecuencia, producto.Cantidad });
                    if (rowsAffected > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
            }
            return result;
        }

        public async Task<int> ObtenerConceptoPresupuestoPorLineaNegocio(int idServicio)
        {
            string query = @"SELECT id_concepto FROM tb_conceptoptto WHERE id_lineanegocio = @idServicio";
            int idConcepto = 0;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    idConcepto = await connection.QueryFirstAsync<int>(query, new { idServicio });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return idConcepto;
        }
        public bool InsertarPresupuestoMaterialXML(string presupuestoMateialXML)
        {
            bool result = false;
            try
            {
                using var connection = ctx.CreateConnection();
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Cabecero", new SqlXml(new System.Xml.XmlTextReader(presupuestoMateialXML, System.Xml.XmlNodeType.Document, null)), DbType.Xml, ParameterDirection.Input);
                connection.Execute("sp_material", parameters, commandType: CommandType.StoredProcedure);
                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return result;
        }
        public bool InsertarPresupuestoEquipoHerramientaXML(string presupuestoEquipoHerramientaXML)
        {
            bool result = false;
            try
            {
                using var connection = ctx.CreateConnection();
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Cabecero", new SqlXml(new System.Xml.XmlTextReader(presupuestoEquipoHerramientaXML, System.Xml.XmlNodeType.Document, null)), DbType.Xml, ParameterDirection.Input);
                connection.Execute("sp_herramienta", parameters, commandType: CommandType.StoredProcedure);
                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return result;
        }
        public bool InsertarEquipoHerramientaXML(string equipoHerramientaXML)
        {
            bool result = false;
            try
            {
                using var connection = ctx.CreateConnection();
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Cabecero", new SqlXml(new System.Xml.XmlTextReader(equipoHerramientaXML, System.Xml.XmlNodeType.Document, null)), DbType.Xml, ParameterDirection.Input);
                parameters.Add("@Estatus", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection.Execute("sp_listaherramienta", parameters, commandType: CommandType.StoredProcedure);
                int idMov = parameters.Get<int>("@Estatus");
                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return result;
        }

        public int InsertarXMLAsuntoLegal(string asuntoLegalXML)
        {
            int idAsuntoLegal;
            try
            {
                using var connection = ctx.CreateConnection();
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Cabecero", new SqlXml(new System.Xml.XmlTextReader(asuntoLegalXML, System.Xml.XmlNodeType.Document, null)), DbType.Xml, ParameterDirection.Input);
                parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection.Execute("sp_asunto", parameters, commandType: CommandType.StoredProcedure);
                int idMov = parameters.Get<int>("@id");
                idAsuntoLegal = idMov;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return idAsuntoLegal;
        }

        public async Task<int> ObtenerIdAsuntoPasoContrato(int idAsuntoCreado)
        {
            int idAsuntoPaso;
            string query = @"SELECT b.id_asunto_paso FROM tb_asunto a
                             INNER JOIN tb_asunto_pasos b on a.id_asunto = b.id_asunto
                             WHERE a.id_asunto = @idAsuntoCreado AND b.id_paso = 9";
            try
            {
                using var connection = ctx.CreateConnection();
                idAsuntoPaso = await connection.ExecuteScalarAsync<int>(query, new { idAsuntoCreado });

            }
            catch (Exception ex) { throw ex; }
            return idAsuntoPaso;
        }

        public bool InsertarContratoClienteXML(string contratoClienteXML)
        {
            bool result;
            try
            {
                using var connection = ctx.CreateConnection();
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Cabecero", new SqlXml(new System.Xml.XmlTextReader(contratoClienteXML, System.Xml.XmlNodeType.Document, null)), DbType.Xml, ParameterDirection.Input);
                connection.Execute("sp_asuntopaso_documento", parameters, commandType: CommandType.StoredProcedure);
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public async Task<bool> ActualizarEstatusAsuntoPaso(int idAsuntoPaso)
        {
            string query = @"UPDATE tb_asunto_pasos
                            SET id_estatus = 2
                            WHERE id_asunto_paso = @idAsuntoPaso";
            bool result = false;
            try
            {
                using var connection = ctx.CreateConnection();
                var rowsAffected = await connection.ExecuteAsync(query, new { idAsuntoPaso });
                if (rowsAffected > 0)
                {
                    result = true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public async Task<decimal> ObtenerTotalDireccion(int idCotizacion, int idDireccionCotizacion)
        {
            decimal totalPuestosDireccion;
            decimal totalExtrasDireccion;
            decimal result;
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idCotizacion", idCotizacion, DbType.Int32);
                    parameters.Add("@idDireccionCotizacion", idDireccionCotizacion, DbType.Int32);
                    totalPuestosDireccion = await connection.QuerySingleAsync<decimal>("sp_obtenerigualapuestosdireccion", parameters, commandType: CommandType.StoredProcedure);
                    totalExtrasDireccion = await connection.QuerySingleAsync<decimal>("sp_obtenerigualadireccionextras", parameters, commandType: CommandType.StoredProcedure);
                    result = totalPuestosDireccion + totalExtrasDireccion;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public void InsertarIgualasXML(string IgualasXML)
        {
            try
            {
                using var connection = ctx.CreateConnection();
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Cabecero", IgualasXML, DbType.String, ParameterDirection.Input);
                connection.Execute("sp_clienteiguala", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task ActualizarPresupuestosSucursal(int idPuntoAtencion, decimal totalMaterial, decimal totalHigienicos)
        {
            string query = @"UPDATE tb_cliente_inmueble SET presupuestol = @totalMaterial,presupuestoh = @totalHigienicos WHERE id_inmueble = @idPuntoAtencion";
            try
            {
                using var connection = ctx.CreateConnection();
                await connection.ExecuteAsync(query, new { idPuntoAtencion, totalMaterial, totalHigienicos });
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public async Task<CotizaPorcentajes> ObtenerPorcentajesCotizacion(int idCotizacion)
        {
            string query = @"SELECT costo_indirecto CostoIndirecto, utilidad Utilidad, comision_venta ComisionSobreVenta, comision_externa ComisionExterna FROM tb_cotizacion WHERE id_cotizacion = @idCotizacion";
            var porcentajes = new CotizaPorcentajes();
            try
            {
                using var connection = ctx.CreateConnection();
                porcentajes = await connection.QueryFirstAsync<CotizaPorcentajes>(query, new { idCotizacion });
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return porcentajes;
        }
        public async Task<string> ObtenerDescripcionServicio(int idServicioExtra)
        {
            string query = @"SELECT descripcion FROM tb_servicioextra WHERE id_servicioextra = @idServicioExtra";
            string concepto;
            try
            {
                using var connection = ctx.CreateConnection();
                concepto = await connection.QueryFirstAsync<string>(query, new { idServicioExtra });

            }
            catch(Exception ex)
            {
                throw ex;
            }
            return concepto;
        }

        public async Task InsertarSubcontrato(int idClienteCreado, int idServicio, int idFrecuencia, int idServicioExtra, decimal total)
        {

            switch (idFrecuencia){
                case 1: idFrecuencia = 4; break;
                case 2: idFrecuencia = 5; break;
                case 3: idFrecuencia = 6; break;
                case 4: idFrecuencia = 7; break;
                case 6: idFrecuencia = 8; break;
                case 12: idFrecuencia = 9; break;
                case 18: idFrecuencia = 11; break;
                case 24: idFrecuencia = 10; break;
            }
            string concepto = await ObtenerDescripcionServicio(idServicioExtra);
            string query = @"INSERT INTO tb_cliente_subcontrato
                            (
                                id_subcontrato,
                                id_cliente,
                                id_lineanegocio,
                                id_periodo,
                                concepto,
                                importe,
                                fechaaplica,
                                id_status
                            )
                            VALUES
                            (
                                (Select MAX(id_subcontrato)+1 FROM tb_cliente_subcontrato),
                                @idClienteCreado,
                                @idServicio,
                                @idFrecuencia,
                                @concepto,
                                @total,
                                GETDATE(),
                                1
                            )";
            try
            {
                using var connection = ctx.CreateConnection();
                await connection.ExecuteAsync(query, new { idClienteCreado, idServicio, idFrecuencia, concepto, total });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task InsertarCargaSocialPuesto(int idPlantillaCreada, decimal cargaSocial, decimal uniforme, decimal bonos, decimal primaDominical, decimal otrasComp)
        {
            string query = @"UPDATE tb_cliente_plantilla 
                            SET 
                            cargasocial = @cargaSocial,
                            uniforme = @uniforme,
                            bonoasist = @bonos,
                            primadominical = @primaDominical,
                            otrascomp = @otrasComp
                            WHERE id_plantilla = @idPlantillaCreada";
            try
            {
                using var connection = ctx.CreateConnection();
                await connection.ExecuteAsync(query, new { idPlantillaCreada, cargaSocial, uniforme,bonos, primaDominical, otrasComp });
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> ConsultarPoliza(int idCotizacion)
        {
            string query = @"SELECT poliza_cumplimiento FROM tb_cotizacion WHERE id_cotizacion = @idCotizacion";
            try
            {
                using var connection = ctx.CreateConnection();
                bool? polizaCumplimiento = await connection.QuerySingleOrDefaultAsync<bool?>(query, new { idCotizacion });
                return polizaCumplimiento ?? false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar la póliza de cumplimiento", ex);
            }
        }

    }

}
