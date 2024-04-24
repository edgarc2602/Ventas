using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using OfficeOpenXml.Drawing.Chart;
using SistemaVentasBatia.Enums;
using Turno = SistemaVentasBatia.Enums.Turno;
using System.Data.Common;
using Xceed.Words.NET;
using iTextSharp.text.log;
using iTextSharp.text.pdf;
using iTextSharp.text;
namespace SistemaVentasBatia.Services
{
    public interface IReportService
    {
        //Task<bool> CargarDirecciones(int idCotizacion, int idProspecto, IFormFile archivo);
        Task<byte[]> ObtenerContratoDOCX(int idCotizacion, ClienteContratoDTO contrato);
    }
    public class ReportService : IReportService
    {
        private readonly ICotizacionesService _logicCot;
        private readonly ISalarioService _logicSal;
        private readonly IProspectosService _prosService;
        private readonly IReportRepository _repo;
        private readonly IMapper _mapper;

        public ReportService(IReportRepository repoReport, IMapper mapper, IProspectosService prospectosService, ISalarioService logicSal, ICotizacionesService logicCot)
        {
            _logicCot = logicCot;
            _logicSal = logicSal;
            _prosService = prospectosService;
            _repo = repoReport;
            _mapper = mapper;
            _logicSal = logicSal;
        }

