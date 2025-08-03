using Comun.Areas.Admin;
using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Comun.General;
using System.Data;

namespace Negocio.Interfaz.Irisp1
{
    public interface IDbIrisp1
    {
        #region Métodos de Consulta        

        public Task<DtoResultado<List<DtoIrisp1>>> F_GetAniosIrisP1();
        public Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetInfoGrillas(Int32 V_Anio);
        public Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetEstadosIrisP1();
        public Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetCuadrantes(string V_unidadLabora); // NUEVO MÉTODO

        public Task<DtoResultado<long>> F_ConsultarSeqIris();

        #endregion



        #region Métodos de Insersión

        public Task<DtoResultado<string>> P_InsIntegrantes(DtoIntegrantes Obj_Integrante);


        #endregion


    }
}
