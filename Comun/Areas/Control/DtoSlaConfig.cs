using System.Text.Json.Serialization;

namespace Comun.Areas.Control
{
    public class DtoSlaConfig
    {
        [JsonPropertyName("SlaConfigId")]
        public string? SlaConfigId { get; set; }

        [JsonPropertyName("IdListaTareas")]
        public int IdListaTareas { get; set; }

        [JsonPropertyName("DescTipoTarea")]
        public string? DescTipoTarea { get; set; }

        [JsonPropertyName("TiempoAlertaHoras")]
        public int TiempoAlertaHoras { get; set; }

        [JsonPropertyName("TiempoMaximoHoras")]
        public int TiempoMaximoHoras { get; set; }

        [JsonPropertyName("Vigente")]
        public int Vigente { get; set; }

        [JsonPropertyName("Justificacion")]
        public string? Justificacion { get; set; }

        [JsonPropertyName("FechaCreacion")]
        public DateTime? FechaCreacion { get; set; }

        [JsonPropertyName("FechaModifica")]
        public DateTime? FechaModifica { get; set; }
    }
}
