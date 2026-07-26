using System.Text.Json.Serialization;

namespace Comun.Areas.Control
{
    public class DtoResultadoCasoIrisp1
    {
        [JsonPropertyName("CriminalidadId")]
        public string? CriminalidadId { get; set; }

        [JsonPropertyName("Codigo")]
        public string? Codigo { get; set; }

        [JsonPropertyName("Unidad")]
        public string? Unidad { get; set; }

        [JsonPropertyName("UnidadSigla")]
        public string? UnidadSigla { get; set; }

        [JsonPropertyName("Dependencia")]
        public string? Dependencia { get; set; }

        [JsonPropertyName("UnidadVerificacionSigla")]
        public string? UnidadVerificacionSigla { get; set; }

        [JsonPropertyName("UnidadVerificacion")]
        public string? UnidadVerificacion { get; set; }

        [JsonPropertyName("FechaCreacion")]
        public DateTime? FechaCreacion { get; set; }

        [JsonPropertyName("IdEstado")]
        public int? IdEstado { get; set; }

        [JsonPropertyName("DescEstado")]
        public string? DescEstado { get; set; }

        [JsonPropertyName("IdEstadoExistencia")]
        public int? IdEstadoExistencia { get; set; }

        [JsonPropertyName("DescEstadoExistencia")]
        public string? DescEstadoExistencia { get; set; }
    }
}
