using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.DTOs
{
    public class AlertaViewModel
    {
        public string Descripcion { get; set; }

        public TipoAlerta IdTipoAlerta { get; set; }
    }
}
