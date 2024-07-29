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

namespace SistemaVentasBatia.Services
{
    public interface IProspectosService
    {
        Task CrearProspecto(ProspectoDTO prospectoVM);
        Task ObtenerListaProspectos(ListaProspectoDTO listaProspectosVM, int autorizacion, int idPersonal);
        Task<ProspectoDTO> ObtenerProspecto(int idProspecto);
        Task EditarProspecto(ProspectoDTO prospectoVM);
        Task ObtenerListaDirecciones(ListaDireccionDTO listaDireccionesVM);
        Task CrearDireccion(DireccionDTO direccionVM);
        Task<List<ProspectoDTO>> ObtenerCatalogoProspectos(int autorizacion, int idPersonal);
        Task<int> ObtenerIdProspectoPorCotizacion(int idCotizacion);
        Task<ProspectoDTO> ObtenerProspectoPorCotizacion(int idCotizacion);
        Task EliminarProspecto(int registroAEliminar);
        Task<List<ProspectoDTO>> ObtenerCoincidenciasProspecto(string nombreComercial, string rfc);
        Task<int> ObtenerNumeroCoincidenciasProspecto(string nombreComercial, string rfc);
        Task<DireccionDTO> ObtenerDireccionPorId(int id);
        Task ActualizarDireccion(DireccionDTO direccionVM);
        Task<PuestoDireccionCotizacionDTO> ObtenerOperarioPorId(int id);
        Task<bool> ActivarProspecto(int idProspecto);
        Task<bool> DesactivarProspecto(int idProspecto);
        Task<DireccionResponseAPIDTO> GetDireccionAPI(string cp);
        Task<ProspectoDTO> ObtenerDatosProspecto(int idProspecto);
    }

    public class ProspectosService : IProspectosService
    {
        private readonly IProspectosRepository prospectosRepo;
        private readonly IMapper mapper;
        private readonly ICotizacionesRepository cotizacionRepo;

        public ProspectosService(IProspectosRepository prospectosRepo, IMapper mapper, ICotizacionesRepository cotizacionRepo)
        {
            this.prospectosRepo = prospectosRepo;
            this.mapper = mapper;
            this.cotizacionRepo = cotizacionRepo;
        }

        public async Task CrearProspecto(ProspectoDTO prospectoVM)

        {
            var prospecto = mapper.Map<Prospecto>(prospectoVM);

            prospecto.IdEstatusProspecto = EstatusProspecto.Activo;

            prospecto.FechaAlta = DateTime.Now;

            prospecto.Documentacion = (Documento)prospectoVM.ListaDocumentos.Where(x => x.Act).Sum(x => x.Id);

            await prospectosRepo.InsertarProspecto(prospecto);

            prospectoVM.IdProspecto = prospecto.IdProspecto;
        }

        public async Task ObtenerListaProspectos(ListaProspectoDTO listaProspectosVM, int autorizacion, int idPersonal)
        {
            listaProspectosVM.Rows = await prospectosRepo.ContarProspectos(listaProspectosVM.IdEstatusProspecto, listaProspectosVM.Keywords, idPersonal, autorizacion);

            if (listaProspectosVM.Rows > 0)
            {
                listaProspectosVM.NumPaginas = (listaProspectosVM.Rows / 40);

                if (listaProspectosVM.Rows % 40 > 0)
                {
                    listaProspectosVM.NumPaginas++;
                }

                listaProspectosVM.Prospectos = mapper.Map<List<ProspectoDTO>>(
                    await prospectosRepo.ObtenerProspectos(listaProspectosVM.Pagina, listaProspectosVM.IdEstatusProspecto, listaProspectosVM.Keywords, autorizacion, idPersonal));
            }
            else
            {
                listaProspectosVM.Prospectos = new List<ProspectoDTO>();
            }
        }

