using Comun.Areas.Expendios;
using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Comun.Areas.Reportes;
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
        public Task<DtoResultado<string>> F_ConsultarSeqIris();
        Task<DtoResultado<List<DtoAnioE>>> F_GetAniosIrisP1();
        
        public Task<DtoResultado<List<DtoExpendios>>> F_GetInfoGrillas(Int32 V_Anio, string RolUsuario, Int64 CodigoUnidad);
        Task<DtoResultado<List<DtoDominios>>> F_GetEstaciones(string V_Sigla);
        Task<DtoResultado<List<DtoDominios>>> F_GetEspecialidad(string V_Sigla);

        Task<DtoResultado<List<DtoIntegrantes>>> F_GetIntegrantes(string V_CriminalidadId);
        Task<DtoResultado<List<DtoDelitosIris>>> F_GetDelitosIris(string V_CriminalidadId);
        Task<DtoResultado<List<DtoInfoAdicional>>> F_GetBitacora(string V_CriminalidadId);
        Task<DtoResultado<List<DtoResultadosExpendio>>> F_GetResultados(string V_CriminalidadId);
        Task<DtoResultado<List<DtoIntegrantes>>> F_GetIntegranteAll(Int64 V_Identificacion);

        public Task<DtoResultado<List<DtoIntegrantes>>> F_GetIntegrantesPreliminar(string V_CriminalidadId);

        public Task<DtoResultado<string>> P_InsRegistroExpendio(DtoInsExpendios Obj_NuevoExpendio, string usuario, string maquina);

        Task<DtoResultado<Int32>> P_InsIntegrante(DtoIntegrantes Obj_Integrante, string usuario, string maquina);
        Task<DtoResultado<Int32>> P_InsIntegrantePreliminar(DtoIntegrantes Obj_Integrante, string usuario, string maquina);
        Task<DtoResultado<Int32>> P_InsDelito(DtoDelitosIris Obj_Delito, string usuario, string maquina);
        Task<DtoResultado<Int32>> P_InsBitacora(DtoInfoAdicional Obj_Bitacora, string usuario, string maquina);
        Task<DtoResultado<Int32>> P_InsResultados(DtoResultadosExpendio Obj_Resultados, string usuario, string maquina);
        Task<DtoResultado<Int32>> P_UpdExpendio(DtoExpendios DtoExpendios, string usuario, string maquina);
        Task<DtoResultado<Int32>> P_UpdIntegrante(DtoIntegrantes Obj_Integrante, string usuario, string maquina);





    }
}
