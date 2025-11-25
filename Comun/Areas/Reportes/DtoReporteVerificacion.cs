using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Reportes
{
    public class DtoReporteVerificacion
    {
        [JsonPropertyName("criminalidad_id")]
        public string? CriminalidadId { get; set; }

        [JsonPropertyName("codigo_irisp")]
        public string? CodigoIrisp { get; set; }

        [JsonPropertyName("cuenta_delitos_p")]
        public int CuentaDelitosP { get; set; }

        [JsonPropertyName("cuenta_delitos_c")]
        public int CuentaDelitosC { get; set; }

        [JsonPropertyName("cuenta_documentos")]
        public int CuentaDocumentos { get; set; }

        [JsonPropertyName("cuenta_informacion")]
        public int CuentaInformacion { get; set; }

        [JsonPropertyName("cuenta_integrantes")]
        public int CuentaIntegrantes { get; set; }

        [JsonPropertyName("cuenta_ubicaciones")]
        public int CuentaUbicaciones { get; set; }

        [JsonPropertyName("cuenta_unidad_responsable")]
        public int CuentaUnidadResponsable { get; set; }

        [JsonPropertyName("cuenta_responsable")]
        public int CuentaResponsable { get; set; }
    }
}
