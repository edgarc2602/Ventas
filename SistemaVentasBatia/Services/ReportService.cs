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
using System.Diagnostics.Contracts;
using System.Linq;
using Xceed.Document.NET;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Drawing;
using OfficeOpenXml.Style;
namespace SistemaVentasBatia.Services
{
    public interface IReportService
    {
        //Task<bool> CargarDirecciones(int idCotizacion, int idProspecto, IFormFile archivo);
        Task<byte[]> ObtenerContratoDOCX(int idCotizacion, ClienteContratoDTO contrato);
        Task<byte[]> DescargarProspectosCotizacionesDocx(int idEstatus, DateTime Finicio, DateTime Ffin);
        Task<byte[]> DescargarProspectosCotizacionesExcel(int idEstatus, DateTime Finicio, DateTime Ffin);
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
                    doc.ReplaceText("@P_CON_FECHA", FormatearFechaDateTime(empresa.ConstitutivaFecha.ToString()));
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

        public async Task<string> GetEstadoFormateado(int idEstado)
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

        [Obsolete]
        public async Task<byte[]> DescargarProspectosCotizacionesDocx(int idEstatus, DateTime Finicio, DateTime Ffin)
        {
            string rutaArchivo = System.IO.Path.Combine("Docs", "ReporteProspectos.docx");
            try
            {
                //obtener Prospectos y cotizaciones
                var prospectosCotizaciones = new List<Prospecto>();
                prospectosCotizaciones = await _repo.ObtenerListaProspectos(idEstatus, Finicio, Ffin);
                foreach (var pros in prospectosCotizaciones)
                {
                    pros.Cotizaciones = await _repo.ObtenerCotizacionesPorIdProspecto(idEstatus, pros.IdProspecto, Finicio, Ffin);
                }
                using (DocX doc = DocX.Load(rutaArchivo))
                {
                    // Tomar tabla
                    Table table = doc.Tables[0];
                    //obtener diseño
                    TableDesign originalDesign = table.Design;
                    table.Design = TableDesign.TableGrid;
                    int rowIndex = 0;
                    foreach (var prospecto in prospectosCotizaciones)
                    {
                        table.InsertRow();
                        // Encabezado del prospecto
                        table.Rows[rowIndex].Cells[0].Paragraphs.First().Append("ID Prospecto");
                        table.Rows[rowIndex].Cells[1].Paragraphs.First().Append("Nombre comercial");
                        table.Rows[rowIndex].Cells[2].Paragraphs.First().Append("Razón social");
                        table.Rows[rowIndex].Cells[3].Paragraphs.First().Append("Fecha alta");
                        table.Rows[rowIndex].Cells[4].Paragraphs.First().Append("Vendedor");
                        table.Rows[rowIndex].Cells[5].Paragraphs.First().Append("Estatus");
                        Color colorDeFondoProspecto = Color.FromArgb(191, 191, 191, 255);
                        AplicarNegritaACeldas(table, rowIndex, colorDeFondoProspecto);
                        rowIndex++;
                        table.InsertRow();
                        // Datos del prospecto
                        table.Rows[rowIndex].Cells[0].Paragraphs.First().Append(prospecto.IdProspecto.ToString());
                        table.Rows[rowIndex].Cells[1].Paragraphs.First().Append(prospecto.NombreComercial.ToString());
                        table.Rows[rowIndex].Cells[2].Paragraphs.First().Append(prospecto.RazonSocial.ToString());
                        table.Rows[rowIndex].Cells[3].Paragraphs.First().Append(prospecto.FechaAlta.ToString("dd-MM-yyyy"));
                        table.Rows[rowIndex].Cells[4].Paragraphs.First().Append(prospecto.RepresentanteLegal.ToString());
                        table.Rows[rowIndex].Cells[5].Paragraphs.First().Append(prospecto.IdEstatusProspecto.ToString());
                        rowIndex++;
                        table.InsertRow();
                        // Encabezado de cotización
                        table.Rows[rowIndex].Cells[0].Paragraphs.First().Append("ID Cotización");
                        table.Rows[rowIndex].Cells[1].Paragraphs.First().Append("Servicio");
                        table.Rows[rowIndex].Cells[2].Paragraphs.First().Append("Vigencia");
                        table.Rows[rowIndex].Cells[3].Paragraphs.First().Append("Fecha Alta");
                        table.Rows[rowIndex].Cells[4].Paragraphs.First().Append("Total");
                        table.Rows[rowIndex].Cells[5].Paragraphs.First().Append("Estatus");
                        Color colorDeFondoCotizacion = Color.FromArgb(245, 245, 245, 255);
                        AplicarNegritaACeldas(table, rowIndex, colorDeFondoCotizacion);
                        rowIndex++;
                        foreach (var cotizacion in prospecto.Cotizaciones)
                        {
                            table.InsertRow();
                            // Datos de cotización
                            table.Rows[rowIndex].Cells[0].Paragraphs.First().Append(cotizacion.IdCotizacion.ToString());
                            table.Rows[rowIndex].Cells[1].Paragraphs.First().Append(cotizacion.IdServicio.ToString());
                            table.Rows[rowIndex].Cells[2].Paragraphs.First().Append(cotizacion.DiasVigencia.ToString());
                            table.Rows[rowIndex].Cells[3].Paragraphs.First().Append(cotizacion.FechaAlta.ToString("dd-MM-yyyy"));
                            table.Rows[rowIndex].Cells[4].Paragraphs.First().Append(cotizacion.Total.ToString());
                            table.Rows[rowIndex].Cells[5].Paragraphs.First().Append(cotizacion.IdEstatusCotizacion.ToString());
                            rowIndex++;
                        }
                    }
                    doc.InsertTable(table);

                    table.Design = originalDesign;
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
        void AplicarNegritaACeldas(Table table, int rowIndex, Color colorDeFondo)
        {
            foreach (Cell cell in table.Rows[rowIndex].Cells)
            {
                Xceed.Document.NET.Paragraph paragraph = cell.Paragraphs.First();
                paragraph.Bold();
                cell.FillColor = colorDeFondo;
            }
        }

        public async Task<byte[]> DescargarProspectosCotizacionesExcel(int idEstatus, DateTime Finicio, DateTime Ffin)
        {
            string rutaArchivo = Path.Combine("Docs", "ReporteProspectosCotizaciones.xlsx");

            using (var stream = new MemoryStream(File.ReadAllBytes(rutaArchivo)))
            {
                using (var package = new ExcelPackage(stream))
                {
                    var prospectosCotizaciones = new List<Prospecto>();
                    prospectosCotizaciones = await _repo.ObtenerListaProspectos(idEstatus, Finicio, Ffin);
                    foreach (var pros in prospectosCotizaciones)
                    {
                        pros.Cotizaciones = await _repo.ObtenerCotizacionesPorIdProspecto(idEstatus, pros.IdProspecto, Finicio, Ffin);
                    }
                    if(prospectosCotizaciones != null)
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        string prospectotext = "";
                        int rowIndex = 3;
                        foreach (var prospecto in prospectosCotizaciones)
                        {
                            
                            if(prospecto.RepresentanteLegal != prospectotext){
                                worksheet.Cells[rowIndex, 1].Value = prospecto.RepresentanteLegal;
                                prospectotext = prospecto.RepresentanteLegal;
                                AgregarBordeSuperior(rowIndex, worksheet);
                            }
                            // Datos de cotizaciones
                            int i = 0;
                            foreach (var cotizacion in prospecto.Cotizaciones)
                            {
                                if(i == 0)
                                {
                                    worksheet.Cells[rowIndex, 2].Value = prospecto.NombreComercial;
                                    worksheet.Cells[rowIndex, 3].Value = prospecto.RazonSocial;
                                    i = 1;
                                }
                                worksheet.Cells[rowIndex, 4].Value = cotizacion.IdCotizacion;
                                worksheet.Cells[rowIndex, 5].Value = cotizacion.IdServicio;
                                worksheet.Cells[rowIndex, 6].Value = cotizacion.DiasVigencia;
                                worksheet.Cells[rowIndex, 7].Value = cotizacion.FechaAlta.ToString("dd-MM-yyyy");
                                worksheet.Cells[rowIndex, 8].Value = cotizacion.Total;
                                worksheet.Cells[rowIndex, 9].Value = cotizacion.IdEstatusCotizacion;
                                rowIndex++;
                            }
                            //// Encabezado del prospecto
                            //worksheet.Cells[rowIndex, 1].Value = "ID Prospecto";
                            //worksheet.Cells[rowIndex, 2].Value = "Nombre comercial";
                            //worksheet.Cells[rowIndex, 3].Value = "Razón social";
                            //worksheet.Cells[rowIndex, 4].Value = "Fecha alta";
                            //worksheet.Cells[rowIndex, 5].Value = "Vendedor";
                            //worksheet.Cells[rowIndex, 6].Value = "Estatus";

                            //// Aplicar estilo al encabezado del prospecto
                            //worksheet.Cells[rowIndex, 1, rowIndex, 6].Style.Font.Bold = true;
                            //worksheet.Cells[rowIndex, 1, rowIndex, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            //worksheet.Cells[rowIndex, 1, rowIndex, 6].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(191, 191, 191, 255));

                            //rowIndex++;

                            //// Datos del prospecto
                            //worksheet.Cells[rowIndex, 1].Value = prospecto.IdProspecto;
                            //worksheet.Cells[rowIndex, 2].Value = prospecto.NombreComercial;
                            //worksheet.Cells[rowIndex, 3].Value = prospecto.RazonSocial;
                            //worksheet.Cells[rowIndex, 4].Value = prospecto.FechaAlta.ToString("dd-MM-yyyy");
                            //worksheet.Cells[rowIndex, 5].Value = prospecto.RepresentanteLegal;
                            //worksheet.Cells[rowIndex, 6].Value = prospecto.IdEstatusProspecto;

                            //rowIndex++;

                            //// Encabezado de cotización
                            //worksheet.Cells[rowIndex, 1].Value = "ID Cotización";
                            //worksheet.Cells[rowIndex, 2].Value = "Servicio";
                            //worksheet.Cells[rowIndex, 3].Value = "Vigencia";
                            //worksheet.Cells[rowIndex, 4].Value = "Fecha Alta";
                            //worksheet.Cells[rowIndex, 5].Value = "Total";
                            //worksheet.Cells[rowIndex, 6].Value = "Estatus";

                            //// Aplicar estilo al encabezado de cotización
                            //worksheet.Cells[rowIndex, 1, rowIndex, 6].Style.Font.Bold = true;
                            //worksheet.Cells[rowIndex, 1, rowIndex, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            //worksheet.Cells[rowIndex, 1, rowIndex, 6].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 245, 245, 255));

                            //rowIndex++;

                            //// Datos de cotizaciones
                            //foreach (var cotizacion in prospecto.Cotizaciones)
                            //{
                            //    worksheet.Cells[rowIndex, 1].Value = cotizacion.IdCotizacion;
                            //    worksheet.Cells[rowIndex, 2].Value = cotizacion.IdServicio;
                            //    worksheet.Cells[rowIndex, 3].Value = cotizacion.DiasVigencia;
                            //    worksheet.Cells[rowIndex, 4].Value = cotizacion.FechaAlta.ToString("dd-MM-yyyy");
                            //    worksheet.Cells[rowIndex, 5].Value = cotizacion.Total;
                            //    worksheet.Cells[rowIndex, 6].Value = cotizacion.IdEstatusCotizacion;

                            //    rowIndex++;
                            //}
                        }
                    }
                    return package.GetAsByteArray();
                }
            }
        }

        public void AgregarBordeSuperior(int rowIndex, ExcelWorksheet worksheet)
        {
            // Comprobamos si el índice de fila es válido
            if (rowIndex <= worksheet.Dimension.End.Row)
            {
                // Iteramos sobre las 8 celdas
                for (int columnIndex = 1; columnIndex <= 9; columnIndex++)
                {
                    // Obtener la celda
                    var celda = worksheet.Cells[rowIndex, columnIndex];

                    // Establecer el borde superior
                    celda.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                }
            }
        }
    }
}
