using Comun.Areas.Expendios;
using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Comun.General;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Gestion.Utilidades;
using Negocio.Interfaz.Expendios;
using Negocio.Interfaz.Irisp1;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Gestion.Expendios
{
    public class DbRegistroExpendio: IDbRegistroExpendio
    {


        #region Propiedades
        private readonly IConfiguration _iConfiguration;
       
        private readonly string _strConexionIris_Disec;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public DbRegistroExpendio(IConfiguration iConfiguration, ILogger<IDbRegistroExpendio> logger)
        {
            _iConfiguration = iConfiguration;
           
            _strConexionIris_Disec = _iConfiguration.GetConnectionString("strConexionIris_Disec");
            _logger = logger;
        }
        #endregion


        public async Task<DtoResultado<List<DtoExpendios>>> F_GetAniosIrisP1()
        {
            List<DtoExpendios> Retorno = new();
            DtoResultado<List<DtoExpendios>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_EXPENDIOS_IRIS.F_GetAniosIrisP1";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;

                if (Conexion.State == ConnectionState.Open)
                {
                    var reader = await objCommand.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        var domi = new DtoExpendios()
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

        public async Task<DtoResultado<List<DtoExpendios>>> F_GetInfoGrillas(Int32 V_Anio)
        {
            DataTable resultado = new();
            List<DtoExpendios> retorno = new();
            DtoResultado<List<DtoExpendios>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_EXPENDIOS_IRIS.F_GetInfoGrillas";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_Anio", OracleDbType.Int32, ParameterDirection.Input).Value = V_Anio;
                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());

                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoExpendios>(resultado);

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


        public async Task<DtoResultado<List<DtoIntegrantes>>> F_GetIntegrantes(string V_CriminalidadId)
        {
            var respuesta = new DtoResultado<List<DtoIntegrantes>>();

            try
            {
                using (var conn = new OracleConnection(_strConexionIris_Disec))
                {
                    await conn.OpenAsync();

                    using (var cmd = new OracleCommand("PK_EXPENDIOS_IRIS.F_GetIntegrantes", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;


                        cmd.Parameters.Add("P_Criminalidad_Id", OracleDbType.Varchar2, 40).Value = V_CriminalidadId;
                        cmd.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var lista = new List<DtoIntegrantes>();

                            while (await reader.ReadAsync())
                            {
                                lista.Add(new DtoIntegrantes
                                {
                                    INTEGRANTE_DIREC_ID = reader["INTEGRANTE_DIREC_ID"]?.ToString(),
                                    CRIMINALIDAD_DIREC_ID = reader["CRIMINALIDAD_DIREC_ID"]?.ToString(),
                                    ALIAS = reader["ALIAS"]?.ToString(),
                                    NOMBRE = reader["NOMBRE"]?.ToString(),
                                    APELLIDO = reader["APELLIDO"]?.ToString(),
                                    CEDULA = reader["CEDULA"] as long?,
                                    //ID_TIPO_INFO = reader["ID_TIPO_INFO"] as int?,
                                    VIGENTE = reader["VIGENTE"] as int?,
                                    FECHA_CREACION = reader["FECHA_CREACION"] as DateTime?,
                                    IDENTIFICACION_CREACION = reader["IDENTIFICACION_CREACION"] as long?,
                                    MAQUINA_CREACION = reader["MAQUINA_CREACION"]?.ToString(),
                                    FECHA_MODIFICA = reader["FECHA_MODIFICA"] as DateTime?,
                                    IDENTIFICACION_MODIFICA = reader["IDENTIFICACION_MODIFICA"] as long?,
                                    MAQUINA_MODIFICA = reader["MAQUINA_MODIFICA"]?.ToString(),
                                    //TIPO_DOCUMENTO = reader["TIPO_DOCUMENTO"] as int?,
                                    //CELULAR = reader["CELULAR"] as long?,
                                    //DIRECCION = reader["DIRECCION"]?.ToString(),
                                   // ID_INTEGRANTE = reader["ID_INTEGRANTE"] as long?,
                                    //ID_CRIMINALIDAD = reader["ID_CRIMINALIDAD"] as long?
                                });
                            }

                            respuesta.Data = lista;
                            respuesta.IdRespuesta = 1;
                            respuesta.Mensaje = "Consulta exitosa";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 0;
                respuesta.Mensaje = $"Error: {ex.Message}";
                respuesta.Data = new List<DtoIntegrantes>();
            }

            return respuesta;
        }



        public async Task<DtoResultado<List<DtoDelitosIris>>> F_GetDelitosIris(string V_CriminalidadId)
        {
            DataTable resultado = new();
            List<DtoDelitosIris> retorno = new();
            DtoResultado<List<DtoDelitosIris>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_EXPENDIOS_IRIS.F_GetDelitosIris";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_Criminalidad_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_CriminalidadId;
                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());
                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoDelitosIris>(resultado);

                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetDelitosIris";
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
                _logger.LogWarning("Error Ejecutando PK_REGISTRO_IRIS.F_GetDelitosIris " + e);

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



        public async Task<DtoResultado<List<DtoInfoAdicional>>> F_GetBitacora(string V_CriminalidadId)
        {
            DataTable resultado = new();
            List<DtoInfoAdicional> retorno = new();
            DtoResultado<List<DtoInfoAdicional>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_EXPENDIOS_IRIS.F_GetBitacora";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_Criminalidad_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_CriminalidadId;
                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());
                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoInfoAdicional>(resultado);

                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetDelitosIris";
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
                _logger.LogWarning("Error Ejecutando PK_EXPENDIOS_IRIS.F_GetBitacora " + e);

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



        public async Task<DtoResultado<List<DtoResultadosExpendio>>> F_GetResultados(string V_CriminalidadId)
        {
            DataTable resultado = new();
            List<DtoResultadosExpendio> retorno = new();
            DtoResultado<List<DtoResultadosExpendio>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_EXPENDIOS_IRIS.F_GetResultados";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_Criminalidad_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_CriminalidadId;
                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());
                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoResultadosExpendio>(resultado);

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
                _logger.LogWarning("Error Ejecutando PK_EXPENDIOS_IRIS.F_GetResultados " + e);

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
