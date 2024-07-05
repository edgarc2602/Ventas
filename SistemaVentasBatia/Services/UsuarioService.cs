using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Services
{
    public interface IUsuarioService
    {
        Task<UsuarioDTO> Login(AccesoDTO dto, UsuarioDTO usu);
        Task<bool> Existe(AccesoDTO dto);
        Task<bool> InsertarUsuario(UsuarioRegistro usuario);
        Task<List<UsuarioGrafica>> ObtenerCotizacionesUsuarios();
        Task<List<UsuarioGraficaMensual>> ObtenerCotizacionesMensuales();
        Task<List<AgregarUsuario>> ObtenerUsuarios();
        Task<List<AgregarUsuario>> ObtenerVendedores();
        Task<AgregarUsuario> ObtenerUsuarioPorIdPersonal(int idPersonal);
        Task<bool> ActualizarUsuario(AgregarUsuario usuario);
        Task<bool> ActivarUsuario(int idPersonal);
        Task<bool> DesactivarUsuario(int idPersonal);
        Task<bool> AgregarUsuario(AgregarUsuario usuario);
        Task<bool> EliminarUsuario(int idPersonal);
    }
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repo;
        private readonly IMapper _mapper;

        public UsuarioService(IUsuarioRepository repoUsuario, IMapper mapper)
        {
            _repo = repoUsuario;
            _mapper = mapper;
        }

        public async Task<bool> Existe(AccesoDTO dto)
        {
            bool existe = false;
            try
            {
                Acceso acc = _mapper.Map<Acceso>(dto);
                UsuarioDTO usu = _mapper.Map<UsuarioDTO>(await _repo.Login(acc));
                if (usu == null)
                {
                    existe = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return existe;
        }

        public async Task<UsuarioDTO> Login(AccesoDTO dto, UsuarioDTO usu)
        {
            dto.Contrasena = Encriptar(dto.Contrasena);
            string ip = usu.DireccionIP;
            try
            {
                Acceso acc = _mapper.Map<Acceso>(dto);
                usu = _mapper.Map<UsuarioDTO>(await _repo.Login(acc));
                if(usu != null)
                {
                    usu.DireccionIP = ip;
                }
                if (usu == null)
                {
                    throw new CustomException("Usuario no Existe");
                }
                if (usu.Estatus == 0)
                {
                    throw new CustomException("Usuario inactivo");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return usu;
        }
        public static string Encriptar(string pass)
        {
            // Convertir la contraseña a bytes UTF-8
            byte[] bytes = Encoding.UTF8.GetBytes(pass);

            // Crear un objeto SHA256 para calcular el hash
            using (SHA256 sha256 = SHA256.Create())
            {
                // Calcular el hash de la contraseña
                byte[] hashBytes = sha256.ComputeHash(bytes);

                // Convertir el hash a una cadena hexadecimal
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                // Devolver el hash en formato hexadecimal
                return builder.ToString();
            }
        }
        public async Task<bool> InsertarUsuario(UsuarioRegistro usuario)
        {
            bool existe = false;
            bool result = false;
            existe = await _repo.ConsultarUsuario(usuario.IdPersonal, usuario.Nombres);
            if (existe == true)
            {
                result = await _repo.InsertarUsuario(usuario);

            }
            else
            {
                result = false;
            }
            return result;
        }

        public async Task<List<UsuarioGrafica>> ObtenerCotizacionesUsuarios()
        {
            return await _repo.ObtenerCotizacionesUsuarios();
        }

        public async Task<List<UsuarioGraficaMensual>> ObtenerCotizacionesMensuales()
        {
            return await _repo.ObtenerCotizacionesMensuales();
        }

        public async Task<List<AgregarUsuario>> ObtenerUsuarios()
        {
            return await _repo.ObtenerUsuarios();
        }
        public async Task<List<AgregarUsuario>> ObtenerVendedores()
        {
            return await _repo.ObtenerVendedores();
        }

        public async Task<AgregarUsuario> ObtenerUsuarioPorIdPersonal(int idPersonal)
        {
            return await _repo.ObtenerUsuarioPorIdPersonal(idPersonal);
        }

        public async Task<bool> ActualizarUsuario(AgregarUsuario usuario)
        {
            var base64Data = usuario.Firma;

            if (base64Data.StartsWith("data:image/jpeg;base64,"))
            {
                base64Data = base64Data.Substring("data:image/jpeg;base64,".Length);
            }
            if (base64Data.StartsWith("data:image/png;base64,"))
            {
                base64Data = base64Data.Substring("data:image/png;base64,".Length);
            }
            usuario.Firma = base64Data;
            return await _repo.ActualizarUsuario(usuario);
        }

        public async Task<bool> ActivarUsuario(int idPersonal)
        {
            return await _repo.ActivarUsuario(idPersonal);
        }

        public async Task<bool> DesactivarUsuario(int idPersonal)
        {
            return await _repo.DesactivarUsuario(idPersonal);
        }

        public async Task<bool> AgregarUsuario(AgregarUsuario usuario)
        {
            int idPersonal = await _repo.ConsultarUsuario(usuario);
            usuario.IdPersonal = idPersonal;
            var base64Data = usuario.Firma;

            if (base64Data.StartsWith("data:image/jpeg;base64,"))
            {
                base64Data = base64Data.Substring("data:image/jpeg;base64,".Length);
            }
            if (base64Data.StartsWith("data:image/png;base64,"))
            {
                base64Data = base64Data.Substring("data:image/png;base64,".Length);
            }
            usuario.Firma = base64Data;
            return await _repo.AgregarUsuario(usuario);
        }

        public async Task<bool> EliminarUsuario(int idPersonal)
        {
            return await _repo.EliminarUsuario(idPersonal);
        }
    }
}
