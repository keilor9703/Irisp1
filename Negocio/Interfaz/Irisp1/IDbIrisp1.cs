using Comun.Areas.Admin;
using Comun.Areas.AplicacionDTO;
using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Comun.General;
using System.Data;


namespace Negocio.Interfaz.Irisp1
{
    public interface IDbIrisp1
    {
        #region Métodos de Consulta        

        public Task<DtoResultado<List<DtoAnio>>> F_GetAniosIrisP1();
     
        public Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetInfoGrillas(Int32 V_Anio, string RolUsuario, Int64 CodigoUnidad);


        public Task<DtoResultado<List<DtoCuadrantes>>> F_GetCuadrantes(string V_unidadLabora, string V_unidadLabora2); 

        public Task<DtoResultado<long>> F_ConsultarSeqIris();
        public Task<DtoResultado<long>> F_ConsultarSeqIntegrante();
        public Task<DtoResultado<List<DtoIntegrantes>>> F_GetIntegrantes(string V_CriminalidadId);
        public Task<DtoResultado<List<DtoIntegrantes>>> F_GetIntegrantesPreliminar(string V_CriminalidadId);
        public Task<DtoResultado<List<DtoDelitosIris>>> F_GetDelitosIris(string V_CriminalidadId);
        public Task<DtoResultado<List<DtoInfoAdicional>>> F_GetInfoAdicional(string V_CriminalidadId);


        //public Task<DtoResultado<List<DtoDocumentoIris>>> F_GetDocIris(string criminalidadId);
        Task<DtoResultado<List<DtoDocumentoIris>>> F_GetDocIris(string criminalidadId);


        public Task<DtoResultado<List<DtoCriminalidadFoto>>> F_GetCriminalidadFotos(string V_CriminalidadId);
        public Task<DtoResultado<List<DtoUbicacionIris>>> F_GetUbicacionIris(string V_CriminalidadId);

        #endregion



        #region Métodos de Insersión


        public Task<DtoResultado<Int32>> P_InsIntegrantes(DtoIntegrantes Obj_Integrante, string usuario, string maquina);
        public Task<DtoResultado<Int32>> P_InsIntegrantesPreliminar(DtoIntegrantes Obj_Integrante, string usuario, string maquina);


        public Task<DtoResultado<string>> P_InsRegistroIrisP1(DtoIrispCriminalidad Obj_NuevoIrisP1, string usuario, string maquina);
        public Task<DtoResultado<string>> P_InsDelitosIris(DtoIrispCriminalidad Obj_DelitosIris, string usuario, string maquina);
        public Task<DtoResultado<string>> P_InsInfoAdicionalIris(DtoInfoAdicional Obj_InfoAdicional, string usuario, string maquina);
        public Task<DtoResultado<string>> P_UpdCriminalidad(DtoIrispCriminalidad data, string usuario, string maquina);
        public Task<DtoResultado<string>> P_UpdEstadoCriminalidad(DtoIrispCriminalidad data, string usuario, string maquina);
        public Task<DtoResultado<string>> P_UpdExistenciaCriminalidad(DtoIrispCriminalidad data, string usuario, string maquina);
        public Task<DtoResultado<string>> P_InsUbicacionIris(DtoUbicacionIris Obj_Ubicacion, string usuario, string maquina);

        



        #endregion


        #region Métodos de Insersión

        public Task<DtoResultado<string>> P_DellIris(string CriminalidadId, string usuario, string maquina);
        public Task<DtoResultado<string>> P_DelIntegranteIris(string IntegranteId, string usuario, string maquina);
        public Task<DtoResultado<string>> P_DelDelitosIris(string DelitoId, string usuario, string maquina);
        public Task<DtoResultado<string>> P_DelDelInfoAdicionalIris(string CriminalidadId, string usuario, string maquina);
        public Task<DtoResultado<string>> P_DelUbicacionIris(string UbicacionId, string usuario, string maquina);
        public Task<DtoResultado<string>> P_DelDocumentoIris(string DocumentoId, string usuario, string maquina);


        #endregion


    }
}
