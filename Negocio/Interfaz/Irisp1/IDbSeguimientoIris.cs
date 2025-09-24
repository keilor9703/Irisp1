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

       /*ask<DtoResultado<List<DtoIrisp1>>> F_GetAniosIrisP1();*/
        Task<DtoResultado<List<SeguimientoIrisDto>>> F_GetAniosIrisP1();

        Task<List<SeguimientoDto>> ConsultarSeguimientoIris(string _anio);

        Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetInfoGrillas(Int32 V_Anio);

        
        /* cambiar datos */
        #region Métodos de Insersión  

        //public Task<DtoResultado<Int32>> P_InsIntegrantes(DtoIntegrantes Obj_Integrante, string usuario, string maquina);

       
        #endregion
    }



}
