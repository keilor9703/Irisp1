using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ponal.Seguridad.MfaCliente.Modelos.DtoMfa
{
    public class DtoMfaEnrollStartReq
    {
        public long Identificacion { get; set; }
        public string Usuario { get; set; } = "";
        public string SystemId { get; set; } = "";
    }
}
