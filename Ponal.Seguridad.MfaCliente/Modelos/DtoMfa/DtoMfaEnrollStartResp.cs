using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ponal.Seguridad.MfaCliente.Modelos.DtoMfa
{
    public class DtoMfaEnrollStartResp
    {
        public string QrBase64 { get; set; } = "";
        public string ManualKey { get; set; } = "";
        public string EnrollToken { get; set; } = "";
    }
}
