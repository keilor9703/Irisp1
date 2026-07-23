using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ponal.Seguridad.MfaCliente.Modelos.DtoMfa
{
    public class DtoTokenSistemaReq
    {
        public string Usuario { get; set; } = "";
        public long? Identificacion { get; set; }
        public string Sistema { get; set; } = "";
        public DateTimeOffset FechaUtc { get; set; }  // DateTime.UtcNow
        public string Nonce { get; set; } = ""; // Guid.NewGuid().ToString("N")
        public string Signature { get; set; } = ""; // HMAC
    }

}
