using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Irisp1
{
   public class DtoIrispCriminalidadResumen
    {
        // 🔹 Identificadores
        [JsonPropertyName("CriminalidadId")]
        public string? CriminalidadId { get; set; }

        [JsonPropertyName("IdResponsable")]
        public string? IdResponsable { get; set; }

        [JsonPropertyName("IdUnidad")]
        public long? IdUnidad { get; set; }

        [JsonPropertyName("IdUnidadResponsable")]
        public long? IdUnidadResponsable { get; set; }

        // 🔹 Unidad responsable y fechas de verificación
        [JsonPropertyName("UnidadResponsable")]
        public string? UnidadResponsable { get; set; }

        [JsonPropertyName("FechaVerificacionExistencia")]
        public DateTime? FechaVerificacionExistencia { get; set; }

        [JsonPropertyName("FechaRespuestaVerificacion")]
        public DateTime? FechaRespuestaVerificacion { get; set; }

        [JsonPropertyName("ContadorVerificacionExistencia")]
        public string? ContadorVerificacionExistencia { get; set; }

        // 🔹 Proceso investigativo
        [JsonPropertyName("UnidadProcesoInvestigativo")]
        public string? UnidadProcesoInvestigativo { get; set; }

        [JsonPropertyName("FechaProcesoInvestigativo")]
        public DateTime? FechaProcesoInvestigativo { get; set; }

        [JsonPropertyName("FechaRespuestaInvestigativo")]
        public DateTime? FechaRespuestaInvestigativo { get; set; }

        [JsonPropertyName("ContadorProcesoInvestigativo")]
        public string? ContadorProcesoInvestigativo { get; set; }

        // 🔹 Resultados SIEDCO / SPOA
        [JsonPropertyName("NumeroSiedco")]
        public string? NumeroSiedco { get; set; }

        [JsonPropertyName("NumeroSpoa")]
        public string? NumeroSpoa { get; set; }

        [JsonPropertyName("Resultados")]
        public string? Resultados { get; set; }

        [JsonPropertyName("EstadoResultados")]
        public string? EstadoResultados { get; set; }

        // 🔹 Datos de ubicación e identificación
        [JsonPropertyName("IdZona")]
        public int? IdZona { get; set; }

        [JsonPropertyName("IdentificacionInforma")]
        public long? IdentificacionInforma { get; set; }

        [JsonPropertyName("Celular")]
        public string? Celular { get; set; }

        [JsonPropertyName("IdTipoServicio")]
        public int? IdTipoServicio { get; set; }

        [JsonPropertyName("IdCuadrante")]
        public int? IdCuadrante { get; set; }

        [JsonPropertyName("IdClase")]
        public int? IdClase { get; set; }

        [JsonPropertyName("NombreClase")]
        public string? NombreClase { get; set; }

        [JsonPropertyName("FechaInicioExistencia")]
        public DateTime? FechaInicioExistencia { get; set; }

        [JsonPropertyName("CantidadIntegrantes")]
        public int? CantidadIntegrantes { get; set; }

        [JsonPropertyName("CaracteristicasGenerales")]
        public string? CaracteristicasGenerales { get; set; }

        // 🔹 Metadatos de auditoría
        [JsonPropertyName("Vigente")]
        public int? Vigente { get; set; }

        [JsonPropertyName("FechaCreacion")]
        public DateTime? FechaCreacion { get; set; }

        [JsonPropertyName("IdentificacionCrea")]
        public long? IdentificacionCrea { get; set; }

        [JsonPropertyName("MaquinaCrea")]
        public string? MaquinaCrea { get; set; }

        [JsonPropertyName("FechaModifica")]
        public DateTime? FechaModifica { get; set; }

        [JsonPropertyName("IdentificacionModifica")]
        public long? IdentificacionModifica { get; set; }

        [JsonPropertyName("MaquinaModifica")]
        public string? MaquinaModifica { get; set; }

        [JsonPropertyName("Codigo")]
        public string? Codigo { get; set; }

        [JsonPropertyName("ConsecutivoCodigo")]
        public int? ConsecutivoCodigo { get; set; }

        [JsonPropertyName("SiglaUnidad")]
        public string? SiglaUnidad { get; set; }

        [JsonPropertyName("IdEstado")]
        public int? IdEstado { get; set; }

        [JsonPropertyName("IdFuente")]
        public int? IdFuente { get; set; }

        [JsonPropertyName("IdEstadoExistencia")]
        public int? IdEstadoExistencia { get; set; }

        [JsonPropertyName("DescripcionTramite")]
        public string? DescripcionTramite { get; set; }

        [JsonPropertyName("EstadoDescripcion")]
        public string? EstadoDescripcion { get; set; }

        [JsonPropertyName("EstadoExistenciaDescripcion")]
        public string? EstadoExistenciaDescripcion { get; set; }

        // 🔹 Geografía y catálogos
        [JsonPropertyName("Municipio")]
        public string? Municipio { get; set; }

        [JsonPropertyName("Zona")]
        public string? Zona { get; set; }

        [JsonPropertyName("TipoServicio")]
        public string? TipoServicio { get; set; }

        [JsonPropertyName("Fuente")]
        public string? Fuente { get; set; }

        [JsonPropertyName("Clase")]
        public string? Clase { get; set; }

        // 🔹 Cuadrantes
        [JsonPropertyName("Cuadrante")]
        public string? Cuadrante { get; set; }

        [JsonPropertyName("DependCuadrante")]
        public string? DependCuadrante { get; set; }

        [JsonPropertyName("Estacioncuadrante")]
        public string? Estacioncuadrante { get; set; }

        [JsonPropertyName("Nivel1cuadrante")]
        public string? Nivel1cuadrante { get; set; }

        [JsonPropertyName("CelularCuadrante")]
        public long? CelularCuadrante { get; set; }

        [JsonPropertyName("Dependencia")]
        public string? Dependencia { get; set; }
    }
}

