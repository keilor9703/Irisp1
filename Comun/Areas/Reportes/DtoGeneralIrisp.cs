using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Reportes
{
    public class DtoGeneralIrisp
    {
        // ==============================
        // DATOS BÁSICOS
        // ==============================

        [JsonPropertyName("estado")]
        public string? Estado { get; set; }

        [JsonPropertyName("estado_existencia")]
        public string? EstadoExistencia { get; set; }

        [JsonPropertyName("codigo")]
        public string? Codigo { get; set; }

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

        [JsonPropertyName("nombre_clase")]
        public string? NombreClase { get; set; }

        [JsonPropertyName("nro_cuadrante")]
        public string? NroCuadrante { get; set; }

        [JsonPropertyName("region_p")]
        public string? RegionP { get; set; }

        [JsonPropertyName("sigla_unidad")]
        public string? SiglaUnidad { get; set; }

        [JsonPropertyName("tipo_servicio")]
        public string? TipoServicio { get; set; }



        // ==============================
        // FECHAS Y STRING FORMAT
        // ==============================

        [JsonPropertyName("fecha_inicio_existencia")]
        public DateTime? FechaInicioExistencia { get; set; }

        [JsonPropertyName("fecha_inicio_existencia_str")]
        public string? FechaInicioExistenciaStr =>
            FechaInicioExistencia?.ToString("dd/MM/yyyy");

        [JsonPropertyName("fecha_creacion_irisp1")]
        public DateTime? FechaCreacionIrisp1 { get; set; }

        [JsonPropertyName("fecha_creacion_irisp1_str")]
        public string? FechaCreacionIrisp1Str =>
            FechaCreacionIrisp1?.ToString("dd/MM/yyyy");

        [JsonPropertyName("fecha_actualiza_vm")]
        public DateTime? FechaActualizaVm { get; set; }

        [JsonPropertyName("fecha_actualiza_vm_str")]
        public string? FechaActualizaVmStr =>
            FechaActualizaVm?.ToString("dd/MM/yyyy");

        [JsonPropertyName("fecha_asig_tarea_verifica")]
        public DateTime? FechaAsigVerifica { get; set; }

        [JsonPropertyName("fecha_asig_tarea_verifica_str")]
        public string? FechaAsigVerificaStr =>
            FechaAsigVerifica?.ToString("dd/MM/yyyy");

        [JsonPropertyName("fecha_resp_tarea_verifica")]
        public DateTime? FechaRespVerifica { get; set; }

        [JsonPropertyName("fecha_resp_tarea_verifica_str")]
        public string? FechaRespVerificaStr =>
            FechaRespVerifica?.ToString("dd/MM/yyyy");

        [JsonPropertyName("fecha_asig_tarea_inves")]
        public DateTime? FechaAsigInves { get; set; }

        [JsonPropertyName("fecha_asig_tarea_inves_str")]
        public string? FechaAsigInvesStr =>
            FechaAsigInves?.ToString("dd/MM/yyyy");

        [JsonPropertyName("fecha_resp_tarea_inves")]
        public DateTime? FechaRespInves { get; set; }

        [JsonPropertyName("fecha_resp_tarea_inves_str")]
        public string? FechaRespInvesStr =>
            FechaRespInves?.ToString("dd/MM/yyyy");



        // ==============================
        // CAMPOS NUMÉRICOS Y GEODATOS
        // ==============================

        [JsonPropertyName("latitud")]
        public decimal? Latitud { get; set; }

        [JsonPropertyName("longitud")]
        public decimal? Longitud { get; set; }

        [JsonPropertyName("cantidad_integrante")]
        public int? CantidadIntegrante { get; set; }

        [JsonPropertyName("cantidad_siedco")]
        public int? CantidadSiedco { get; set; }

        [JsonPropertyName("cantidad_spoa")]
        public int? CantidadSpoa { get; set; }



        // ==============================
        // TEXTO Y DESCRIPCIONES
        // ==============================

        [JsonPropertyName("caracteristicas_generales")]
        public string? CaracteristicasGenerales { get; set; }

        [JsonPropertyName("descripcion_tramite")]
        public string? DescripcionTramite { get; set; }

        [JsonPropertyName("barrio")]
        public string? Barrio { get; set; }

        [JsonPropertyName("direccion")]
        public string? Direccion { get; set; }

        [JsonPropertyName("origen")]
        public string? Origen { get; set; }



        // ==============================
        // PROCESOS / RESULTADOS
        // ==============================

        [JsonPropertyName("nunc")]
        public string? Nunc { get; set; }

        [JsonPropertyName("criminalidad_id")]
        public string? CriminalidadId { get; set; }

        [JsonPropertyName("delito_principal")]
        public string? DelitoPrincipal { get; set; }

        [JsonPropertyName("municipio_2")]
        public string? Municipio2 { get; set; }



        // ==============================
        // FUNCIONARIO INFORMA
        // ==============================

        [JsonPropertyName("funcionario_informa")]
        public string? FuncionarioInforma { get; set; }

        [JsonPropertyName("identificacion_informa")]
        public string? IdentificacionInforma { get; set; }

        [JsonPropertyName("unidad_funcionario_informa")]
        public string? UnidadFuncionarioInforma { get; set; }



        // ==============================
        // PROCESO INVESTIGATIVO
        // ==============================

        [JsonPropertyName("unidad_asig_inves")]
        public string? UnidadAsignacionInves { get; set; }



        // ==============================
        // VERIFICACIÓN
        // ==============================

        [JsonPropertyName("unidad_verifica")]
        public string? UnidadVerifica { get; set; }



        // ==============================
        // COMPATIBILIDAD CON GRILLA ANTERIOR
        // ==============================

        [JsonPropertyName("dependencia")]
        public string? Dependencia => Unidad;

        [JsonPropertyName("unidad_responsable")]
        public string? UnidadResponsable => UnidadVerifica;

        [JsonPropertyName("estado_descripcion")]
        public string? EstadoDescripcion => Estado;

        [JsonPropertyName("estado_existencia_descripcion")]
        public string? EstadoExistenciaDescripcion => EstadoExistencia;
    }
}
