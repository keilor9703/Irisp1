using System.Text.Json.Serialization;

namespace Comun.Areas.Control
{
    public class DtoRegionIrisp1
    {
        [JsonPropertyName("RegionCodigo")]
        public int? RegionCodigo { get; set; }

        [JsonPropertyName("RegionDescripcion")]
        public string? RegionDescripcion { get; set; }
    }
}
