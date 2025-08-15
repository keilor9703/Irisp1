using System.Text.Json.Serialization;

namespace Comun.Areas.Admin
{
    public class DtoFuncionarios
    {
        
        
        [JsonPropertyName("Identificacion")]
        public decimal IDENTIFICACION { get; set; }

        [JsonPropertyName("GradAlfabetico")]
        public string? GRADALFABETICO { get; set; }

        [JsonPropertyName("NombreGrado")]
        public string? NOMBREGRADO { get; set; }

        [JsonPropertyName("Apellidos")]
        public string? APELLIDOS { get; set; }

        [JsonPropertyName("Nombres")]
        public string? NOMBRES { get; set; }

        [JsonPropertyName("Funcionario")]
        public string? FUNCIONARIO { get; set; }

        [JsonPropertyName("CodigoCargo")]
        public decimal CODIGOCARGO { get; set; }

        [JsonPropertyName("CargoActual")]
        public string? CARGOACTUAL { get; set; }

        [JsonPropertyName("SituacionLaboral")]
        public string? SITUACIONLABORAL { get; set; }

        [JsonPropertyName("Correo")]
        public string? CORREO { get; set; }

        [JsonPropertyName("Usuario")]
        public string? USUARIOINSTITUCIONAL { get; set; }

        [JsonPropertyName("Celular")]
        public decimal CELULAR { get; set; }

        [JsonPropertyName("Fisica")]
        public string? FISICA { get; set; }

        [JsonPropertyName("Dependencia")]
        public string? DEPENDENCIA { get; set; }

        [JsonPropertyName("Direccion")]
        public string DIRECCION { get; set; }

        [JsonPropertyName("UndeLaborando")]
        public int UNDELABORANDO { get; set; }


        [JsonPropertyName("Estacion")]
        public string ESTACION { get; set; }
    }
}
