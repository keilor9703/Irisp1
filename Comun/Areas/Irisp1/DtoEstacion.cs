using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Irisp1
{
    public class DtoEstacion
    {


       
            [JsonPropertyName("CONSECUTIVO")]
            public Int64? CONSECUTIVO { get; set; }

            [JsonPropertyName("DESCRIPCION_DEPENDENCIA")]
            public string? DESCRIPCION_DEPENDENCIA { get; set; }

        
    }
}
