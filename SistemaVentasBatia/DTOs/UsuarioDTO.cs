using System.Collections.Generic;

namespace SistemaVentasBatia.DTOs
{
    public class UsuarioDTO
    {
        public string Identificador { get; set; }
        public string Nombre { get; set; }
        public int IdPersonal { get; set; }
        public int IdInterno { get; set; }
        public int IdEmpleado { get; set; }
        public int Estatus { get; set; }
        public int IdAutoriza { get; set; }
        public int IdSupervisa { get; set; }
        public string DireccionIP { get; set; }
        public List<UsuarioGrupoDTO> Grupo { get; set; }
        public int IdGrupoActivo { get; set; }
        public string DescripcionGrupoActivo { get; set; }
    }
    public class UsuarioGrupoDTO
    {
        public int IdGrupo { get; set; }
        public string Descripcion { get; set; }
        public bool Principal { get; set; }
    }
}
