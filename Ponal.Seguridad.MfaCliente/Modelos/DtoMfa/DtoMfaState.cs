using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ponal.Seguridad.MfaCliente.Modelos.DtoMfa
{
    public class DtoMfaState
    {
        public int MfaHabilitado { get; set; }
        public int RequireReenroll { get; set; }
        public DateTime? BloqueoHasta { get; set; }
        public int IntentosFallidos { get; set; }
        public string? UsuarioInstitucional { get; set; }
    }
}
