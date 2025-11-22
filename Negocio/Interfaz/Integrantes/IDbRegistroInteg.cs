using Comun.Areas.Expendios;
using Comun.Areas.Integrantes;
using Comun.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaz.Integrantes
{
    public interface IDbRegistroInteg
    {


        Task<DtoResultado<List<DtoReincidentes>>> F_GetReincidentes();

        Task<DtoResultado<List<DtoReincidentes>>> F_GetReincidentesPorId(Int64 V_Identificacion);

      Task<DtoResultado<string>> P_InsOrUpdReincidente(DtoReincidentes Obj_Reincidente, string usuario, string maquina);
      Task<DtoResultado<string>> P_UpdReincidente(DtoReincidentes Obj_Reincidente, string usuario, string maquina);
      Task<DtoResultado<string>> P_DellReincidente(DtoReincidentes Obj_Reincidente, string usuario, string maquina);

    }
}
