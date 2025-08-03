using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Integrantes
{
    public class DtoIntegrantes
    {

        [JsonPropertyName("integranteId")]
        public string IntegranteId { get; set; }

        [JsonPropertyName("criminalidadId")]
        public string? CriminalidadId { get; set; }

        [JsonPropertyName("alias")]
        public string? Alias { get; set; }

        [JsonPropertyName("nombre")]
        public string? Nombre { get; set; }

        [JsonPropertyName("apellido")]
        public string? Apellido { get; set; }

        [JsonPropertyName("cedula")]
        public long? Cedula { get; set; }

        [JsonPropertyName("idTipoInfo")]
        public int? IdTipoInfo { get; set; }

        [JsonPropertyName("vigente")]
        public int Vigente { get; set; }

        [JsonPropertyName("fechaCreacion")]
        public DateTime? FechaCreacion { get; set; }

        [JsonPropertyName("identificacionCreacion")]
        public long? IdentificacionCreacion { get; set; }

        [JsonPropertyName("maquinaCreacion")]
        public string? MaquinaCreacion { get; set; }

        [JsonPropertyName("fechaModifica")]
        public DateTime? FechaModifica { get; set; }

        [JsonPropertyName("identificacionModifica")]
        public long? IdentificacionModifica { get; set; }

        [JsonPropertyName("maquinaModifica")]
        public string? MaquinaModifica { get; set; }

        [JsonPropertyName("tipoDocumento")]
        public int? TipoDocumento { get; set; }

        [JsonPropertyName("celular")]
        public long? Celular { get; set; }

        [JsonPropertyName("direccion")]
        public string? Direccion { get; set; }

        [JsonPropertyName("idIntegrante")]
        public int? IdIntegrante { get; set; }

        [JsonPropertyName("idCriminalidad")]
        public int? IdCriminalidad { get; set; }

    }
}
