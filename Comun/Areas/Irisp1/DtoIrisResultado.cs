using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Irisp1
{
    public class DtoIrisResultado
    {
        public string ResultadoId { get; set; }            // GUID generado en la BD
        
        [JsonPropertyName("CriminalidadId")]
        public string? CriminalidadId { get; set; }
      
        public int? IdTipo { get; set; }                   // Tipo de resultado
        public string Numero { get; set; }                 // Nro SPOA / SIEDCO
        public string Observacion { get; set; }            // Observaciones
        public DateTime? Fecha { get; set; }               // Fecha SPOA / SIEDCO

        // Metadata (pueden venir del contexto y no del frontend)
        public int? Vigente { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public long? IdentificacionCrea { get; set; }
        public string MaquinaCreacion { get; set; }
        public DateTime? FechaModifica { get; set; }
        public long? IdentificacionModifica { get; set; }
        public string MaquinaModifica { get; set; }
    }
}
