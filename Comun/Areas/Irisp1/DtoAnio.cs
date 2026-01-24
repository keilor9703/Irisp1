using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Irisp1
{
    public class DtoAnio
    {

        [JsonPropertyName("AnoIrisp1")]
        public Int32 AnoIrisp1 { get; set; }
    }
}
