using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ponal.Seguridad.MfaCliente.Modelos.DtoMfa
{
    public class DtoMfaResetRequestReq
    {
        public long Identificacion { get; set; }
        public string Usuario { get; set; } = "";
        public string IpMaquina { get; set; } = "";
        public long UserAudit { get; set; }
        public string Sistema { get; set; } = "";
    }


    public class DtoMfaResetConfirmReq : DtoMfaResetRequestReq
    {
        public string Code { get; set; } = "";
    }

    public class DtoMfaResetExecuteReq : DtoMfaResetRequestReq { }

    public class DtoMfaResetConfirmResp
    {
        public DateTime? AvailableAt { get; set; }
    }
}
