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
using Microsoft.AspNetCore.WebUtilities;
using OfficeOpenXml.Drawing.Slicer.Style;

namespace SistemaVentasBatia.Services
{
    public interface ICargaMasivaService
    {
        Task<bool> CargarDirecciones(int idCotizacion, int idProspecto, IFormFile archivo);
        Task<byte[]> ObtenerSucursalesLayout(int idCotizacion);
        Task<byte[]> ObtenerSucursalesLayoutProductoExtra(int idCotizacion);
        Task<byte[]> ObtenerDatosCotizacion(int idCotizacion);
        Task<bool> CargarPlantilla(IFormFile file, int idCotizacion);
        Task<bool> CargaProductoExtra(IFormFile archivo, int idCotizacion, string tipo, int idPersonal);

    }
    public class CargaMasivaService : ICargaMasivaService
    {
        private readonly ICotizacionesService _logicCot;
        private readonly ISalarioService _logicSal;
        private readonly IProspectosService _prosService;
        private readonly ICargaMasivaRepository _repo;
        private readonly IMapper _mapper;
        private readonly IMaterialService _materialService;

        public CargaMasivaService(ICargaMasivaRepository repoCargaMasiva, IMapper mapper, IProspectosService prospectosService, ISalarioService logicSal, ICotizacionesService logicCot, IMaterialService materialService)
        {
            _logicCot = logicCot;
            _logicSal = logicSal;
            _prosService = prospectosService;
            _repo = repoCargaMasiva;
            _mapper = mapper;
            _logicSal = logicSal;
            _materialService = materialService;
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
                            IdEstado = Convert.ToInt32(worksheet.Cells[row, 6].Value)
                        };
                        //LLenar con datos enduro
                        direccion.IdTabulador = 1;
                        direccion.IdProspecto = idProspecto;
                        direccion.IdEstatusDireccion = (Enums.EstatusDireccion)1;
                        direccion.FechaAlta = DateTime.Now;
                        //obtener informacion por CP
                        var direccionAPI = new DireccionResponseAPIDTO();
                        direccionAPI = await _prosService.GetDireccionAPI(direccion.CodigoPostal);
                        if (direccionAPI != null)
                        {
                            direccion.Ciudad = direccionAPI.CodigoPostal.Estado;
                            direccion.Municipio = direccionAPI.CodigoPostal.Municipio;
                            direccion.IdEstado = direccionAPI.CodigoPostal.IdEstado;
                            direccion.IdMunicipio = direccionAPI.CodigoPostal.IdMunicipio;
                            direccion.IdMunicipio = 0;
                        }
                        else
                        {
                            direccion.Ciudad = direccion.Colonia;
                            direccion.Municipio = "N/A";
                            direccion.IdMunicipio = 0;
                        }
                        
