using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Enums;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CotizacionController : ControllerBase
    {
        private readonly ILogger<CotizacionController> _logger;
        private readonly ICotizacionesService cotizacionesSvc;
        private readonly IProspectosService prospectosSvc;
        private readonly ICatalogosService catalogosSvc;
        private readonly IUsuarioService _logic;

        public CotizacionController(ILogger<CotizacionController> logger, ICotizacionesService cotizacionesSvc, IProspectosService prospectosSvc, ICatalogosService catalogosSvc, IUsuarioService _usuarioService)
        {
            _logger = logger;
            this.cotizacionesSvc = cotizacionesSvc;
            this.prospectosSvc = prospectosSvc;
            this.catalogosSvc = catalogosSvc;
            _logic = _usuarioService;
        }

        [HttpGet("{idPersonal}/{pagina}/{idGrupoActivo}")]
        public async Task<ActionResult<ListaCotizacionDTO>> Index(int idProspecto, EstatusCotizacion estatus, int servicio, int idPersonal = 0, int pagina = 1, int idGrupoActivo = 0)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            var listaCotizacionesVM = new ListaCotizacionDTO();
            listaCotizacionesVM.Pagina = pagina;
            listaCotizacionesVM.IdEstatusCotizacion = estatus;
            listaCotizacionesVM.IdServicio = servicio;
            listaCotizacionesVM.IdProspecto = idProspecto;
            int autorizacion = await cotizacionesSvc.ObtenerAutorizacion(idPersonal);
            await cotizacionesSvc.ObtenerListaCotizaciones(listaCotizacionesVM, autorizacion, idPersonal, idGrupoActivo);
            return listaCotizacionesVM;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<CotizacionDTO>> NuevoProspecto([FromBody] ProspectoDTO prospectoVM)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            await prospectosSvc.CrearProspecto(prospectoVM);
            var cotizacionVM = new CotizacionDTO { IdProspecto = prospectoVM.IdProspecto };
            return cotizacionVM;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> SeleccionarProspecto([FromBody] CotizacionDTO cotizacionVM)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

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
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            var resumen = await cotizacionesSvc.ObtenerResumenCotizacionLimpieza(id);

            return resumen;
        }

        [HttpGet]
        public async Task<ActionResult<ProspectoDTO>> LimpiezaInfoProspecto(int id)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            var prospecto = await prospectosSvc.ObtenerProspectoPorCotizacion(id);

            prospecto.IdCotizacion = id;

            // TempData["IdCotizacion"] = id;
            // TempData["Action"] = "LimpiezaInfoProspecto";

            return prospecto;
        }

        [HttpGet("[action]/{id}/{pagina}")]
        public async Task<ActionResult<ListaDireccionDTO>> LimpiezaDirectorio(int id, int pagina = 0)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            var listaDireccionesVM = new ListaDireccionDTO();

            listaDireccionesVM.IdCotizacion = id;

            listaDireccionesVM.Pagina = pagina;

            await cotizacionesSvc.ObtenerListaDireccionesPorCotizacion(listaDireccionesVM);

            return listaDireccionesVM;
        }

        [HttpGet("{id}/{idDireccionCotizacion}/{idPuestoDireccionCotizacion}/{pagina}")]
        public async Task<ActionResult<ListaPuestosDireccionCotizacionDTO>> LimpiezaPlantilla(int id, int idDireccionCotizacion = 0, int idPuestoDireccionCotizacion = 0, int pagina = 0)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            var listaPuestosDireccionCotizacionVM = new ListaPuestosDireccionCotizacionDTO { IdCotizacion = id, IdDireccionCotizacion = idDireccionCotizacion, IdPuestoDireccionCotizacion = idPuestoDireccionCotizacion, Pagina = pagina };

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
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await cotizacionesSvc.ActualizarIndirectoUtilidad(cotizacionupd.IdCotizacion, cotizacionupd.Indirecto, cotizacionupd.Utilidad, cotizacionupd.ComisionSV, cotizacionupd.ComisionExt, cotizacionupd.PolizaPor, cotizacionupd.PorcentajeFinanciamiento);
            //return RedirectToAction("LimpiezaResumen");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<DireccionCotizacionDTO>> AgregarDireccion([FromBody] DireccionCotizacionDTO direccionCVM)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            var direcciones = await cotizacionesSvc.ObtenerListaDireccionesPorCotizacion(direccionCVM.IdCotizacion);

            foreach (var direccion in direcciones)
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
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await cotizacionesSvc.EliminarCotizacion(idCotizacion);
        }

        [HttpGet("[action]/{idDC}")]
        public async Task<bool> EliminarDireccionCotizacion(int idDC)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            var idCotizacion = await cotizacionesSvc.ObtenerIdCotizacionPorDireccion(idDC);

            await cotizacionesSvc.EliminarDireccionCotizacion(idDC);

            // TempData["DescripcionAlerta"] = "Se quitó correctamente la dirección de la cotización";
            // TempData["IdTipoAlerta"] = TipoAlerta.Info;

            return true;
        }

        [HttpPut]
        public async Task<IActionResult> EditarProspecto([FromBody] ProspectoDTO prospectoVM)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

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

        [HttpGet("[action]/{incluyeProducto}/{idCotizacion}")]
        public async Task<int> DuplicarCotizacion(bool incluyeProducto, int idCotizacion)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            var idNuevaCotizacion = await cotizacionesSvc.DuplicarCotizacion(idCotizacion, incluyeProducto);

            // TempData["DescripcionAlerta"] = "Se duplicó correctamente la cotización";
            // TempData["IdTipoAlerta"] = TipoAlerta.Exito;

            return idNuevaCotizacion;
        }

        [HttpGet("[action]")]
        public IEnumerable<Item<int>> GetEstatus()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

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
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            await cotizacionesSvc.ActualizarSalarios(salarios);
            return salarios;

        }

        [HttpGet("[action]/{idCotizacion}/{idServicio}/{polizaCumplimiento}/{diasEvento}")]
        public async Task<ActionResult<bool>> ActualizarCotizacion(int idCotizacion, int idServicio, bool polizaCumplimiento, int diasEvento)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await cotizacionesSvc.ActualizarCotizacion(idCotizacion, idServicio, polizaCumplimiento, diasEvento);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<CotizaPorcentajes>> ObtenerPorcentajesCotizacion()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            CotizaPorcentajes porcentajes = new CotizaPorcentajes();
            porcentajes = await cotizacionesSvc.ObtenerPorcentajesCotizacion();
            return porcentajes;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<bool>> ActualizarPorcentajesPredeterminadosCotizacion([FromBody] CotizaPorcentajes porcentajes)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            await cotizacionesSvc.ActualizarPorcentajesPredeterminadosCotizacion(porcentajes);
            return true;
        }

        [HttpGet("[action]/{idPersonal}")]
        public async Task<ActionResult<int>> ObtenerAutorizacion(int idPersonal = 0)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            int autorizacion = await cotizacionesSvc.ObtenerAutorizacion(idPersonal);
            return autorizacion;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<decimal>> ObtenerImssBase()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await cotizacionesSvc.ObtenerImssBase();
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<bool>> ActualizarImssBase([FromBody] decimal imss)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            await cotizacionesSvc.ActualizarImssBase(imss);
            return true;
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<bool>> ActivarCotizacion([FromBody] int idCotizacion)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await cotizacionesSvc.ActivarCotizacion(idCotizacion);
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<bool>> DesactivarCotizacion([FromBody] int idCotizacion)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await cotizacionesSvc.DesactivarCotizacion(idCotizacion);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<ImmsJornadaDTO>> ObtenerImssJornada()
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await cotizacionesSvc.ObtenerImssJornada();
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<bool>> ActualizarImssJornada(ImmsJornadaDTO imssJormada)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await cotizacionesSvc.ActualizarImssJornada(imssJormada);
        }

        [HttpGet("[action]/{idVendedor}")]
        public async Task<ActionResult<CotizacionVendedorDetalleDTO>> CotizacionVendedorDetallePorIdVendedor(int idVendedor)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await cotizacionesSvc.ObtenerCotizacionVendedorDetallePorIdVendedor(idVendedor);
        }

        [HttpGet("[action]/{idCotizacion}/{motivoCierre}")]
        public async Task<ActionResult<bool>> CerrarCotizacion(int idCotizacion, string motivoCierre)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            await cotizacionesSvc.DesactivarCotizacion(idCotizacion);
            return await cotizacionesSvc.InsertarMotivoCierreCotizacion(motivoCierre, idCotizacion);
        }

        [HttpGet("[action]/{idCotizacion}")]
        public async Task<int> ObtenerTotalSucursalesCotizacion(int idCotizacion)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await cotizacionesSvc.ObtenerTotalSucursalesCotizacion(idCotizacion);
        }
        [HttpGet("[action]/{idCotizacion}")]
        public async Task<int> ObtenerTotalEmpleadosCotizacion(int idCotizacion)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await cotizacionesSvc.ObtenerTotalEmpleadosCotizacion(idCotizacion);
        }
        [HttpPost("[action]")]
        public async Task<bool> AutorizarCotizacion([FromBody] int idCotizacion = 0)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await cotizacionesSvc.AutorizarCotizacion(idCotizacion);
        }

        [HttpPost("[action]")]
        public async Task<bool> RemoverAutorizacionCotizacion([FromBody] int idCotizacion = 0)
        {
            var token = _logic.GenerarToken();
            Response.Headers.Add("Authorization", $"Bearer {token}");

            return await cotizacionesSvc.RemoverAutorizacionCotizacion(idCotizacion);
        }
    }
}
