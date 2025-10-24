using Comun.Areas.Expendios;
using Comun.Areas.Irisp1;
using Comun.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaz.Expendios
{
    public interface IDbRegistroExpendio
    {

        Task<DtoResultado<List<DtoExpendios>>> F_GetAniosIrisP1();
        Task<DtoResultado<List<DtoExpendios>>> F_GetInfoGrillas(Int32 V_Anio);


    }
}
