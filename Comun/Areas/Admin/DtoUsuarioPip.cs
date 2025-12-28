using System.Text.Json.Serialization;

namespace Comun.Areas.Admin
{
    public class DtoUsuarioPip
    {
        #region Propiedades

        [JsonPropertyName("usuario")]
        public string Usuario { get; set; }

        [JsonPropertyName("clave")]
        public string Clave { get; set; }
        #endregion
    }
}
