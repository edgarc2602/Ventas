using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.Models
{
    public class Turno
    {
        public int IdTurno { get; set; }

        public string Descripcion { get; set; }

        public EstatusTurno IdEstatusTurno { get; set; }
    }
}
