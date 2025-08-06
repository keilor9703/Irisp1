using System.Text.Json.Serialization;

namespace Comun.Areas.Irisp1
{
    public class DtoIrispCriminalidad
    {
        [JsonPropertyName("CriminalidadId")]
        public string? CriminalidadId { get; set; }



        [JsonPropertyName("IdCriminalidad")]
        public Int64? IdCriminalidad { get; set; }



        [JsonPropertyName("IdUnidad")]
        public Int64? IdUnidad { get; set; }
        [JsonPropertyName("IdZona")]
        public Int64? IdZona { get; set; }
        [JsonPropertyName("IdentificacionInforma")]
        public Int64? IdentificacionInforma { get; set; }
        [JsonPropertyName("Celular")]
        public string? Celular { get; set; }
        [JsonPropertyName("IdTipoServicio")]
        public Int32? IdTipoServicio { get; set; }
        [JsonPropertyName("IdCuadrante")]
        public Int32? IdCuadrante { get; set; }
        [JsonPropertyName("IdClase")]
        public Int32? IdClase { get; set; }
        [JsonPropertyName("NombreClase")]
        public string? NombreClase { get; set; }
        [JsonPropertyName("FechaInicioExistencia")]
        public DateTime? FechaInicioExistencia { get; set; }
        [JsonPropertyName("CantidadIntegrantes")]
        public Int32? CantidadIntegrantes { get; set; }
        [JsonPropertyName("CaracteristicasGenerales")]
        public string? CaracteristicasGenerales { get; set; }
        [JsonPropertyName("Vigente")]
        public Int32? Vigente { get; set; }
        [JsonPropertyName("CodigoDominio")]
        public Int32? CodigoDominio { get; set; }
        [JsonPropertyName("FechaCreacion")]
        public DateTime? FechaCreacion { get; set; }
        [JsonPropertyName("IdentificacionCrea")]
        public Int32? IdentificacionCrea { get; set; }
        [JsonPropertyName("MaquinaCrea")]
        public string? MaquinaCrea { get; set; }
        [JsonPropertyName("FechaModifica")]
        public DateTime? FechaModifica { get; set; }
        [JsonPropertyName("IdentificacionModifica")]
        public Int32? IdentificacionModifica { get; set; }
        [JsonPropertyName("MaquinaModifica")]
        public string? MaquinaModifica { get; set; }
        [JsonPropertyName("Codigo")]
        public string? Codigo { get; set; }
        [JsonPropertyName("ConsecutivoCodigo")]
        public Int32? ConsecutivoCodigo { get; set; }
        [JsonPropertyName("SiglaUnidad")]
        public string? SiglaUnidad { get; set; }
        [JsonPropertyName("IdEstado")]
        public Int32? IdEstado { get; set; }
        [JsonPropertyName("IdFuente")]
        public Int32? IdFuente { get; set; }
        [JsonPropertyName("IdEstadoExistencia")]
        public Int32? IdEstadoExistencia { get; set; }
        
        [JsonPropertyName("EntornoAfectado")]
        public Int32? EntornoAfectado { get; set; }

        [JsonPropertyName("IdtiempoDelito")]
        public Int32? IdtiempoDelito { get; set; }

        [JsonPropertyName("Clasificacion")]
        public Int32? Clasificacion { get; set; }

        [JsonPropertyName("Modalidadexpendio")]
        public Int32? Modalidadexpendio { get; set; }

        [JsonPropertyName("DescripcionTramite")]
        public string? DescripcionTramite { get; set; }

        [JsonPropertyName("Origen")]
        public string? Origen { get; set; }

        [JsonPropertyName("NombreEntornoAfectado")]
        public string? NombreEntornoAfectado { get; set; }



        [JsonPropertyName("EstadoDescripcion")]
        public string? EstadoDescripcion { get; set; }
        [JsonPropertyName("EstadoExistenciaDescripcion")]
        public string? EstadoExistenciaDescripcion { get; set; }

        [JsonPropertyName("Municipio")]
        public string? Municipio { get; set; }
        [JsonPropertyName("DescripcionEstado")]
        public string? DescripcionEstado { get; set; }



        [JsonPropertyName("EspecialidadAporta")]
        public int? EspecialidadAporta { get; set; }


        [JsonPropertyName("Zona")]
        public string? Zona { get; set; }
        [JsonPropertyName("TipoServicio")]
        public string? TipoServicio { get; set; }
        [JsonPropertyName("Fuente")]
        public string? Fuente { get; set; }
        [JsonPropertyName("Clase")]
        public string? Clase { get; set; }
        [JsonPropertyName("UnidadVerificacionExiostencia")]
        public string? UnidadVerificacionExiostencia { get; set; }
        [JsonPropertyName("FechaVerificacionExistencia")]
        public DateTime? FechaVerificacionExistencia { get; set; }
        [JsonPropertyName("FechaRespuestaVerificacion")]
        public DateTime? FechaRespuestaVerificacion { get; set; }
        [JsonPropertyName("ContadorVerificacionExistencia")]
        public Int64? ContadorVerificacionExistencia { get; set; }
        [JsonPropertyName("UnidadProcesoInvestigativo")]
        public string? UnidadProcesoInvestigativo { get; set; }
        [JsonPropertyName("FechaProcesoInvestigativo")]
        public DateTime? FechaProcesoInvestigativo { get; set; }
        [JsonPropertyName("FechaRespuestaInvestigativo")]
        public DateTime? FechaRespuestaInvestigativo { get; set; }
        [JsonPropertyName("ContadorProcesoInvestigativo")]
        public Int64? ContadorProcesoInvestigativo { get; set; }
        [JsonPropertyName("Resultados")]
        public string? Resultados { get; set; }

        [JsonPropertyName("Codigo")]
        public Int32 CODIGO { get; set; }



        [JsonPropertyName("DESCRIPCION")]
        public string? DESCRIPCION { get; set; }
    
        [JsonPropertyName("Id_modalidad")]
        public string? Id_modalidad { get; set; }




    }
}
