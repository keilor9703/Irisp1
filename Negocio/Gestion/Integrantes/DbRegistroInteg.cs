using Comun.Areas.Expendios;
using Comun.Areas.Integrantes;
using Comun.General; // ← Asegúrate de tener aquí tu clase DtoResultado
using Dapper;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Gestion.Utilidades;
using Negocio.Interfaz.Integrantes;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Negocio.Gestion.Integrantes
{
    public class DbRegistroInteg : IDbRegistroInteg
    {
        private readonly IConfiguration _iConfiguration;
        private readonly string _strConexionIris_Disec;
        private readonly ILogger _logger;

        public DbRegistroInteg(IConfiguration iConfiguration, ILogger<IDbRegistroInteg> logger)
        {
            _iConfiguration = iConfiguration;
            _strConexionIris_Disec = _iConfiguration.GetConnectionString("strConexionIris_Disec");
            _logger = logger;
        }

        
        public async Task<DtoResultado<List<DtoReincidentes>>> F_GetReincidentes()
        {
            var resp = new DtoResultado<List<DtoReincidentes>>();

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();
                parametros.Add("RESULT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                string sql = @"BEGIN 
                                  :RESULT := PK_INTEGRANTES_IRIS.F_GetReincidentes; 
                               END;";

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoReincidentes>(
                    sql,
                    parametros,
                    commandType: CommandType.Text
                )).ToList();

                // 🔹 Construcción del DTO de respuesta
                resp.Data = lista;
                resp.IdRespuesta = lista.Count > 0 ? 1 : 0;
                resp.Mensaje = lista.Count > 0 ? "Consulta exitosa" : "No se encontraron registros";
                resp.Operacion = "F_GetReincidentes";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando PK_INTEGRANTES_IRIS.F_GetReincidentes");

                resp.IdRespuesta = 0;
                resp.Mensaje = "Error consultando reincidentes: " + ex.Message;
                resp.Data = new List<DtoReincidentes>();
                resp.Operacion = "F_GetReincidentes";
            }

            return resp;
        }





        public async Task<DtoResultado<List<DtoReincidentes>>> F_GetReincidentesPorId(Int64 V_Identificacion)
        {
            DataTable resultado = new();
            List<DtoReincidentes> retorno = new();
            DtoResultado<List<DtoReincidentes>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_INTEGRANTES_IRIS.F_GetReincidentesPorId";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_Identificacion", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_Identificacion;
                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());
                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoReincidentes>(resultado);

                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetIntegranteAll";
                        resp.Data = retorno;
                    }
                    else
                    {
                        resp.IdRespuesta = 0;
                        resp.Mensaje = "No se encontraron datos";
                        resp.Operacion = "0";
                    }
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "Error conexión base de datos";
                    resp.Operacion = "0";
                }

            }
            catch (Exception e)
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Connection.Close();
                _logger.LogError("Creacion de log");
                _logger.LogWarning("Error Ejecutando PK_EXPENDIOS_IRIS.F_GetIntegranteAll " + e);

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




        public async Task<DtoResultado<string>> P_InsOrUpdReincidente(DtoReincidentes Obj_Reincidente, string usuario, string maquina)
        {
            DtoResultado<string> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec); // Usa tu cadena correcta
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_INTEGRANTES_IRIS.P_InsOrUpdReincidente";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();


                // Parámetros de entrada
                objCommand.Parameters.Add("P_IDENTIFICACION", OracleDbType.Int32).Value = Obj_Reincidente.Identificacion;
                objCommand.Parameters.Add("P_ALIAS", OracleDbType.Varchar2).Value = Obj_Reincidente.Alias;
                objCommand.Parameters.Add("P_NOMBRE", OracleDbType.Varchar2).Value = Obj_Reincidente.Nombre;
                objCommand.Parameters.Add("P_APELLIDO", OracleDbType.Varchar2).Value = Obj_Reincidente.Apellido;
                objCommand.Parameters.Add("P_OBSERVACION", OracleDbType.Varchar2).Value = Obj_Reincidente.Observacion;
                objCommand.Parameters.Add("P_ID_TIPO", OracleDbType.Varchar2).Value = Obj_Reincidente.IdTipo;

                objCommand.Parameters.Add("P_USUARIO", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA", OracleDbType.Varchar2).Value = maquina;

                // Parámetros de salida
                objCommand.Parameters.Add("P_RESULTADO", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                if (Conexion.State == ConnectionState.Open)
                    await objCommand.ExecuteNonQueryAsync();

                int resultado = Convert.ToInt32(objCommand.Parameters["P_RESULTADO"].Value?.ToString() ?? "0");
                string mensaje = objCommand.Parameters["SRV_Message"].Value?.ToString() ?? "";

                if (resultado > 0)
                {
                    resp.IdRespuesta = 1;
                    resp.Mensaje = mensaje;
                    resp.Data = "OK";
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = mensaje;
                    resp.Data = "";
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error ejecutando PK_INTEGRANTES_IRIS.P_InsOrUpdReincidente");
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {e.Message}";
                resp.Data = "";
            }
            finally
            {
                if (Conexion.State == ConnectionState.Open)
                    Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
            }

            return resp;
        }


    }
}
