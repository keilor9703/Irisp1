using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Irisp1
{
     public class DtoDocumentoIris
    {



        [JsonPropertyName("DocumentoId")]
        public string? DocumentoId { get; set; }


        [JsonPropertyName("Nombre")]
        public string? Nombre { get; set; }

        [JsonPropertyName("Url")]
        public string? Url { get; set; }

        [JsonPropertyName("Vigente")]
        public Int32? Vigente { get; set; }

        [JsonPropertyName("CriminalidadId")]
        public Int32? CriminalidadId { get; set; }

        [JsonPropertyName("FechaCreacion")]
        public string? FechaCreacion { get; set; }


        [JsonPropertyName("IdTipoInfo")]
        public Int32? IdTipoInfo { get; set; }

        [JsonPropertyName("DescTipoInfo")]
        public string? DescTipoInfo { get; set; }




    }
}
