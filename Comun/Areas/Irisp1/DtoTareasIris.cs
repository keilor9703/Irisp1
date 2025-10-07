using System;
using System.Text.Json.Serialization;

namespace Comun.Areas.Irisp1
{
    public class DtoTareasIris
    {
        [JsonPropertyName("TareaId")]
        public string? TareaId { get; set; }  // Cambiado a string

        [JsonPropertyName("ResponValidacionId")]
        public string? ResponValidacionId { get; set; } // Cambiado a string

        [JsonPropertyName("IdListaTareas")]
        public long? IdListaTareas { get; set; }

        [JsonPropertyName("IdEstadoTarea")]
        public long? IdEstadoTarea { get; set; }

        [JsonPropertyName("Vigente")]
        public int? Vigente { get; set; }

        [JsonPropertyName("EstadoExistencia")]
        public int? EstadoExistencia { get; set; }

        [JsonPropertyName("IdentificacionCreacion")]
        public long? IdentificacionCreacion { get; set; }

        [JsonPropertyName("IdentificacionModifica")]
        public long? IdentificacionModifica { get; set; }

        [JsonPropertyName("ResultadoId")]
        public string? ResultadoId { get; set; }

        [JsonPropertyName("IdTipo")]
        public long? IdTipo { get; set; }

        [JsonPropertyName("NroSpoaSiedco")]
        public string? NroSpoaSiedco { get; set; } // Cambiado a string

        [JsonPropertyName("FechaVerifica")]
        public DateTime? FechaVerifica { get; set; }

        [JsonPropertyName("FechaCreacion")]
        public DateTime? FechaCreacion { get; set; }

        [JsonPropertyName("FechaModifica")]
        public DateTime? FechaModifica { get; set; }

        [JsonPropertyName("FechaCreaResultado")]
        public DateTime? FechaCreaResultado { get; set; }

        [JsonPropertyName("FechaResultado")]
        public DateTime? FechaResultado { get; set; }

        [JsonPropertyName("DescListaTarea")]
        public string? DescListaTarea { get; set; }

        [JsonPropertyName("Observacion")]
        public string? Observacion { get; set; }

        [JsonPropertyName("Justificacion")]
        public string? Justificacion { get; set; }

        [JsonPropertyName("MaquinaCreacion")]
        public string? MaquinaCreacion { get; set; }

        [JsonPropertyName("MaquinaModifica")]
        public string? MaquinaModifica { get; set; }

        [JsonPropertyName("DescEstadoTarea")]
        public string? DescEstadoTarea { get; set; }

        [JsonPropertyName("EstadoTareasGrilla")]
        public string? EstadoTareasGrilla { get; set; }

        [JsonPropertyName("Evidencia")]
        public string? Evidencia { get; set; }

        [JsonPropertyName("Unidad")]
        public string? Unidad { get; set; }

        [JsonPropertyName("SiglaUnidadResp")]
        public string? SiglaUnidadResp { get; set; }

        [JsonPropertyName("DescTipoResultado")]
        public string? DescTipoResultado { get; set; }

        [JsonPropertyName("ObservacionResultado")]
        public string? ObservacionResultado { get; set; }

        [JsonPropertyName("DescUnidad")]
        public string? DescUnidad { get; set; }

        [JsonPropertyName("Seguimiento")]
        public string? Seguimiento { get; set; }

        [JsonPropertyName("CriminalidadId")]
        public string? CriminalidadId { get; set; }
    }

}
