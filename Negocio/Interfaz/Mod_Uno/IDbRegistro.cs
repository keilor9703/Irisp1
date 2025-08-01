using Comun.Areas.Mod_Uno;
using Comun.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaz.Modulo1
{
    public interface IDbRegistro
    {
        public Task<DtoResultado<Int32>> P_InsUdpCriminalidadIris(CriminalidadDTO Obj, Int32 V_Usuario, string V_Maquina);
    }
}
