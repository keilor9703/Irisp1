using System.Text.Json.Serialization;

namespace Comun.Areas.Irisp1
{
    public class DtoIrispCriminalidad
    {
        // ---------- Campos numéricos ----------
        [JsonPropertyName("IdCriminalidad")]
        public Int64? IdCriminalidad { get; set; }

        [JsonPropertyName("IdUnidad")]
        public Int64? IdUnidad { get; set; }

        [JsonPropertyName("IdUnidadResponsable")]
        public Int64? IdUnidadResponsable { get; set; }


        [JsonPropertyName("IdZona")]
        public Int64? IdZona { get; set; }

        [JsonPropertyName("IdentificacionInforma")]
        public Int64? IdentificacionInforma { get; set; }

        [JsonPropertyName("IdTipoServicio")]
        public Int32? IdTipoServicio { get; set; }

        [JsonPropertyName("IdCuadrante")]
        public Int32? IdCuadrante { get; set; }

        [JsonPropertyName("IdClase")]
        public Int32? IdClase { get; set; }

        [JsonPropertyName("CantidadIntegrantes")]
        public Int32? CantidadIntegrantes { get; set; }

        [JsonPropertyName("Vigente")]
        public Int32? Vigente { get; set; }

        [JsonPropertyName("CodigoDominio")]
        public Int32? CodigoDominio { get; set; }

        [JsonPropertyName("ConsecutivoCodigo")]
        public Int32? ConsecutivoCodigo { get; set; }

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

        [JsonPropertyName("EspecialidadAporta")]
        public int? EspecialidadAporta { get; set; }

        

        

        [JsonPropertyName("IdentificacionCrea")]
        public Int64? IdentificacionCrea { get; set; }

        [JsonPropertyName("IdentificacionModifica")]
        public Int64? IdentificacionModifica { get; set; }

        [JsonPropertyName("CODIGOC")]
        public Int64? CODIGOC { get; set; }

        // ---------- Fechas ----------
        [JsonPropertyName("FechaInicioExistencia")]
        public DateTime? FechaInicioExistencia { get; set; }

        [JsonPropertyName("FechaCreacion")]
        public DateTime? FechaCreacion { get; set; }

        [JsonPropertyName("FechaModifica")]
        public DateTime? FechaModifica { get; set; }

        [JsonPropertyName("FechaVerificacionExistencia")]
        public DateTime? FechaVerificacionExistencia { get; set; }

        [JsonPropertyName("FechaRespuestaVerificacion")]
        public DateTime? FechaRespuestaVerificacion { get; set; }

        [JsonPropertyName("FechaProcesoInvestigativo")]
        public DateTime? FechaProcesoInvestigativo { get; set; }

        [JsonPropertyName("FechaRespuestaInvestigativo")]
        public DateTime? FechaRespuestaInvestigativo { get; set; }

        // ---------- Nuevas fechas de tareas ----------
        [JsonPropertyName("FechaVerificaResponsable")]
        public DateTime? FechaVerificaResponsable { get; set; }

        [JsonPropertyName("FechaCreacionTareaRespon")]
        public DateTime? FechaCreacionTareaRespon { get; set; }

        // ---------- Cadenas ----------
        [JsonPropertyName("CriminalidadId")]
        public string? CriminalidadId { get; set; }

        [JsonPropertyName("Celular")]
        public string? Celular { get; set; }

        [JsonPropertyName("NombreClase")]
        public string? NombreClase { get; set; }

        [JsonPropertyName("CaracteristicasGenerales")]
        public string? CaracteristicasGenerales { get; set; }

        [JsonPropertyName("MaquinaCrea")]
        public string? MaquinaCrea { get; set; }

        [JsonPropertyName("MaquinaModifica")]
        public string? MaquinaModifica { get; set; }

        [JsonPropertyName("Codigo")]
        public string? Codigo { get; set; }

        [JsonPropertyName("SiglaUnidad")]
        public string? SiglaUnidad { get; set; }

        [JsonPropertyName("DescripcionTramite")]
        public string? DescripcionTramite { get; set; }

        [JsonPropertyName("ContadorVerificacionExistencia")]
        public string? ContadorVerificacionExistencia { get; set; }

        [JsonPropertyName("ContadorProcesoInvestigativo")]
        public string? ContadorProcesoInvestigativo { get; set; }

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

        [JsonPropertyName("UnidadProcesoInvestigativo")]
        public string? UnidadProcesoInvestigativo { get; set; }

        [JsonPropertyName("Resultados")]
        public string? Resultados { get; set; }

