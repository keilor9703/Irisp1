using Comun.Areas.Admin;
using Comun.General;

namespace Negocio.Interfaz.Admin
{
    public interface IDbFuncionarios
    {
        #region Métodos de Consulta
        public Task<DtoResultado<List<DtoFuncionarios>>> F_GetFuncionarios(long V_Identificacion);
        public Task<DtoResultado<List<DtoFuncionarios>>> F_GetEmpleadoIntel(string V_Busqueda);
        #endregion
    }
}
