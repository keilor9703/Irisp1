using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Irisp1
{
    public class DtoCuadrantes
    {
        [JsonPropertyName("CODIGOC")]
        public Int64? CODIGOC { get; set; }

        [JsonPropertyName("DESCRIPCION")]
        public string? DESCRIPCION { get; set; }

    }
}
