using System;
using System.Text.Json.Serialization;

namespace Comun.Areas.Integrantes
{
    public class DtoIntegrantes
    {
        [JsonPropertyName("INTEGRANTE_DIREC_ID")]
        public string INTEGRANTE_DIREC_ID { get; set; } 
        
        [JsonPropertyName("INTEGRANTE_ID")]
        public string INTEGRANTE_ID { get; set; }

        [JsonPropertyName("CRIMINALIDAD_ID")]
        public string? CRIMINALIDAD_ID { get; set; } 
        
        [JsonPropertyName("CRIMINALIDAD_DIREC_ID")]
        public string? CRIMINALIDAD_DIREC_ID { get; set; }

        [JsonPropertyName("ALIAS")]
        public string? ALIAS { get; set; }

        [JsonPropertyName("NOMBRE")]
        public string? NOMBRE { get; set; }

        [JsonPropertyName("APELLIDO")]
        public string? APELLIDO { get; set; }

        [JsonPropertyName("CEDULA")]
        public long? CEDULA { get; set; }

        [JsonPropertyName("ID_TIPO_INFO")]
        public int? ID_TIPO_INFO { get; set; }

        [JsonPropertyName("VIGENTE")]
        public int? VIGENTE { get; set; }

        [JsonPropertyName("FECHA_CREACION")]
        public DateTime? FECHA_CREACION { get; set; }

        [JsonPropertyName("IDENTIFICACION_CREACION")]
        public long? IDENTIFICACION_CREACION { get; set; }

        [JsonPropertyName("MAQUINA_CREACION")]
        public string? MAQUINA_CREACION { get; set; }

        [JsonPropertyName("FECHA_MODIFICA")]
        public DateTime? FECHA_MODIFICA { get; set; }

        [JsonPropertyName("IDENTIFICACION_MODIFICA")]
        public long? IDENTIFICACION_MODIFICA { get; set; }

        [JsonPropertyName("MAQUINA_MODIFICA")]
        public string? MAQUINA_MODIFICA { get; set; }

        [JsonPropertyName("TIPO_DOCUMENTO")]
        public int? TIPO_DOCUMENTO { get; set; }

        [JsonPropertyName("CELULAR")]
        public long? CELULAR { get; set; }

        [JsonPropertyName("DIRECCION")]
        public string? DIRECCION { get; set; }

        [JsonPropertyName("ID_INTEGRANTE")]
        public long? ID_INTEGRANTE { get; set; }

        [JsonPropertyName("ID_CRIMINALIDAD")]
        public long? ID_CRIMINALIDAD { get; set; }
    }
}
