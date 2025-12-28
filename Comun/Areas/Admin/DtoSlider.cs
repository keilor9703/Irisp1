using System.Text.Json.Serialization;

namespace Comun.Areas.Admin
{
    public class DtoSlider
    {
        [JsonPropertyName("Orden")]
        public decimal ORDEN { get; set; }
        [JsonPropertyName("Consecutivo")]
        public int? CONSECUTIVO { get; set; }
        [JsonPropertyName("ImagenesConsecutivas")]
        public int? IMAGENES_CONSECUTIVO { get; set; }
        [JsonPropertyName("Url")]
        public string? URL { get; set; }
        [JsonPropertyName("Filename")]
        public string? FILENAME { get; set; }
        [JsonPropertyName("UrlLink")]
        public string? URL_LINK { get; set; }
        [JsonPropertyName("Ruta")]
        public string? RUTA { get; set; }

    }
}
