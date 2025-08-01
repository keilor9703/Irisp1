using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Comun.Areas.AplicacionDTO
{
    public class DelitoDTO
    {
        public string DELITO_ID { get; set; }
        public string CRIMINALIDAD_ID { get; set; }

        [Display(Name = "Delito")]
        public Nullable<decimal> ID_DELITO { get; set; }

        [Display(Name = "Delito")]
        public string ID_DELITO_STR { get; set; }

        [Display(Name = "Tipo")]
        public Nullable<decimal> ID_TIPO { get; set; }

        [Display(Name = "Tipo")]
        public string ID_TIPO_STR { get; set; }

        [Display(Name = "Tipo Informacion")]
        public Nullable<decimal> ID_TIPO_INFO { get; set; }

        [Display(Name = "Tipo Informacion")]
        public string ID_TIPO_INFO_STR { get; set; }

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

        //public virtual ICollection<ModalidadDTO> IRISP_MODALIDAD { get; set; }
    }


}

