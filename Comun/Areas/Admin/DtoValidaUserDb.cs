using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.Areas.Admin
{
    public class DtoValidaUserDb
    {
        public long Identificacion { get; set; }
        public int IdUsuario { get; set; }
        public int Bloqueado { get; set; }
        public int Resultado { get; set; }
        public List<DtoUserRoles> Roles { get; set; } = new();
        public string? MensajeDb { get; set; } // opcional (si luego expones SRV_Message)
    }

}
