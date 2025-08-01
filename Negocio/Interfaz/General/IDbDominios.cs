using Comun.General;

namespace Negocio.Interfaz.General
{
    public interface IDbDominios
    {
        #region Métodos de Consutla
        public Task<DtoResultado<List<DtoDominios>>> F_GetDominios(Int32 V_Id);
        public Task<DtoResultado<List<DtoDominios>>> F_GetDepartamentos(Int32 V_Id);
        //public Task<DtoResultado<List<DtoDominios>>> F_GetMunicipios(Int32 V_Id)
        public Task<DtoResultado<List<DtoDominios>>> F_GetMunicipios(Int32 V_Id);
        public Task<DtoResultado<List<DtoDominios>>> F_GetDependencias(string V_SiglaPapa);

        public Task<DtoResultado<List<DtoDominios>>> F_GetUnidadesPoliciales(Int32 V_Id);
        //public Task<DtoResultado<List<DtoDominios>>> F_GetUnidadesPoliciales(string V_Id);

        #endregion

        #region Métodos de Inserción

        #endregion

        #region Métodos de Actualízación

        #endregion
    }
}
