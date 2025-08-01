using System.Text.Json.Serialization;

namespace Comun.Areas.Clientes
{
    public class DtoKardex
    {
        [JsonPropertyName("IdKardex")]
        public Int32 IdKardex { get; set; }


        [JsonPropertyName("Apellidos")]
        public string? Apellidos { get; set; }


        [JsonPropertyName("Nombres")]
        public string? Nombres { get; set; }


        [JsonPropertyName("Identificacion")]
        public Int64 Identificacion { get; set; }


        [JsonPropertyName("FechaNace")]
        public string? FechaNace { get; set; }


        [JsonPropertyName("IdDto")]
        public Int32 IdDto { get; set; }


        [JsonPropertyName("IdLugar")]
        public Int32 IdLugar { get; set; }


        [JsonPropertyName("Direccion")]
        public string? Direccion { get; set; }


        [JsonPropertyName("IdGenero")]
        public Int32 IdGenero { get; set; }

        [JsonPropertyName("Unidad")]
        public string? Unidad { get; set; }


        [JsonPropertyName("Dependencia")]
        public string? Dependencia { get; set; }


        [JsonPropertyName("Observaciones")]
        public string? Observaciones { get; set; }


        [JsonPropertyName("Usuario")]
        public Int64 Usuario { get; set; }


        [JsonPropertyName("Maquina")]
        public string? Maquina { get; set; }
    }
}
