using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.Areas.Irisp1
{
    public class DtoCriminalidadFoto
    {
        public string IdFoto { get; set; }
        public string NombreArchivo { get; set; }
        public string Ruta { get; set; }
        public string Tipo { get; set; }
        public DateTime FechaCreacion { get; set; }

        public int TipoRuta { get; set; }
    }

}
