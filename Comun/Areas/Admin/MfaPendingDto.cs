using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.Areas.Admin
{
    public class MfaPendingDto
    {
        public int IdUsuario { get; set; }
        public long Identificacion { get; set; }
        public string Usuario { get; set; }
        public string Funcionario { get; set; }
        public string Ip { get; set; }
    }

}
