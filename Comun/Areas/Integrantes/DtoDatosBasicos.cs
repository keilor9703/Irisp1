using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Integrantes
{
    public class DtoDatosBasicos
    {


        [JsonPropertyName("alias")]
        public string? Alias { get; set; }

        [JsonPropertyName("nombre")]
        public string? Nombre { get; set; }

        [JsonPropertyName("apellido")]
        public string? Apellido { get; set; }

        [JsonPropertyName("observacion")]
        public string? Observacion { get; set; }

    }
}
