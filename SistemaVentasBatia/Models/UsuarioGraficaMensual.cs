﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Models
{
    public class UsuarioGraficaMensual
    {
        public int IdPersonal { get; set; }
        public string Nombre { get; set; }
        public int Mes { get; set; }
        public int CotizacionesPorMes { get; set; }
    }
}
