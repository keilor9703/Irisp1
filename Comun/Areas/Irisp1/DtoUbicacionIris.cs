using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Irisp1
{
    public class DtoUbicacionIris
    {
        [JsonPropertyName("UbicacionId")]
        public string? UbicacionId { get; set; }

        [JsonPropertyName("Latitud")]
        public string? Latitud { get; set; }

        [JsonPropertyName("Longitud")]
        public string? Longitud { get; set; }

        [JsonPropertyName("DelitoDesc")]
        public string? DelitoDesc { get; set; }

        [JsonPropertyName("Barrio")]
        public string? Barrio { get; set; }

        [JsonPropertyName("MunicipioUbica")]
        public string? MunicipioUbica { get; set; }

        [JsonPropertyName("Cuadrante")]
        public string? CuadranteUbica { get; set; }

        [JsonPropertyName("RadioAccion")]
        public string? RadioAccion { get; set; }

        [JsonPropertyName("Vigente")]
        public Int32? Vigente { get; set; }

        [JsonPropertyName("FechaCreacion")]
        public DateTime? FechaCreacion { get; set; }

        [JsonPropertyName("IdentificacionCreacion")]
        public string? IdentificacionCreacion { get; set; }

        [JsonPropertyName("MaquinaCreacion")]
        public string? MaquinaCreacion { get; set; }

        [JsonPropertyName("Direccion")]
        public string? Direccion { get; set; }

        [JsonPropertyName("CodigoDane")]
        public string? CodigoDane { get; set; }

        [JsonPropertyName("CodigoEstacion")]
        public string? CodigoEstacion { get; set; }

        [JsonPropertyName("CodigoSiedcoCuadrante")]
        public string? CodigoSiedcoCuadrante { get; set; }

        [JsonPropertyName("CriminalidadId")]
        public string? CriminalidadId { get; set; }


        [JsonPropertyName("IdCriminalidad")]
        public Int64? IdCriminalidad { get; set; }
    }
}
