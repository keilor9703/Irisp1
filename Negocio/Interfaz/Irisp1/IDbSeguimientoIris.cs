using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Comun.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaz.Irisp1
{
    public interface IDbSeguimientoIris
    {

      
        Task<DtoResultado<List<SeguimientoIrisDto>>> F_GetAniosIrisP1();

      
        Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetInfoGrillas(Int32 V_Anio);

         Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetResponsables(string V_CriminalidadId);


        #region Métodos de Insersión  



        #endregion
    }



}
