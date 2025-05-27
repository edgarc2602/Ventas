using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.Models
{
    public class PlantillaUniforme
    {
        public int IdPlantillaUniforme { get; set; }

        public int IdPuesto { get; set; }

        public int Prendas { get; set; }

        public decimal? Costo { get; set; }

        public Frecuencia IdFrecuencia { get; set; }

        public int IdEstatusPlantillaUnfirome { get; set; }
    }
}
