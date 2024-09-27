using AutoMapper;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using SistemaVentasBatia.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;
using System.Net.Http;
using Newtonsoft.Json;
using System.Xml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateAndTime.Workdays;
using Org.BouncyCastle.Asn1.BC;
using System.Runtime.CompilerServices;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Asn1.Tsp;
using Microsoft.AspNetCore.Routing;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Diagnostics.Eventing.Reader;
using System.Net.Mail;
using System.Net;

namespace SistemaVentasBatia.Services
{
    public interface IClienteService
    {
        Task<ClienteContratoDTO> ObtenerDatosClienteContrato(int idProspecto);
        Task<bool> InsetarDatosClienteContrato(ClienteContratoDTO contrato);
        Task<int> ConvertirProspectoACliente(ClienteDTO cliente, string usuarioIP);
        Task<bool> InsertarContratoCliente(byte[] contrato, int idClienteCreado, string nombreCliente);
    }

    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository clienteRepo;
        private readonly IMaterialRepository materialRepo;
        private readonly IServicioRepository servicioRepo;
        private readonly ICotizacionesService cotizacionesService;
        private readonly IMapper mapper;

        public ClienteService(IClienteRepository clienteRepo, IMapper mapper, IMaterialRepository materialRepo, IServicioRepository servicioRepo, ICotizacionesService cotizacionesService)
        {
            this.clienteRepo = clienteRepo;
            this.mapper = mapper;
            this.materialRepo = materialRepo;
            this.servicioRepo = servicioRepo;
            this.cotizacionesService = cotizacionesService;
        }

