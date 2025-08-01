using System.Text.Json.Serialization;

namespace Comun.Areas.Admin
{
    public class DtoRoles
    {
        [JsonPropertyName("IdRol")]
        public decimal IDROL { get; set; }

        [JsonPropertyName("Descripcion")]
        public string? DESCRIPCION { get; set; }
    }
}
