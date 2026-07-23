using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ponal.Seguridad.MfaCliente.Modelos.DtoMfa
{
    public class DtoMfaEnrollConfirmReq
    {
        public long Identificacion { get; set; }
        public string Usuario { get; set; } = "";
        public string EnrollToken { get; set; } = "";
        public string Code { get; set; } = "";
        public string IpMaquina { get; set; } = "";
       
    }
}
