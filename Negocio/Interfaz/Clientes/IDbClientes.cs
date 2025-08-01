using Comun.Areas.Clientes;
using Comun.General;

namespace Negocio.Interfaz.Clientes
{
    public interface IDbClientes
    {
        #region Métodos de Consulta

        #endregion

        #region Métodos de Inserción
        public Task<DtoResultado<Int32>> P_InsUdpKardex(DtoKardex Obj, Int32 V_Usuario, string V_Maquina);



        #endregion


        #region Métodos de Actualización

        #endregion
    }
}
