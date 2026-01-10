using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DtoMfa
{
    public class DtoMfaVerifyReq
    {
        public long Identificacion { get; set; }
        public string Usuario { get; set; } = "";
        public string Code { get; set; } = "";
        public bool RememberDevice { get; set; }
        public string? DeviceId { get; set; }
        public string IpMaquina { get; set; } = "";
        public long UserAudit { get; set; }
    }

}
