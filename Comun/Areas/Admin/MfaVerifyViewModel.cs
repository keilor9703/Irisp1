using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.Areas.Admin
{
    public class MfaVerifyViewModel
    {
        public string Code { get; set; } = "";
        public bool RememberDevice { get; set; }
    }
}
