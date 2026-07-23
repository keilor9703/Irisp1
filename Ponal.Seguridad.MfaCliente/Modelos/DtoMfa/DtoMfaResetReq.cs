using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ponal.Seguridad.MfaCliente.Modelos.DtoMfa
{
    public class DtoMfaResetReq
    {
        public long Identificacion { get; set; }
        public string Usuario { get; set; } = "";   
        public string IpMaquina { get; set; } = "";
      
        public string Sistema { get; set; }

        public int? Estado2FA { get; set; }
    }

}
