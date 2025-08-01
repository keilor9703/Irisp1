using System.Text.Json.Serialization;

namespace Comun.Areas.Admin
{
    public class DtoUserRoles
    {

        [JsonPropertyName("IdRol")]
        public Int32 IdRol { get; set; }

        [JsonPropertyName("IdUserRol")]
        public Int32 IdUserRol { get; set; }

        [JsonPropertyName("IdUsuario")]
        public Int32 IdUsuario { get; set; }

        [JsonPropertyName("Descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [JsonPropertyName("FechaCreacion")]
        public string? FechaCreacion { get; set; }

        [JsonPropertyName("FuncionarioCreacion")]
        public string? FuncionarioCreacion { get; set; }

        [JsonPropertyName("Bloqueado")]
        public Int32 Bloqueado { get; set; }

        [JsonPropertyName("FechaFin")]
        public string? FechaFin { get; set; }

        [JsonPropertyName("Justificacion")]
        public string? Justificacion { get; set; }

    }
}
