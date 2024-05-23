using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using iTextSharp.text.pdf.parser;
using Xceed.Words.NET;
using SistemaVentasBatia.Services;
using SistemaVentasBatia.DTOs;
using Microsoft.Extensions.Logging;
using iTextSharp.text.log;

namespace SistemaVentasBatia.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IClienteService _clienteService;
        public ReportController(IReportService reportService, IClienteService clienteService)
        {
            this._reportService = reportService;
            _clienteService = clienteService;
        }
        [HttpPost("[action]/{tipo}")]
        public IActionResult DescargarReporteCotizacion([FromBody] int idCotizacion, int tipo = 0)
        {
            if (tipo == 1)
            {
                try
                {
                    var url = ("http://192.168.2.4/Reporte?%2freportecotizacion&rs:Format=PDF&idCotizacion=" + idCotizacion.ToString());
                    WebClient wc = new WebClient
                    {
                        Credentials = new NetworkCredential("Administrador", "GrupoBatia@")
                    };
                    byte[] myDataBuffer = wc.DownloadData(url.ToString());

                    return new FileContentResult(myDataBuffer, "application/pdf")
                    {
                        FileDownloadName = "PropuestaTecnica"
                    };

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener el archivo PDF: {ex.Message}");
                    return StatusCode(500, "Error al obtener el archivo PDF");
                }
            }
            if (tipo == 2)
            {
                try
                {
                    var url = ("http://192.168.2.4/Reporte?%2freportecotizacion2&rs:Format=PDF&idCotizacion=" + idCotizacion.ToString());
                    WebClient wc = new WebClient
                    {
                        Credentials = new NetworkCredential("Administrador", "GrupoBatia@")
                    };
                    byte[] myDataBuffer = wc.DownloadData(url.ToString());

                    return new FileContentResult(myDataBuffer, "application/pdf")
                    {
                        FileDownloadName = "PropuestaEconomica.pdf"
                    };

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener el archivo PDF: {ex.Message}");
                    return StatusCode(500, "Error al obtener el archivo PDF");
                }
            }
            if (tipo == 3)
            {
                try
                {
                    var url = ("http://192.168.2.4/Reporte?%2freportecotizacionmanodeobra&rs:Format=PDF&idCotizacion=" + idCotizacion.ToString());
                    WebClient wc = new WebClient
                    {
                        Credentials = new NetworkCredential("Administrador", "GrupoBatia@")
                    };
                    byte[] myDataBuffer = wc.DownloadData(url.ToString());

                    return new FileContentResult(myDataBuffer, "application/pdf")
                    {
                        FileDownloadName = "PropuestaTecnicaManoDeObra"
                    };

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener el archivo PDF: {ex.Message}");
                    return StatusCode(500, "Error al obtener el archivo PDF");
                }
            }
            //if (tipo == 3)
            //{
            //    try
            //    {
            //        var url = ("http://192.168.2.4/Reporte?%2freportecotizacion&rs:Format=DOCX&idCotizacion=" + idCotizacion.ToString()); // Cambia rs:Format a DOCX
            //        WebClient wc = new WebClient
            //        {
            //            Credentials = new NetworkCredential("Administrador", "GrupoBatia@")
            //        };
            //        byte[] myDataBuffer = wc.DownloadData(url.ToString());

            //        return new FileContentResult(myDataBuffer, "application/docx") // Cambia el tipo MIME a application/docx
            //        {
            //            FileDownloadName = "PropuestaTecnica.docx" // Cambia el nombre del archivo a .docx
            //        };
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Error al obtener el archivo DOCX: {ex.Message}");
            //        return StatusCode(500, "Error al obtener el archivo DOCX");
            //    }
            //}
            //if (tipo == 4)
            //{
            //    try
            //    {
            //        var url = ("http://192.168.2.4/Reporte?%2freportecotizacion2&rs:Format=DOCX&idCotizacion=" + idCotizacion.ToString()); // Cambia rs:Format a DOCX
            //        WebClient wc = new WebClient
            //        {
            //            Credentials = new NetworkCredential("Administrador", "GrupoBatia@")
            //        };
            //        byte[] myDataBuffer = wc.DownloadData(url.ToString());

            //        return new FileContentResult(myDataBuffer, "application/docx") // Cambia el tipo MIME a application/docx
            //        {
            //            FileDownloadName = "PropuestaEconomica.docx" // Cambia el nombre del archivo a .docx
            //        };
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Error al obtener el archivo DOCX: {ex.Message}");
            //        return StatusCode(500, "Error al obtener el archivo DOCX");
            //    }
            //}
            else
            {
                throw new Exception();
            }
        }

        [HttpGet("[action]/{idEstado}/{idFamilia}")]
        public IActionResult DescargarListaProductosPorEstado(int idEstado = 0, int idFamilia = 0)
        {
            try
            {
                var url = ("http://192.168.2.4/Reporte?%2freporteproductoestado&rs:Format=PDF&idEstado=" + idEstado.ToString() + "&idFamilia=" + idFamilia.ToString());
                WebClient wc = new WebClient
                {
                    Credentials = new NetworkCredential("Administrador", "GrupoBatia@")
                };
                byte[] myDataBuffer = wc.DownloadData(url.ToString());
                return new FileContentResult(myDataBuffer, "application/pdf")
                {
                    FileDownloadName = "Productos.pdf"
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el archivo PDF: {ex.Message}");
                return StatusCode(500, "Error al obtener el archivo PDF");
            }
        }

        [HttpPost("[action]/{idCotizacion}/{idClienteGenerado}")]
        public async Task<IActionResult> GenerarYDescargarContratoBaseCliente(int idCotizacion, int idClienteGenerado, ClienteContratoDTO contrato)
        {
            try
            {
                //GENERAR CONTRATO
                byte[] contratoBytes = await _reportService.ObtenerContratoDOCX(idCotizacion, contrato);
                //INSERTAR CONTRATO 
                bool result = _clienteService.InsertarContratoCliente(contratoBytes, idClienteGenerado, contrato.ClienteRazonSocial);
                //REGRESAR COPIA DE CONTRATO AL USUARIO
                return File(contratoBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "LIMPIEZASERV.IND.BATIA_actualizado.docx");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al descargar el contrato Word: " + ex.Message);
                return StatusCode(500, "Error al descargar el contrato Word");
            }
        }

        [HttpGet("[action]/{idEstatus}")]
        public IActionResult DescargarReporteProspectos(int idEstatus)
        {

            try
            {
                var url = ("http://192.168.2.4/Reporte?%2freporteprospectos&rs:Format=PDF&idEstatus=" + idEstatus.ToString());
                WebClient wc = new WebClient
                {
                    Credentials = new NetworkCredential("Administrador", "GrupoBatia@")
                };
                byte[] myDataBuffer = wc.DownloadData(url.ToString());

                return new FileContentResult(myDataBuffer, "application/pdf")
                {
                    FileDownloadName = "ReporteProspectos"
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el archivo PDF: {ex.Message}");
                return StatusCode(500, "Error al obtener el archivo PDF");
            }

        }

        [HttpGet("[action]/{idEstatus}/{Finicio}/{Ffin}")]
        public async Task<IActionResult> DescargarProspectosCotizacionesDocx(int idEstatus, DateTime Finicio, DateTime Ffin)
        {
            try
            {
                byte[] contratoBytes = await _reportService.DescargarProspectosCotizacionesDocx(idEstatus, Finicio, Ffin);

                return File(contratoBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "LIMPIEZASERV.IND.BATIA_actualizado.docx");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al descargar el contrato Word: " + ex.Message);
                return StatusCode(500, "Error al descargar el contrato Word");
            }
        }

        [HttpPost("[action]/{idEstatus}/{Finicio}/{Ffin}")]
        public async Task<FileContentResult> DescargarProspectosCotizacionesExcel(int idEstatus, DateTime Finicio, DateTime Ffin)
        {
            byte[] fileContents = await _reportService.DescargarProspectosCotizacionesExcel(idEstatus, Finicio, Ffin);

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "archivo.xlsx");
        }
    }
}
