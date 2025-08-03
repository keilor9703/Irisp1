using Comun.Areas.Admin;
using Comun.General;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Interfaz.Irisp1;
using Comun.Areas.Irisp1;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Negocio.Gestion.Utilidades;
using Comun.Areas.Integrantes;

namespace Negocio.Gestion.Irisp1
{
    public class DbIrisp1 : IDbIrisp1
    {
        #region Propiedades
        private readonly IConfiguration _iConfiguration;
        private readonly string _strConexionIris_Test;
        private readonly string _strConexionTelepol;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public DbIrisp1(IConfiguration iConfiguration,
                                ILogger<IDbIrisp1> logger
                                )
        {
            _iConfiguration = iConfiguration;
            _strConexionIris_Test = _iConfiguration.GetConnectionString("strConexionIris_Test");
            _strConexionTelepol = _iConfiguration.GetConnectionString("strConexionTelepol");
            _logger = logger;
        }
        #endregion

        public async Task<DtoResultado<List<DtoIrisp1>>> F_GetAniosIrisP1()
        {
            List<DtoIrisp1> Retorno = new();
            DtoResultado<List<DtoIrisp1>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Test);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_CONSULTA_IRISP.F_GetAniosIrisP1";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;

                if (Conexion.State == ConnectionState.Open)
                {
                    var reader = await objCommand.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        var domi = new DtoIrisp1()
                        {
                            AnoIrisp1 = reader.GetInt32(0),
                        };
                        Retorno.Add(domi);
                    }

                    if (Retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_AniosIris";
                        resp.Data = Retorno;
                    }
                    else
                    {
                        resp.IdRespuesta = 0;
                        resp.Mensaje = "No se encuentran registros en base de datos";
                        resp.Operacion = "0";
                    }

                    reader.Close();
                    Conexion.Close();
                    Conexion.Dispose();
                    objCommand.Connection.Close();
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
                _logger.LogWarning("Error Ejecutando PK_CONSULTA_IRISP.F_AniosIris " + e);

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
            }
            return resp;
        }

        public async Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetInfoGrillas(Int32 V_Anio)
        {
            DataTable resultado = new();
            List<DtoIrispCriminalidad> retorno = new();
            DtoResultado<List<DtoIrispCriminalidad>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Test);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_CONSULTA_IRISP.F_GetInfoGrillas";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_Anio", OracleDbType.Int32, ParameterDirection.Input).Value = V_Anio;
                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());
                    objCommand.Parameters.Add("P_Anio", OracleDbType.Int32, ParameterDirection.Input).Value = V_Anio;
                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoIrispCriminalidad>(resultado);

                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetInfoGrillas";
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
                _logger.LogWarning("Error Ejecutando PK_CONSULTA_IRISP.F_GetInfoGrillas " + e);

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

        public async Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetEstadosIrisP1()
        {
            DataTable resultado = new();
            List<DtoIrispCriminalidad> retorno = new();
            DtoResultado<List<DtoIrispCriminalidad>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Test);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_CONSULTA_IRISP.F_GetEstadosIrisP1";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());         
                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoIrispCriminalidad>(resultado);

                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetMenu";
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
                _logger.LogWarning("Error Ejecutando PK_CONSULTA_IRISP.F_GetEstadosIrisP1 " + e);

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

        public async Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetCuadrantes(string V_unidadLabora)
        {
            DataTable resultado = new();
            List<DtoIrispCriminalidad> retorno = new();
            DtoResultado<List<DtoIrispCriminalidad>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Test);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_REGISTRO_IRIS.F_GetCuadrantes";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
            
                objCommand.Parameters.Add("P_Dependencia", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_unidadLabora;
                objCommand.Parameters.Add("p_resultados", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());
                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoIrispCriminalidad>(resultado);

                    resp.IdRespuesta = 1;
                    resp.Mensaje = "Consulta Exitosa";
                    resp.Operacion = "F_GetCuadrantes";
                    resp.Data = retorno;
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "Error conexión base de datos";
                    resp.Operacion = "F_GetCuadrantes";
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error ejecutando F_GetCuadrantes: {e.Message}", e);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"{e.Message} - {e.InnerException?.Message}";
                resp.Operacion = "F_GetCuadrantes";
            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
                resultado.Dispose();
            }

            return resp;
        }

        public async Task<DtoResultado<long>> F_ConsultarSeqIris()
        {
            DtoResultado<long> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Test);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.Text; // IMPORTANTE: Es una función, no un procedimiento
                objCommand.CommandText = "SELECT PK_REGISTRO_IRIS.f_consultar_seq_Iris FROM dual";
                Conexion.Open();

                var result = await objCommand.ExecuteScalarAsync();
                long consecutivo = Convert.ToInt64(result?.ToString() ?? "0");

                if (consecutivo > 0)
                {
                    resp.IdRespuesta = 1;
                    resp.Mensaje = "Consulta exitosa";
                    resp.Data = consecutivo;
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "No se pudo obtener el consecutivo";
                    resp.Data = 0;
                }
            }
            catch (Exception e)
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Connection.Close();

                resp.IdRespuesta = 0;
                resp.Mensaje = $"{e.Message} - {e.InnerException}";
                resp.Data = 0;
            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
                objCommand.Connection.Close();
            }
            return resp;
        }

        
        public async Task<DtoResultado<string>> P_InsIntegrantes(DtoIntegrantes Obj_Integrante)
        {
            DtoResultado<string> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Test);
            using var objCommand = new OracleCommand();



            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_RECEPCION_NET8.P_GuardarLlamada";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                // Parámetros de entrada
              

                // Parámetros de salida
                objCommand.Parameters.Add("P_RESULTADO", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("P_MENSAJE", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                if (Conexion.State == ConnectionState.Open)
                    await objCommand.ExecuteNonQueryAsync();

                int resultado = Convert.ToInt32(objCommand.Parameters["P_RESULTADO"].Value?.ToString() ?? "0");
                string mensaje = objCommand.Parameters["P_MENSAJE"].Value?.ToString() ?? "";

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
                _logger.LogError(e, "Error ejecutando PK_RECEPCION_NET8.P_GuardarLlamada");
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
