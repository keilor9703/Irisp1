using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Irisp1
{
     public class DtoDocumentoIris
    {



        public string DocumentoId { get; set; }
        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string CriminalidadId { get; set; }




    }
}
