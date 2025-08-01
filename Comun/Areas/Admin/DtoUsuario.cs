using System.Text.Json.Serialization;

namespace Comun.Areas.Admin
{
    public class DtoUsuario
    {
        [JsonPropertyName("IdUsuario")]
        public Int32 IdUsuario { get; set; }

        [JsonPropertyName("IdCargo")]
        public Int32 IdCargo { get; set; }

        [JsonPropertyName("EmplConsecutivo")]
        public Int32 EmplConsecutivo { get; set; }

        [JsonPropertyName("EmplUndeConsecutivo")]
        public Int32 EmplUndeConsecutivo { get; set; }

        [JsonPropertyName("EmplUndeFuerza")]
        public Int32 EmplUndeFuerza { get; set; }

        [JsonPropertyName("Identificacion")]
        public Int64 Identificacion { get; set; }

        [JsonPropertyName("Usuario")]
        public string? Usuario { get; set; }

        [JsonPropertyName("GradAlfabetico")]
        public string? GradAlfabetico { get; set; }

        [JsonPropertyName("NombreGrado")]
        public string? NombreGrado { get; set; }
        [JsonPropertyName("Funcionario")]
        public string? Funcionario { get; set; }

        [JsonPropertyName("ApellidosNombres")]
        public string? ApellidosNombres { get; set; }

        [JsonPropertyName("Cargo")]
        public string? Cargo { get; set; }

        [JsonPropertyName("Dependencia")]
        public string? Dependencia { get; set; }

        [JsonPropertyName("Fisica")]
        public string? Fisica { get; set; }

        [JsonPropertyName("IdUndeLaborando")]
        public Int32 IdUndeLaborando { get; set; }

        [JsonPropertyName("Correo")]
        public string? Correo { get; set; }

        [JsonPropertyName("SituacionLaboral")]
        public string? SituacionLaboral { get; set; }

        [JsonPropertyName("Bloqueado")]
        public int Bloqueado { get; set; }

        [JsonPropertyName("Celular")]
        public Int64 Celular { get; set; }

        [JsonPropertyName("FechaCreacion")]
        public string? FechaCreacion { get; set; }

        [JsonPropertyName("DtoUserRoles")]
        public List<DtoUserRoles> DtoUserRoles { get; set; }

        [JsonPropertyName("Resultado")]
        public Int32? Resultado { get; set; }
    }
}
