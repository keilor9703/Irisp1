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



        [JsonPropertyName("CONSECUTIVO")]
        public string? CONSECUTIVO { get; set; }


        [JsonPropertyName("DESCRIPCION_DEPENDENCIA")]
        public string? DESCRIPCION_DEPENDENCIA { get; set; }

        [JsonPropertyName("SIGLA")]
        public string? SIGLA { get; set; }

    }
}
