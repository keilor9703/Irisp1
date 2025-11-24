using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Integrantes
{
    public class DtoAntecedentesLogs
    {

        [JsonPropertyName("nombres")]
        public string? Nombres { get; set; }

        [JsonPropertyName("apellidos")]
        public string? Apellidos { get; set; }

        [JsonPropertyName("identificacion")]
        public decimal? Identificacion { get; set; }

        [JsonPropertyName("latitud")]
        public decimal? Latitud { get; set; }

        [JsonPropertyName("longitud")]
        public decimal? Longitud { get; set; }

        [JsonPropertyName("fecha_creacion")]
        public DateTime? FechaCreacion { get; set; }

        [JsonPropertyName("fecha_creacion_str")]
        public string? FechaCreacionStr => FechaCreacion?.ToString("dd/MM/yyyy HH:mm");

        [JsonPropertyName("clase")]
        public string? Clase { get; set; }
    }
}
