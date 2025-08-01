using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.Areas.AplicacionDTO
{ 
    public class CriminalidadDTO
{
    public string CRIMINALIDAD_ID { get; set; }

    [Display(Name = "Dependencia")]
    public decimal ID_UNIDAD { get; set; }

    [Display(Name = "Zona")]
    public decimal? ID_ZONA { get; set; }

    [Display(Name = "Tipo Servicio")]
    public decimal? ID_TIPO_SERVICIO { get; set; }

    [Display(Name = "Estado Existencia")]
    public decimal? ID_ESTADO_EXISTENCIA { get; set; }

    [Display(Name = "Estado Existencia")]
    public string ID_ESTADO_EXISTENCIA_STR { get; set; }

    [Display(Name = "Cuadrante")]
    public decimal? ID_CUADRANTE { get; set; }

    [Display(Name = "Clase")]
    public decimal? ID_CLASE { get; set; }

    [Display(Name = "Estado")]
    public Nullable<decimal> ID_ESTADO { get; set; }
    [Display(Name = "TiempoDelinque")]
    public Nullable<decimal> ID_TIEMPO_DELITO { get; set; }

    [Display(Name = "Fuente")]
    public Nullable<decimal> ID_FUENTE { get; set; }

    [Display(Name = "Identificación Policía que informa")]
    public long? IDENTIFICACION_INFORMA { get; set; }

    [Display(Name = "Celular")]
    public string CELULAR { get; set; }

    #region Listas Adicionales

    public decimal? CELULAR_SIATH { get; set; }

    public string CORREO { get; set; }

    public string UNIDAD_FUNCIONARIO_INFORMA { get; set; }

    public string ESTACION_SIVICC { get; set; }

    public string DEPENDENCIA_SIVICC { get; set; }

    public string NIVEL_SIVICC { get; set; }

    public string SIGLA_FISICA_SIVICC { get; set; }

    public long? CELULAR_CUADRANTE_SIVICC { get; set; }

    public string DESCRIPCION_DEPENDENCIA { get; set; }

    public string SIGLA_PAPA { get; set; }
    #endregion


    #region Listas Strings

    [Display(Name = "Unidad Verificacion")]
    public string ID_UNIDAD_VERIFICA_STR { get; set; }

    [Display(Name = "Unidad")]
    public string ID_UNIDAD_STR { get => $"{this.SIGLA_PAPA} - {this.DESCRIPCION_DEPENDENCIA} "; }

    [Display(Name = "Funcionario Informa")]
    public string FUNCIONARIO_INFORMA { get; set; }

    [Display(Name = "Municipio")]
    public string MUNICIPIO { get; set; }

    public string REGION { get; set; }

    [Display(Name = "Zona")]
    public string ID_ZONA_STR { get; set; }

    [Display(Name = "Tipo Servicio")]
    public string ID_TIPO_SERVICIO_STR { get; set; }

    [Display(Name = "Cuadrante")]
    public string ID_CUADRANTE_STR { get; set; }

    [Display(Name = "Clase")]
    public string ID_CLASE_STR { get; set; }

    [Display(Name = "Estado")]
    public string ID_ESTADO_STR { get; set; }

    [Display(Name = "Fuente")]
    public string ID_FUENTE_STR { get; set; }
    [Display(Name = "Fecha Delito")]
    public string ID_TIEMPO_DELITO_STR { get; set; }
    //public FormFile FOTO_EXPENDIO { get; set; }

    [Display(Name = "Fecha Inicio Actividad")]
    public string FECHA_INICIO_EXISTENCIA_STR { get => this.FECHA_INICIO_EXISTENCIA != null ? this.FECHA_INICIO_EXISTENCIA.Value.ToShortDateString() : ""; }

    [Display(Name = "Fecha creación")]
    public string FECHA_CREACION_STR { get => this.FECHA_CREACION != null ? this.FECHA_CREACION.Value.ToShortDateString() : ""; }

    [Display(Name = "Fecha modifica")]
    public string FECHA_MODIFICA_STR { get => this.FECHA_MODIFICA != null ? this.FECHA_MODIFICA.Value.ToShortDateString() : ""; }

    #endregion

    [Display(Name = "Nombre")]
    [RegularExpression("^[A-Z0-9 a-zñÑáéíóúÁÉÍÓÚ,./¿?$#]*$", ErrorMessage = "No se puede ingresar caracteres especiales en el campo 'Nombre'")]
    [StringLength(30, ErrorMessage = "Solo puede ingresar una máximo de 30 caracteres")]
    public string NOMBRE_CLASE { get; set; }

    [Display(Name = "Tiempo Actividad Delito")]
    public System.DateTime? FECHA_INICIO_EXISTENCIA { get; set; }

    [Display(Name = "Cantidad")]
    public decimal? CANTIDAD_INTEGRANTE { get; set; }

    [Display(Name = "Caracteristicas Generales")]
    [RegularExpression("^[A-Z0-9 a-zñÑáéíóúÁÉÍÓÚ,./¿?$#]*$", ErrorMessage = "No se puede ingresar caracteres especiales en el campo 'Caracteristicas Generales'")]
    [StringLength(254, ErrorMessage = "Solo puede ingresar una máximo de 255 caracteres")]
    public string CARACTERISTICAS_GENERALES { get; set; }

    [Display(Name = "Caracteristicas Generales")]
    public string CARACTERISTICAS_GENERALES_BOTON { get; set; }

    [Display(Name = "Codigo")]
    public string CODIGO { get; set; }

    [Display(Name = "Consecutivo")]
    public Nullable<decimal> CONSECUTIVO_CODIGO { get; set; }

    [Display(Name = "Sigla Unidad")]
    public string SIGLA_UNIDAD { get; set; }

    [Display(Name = "Vigente")]
    public Nullable<bool> VIGENTE { get; set; }

    [Display(Name = "Identificación creación")]
    public Nullable<long> IDENTIFICACION_CREACION { get; set; }

    [Display(Name = "Maquina creación")]
    public string MAQUINA_CREACION { get; set; }

    [Display(Name = "Fecha creación")]
    public Nullable<System.DateTime> FECHA_CREACION { get; set; }

    [Display(Name = "Identificación modifica")]
    public Nullable<long> IDENTIFICACION_MODIFICA { get; set; }

    [Display(Name = "Maquina modifica")]
    public string MAQUINA_MODIFICA { get; set; }

    [Display(Name = "Fecha modifica")]
    public Nullable<System.DateTime> FECHA_MODIFICA { get; set; }

    public string RESPON_VALIDACION_ID { get; set; }

    [Display(Name = "Descripción Trámite")]
    public string DESCRIPCION_TRAMITE { get; set; }

    public virtual List<ResponsabilidadValidacionDTO> IRISP_RESPON_VALIDACION { get; set; }

    [Display(Name = "Tarea")]
    public string TAREA_STR { get; set; }

    [Display(Name = "Resultados ")]
    public string IRISP_RESULTADO_STR { get; set; }

    public List<ResultadoDTO> IRISP_RESULTADO { get; set; }
    public bool IRISP_RESULTADO_BOOL { get; set; }

    public List<TareaDTO> IRISP_TAREA { get; set; }

    #region Campos para consultar la fecha de verificación de la existencia del IRISP

    [Display(Name = "Unidad Verificación Existencia")]
    public string UNIDAD_VERIFICACION { get; set; }

    [Display(Name = "Fecha Asignación Verificación Existencia")]
    public string FECHA_ASIGNACION_VERIFICA { get; set; }

    [Display(Name = "Fecha Respuesta Verificación Existencia")]
    public string FECHA_RESPUESTA_VERIFICA { get; set; }

    [Display(Name = "Contador Verificación Existencia")]
    public string CONTADOR_VERIFICA { get; set; }

    [Display(Name = "Unidad Proceso Investigativo")]
    public string UNIDAD_PROCESO_INVESTIGATIVO { get; set; }

    [Display(Name = "Fecha Asignación Proceso Investigativo")]
    public string FECHA_ASIGNACION_INVESTIGA { get; set; }

    [Display(Name = "Fecha Respuesta Proceso Investigativo")]
    public string FECHA_RESPUESTA_INVESTIGA { get; set; }

    [Display(Name = "Contador Proceso Investigativo")]
    public string CONTADOR_INVESTIGA { get; set; }

    public string DELITO_PRINCIPAL { get; set; }
    #endregion

    #region  LAS VARIABLES NUEVAS PARA GUARDAR FOTO DE LOS EXPENDIOS

    //public string servidor { get; set; }

    //public string tipo_doc { get; set; }

    //public string name_file { get; set; }

    //public string ruta { get; set; }
    public List<VMFoto> FOTOS { get; set; }

    [Display(Name = "Especialidad que aporta información")]
    public decimal? ESPECIALIDAD { get; set; }

    [Display(Name = "Clasificación Narcotráfico")]
    public decimal? CLASIFICACION_NARCOTRAFICO { get; set; }
    [Display(Name = "Modalidad Expendio")]
    public decimal? MODALIDAD_EXPENDIO { get; set; }
    [Display(Name = "Entorno Afectado")]
    public decimal ENTORNO_AFECTADO { get; set; }
    #endregion
    [Display(Name = "Celular")]
    public decimal? CELULAR_INTEGRANTE { get; set; }
    public decimal? DIRECCION_INTEGRANTE { get; set; }

    public decimal? CLASIFICACION_NARCOTRAFICO_MODAL { get; set; }

    public decimal? MODALIDAD_EXPENDIO_MODAL { get; set; }
}

public class VMFoto
{
    public string servidor { get; set; }

    public string tipo_doc { get; set; }

    public string name_file { get; set; }

    public string ruta { get; set; }
    public string id_irisp_criminalidad_fotos { get; set; }
}
}
