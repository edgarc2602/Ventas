using AutoMapper;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Services
{
    public interface ITabuladorService
    {
        Task<int> Create(TabuladorDTO dto);
        Task<TabuladorDTO> Get(int id);
        Task<IEnumerable<CatalogoDTO>> GetPorEstado(int id);
        Task<bool> Update(TabuladorDTO dto);
        Task<bool> Delete(int id);
        Task<PuestoTabulador> ObtenerTabuladorPuesto(int id, int idClase);
    }

    public class TabuladorService : ITabuladorService
    {
        private readonly ITabuladorRepository _repo;
        private readonly IMapper _mapper;

        public TabuladorService(ITabuladorRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public Task<int> Create(TabuladorDTO dto)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<TabuladorDTO> Get(int id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<CatalogoDTO>> GetPorEstado(int id)
        {
            IEnumerable<CatalogoDTO> li = (await _repo.ObtenerPorEstado(id)).Select(t => new CatalogoDTO
            {
                Id = t.IdTabulador,
                Descripcion = t.Nombre,
                Clave = ""
            });

            return li;
        }

        public Task<bool> Update(TabuladorDTO dto)
        {
            throw new System.NotImplementedException();
        }

        public async Task<PuestoTabulador> ObtenerTabuladorPuesto(int id, int idClase)
        {
            PuestoTabulador result = new PuestoTabulador();

            result = await _repo.ObtenerTabuladorPuesto(id, idClase);
            
            return result;
        }
    }
}
