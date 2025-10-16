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
        Task<DtoResultado<List<DtoDominios>>> F_GetUnidadesPorSigla(string V_Sigla);

        Task<DtoResultado<List<DtoDominios>>> F_GetUnidadesSeguimiento();

        #region Métodos de Insersión  

        Task<DtoResultado<Int32>> P_InsResponsable(DtoIrispCriminalidad Obj_Responsable, string usuario, string maquina);
        Task<DtoResultado<Int32>> P_UpdUnidadResponsable(DtoIrispCriminalidad obj_responsableUpd, string usuario, string maquina);
        Task<DtoResultado<Int32>> P_DelUnidadResponsable(DtoIrispCriminalidad obj_DelResponsable, string usuario, string maquina);
        Task<DtoResultado<Int32>> P_EvalTarea(DtoIrispCriminalidad obj_EvalTarea, string usuario, string maquina);
        


        #endregion
    }



}
