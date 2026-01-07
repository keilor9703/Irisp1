using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.Areas.Admin
{
    public class MfaDbDto
    {
        public int MfaHabilitado { get; set; }
        public string? TotpSecretEnc { get; set; }
        public int RequireReenroll { get; set; }
        public DateTime? BloqueoHasta { get; set; }
        public int IntentosFallidos { get; set; }
    }
}
