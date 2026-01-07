using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.Areas.Admin
{
    public record MfaStateDto(
     long IdUsuario,
     long Identificacion,
     string Usuario,
     string Funcionario,
     string Ip
 );
}
