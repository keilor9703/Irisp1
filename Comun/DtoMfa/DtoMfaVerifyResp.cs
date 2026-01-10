using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DtoMfa
{
    public class DtoMfaVerifyResp
    {
        public bool Ok { get; set; }
        public DateTime? BloqueoHasta { get; set; }
    }
}
