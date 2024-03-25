using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Models
{
    public class ProductoPrecio
    {
        public string Clave { get; set; }

        public int IdProveedor { get; set; }

        public decimal Precio { get; set; }
    }
}