                        //obtener isFrontera
                        bool isFrontera = await _repo.ObtenerFronteraPorIdMunicipio(direccion.IdMunicipio);
                        direccion.Frontera = isFrontera;
                        Console.WriteLine("Direccion no:" + row + "agregada");
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
                        worksheet.Cells[rowNum, 3].Value = sucursal.IdTabulador;
                        rowNum++;
                    }
                    return package.GetAsByteArray();
                }
            }
        }

        public async Task<byte[]> ObtenerSucursalesLayoutProductoExtra(int idCotizacion)
        {
            var sucursales = await _repo.ObtenerSucursalesCotizacion(idCotizacion);

            string rutaArchivo = Path.Combine("Layouts", "LayoutProductoExtra.xlsx");


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
                            if (diaini2 == 0)
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
                            int incluyeProducto = Convert.ToInt32(worksheet.Cells[row, 22].Value);
                            bool incluyeProductos = (incluyeProducto != 0);
                            puesto.IncluyeMaterial = incluyeProductos;

                            //LLenar con datos enduro
                            puesto.FechaAlta = DateTime.Now;
                            puesto.IdSalario = 0;
                            puesto.IdCotizacion = idCotizacion;

                            //Calcular sueldo
                            decimal sueldo = await _logicSal.GetSueldo(puesto.IdPuesto, puesto.IdClase, puesto.IdTabulador, (int)puesto.IdTurno,0);
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
                            Console.WriteLine("puesto" + row.ToString());
                            puestos.Add(puesto);
                            Console.WriteLine("error");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    foreach (var puesto in puestos)
                    {
                        await _logicCot.CrearPuestoDireccionCotizacion(puesto,0);
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
                    var rC = await _logicCot.ObtenerResumenCotizacionLimpieza(idCotizacion);
                    decimal? poliza = await _repo.ObtenerPolizaCotizacion(idCotizacion);
                    if (rC != null)
                    {
                        var wsRes = package.Workbook.Worksheets[0];
                        wsRes.Cells[1, 1].Value = "Salario";
                        wsRes.Cells[1, 2].Value = "$" + rC.Salario.ToString("N2");
                        wsRes.Cells[2, 1].Value = "Prestaciones";
                        wsRes.Cells[2, 2].Value = "$" + rC.Provisiones.ToString("N2");
                        wsRes.Cells[3, 1].Value = "Carga Social";
                        wsRes.Cells[3, 2].Value = "$" + rC.CargaSocial.ToString("N2");
                        wsRes.Cells[4, 1].Value = "Provisiones";
                        wsRes.Cells[4, 2].Value = "$" + rC.Prestaciones.ToString("N2");
                        wsRes.Cells[5, 1].Value = "Material";
                        wsRes.Cells[5, 2].Value = "$" + rC.Material.ToString("N2");
                        wsRes.Cells[6, 1].Value = "Uniforme";
                        wsRes.Cells[6, 2].Value = "$" + rC.Uniforme.ToString("N2");
                        wsRes.Cells[7, 1].Value = "Equipo";
                        wsRes.Cells[7, 2].Value = "$" + rC.Equipo.ToString("N2");
                        wsRes.Cells[8, 1].Value = "Herramienta";
                        wsRes.Cells[8, 2].Value = "$" + rC.Herramienta.ToString("N2");
                        wsRes.Cells[9, 1].Value = "Servicio";
                        wsRes.Cells[9, 2].Value = "$" + rC.Servicio.ToString("N2");
                        
                        wsRes.Cells[10, 1].Value = "Total Costo Directo";
                        wsRes.Cells[10, 2].Value = "$" + rC.SubTotal.ToString("N2");
                        wsRes.Cells[11, 1].Value = "Costo Indirecto (" + rC.IndirectoPor + "%)";
                        wsRes.Cells[11, 2].Value = "$" + rC.Indirecto.ToString("N2");
                        wsRes.Cells[12, 1].Value = "Total CI ";
                        wsRes.Cells[12, 2].Value = "$" + (rC.SubTotal + rC.Indirecto).ToString("N2");
                        wsRes.Cells[13, 1].Value = "Utilidad (" + rC.UtilidadPor + "%)";
                        wsRes.Cells[13, 2].Value = "$" + rC.Utilidad.ToString("N2");
                        wsRes.Cells[14, 1].Value = "Total U";
                        wsRes.Cells[14, 2].Value = "$" + (rC.SubTotal+ rC.Indirecto + rC.Utilidad).ToString("N2");
                        wsRes.Cells[15, 1].Value = "Comisión Sobre Venta (" + rC.CsvPor + "%)";
                        wsRes.Cells[15, 2].Value = "$" + rC.ComisionSV.ToString("N2");
                        wsRes.Cells[16, 1].Value = "Total CSV";
                        wsRes.Cells[16, 2].Value = "$" + (rC.SubTotal + rC.Indirecto + rC.Utilidad + rC.ComisionSV).ToString("N2");
                        wsRes.Cells[17, 1].Value = "Comisión Externa (" + rC.ComisionExtPor + "%)";
                        wsRes.Cells[17, 2].Value = "$" + rC.ComisionExt.ToString("N2");
                        wsRes.Cells[18, 1].Value = "Total CE";
                        wsRes.Cells[18, 2].Value = "$" + (rC.SubTotal + rC.Indirecto + rC.Utilidad + rC.ComisionSV + rC.ComisionExt).ToString("N2");
                        wsRes.Cells[19, 1].Value = "Póliza de Cumplimiento";
                        wsRes.Cells[19, 2].Value = "$" + poliza.Value.ToString("N2");
                        wsRes.Cells[20, 1].Value = "Total";
                        wsRes.Cells[20, 2].Value = "$" + (rC.SubTotal + rC.Indirecto + rC.Utilidad + rC.ComisionSV + rC.ComisionExt + poliza.Value).ToString("N2");



                    }

                    //Prospecto
                    var pro = await _repo.ObtenerProspecto(idCotizacion);
                    if (pro != null)
                    {
                        var wsPro = package.Workbook.Worksheets[1];
                        wsPro.Cells[1, 1].Value = "Id Prospecto";
                        wsPro.Cells[1, 2].Value = pro.IdProspecto;
                        wsPro.Cells[2, 1].Value = "Nombre comercial";
                        wsPro.Cells[2, 2].Value = pro.NombreComercial;
                        wsPro.Cells[3, 1].Value = "Razón Social";
                        wsPro.Cells[3, 2].Value = pro.RazonSocial;
                        wsPro.Cells[4, 1].Value = "RFC";
                        wsPro.Cells[4, 2].Value = pro.Rfc;
                        wsPro.Cells[5, 1].Value = "Domicilio Fiscal";
                        wsPro.Cells[5, 2].Value = pro.DomicilioFiscal;
                        wsPro.Cells[6, 1].Value = "Nombre Contacto";
                        wsPro.Cells[6, 2].Value = pro.NombreContacto;
                        wsPro.Cells[7, 1].Value = "Email Contacto";
                        wsPro.Cells[7, 2].Value = pro.EmailContacto;
                        wsPro.Cells[8, 1].Value = "Numero Contacto";
                        wsPro.Cells[8, 2].Value = pro.NumeroContacto;
                        wsPro.Cells[9, 1].Value = "Ext Contacto";
                        wsPro.Cells[9, 2].Value = pro.ExtContacto;
                    }

                    //Directorio
                    var dir = await _repo.ObtenerDirecciones(idCotizacion);
                    if (dir.Count != 0)
                    {
                        var wsD = package.Workbook.Worksheets[2];
                        int row = 2;
                        foreach (var d in dir)
                        {
                            wsD.Cells[row, 1].Value = d.IdDireccionCotizacion;
                            wsD.Cells[row, 2].Value = d.NombreSucursal;
                            wsD.Cells[row, 3].Value = d.TipoInmueble;
                            wsD.Cells[row, 4].Value = d.Estado;
                            wsD.Cells[row, 5].Value = d.Municipio;
                            wsD.Cells[row, 6].Value = d.Ciudad;
                            wsD.Cells[row, 7].Value = d.Colonia;
                            wsD.Cells[row, 8].Value = d.Domicilio;
                            wsD.Cells[row, 9].Value = d.CodigoPostal;
                            row++;
                        }
                    }

                    //Plantilla
                    var plan = await _repo.ObtenerPlantillas(idCotizacion);
                    if(plan.Count != 0)
                    {
                        var wsP = package.Workbook.Worksheets[3];
                        int rowp = 2;
                        foreach(var p in plan)
                        {
                            wsP.Cells[rowp, 1].Value = p.IdPuestoDireccionCotizacion;
                            wsP.Cells[rowp, 2].Value = p.Puesto;
                            wsP.Cells[rowp, 3].Value = p.IdDireccionCotizacion;
                            wsP.Cells[rowp, 4].Value = p.Cantidad;
                            wsP.Cells[rowp, 5].Value = p.Clase;
                            wsP.Cells[rowp, 6].Value = p.Sueldo;
                            wsP.Cells[rowp, 7].Value = p.Aguinaldo;
                            wsP.Cells[rowp, 8].Value = p.Vacaciones;
                            wsP.Cells[rowp, 9].Value = p.PrimaVacacional;
                            wsP.Cells[rowp, 10].Value = p.ISN;
                            wsP.Cells[rowp, 11].Value = p.IMSS;
                            wsP.Cells[rowp, 12].Value = p.Bonos;
                            wsP.Cells[rowp, 13].Value = p.Vales;
                            wsP.Cells[rowp, 14].Value = p.Festivos;
                            wsP.Cells[rowp, 15].Value = p.Domingos;
                            wsP.Cells[rowp, 16].Value = p.CubreDescansos;
                            wsP.Cells[rowp, 17].Value = p.Total;
                            wsP.Cells[rowp, 18].Value = p.Jornada;
                            wsP.Cells[rowp, 19].Value = p.Turno;
                            wsP.Cells[rowp, 20].Value = p.Horario;
                            wsP.Cells[rowp, 21].Value = (DiaSemana)p.IdDiaDescanso;
                            wsP.Cells[rowp, 22].Value = (p.IdTieneMaterial == true)? "Si" : "No";
                            wsP.Cells[rowp, 23].Value = p.FechaAlta.ToString("dd-MM-yyyy");
                            rowp++;
                        }
                    } 
                    
                    //MaterialPlantilla
                    var mP = await _repo.ObtenerMaterialPlantillas(idCotizacion);
                    if (mP.Count != 0)
                    {
                        var wsMP = package.Workbook.Worksheets[4];
                        int rowMP = 2;
                        foreach (var m in mP)
                        {
                            wsMP.Cells[rowMP, 1].Value = m.IdMaterialCotizacion;
                            wsMP.Cells[rowMP, 2].Value = m.IdDireccionCotizacion;
                            wsMP.Cells[rowMP, 3].Value = m.IdPuestoDireccionCotizacion;
                            wsMP.Cells[rowMP, 4].Value = m.ClaveProducto;
                            wsMP.Cells[rowMP, 5].Value = m.DescripcionMaterial;
                            wsMP.Cells[rowMP, 6].Value = m.PrecioUnitario;
                            wsMP.Cells[rowMP, 7].Value = m.Cantidad;
                            wsMP.Cells[rowMP, 8].Value = m.Total;
                            wsMP.Cells[rowMP, 9].Value = m.ImporteMensual;
                            wsMP.Cells[rowMP, 10].Value = m.IdFrecuencia;
                            wsMP.Cells[rowMP, 11].Value = m.FechaAlta.ToString("dd-MM-yyyy");
                            rowMP++;
                        }
                    }

                    //UniformePlantilla
                    var uP = await _repo.ObtenerUniformePlantillas(idCotizacion);
                    if (uP.Count != 0)
                    {
                        var wsUP = package.Workbook.Worksheets[5];
                        int rowUP = 2;
                        foreach (var u in uP)
                        {
                            wsUP.Cells[rowUP, 1].Value = u.IdMaterialCotizacion;
                            wsUP.Cells[rowUP, 2].Value = u.IdDireccionCotizacion;
                            wsUP.Cells[rowUP, 3].Value = u.IdPuestoDireccionCotizacion;
                            wsUP.Cells[rowUP, 4].Value = u.ClaveProducto;
                            wsUP.Cells[rowUP, 5].Value = u.DescripcionMaterial;
                            wsUP.Cells[rowUP, 6].Value = u.PrecioUnitario;
                            wsUP.Cells[rowUP, 7].Value = u.Cantidad;
                            wsUP.Cells[rowUP, 8].Value = u.Total;
                            wsUP.Cells[rowUP, 9].Value = u.ImporteMensual;
                            wsUP.Cells[rowUP, 10].Value = u.IdFrecuencia;
                            wsUP.Cells[rowUP, 11].Value = u.FechaAlta.ToString("dd-MM-yyyy");
                            rowUP++;
                        }
                    }

                    //EquipoPlantilla
                    var eP = await _repo.ObtenerEquipoPlantillas(idCotizacion);
                    if (eP.Count != 0)
                    {
                        var wsEP = package.Workbook.Worksheets[6];
                        int rowEP = 2;
                        foreach (var e in eP)
                        {
                            wsEP.Cells[rowEP, 1].Value = e.IdMaterialCotizacion;
                            wsEP.Cells[rowEP, 2].Value = e.IdDireccionCotizacion;
                            wsEP.Cells[rowEP, 3].Value = e.IdPuestoDireccionCotizacion;
                            wsEP.Cells[rowEP, 4].Value = e.ClaveProducto;
                            wsEP.Cells[rowEP, 5].Value = e.DescripcionMaterial;
                            wsEP.Cells[rowEP, 6].Value = e.PrecioUnitario;
                            wsEP.Cells[rowEP, 7].Value = e.Cantidad;
                            wsEP.Cells[rowEP, 8].Value = e.Total;
                            wsEP.Cells[rowEP, 9].Value = e.ImporteMensual;
                            wsEP.Cells[rowEP, 10].Value = e.IdFrecuencia;
                            wsEP.Cells[rowEP, 11].Value = e.FechaAlta.ToString("dd-MM-yyyy");
                            rowEP++;
                        }
                    }

                    //HerramientaPlantilla
                    var hP = await _repo.ObtenerHerramientaPlantillas(idCotizacion);
                    if (hP.Count != 0)
                    {
                        var wsHP = package.Workbook.Worksheets[7];
                        int rowHP = 2;
                        foreach (var h in hP)
                        {
                            wsHP.Cells[rowHP, 1].Value = h.IdMaterialCotizacion;
                            wsHP.Cells[rowHP, 2].Value = h.IdDireccionCotizacion;
                            wsHP.Cells[rowHP, 3].Value = h.IdPuestoDireccionCotizacion;
                            wsHP.Cells[rowHP, 4].Value = h.ClaveProducto;
                            wsHP.Cells[rowHP, 5].Value = h.DescripcionMaterial;
                            wsHP.Cells[rowHP, 6].Value = h.PrecioUnitario;
                            wsHP.Cells[rowHP, 7].Value = h.Cantidad;
                            wsHP.Cells[rowHP, 8].Value = h.Total;
                            wsHP.Cells[rowHP, 9].Value = h.ImporteMensual;
                            wsHP.Cells[rowHP, 10].Value = h.IdFrecuencia;
                            wsHP.Cells[rowHP, 11].Value = h.FechaAlta.ToString("dd-MM-yyyy");
                            rowHP++;
                        }
                    }

                    //MaterialExtra
                    var mE = await _repo.ObtenerMaterialExtra(idCotizacion);
                    if (mE.Count != 0)
                    {
                        var wsME = package.Workbook.Worksheets[8];
                        int rowME = 2;
                        foreach (var m in mE)
                        {
                            wsME.Cells[rowME, 1].Value = m.IdMaterialCotizacion;
                            wsME.Cells[rowME, 2].Value = m.IdDireccionCotizacion;
                            wsME.Cells[rowME, 3].Value = m.ClaveProducto;
                            wsME.Cells[rowME, 4].Value = m.DescripcionMaterial;
                            wsME.Cells[rowME, 5].Value = m.PrecioUnitario;
                            wsME.Cells[rowME, 6].Value = m.Cantidad;
                            wsME.Cells[rowME, 7].Value = m.Total;
                            wsME.Cells[rowME, 8].Value = m.ImporteMensual;
                            wsME.Cells[rowME, 9].Value = m.IdFrecuencia;
                            wsME.Cells[rowME, 10].Value = m.FechaAlta.ToString("dd-MM-yyyy");
                            rowME++;
                        }
                    }

                    //UniformeExtra
                    var uE = await _repo.ObtenerUniformeExtra(idCotizacion);
                    if (uE.Count != 0)
                    {
                        var wsUE = package.Workbook.Worksheets[9];
                        int rowUE = 2;
                        foreach (var u in uE)
                        {
                            wsUE.Cells[rowUE, 1].Value = u.IdMaterialCotizacion;
                            wsUE.Cells[rowUE, 2].Value = u.IdDireccionCotizacion;
                            wsUE.Cells[rowUE, 3].Value = u.ClaveProducto;
                            wsUE.Cells[rowUE, 4].Value = u.DescripcionMaterial;
                            wsUE.Cells[rowUE, 5].Value = u.PrecioUnitario;
                            wsUE.Cells[rowUE, 6].Value = u.Cantidad;
                            wsUE.Cells[rowUE, 7].Value = u.Total;
                            wsUE.Cells[rowUE, 8].Value = u.ImporteMensual;
                            wsUE.Cells[rowUE, 9].Value = u.IdFrecuencia;
                            wsUE.Cells[rowUE, 10].Value = u.FechaAlta.ToString("dd-MM-yyyy");
                            rowUE++;
                        }
                    }

                    //EquipoExtra
                    var eE = await _repo.ObtenerEquipoExtra(idCotizacion);
                    if (eE.Count != 0)
                    {
                        var wsEE = package.Workbook.Worksheets[10];
                        int rowEE = 2;
                        foreach (var e in eE)
                        {
                            wsEE.Cells[rowEE, 1].Value = e.IdMaterialCotizacion;
                            wsEE.Cells[rowEE, 2].Value = e.IdDireccionCotizacion;
                            wsEE.Cells[rowEE, 3].Value = e.ClaveProducto;
                            wsEE.Cells[rowEE, 4].Value = e.DescripcionMaterial;
                            wsEE.Cells[rowEE, 5].Value = e.PrecioUnitario;
                            wsEE.Cells[rowEE, 6].Value = e.Cantidad;
                            wsEE.Cells[rowEE, 7].Value = e.Total;
                            wsEE.Cells[rowEE, 8].Value = e.ImporteMensual;
                            wsEE.Cells[rowEE, 9].Value = e.IdFrecuencia;
                            wsEE.Cells[rowEE, 10].Value = e.FechaAlta.ToString("dd-MM-yyyy");
                            rowEE++;
                        }
                    }

                    //HerramientaExtra
                    var hE = await _repo.ObtenerHerramientaExtra(idCotizacion);
                    if (hE.Count != 0)
                    {
                        var wsHE = package.Workbook.Worksheets[11];
                        int rowHE = 2;
                        foreach (var h in hE)
                        {
                            wsHE.Cells[rowHE, 1].Value = h.IdMaterialCotizacion;
                            wsHE.Cells[rowHE, 2].Value = h.IdDireccionCotizacion;
                            wsHE.Cells[rowHE, 3].Value = h.ClaveProducto;
                            wsHE.Cells[rowHE, 4].Value = h.DescripcionMaterial;
                            wsHE.Cells[rowHE, 5].Value = h.PrecioUnitario;
                            wsHE.Cells[rowHE, 6].Value = h.Cantidad;
                            wsHE.Cells[rowHE, 7].Value = h.Total;
                            wsHE.Cells[rowHE, 8].Value = h.ImporteMensual;
                            wsHE.Cells[rowHE, 9].Value = h.IdFrecuencia;
                            wsHE.Cells[rowHE, 10].Value = h.FechaAlta.ToString("dd-MM-yyyy");
                            rowHE++;
                        }
                    }

                    //Servicio
                    var sE = await _repo.ObtenerServicioExtra(idCotizacion);
                    if (sE.Count != 0)
                    {
                        var wsSE = package.Workbook.Worksheets[12];
                        int rowSE = 2;
                        foreach (var s in sE)
                        {
                            wsSE.Cells[rowSE, 1].Value = s.IdMaterialCotizacion;
                            wsSE.Cells[rowSE, 2].Value = s.IdDireccionCotizacion;
                            wsSE.Cells[rowSE, 3].Value = s.ClaveProducto;
                            wsSE.Cells[rowSE, 4].Value = s.PrecioUnitario;
                            wsSE.Cells[rowSE, 5].Value = s.Cantidad;
                            wsSE.Cells[rowSE, 6].Value = s.Total;
                            wsSE.Cells[rowSE, 7].Value = s.ImporteMensual;
                            wsSE.Cells[rowSE, 8].Value = s.IdFrecuencia;
                            wsSE.Cells[rowSE, 9].Value = s.FechaAlta.ToString("dd-MM-yyyy");
                            rowSE++;
                        }
                    }

                    return package.GetAsByteArray();
                }
            }
        }

        public async Task<bool> CargaProductoExtra(IFormFile archivo, int idCotizacion, string tipo, int idPersonal)
        {
            var productos = new List<MaterialCotizacionDTO>();
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
                        var producto = new MaterialCotizacionDTO
                        {
                            IdDireccionCotizacion = Convert.ToInt32(worksheet.Cells[row, 1].Value),
                            ClaveProducto = worksheet.Cells[row, 2].Value.ToString().ToString(),
                            Cantidad = Convert.ToInt32(worksheet.Cells[row, 3].Value),
                            IdFrecuencia = (Frecuencia)Convert.ToInt64(worksheet.Cells[row, 4].Value),
                            //LLenar con datos enduro
                            IdMaterialCotizacion = 0,
                            IdCotizacion = idCotizacion,
                            IdPuestoDireccionCotizacion = 0,
                            PrecioUnitario = 0,
                            Total = 0,
                            FechaAlta = DateTime.Now,
                            IdPersonal = idPersonal
                        };
                        productos.Add(producto);
                    }
                    bool valida = ValidarProductos(productos, tipo);
                    if (valida)
                    {
                        foreach (var producto in productos)
                        {
                            switch (tipo)
                            {
                                case "material":
                                    {
                                        await _materialService.AgregarMaterialOperario(producto);
                                    }
                                    break;
                                case "equipo":
                                    {
                                        await _materialService.AgregarEquipoOperario(producto);
                                    }
                                    break;
                                case "herramienta":
                                    {
                                        await _materialService.AgregarHerramientaOperario(producto);
                                    }
                                    break;
                                case "uniforme":
                                    {
                                        await _materialService.AgregarUniformeOperario(producto);
                                    }
                                    break;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        throw new Exception("Uno o mas productos no pertenecen a la familia " + tipo);
                    }
                    
                }
            }
        }
        protected bool ValidarProductos(List<MaterialCotizacionDTO> productos, string tipo)
        {
            bool result = false;
            string comparacion = "";

            switch( tipo)
            {
                case "material":
                    {
                        comparacion = "M-";
                        break;
                    }
                case "equipo":
                    {
                        comparacion = "H-";
                        break;
                    }
                case "herramienta":
                    {
                        comparacion = "H-";
                        break;
                    }
                case "uniforme":
                    {
                        comparacion = "H-UNI";
                        break;
                    }
            }
            foreach (var producto in productos)
            {
                if (producto.ClaveProducto.StartsWith(comparacion))
                {
                    result = true;
                }
                else
                {
                    result = false;
                    return result;
                }
            }
            return result;
        }
    }
}