        [JsonPropertyName("DESCRIPCION")]
        public string? DESCRIPCION { get; set; }

        [JsonPropertyName("Id_modalidad")]
        public string? Id_modalidad { get; set; }

        [JsonPropertyName("IdDelitoPrincipal")]
        public string? IdDelitoPrincipal { get; set; }

        [JsonPropertyName("IdTipo")]
        public string? IdTipo { get; set; }

        [JsonPropertyName("IdTipoInfo")]
        public string? IdTipoInfo { get; set; }

        // ---------- Nuevas cadenas para datos de tareas ----------
        [JsonPropertyName("IdTarea")]
        public string? IdTarea { get; set; }

        [JsonPropertyName("IdResponsable")]
        public string? IdResponsable { get; set; }

        [JsonPropertyName("IdResponsableTarea")]
        public string? IdResponsableTarea { get; set; }

        [JsonPropertyName("IdListaTarea")]
        public string? IdListaTarea { get; set; }

        [JsonPropertyName("DescListaTarea")]
        public string? DescListaTarea { get; set; }

        [JsonPropertyName("ObservacionResponsable")]
        public string? ObservacionResponsable { get; set; }

        [JsonPropertyName("JustificacionResponsable")]
        public string? JustificacionResponsable { get; set; }

        [JsonPropertyName("IdEstadoTarea")]
        public string? IdEstadoTarea { get; set; }

        [JsonPropertyName("DescEstadoTarea")]
        public string? DescEstadoTarea { get; set; }

        [JsonPropertyName("UnidadResponsable")]
        public string? UnidadResponsable { get; set; }

        [JsonPropertyName("EstadoTareasGrilla")]
        public string? EstadoTareasGrilla { get; set; }

        [JsonPropertyName("IdTipoResultado")]
        public string? IdTipoResultado { get; set; }

        [JsonPropertyName("DescTipoResultado")]
        public string? DescTipoResultado { get; set; }

        [JsonPropertyName("NumeroResultado")]
        public string? NumeroResultado { get; set; }

        [JsonPropertyName("EstadoResultados")]
        public string? EstadoResultados { get; set; }

        // ---------- Listas ----------
        public List<int> IdDelitoSecundario { get; set; } = new List<int>();

        // ---------- Ubicación ----------
        [JsonPropertyName("UbicacionId")]
        public string? UbicacionId { get; set; }

        [JsonPropertyName("Latitud")]
        public string? Latitud { get; set; }

        [JsonPropertyName("Longitud")]
        public string? Longitud { get; set; }

        [JsonPropertyName("MunicipioUbica")]
        public string? MunicipioUbica { get; set; }

        [JsonPropertyName("Barrio")]
        public string? Barrio { get; set; }

        [JsonPropertyName("Cuadrante")]
        public string? Cuadrante { get; set; }

        [JsonPropertyName("CuadranteUbica")]
        public string? CuadranteUbica { get; set; }

        [JsonPropertyName("DependCuadrante")]
        public string? DependCuadrante { get; set; }

        [JsonPropertyName("Dependencia")]
        public string? Dependencia { get; set; }


        [JsonPropertyName("Estacioncuadrante")]
        public string? Estacioncuadrante { get; set; }

        [JsonPropertyName("Nivel1cuadrante")]
        public string? Nivel1cuadrante { get; set; }

        [JsonPropertyName("CelularCuadrante")]
        public Int64? CelularCuadrante { get; set; }

        [JsonPropertyName("RadioAccion")]
        public Int32? RadioAccion { get; set; }

        [JsonPropertyName("IdentificacionCreacion")]
        public long? IdentificacionCreacion { get; set; }

        [JsonPropertyName("MaquinaCreacion")]
        public string? MaquinaCreacion { get; set; }

        [JsonPropertyName("Direccion")]
        public string? Direccion { get; set; }

        [JsonPropertyName("CuadranteRural")]
        public string? CuadranteRural { get; set; }

        [JsonPropertyName("FuncionarioResponsable")]
        public string? FuncionarioResponsable { get; set; }


        [JsonPropertyName("UnidadFuncionarioResponsable")]
        public string? UnidadFuncionarioResponsable { get; set; }

        [JsonPropertyName("CodigoDane")]
        public Int32? CodigoDane { get; set; }

        [JsonPropertyName("CodigoEstacion")]
        public Int32? CodigoEstacion { get; set; }

        [JsonPropertyName("CodigoSiedcoCuadrante")]
        public Int32? CodigoSiedcoCuadrante { get; set; }

        [JsonPropertyName("IdUbicacion")]
        public Int32? IdUbicacion { get; set; }
    }
}
