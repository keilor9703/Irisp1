using Comun.Areas.Admin;
using Comun.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaz.Admin
{
   public  interface IDbConsultasPIP
    {

        public Task<DtoRespuesta<string>> ObtenerTokenAsync();
        public Task<DtoRespuesta<bool>> ObtenerOudAsync(DtoCredenciales _credenciales);
        public Task<DtoRespuesta<DtoFuncionariosPIP>> ObtenerDatosFuncionarioIdAsync(long identificacion);

      //  public Task<DtoRespuesta<DtoFuncionariosPIP>> ObtenerCarruselImgAsync();
    }
}
