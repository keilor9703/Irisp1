using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Integrantes
{
    public class DtoReincidentes
    {


        [JsonPropertyName("reincidenteId")]
        public string ReincidenteId { get; set; }

        [JsonPropertyName("alias")]
        public string Alias { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("apellido")]
        public string Apellido { get; set; }

        [JsonPropertyName("identificacion")]
        public long Identificacion { get; set; }

        //[JsonPropertyName("VIGENTE")]
        //public int? Vigente { get; set; }

        //[JsonPropertyName("FECHA_CREACION")]
        //public DateTime? FechaCreacion { get; set; }

        //[JsonPropertyName("IDENTIFICACION_CREACION")]
        //public long? IdentificacionCreacion { get; set; }

        //[JsonPropertyName("MAQUINA_CREACION")]
        //public string MaquinaCreacion { get; set; }

        //[JsonPropertyName("FECHA_MODIFICA")]
        //public DateTime? FechaModifica { get; set; }

        //[JsonPropertyName("IDENTIFICACION_MODIFICA")]
        //public long? IdentificacionModifica { get; set; }

        //[JsonPropertyName("MAQUINA_MODIFICA")]
        //public string MaquinaModifica { get; set; }

        [JsonPropertyName("observacion")]
        public string Observacion { get; set; }

        [JsonPropertyName("idTipo")]
        public int IdTipo { get; set; }

        [JsonPropertyName("tipoId")]
        public string TipoId { get; set; }



    }
}
