using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Mod_Uno
{
    public class CriminalidadDTO
    {
        [JsonPropertyName("CriminalidadId")]
        public string? CriminalidadId { get; set; }

        [JsonPropertyName("IdentificacionInforma")]
        public Int64? IdentificacionInforma { get; set; }

        [JsonPropertyName("IdUnidad")]
        public Int32 IdUnidad { get; set; }

        [JsonPropertyName("IdZona")]
        public Int32 IdZona { get; set; }

        [JsonPropertyName("Celular")]
        public string? Celular { get; set; }

        [JsonPropertyName("IdTipoServicio")]
        public Int32 IdTipoServicio { get; set; }

        [JsonPropertyName("IdCuadrante")]
        public Int32 IdCuadrante { get; set; }

        [JsonPropertyName("IdClase")]
        public Int32 IdClase { get; set; }

        [JsonPropertyName("NombreClase")]
        public string? NombreClase { get; set; }

        [JsonPropertyName("FechaInicioExistencia")]
        public string? FechaInicioExistencia { get; set; }

        [JsonPropertyName("CantidadIntegrante")]
        public Int32 CantidadIntegrante { get; set; }

        [JsonPropertyName("CaracteristicasGenerales")]
        public string? CaracteristicasGenerales { get; set; }

        [JsonPropertyName("Vigente")]
        public Int32 Vigente { get; set; }

        [JsonPropertyName("Codigo")]
        public string? Codigo { get; set; }

        [JsonPropertyName("ConsecutivoCodigo")]
        public Int32 ConsecutivoCodigo { get; set; }

        [JsonPropertyName("SiglaUnidad")]
        public string? SiglaUnidad { get; set; }

        [JsonPropertyName("IdEstado")]
        public Int32 IdEstado { get; set; }

        [JsonPropertyName("IdFuente")]
        public Int32 IdFuente { get; set; }

        [JsonPropertyName("IdEstadoExistencia")]
        public Int32 IdEstadoExistencia { get; set; }

        [JsonPropertyName("DescripcionTramite")]
        public string? DescripcionTramite { get; set; }

        [JsonPropertyName("EntornoAfectado")]
        public Int32 EntornoAfectado { get; set; }

        [JsonPropertyName("IdTiempoDelito")]
        public Int32 IdTiempoDelito { get; set; }

        [JsonPropertyName("Clasificacion")]
        public Int32 Clasificacion { get; set; }

        [JsonPropertyName("ModalidadExpendio")]
        public Int32 ModalidadExpendio { get; set; }

        [JsonPropertyName("Origen")]
        public string? Origen { get; set; }

        [JsonPropertyName("NombreEntornoAfectado")]
        public string? NombreEntornoAfectado { get; set; }

        [JsonPropertyName("EspecialidadAportaInfo")]
        public Int32 EspecialidadAportaInfo { get; set; }

        [JsonPropertyName("IdCriminalidad")]
        public Int32 IdCriminalidad { get; set; }

        [JsonPropertyName("Usuario")]
        public Int32 Usuario { get; set; }

        [JsonPropertyName("Maquina")]
        public string? Maquina { get; set; }

    }
}

