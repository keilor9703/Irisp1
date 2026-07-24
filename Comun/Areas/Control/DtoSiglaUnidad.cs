using System.Text.Json.Serialization;

namespace Comun.Areas.Control
{
    public class DtoSiglaUnidad
    {
        [JsonPropertyName("SiglaUnidad")]
        public string? SiglaUnidad { get; set; }
    }
}
