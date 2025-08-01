using Comun.Areas.Admin;
using Comun.General;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Gestion.Utilidades;
using Negocio.Interfaz.Admin;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Negocio.Gestion.Admin
{
    public class DbFuncionarios : IDbFuncionarios
    {
        #region Propiedades
        private readonly IConfiguration _iConfiguration;
        private readonly string _strConexionTelepol;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public DbFuncionarios(IConfiguration iConfiguration,
                                ILogger<DbAdministracion> logger
                                )
        {
            _iConfiguration = iConfiguration;
            _strConexionTelepol = _iConfiguration.GetConnectionString("strConexionTelepol");
            _logger = logger;
        }
        #endregion

        #region Métodos de Consulta
        public async Task<DtoResultado<List<DtoFuncionarios>>> F_GetFuncionarios(Int64 V_Identificacion)
        {
            DataTable resultado = new();
            List<DtoFuncionarios> retorno = new();
            DtoResultado<List<DtoFuncionarios>> resp = new();

            using var Conexion = new OracleConnection(_strConexionTelepol);
            using var objCommand = new OracleCommand();
            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "USR_MATERIALIZADAS.PK_FUNCIONARIOS.F_GetFuncionarioId";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add("P_Identificacion", OracleDbType.Int64, ParameterDirection.Input).Value = V_Identificacion;

                if (Conexion.State == ConnectionState.Open)
                    resultado.Load(await objCommand.ExecuteReaderAsync());

                retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoFuncionarios>(resultado);

                if (retorno.Count > 0)
                {
                    resp.IdRespuesta = 1;
                    resp.Mensaje = "Consulta Exitosa";
                    resp.Operacion = "F_GetFuncionarios";
                    resp.Data = retorno;
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "No se encontraron datos";
                    resp.Operacion = "0";
                }
            }
            catch (Exception e)
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Connection.Close();
                _logger.LogError("Creacion de log");
                _logger.LogWarning("Error Ejecutando PK_FUNCIONARIOS.F_GetFuncionarioId " + e);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"{e.Message} - {e.InnerException}";
                resp.Operacion = "0";

            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
                objCommand.Connection.Close();
                resultado.Dispose();
            }
            return resp;
        }
        public async Task<DtoResultado<List<DtoFuncionarios>>> F_GetEmpleadoIntel(string V_Busqueda)
        {
            DataTable resultado = new();
            List<DtoFuncionarios> retorno = new();
            DtoResultado<List<DtoFuncionarios>> resp = new();

            using var Conexion = new OracleConnection(_strConexionTelepol);
            using var objCommand = new OracleCommand();
            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "USR_MATERIALIZADAS.PK_FUNCIONARIOS.F_GetEmpleadoIntel";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add("V_Busqueda", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_Busqueda;

                if (Conexion.State == ConnectionState.Open)
                    resultado.Load(await objCommand.ExecuteReaderAsync());

                retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoFuncionarios>(resultado);

                if (retorno.Count > 0)
                {
                    resp.IdRespuesta = 1;
                    resp.Mensaje = "Consulta Exitosa";
                    resp.Operacion = "F_GetFuncionarios";
                    resp.Data = retorno;
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "No se encontraron datos";
                    resp.Operacion = "0";
                }
            }
            catch (Exception e)
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Connection.Close();
                _logger.LogError("Creacion de log");
                _logger.LogWarning("Error Ejecutando PK_FUNCIONARIOS.F_GetEmpleadoIntel " + e);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"{e.Message} - {e.InnerException}";
                resp.Operacion = "0";

            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
                objCommand.Connection.Close();
                resultado.Dispose();
            }
            return resp;
        }
        #endregion
    }
}
