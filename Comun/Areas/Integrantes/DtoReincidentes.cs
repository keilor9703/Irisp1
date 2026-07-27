using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Integrantes
{
    public class DtoReincidentes
    {


        [JsonPropertyName("reincidenteId")]
        public string ReincidenteId { get; set; }

        [JsonPropertyName("alias")]
        public string? Alias { get; set; }

        [JsonPropertyName("nombre")]
        public string? Nombre { get; set; }

        [JsonPropertyName("apellido")]
        public string? Apellido { get; set; }

        [JsonPropertyName("identificacion")]
        public long Identificacion { get; set; }


        [JsonPropertyName("observacion")]
        public string? Observacion { get; set; }

        [JsonPropertyName("idTipo")]
        public int IdTipo { get; set; }

        [JsonPropertyName("tipoId")]
        public string TipoId { get; set; }

        // Correlación con las apariciones reales del sujeto como integrante de casos IRISP1.
        [JsonPropertyName("totalCasos")]
        public int TotalCasos { get; set; }

        [JsonPropertyName("casosConResultados")]
        public int CasosConResultados { get; set; }

    }
}