        public async Task<ProspectoDTO> ObtenerProspecto(int idProspecto)
        {
            var prospecto = await prospectosRepo.ObtenerProspectoPorId(idProspecto);

            var prospectoVM = mapper.Map<ProspectoDTO>(prospecto);

            prospectoVM.ListaDocumentos = new List<Item<int>>();

            foreach (Documento documento in (Documento[])Enum.GetValues(typeof(Documento)))
            {
                Item<int> doc = new Item<int>() { Id = (int)documento, Nom = documento.ToString(), Act = false };
                if (prospecto.Documentacion.HasFlag(documento))
                {
                    doc.Act = true;
                }
                prospectoVM.ListaDocumentos.Add(doc);
            }

            return prospectoVM;
        }

        public async Task<ProspectoDTO> ObtenerProspectoPorCotizacion(int idCotizacion)
        {
            var prospecto = await prospectosRepo.ObtenerProspectoPorCotizacion(idCotizacion);

            var prospectoVM = mapper.Map<ProspectoDTO>(prospecto);

            prospectoVM.ListaDocumentos = new List<Item<int>>();

            foreach (Documento documento in (Documento[])Enum.GetValues(typeof(Documento)))
            {
                Item<int> doc = new Item<int>() { Id = (int)documento, Nom = documento.ToString(), Act = false };
                if (prospecto.Documentacion.HasFlag(documento))
                {
                    doc.Act = true;
                }
                prospectoVM.ListaDocumentos.Add(doc);
            }

            return prospectoVM;
        }

        public async Task EditarProspecto(ProspectoDTO prospectoVM)
        {
            var prospecto = mapper.Map<Prospecto>(prospectoVM);

            prospecto.Documentacion = (Documento)prospectoVM.ListaDocumentos.Where(x => x.Act).Sum(x => x.Id);

            await prospectosRepo.ActualizarProspecto(prospecto);
            //if (prospecto.PolizaCumplimiento == false)
            //{
            //    await prospectosRepo.EliminarTotalPolizasByIdProspecto(prospecto.IdProspecto);
            //}
        }

        public async Task ObtenerListaDirecciones(ListaDireccionDTO listaDireccionesVM)
        {
            var lista = await prospectosRepo.ObtenerDireccionesPorProspecto(listaDireccionesVM.IdProspecto, listaDireccionesVM.Pagina);
            listaDireccionesVM.Direcciones = lista.Select(d =>
            new DireccionMinDTO
            {
                IdDireccion = d.IdDireccion,
                IdProspecto = d.IdProspecto,
                NombreSucursal = d.NombreSucursal,
                Estado = d.Estado,
                TipoInmueble = d.TipoInmueble,
                DomicilioCompleto = (d.Domicilio ?? "") + ", " + (d.Colonia ?? "") + ", " + (d.Municipio ?? "") + ", " + (d.Ciudad ?? "") + ", " + (d.Estado ?? "") + ", CP " + (d.CodigoPostal),
                IdDireccionCotizacion = d.IdDireccionCotizacion
            }).ToList();
        }

        public async Task CrearDireccion(DireccionDTO direccionVM)
        {
            var direccion = mapper.Map<Direccion>(direccionVM);

            direccion.IdEstatusDireccion = EstatusDireccion.Activo;

            direccion.FechaAlta = DateTime.Now;
            direccion.Frontera = await prospectosRepo.GetFronteraPorIdMunicipio(direccion.IdMunicipio);
            await prospectosRepo.InsertarDireccion(direccion);

            direccionVM.IdDireccion = direccion.IdDireccion;

        }

        public async Task<List<ProspectoDTO>> ObtenerCatalogoProspectos(int autorizacion, int idPersonal)
        {
            var prospectos = mapper.Map<List<ProspectoDTO>>(await prospectosRepo.ObtenerCatalogoProspectos(autorizacion, idPersonal));

            return prospectos;
        }

        public async Task<int> ObtenerIdProspectoPorCotizacion(int idCotizacion)
        {
            var idProspecto = await prospectosRepo.ObtenerIdProspectoPorCotizacion(idCotizacion);

            return idProspecto;
        }

