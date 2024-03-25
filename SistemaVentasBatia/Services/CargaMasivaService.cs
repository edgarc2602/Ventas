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

namespace SistemaVentasBatia.Services
{
    public interface ICargaMasivaService
    {
        Task<bool> CargarDirecciones(int idCotizacion, int idProspecto, IFormFile archivo);
        Task<byte[]> ObtenerSucursalesLayout(int idCotizacion);
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
                            Bonos = Convert.ToDecimal(worksheet.Cells[row, 12].Value),
                            Vales = Convert.ToDecimal(worksheet.Cells[row, 13].Value),
                        };
                        //Parseo horas TimeSpan
                        int hrini = Convert.ToInt32(worksheet.Cells[row, 8].Value);
                        TimeSpan hrinif = TimeSpan.FromHours(hrini);
                        puesto.HrInicio = hrinif;
                        int hrfin = Convert.ToInt32(worksheet.Cells[row, 9].Value);
                        TimeSpan hrfinf = TimeSpan.FromHours(hrfin);
                        puesto.HrFin = hrfinf;
                        //Parseo dias Enum.DiaSemana
                        int diaini = Convert.ToInt32(worksheet.Cells[row, 10].Value);
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
                        int diafin = Convert.ToInt32(worksheet.Cells[row, 11].Value);
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
                        int diaFestivo = Convert.ToInt32(worksheet.Cells[row, 14].Value);
                        bool esDiaFestivo = (diaFestivo != 0);
                        puesto.DiaFestivo = esDiaFestivo;
                        int diaDomingo = Convert.ToInt32(worksheet.Cells[row, 15].Value);
                        bool esDiaDomingoo = (diaDomingo != 0);
                        puesto.DiaDomingo = esDiaDomingoo;
                        //LLenar con datos enduro
                        puesto.FechaAlta = DateTime.Now;
                        puesto.IdSalario = 0;
                        puesto.IdCotizacion = idCotizacion;
                        //Calcular sueldo
                        decimal sueldo = await _logicSal.GetSueldo(puesto.IdPuesto, puesto.IdClase, puesto.IdTabulador,(int)puesto.IdTurno);
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
                    foreach (var puesto in puestos)
                    {
                        await _logicCot.CrearPuestoDireccionCotizacion(puesto);
                    }
                }
                return true;
            }
        }
    }
}
