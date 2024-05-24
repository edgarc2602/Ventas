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
            decimal totalCotizacion = await clienteRepo.ObtenerTotalCotizacion(cliente.IdCotizacion);
            //ResumenCotizacionLimpiezaDTO resumenCotizacion = await cotizacionesService.ObtenerResumenCotizacionLimpieza(cliente.IdCotizacion);
            //CREAR LOG
            string logXMLString = CrearXMLLog(cliente.IdPersonal, usuarioIP);

            //CREAR XML E INSERTAR CLIENTE
            string clienteXMLString = CrearXMLCliente(cliente);
            int idClienteCreado = clienteRepo.InsertarClienteXML(clienteXMLString, logXMLString);

            //CREAR XML E INSERTAR OFICINA
            string oficinaXMLString = CrearXMLOficina(idClienteCreado);
            bool oficina = clienteRepo.InsertarXMLOficina(oficinaXMLString);

            //CREAR XML E INSERTAR LINEA DE NEGOCIO
            string lineaNegocioXMLString = CrearXMLLineaNegocio(idClienteCreado, (int)cliente.IdServicio, totalCotizacion);
            bool lineaNegocio = clienteRepo.InsertarLineaNegocioXML(lineaNegocioXMLString);

            int idPuntoAtencionCreado = 0;
            int idPlantillaCreada = 0;
            decimal totalMaterialesPuesto = 0;
            decimal totalHerramientaPuesto = 0;
            decimal totalEquipoPuesto = 0;
            //decimal totalUniformePuesto = 0;
            decimal totalHigienicosPuesto = 0;

            decimal totalMaterialesExtra = 0;
            decimal totalHerramientaExtra = 0;
            decimal totalEquipoExtra = 0;
            //decimal totalUniformeExtra = 0;
            decimal totalHigienicosExtra = 0;
            //decimal totalServicioExtra = 0;

            decimal totalGeneralMateriales = 0;
            //decimal totalGeneralHerramienta = 0;
            decimal totalGeneralEquipoHerramienta = 0;
            //decimal totalGeneralUniforme = 0;

            decimal totalGeneralHigienico = 0;

            //OBTENER LISTA DIRECCIONES
            var direcciones = mapper.Map<List<DireccionDTO>>(await clienteRepo.ObtenerDireccionesCotizacion(cliente.IdCotizacion));

            foreach (var direccion in direcciones)
            {
                //CREAR XML E INSERTAR DIRECCION
                string direccionXMLString = CrearXMLDireccion(idClienteCreado, direccion, cliente.IdPersonal);
                idPuntoAtencionCreado = clienteRepo.InsertarDireccionXML(direccionXMLString, logXMLString);

                //OBTENER LISTA PUESTOS
                var plantillas = mapper.Map<List<PuestoDireccionCotizacionDTO>>(await clienteRepo.ObtenerPuestosDireccionCotizacion(direccion.IdDireccionCotizacion)).ToList();

                //INSERTAR PUESTO A DIRECCION CREADA
                foreach (var puesto in plantillas)
                {
                    string plantillaXMLString = CrearXMLPlantilla(idPuntoAtencionCreado, puesto, cliente.IdServicio);
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
                        await clienteRepo.InsertarMaterial(mat, idPuntoAtencionCreado, idClienteCreado);
                        await clienteRepo.InsertarMaterialDireccion(mat, idPuntoAtencionCreado, idClienteCreado);
                        if (mat.ClaveProducto.StartsWith("M-HIG"))
                        {
                            totalHigienicosPuesto += (mat.Total / (int)mat.IdFrecuencia) * puesto.Cantidad;
                        }
                        else
                        {
                            totalMaterialesPuesto += (mat.Total / (int)mat.IdFrecuencia) * puesto.Cantidad;
                        }
                    }
                    foreach (var herr in herramienta)
                    {
                        string herramientaXMLString = CrearXMLInsertarEquipoHerramienta(idPuntoAtencionCreado, herr.ClaveProducto, (int)herr.IdFrecuencia, herr.Cantidad, herr.Total);
                        clienteRepo.InsertarEquipoHerramientaXML(herramientaXMLString);
                        totalHerramientaPuesto += (herr.Total / (int)herr.IdFrecuencia) * puesto.Cantidad;
                    }
                    foreach (var equ in equipo)
                    {
                        string equipoXMLString = CrearXMLInsertarEquipoHerramienta(idPuntoAtencionCreado, equ.ClaveProducto, (int)equ.IdFrecuencia, equ.Cantidad, equ.Total);
                        clienteRepo.InsertarEquipoHerramientaXML(equipoXMLString);
                        totalEquipoPuesto += (equ.Total / (int)equ.IdFrecuencia) * puesto.Cantidad;
                    }
                    //foreach (var uni in uniforme)
                    //{
                    //    await clienteRepo.InsertarProducto(uni, idPlantillaCreada, idClienteCreado); 
                    //    totalUniformePuesto += uni.Total;
                    //}
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
                    await clienteRepo.InsertarMaterialDireccion(mat, idPuntoAtencionCreado, idClienteCreado);
                    if (mat.ClaveProducto.StartsWith("M-HIG"))
                    {
                        totalHigienicosExtra += mat.Total / (int)mat.IdFrecuencia;
                    }
                    else
                    {
                        totalMaterialesExtra += mat.Total / (int)mat.IdFrecuencia;
                    }
                }
                foreach (var herr in herramientaExtraSuc)
                {
                    string herramientaXMLString = CrearXMLInsertarEquipoHerramienta(idPuntoAtencionCreado, herr.ClaveProducto, (int)herr.IdFrecuencia, herr.Cantidad, herr.Total);
                    clienteRepo.InsertarEquipoHerramientaXML(herramientaXMLString);
                    totalHerramientaExtra += herr.Total / (int)herr.IdFrecuencia;
                }
                foreach (var equ in equipoExtraSuc)
                {
                    string equipoXMLString = CrearXMLInsertarEquipoHerramienta(idPuntoAtencionCreado, equ.ClaveProducto, (int)equ.IdFrecuencia, equ.Cantidad, equ.Total);
                    clienteRepo.InsertarEquipoHerramientaXML(equipoXMLString);
                    totalEquipoExtra += equ.Total / (int)equ.IdFrecuencia;
                }
                //foreach (var uni in uniforme)
                //{
                //    await clienteRepo.InsertarProducto(uni, idPlantillaCreada, idClienteCreado); 
                //    totalUniformePuesto += uni.Total;
                //}1585.33

            }

            //INSERTAR PRODUCTOS EXTRA EN AUTORIZADOS
            //OBTENER LISTA DE PRODUCTOS EXTRA
            var materialExtra = await materialRepo.ObtenerMaterialExtraCotizacion(cliente.IdCotizacion);
            var herramientaExtra = await materialRepo.ObtenerHerramientaExtraCotizacion(cliente.IdCotizacion);
            var equipoExtra = await materialRepo.ObtenerEquipoExtraCotizacion(cliente.IdCotizacion);
            var uniformeExtra = await materialRepo.ObtenerUniformeExtraCotizacion(cliente.IdCotizacion);
            var servicioExtra = await servicioRepo.ObtenerListaServiciosCotizacion(cliente.IdCotizacion, 0);

            //CALCULAR TOTAL PRODUCTOS EXTRA
            foreach (var matext in materialExtra)
            {
                await clienteRepo.InsertarMaterial(matext, idPuntoAtencionCreado, idClienteCreado);
                //if (matext.ClaveProducto.StartsWith("M-HIG"))
                //{
                //    totalHigienicosExtra += matext.Total;
                //}
                //else
                //{
                //    totalMaterialesExtra += matext.Total;
                //}
            }
            foreach (var herrext in herramientaExtra)
            {
                string herramientaXMLString = CrearXMLInsertarEquipoHerramienta(idPuntoAtencionCreado, herrext.ClaveProducto, (int)herrext.IdFrecuencia, herrext.Cantidad, herrext.Total);
                clienteRepo.InsertarEquipoHerramientaXML(herramientaXMLString);
                //totalHerramientaExtra += herrext.Total;
            }
            foreach (var equext in equipoExtra)
            {
                string equipoXMLString = CrearXMLInsertarEquipoHerramienta(idPuntoAtencionCreado, equext.ClaveProducto, (int)equext.IdFrecuencia, equext.Cantidad, equext.Total);
                clienteRepo.InsertarEquipoHerramientaXML(equipoXMLString);
                //totalEquipoExtra += equext.Total;
            }
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


            totalGeneralMateriales = totalMaterialesPuesto + totalMaterialesExtra;
            totalGeneralHigienico = totalHigienicosPuesto + totalHigienicosExtra;
            totalGeneralEquipoHerramienta = totalEquipoPuesto + totalEquipoExtra + totalHerramientaPuesto + totalHerramientaExtra;
            //totalGeneralMateriales = ((((totalGeneralMateriales * (resumenCotizacion.IndirectoPor + 1)) * (resumenCotizacion.UtilidadPor + 1)) * (resumenCotizacion.CsvPor + 1)) * (resumenCotizacion.ComisionExtPor +1));
            //totalGeneralHigienico = ((((totalGeneralHigienico * (resumenCotizacion.IndirectoPor + 1)) * (resumenCotizacion.UtilidadPor + 1)) * (resumenCotizacion.CsvPor + 1)) * (resumenCotizacion.ComisionExtPor + 1));
            //totalGeneralEquipoHerramienta = ((((totalGeneralEquipoHerramienta * (resumenCotizacion.IndirectoPor + 1)) * (resumenCotizacion.UtilidadPor + 1)) * (resumenCotizacion.CsvPor + 1)) * (resumenCotizacion.ComisionExtPor + 1));

            // GENERAR 3 PRESUPUESTOS, SOLO MATERIAL, SOLO HIGIENICOS Y HERRAMIENTA/EQUIPO
            if (totalMaterialesPuesto > 0)
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

        public async Task<bool> InsertarContratoCliente(byte[] contrato, int idClienteCreado, string nombreCliente)
        {
            bool result;

            //string pathContrato = "C:\\inetpub\\wwwroot\\SINGA_APP\\Doctos\\leg\\asuntos\\"; //RUTA PROD
            string pathContrato = "C:\\Users\\LAP_Sistemas5\\source\\repos\\SINGA_NEW\\Doctos\\Leg\\asuntos"; //RUTA DEV

            //INSERTAR ASUNTO
            string asuntoLegalXMLString = CrearXMLAsuntoLegal(idClienteCreado, nombreCliente);
            int idAsuntoCreado = clienteRepo.InsertarXMLAsuntoLegal(asuntoLegalXMLString);

            //CREAR CARPETA SI NO EXISTE
            Path.Combine(pathContrato, idAsuntoCreado.ToString());
            if (!Directory.Exists(pathContrato))
            {
                Directory.CreateDirectory(pathContrato);
            }

            //CREAR RUTA DEL ARCHIVO
            string nombreArchivo = "Contrato_" + nombreCliente + ".docx";
            var filePath = Path.Combine(pathContrato, nombreArchivo);

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
        public string CrearXMLPlantilla(int idPuntoAtencionCreado, PuestoDireccionCotizacionDTO puesto, int idServicio)
        {
            string fechaActual = DateTime.Today.ToString("yyyyMMdd");
            var plantillaXML = new XmlDocument();
            var plantillaElement = plantillaXML.CreateElement("plantilla");
            plantillaElement.SetAttribute("id", "0");
            plantillaElement.SetAttribute("idinm", idPuntoAtencionCreado.ToString());
            plantillaElement.SetAttribute("puesto", puesto.IdPuesto.ToString());
            plantillaElement.SetAttribute("cantidad", puesto.Cantidad.ToString());
            plantillaElement.SetAttribute("turno", ((int)puesto.IdTurno).ToString());
            plantillaElement.SetAttribute("jornal", puesto.Jornada.ToString());
            plantillaElement.SetAttribute("smntope", puesto.Sueldo.ToString());
            plantillaElement.SetAttribute("servicio", idServicio.ToString());
            plantillaElement.SetAttribute("uniforme", "0");
            plantillaElement.SetAttribute("formapago", "1");
            plantillaElement.SetAttribute("sexo", "3");
            plantillaElement.SetAttribute("movcant", puesto.Cantidad.ToString());
            plantillaElement.SetAttribute("movsuel", puesto.Sueldo.ToString());
            plantillaElement.SetAttribute("faplica", fechaActual);
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
    }
}