        [Obsolete]
        public async Task<byte[]> ObtenerContratoDOCX(int idCotizacion, ClienteContratoDTO contrato)
        {
            string rutaArchivo = System.IO.Path.Combine("Docs", "LIMPIEZASERV.IND.BATIA.docx");
            string fechaActual = DateTime.Now.ToString("yyyy-MM-dd");
            try
            {
                decimal total = await _repo.ObtenerTotalCotizacion(idCotizacion);
                var empresa = _mapper.Map<EmpresaDTO>(await _repo.ObtenerEmpresa(contrato.IdEmpresa));
                DateTime fecha = DateTime.ParseExact(contrato.PoderFecha, "yyyy-MM-dd", null);
                using (DocX doc = DocX.Load(rutaArchivo))
                {
                    doc.ReplaceText("@C_RAZONSOCIAL", contrato.ClienteRazonSocial);
                    doc.ReplaceText("@C_REPRESENTANTE", contrato.ClienteRepresentante);
                    doc.ReplaceText("@P_RAZONSOCIAL", empresa.Nombre);
                    doc.ReplaceText("@P_REPRESENTANTE", empresa.Representante);
                    //Cliente Constitutiva
                    doc.ReplaceText("@C_CON_ESCRITURA", contrato.ConstitutivaEscrituraPublica);
                    doc.ReplaceText("@C_CON_FECHA", contrato.ConstitutivaFecha = FormatearFecha(contrato.ConstitutivaFecha));
                    doc.ReplaceText("@C_CON_LIC", contrato.ConstitutivaLicenciado);
                    doc.ReplaceText("@C_CON_NUMNOTARIO", contrato.ConstitutivaNumeroNotario);
                    doc.ReplaceText("@C_CON_FOLIOM", contrato.ConstitutivaFolioMercantil);
                    doc.ReplaceText("@C_CON_ESTADO", await GetEstadoFormateado(contrato.ConstitutivaIdEstado));
                    //Cliente Poder
                    doc.ReplaceText("@C_POD_ESCRITURA", contrato.PoderEscrituraPublica);
                    doc.ReplaceText("@C_POD_FECHA", contrato.PoderFecha = FormatearFecha(contrato.PoderFecha));
                    doc.ReplaceText("@C_POD_LIC", contrato.PoderLicenciado);
                    doc.ReplaceText("@C_POD_NUMNOTARIO", contrato.PoderNumeroNotario);
                    doc.ReplaceText("@C_POD_ESTADO", await GetEstadoFormateado(contrato.PoderIdEstado));
                    //PrestadorConstitutiva
                    doc.ReplaceText("@P_CON_ESCRITURA", empresa.ConstitutivaEscrituraPublica);
                    doc.ReplaceText("@P_CON_FECHA",  FormatearFechaDateTime(empresa.ConstitutivaFecha.ToString()));
                    doc.ReplaceText("@P_CON_LIC", empresa.ConstitutivaLicenciado);
                    doc.ReplaceText("@P_CON_NUMNOTARIO", empresa.ConstitutivaNumeroNotario);
                    doc.ReplaceText("@P_CON_FOLIOM", empresa.ConstitutivaFolioMercantil);
                    doc.ReplaceText("@P_CON_ESTADO", await GetEstadoFormateado(empresa.ConstitutivaIdEstado));
                    //Prestador Poder
                    doc.ReplaceText("@P_POD_ESCRITURA", empresa.PoderEscrituraPublica);
                    doc.ReplaceText("@P_POD_FECHA", FormatearFechaDateTime(empresa.PoderFecha.ToString()));
                    doc.ReplaceText("@P_POD_LIC", empresa.PoderLicenciado);
                    doc.ReplaceText("@P_POD_NUMNOTARIO", empresa.PoderNumeroNotario);
                    doc.ReplaceText("@P_POD_ESTADO", await GetEstadoFormateado(empresa.PoderIdEstado));
                    //Cliente Direccion
                    doc.ReplaceText("@C_RFC", contrato.ClienteRfc);
                    doc.ReplaceText("@C_ESTADO", await _repo.ObtenerEstadoByIdEstado(contrato.ClienteEstado));
                    doc.ReplaceText("@C_MUNICIPIO", await _repo.ObtenerMunicipioByIdMunicipio(contrato.ClienteMunicipio));
                    doc.ReplaceText("@C_COLONIA", contrato.ClienteColoniaDescripcion);
                    doc.ReplaceText("@C_CP", contrato.CP);
                    //Prestador Direccion
                    doc.ReplaceText("@P_RFC", empresa.Rfc);
                    doc.ReplaceText("@P_ESTADO", await _repo.ObtenerEstadoByIdEstado(empresa.IdEstado));
                    doc.ReplaceText("@P_MUNICIPIO", empresa.Municipio);
                    doc.ReplaceText("@P_COLONIA", empresa.Colonia);
                    doc.ReplaceText("@P_CP", empresa.CP);
                    //Contrato datos
                    doc.ReplaceText("@P_FOLIOVIGENTE", empresa.FolioVigente);
                    doc.ReplaceText("@TOTAL", total.ToString("N2"));
                    doc.ReplaceText("@VIGENCIA", contrato.ContratoVigencia = FormatearFecha(contrato.ContratoVigencia));
                    doc.ReplaceText("@FECHAFIRMA", FormatearFecha(fechaActual));
                    //Poliza
                    doc.ReplaceText("@POLIZA_MONTO", contrato.PolizaMonto.ToString("N2"));
                    doc.ReplaceText("@POLIZA_LETRA", contrato.PolizaMontoLetra);
                    doc.ReplaceText("@POLIZA_EMPRESA", contrato.PolizaEmpresa);
                    doc.ReplaceText("@POLIZA_NUMERO", contrato.PolizaNumero);
                    //Contactos
                    doc.ReplaceText("@C_CONTACTONOMBRE", contrato.ClienteContactoNombre);
                    doc.ReplaceText("@P_CONTACTONOMBRE", contrato.EmpresaContactoNombre);
                    doc.ReplaceText("@C_EMAIL", contrato.ClienteEmail);
                    doc.ReplaceText("@P_EMAIL", contrato.EmpresaContactoCorreo);
                    doc.ReplaceText("@C_TELEFONO", contrato.ClienteContactoTelefono);
                    doc.ReplaceText("@P_TELEFONO", contrato.EmpresaContactoTelefono);
                    

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        doc.SaveAs(memoryStream);
                        return memoryStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al generar el contrato Word: " + ex.Message);
                throw;
            }
        }

        static string FormatearFecha(string fechaStr)
        {
            DateTime fecha = DateTime.ParseExact(fechaStr, "yyyy-MM-dd", null);
            string fechaFormateada = fecha.ToString("d 'de' MMMM 'del' yyyy");
            return fechaFormateada;
        }

        static string FormatearFechaDateTime(string fechaStr)
        {
            DateTime fecha = DateTime.ParseExact(fechaStr, "dd/MM/yyyy hh:mm:ss tt", null);
            string fechaFormateada = fecha.ToString("d 'de' MMMM 'del' yyyy");
            return fechaFormateada;
        }

        public async Task <string> GetEstadoFormateado(int idEstado)
        {
            string estadoFormat;
            string estado;
            estado = await _repo.ObtenerEstadoByIdEstado(idEstado);
            if (idEstado == 7)
            {
                estadoFormat = "de la " + estado;
            }
            else if (idEstado == 11)
            {
                estadoFormat = "del " + estado;
            }
            else
            {
                estadoFormat = "de " + estado;
            }
            return estadoFormat;
        }
    }
}
