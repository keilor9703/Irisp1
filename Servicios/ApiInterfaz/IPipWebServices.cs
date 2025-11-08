using Comun.Areas.Admin;
using Comun.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.ApiInterfaz
{
   public interface IPipWebServices
    {

        public Task<DtoRespuesta<string>> ObtenerTokenSeviciosAsync(DtoUsuarioPip _usuarioMs);
        public Task<DtoRespuesta<bool>> ObtenerOudSeviciosAsync(DtoCredenciales _credenciales, string token);
        public Task<DtoRespuesta<DtoFuncionariosPIP>> ObtenerFuncionariosIdSeviciosAsync(Int64 Identificacion, string token);

    }
}
