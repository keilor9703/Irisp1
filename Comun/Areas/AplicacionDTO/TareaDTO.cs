using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.Areas.AplicacionDTO
{
    public class TareaDTO
    {
        public string TAREA_ID { get; set; }
        public string RESPON_VALIDACION_ID { get; set; }

        [Display(Name = "Tareas")]
        public Nullable<decimal> ID_LISTA_TAREAS { get; set; }

        [Display(Name = "Tareas")]
        public string ID_LISTA_TAREAS_STR { get; set; }

        [Display(Name = "Observación")]
        [RegularExpression("^[A-Z0-9 a-zñÑáéíóúÁÉÍÓÚ,./¿?$#]*$", ErrorMessage = "No se puede ingresar caracteres especiales, solo está permitido los caracteres ,./¿?$#")]
        [StringLength(255, ErrorMessage = "Solo puede ingresar una máximo de 255 caracteres")]
        public string OBSERVACION { get; set; }

        [Display(Name = "Estado tarea")]
        public Nullable<decimal> ID_ESTADO_TAREA { get; set; }

        [Display(Name = "Estado tarea")]
        public string ID_ESTADO_TAREA_STR { get; set; }

        [Display(Name = "Fecha verificación")]
        public Nullable<System.DateTime> FECHA_VERIFICA { get; set; }

        [Display(Name = "Justificación")]
        [RegularExpression("^[A-Z0-9 a-zñÑáéíóúÁÉÍÓÚ,./¿?$#]*$", ErrorMessage = "No se puede ingresar caracteres especiales, solo está permitido los caracteres ,./¿?$#")]
        [StringLength(2000, ErrorMessage = "Solo puede ingresar una máximo de 2000 caracteres")]
        public string JUSTIFICACION { get; set; }

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

        public virtual ICollection<TareaDocumentoDTO> IRISP_TAREA_DOCUMENTO { get; set; }

        // Campo de la Tabla IRISP_CRIMINALIDAD
        public decimal? ID_ESTADO_EXISTENCIA { get; set; }


        //Campo de la Tabla IRISP_RESPON_VALIDACION
        public decimal? DEPENDENCIA { get; set; }
        public string DEPENDENCIA_STR { get; set; }

        [Display(Name = "Contador")]
        public string CONTADOR { get; set; }
    }
}
