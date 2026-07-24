using System.Text.Json.Serialization;

namespace Comun.Areas.Control
{
    public class DtoCasoControlGestion
    {
        [JsonPropertyName("CriminalidadId")]
        public string? CriminalidadId { get; set; }

        [JsonPropertyName("Codigo")]
        public string? Codigo { get; set; }

        [JsonPropertyName("Unidad")]
        public string? Unidad { get; set; }

        [JsonPropertyName("UnidadSigla")]
        public string? UnidadSigla { get; set; }

        [JsonPropertyName("FechaCreacion")]
        public DateTime? FechaCreacion { get; set; }

        [JsonPropertyName("IdEstado")]
        public int? IdEstado { get; set; }

        [JsonPropertyName("FechaFinalizacion")]
        public DateTime? FechaFinalizacion { get; set; }

        [JsonPropertyName("HorasTotalCaso")]
        public decimal? HorasTotalCaso { get; set; }

        [JsonPropertyName("VerifInicio")]
        public DateTime? VerifInicio { get; set; }

        [JsonPropertyName("VerifFin")]
        public DateTime? VerifFin { get; set; }

        [JsonPropertyName("HorasVerificacion")]
        public decimal? HorasVerificacion { get; set; }

        [JsonPropertyName("InvesInicio")]
        public DateTime? InvesInicio { get; set; }

        [JsonPropertyName("InvesFin")]
        public DateTime? InvesFin { get; set; }

        [JsonPropertyName("HorasInvestigacion")]
        public decimal? HorasInvestigacion { get; set; }
    }
}
