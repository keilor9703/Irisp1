using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.Areas.AplicacionDTO
{
    public class ResultadoDTO
    {
        public string RESULTADO_ID { get; set; }
        public string CRIMINALIDAD_ID { get; set; }

        [Required]
        public Nullable<decimal> ID_TIPO { get; set; }
        public string ID_TIPO_STR { get; set; }

        [Required]
        public string NRO_SPOA_SIEDCO { get; set; }
        public string OBSERVACION { get; set; }
        public Nullable<bool> VIGENTE { get; set; }

        [Required]
        public Nullable<System.DateTime> FECHA { get; set; }
        public Nullable<System.DateTime> FECHA_CREACION { get; set; }
        public Nullable<long> IDENTIFICACION_CREA { get; set; }
        public string MAQUINA_CREACION { get; set; }
        public Nullable<System.DateTime> FECHA_MODIFICA { get; set; }
        public Nullable<long> IDENTIFICACION_MODIFICA { get; set; }
        public string MAQUINA_MODIFICA { get; set; }
    }
}
