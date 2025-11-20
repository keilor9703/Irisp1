using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Expendios
{
    public class DtoInsExpendios
    {

        [JsonPropertyName("CRIMINALIDAD_ID")]
        public string? CRIMINALIDAD_ID { get; set; }

        [JsonPropertyName("ID_UNIDAD")]
        public int? ID_UNIDAD { get; set; }

        [JsonPropertyName("BARRIO")]
        public string? BARRIO { get; set; }

        [JsonPropertyName("DIRECCION")]
        public string? DIRECCION { get; set; }

        [JsonPropertyName("LONGITUD")]
        public string? LONGITUD { get; set; }

        [JsonPropertyName("LATITUD")]
        public string? LATITUD { get; set; }

        [JsonPropertyName("CUADRANTE")]
        public string? CUADRANTE { get; set; }


        [JsonPropertyName("FECHA_INICIO_EXISTENCIA")]
        public DateTime? FECHA_INICIO_EXISTENCIA { get; set; }

        [JsonPropertyName("CATEGORIA")]
        public int? CATEGORIA { get; set; }

        [JsonPropertyName("OTRA_CATEGORIA")]
        public string? OTRA_CATEGORIA { get; set; }

        [JsonPropertyName("MUNICIPIO")]
        public string? MUNICIPIO { get; set; }

        [JsonPropertyName("ID_UNIDAD_INFORMA")]
        public int? ID_UNIDAD_INFORMA { get; set; }

        [JsonPropertyName("ID_ZONA")]
        public int? ID_ZONA { get; set; }

        [JsonPropertyName("ID_CLASE")]
        public int? ID_CLASE { get; set; }

        [JsonPropertyName("ID_EXPENDIO")]
        public int? ID_EXPENDIO { get; set; }

        [JsonPropertyName("ID_ESTADO")]
        public int? ID_ESTADO { get; set; }

        [JsonPropertyName("ID_FUENTE")]
        public int? ID_FUENTE { get; set; }

        [JsonPropertyName("CARACTERISTICAS_GENERALES")]
        public string? CARACTERISTICAS_GENERALES { get; set; }

        [JsonPropertyName("IDENTIFICACION_CREA")]
        public long? IDENTIFICACION_CREA { get; set; }

        [JsonPropertyName("MAQUINA_CREACION")]
        public string? MAQUINA_CREACION { get; set; }

      


        public List<int> ID_DELITOS { get; set; } = new List<int>();
    }
}
