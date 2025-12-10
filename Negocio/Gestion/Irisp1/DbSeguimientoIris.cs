using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Comun.Areas.Irisp1;
using Comun.General;
using Dapper;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Negocio.Gestion.Utilidades;
using Negocio.Interfaz.Irisp1;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data;
using System.Diagnostics;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Gestion.Irisp1
{
    public class DbSeguimientoIris : IDbSeguimientoIris
    {
        #region Propiedades
        private readonly IConfiguration _iConfiguration;
        private readonly string _strConexionIris_Test;
        private readonly string _strConexionTelepol;
        private readonly string _strConexionIris_Disec;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public DbSeguimientoIris(IConfiguration iConfiguration, ILogger<IDbSeguimientoIris> logger)
        {
            _iConfiguration = iConfiguration;
            _strConexionIris_Test = _iConfiguration.GetConnectionString("strConexionIris_Test");
            _strConexionTelepol = _iConfiguration.GetConnectionString("strConexionTelepol");
            _strConexionIris_Disec = _iConfiguration.GetConnectionString("strConexionIris_Disec");
            _logger = logger;
        }
        #endregion



        #region Métodos de Consulta

        public async Task<DtoResultado<List<SeguimientoIrisDto>>> F_GetAniosIrisP1()
        {
            List<SeguimientoIrisDto> Retorno = new();
            DtoResultado<List<SeguimientoIrisDto>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
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
                        var domi = new SeguimientoIrisDto()
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

        public async Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetInfoGrillas(
      Int32 V_Anio, string RolesUsuario, Int64 CodigoUnidad)
        {
            var resp = new DtoResultado<List<DtoIrispCriminalidad>>();

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();

                parametros.Add("P_Anio", V_Anio, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_Roles", RolesUsuario, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_CodigoUnidad", CodigoUnidad, OracleMappingType.Int64, ParameterDirection.Input);

                // Cursor de salida
                parametros.Add("RESULT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                string sql = @"
                    BEGIN
                        :RESULT := PK_CONSULTA_IRISP.F_GetInfoGrillas(
                            :P_Anio,
                            :P_Roles,
                            :P_CodigoUnidad
                        );
                    END;";

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoIrispCriminalidad>(
                    sql,
                    parametros,
                    commandType: CommandType.Text
                )).ToList();

                // respuesta final
                resp.IdRespuesta = lista.Count > 0 ? 1 : 0;
                resp.Mensaje = lista.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";
                resp.Data = lista;
                resp.Operacion = "F_GetInfoGrillas";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando PK_CONSULTA_IRISP.F_GetInfoGrillas");
                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                resp.Data = null;
            }

            return resp;
        }



        public async Task<DtoResultado<List<DtoTareasIris>>> F_GetResponsablesTareasIris(string V_Criminalidad)
         {
            DataTable resultado = new();
            List<DtoTareasIris> retorno = new();
            DtoResultado<List<DtoTareasIris>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_SEGUIMIENTO_IRIS.F_GetResponsablesTareasIris"; // Ajusta si tu SP tiene otro nombre
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_Criminalidad_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_Criminalidad;
                // objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.ReturnValue);
                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor, ParameterDirection.Output);

                if (Conexion.State == ConnectionState.Open)
                {
                    //resultado.Load(await objCommand.ExecuteReaderAsync());

                    //retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoTareasIris>(resultado);



                    using var reader = await objCommand.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var dto = new DtoTareasIris
                        {
                            ResponValidacionId = reader["IDRESPONSABLE"]?.ToString(),
                            IdUnidadResponsable = reader["IDUNIDADRESPONSABLE"]?.ToString(),
                           // DescUnidad = reader["DESCUNIDAD"]?.ToString(),
                            UnidadCompleta = reader["UNIDADCOMPLETA"]?.ToString(),
                            Aceptada = reader["ACEPTADA"]?.ToString(),
                        };

                        // 👇 Aquí manejamos correctamente el CLOB:
                        if (reader["SEGUIMIENTO"] is OracleClob clob && !clob.IsNull)
                            dto.Seguimiento = clob.Value; // ← Extrae el texto completo del CLOB
                        else
                            dto.Seguimiento = reader["SEGUIMIENTO"]?.ToString();

                        retorno.Add(dto);
                    }

                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetResponsablesTareasIris";
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
                _logger.LogWarning("Error Ejecutando PK_SEGUIMIENTO_IRIS.F_GetResponsablesTareasIris " + e);

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

        //public async Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetResponsables(string V_CriminalidadId)
        //{
        //    DataTable resultado = new();
        //    List<DtoIrispCriminalidad> retorno = new();
        //    DtoResultado<List<DtoIrispCriminalidad>> resp = new();

        //    using var Conexion = new OracleConnection(_strConexionIris_Disec);
        //    using var objCommand = new OracleCommand();

        //    try
        //    {
        //        objCommand.Connection = Conexion;
        //        objCommand.CommandType = CommandType.StoredProcedure;
        //        objCommand.CommandText = "PK_SEGUIMIENTO_IRIS.P_GetResponsables";
        //        objCommand.BindByName = true;
        //        Conexion.Open();

        //        objCommand.Parameters.Clear();
        //        objCommand.Parameters.Add("P_Criminalidad_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_CriminalidadId;
        //        objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        if (Conexion.State == ConnectionState.Open)
        //        {
        //            resultado.Load(await objCommand.ExecuteReaderAsync());
        //            retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoIrispCriminalidad>(resultado);

        //            if (retorno.Count > 0)
        //            {
        //                resp.IdRespuesta = 1;
        //                resp.Mensaje = "Consulta Exitosa";
        //                resp.Operacion = "P_GetResponsables";
        //                resp.Data = retorno;
        //            }
        //            else
        //            {
        //                resp.IdRespuesta = 0;
        //                resp.Mensaje = "No se encontraron datos";
        //                resp.Operacion = "0";
        //            }
        //        }
        //        else
        //        {
        //            resp.IdRespuesta = 0;
        //            resp.Mensaje = "Error conexión base de datos";
        //            resp.Operacion = "0";
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        Conexion.Close();
        //        Conexion.Dispose();
        //        objCommand.Connection.Close();
        //        _logger.LogError("Creacion de log");
        //        _logger.LogWarning("Error Ejecutando PK_SEGUIMIENTO_IRIS.P_GetResponsables " + e);

        //        resp.IdRespuesta = 0;
        //        resp.Mensaje = $"{e.Message} - {e.InnerException}";
        //        resp.Operacion = "0";

        //    }
        //    finally
        //    {
        //        Conexion.Close();
        //        Conexion.Dispose();
        //        objCommand.Dispose();
        //        objCommand.Connection.Close();
        //        resultado.Dispose();
        //    }
        //    return resp;
        //}

        public async Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetResponsables(string V_CriminalidadId)
        {
            var retorno = new List<DtoIrispCriminalidad>();
            var resp = new DtoResultado<List<DtoIrispCriminalidad>>();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand("PK_SEGUIMIENTO_IRIS.P_GetResponsables", Conexion)
            {
                CommandType = CommandType.StoredProcedure,
                BindByName = true
            };

            try
            {
                objCommand.Parameters.Add("P_Criminalidad_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_CriminalidadId;
                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                await Conexion.OpenAsync();

                using var reader = await objCommand.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var dto = new DtoIrispCriminalidad
                    {
                        FuncionarioResponsable = reader["FuncionarioResponsable"] != DBNull.Value
                            ? reader["FuncionarioResponsable"].ToString()
                            : string.Empty,

                        UnidadFuncionarioResponsable = reader["UnidadFuncionarioResponsable"] != DBNull.Value
                            ? reader["UnidadFuncionarioResponsable"].ToString()
                            : string.Empty
                    };

                    retorno.Add(dto);
                }

                if (retorno.Count > 0)
                {
                    resp.IdRespuesta = 1;
                    resp.Mensaje = "Consulta Exitosa";
                    resp.Operacion = "P_GetResponsables";
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
                _logger.LogError("Error ejecutando PK_SEGUIMIENTO_IRIS.P_GetResponsables", e);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"{e.Message} - {e.InnerException}";
                resp.Operacion = "0";
            }
            finally
            {
                if (Conexion.State == ConnectionState.Open)
                    await Conexion.CloseAsync();

                objCommand.Dispose();
            }

            return resp;
        }


        public async Task<DtoResultado<List<DtoDominios>>> F_GetUnidadesSeguimiento()
        {
            DataTable resultado = new();
            List<DtoDominios> retorno = new();
            DtoResultado<List<DtoDominios>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();
            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_SEGUIMIENTO_IRIS.f_GetUnidadesSeguimiento";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
              



                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());

                    if (resultado.Rows.Count > 0)
                    {
                        foreach (DataRow fila in resultado.Rows)
                        {
                            retorno.Add(new DtoDominios
                            {
                                SIGLA = fila["SIGLA"].ToString(),
                                DESCRIPCION_DEPENDENCIA = fila["DESCRIPCION_DEPENDENCIA"].ToString()
                            });
                        }

                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetDominios";
                        resp.Data = retorno;
                    }
                    else
                    {
                        resp.IdRespuesta = 0;
                        resp.Mensaje = "No se encontraron datos";
                        resp.Operacion = "0";
                        resp.Data = new List<DtoDominios>(); // 👈💥 evita el NullReference
                    }

                    Conexion.Close();
                    Conexion.Dispose();
                    objCommand.Connection.Close();
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "No se pudo realizar la conexión a la base de datos";
                    resp.Operacion = "0";
                }

            }
            catch (Exception e)
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Connection.Close();
                _logger.LogError("Creacion de log");
                _logger.LogWarning("Error Ejecutando PK_SEGUIMIENTO_IRIS.f_GetUnidadesSeguimiento " + e);

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

        public async Task<DtoResultado<List<DtoDominios>>> F_GetUnidadesPorSigla(string V_Sigla)
        {
            DataTable resultado = new();
            List<DtoDominios> retorno = new();
            DtoResultado<List<DtoDominios>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();
            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_SEGUIMIENTO_IRIS.F_GetUnidadesPorSigla";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;

                objCommand.Parameters.Add("p_sigla", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_Sigla;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());

                    if (resultado.Rows.Count > 0)
                    {
                        foreach (DataRow fila in resultado.Rows)
                        {
                            retorno.Add(new DtoDominios
                            {
                                CONSECUTIVO = fila["CONSECUTIVO"].ToString(),
                                DESCRIPCION_DEPENDENCIA = fila["DESCRIPCION_DEPENDENCIA"].ToString(),
                                SIGLA = fila["SIGLA_PAPA"].ToString()
                            });
                        }

                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetDominios";
                        resp.Data = retorno;
                    }
                    else
                    {
                        resp.IdRespuesta = 0;
                        resp.Mensaje = "No se encontraron datos";
                        resp.Operacion = "0";
                        resp.Data = new List<DtoDominios>(); // 👈💥 evita el NullReference
                    }

                    Conexion.Close();
                    Conexion.Dispose();
                    objCommand.Connection.Close();
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "No se pudo realizar la conexión a la base de datos";
                    resp.Operacion = "0";
                }

            }
            catch (Exception e)
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Connection.Close();
                _logger.LogError("Creacion de log");
                _logger.LogWarning("Error Ejecutando PK_SEGUIMIENTO_IRIS.f_GetUnidadesSeguimiento " + e);

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



        public async Task<DtoResultado<Int32>> P_InsResponsable(DtoIrispCriminalidad Obj_Responsable, string usuario, string maquina)
        {
            DtoResultado<Int32> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_SEGUIMIENTO_IRIS.P_InsResponsable";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                objCommand.Parameters.Add("P_CRIMINALIDAD_ID", OracleDbType.Varchar2).Value = Obj_Responsable.CriminalidadId;
                objCommand.Parameters.Add("P_ID_UNIDAD", OracleDbType.Int32).Value = Obj_Responsable.IdUnidad ?? 0;
                objCommand.Parameters.Add("P_TAREA", OracleDbType.Int32).Value = Obj_Responsable.IdTareai ?? 1;
                objCommand.Parameters.Add("P_OBSERVACION", OracleDbType.Varchar2).Value = Obj_Responsable.Observacion;
                objCommand.Parameters.Add("P_USUARIO", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA", OracleDbType.Varchar2).Value = maquina;

                objCommand.Parameters.Add("P_RESULTADO", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                await objCommand.ExecuteNonQueryAsync();

                int resultado = Convert.ToInt32(objCommand.Parameters["P_RESULTADO"].Value.ToString());
                string mensaje = objCommand.Parameters["SRV_Message"].Value.ToString();

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
            }

            return resp;
        }



        public async Task<DtoResultado<Int32>> P_UpdUnidadResponsable(DtoIrispCriminalidad obj_responsableUpd, string usuario, string maquina)
        {
            DtoResultado<Int32> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_SEGUIMIENTO_IRIS.P_UpdUnidadResponsable";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                objCommand.Parameters.Add("P_RESPON_VALIDACION_ID", OracleDbType.Varchar2).Value = obj_responsableUpd.IdResponsable;
                objCommand.Parameters.Add("P_ID_UNIDAD_NUEVA", OracleDbType.Int32).Value = obj_responsableUpd.IdUnidad ?? 0;
               //objCommand.Parameters.Add("P_ID_DEPENDENCIA_NUEVA", OracleDbType.Int32).Value = obj_responsableUpd.IdDependencia ?? 0;
               
                objCommand.Parameters.Add("P_IDENTIFICACION_MODIFICA", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA_MODIFICA", OracleDbType.Varchar2).Value = maquina;

                objCommand.Parameters.Add("P_RESULTADO", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                await objCommand.ExecuteNonQueryAsync();

                int resultado = Convert.ToInt32(objCommand.Parameters["P_RESULTADO"].Value.ToString());
                string mensaje = objCommand.Parameters["SRV_Message"].Value.ToString();

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
            }

            return resp;
        }



        public async Task<DtoResultado<string>> P_FinalizarIris(DtoIrispCriminalidad datos, string usuario, string maquina)
        {
            DtoResultado<string> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_SEGUIMIENTO_IRIS.P_FinalizarIris";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                // Parámetros de entrada
                objCommand.Parameters.Add("P_CRIMINALIDAD_ID", OracleDbType.Varchar2).Value = datos.CriminalidadId;
              //  objCommand.Parameters.Add("P_ID_ESTADO", OracleDbType.Int32).Value = datos.IdEstado;
                objCommand.Parameters.Add("P_IDENTIFICACION_MODIFICA", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA_MODIFICA", OracleDbType.Varchar2).Value = maquina;
                objCommand.Parameters.Add("P_JUSTIFICACION", OracleDbType.Varchar2).Value = datos.Justificacion;

                // Parámetros de salida
                objCommand.Parameters.Add("P_RESULTADO", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;

                if (Conexion.State == ConnectionState.Open)
                    await objCommand.ExecuteNonQueryAsync();

                int resultado = Convert.ToInt32(objCommand.Parameters["P_RESULTADO"].Value?.ToString() ?? "0");
                string mensaje = objCommand.Parameters["SRV_Message"].Value?.ToString() ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? "OK" : "";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error ejecutando PK_SEGUIMIENTO_IRIS.P_FinalizarIris");
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





        public async Task<DtoResultado<Int32>> P_DelUnidadResponsable(DtoIrispCriminalidad obj_responsableUpd, string usuario, string maquina)
        {
            DtoResultado<Int32> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_SEGUIMIENTO_IRIS.P_DelUnidadResponsable";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                objCommand.Parameters.Add("P_RESPON_VALIDACION_ID", OracleDbType.Varchar2).Value = obj_responsableUpd.IdResponsable;
                objCommand.Parameters.Add("P_ID_UNIDAD", OracleDbType.Int32).Value = obj_responsableUpd.IdUnidad ?? 0;

                objCommand.Parameters.Add("P_IDENTIFICACION_MODIFICA", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA_MODIFICA", OracleDbType.Varchar2).Value = maquina;

                objCommand.Parameters.Add("P_RESULTADO", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                await objCommand.ExecuteNonQueryAsync();

                int resultado = Convert.ToInt32(objCommand.Parameters["P_RESULTADO"].Value.ToString());
                string mensaje = objCommand.Parameters["SRV_Message"].Value.ToString();

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
            }

            return resp;
        }




        public async Task<DtoResultado<Int32>> P_EvalTarea(DtoIrispCriminalidad obj_EvalTarea, string usuario, string maquina)
        {
            DtoResultado<Int32> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_SEGUIMIENTO_IRIS.P_EvalTarea";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                objCommand.Parameters.Add("P_TAREA_ID", OracleDbType.Varchar2).Value = obj_EvalTarea.IdTarea;
                objCommand.Parameters.Add("P_EVAL_TAREA_ID", OracleDbType.Int32).Value = obj_EvalTarea.IdEstado ?? 0;

                objCommand.Parameters.Add("P_IDENTIFICACION_MODIFICA", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA_MODIFICA", OracleDbType.Varchar2).Value = maquina;

                objCommand.Parameters.Add("P_RESULTADO", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                await objCommand.ExecuteNonQueryAsync();

                int resultado = Convert.ToInt32(objCommand.Parameters["P_RESULTADO"].Value.ToString());
                string mensaje = objCommand.Parameters["SRV_Message"].Value.ToString();

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
            }

            return resp;
        }


        public async Task<DtoResultado<Int32>> P_ReasignarTarea(DtoTareasIris obj_ReasignarTarea, string usuario, string maquina)
        {
            DtoResultado<Int32> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_SEGUIMIENTO_IRIS.P_ReasignarTarea";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                objCommand.Parameters.Add("p_CRIMINALIDAD_ID", OracleDbType.Varchar2).Value = obj_ReasignarTarea.CriminalidadId;
                objCommand.Parameters.Add("p_RESPON_VALIDACION_ID", OracleDbType.Varchar2).Value = obj_ReasignarTarea.ResponValidacionId;
                objCommand.Parameters.Add("p_OBSERVACION", OracleDbType.Varchar2).Value = obj_ReasignarTarea.Observacion ?? "";

                objCommand.Parameters.Add("p_IDENTIFICACION_CREACION", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("p_MAQUINA_CREACION", OracleDbType.Varchar2).Value = maquina;

                objCommand.Parameters.Add("P_RESULTADO", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                await objCommand.ExecuteNonQueryAsync();

                int resultado = Convert.ToInt32(objCommand.Parameters["P_RESULTADO"].Value.ToString());
                string mensaje = objCommand.Parameters["SRV_Message"].Value.ToString();

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
            }
            finally
            {
                Conexion.Close();
                Conexion.Dispose();
                objCommand.Dispose();
            }

            return resp;
        }




    }
}
