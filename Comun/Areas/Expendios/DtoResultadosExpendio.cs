using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Expendios
{
    public class DtoResultadosExpendio
    {


        [JsonPropertyName("RESULTADO_DIREC_ID")]
        public string? RESULTADO_DIREC_ID { get; set; }


        [JsonPropertyName("CRIMINALIDAD_DIREC_ID")]
        public string? CRIMINALIDAD_DIREC_ID { get; set; }
        
        [JsonPropertyName("ID_TIPO")]
        public int? ID_TIPO { get; set; }

        [JsonPropertyName("DescTipo")]
        public string? DescTipo { get; set; }

        [JsonPropertyName("ID_SUBTIPO")]
        public int? ID_SUBTIPO { get; set; }

        [JsonPropertyName("DescSubTipo")]
        public string? DescSubTipo { get; set; }


        [JsonPropertyName("CANTIDAD")]
        public int? CANTIDAD { get; set; }


        [JsonPropertyName("FECHA")]
        public DateTime? FECHA { get; set; }
    }
}
