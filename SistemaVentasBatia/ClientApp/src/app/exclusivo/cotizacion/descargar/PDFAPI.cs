using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TuProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ReportController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("descargar-report")]
        public async Task<IActionResult> DescargarReporte(string reg)
        {
            try
            {
                // URL del informe SSRS
                var reportServerUrl = "http://192.168.2.4/ReportServer";
                var reportPath = "/reportesupervision";
                var format = "PDF";
                var parameter1 = reg;

                // Construye la URL completa
                var url = $"{reportServerUrl}/report?%2f{reportPath}&rs:Format={format}&Parameter1={parameter1}";

                // Configura el cliente HTTP
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("Administrador:GrupoBatia@"))
                );

                // Realiza la solicitud HTTP
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var contentStream = await response.Content.ReadAsStreamAsync();

                    // Guarda el informe en el dispositivo
                    var filePath = Path.Combine("ruta/donde/guardar", "nombre_del_archivo.pdf");
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await contentStream.CopyToAsync(fileStream);
                    }

                    return File(filePath, "application/pdf");
                }
                else
                {
                    return StatusCode((int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
