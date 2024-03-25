using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.Models
{
    public class Puesto
    {
        public int IdPuesto { get; set; }

        public string Descripcion { get; set; }

        public EstatusPuesto IdEstatusPuesto { get; set; }
    }
}
