using System.Text.Json.Serialization;

namespace Comun.Areas.Control
{
    public class DtoMapaIrisp1
    {
        [JsonPropertyName("CriminalidadId")]
        public string? CriminalidadId { get; set; }

        [JsonPropertyName("Codigo")]
        public string? Codigo { get; set; }

        [JsonPropertyName("SiglaUnidad")]
        public string? SiglaUnidad { get; set; }

        [JsonPropertyName("Municipio")]
        public string? Municipio { get; set; }

        [JsonPropertyName("Barrio")]
        public string? Barrio { get; set; }

        [JsonPropertyName("DelitoPrincipal")]
        public string? DelitoPrincipal { get; set; }

        [JsonPropertyName("Estado")]
        public string? Estado { get; set; }

        [JsonPropertyName("EstadoExistencia")]
        public string? EstadoExistencia { get; set; }

        [JsonPropertyName("FechaCreacion")]
        public DateTime? FechaCreacion { get; set; }

        [JsonPropertyName("Latitud")]
        public string? Latitud { get; set; }

        [JsonPropertyName("Longitud")]
        public string? Longitud { get; set; }
    }
}
