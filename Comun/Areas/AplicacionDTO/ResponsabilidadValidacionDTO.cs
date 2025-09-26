using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Comun.Areas.AplicacionDTO
{
    public class ResponsabilidadValidacionDTO
    {
        public string RESPON_VALIDACION_ID { get; set; }
        public string CRIMINALIDAD_ID { get; set; }

        [Display(Name = "Dependencia")]
        public Nullable<decimal> ID_UNIDAD_RESPON { get; set; }

        [Display(Name = "Unidad")]
        public string UNIDAD_STR { get; set; }

        [Display(Name = "Dependencia")]
        public string DEPENDENCIA_STR { get; set; }

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

        public virtual ICollection<TareaDTO> IRISP_TAREA { get; set; }

    }


}

