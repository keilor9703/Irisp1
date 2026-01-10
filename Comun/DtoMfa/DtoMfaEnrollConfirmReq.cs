using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DtoMfa
{
    public class DtoMfaEnrollConfirmReq
    {
        public long Identificacion { get; set; }
        public string Usuario { get; set; } = "";
        public string EnrollToken { get; set; } = "";
        public string Code { get; set; } = "";
        public string IpMaquina { get; set; } = "";
        public long UserAudit { get; set; }
    }
}
