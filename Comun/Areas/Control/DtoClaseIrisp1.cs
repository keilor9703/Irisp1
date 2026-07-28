using System.Text.Json.Serialization;

namespace Comun.Areas.Control
{
    // Catálogo de clases (IRISP_CRIMINALIDAD.ID_CLASE, dominio PADRE_ID=12) para el filtro del tablero.
    public class DtoClaseIrisp1
    {
        [JsonPropertyName("IdClase")]
        public int? IdClase { get; set; }

        [JsonPropertyName("ClaseDescripcion")]
        public string? ClaseDescripcion { get; set; }
    }
}
