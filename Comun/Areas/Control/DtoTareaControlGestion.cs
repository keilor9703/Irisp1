using System.Text.Json.Serialization;

namespace Comun.Areas.Control
{
    public class DtoTareaControlGestion
    {
        [JsonPropertyName("CriminalidadId")]
        public string? CriminalidadId { get; set; }

        [JsonPropertyName("Codigo")]
        public string? Codigo { get; set; }

        [JsonPropertyName("IdResponsable")]
        public string? IdResponsable { get; set; }

        [JsonPropertyName("Unidad")]
        public string? Unidad { get; set; }

        [JsonPropertyName("UnidadSigla")]
        public string? UnidadSigla { get; set; }

        [JsonPropertyName("TareaId")]
        public string? TareaId { get; set; }

        [JsonPropertyName("IdListaTareas")]
        public int? IdListaTareas { get; set; }

        [JsonPropertyName("DescListaTarea")]
        public string? DescListaTarea { get; set; }

        [JsonPropertyName("IdEstadoTarea")]
        public int? IdEstadoTarea { get; set; }

        [JsonPropertyName("DescEstadoTarea")]
        public string? DescEstadoTarea { get; set; }

        [JsonPropertyName("FechaCreacion")]
        public DateTime? FechaCreacion { get; set; }

        [JsonPropertyName("FechaModifica")]
        public DateTime? FechaModifica { get; set; }

        [JsonPropertyName("HorasTranscurridas")]
        public decimal? HorasTranscurridas { get; set; }

        [JsonPropertyName("TiempoAlertaHoras")]
        public int? TiempoAlertaHoras { get; set; }

        [JsonPropertyName("TiempoMaximoHoras")]
        public int? TiempoMaximoHoras { get; set; }

        [JsonPropertyName("EstadoSla")]
        public string? EstadoSla { get; set; }
    }
}
