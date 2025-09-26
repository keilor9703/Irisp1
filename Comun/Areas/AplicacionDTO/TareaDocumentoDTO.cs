using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.Areas.AplicacionDTO
{
    public class TareaDocumentoDTO
    {
        public string TAREA_ID { get; set; }
        public string DOCUMENTO_ID { get; set; }
        public string URL { get; set; }
        public string NOMBRE { get; set; }
        public Nullable<bool> VIGENTE { get; set; }
    }
}