        public async Task<ClienteContratoDTO> ObtenerDatosClienteContrato(int idProspecto)
        {
            var contrato = mapper.Map<ClienteContratoDTO>(await clienteRepo.ObtenerDatosClienteContrato(idProspecto));
            return contrato;
        }

        
        public async Task<bool> InsetarDatosClienteContrato(ClienteContratoDTO contrato)
        {
            var coincidencia = await clienteRepo.ConsultarContratoExistente(contrato.IdProspecto);
            var contratoData = mapper.Map<ClienteContrato>(contrato);
            if (coincidencia)
            {
                return await clienteRepo.ActualizarDatosClienteContrato(contratoData);
            }
            else
            {
                return await clienteRepo.InsertarDatosClienteContrato(contratoData);
            }
        }
        public async Task<int> ConvertirProspectoACliente(ClienteDTO cliente, string usuarioIP)
        {
            var porcentajes = await clienteRepo.ObtenerPorcentajesCotizacion(cliente.IdCotizacion);
            decimal totalCotizacion = await clienteRepo.ObtenerTotalCotizacion(cliente.IdCotizacion);
            //ResumenCotizacionLimpiezaDTO resumenCotizacion = await cotizacionesService.ObtenerResumenCotizacionLimpieza(cliente.IdCotizacion);
            //CREAR LOG
            string logXMLString = CrearXMLLog(cliente.IdPersonal, usuarioIP);

            //CREAR XML E INSERTAR CLIENTE
            string clienteXMLString = CrearXMLCliente(cliente);
            int idGrupoEmpresa = await clienteRepo.ConsultarGrupoEmpresas(cliente.IdEmpresaPagadora);
            int idClienteCreado = clienteRepo.InsertarClienteXML(clienteXMLString, logXMLString, idGrupoEmpresa);

            //CREAR XML E INSERTAR OFICINA
            string oficinaXMLString = CrearXMLOficina(idClienteCreado);
            bool oficina = clienteRepo.InsertarXMLOficina(oficinaXMLString);

            //CREAR XML E INSERTAR LINEA DE NEGOCIO
            string lineaNegocioXMLString = CrearXMLLineaNegocio(idClienteCreado, (int)cliente.IdServicio, totalCotizacion);
            bool lineaNegocio = clienteRepo.InsertarLineaNegocioXML(lineaNegocioXMLString);

            int idPuntoAtencionCreado = 0;
            int idPlantillaCreada = 0;
            decimal totalMateriales = 0;
            decimal totalHerramienta = 0;
            decimal totalEquipo = 0;
            decimal totalHigienicos = 0;
            decimal totalUniforme = 0;
            decimal totalEquipoHerramienta = 0;
            string IgualaXML = "";

            decimal totalGeneralMateriales = 0;
            decimal totalGeneralEquipo = 0;
            decimal totalGeneralHerramienta = 0;
            decimal totalGeneralHigienico = 0;
            decimal totalGeneralEquipoHerramienta = 0;

            //OBTENER LISTA DIRECCIONES
            var direcciones = mapper.Map<List<DireccionDTO>>(await clienteRepo.ObtenerDireccionesCotizacion(cliente.IdCotizacion));

            bool isPoliza = await clienteRepo.ConsultarPoliza(cliente.IdCotizacion);

            foreach (var direccion in direcciones)
            {
                totalMateriales = 0;
                totalHerramienta = 0;
                totalEquipo = 0;
                totalHigienicos = 0;
                totalUniforme = 0;
                totalEquipoHerramienta = 0;
                //CREAR XML E INSERTAR DIRECCION
                string direccionXMLString = CrearXMLDireccion(idClienteCreado, direccion, cliente.IdPersonal);
                idPuntoAtencionCreado = clienteRepo.InsertarDireccionXML(direccionXMLString, logXMLString);

                //OBTENER IGUALA POR PUNTO DE ATENCION Y CREAR XML
                decimal totalPuntoAtencion = await clienteRepo.ObtenerTotalDireccion(cliente.IdCotizacion, direccion.IdDireccionCotizacion);

                if (isPoliza)
                {
                    totalPuntoAtencion = totalPuntoAtencion * 1.10M;
                }
                IgualaXML = CrearXMLIgualaPuntoAtencion(idClienteCreado, idPuntoAtencionCreado, cliente.IdServicio, totalPuntoAtencion, IgualaXML);

                //INSERTAR XML tb_cliente_inmueble_ig


                //OBTENER LISTA PUESTOS
                var plantillas = mapper.Map<List<PuestoDireccionCotizacionDTO>>(await clienteRepo.ObtenerPuestosDireccionCotizacion(direccion.IdDireccionCotizacion)).ToList();

                //INSERTAR PUESTO A DIRECCION CREADA
                foreach (var puesto in plantillas)
                {
                    totalUniforme = 0;
                    string plantillaXMLString = CrearXMLPlantilla(idPuntoAtencionCreado, puesto, cliente.IdServicio, cliente.FechaInicio);
                    idPlantillaCreada = clienteRepo.InsertarPlantillaXML(plantillaXMLString);

                    //CREAR XML E INSERTAR HORARIO
                    string horarioplantillaXMLString = CrearXMLHorarioPlantilla(idPlantillaCreada, puesto);
                    bool horarioPlantilla = clienteRepo.InsertarHorarioPlantillaXML(horarioplantillaXMLString);

                    if (puesto.Cantidad > 0)
                    {
                        bool plantillap = clienteRepo.InsertarPlantillaPXML(idPlantillaCreada, puesto.Cantidad);
                        bool vacanteresult = clienteRepo.InsertarVacantePlantillaXML(idPlantillaCreada, cliente.IdPersonal);
                    }

                    //OBTENER LISTAS DE PRODUCTOS POR PUESTO
                    var material = await materialRepo.ObtenerMaterialCotizacionOperario(puesto.IdPuestoDireccionCotizacion, cliente.IdCotizacion);
                    var herramienta = await materialRepo.ObtenerHerramientaCotizacionOperario(puesto.IdPuestoDireccionCotizacion, cliente.IdCotizacion);
                    var equipo = await materialRepo.ObtenerEquipoCotizacionOperario(puesto.IdPuestoDireccionCotizacion, cliente.IdCotizacion);
                    var uniforme = await materialRepo.ObtenerUniformeCotizacionOperario(puesto.IdPuestoDireccionCotizacion, cliente.IdCotizacion);

                    //CALCULAR TOTAL E INSERTAR PRODUCTOS POR PUESTO
                    foreach (var mat in material)
                    {
                        await clienteRepo.InsertarMaterialAutorizado(mat, idPuntoAtencionCreado, idClienteCreado);
                        await clienteRepo.InsertarMaterialDireccion(mat, idPuntoAtencionCreado, idClienteCreado);
                        if (mat.ClaveProducto.StartsWith("M-HIG"))
                        {
                            totalHigienicos += (mat.Total / (int)mat.IdFrecuencia) * puesto.Cantidad;
                        }
                        else
                        {
                            totalMateriales += (mat.Total / (int)mat.IdFrecuencia) * puesto.Cantidad;

                        }
                    }
                    foreach (var herr in herramienta)
                    {

                        await clienteRepo.InsertarMaterialAutorizado(herr, idPuntoAtencionCreado, idClienteCreado);
                        string herramientaXMLString = CrearXMLInsertarEquipoHerramienta(idPuntoAtencionCreado, herr.ClaveProducto, (int)herr.IdFrecuencia, herr.Cantidad, herr.Total);
                        clienteRepo.InsertarEquipoHerramientaXML(herramientaXMLString);
                        totalHerramienta += (herr.Total / (int)herr.IdFrecuencia) * puesto.Cantidad;
                    }
                    foreach (var equ in equipo)
                    {
                        await clienteRepo.InsertarMaterialAutorizado(equ, idPuntoAtencionCreado, idClienteCreado);
                        string equipoXMLString = CrearXMLInsertarEquipoHerramienta(idPuntoAtencionCreado, equ.ClaveProducto, (int)equ.IdFrecuencia, equ.Cantidad, equ.Total);
                        clienteRepo.InsertarEquipoHerramientaXML(equipoXMLString);
                        totalEquipo += (equ.Total / (int)equ.IdFrecuencia) * puesto.Cantidad;
                    }
                    foreach (var uni in uniforme)
                    {
                        totalUniforme += (uni.Total / (int)uni.IdFrecuencia) * puesto.Cantidad;
                    }

                    decimal cargaSocial = 0;
                    cargaSocial = puesto.ISN + puesto.IMSS;
                    decimal otrasComp = puesto.Vales + puesto.Festivo + puesto.CubreDescanso;
                    await clienteRepo.InsertarCargaSocialPuesto(idPlantillaCreada, cargaSocial, totalUniforme, puesto.Bonos, puesto.Domingo, otrasComp);

                    //CREAR E INSERTAR HORARIO
                    string horarioActualizadoXML = CrearXMLHorario(puesto, idPlantillaCreada);
                    clienteRepo.InsertarHorarioActualizadoPlantillaXML(horarioActualizadoXML);
                    DateTime fecha = DateTime.Now;
                    EnviaCorreoVacantes(fecha, "Registro de Vacante", idPlantillaCreada);
                }

                //OBTENER LISTAS DE PRODUCTOS EXTRA POR SUCURSAL
                var materialExtraSuc = await materialRepo.ObtenerMaterialExtraCotizacionDireccion(cliente.IdCotizacion, direccion.IdDireccionCotizacion);
                var herramientaExtraSuc = await materialRepo.ObtenerHerramientaExtraCotizacionDireccion(cliente.IdCotizacion, direccion.IdDireccionCotizacion);
                var equipoExtraSuc = await materialRepo.ObtenerEquipoExtraCotizacionDireccion(cliente.IdCotizacion, direccion.IdDireccionCotizacion);
                var uniformeExtraSuc = await materialRepo.ObtenerUniformeExtraCotizacionDireccion(cliente.IdCotizacion, direccion.IdDireccionCotizacion);
                //var servicioExtraSuc = await servicioRepo.ObtenerListaServiciosCotizacionDireccion(cliente.IdCotizacion, 0, direccion.IdDireccionCotizacion);

                //CALCULAR TOTAL E INSERTAR PRODUCTOS EXTRA POR SUCURSAL
                foreach (var mat in materialExtraSuc)
                {
                    await clienteRepo.InsertarMaterialAutorizado(mat, idPuntoAtencionCreado, idClienteCreado);
                    await clienteRepo.InsertarMaterialDireccion(mat, idPuntoAtencionCreado, idClienteCreado);
                    if (mat.ClaveProducto.StartsWith("M-HIG"))
                    {
                        totalHigienicos += mat.Total / (int)mat.IdFrecuencia;
                    }
                    else
                    {
                        totalMateriales += mat.Total / (int)mat.IdFrecuencia;
                    }
                }
                foreach (var herr in herramientaExtraSuc)
                {
                    await clienteRepo.InsertarMaterialAutorizado(herr, idPuntoAtencionCreado, idClienteCreado);
                    string herramientaXMLString = CrearXMLInsertarEquipoHerramienta(idPuntoAtencionCreado, herr.ClaveProducto, (int)herr.IdFrecuencia, herr.Cantidad, herr.Total);
                    clienteRepo.InsertarEquipoHerramientaXML(herramientaXMLString);
                    totalHerramienta += herr.Total / (int)herr.IdFrecuencia;
                }
                foreach (var equ in equipoExtraSuc)
                {
                    await clienteRepo.InsertarMaterialAutorizado(equ, idPuntoAtencionCreado, idClienteCreado);
                    string equipoXMLString = CrearXMLInsertarEquipoHerramienta(idPuntoAtencionCreado, equ.ClaveProducto, (int)equ.IdFrecuencia, equ.Cantidad, equ.Total);
                    clienteRepo.InsertarEquipoHerramientaXML(equipoXMLString);
                    totalEquipo += equ.Total / (int)equ.IdFrecuencia;

                }
                //foreach (var uni in uniforme)
                //{
                //    await clienteRepo.InsertarProducto(uni, idPlantillaCreada, idClienteCreado); 
                //    totalUniformePuesto += uni.Total;
                //}

                //INSERTAR presupuestos por direccion
                await clienteRepo.ActualizarPresupuestosSucursal(idPuntoAtencionCreado, totalMateriales, totalHigienicos);

                totalGeneralMateriales += totalMateriales;
                totalGeneralEquipo += totalEquipo;
                totalGeneralHerramienta += totalHerramienta;
                totalGeneralHigienico += totalHigienicos;
                totalGeneralEquipoHerramienta += (totalEquipo + totalHerramienta);
            }

            //INSERTAR XML IGUALA POR PUNTO DE ATENCION
            clienteRepo.InsertarIgualasXML(IgualaXML);

            //INSERTAR PRODUCTOS EXTRA EN AUTORIZADOS
            //OBTENER LISTA DE PRODUCTOS EXTRA
            //var materialExtra = await materialRepo.ObtenerMaterialExtraCotizacion(cliente.IdCotizacion);
            //var herramientaExtra = await materialRepo.ObtenerHerramientaExtraCotizacion(cliente.IdCotizacion);
            //var equipoExtra = await materialRepo.ObtenerEquipoExtraCotizacion(cliente.IdCotizacion);
            //var uniformeExtra = await materialRepo.ObtenerUniformeExtraCotizacion(cliente.IdCotizacion);
            //var servicioExtra = await servicioRepo.ObtenerListaServiciosCotizacion(cliente.IdCotizacion, 0);

            //INSERTAR SERVICIOS EXTRA COMO SUBCONTRATOS
            //if (servicioExtra != null)
            //{
            //    foreach (var item in servicioExtra)
            //    {
            //        //decimal total = (((item.ImporteMensual * (porcentajes.CostoIndirecto + 1)) * (porcentajes.Utilidad + 1)) * (porcentajes.ComisionSobreVenta +1)) * (porcentajes.ComisionExterna +1);
            //        await clienteRepo.InsertarSubcontrato(idClienteCreado, cliente.IdServicio, (int)item.IdFrecuencia, item.IdServicioExtra, item.ImporteMensual);
            //    }
            //}


            ////CALCULAR TOTAL PRODUCTOS EXTRA
            //foreach (var matext in materialExtra)
            //{
            //    await clienteRepo.InsertarMaterial(matext, idPuntoAtencionCreado, idClienteCreado);
            //if (matext.ClaveProducto.StartsWith("M-HIG"))
            //{
            //    totalHigienicosExtra += matext.Total;
            //}
            //else
            //{
            //    totalMaterialesExtra += matext.Total;
            //}
            //}
            //foreach (var herrext in herramientaExtra)
            //{
            //    string herramientaXMLString = CrearXMLInsertarEquipoHerramienta(idPuntoAtencionCreado, herrext.ClaveProducto, (int)herrext.IdFrecuencia, herrext.Cantidad, herrext.Total);
            //    clienteRepo.InsertarEquipoHerramientaXML(herramientaXMLString);
            //    //totalHerramientaExtra += herrext.Total;
            //}
            //foreach (var equext in equipoExtra)
            //{
            //    string equipoXMLString = CrearXMLInsertarEquipoHerramienta(idPuntoAtencionCreado, equext.ClaveProducto, (int)equext.IdFrecuencia, equext.Cantidad, equext.Total);
            //    clienteRepo.InsertarEquipoHerramientaXML(equipoXMLString);
            //    //totalEquipoExtra += equext.Total;
            //}
            //foreach (var uniext in uniformeExtra)
            //{
            //    await clienteRepo.InsertarProducto(uniext, idPuntoAtencionCreado, idClienteCreado);
            //    totalUniformeExtra += uniext.Total;
            //}
            //foreach(var servext in servicioExtra)
            //{
            //    //await clienteRepo.InsertarServicio();
            //    totalServicioExtra = servext.Total;
            //}1852.5    totalMaterialesPuesto 1307.8

            totalEquipoHerramienta = totalEquipo + totalHerramienta;

            // GENERAR 3 PRESUPUESTOS, SOLO MATERIAL, SOLO HIGIENICOS Y HERRAMIENTA/EQUIPO
            if (totalGeneralMateriales > 0)
            {
                string presupuestoMaterialesXMLString = CrearXMLPresupuestoMateriales(idClienteCreado, cliente.IdServicio, 4, 1, totalGeneralMateriales, cliente.FechaInicio.ToString("yyyy-MM-dd HH:mm:ss"));
                clienteRepo.InsertarPresupuestoMaterialXML(presupuestoMaterialesXMLString);
            }
            if (totalGeneralHigienico > 0)
            {
                string presupuestoHigienicosXMLString = CrearXMLPresupuestoHigienicos(idClienteCreado, cliente.IdServicio, 4, 2, totalGeneralHigienico, cliente.FechaInicio.ToString("yyyy-MM-dd HH:mm:ss"));
                clienteRepo.InsertarPresupuestoMaterialXML(presupuestoHigienicosXMLString);
            }
            if (totalGeneralEquipoHerramienta > 0)
            {
                string presupuestoEquipoHerramientaXMLString = CrearXMLPresupuestoEquipoHerramienta(idClienteCreado, cliente.IdServicio, 4, "LISTA DE HERRAMIENTAS Y EQUIPO", totalGeneralEquipoHerramienta, cliente.FechaInicio.ToString("yyyy-MM-dd HH:mm:ss"));
                clienteRepo.InsertarPresupuestoEquipoHerramientaXML(presupuestoEquipoHerramientaXMLString);
            }

            //ACTUALIZAR ESTATUS DE PROSPECTO Y COTIZACIONES
            await cotizacionesService.CambiarEstatusProspectoContratado(cliente.IdProspecto);
            await cotizacionesService.CambiarEstatusCotizacionContratada(cliente.IdCotizacion);
            await cotizacionesService.CambiarEstatusCotizacionesNoSeleccionadas(cliente.IdCotizacion, cliente.IdProspecto);

            return idClienteCreado;
        }
        public string CrearXMLHorario(PuestoDireccionCotizacionDTO puesto, int idPlantillaCreada)
        {
            var horarioXML = new XmlDocument();
            var horarioElement = horarioXML.CreateElement("horario");
            horarioElement.SetAttribute("idplantilla", idPlantillaCreada.ToString());
            //VALIDAR SI ES UN HORARIO CRUZADO
            if ((int)puesto.DiaInicio < (int)puesto.DiaFin)
            {
                //SI EL DIA DE INICIO NO COMIENZA EN LUNES
                if ((int)puesto.DiaInicio > 1)
                {
                    //LLENAR EN REVERSA DIAS VACIOS
                    for (int i = (int)puesto.DiaInicio - 1; i >= 1; i--)
                    {
                        horarioElement.SetAttribute("dia" + i.ToString() + "de", "0");
                        horarioElement.SetAttribute("dia" + i.ToString() + "a", "0");
                    }
                }
                //LLENAR HASTA EL DIAFIN
                for (int i = (int)puesto.DiaInicio; i <= (int)puesto.DiaFin; i++)
                {
                    horarioElement.SetAttribute("dia" + i.ToString() + "de", puesto.HrInicio.Hours.ToString());
                    horarioElement.SetAttribute("dia" + i.ToString() + "a", puesto.HrFin.Hours.ToString());
                }

                if((int)puesto.DiaInicioFin > (int)puesto.DiaFin)
                {
                    for (int i = (int)puesto.DiaInicioFin; i <= (int)puesto.DiaFinFin; i++)
                    {
                        horarioElement.SetAttribute("dia" + i.ToString() + "de", puesto.HrInicioFin.Hours.ToString());
                        horarioElement.SetAttribute("dia" + i.ToString() + "a", puesto.HrFinFin.Hours.ToString());
                    }
                    if ((int)puesto.DiaFinFin <= 6)
                    {
                        for (int i = (int)puesto.DiaFinFin + 1; i <= 7; i++)
                        {
                            horarioElement.SetAttribute("dia" + i.ToString() + "de", "0");
                            horarioElement.SetAttribute("dia" + i.ToString() + "a", "0");
                        }
                    }
                }
                else
                {
                    //LLENAR DIAS VACIOS SI EL FIN DEL HORARIO NO LLEGA AL DOMINGO Y NO EXISTE UN SEGUNDO HORARIO
                    if ((int)puesto.DiaFin <= 6)
                    {
                        for (int i = (int)puesto.DiaFin + 1; i <= 7; i++)
                        {
                            horarioElement.SetAttribute("dia" + i.ToString() + "de", "0");
                            horarioElement.SetAttribute("dia" + i.ToString() + "a", "0");
                        }
                    }
                }
            }
            else
            {
                //EJ: viernes a miercoles
                //LLENAR DE VIERNES A FIN =>
                for (int i = (int)puesto.DiaInicio; i <= 7; i++)
                {
                    horarioElement.SetAttribute("dia" + i.ToString() + "de", puesto.HrInicio.Hours.ToString());
                    horarioElement.SetAttribute("dia" + i.ToString() + "a", puesto.HrFin.Hours.ToString());
                }

                //LLENAR DE MIERCOLES A LUNES <=
                for (int i = (int)puesto.DiaFin; i >= 1; i--)
                {
                    horarioElement.SetAttribute("dia" + i.ToString() + "de", puesto.HrInicio.Hours.ToString());
                    horarioElement.SetAttribute("dia" + i.ToString() + "a", puesto.HrFin.Hours.ToString());
                }

                //LENAR VACIOS SI EXISTE
                for(int i = (int)puesto.DiaFin + 1; i < (int)puesto.DiaInicio; i++)
                {
                    horarioElement.SetAttribute("dia" + i.ToString() + "de", "0");
                    horarioElement.SetAttribute("dia" + i.ToString() + "a", "0");
                }
            }


            horarioXML.AppendChild(horarioElement);
            string horarioXMLString = horarioXML.OuterXml;
            return horarioXMLString;
        }
        public string CrearXMLCliente(ClienteDTO cliente)
        {
            var clienteXML = new XmlDocument();
            var clienteElement = clienteXML.CreateElement("cliente");
            clienteElement.SetAttribute("id", "0");
            clienteElement.SetAttribute("tipo", cliente.IdTipo.ToString());
            clienteElement.SetAttribute("nombre", cliente.NombreComercial);
            clienteElement.SetAttribute("contacto", cliente.Contacto);
            clienteElement.SetAttribute("depto", cliente.Departamento);
            clienteElement.SetAttribute("puesto", cliente.Puesto);
            clienteElement.SetAttribute("correo", cliente.Email);
            clienteElement.SetAttribute("telefono", cliente.Telefonos);
            clienteElement.SetAttribute("sucursales", cliente.TotalSucursales.ToString());
            clienteElement.SetAttribute("empleados", cliente.TotalEmpleados.ToString());
            clienteElement.SetAttribute("reqmaterial", cliente.IncluyeMaterial.ToString());
            clienteElement.SetAttribute("reqherr", cliente.IncluyeHerramienta.ToString());
            clienteElement.SetAttribute("ejecutivo", cliente.IdEjecutivo.ToString());
            clienteElement.SetAttribute("encargado", cliente.IdGerenteLimpieza.ToString());
            clienteElement.SetAttribute("factura", cliente.Facturacion);
            clienteElement.SetAttribute("tipofac", cliente.TipoFacturacion);
            clienteElement.SetAttribute("diasfac", cliente.DiasFacturacion.ToString());
            clienteElement.SetAttribute("credito", cliente.Credito.ToString());
            clienteElement.SetAttribute("fini", cliente.FechaInicio.ToString("yyyy-MM-dd HH:mm:ss"));
            clienteElement.SetAttribute("vigencia", cliente.Vigencia.ToString());
            clienteElement.SetAttribute("ffin", cliente.FechaTermino.ToString("yyyy-MM-dd HH:mm:ss"));
            clienteElement.SetAttribute("usuario", cliente.IdPersonal.ToString());
            clienteElement.SetAttribute("ptmat", cliente.PorcentajeMateriales.ToString());
            clienteElement.SetAttribute("ptind", cliente.PorcentajeIndirectos.ToString());
            clienteElement.SetAttribute("dialimfac", cliente.DiaLimiteFacturar.ToString());
            clienteElement.SetAttribute("montotot", "0");
            clienteElement.SetAttribute("pmat", cliente.DeductivaMaterial ? "1" : "0");
            clienteElement.SetAttribute("pser", cliente.DeductivaServicio ? "1" : "0");
            clienteElement.SetAttribute("ppla", cliente.DeductivaPlantilla ? "1" : "0");
            clienteElement.SetAttribute("ppzo", cliente.DeductivaPlazoEntrega ? "1" : "0");
            clienteElement.SetAttribute("empresa", cliente.IdEmpresaPagadora.ToString());
            clienteElement.SetAttribute("idpersonal", cliente.IdPersonal.ToString());
            clienteXML.AppendChild(clienteElement);
            string clienteXMLString = clienteXML.OuterXml;
            return clienteXMLString;
        }
        public string CrearXMLOficina(int idClienteCreado)
        {
            var oficinaXML = new XmlDocument();
            var oficinaMovimientoElement = oficinaXML.CreateElement("Movimiento");
            oficinaMovimientoElement.SetAttribute("cliente", idClienteCreado.ToString());
            var oficinaCTEElement = oficinaXML.CreateElement("cte");
            oficinaCTEElement.SetAttribute("idcliente", idClienteCreado.ToString());
            oficinaCTEElement.SetAttribute("idplaza", "4");
            oficinaMovimientoElement.AppendChild(oficinaCTEElement);
            oficinaXML.AppendChild(oficinaMovimientoElement);
            string oficinaXMLString = oficinaXML.OuterXml;
            return oficinaXMLString;
        }
        public string CrearXMLDireccion(int idClienteCreado, DireccionDTO direccion, int idPersonal)
        {
            var direccionXML = new XmlDocument();
            var direccionElement = direccionXML.CreateElement("inmueble");
            direccionElement.SetAttribute("id", "0");
            direccionElement.SetAttribute("idcte", idClienteCreado.ToString());
            direccionElement.SetAttribute("nombre", direccion.NombreSucursal);
            direccionElement.SetAttribute("ceco", "0");
            direccionElement.SetAttribute("oficina", "0");
            direccionElement.SetAttribute("tipo", direccion.IdTipoInmueble.ToString());
            direccionElement.SetAttribute("calle", direccion.Domicilio);
            direccionElement.SetAttribute("entrecalle", "");
            direccionElement.SetAttribute("colonia", direccion.Colonia);
            direccionElement.SetAttribute("del", direccion.Municipio);
            direccionElement.SetAttribute("cp", direccion.CodigoPostal);
            direccionElement.SetAttribute("ciudad", direccion.Ciudad);
            direccionElement.SetAttribute("estado", direccion.IdEstado.ToString());
            direccionElement.SetAttribute("tel1", "");
            direccionElement.SetAttribute("tel2", "");
            direccionElement.SetAttribute("contacto", "");
            direccionElement.SetAttribute("correo", "");
            direccionElement.SetAttribute("cargo", "");
            direccionElement.SetAttribute("ptto1", "");
            direccionElement.SetAttribute("ptto2", "");
            direccionElement.SetAttribute("ptto3", "");
            direccionElement.SetAttribute("prefijo", "");
            direccionElement.SetAttribute("lat", "");
            direccionElement.SetAttribute("lon", "");
            direccionElement.SetAttribute("banio", "");
            direccionElement.SetAttribute("idpersonal", idPersonal.ToString());
            direccionXML.AppendChild(direccionElement);
            string direccionXMLString = direccionXML.OuterXml;
            return direccionXMLString;
        }
        public string CrearXMLPlantilla(int idPuntoAtencionCreado, PuestoDireccionCotizacionDTO puesto, int idServicio, DateTime FechaInicio)
        {
            string fechaActual = DateTime.Today.ToString("yyyyMMdd");
            var plantillaXML = new XmlDocument();
            var plantillaElement = plantillaXML.CreateElement("plantilla");
            plantillaElement.SetAttribute("id", "0");
            plantillaElement.SetAttribute("idinm", idPuntoAtencionCreado.ToString());
            plantillaElement.SetAttribute("puesto", puesto.IdPuesto.ToString());
            plantillaElement.SetAttribute("cantidad", puesto.Cantidad.ToString());
            plantillaElement.SetAttribute("turno", ((int)puesto.IdTurno).ToString());
            plantillaElement.SetAttribute("jornal", ParseNumberFromString(puesto.JornadaDesc).ToString());
            plantillaElement.SetAttribute("smntope", puesto.Sueldo.ToString());
            plantillaElement.SetAttribute("servicio", idServicio.ToString());
            plantillaElement.SetAttribute("uniforme", "0");
            plantillaElement.SetAttribute("formapago", "1");
            plantillaElement.SetAttribute("sexo", "3");
            plantillaElement.SetAttribute("movcant", puesto.Cantidad.ToString());
            plantillaElement.SetAttribute("movsuel", puesto.Sueldo.ToString());
            plantillaElement.SetAttribute("faplica", FechaInicio.ToString("yyyy-MM-dd HH:mm:ss"));
            plantillaElement.SetAttribute("tipon", "1");
            plantillaXML.AppendChild(plantillaElement);
            string plantillaXMLString = plantillaXML.OuterXml;
            return plantillaXMLString;
        }
        public string CrearXMLHorarioPlantilla(int idPlantillaCreada, PuestoDireccionCotizacionDTO puesto)
        {
            string fechaActual = DateTime.Today.ToString("yyyyMMdd");
            var horarioPlantillaXML = new XmlDocument();
            var horarioPlantillaElement = horarioPlantillaXML.CreateElement("horario");
            horarioPlantillaElement.SetAttribute("id", idPlantillaCreada.ToString());
            horarioPlantillaElement.SetAttribute("edadde", " ");
            horarioPlantillaElement.SetAttribute("edada", " ");
            horarioPlantillaElement.SetAttribute("horariode", (puesto.HrInicio.ToString()).Substring(0, 5));
            horarioPlantillaElement.SetAttribute("horarioa", (puesto.HrFin.ToString()).Substring(0, 5));
            horarioPlantillaElement.SetAttribute("diasde", puesto.DiaInicio.ToString());
            horarioPlantillaElement.SetAttribute("diasa", puesto.DiaFin.ToString());
            if (puesto.DiaInicioFin == puesto.DiaFinFin && puesto.DiaInicioFin != 0)
            {
                horarioPlantillaElement.SetAttribute("horariofs", puesto.DiaFinFin.ToString() + " de " + (puesto.HrInicioFin.ToString()).Substring(0, 5) + " a " + (puesto.HrFinFin.ToString()).Substring(0, 5));
            }
            if (puesto.DiaInicioFin != puesto.DiaFinFin && puesto.DiaInicioFin != 0)
            {
                horarioPlantillaElement.SetAttribute("horariofs", puesto.DiaInicioFin.ToString() + " a " + puesto.DiaFinFin.ToString() + " de " + (puesto.HrInicioFin.ToString()).Substring(0, 5) + " a " + (puesto.HrFinFin.ToString()).Substring(0, 5));
            }
            horarioPlantillaElement.SetAttribute("descanso", puesto.DiaDescanso.ToString());
            horarioPlantillaXML.AppendChild(horarioPlantillaElement);
            string horarioPlantillaXMLString = horarioPlantillaXML.OuterXml;
            return horarioPlantillaXMLString;
        }
        public string CrearXMLLog(int idPersonal, string usuarioIP)
        {
            var logXML = new XmlDocument();
            var logElement = logXML.CreateElement("log");
            logElement.SetAttribute("IdArea", "");
            logElement.SetAttribute("titulovista", "Cotizador(SINGACRM)");
            logElement.SetAttribute("idpersonal", idPersonal.ToString());
            logElement.SetAttribute("direccionip", usuarioIP);
            logXML.AppendChild(logElement);
            string logXMLString = logXML.OuterXml;
            return logXMLString;
        }
        public string CrearXMLLineaNegocio(int idClienteCreado, int idLineaNegocio, decimal totalCotizacion)
        {
            var lineaNegocioXML = new XmlDocument();
            var lineaNegocioElement = lineaNegocioXML.CreateElement("Movimiento");
            lineaNegocioElement.SetAttribute("cliente", idClienteCreado.ToString());
            var lineaNegocioClienteElement = lineaNegocioXML.CreateElement("cte");
            lineaNegocioClienteElement.SetAttribute("idcliente", idClienteCreado.ToString());
            lineaNegocioClienteElement.SetAttribute("idlinea", idLineaNegocio.ToString());
            lineaNegocioClienteElement.SetAttribute("monto", totalCotizacion.ToString());
            lineaNegocioElement.AppendChild(lineaNegocioClienteElement);
            lineaNegocioXML.AppendChild(lineaNegocioElement);
            string lineaNegocioXMLString = lineaNegocioXML.OuterXml;
            return lineaNegocioXMLString;
        }
        public string CrearXMLPresupuestoMateriales(int idClienteCreado, int idLineaNegocio, int idPeriodo, int idConcepto, decimal importe, string fechaInicio)
        {
            var PresMatXML = new XmlDocument();
            var pttoElement = PresMatXML.CreateElement("material");
            pttoElement.SetAttribute("cliente", idClienteCreado.ToString());
            pttoElement.SetAttribute("linea", idLineaNegocio.ToString());
            pttoElement.SetAttribute("periodo", idPeriodo.ToString());
            pttoElement.SetAttribute("concepto", idConcepto.ToString());
            pttoElement.SetAttribute("importe", importe.ToString());
            pttoElement.SetAttribute("faplica", fechaInicio.ToString());
            PresMatXML.AppendChild(pttoElement);
            string PptoMatXMLString = PresMatXML.OuterXml;
            return PptoMatXMLString;
        }
        public string CrearXMLPresupuestoHigienicos(int idClienteCreado, int idLineaNegocio, int idPeriodo, int idConcepto, decimal importe, string fechaInicio)
        {
            var PresMatXML = new XmlDocument();
            var pttoElement = PresMatXML.CreateElement("material");
            pttoElement.SetAttribute("cliente", idClienteCreado.ToString());
            pttoElement.SetAttribute("linea", idLineaNegocio.ToString());
            pttoElement.SetAttribute("periodo", idPeriodo.ToString());
            pttoElement.SetAttribute("concepto", idConcepto.ToString());
            pttoElement.SetAttribute("importe", importe.ToString());
            pttoElement.SetAttribute("faplica", fechaInicio.ToString());
            PresMatXML.AppendChild(pttoElement);
            string PptoMatXMLString = PresMatXML.OuterXml;
            return PptoMatXMLString;
        }
        public string CrearXMLPresupuestoEquipoHerramienta(int idClienteCreado, int idLineaNegocio, int idPeriodo, string concepto, decimal importe, string fechaInicio)
        {
            var PresMatXML = new XmlDocument();
            var pttoElement = PresMatXML.CreateElement("herramienta");
            pttoElement.SetAttribute("cliente", idClienteCreado.ToString());
            pttoElement.SetAttribute("linea", idLineaNegocio.ToString());
            pttoElement.SetAttribute("periodo", idPeriodo.ToString());
            pttoElement.SetAttribute("concepto", concepto);
            pttoElement.SetAttribute("importe", importe.ToString());
            pttoElement.SetAttribute("faplica", fechaInicio.ToString());
            PresMatXML.AppendChild(pttoElement);
            string PptoMatXMLString = PresMatXML.OuterXml;
            return PptoMatXMLString;
        }
        public string CrearXMLInsertarEquipoHerramienta(int idPuntoAtencion, string claveProducto, int idFrecuencia, decimal cantidad, decimal total)
        {
            switch (idFrecuencia)
            {
                case 1: idFrecuencia = 1; break;
                case 2: idFrecuencia = 2; break;
                case 3: idFrecuencia = 3; break;
                case 4: idFrecuencia = 4; break;
                case 6: idFrecuencia = 5; break;
                case 12: idFrecuencia = 6; break;
                case 18: idFrecuencia = 6; break;
                case 24: idFrecuencia = 7; break;
            }

            var equMatXML = new XmlDocument();
            var equMatElement = equMatXML.CreateElement("listaherramienta");
            equMatElement.SetAttribute("folio", idPuntoAtencion.ToString());
            equMatElement.SetAttribute("clave", claveProducto);
            equMatElement.SetAttribute("idfrecuencia", idFrecuencia.ToString());
            equMatElement.SetAttribute("cantidad", ((int)cantidad).ToString());
            equMatElement.SetAttribute("precio", total.ToString());
            equMatXML.AppendChild(equMatElement);
            string PptoMatXMLString = equMatXML.OuterXml;
            return PptoMatXMLString;
        }
        public string CrearXMLAsuntoLegal(int idClienteCreado, string nombreCliente)
        {
            DateTime fechaAlta = DateTime.Now;
            var fechaCierre = fechaAlta.AddDays(12);
            var asuntoXML = new XmlDocument();
            var asuntoLegalElement = asuntoXML.CreateElement("registro");
            asuntoLegalElement.SetAttribute("folio", "");
            asuntoLegalElement.SetAttribute("tipoasunto", "3");
            asuntoLegalElement.SetAttribute("fechaalta", fechaAlta.ToString("yyyy-MM-dd"));
            asuntoLegalElement.SetAttribute("fechacierre", fechaCierre.ToString("yyyy-MM-dd"));
            asuntoLegalElement.SetAttribute("tipoabogado", "1");
            asuntoLegalElement.SetAttribute("abogado", "3283");
            asuntoLegalElement.SetAttribute("nombre_abogado", "CARLOS ROBERTO VARGAS RIOS");
            asuntoLegalElement.SetAttribute("descripcion", "Contrato " + nombreCliente);
            asuntoLegalElement.SetAttribute("empleado", "0");
            asuntoLegalElement.SetAttribute("cliente", idClienteCreado.ToString());
            asuntoXML.AppendChild(asuntoLegalElement);
            string asuntoLegalXMLString = asuntoXML.OuterXml;
            return asuntoLegalXMLString;
        }
        public string CrearXMLAsuntoLegalContrato(int idAsuntoPaso, string nombreArchivo)
        {
            var asuntoContratoXML = new XmlDocument();
            var asuntoLegalContratoElement = asuntoContratoXML.CreateElement("registro");
            asuntoLegalContratoElement.SetAttribute("idasuntopaso", idAsuntoPaso.ToString());
            asuntoLegalContratoElement.SetAttribute("descripcion", nombreArchivo);
            asuntoContratoXML.AppendChild(asuntoLegalContratoElement);
            string asuntoLegalContratoXMLString = asuntoContratoXML.OuterXml;
            return asuntoLegalContratoXMLString;
        }
        public string CrearXMLIgualaPuntoAtencion(int idClienteCreado, int idPuntoAtencionCreado, int idServicio, decimal totalPuntoAtencion, string igualaXML)
        {
            var igualaDireccionXML = new XmlDocument();
            var igualaDireccionElement = igualaDireccionXML.CreateElement("partida");
            igualaDireccionElement.SetAttribute("cliente", idClienteCreado.ToString());
            igualaDireccionElement.SetAttribute("idinmueble", idPuntoAtencionCreado.ToString());
            igualaDireccionElement.SetAttribute("idlinea", idServicio.ToString());
            igualaDireccionElement.SetAttribute("importe", totalPuntoAtencion.ToString());
            igualaDireccionXML.AppendChild(igualaDireccionElement);
            string asuntoLegalContratoXMLString = igualaDireccionXML.OuterXml;
            igualaXML += asuntoLegalContratoXMLString;
            return igualaXML;
        }
        public static int ParseNumberFromString(string input)
        {
            // Usar una expresión regular para encontrar el primer número en la cadena
            Match match = Regex.Match(input, @"\d+");

            // Si se encuentra un número, convertirlo a entero
            if (match.Success)
            {
                return int.Parse(match.Value);
            }
            else
            {
                // Manejar el caso donde no se encuentra ningún número
                throw new FormatException("No se encontró ningún número en la cadena de entrada.");
            }
        }
        public async Task<bool> InsertarContratoCliente(byte[] contrato, int idClienteCreado, string nombreCliente)
        {
            bool result;

            string pathContrato = "\\\\192.168.2.3\\c$\\inetpub\\wwwroot\\SINGA_APP\\Doctos\\leg\\asuntos"; //RUTA PROD
            //string pathContrato = "C:\\Users\\LAP_Sistemas5\\source\\repos\\SINGA_NEW\\Doctos\\Leg\\asuntos"; //RUTA DEV

            //INSERTAR ASUNTO
            string asuntoLegalXMLString = CrearXMLAsuntoLegal(idClienteCreado, nombreCliente);
            int idAsuntoCreado = clienteRepo.InsertarXMLAsuntoLegal(asuntoLegalXMLString);

            //CREAR CARPETA SI NO EXISTE
            string carpeta = Path.Combine(pathContrato, idAsuntoCreado.ToString());
            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }

