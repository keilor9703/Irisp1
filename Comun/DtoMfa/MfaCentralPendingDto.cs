using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DtoMfa
{
    public class MfaCentralPendingDto
    {
        public long Identificacion { get; set; }
        public string Usuario { get; set; } = "";
        public long UserAudit { get; set; }
        public string Ip { get; set; } = "0.0.0.0";

        // solo para el flujo de enroll
        public string? EnrollToken { get; set; }
    }

}
