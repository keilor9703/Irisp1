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

        [JsonPropertyName("criminalidadId")]
        public string? CriminalidadId { get; set; }

        [JsonPropertyName("estado")]
        public string? Estado { get; set; }

        [JsonPropertyName("estadoExistencia")]
        public string? EstadoExistencia { get; set; }

        [JsonPropertyName("codigoIrisp")]
        public string? CodigoIrisp { get; set; }

        [JsonPropertyName("unidad")]
        public string? Unidad { get; set; }

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

        [JsonPropertyName("cantidadIntegrante")]
        public int? CantidadIntegrante { get; set; }

        [JsonPropertyName("caracteristicasGenerales")]
        public string? CaracteristicasGenerales { get; set; }

        [JsonPropertyName("descripcionTramite")]
        public string? DescripcionTramite { get; set; }

        [JsonPropertyName("alias")]
        public string? Alias { get; set; }

        [JsonPropertyName("nombre")]
        public string? Nombre { get; set; }

        [JsonPropertyName("apellido")]
        public string? Apellido { get; set; }

        [JsonPropertyName("cedula")]
        public long? Cedula { get; set; }

        [JsonPropertyName("tipoInfo")]
        public string? TipoInfo { get; set; }

        [JsonPropertyName("integranteId")]
        public string? IntegranteId { get; set; }

        [JsonPropertyName("idTipoInfo")]
        public int? IdTipoInfo { get; set; }

        [JsonPropertyName("fechaCreacion")]
        public DateTime? FechaCreacion { get; set; }

        [JsonPropertyName("fechaModifica")]
        public DateTime? FechaModifica { get; set; }

        [JsonPropertyName("identificacionCreacion")]
        public long? IdentificacionCreacion { get; set; }

        [JsonPropertyName("identificacionModifica")]
        public long? IdentificacionModifica { get; set; }

        [JsonPropertyName("maquinaCreacion")]
        public string? MaquinaCreacion { get; set; }

        [JsonPropertyName("maquinaModifica")]
        public string? MaquinaModifica { get; set; }

        [JsonPropertyName("vigente")]
        public int? Vigente { get; set; }

        [JsonPropertyName("observacion")]
        public string? Observacion { get; set; }

    }
}
