using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Admin
{
    using System.Text.Json.Serialization;

    namespace Comun.Areas.Admin
    {
        public class DtoCarrusel
        {
            [JsonPropertyName("consecutivo")]
            public int consecutivo { get; set; }  // ✅ antes string

            [JsonPropertyName("contentType")]
            public string? contentType { get; set; }

            [JsonPropertyName("fileName")]
            public string? fileName { get; set; }

            [JsonPropertyName("foto")]
            public string? foto { get; set; }
        }
    }


}
