using System.Collections.Generic;

namespace SistemaVentasBatia.Models
{
    public class Usuario
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
        public List<UsuarioGrupo> Grupo { get; set; }
        public string DescripcionGrupoActivo { get; set; }
    }
    public class UsuarioGrupo
    {
        public int IdGrupo { get; set; }
        public string Descripcion { get; set; }
        public bool Principal { get; set; }
    }
}
