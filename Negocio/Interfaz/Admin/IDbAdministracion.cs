using Comun.Areas.Admin;
using Comun.General;
using System.Data;

namespace Negocio.Interfaz.Admin
{
    public interface IDbAdministracion
    {
        #region Métodos de Consulta        
       
        public Task<DtoResultado<List<DtoMenu>>> F_GetMenu(int V_Idrol, long P_Identificacion);
      
        public Task<DtoResultado<DtoUsuario>> P_GetValidaUser(string V_Usuario, string V_Maquina);
        public Task<DtoResultado<List<DtoRoles>>> F_GetRoles();
        public Task<DtoResultado<List<DtoUsuario>>> F_GetListUsuarios();
        public Task<DtoResultado<List<DtoUserRoles>>> F_GetUserRoles(long V_Identificacion);


        public string ConvertirBase64Bytes(string texto);
        public string Decript(string message, string key);
        #endregion

        #region Métodos de Inserción y Actualización
        public Task<DtoResultado<int>> P_InsAuditoria(long V_Identificacion, string V_Evento, string V_Descripcion, long V_Identificador, string V_Maquina);
        public Task<DtoResultado<int>> P_InsRolesUser(DtoInsUserRoles obj, long V_Usuario, string V_Maquina);
        public Task<DtoResultado<int>> P_InsUdpUsuarios(long V_Identificacion, int V_Bloqueado, long V_Usuario, string V_Maquina);



        #endregion

        #region Métodos de Eliminación
        public Task<DtoResultado<int>> P_DelRoles(DtoInsUserRoles obj, long V_Usuario, string V_Maquina);
        #endregion

    }
}
