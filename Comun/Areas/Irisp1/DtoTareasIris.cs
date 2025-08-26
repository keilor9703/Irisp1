using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;



namespace Comun.Areas.Irisp1
{
    public class DtoTareasIris
    {
        // ---------- Campos numéricos ----------
        [JsonPropertyName("TareaId")]
        public Int64? TareaId { get; set; }

        [JsonPropertyName("ResponValidacionId")]
        public Int64? ResponValidacionId { get; set; }

        [JsonPropertyName("IdListaTareas")]
        public Int64? IdListaTareas { get; set; }

        [JsonPropertyName("IdEstadoTarea")]
        public Int64? IdEstadoTarea { get; set; }

        [JsonPropertyName("Vigente")]
        public Int32? Vigente { get; set; }

        [JsonPropertyName("IdentificacionCreacion")]
        public Int64? IdentificacionCreacion { get; set; }

        [JsonPropertyName("IdentificacionModifica")]
        public Int64? IdentificacionModifica { get; set; }

        // ---------- Fechas ----------
        [JsonPropertyName("FechaVerifica")]
        public DateTime? FechaVerifica { get; set; }

        [JsonPropertyName("FechaCreacion")]
        public DateTime? FechaCreacion { get; set; }

        [JsonPropertyName("FechaModifica")]
        public DateTime? FechaModifica { get; set; }

        // ---------- Cadenas ----------
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
    }
}