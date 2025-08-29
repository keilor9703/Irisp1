using Comun.Areas.Irisp1;
using Comun.General;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Gestion.Utilidades;
using Negocio.Interfaz.Irisp1;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Gestion.Irisp1
{
     public class DbVerificacionIris: IDbVerificacionIris
    {



        #region Propiedades
        private readonly IConfiguration _iConfiguration;
        private readonly string _strConexionIris_Test;
        private readonly string _strConexionTelepol;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public DbVerificacionIris(IConfiguration iConfiguration, ILogger<IDbIrisp1> logger)
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
                objCommand.CommandText = "PK_VERIFICACION_IRIS.F_GetInfoGrillas";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_Anio", OracleDbType.Int32, ParameterDirection.Input).Value = V_Anio;
                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());
                   
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
                _logger.LogWarning("Error Ejecutando PK_VERIFICACION_IRIS.F_GetInfoGrillas " + e);

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



        public async Task<DtoResultado<List<DtoTareasIris>>> F_GetTareas(string V_ResponsableId)
        {
            DataTable resultado = new();
            List<DtoTareasIris> retorno = new();
            DtoResultado<List<DtoTareasIris>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Test);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_VERIFICACION_IRIS.F_GetTareas";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
            
                objCommand.Parameters.Add("P_Responsable_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_ResponsableId;
          
                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);


                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());

                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoTareasIris>(resultado);

                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetTareas";
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
                _logger.LogWarning("Error Ejecutando PK_VERIFICACION_IRIS.F_GetTareas " + e);

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


        public async Task<DtoResultado<List<DtoTareasIris>>> F_GetResultados(string V_Criminalidad, string V_ResponsableId)
        {
            DataTable resultado = new();
            List<DtoTareasIris> retorno = new();
            DtoResultado<List<DtoTareasIris>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Test);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_VERIFICACION_IRIS.F_GetResultados";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                objCommand.Parameters.Add("P_Ciminalidad_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_Criminalidad;
                objCommand.Parameters.Add("P_Responable_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_ResponsableId;

                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);


                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());

                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoTareasIris>(resultado);

                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetResultados";
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
                _logger.LogWarning("Error Ejecutando PK_VERIFICACION_IRIS.F_GetResultados " + e);

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

    }
}
