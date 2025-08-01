using System.Text.Json.Serialization;

namespace Comun.Areas.Admin
{
    public class DtoMenu
    {
        [JsonPropertyName("IdMenu")]
        public decimal IDMENU { get; set; }

        [JsonPropertyName("Descripcion")]
        public string DESCRIPCION { get; set; }

        [JsonPropertyName("IdPadre")]
        public decimal IDPADRE { get; set; }

        [JsonPropertyName("Posicion")]
        public decimal POSICION { get; set; }

        [JsonPropertyName("Icono")]
        public string ICONO { get; set; }

        [JsonPropertyName("Detalle")]
        public string DETALLE { get; set; }

        [JsonPropertyName("Tipo")]
        public string TIPO { get; set; }

        [JsonPropertyName("Controlador")]
        public string CONTROLADOR { get; set; }

        [JsonPropertyName("Vista")]
        public string VISTA { get; set; }

        [JsonPropertyName("Area")]
        public string AREA { get; set; }

    }
} 
