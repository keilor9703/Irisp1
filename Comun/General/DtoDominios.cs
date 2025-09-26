using System.Text.Json.Serialization;

namespace Comun.General
{
    public class DtoDominios
    {
        [JsonPropertyName("IdDominio")]
        public Int32 IdDominio { get; set; }


        [JsonPropertyName("Descripcion")]
        public string? Descripcion { get; set; }


        [JsonPropertyName("Descripcion2")]
        public string? Descripcion2 { get; set; }
        public int ANIO { get; set; }
    }
}
