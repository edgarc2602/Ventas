using Newtonsoft.Json;
using System.Collections.Generic;

namespace SistemaVentasBatia.DTOs
{
    public class DireccionResponseAPIDTO
    {
        [JsonProperty("error")]
        public bool Error { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("codigo_postal")]
        public DireccionAPIDTO CodigoPostal { get; set; }
    }
}
