using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SistemaVentasBatia.Services;
using SistemaVentasBatia.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;
using SistemaVentasBatia.Models;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SistemaVentasBatia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CotizacionController : ControllerBase
    {
        private readonly ILogger<CotizacionController> _logger;
        private readonly ICotizacionesService cotizacionesSvc;
        private readonly IProspectosService prospectosSvc;
        private readonly ICatalogosService catalogosSvc;

        public CotizacionController(ILogger<CotizacionController> logger, ICotizacionesService cotizacionesSvc, IProspectosService prospectosSvc, ICatalogosService catalogosSvc)
        {
            _logger = logger;
            this.cotizacionesSvc = cotizacionesSvc;
            this.prospectosSvc = prospectosSvc;
            this.catalogosSvc = catalogosSvc;
        }

        [HttpGet("{idPersonal}/{pagina}")]
        public async Task<ActionResult<ListaCotizacionDTO>> Index(int idProspecto, EstatusCotizacion estatus, int servicio, int idPersonal = 0, int pagina = 1)
        {
            var listaCotizacionesVM = new ListaCotizacionDTO();
            listaCotizacionesVM.Pagina = pagina;
            listaCotizacionesVM.IdEstatusCotizacion = estatus;
            listaCotizacionesVM.IdServicio = servicio;
            listaCotizacionesVM.IdProspecto = idProspecto;
            int autorizacion = await cotizacionesSvc.ObtenerAutorizacion(idPersonal);
            await cotizacionesSvc.ObtenerListaCotizaciones(listaCotizacionesVM, autorizacion, idPersonal);
            return listaCotizacionesVM;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<CotizacionDTO>> NuevoProspecto([FromBody] ProspectoDTO prospectoVM)
        {
            await prospectosSvc.CrearProspecto(prospectoVM);
            var cotizacionVM = new CotizacionDTO { IdProspecto = prospectoVM.IdProspecto };
            return cotizacionVM;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> SeleccionarProspecto([FromBody] CotizacionDTO cotizacionVM)
        {
            foreach (var servicio in cotizacionVM.ListaTipoSalarios)
            {
                if (servicio.Act)
                {   
                    cotizacionVM.SalTipo = (SalarioTipo)servicio.Id;
                }
            }
            foreach (var servicio in cotizacionVM.ListaServicios)
            {
                if (servicio.Act)
                {
                    cotizacionVM.IdServicio = (Servicio)servicio.Id;
                    await cotizacionesSvc.CrearCotizacion(cotizacionVM);
                }
            }
            // TempData["DescripcionAlerta"] = "Se crearon correctamente las cotizaciónes";
            // TempData["IdTipoAlerta"] = TipoAlerta.Exito;
            // HttpContext.Session.SetString("ListaCotizacionesViewModel", JsonSerializer.Serialize(new ListaCotizacionDTO { IdProspecto = cotizacionVM.IdProspecto }));
            return true;
        }

        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<ResumenCotizacionLimpiezaDTO>> LimpiezaResumen(int id)
        {
            var resumen = await cotizacionesSvc.ObtenerResumenCotizacionLimpieza(id);

            return resumen;
        }

        [HttpGet]
        public async Task<ActionResult<ProspectoDTO>> LimpiezaInfoProspecto(int id)
        {
            var prospecto = await prospectosSvc.ObtenerProspectoPorCotizacion(id);

            prospecto.IdCotizacion = id;

            // TempData["IdCotizacion"] = id;
            // TempData["Action"] = "LimpiezaInfoProspecto";

            return prospecto;
        }

        [HttpGet("[action]/{id}/{pagina}")]
        public async Task<ActionResult<ListaDireccionDTO>> LimpiezaDirectorio(int id, int pagina = 0)
        {
            var listaDireccionesVM = new ListaDireccionDTO();

            listaDireccionesVM.IdCotizacion = id;

            listaDireccionesVM.Pagina = pagina;

            await cotizacionesSvc.ObtenerListaDireccionesPorCotizacion(listaDireccionesVM);

            return listaDireccionesVM;
        }

        [HttpGet("{id}/{idDireccionCotizacion}/{idPuestoDireccionCotizacion}")]
        public async Task<ActionResult<ListaPuestosDireccionCotizacionDTO>> LimpiezaPlantilla(int id, int idDireccionCotizacion = 0, int idPuestoDireccionCotizacion = 0)
        {
            var listaPuestosDireccionCotizacionVM = new ListaPuestosDireccionCotizacionDTO { IdCotizacion = id, IdDireccionCotizacion = idDireccionCotizacion, IdPuestoDireccionCotizacion = idPuestoDireccionCotizacion };

            await cotizacionesSvc.ObtenerListaPuestosPorCotizacion(listaPuestosDireccionCotizacionVM);

            await cotizacionesSvc.ObtenerCatalogoDireccionesPorCotizacion(listaPuestosDireccionCotizacionVM);

            // TempData["Turnos"] = new List<SelectListItem>((await catalogosSvc.ObtenerCatalogoTurnos()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Descripcion }));

            // TempData["Puestos"] = new List<SelectListItem>((await catalogosSvc.ObtenerCatalogoPuestos()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Descripcion }));

            // TempData["IdCotizacion"] = id;
            // TempData["Action"] = "LimpiezaPlantilla";


            var empleados = 0;
            foreach (var cant in listaPuestosDireccionCotizacionVM.PuestosDireccionesCotizacion)
            {

                empleados += cant.Cantidad;
            }

            listaPuestosDireccionCotizacionVM.Empleados = empleados;

            return listaPuestosDireccionCotizacionVM;
        }

        [HttpPost("[action]")]
        public async Task<bool> ActualizarIndirectoUtilidadService([FromBody] Cotizacionupd cotizacionupd)
        {
            return await cotizacionesSvc.ActualizarIndirectoUtilidad(cotizacionupd.IdCotizacion, cotizacionupd.Indirecto, cotizacionupd.Utilidad, cotizacionupd.ComisionSV, cotizacionupd.ComisionExt);
            //return RedirectToAction("LimpiezaResumen");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<DireccionCotizacionDTO>> AgregarDireccion([FromBody] DireccionCotizacionDTO direccionCVM)
        {
            var listaPuestosDireccionCotizacionVM = new ListaPuestosDireccionCotizacionDTO { IdCotizacion = direccionCVM.IdCotizacion };
            await cotizacionesSvc.ObtenerCatalogoDireccionesPorCotizacion(listaPuestosDireccionCotizacionVM);

            foreach (var direccion in listaPuestosDireccionCotizacionVM.DireccionesCotizacion)
            {
                if (direccionCVM.IdDireccion == direccion.IdDireccion)
                {
                    throw new CustomException("La dirección ya esta registrada en la cotización actual");
                    //return direccionCVM;
                    // TempData["DescripcionAlerta"] = "La dirección ya esta registrada en la cotización actual.";
                    // TempData["IdTipoAlerta"] = TipoAlerta.False;
                }
            }
            await cotizacionesSvc.AgregarDireccionCotizacion(direccionCVM);

            // TempData["DescripcionAlerta"] = "Se agregó correctamente la dirección.";
            // TempData["IdTipoAlerta"] = TipoAlerta.Exito;

            return direccionCVM;
        }

        [HttpPost("[action]")]
        public async Task<bool> EliminarCotizacion([FromBody] int idCotizacion)
        {
             return await cotizacionesSvc.EliminarCotizacion(idCotizacion);
        }

        [HttpGet("[action]/{idDC}")]
        public async Task<bool> EliminarDireccionCotizacion( int idDC)
        {
            var idCotizacion = await cotizacionesSvc.ObtenerIdCotizacionPorDireccion(idDC);

            await cotizacionesSvc.EliminarDireccionCotizacion(idDC);

            // TempData["DescripcionAlerta"] = "Se quitó correctamente la dirección de la cotización";
            // TempData["IdTipoAlerta"] = TipoAlerta.Info;

            return true;
        }

        [HttpPut]
        public async Task<IActionResult> EditarProspecto([FromBody] ProspectoDTO prospectoVM)
        {
            if (ModelState.IsValid)
            {
                await prospectosSvc.EditarProspecto(prospectoVM);

                // TempData["DescripcionAlerta"] = "Se guardó correctamente el prospecto";
                // TempData["IdTipoAlerta"] = TipoAlerta.Exito;
            }
            else
            {
                // TempData["DescripcionAlerta"] = "Por favor revisa los errores en el formulario.";
                // TempData["IdTipoAlerta"] = TipoAlerta.Error;
            }

            return RedirectToAction("LimpiezaInfoProspecto", new { id = prospectoVM.IdCotizacion });
        }

        [HttpPost("[action]")]
        public async Task<int> DuplicarCotizacion([FromBody] int idCotizacion)
        {
            var idNuevaCotizacion = await cotizacionesSvc.DuplicarCotizacion(idCotizacion);

            // TempData["DescripcionAlerta"] = "Se duplicó correctamente la cotización";
            // TempData["IdTipoAlerta"] = TipoAlerta.Exito;

            return idNuevaCotizacion;
        }

        [HttpGet("[action]")]
        public IEnumerable<Item<int>> GetEstatus()
        {
            List<Item<int>> ls = Enum.GetValues(typeof(EstatusCotizacion))
                .Cast<EstatusCotizacion>().Select(s => new Item<int>
                {
                    Id = (int)s,
                    Nom = s.ToString(),
                    Act = false
                }).ToList();

            return ls;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<PuestoTabulador>> ActualizarSalarios([FromBody] PuestoTabulador salarios)
        {
            await cotizacionesSvc.ActualizarSalarios(salarios);
            return salarios;

        }

        [HttpGet("[action]/{idCotizacion}/{idServicio}")]
        public async Task<ActionResult<bool>> ActualizarCotizacion(int idCotizacion, int idServicio)
        {
             return await cotizacionesSvc.ActualizarCotizacion(idCotizacion, idServicio);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<CotizaPorcentajes>> ObtenerPorcentajesCotizacion()
        {
            CotizaPorcentajes porcentajes = new CotizaPorcentajes();
            porcentajes = await cotizacionesSvc.ObtenerPorcentajesCotizacion();
            return porcentajes;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<bool>> ActualizarPorcentajesPredeterminadosCotizacion([FromBody]CotizaPorcentajes porcentajes)
        {
            await cotizacionesSvc.ActualizarPorcentajesPredeterminadosCotizacion(porcentajes);
                return true;
        }

        [HttpGet("[action]/{idPersonal}")]
        public async Task<ActionResult<int>> ObtenerAutorizacion(int idPersonal = 0)
        {
            int autorizacion = await cotizacionesSvc.ObtenerAutorizacion(idPersonal);
            return autorizacion;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<decimal>> ObtenerImssBase()
        {
            return await cotizacionesSvc.ObtenerImssBase();
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<bool>> ActualizarImssBase([FromBody] decimal imss)
        {
            await cotizacionesSvc.ActualizarImssBase(imss);
            return true;
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<bool>> ActivarCotizacion([FromBody] int idCotizacion)
        {
            return await cotizacionesSvc.ActivarCotizacion(idCotizacion);
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<bool>> DesactivarCotizacion([FromBody] int idCotizacion)
        {
            return await cotizacionesSvc.DesactivarCotizacion(idCotizacion);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<ImmsJornadaDTO>> ObtenerImssJornada()
        {
            return await cotizacionesSvc.ObtenerImssJornada();
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<bool>> ActualizarImssJornada(ImmsJornadaDTO imssJormada)
        {
            return await cotizacionesSvc.ActualizarImssJornada(imssJormada);
        }

        [HttpGet("[action]/{idVendedor}")]
        public async Task<ActionResult<CotizacionVendedorDetalleDTO>> CotizacionVendedorDetallePorIdVendedor(int idVendedor)
        {
            return await cotizacionesSvc.ObtenerCotizacionVendedorDetallePorIdVendedor(idVendedor);
        }
    }
}
