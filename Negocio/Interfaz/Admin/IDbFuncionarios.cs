using Comun.Areas.Admin;
using Comun.General;

namespace Negocio.Interfaz.Admin
{
    public interface IDbFuncionarios
    {
        #region Métodos de Consulta
        public  Task<DtoResultado<DtoUsuario>> F_GetFuncionarios(Int64 V_Identificacion);
        public Task<DtoResultado<List<DtoFuncionarios>>> F_GetEmpleadoIntel(string V_Busqueda);
        #endregion
    }
}
