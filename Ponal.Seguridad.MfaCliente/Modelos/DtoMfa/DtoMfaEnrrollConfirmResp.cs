using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ponal.Seguridad.MfaCliente.Modelos.DtoMfa
{
    public class DtoMfaEnrrollConfirmResp
    {

        public bool Ok { get; set; }
        public DateTime? BloqueoHasta { get; set; }

        
    }
}