        public async Task EliminarProspecto(int registroAEliminar)
        {
            await prospectosRepo.InactivarProspecto(registroAEliminar);
        }

        public async Task<List<ProspectoDTO>> ObtenerCoincidenciasProspecto(string nombreComercial, string rfc)
        {
            var prospectos = new List<ProspectoDTO>();

            if (!string.IsNullOrEmpty(nombreComercial) || !string.IsNullOrEmpty(rfc))
            {
                prospectos = mapper.Map<List<ProspectoDTO>>(await prospectosRepo.ObtenerCoincidenciasProspecto(nombreComercial, rfc));
            }

            return prospectos;
        }

        public async Task<int> ObtenerNumeroCoincidenciasProspecto(string nombreComercial, string rfc)
        {
            var numeroProspectos = 0;

            if (!string.IsNullOrEmpty(nombreComercial) || !string.IsNullOrEmpty(rfc))
            {
                numeroProspectos = (await prospectosRepo.ObtenerCoincidenciasProspecto(nombreComercial, rfc)).Count();
            }

            return numeroProspectos;
        }

        public async Task<DireccionDTO> ObtenerDireccionPorId(int id)
        {
            var direccionViewModel = mapper.Map<DireccionDTO>(await prospectosRepo.ObtenerDireccionPorId(id));
            direccionViewModel.IdMunicipio = await prospectosRepo.GetIdMunucipioByMunicipio(direccionViewModel.IdEstado, direccionViewModel.Municipio);

            return direccionViewModel;
        }

        public async Task ActualizarDireccion(DireccionDTO direccionVM)
        {
            var direccion = mapper.Map<Direccion>(direccionVM);
            direccion.Frontera = await prospectosRepo.GetFronteraPorIdMunicipio(direccion.IdMunicipio);

            await prospectosRepo.ActualizarDireccion(direccion);
        }

        public async Task<PuestoDireccionCotizacionDTO> ObtenerOperarioPorId(int id)
        {
            var puestoDireccionCotizacionViewModel = mapper.Map<PuestoDireccionCotizacionDTO>(await prospectosRepo.ObtenerPuestoDireccionCotizacionPorId(id));

            puestoDireccionCotizacionViewModel.IncluyeMaterial = await cotizacionRepo.ValidarProductoExistentePuesto(id);
            return puestoDireccionCotizacionViewModel;
        }

        public async Task<bool> ActivarProspecto(int idProspecto)
        {
            return await prospectosRepo.ActivarProspecto(idProspecto);
        }

        public async Task<bool> DesactivarProspecto(int idProspecto)
        {
            return await prospectosRepo.DesactivarProspecto(idProspecto);
        }

        public async Task<DireccionResponseAPIDTO> GetDireccionAPI(string cp)
        {
            string apiUrl = "https://api.tau.com.mx/dipomex/v1/codigo_postal?cp=" + cp;
            string apiKey = "0e25c4ffc4b8121957dbfd888aefe45d822578d1";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("ApiKey", apiKey);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        DireccionResponseAPIDTO codigoPostalResponse = JsonConvert.DeserializeObject<DireccionResponseAPIDTO>(jsonResponse);
                        //codigoPostalResponse.CodigoPostal.IdEstado = await prospectosRepo.GetIdEstadoByEstado(codigoPostalResponse.CodigoPostal.Estado);
                        //codigoPostalResponse.CodigoPostal.IdMunicipio = await prospectosRepo.GetIdMunucipioByMunicipio(codigoPostalResponse.CodigoPostal.IdEstado, codigoPostalResponse.CodigoPostal.Municipio);
                        return codigoPostalResponse;
                    }
                    else
                    {
                        Console.WriteLine("Error al hacer la solicitud. Código de estado: " + response.StatusCode);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return null;
                }
            }
        }
        public async Task<ProspectoDTO> ObtenerDatosProspecto(int idProspecto)
        {
            var prospecto = mapper.Map<ProspectoDTO>(await prospectosRepo.ObtenerDatosProspecto(idProspecto));
            return prospecto;
        }
    }
}