            string extension = DetectFileExtension(contrato);
            if (extension == null)
            {
                throw new InvalidOperationException("Formato de archivo no soportado.");
            }

            //CREAR RUTA DEL ARCHIVO
            string nombreArchivo = "Contrato_" + nombreCliente + extension;
            var filePath = Path.Combine(carpeta, nombreArchivo);

            //GENERAR ARCHIVO
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    stream.Write(contrato, 0, contrato.Length);
                }
                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar el archivo: " + ex.Message);
                result = false;
            }
            //GENERAR E INSERTAR REGISTRO DE ARCHIVO CARGADO
            int idAsuntoPaso = await clienteRepo.ObtenerIdAsuntoPasoContrato(idAsuntoCreado);
            string contratoAsuntoLegalXMLString = CrearXMLAsuntoLegalContrato(idAsuntoPaso, nombreArchivo);
            bool contratoInsertado = clienteRepo.InsertarContratoClienteXML(contratoAsuntoLegalXMLString);

            bool actualizaPaso = await clienteRepo.ActualizarEstatusAsuntoPaso(idAsuntoPaso);

            return result;
        }
        private string DetectFileExtension(byte[] fileContent)
        {
            // Verificar las firmas de archivos conocidos
            if (fileContent.Length >= 4)
            {
                // DOCX: PK\x03\x04
                if (fileContent[0] == 0x50 && fileContent[1] == 0x4B && fileContent[2] == 0x03 && fileContent[3] == 0x04)
                {
                    return ".docx";
                }
                // PDF: %PDF-
                if (fileContent[0] == 0x25 && fileContent[1] == 0x50 && fileContent[2] == 0x44 && fileContent[3] == 0x46)
                {
                    return ".pdf";
                }
            }

            // Devolver null si no se detecta el formato
            return null;
        }
        private async void EnviaCorreoVacantes(DateTime fecha, string titulo, int idPlantilla)
        {
            Correo email = new Correo();
            string fechastring = fecha.ToString("yyyyMMdd");
            email = await clienteRepo.ObenerDetalleCorreo(idPlantilla, fechastring);
            //email.Gerente = "edgarc@grupobatia.com.mx";  //comentar para PROD
            
            string body = @"<html style='width:100%;font-family:arial, 'helvetica neue', helvetica, sans-serif;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;padding:0;Margin:0;'>
                <head>
                <meta charset='UTF-8'>
                <meta content='width=device-width, initial-scale=1' name='viewport'>
                <meta name='x-apple-disable-message-reformatting'>
                <meta http-equiv='X-UA-Compatible' content='IE=edge'>
                <meta content='telephone=no' name='format-detection'>
                <title>Nuevo correo electrónico 2</title>
                <!--[if (mso 16)]>
                <style type='text/css'>
                a {text-decoration: none;}
                </style>
                <![endif]-->
                <!--[if gte mso 9]><style>sup { font-size: 100% !important; }</style><![endif]-->
                <!--[if !mso]><!-- -->
                <link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:400,400i,700,700i' rel='stylesheet'>
                <!--<![endif]-->
                <style type='text/css'>
                @media only screen and (max-width:600px) {p, ul li, ol li, a { font-size:16px!important; line-height:150%!important } h1 { font-size:30px!important; text-align:center; line-height:120%!important } h2 { font-size:26px!important; text-align:center; line-hei
                #outlook a {
                padding: 0;
                }
                .ExternalClass {
                width: 100%;
                }
                .ExternalClass,
                .ExternalClass p,
                .ExternalClass span,
                .ExternalClass font,
                .ExternalClass td,
                .ExternalClass div {
                line-height:  100%;
                }
                .es-button {
                mso-style-priority:100!important;
                text-decoration: none!important;
                }
                a[x-apple-data-detectors] {
                color:inherit!important;
                text-decoration: none!important;
                font-size:inherit!important;
                font-family: inherit!important;
                font-weight:inherit!important;
                line-height: inherit!important;
                }
                .es-desk-hidden {
                display:none;
                float: Left;
                overflow:hidden;
                width:  0;
                max-height:0;
                line-height:  0;
                mso-hide: all;
                }
                </style>
                </head>
                <body style='width:100%;font-family:arial, 'helvetica neue', helvetica, sans-serif;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;padding:0;Margin:0;'>
                <div class='es-wrapper-color' style='background-color:#E4E5E7;'>
                <table class='es-header' cellspacing='0' cellpadding='0' align='center' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:transparent;background-repeat:repeat;background-position:center top;'>
                <tr style='border-collapse:collapse;'>
                <td align='center' style='padding:0;Margin:0;'>
                <table class='es-header-body' width='600' cellspacing='0' cellpadding='0' align='center' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#34265F;'>
                <tr style='border-collapse:collapse;'>
                <td align='left' style='padding:0;Margin:0;padding-top:20px;padding-left:20px;padding-right:20px;'>
                <!--[if mso]><table width='560' cellpadding='0' cellspacing='0'><tr><td width='178' valign='top'><![endif]-->
                <table class='es-left' cellspacing='0' cellpadding='0' align='left' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left;'>
                <tr style='border-collapse:collapse;'>
                <td class='es-m-p0r es-m-p20b' width='178' valign='top' align='center' style='padding:0;Margin:0;'>
                <table width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>
                <td class='es-m-txt-c' align='left' style='padding:0;Margin:0;'><img src='http://grupobatia.com.mx/images/gbblanco.png' alt='Logo Batia' title='Logo Batia' width='104' style='display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic;'></td>
                </tr>
                </table></td>
                </tr>
                </table>
                <!--[if mso]></td><td width='20'></td><td width='362' valign='top'><![endif]-->
                <table cellspacing='0' cellpadding='0' align='right' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>
                <td width='362' align='left' style='padding:0;Margin:0;'>
                <table width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>
                <td class='es-m-txt-c' align='right' style='padding:0;Margin:0;padding-top:15px;padding-bottom:20px;'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:14px;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;color:#FFFFFF;'>Fecha:" + fechastring + @"
                </p><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:14px;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;color:#FFFFFF;' id='txfechaenvio'></p></td>
                </tr>
                </table></td>
                </tr>
                </table>
                <!--[if mso]></td></tr></table><![endif]--></td>
                </tr>
                <tr style='border-collapse:collapse;'>
                <td align='left' style='padding:0;Margin:0;'>
                <table width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>
                <td width='600' valign='top' align='center' style='padding:0;Margin:0;'>
                <table width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>
                <td align='center' style='padding:0;Margin:0;'><img class='adapt-img' src='http://grupobatia.com.mx/images/47051523540803179.png' alt style='display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic;' width='600'></td>
                </tr>
                </table></td>
                </tr>
                </table></td>
                </tr>
                </table></td>
                </tr>
                </table>
                <table class='es-content' cellspacing='0' cellpadding='0' align='center' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;'>
                <tr style='border-collapse:collapse;'>
                <td align='center' style='padding:0;Margin:0;'>
                <table class='es-content-body' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#EDEDED;' width='600' cellspacing='0' cellpadding='0' bgcolor='#ededed' align='center'>
                <tr style='border-collapse:collapse;'>
                <td align='left' style='padding:0;Margin:0;'>
                <table width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>
                <td width='600' valign='top' align='center' style='padding:0;Margin:0;'>
                <table width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>
                <td align='center' style='padding:0;Margin:0;padding-top:20px;padding-left:40px;padding-right:40px;'><h1 style='Margin:0;line-height:36px;mso-line-height-rule:exactly;font-family:tahoma, verdana, segoe, sans-serif;font-size:30px;font-style:normal;font-weight:normal;color:#333333;'>Mensaje generado por SINGA</h1></td>
                </tr>
                <tr style='border-collapse:collapse;'>
                <td align='center' style='padding:0;Margin:0;'><img class='adapt-img' src='http://grupobatia.com.mx/images/92931515066045884.jpg' alt width='600' style='display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic;'></td>
                </tr>
                <tr style='border-collapse:collapse;'>
                <td align='center' style='padding:0;Margin:0;padding-bottom:10px;padding-left:40px;padding-right:40px;'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:16px;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:24px;color:#333333;' id='txconcepto'></p></td>
                </tr>
                </table></td>
                </tr>
                </table></td>
                </tr>
                <tr style='border-collapse:collapse;'>
                <td align='left' style='padding:0;Margin:0;'>
                <table width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>
                <td width='600' valign='top' align='center' style='padding:0;Margin:0;'>
                </tr>
                </table></td>
                </tr>
                </table></td>
                </tr>
                </table>
                <table class='es-content' cellspacing='0' cellpadding='0' align='center' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;'>
                <tr style='border-collapse:collapse;'>
                <td align='center' style='padding:0;Margin:0;'>
                <table class='es-content-body' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#EDEDED;' width='600' cellspacing='0' cellpadding='0' bgcolor='#ededed' align='center'>
                <tr style='border-collapse:collapse;'>
                <td align='left' style='Margin:0;padding-top:20px;padding-bottom:20px;padding-left:40px;padding-right:40px;'>
                <table width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>
                <td width='520' align='left' style='padding:0;Margin:0;'>
                <table width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>
                <td align='center' style='padding:0;Margin:0;'><h3 style='Margin:0;line-height:24px;mso-line-height-rule:exactly;font-family:tahoma, verdana, segoe, sans-serif;font-size:20px;font-style:normal;font-weight:normal;color:#333333;'>" + titulo + @"</h3></td>
                </tr>
                <tr style='border-collapse:collapse;'>
                <td align='center' style='padding:0;Margin:0;padding-top:10px;padding-bottom:10px;'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:14px;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;color:#333333;'>Estimado usuario, se ha realizado una modificación de plantillas, lo cual a generado vacantes de forma automática, favor de ingresar a SINGA para seguir con el proceso de reclutamiento, a continuación un resumen de los datos de la solicitud.<br></p></td>
                </tr>
                <tr style='border-collapse:collapse;'>
                <td align='center' style='padding:0;Margin:0;padding-bottom:10px;padding-top:15px;'><h3 style='Margin:0;line-height:24px;mso-line-height-rule:exactly;font-family:tahoma, verdana, segoe, sans-serif;font-size:20px;font-style:normal;font-weight:normal;color:#333333;'> Cliente:" + email.Cliente + @"</h3></td>
                </tr>
                </table></td>
                </tr>
                <tr style='border-collapse:collapse;'>
                <td class='es-m-p20b' width='580' align='center' style='padding:0;Margin:0;'>
                <table width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>
                <td style='padding:0;Margin:0;'>
                <table class='es-table-not-adapt' cellspacing='0' cellpadding='0' align='center' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>
                <td valign='top' align='left' style='padding:0;Margin:0;padding-top:10px;padding-bottom:10px;padding-right:10px;'><img src='http://grupobatia.com.mx/images/Check_Mark_Black5.png' alt=alt width='16' style='display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic;' /></td>
                <td align='left' style='padding:0;Margin:0;'>
                <table width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>
                <td align='left' style='padding:0;Margin:0;'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:14px;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;color:#333333;'>Puesto:" + email.Puesto + @".</p></td>
                </tr>
                </table>
                </td>
                </tr>
                <tr style='border-collapse:collapse;'>
                <td valign='top' align='left' style='padding:0;Margin:0;padding-top:10px;padding-bottom:10px;padding-right:10px;'><img src='http://grupobatia.com.mx/images/Check_Mark_Black5.png' alt=alt width='16' style='display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic;' /></td>
                <td align='left' style='padding:0;Margin:0;'>
                <table width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>        
                <td align='left' style='padding:0;Margin:0;'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:14px;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;color:#333333;'>Punto de Atención:" + email.Sucursal + @".</p></td>
                </tr>
                </table>
                </td>
                </tr>
                <tr style='border-collapse:collapse;'>
                <td valign='top' align='left' style='padding:0;Margin:0;padding-top:10px;padding-bottom:10px;padding-right:10px;'><img src='http://grupobatia.com.mx/images/Check_Mark_Black5.png' alt=alt width='16' style='display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic;' /></td>        
                <td align='left' style='padding:0;Margin:0;'>        
                <table width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>
                <td align='left' style='padding:0;Margin:0;'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:14px;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;color:#333333;'>Ubicación:" + email.Ubicacion + @".</p></td>
                </tr>
                </table>
                </td>
                </tr>
                <tr style='border-collapse:collapse;'>
                <td valign='top' align='left' style='padding:0;Margin:0;padding-top:10px;padding-bottom:10px;padding-right:10px;'><img src='http://grupobatia.com.mx/images/Check_Mark_Black5.png' alt=alt width='16' style='display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic;' /></td>
                <td align='left' style='padding:0;Margin:0;'>
                <table width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>
                <td align='left' style='padding:0;Margin:0;'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:14px;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;color:#333333;'>Resumen de horario:" + email.Horario + @".</p></td>
                </tr>
                </table>
                </td>
                </tr>
                </table></td>
                </tr>
                </table></td>
                </tr>
                </table></td>
                </tr>
                </table></td>
                </tr>
                </table>
                <table class='es-content' cellspacing='0' cellpadding='0' align='center' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;'>
                <tr style='border-collapse:collapse;'>
                <td align='center' style='padding:0;Margin:0;'>
                <table class='es-content-body' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#EDEDED;' width='600' cellspacing='0' cellpadding='0' bgcolor='#ededed' align='center'>
                <tr style='border-collapse:collapse;'>
                <td align='left' style='padding:0;Margin:0;'>
                <table width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>
                <td width='600' valign='top' align='center' style='padding:0;Margin:0;'>
                <table width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>        
                <td align='center' style='padding:0;Margin:0;'><img class='adapt-img' src='http://grupobatia.com.mx/images/94931515066951223.png' alt width='600' style='display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic;'></td>
                </tr>
                </table></td>
                </tr>
                </table></td>
                </tr>
                <tr style='border-collapse:collapse;'>
                <td style='Margin:0;padding-top:10px;padding-bottom:20px;padding-left:20px;padding-right:20px;background-color:#34265F;' bgcolor='#34265f' align='left'>
                <!--[if mso]><table width='560' cellpadding='0' cellspacing='0'><tr><td width='194'><![endif]-->
                <table class='es-left' cellspacing='0' cellpadding='0' align='left' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left;'>
                <tr style='border-collapse:collapse;'>
                <td class='es-m-p0r es-m-p20b' width='174' align='center' style='padding:0;Margin:0;'>
                <table width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>
                <td esdev-links-color='#ffffff' align='right' style='padding:0;Margin:0;padding-top:5px;'><h3 style='Margin:0;line-height:48px;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;font-size:32px;font-style:normal;font-weight:normal;color:#FFFFFF;'><strong><a target='_blank' style='-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;font-size:32px;text-decoration:none;color:#FFFFFF;line-height:48px;' href='https://viewstripo.email/'>SINGA</a></strong><br></h3></td>
                </tr>
                </table></td>
                <td class='es-hidden' width='20' style='padding:0;Margin:0;'></td>
                </tr>
                </table>
                <table class='es-right' cellspacing='0' cellpadding='0' align='right' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:right;'>
                <tr style='border-collapse:collapse;'>
                <td class='es-m-p20b' width='173' align='left' style='padding:0;Margin:0;'>
                </tr>
                </table>
                <!--[if mso]></td></tr></table><![endif]--></td>
                </tr>
                </table></td>
                </tr>
                </table>
                <table class='es-footer' cellspacing='0' cellpadding='0' align='center' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:transparent;background-repeat:repeat;background-position:center top;'>
                <tr style='border-collapse:collapse;'>
                <td align='center' style='padding:0;Margin:0;'>
                <table class='es-footer-body' width='600' cellspacing='0' cellpadding='0' align='center' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:transparent;'>
                <tr style='border-collapse:collapse;'>
                <td align='left' style='padding:0;Margin:0;padding-top:20px;padding-left:20px;padding-right:20px;'>
                <table width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>
                <td width='560' valign='top' align='center' style='padding:0;Margin:0;'>
                <table width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>
                <td align='center' style='padding:0;Margin:0;'><img src='http://grupobatia.com.mx/images/logo%20batia.png' alt='Logo Batia' title='Logo Batia' width='104' style='display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic;'></td>
                </tr>
                <tr style='border-collapse:collapse;'>
                <td align='center' style='padding:10px;Margin:0;'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:12px;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:18px;color:#333333;'>Usted esta recibiendo este correo de forma automática, favor de NO responder al remitente, si usted no es el destinatario correcto favor de hacer caso omiso del mismo.</p><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:12px;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:18px;color:#333333;'><a target='_blank' href='' style='-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;font-size:12px;text-decoration:none;color:#34265F;'>quiero saber mas</a> | <a target='_blank' href='http://singa.com.mx:8083/login.aspx' style='-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;font-size:12px;text-decoration:none;color:#34265F;'>sistema integral de gestión administrativa</a></p></td>
                </tr>
                <tr style='border-collapse:collapse;'>
                <td align='center' style='Margin:0;padding-top:5px;padding-bottom:5px;padding-left:20px;padding-right:20px;'>
                <table width='100%' height='100%' cellspacing='0' cellpadding='0' border='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
                <tr style='border-collapse:collapse;'>
                <td style='padding:0;Margin:0px 0px 0px 0px;border-bottom:1px solid #CCCCCC;background:none;height:1px;width:100%;margin:0px;'></td>
                </tr>
                </table></td>
                </tr>
                <tr style='border-collapse:collapse;'>
                <td align='center' style='padding:10px;Margin:0;'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:12px;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:18px;color:#333333;'>Grupo Batia, Av. Coyoacán 1704,&nbsp;Acacias 03100 Ciudad de México</p></td>
                </tr>
                </table></td>
                </tr>
                </table></td>
                </tr>
                </table></td>
                </tr>
                </table>
                </td>
                </tr>
                </table>
                </div>
                </body>
                </html>";

            string notificacion = "NOTIFICACION DE SINGA";
            var mail = new MailMessage
            {
                From = new MailAddress("adminsinga@grupobatia.com.mx"),
                Subject = notificacion,
                Body = body,
                IsBodyHtml = true
            };

            var destinatarios = email.Gerente.Split(';');
            foreach (var destinatario in destinatarios)
            {
                if (!string.IsNullOrEmpty(destinatario))
                {
                    mail.To.Add(destinatario);
                }
            }
            using (var smtp = new SmtpClient("smtp.office365.com", 587))
            {
                smtp.Credentials = new NetworkCredential("adminsinga@grupobatia.com.mx", "Ad*Gb6584");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
        }
    }
}
