using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.Areas.Admin
{
    public class MfaEnrollViewModel
    {
        public string QrCodeBase64Png { get; set; } = "";
        public string ManualKey { get; set; } = "";
        public string Code { get; set; } = "";
    }
}
