using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Integrantes
{
    public class DtoListaIrisGeneral
    {

        [JsonPropertyName("estadoDescripcion")]
        public string? EstadoDescripcion { get; set; }

        [JsonPropertyName("estadoExistenciaDescripcion")]
        public string? EstadoExistenciaDescripcion { get; set; }

        [JsonPropertyName("codigo")]
        public string? Codigo { get; set; }

        [JsonPropertyName("dependencia")]
        public string? Dependencia { get; set; }

        [JsonPropertyName("municipio")]
        public string? Municipio { get; set; }

        [JsonPropertyName("zona")]
        public string? Zona { get; set; }

        [JsonPropertyName("clase")]
        public string? Clase { get; set; }

        [JsonPropertyName("fuente")]
        public string? Fuente { get; set; }

        [JsonPropertyName("nombreClase")]
        public string? NombreClase { get; set; }

        [JsonPropertyName("fechaInicioExistencia")]
        public DateTime? FechaInicioExistencia { get; set; }

        [JsonPropertyName("cantidadIntegrantes")]
        public int? CantidadIntegrantes { get; set; }

        [JsonPropertyName("caracteristicasGenerales")]
        public string? CaracteristicasGenerales { get; set; }

        [JsonPropertyName("tipoServicio")]
        public string? TipoServicio { get; set; }

        [JsonPropertyName("unidadResponsable")]
        public string? UnidadResponsable { get; set; }

        [JsonPropertyName("fechaVerificacionExistencia")]
        public DateTime? FechaVerificacionExistencia { get; set; }

        [JsonPropertyName("fechaRespuestaVerificacion")]
        public DateTime? FechaRespuestaVerificacion { get; set; }

        [JsonPropertyName("contadorVerificacionExistencia")]
        public string? ContadorVerificacionExistencia { get; set; }

        [JsonPropertyName("unidadProcesoInvestigativo")]
        public string? UnidadProcesoInvestigativo { get; set; }

        [JsonPropertyName("fechaProcesoInvestigativo")]
        public DateTime? FechaProcesoInvestigativo { get; set; }

        [JsonPropertyName("fechaRespuestaInvestigativo")]
        public DateTime? FechaRespuestaInvestigativo { get; set; }

        [JsonPropertyName("contadorProcesoInvestigativo")]
        public string? ContadorProcesoInvestigativo { get; set; }

        [JsonPropertyName("numeroSiedco")]
        public string? NumeroSiedco { get; set; }

        [JsonPropertyName("numeroSpoa")]
        public string? NumeroSpoa { get; set; }

        [JsonPropertyName("resultados")]
        public string? Resultados { get; set; }

        [JsonPropertyName("latitud")]
        public string? Latitud { get; set; }

        [JsonPropertyName("longitud")]
        public string? Longitud { get; set; }

    }
}
