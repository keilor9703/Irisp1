using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Irisp1
{
   public  class DtoInfoAdicional
    {

        [JsonPropertyName("InfoId")]
        public string? InfoId { get; set; } 
        
        [JsonPropertyName("INFORMACION_DIREC_ID")]
        public string? INFORMACION_DIREC_ID { get; set; }

        [JsonPropertyName("CriminalidadId")]
        public string? CriminalidadId { get; set; }
        
        [JsonPropertyName("CRIMINALIDAD_DIREC_ID")]
        public string? CRIMINALIDAD_DIREC_ID { get; set; }

        [JsonPropertyName("Descripcion")]
        public string? Descripcion { get; set; }


        [JsonPropertyName("IdTipoInfo")]
        public Int32? IdTipoInfo { get; set; }

        [JsonPropertyName("DescTipoInfo")]
        public string? DescTipoInfo { get; set; }

    }
}
