using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Irisp1
{
   public class DtoDelitosIris
    {


        [JsonPropertyName("DelitoId")]
        public string? DelitoId { get; set; }

        [JsonPropertyName("CriminalidadId")]
        public string? CriminalidadId { get; set; } 
        
        [JsonPropertyName("IdDelito")]
        public Int32? IdDelito { get; set; } 
        
        [JsonPropertyName("DelitoDesc")]
        public string? DelitoDesc { get; set; }

        [JsonPropertyName("IdTipo")]
        public Int32? IdTipo { get; set; } 
        
        [JsonPropertyName("DescTipo")]
        public string? DescTipo { get; set; }

        [JsonPropertyName("IdTipoInfo")]
        public Int32? IdTipoInfo { get; set; }

        [JsonPropertyName("DescTipoInfo")]
        public string? DescTipoInfo { get; set; }


        [JsonPropertyName("DelitoConsecutivo")]
        public Int32? DelitoConsecutivo { get; set; }


        [JsonPropertyName("IdCriminalidad")]
        public Int32? IdCriminalidad { get; set; }
    }
}
