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

namespace SistemaVentasBatia.Services
{
    public interface ICargaMasivaService
    {
        Task<bool> CargarDirecciones(int idCotizacion, int idProspecto, IFormFile archivo);
        Task<byte[]> ObtenerSucursalesLayout(int idCotizacion);
        Task<byte[]> ObtenerDatosCotizacion(int idCotizacion);
        Task<bool> CargarPlantilla(IFormFile file, int idCotizacion);

    }
    public class CargaMasivaService : ICargaMasivaService
    {
        private readonly ICotizacionesService _logicCot;
        private readonly ISalarioService _logicSal;
        private readonly IProspectosService _prosService;
        private readonly ICargaMasivaRepository _repo;
        private readonly IMapper _mapper;

        public CargaMasivaService(ICargaMasivaRepository repoCargaMasiva, IMapper mapper, IProspectosService prospectosService, ISalarioService logicSal, ICotizacionesService logicCot)
        {
            _logicCot = logicCot;
            _logicSal = logicSal;
            _prosService = prospectosService;
            _repo = repoCargaMasiva;
            _mapper = mapper;
            _logicSal = logicSal;
        }

        public async Task<bool> CargarDirecciones(int idCotizacion, int idProspecto, IFormFile archivo)
        {
            var direcciones = new List<Direccion>();
            using (var stream = new MemoryStream())
            {
                await archivo.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        if (worksheet.Cells[row, 1].Value == null || string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Value.ToString()))
                        {
                            break;
                        }
                        //Obtener informacion de Excel
                        var direccion = new Direccion
                        {
                            NombreSucursal = worksheet.Cells[row, 1].Value?.ToString(),
                            IdTipoInmueble = Convert.ToInt32(worksheet.Cells[row, 2].Value),
                            Colonia = worksheet.Cells[row, 3].Value?.ToString(),
                            Domicilio = worksheet.Cells[row, 4].Value?.ToString(),
                            CodigoPostal = worksheet.Cells[row, 5].Value?.ToString(),
                        };
                        //LLenar con datos enduro
                        direccion.IdTabulador = 1;
                        direccion.IdProspecto = idProspecto;
                        direccion.IdEstatusDireccion = (Enums.EstatusDireccion)1;
                        direccion.FechaAlta = DateTime.Now;
                        //obtener informacion por CP
                        var direccionAPI = new DireccionResponseAPIDTO();
                        direccionAPI = await _prosService.GetDireccionAPI(direccion.CodigoPostal);
                        direccion.Ciudad = direccionAPI.CodigoPostal.Estado;
                        direccion.Municipio = direccionAPI.CodigoPostal.Municipio;
                        direccion.IdEstado = direccionAPI.CodigoPostal.IdEstado;
                        direccion.IdMunicipio = direccionAPI.CodigoPostal.IdMunicipio;
                        //obtener isFrontera
                        bool isFrontera = await _repo.ObtenerFronteraPorIdMunicipio(direccion.IdMunicipio);
                        direccion.Frontera = isFrontera;

                        direcciones.Add(direccion);
                    }
                    var result = _repo.InsertarDireccionesExcel(direcciones, idCotizacion);
                    var idInsertados = new List<int>();
                    idInsertados = await _repo.ObtenerIdDireccionesInsertadas(idProspecto, direcciones.Count);
                    foreach (int id in idInsertados)
                    {
                        await _repo.InsertarDireccionCotizacion(id, idCotizacion);
                    }
                }
                return true;
            }
        }

        public async Task<byte[]> ObtenerSucursalesLayout(int idCotizacion)
        {
            var sucursales = await _repo.ObtenerSucursalesCotizacion(idCotizacion);

            string rutaArchivo = Path.Combine("Layouts", "LayoutPlantilla.xlsx");


            using (var stream = new MemoryStream(File.ReadAllBytes(rutaArchivo)))
            {
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[1];

                    int rowNum = 2;
                    foreach (var sucursal in sucursales)
                    {
                        worksheet.Cells[rowNum, 1].Value = sucursal.NombreSucursal;
                        worksheet.Cells[rowNum, 2].Value = sucursal.IdDireccionCotizacion;
                        rowNum++;
                    }
                    return package.GetAsByteArray();
                }
            }
        }

        public async Task<bool> CargarPlantilla(IFormFile file, int idCotizacion)
        {
            var puestos = new List<PuestoDireccionCotizacionDTO>();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    try
                    {


                        for (int row = 2; row <= rowCount; row++)
                        {
                            if (worksheet.Cells[row, 1].Value == null || string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Value.ToString()))
                            {
                                break;
                            }

                            //Obtener informacion de Excel
                            var puesto = new PuestoDireccionCotizacionDTO
                            {
                                IdDireccionCotizacion = Convert.ToInt32(worksheet.Cells[row, 1].Value),
                                IdPuesto = Convert.ToInt32(worksheet.Cells[row, 2].Value),
                                Jornada = Convert.ToDecimal(worksheet.Cells[row, 3].Value),
                                IdTabulador = Convert.ToInt32(worksheet.Cells[row, 5].Value),
                                IdClase = Convert.ToInt32(worksheet.Cells[row, 6].Value),
                                Cantidad = Convert.ToInt32(worksheet.Cells[row, 7].Value),
                                Bonos = Convert.ToDecimal(worksheet.Cells[row, 17].Value),
                                Vales = Convert.ToDecimal(worksheet.Cells[row, 18].Value),
                            };

                            //Parseo dias Enum.DiaSemana
                            int diaini = Convert.ToInt32(worksheet.Cells[row, 8].Value);
                            DiaSemana diainif;
                            if (Enum.IsDefined(typeof(DiaSemana), diaini))
                            {
                                diainif = (DiaSemana)diaini;
                                puesto.DiaInicio = diainif;
                            }
                            else
                            {
                                return false;
                            }
                            int diafin = Convert.ToInt32(worksheet.Cells[row, 9].Value);
                            DiaSemana diafinf;
                            if (Enum.IsDefined(typeof(DiaSemana), diafin))
                            {
                                diafinf = (DiaSemana)diafin;
                                puesto.DiaFin = diafinf;
                            }
                            else
                            {
                                return false;
                            }
                            int diaini2 = Convert.ToInt32(worksheet.Cells[row, 13].Value);
                            DiaSemana diaini2f;
                            if( diaini2 == 0)
                            {
                                puesto.DiaInicioFin = 0;
                            }
                            else
                            {
                                if (Enum.IsDefined(typeof(DiaSemana), diaini2))
                                {
                                    diaini2f = (DiaSemana)diaini2;
                                    puesto.DiaInicioFin = diaini2f;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            
                            int diafin2 = Convert.ToInt32(worksheet.Cells[row, 14].Value);
                            DiaSemana diafin2f;
                            if (diafin2 == 0)
                            {
                                puesto.DiaFinFin = 0;
                            }
                            else
                            {
                                if (Enum.IsDefined(typeof(DiaSemana), diafin2))
                                {
                                    diafin2f = (DiaSemana)diafin2;
                                    puesto.DiaFinFin = diafin2f;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            
                            int diadescanso = Convert.ToInt32(worksheet.Cells[row, 12].Value);
                            DiaSemana diadescansof;
                            if (Enum.IsDefined(typeof(DiaSemana), diadescanso))
                            {
                                diadescansof = (DiaSemana)diadescanso;
                                puesto.DiaDescanso = diadescansof;
                            }
                            else
                            {
                                return false;
                            }

                            //Parseo horas TimeSpan
                            int hrini = Convert.ToInt32(worksheet.Cells[row, 10].Value);
                            TimeSpan hrinif = TimeSpan.FromHours(hrini);
                            puesto.HrInicio = hrinif;
                            int hrfin = Convert.ToInt32(worksheet.Cells[row, 11].Value);
                            TimeSpan hrfinf = TimeSpan.FromHours(hrfin);
                            puesto.HrFin = hrfinf;
                            int hrini2 = Convert.ToInt32(worksheet.Cells[row, 15].Value);
                            TimeSpan hrini2f = TimeSpan.FromHours(hrini2);
                            puesto.HrInicioFin = hrini2f;
                            int hrfin2 = Convert.ToInt32(worksheet.Cells[row, 16].Value);
                            TimeSpan hrfin2f = TimeSpan.FromHours(hrfin2);
                            puesto.HrFinFin = hrfin2f;


                            //Parseo Turno
                            int turno = Convert.ToInt32(worksheet.Cells[row, 4].Value);
                            Turno turnof;
                            if (Enum.IsDefined(typeof(Turno), turno))
                            {
                                turnof = (Turno)turno;
                                puesto.IdTurno = turnof;
                            }
                            else
                            {
                                return false;
                            }

                            //Parseo Bool
                            int diaFestivo = Convert.ToInt32(worksheet.Cells[row, 19].Value);
                            bool esDiaFestivo = (diaFestivo != 0);
                            puesto.DiaFestivo = esDiaFestivo;
                            int diaDomingo = Convert.ToInt32(worksheet.Cells[row, 20].Value);
                            bool esDiaDomingoo = (diaDomingo != 0);
                            puesto.DiaDomingo = esDiaDomingoo;
                            int cubreDescanso = Convert.ToInt32(worksheet.Cells[row, 21].Value);
                            bool escubreDescanso = (cubreDescanso != 0);
                            puesto.DiaCubreDescanso = escubreDescanso;

                            //LLenar con datos enduro
                            puesto.FechaAlta = DateTime.Now;
                            puesto.IdSalario = 0;
                            puesto.IdCotizacion = idCotizacion;

                            //Calcular sueldo
                            decimal sueldo = await _logicSal.GetSueldo(puesto.IdPuesto, puesto.IdClase, puesto.IdTabulador, (int)puesto.IdTurno);
                            puesto.Sueldo = sueldo;
                            switch (puesto.Jornada)
                            {
                                case 1:
                                    puesto.Sueldo *= 0.35M;
                                    break;
                                case 2:
                                    puesto.Sueldo *= 0.60M;
                                    break;
                                case 4:
                                    puesto.Sueldo *= 1.5M;
                                    break;
                                default:
                                    break;
                            }
                            puestos.Add(puesto);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    foreach (var puesto in puestos)
                    {
                        await _logicCot.CrearPuestoDireccionCotizacion(puesto);
                    }
                }
                return true;
            }
        }

        public async Task<byte[]> ObtenerDatosCotizacion(int idCotizacion)
        {

            string rutaArchivo = Path.Combine("Layouts", "DatosCotizacion.xlsx");

            using (var stream = new MemoryStream(File.ReadAllBytes(rutaArchivo)))
            {
                using (var package = new ExcelPackage(stream))
                {
                    //Resumen
                    var resumenCotizacion = _logicCot.ObtenerResumenCotizacionLimpieza(idCotizacion);
                    var resumen = await _repo.ObtenerResumenCotizacionLimpieza(idCotizacion);
                    var cotizacion = await _repo.ObtenerCotizacion(idCotizacion);
                    decimal? poliza = await _repo.ObtenerPolizaCotizacion(idCotizacion);
                    if (resumen != null)
                    {
                        var wsRes = package.Workbook.Worksheets[0];
                        wsRes.Cells[1, 1].Value = "Salario";
                        wsRes.Cells[1, 2].Value = "$" + resumen.Salario.ToString("0.00");
                        wsRes.Cells[2, 1].Value = "Prestaciones";
                        wsRes.Cells[2, 2].Value = "$" + resumen.Provisiones.ToString("0.00");
                        wsRes.Cells[3, 1].Value = "Carga Social";
                        wsRes.Cells[3, 2].Value = "$" + resumen.CargaSocial.ToString("0.00");
                        wsRes.Cells[4, 1].Value = "Provisiones";
                        wsRes.Cells[4, 2].Value = "$" + resumen.Prestaciones.ToString("0.00");
                        wsRes.Cells[5, 1].Value = "Material";
                        wsRes.Cells[5, 2].Value = "$" + resumen.Material.ToString("0.00");
                        wsRes.Cells[6, 1].Value = "Uniforme";
                        wsRes.Cells[6, 2].Value = "$" + resumen.Uniforme.ToString("0.00");
                        wsRes.Cells[7, 1].Value = "Equipo";
                        wsRes.Cells[7, 2].Value = "$" + resumen.Equipo.ToString("0.00");
                        wsRes.Cells[8, 1].Value = "Herramienta";
                        wsRes.Cells[8, 2].Value = "$" + resumen.Herramienta.ToString("0.00");
                        wsRes.Cells[9, 1].Value = "Servicio";
                        wsRes.Cells[9, 2].Value = "$" + resumen.Servicio.ToString("0.00");
                        wsRes.Cells[10, 1].Value = "Total costo directo";
                        wsRes.Cells[10, 2].Value = "$" + (resumen.Salario + resumen.Provisiones + resumen.CargaSocial + resumen.Prestaciones + resumen.Material + resumen.Uniforme + resumen.Equipo + resumen.Herramienta + resumen.Servicio).ToString("0.00");

                        wsRes.Cells[11, 1].Value = "Costo Indirecto (%) ";
                        wsRes.Cells[11, 2].Value = "$" + resumen.Indirecto.ToString("0.00");
                        wsRes.Cells[11, 1].Value = "Total CI (%) ";
                        wsRes.Cells[11, 2].Value = "$" + (resumen.Salario + resumen.Provisiones + resumen.CargaSocial + resumen.Prestaciones + resumen.Material + resumen.Uniforme + resumen.Equipo + resumen.Herramienta + resumen.Servicio + resumen.Indirecto).ToString("0.00");
                        wsRes.Cells[10, 1].Value = "Póliza de Cumplimiento (%) ";
                        wsRes.Cells[2, 4].Value = "Utilidad (%) ";
                        wsRes.Cells[2, 5].Value = "$" + resumen.Utilidad.ToString("0.00");
                        wsRes.Cells[2, 4].Value = "Total U(%) ";
                        wsRes.Cells[2, 5].Value = "$" + (resumen.Salario + resumen.Provisiones + resumen.CargaSocial + resumen.Prestaciones + resumen.Material + resumen.Uniforme + resumen.Equipo + resumen.Herramienta + resumen.Servicio + resumen.Indirecto + resumen.Utilidad).ToString("0.00");
                        wsRes.Cells[10, 2].Value = "$" + poliza.Value.ToString("0.00");
                        wsRes.Cells[3, 4].Value = "Comisión Sobre Venta (%) ";
                        wsRes.Cells[3, 5].Value = "$" + resumen.ComisionSV.ToString("0.00");
                        wsRes.Cells[3, 4].Value = "Total CSV (%) ";
                        wsRes.Cells[3, 5].Value = "$" + (resumen.Salario + resumen.Provisiones + resumen.CargaSocial + resumen.Prestaciones + resumen.Material + resumen.Uniforme + resumen.Equipo + resumen.Herramienta + resumen.Servicio + resumen.Indirecto + resumen.Utilidad + resumen.ComisionSV).ToString("0.00");
                        wsRes.Cells[4, 4].Value = "Comisión Externa (%) ";
                        wsRes.Cells[4, 5].Value = "$" + cotizacion.ComisionExt.ToString("0.00");
                        wsRes.Cells[11, 1].Value = "Total";
                        wsRes.Cells[11, 2].Value = "$" + resumen.Total.ToString("0.00");



                    }

                    //Prospecto
                    var prospecto = new Prospecto();
                    prospecto = await _repo.ObtenerProspecto(idCotizacion);
                    if (prospecto != null)
                    {
                        var wsPros = package.Workbook.Worksheets[1];
                        wsPros.Cells[1, 1].Value = "Id Prospecto";
                        wsPros.Cells[1, 2].Value = prospecto.IdProspecto;
                        wsPros.Cells[2, 1].Value = "Nombre comercial";
                        wsPros.Cells[2, 2].Value = prospecto.NombreComercial;
                        wsPros.Cells[3, 1].Value = "Razón Social";
                        wsPros.Cells[3, 2].Value = prospecto.RazonSocial;
                        wsPros.Cells[4, 1].Value = "RFC";
                        wsPros.Cells[4, 2].Value = prospecto.Rfc;
                        wsPros.Cells[5, 1].Value = "Domicilio Fiscal";
                        wsPros.Cells[5, 2].Value = prospecto.DomicilioFiscal;
                        wsPros.Cells[6, 1].Value = "Nombre Contacto";
                        wsPros.Cells[6, 2].Value = prospecto.NombreContacto;
                        wsPros.Cells[7, 1].Value = "Email Contacto";
                        wsPros.Cells[7, 2].Value = prospecto.EmailContacto;
                        wsPros.Cells[8, 1].Value = "Numero Contacto";
                        wsPros.Cells[8, 2].Value = prospecto.NumeroContacto;
                        wsPros.Cells[9, 1].Value = "Ext Contacto";
                        wsPros.Cells[9, 2].Value = prospecto.ExtContacto;

                    }
                    //Directorio
                    var directorios = new List<Direccion>();
                    directorios = await _repo.ObtenerDirecciones(idCotizacion);
                    if (directorios.Count != 0)
                    {
                        var wsDir = package.Workbook.Worksheets[2];
                        int row = 2;
                        foreach (var dir in directorios)
                        {
                            wsDir.Cells[row, 1].Value = dir.IdDireccion;
                            wsDir.Cells[row, 2].Value = dir.NombreSucursal;
                            wsDir.Cells[row, 3].Value = dir.TipoInmueble;
                            wsDir.Cells[row, 4].Value = dir.Estado;
                            wsDir.Cells[row, 5].Value = dir.Municipio;
                            wsDir.Cells[row, 6].Value = dir.Ciudad;
                            wsDir.Cells[row, 7].Value = dir.Colonia;
                            wsDir.Cells[row, 8].Value = dir.Domicilio;
                            wsDir.Cells[row, 9].Value = dir.CodigoPostal;
                            row++;
                        }
                    }
                    //Plantilla
                    var wsPlan = package.Workbook.Worksheets[3];

                    //Producto
                    var wsProd = package.Workbook.Worksheets[4];

                    //ProductoExtra
                    var wsProdExt = package.Workbook.Worksheets[5];

                    //Servicio
                    var wsSer = package.Workbook.Worksheets[6];

                    return package.GetAsByteArray();
                }
            }
        }
    }
}
