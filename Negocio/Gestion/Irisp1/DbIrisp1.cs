using Comun.Areas.Admin;
using Comun.Areas.AplicacionDTO;
using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Comun.General;
using Dapper;
using Dapper.Oracle;
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
    public class DbIrisp1 : IDbIrisp1
    {
        #region Propiedades
        private readonly IConfiguration _iConfiguration;
        private readonly string _strConexionIris_Test;
        private readonly string _strConexionIris_Disec;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public DbIrisp1(IConfiguration iConfiguration, ILogger<IDbIrisp1> logger  )
        {
            _iConfiguration = iConfiguration;
            _strConexionIris_Test = _iConfiguration.GetConnectionString("strConexionIris_Test");
            _strConexionIris_Disec = _iConfiguration.GetConnectionString("strConexionIris_Disec");
            _logger = logger;
        }
        #endregion


        #region Métodos de Consulta

        public async Task<DtoResultado<List<DtoIrisp1>>> F_GetAniosIrisP1()
        {
            List<DtoIrisp1> Retorno = new();
            DtoResultado<List<DtoIrisp1>> resp = new();

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

        //public async Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetInfoGrillas(Int32 V_Anio)
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
        //        objCommand.CommandText = "PK_CONSULTA_IRISP.F_GetInfoGrillas";
        //        objCommand.BindByName = true;
        //        Conexion.Open();

        //        objCommand.Parameters.Clear();
        //        objCommand.Parameters.Add("P_Anio", OracleDbType.Int32, ParameterDirection.Input).Value = V_Anio;
        //        objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

        //        if (Conexion.State == ConnectionState.Open)
        //        {
        //            resultado.Load(await objCommand.ExecuteReaderAsync());

        //            retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoIrispCriminalidad>(resultado);

        //            if (retorno.Count > 0)
        //            {
        //                resp.IdRespuesta = 1;
        //                resp.Mensaje = "Consulta Exitosa";
        //                resp.Operacion = "F_GetInfoGrillas";
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
        //        _logger.LogWarning("Error Ejecutando PK_CONSULTA_IRISP.F_GetInfoGrillas " + e);

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

        //public async Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetInfoGrillas( Int32 V_Anio, string RolesUsuario, Int64 CodigoUnidad)
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
        //        objCommand.CommandText = "PK_CONSULTA_IRISP.F_GetInfoGrillas";
        //        objCommand.BindByName = true;
        //        Conexion.Open();

        //        objCommand.Parameters.Clear();
        //        objCommand.Parameters.Add("P_Anio", OracleDbType.Int32, ParameterDirection.Input).Value = V_Anio;
        //        objCommand.Parameters.Add("P_Roles", OracleDbType.Varchar2, ParameterDirection.Input).Value = RolesUsuario;
        //        objCommand.Parameters.Add("P_CodigoUnidad", OracleDbType.Int64, ParameterDirection.Input).Value = CodigoUnidad;
        //        objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

        //        if (Conexion.State == ConnectionState.Open)
        //        {
        //            resultado.Load(await objCommand.ExecuteReaderAsync());
        //            retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoIrispCriminalidad>(resultado);

        //            resp.IdRespuesta = retorno.Count > 0 ? 1 : 0;
        //            resp.Mensaje = retorno.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";
        //            resp.Data = retorno;
        //            resp.Operacion = "F_GetInfoGrillas";
        //        }
        //        else
        //        {
        //            resp.IdRespuesta = 0;
        //            resp.Mensaje = "Error conexión base de datos";
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "Error ejecutando PK_CONSULTA_IRISP.F_GetInfoGrillas");
        //        resp.IdRespuesta = 0;
        //        resp.Mensaje = e.Message;
        //    }
        //    finally
        //    {
        //        Conexion.Close();
        //        objCommand.Dispose();
        //    }

        //    return resp;
        //}



        public async Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetInfoGrillas( Int32 V_Anio, string RolesUsuario, Int64 CodigoUnidad)
        {
            var resp = new DtoResultado<List<DtoIrispCriminalidad>>();

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();
                parametros.Add("P_Anio", V_Anio, OracleMappingType.Int32, ParameterDirection.Input);
                parametros.Add("P_Roles", RolesUsuario, OracleMappingType.Varchar2, ParameterDirection.Input);
                parametros.Add("P_CodigoUnidad", CodigoUnidad, OracleMappingType.Int64, ParameterDirection.Input);

                // 🔥 Cursor de salida
                parametros.Add("RESULT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                var sql = @"
                            BEGIN
                                :RESULT := PK_CONSULTA_IRISP.F_GetInfoGrillas(
                                    :P_Anio,
                                    :P_Roles,
                                    :P_CodigoUnidad
                                );
                            END;";

                await connection.OpenAsync();

                // 🔥 Mapeo automático directo a la lista de DTO
                var lista = (await connection.QueryAsync<DtoIrispCriminalidad>(
                    sql,
                    parametros,
                    commandType: CommandType.Text
                )).ToList();

                resp.IdRespuesta = lista.Count > 0 ? 1 : 0;
                resp.Mensaje = lista.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";
                resp.Data = lista;
                resp.Operacion = "F_GetInfoGrillas";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper F_GetInfoGrillas");
                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                resp.Data = null;
            }

            return resp;
        }



        //public async Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetInfoGrillas(Int32 V_Anio)
        //{
        //    List<DtoIrispCriminalidad> retorno = new();
        //    DtoResultado<List<DtoIrispCriminalidad>> resp = new();

        //    using var Conexion = new OracleConnection(_strConexionIris_Disec);
        //    using var objCommand = new OracleCommand();

        //    try
        //    {
        //        objCommand.Connection = Conexion;
        //        objCommand.CommandType = CommandType.StoredProcedure;
        //        objCommand.CommandText = "PK_CONSULTA_IRISP.F_GetInfoGrillas";
        //        objCommand.BindByName = true;

        //        objCommand.Parameters.Clear();
        //        objCommand.Parameters.Add("P_Anio", OracleDbType.Int32, ParameterDirection.Input).Value = V_Anio;
        //        objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

        //        await Conexion.OpenAsync();

        //        using var reader = await objCommand.ExecuteReaderAsync();
        //        while (await reader.ReadAsync())
        //        {
        //            var dto = new DtoIrispCriminalidad
        //            {
        //                // ---------- Identificadores ----------
        //                CriminalidadId = reader["CriminalidadId"]?.ToString(),
        //                IdResponsable = reader["IdResponsable"]?.ToString(),
        //                IdUnidad = reader["IdUnidad"] == DBNull.Value ? null : Convert.ToInt64(reader["IdUnidad"]),
        //                IdUnidadResponsable = reader["IdUnidadResponsable"] == DBNull.Value ? null : Convert.ToInt64(reader["IdUnidad"]),

        //                // ---------- Numéricos ----------
        //                IdZona = reader["IdZona"] == DBNull.Value ? null : Convert.ToInt32(reader["IdZona"]),
        //                IdentificacionInforma = reader["IdentificacionInforma"] == DBNull.Value ? null : Convert.ToInt64(reader["IdentificacionInforma"]),
        //                IdTipoServicio = reader["IdTipoServicio"] == DBNull.Value ? null : Convert.ToInt32(reader["IdTipoServicio"]),
        //                IdCuadrante = reader["IdCuadrante"] == DBNull.Value ? null : Convert.ToInt32(reader["IdCuadrante"]),
        //                IdClase = reader["IdClase"] == DBNull.Value ? null : Convert.ToInt32(reader["IdClase"]),
        //                CantidadIntegrantes = reader["CantidadIntegrantes"] == DBNull.Value ? null : Convert.ToInt32(reader["CantidadIntegrantes"]),
        //                Vigente = reader["Vigente"] == DBNull.Value ? null : Convert.ToInt32(reader["Vigente"]),
        //                ConsecutivoCodigo = reader["ConsecutivoCodigo"] == DBNull.Value ? null : Convert.ToInt32(reader["ConsecutivoCodigo"]),
        //                IdEstado = reader["IdEstado"] == DBNull.Value ? null : Convert.ToInt32(reader["IdEstado"]),
        //                IdFuente = reader["IdFuente"] == DBNull.Value ? null : Convert.ToInt32(reader["IdFuente"]),
        //                IdEstadoExistencia = reader["IdEstadoExistencia"] == DBNull.Value ? null : Convert.ToInt32(reader["IdEstadoExistencia"]),
        //                CelularCuadrante = reader["CelularCuadrante"] == DBNull.Value ? null : Convert.ToInt64(reader["CelularCuadrante"]),

        //                // ---------- Fechas ----------
        //                FechaInicioExistencia = reader["FechaInicioExistencia"] == DBNull.Value ? null : Convert.ToDateTime(reader["FechaInicioExistencia"]),
        //                FechaCreacion = reader["FechaCreacion"] == DBNull.Value ? null : Convert.ToDateTime(reader["FechaCreacion"]),
        //                FechaModifica = reader["FechaModifica"] == DBNull.Value ? null : Convert.ToDateTime(reader["FechaModifica"]),

        //                // ---------- Auditoría ----------
        //                IdentificacionCrea = reader["IdentificacionCrea"] == DBNull.Value ? null : Convert.ToInt64(reader["IdentificacionCrea"]),
        //                MaquinaCrea = reader["MaquinaCrea"]?.ToString(),
        //                IdentificacionModifica = reader["IdentificacionModifica"] == DBNull.Value ? null : Convert.ToInt64(reader["IdentificacionModifica"]),
        //                MaquinaModifica = reader["MaquinaModifica"]?.ToString(),

        //                // ---------- Códigos ----------
        //                Codigo = reader["Codigo"]?.ToString(),
        //                SiglaUnidad = reader["SiglaUnidad"]?.ToString(),

        //                // ---------- Texto ----------
        //                CaracteristicasGenerales = reader["CaracteristicasGenerales"]?.ToString(),
        //                DescripcionTramite = reader["DescripcionTramite"]?.ToString(),
        //                Celular = reader["Celular"]?.ToString(),
        //                EstadoDescripcion = reader["EstadoDescripcion"]?.ToString(),
        //                EstadoExistenciaDescripcion = reader["EstadoExistenciaDescripcion"]?.ToString(),
        //                Municipio = reader["Municipio"]?.ToString(),
        //                Zona = reader["Zona"]?.ToString(),
        //                TipoServicio = reader["TipoServicio"]?.ToString(),
        //                Fuente = reader["Fuente"]?.ToString(),
        //                Clase = reader["Clase"]?.ToString(),
        //                NombreClase = reader["NombreClase"]?.ToString(),
        //                UnidadResponsable = reader["UnidadResponsable"]?.ToString(),

        //                // ---------- Cuadrantes ----------
        //                Cuadrante = reader["Cuadrante"]?.ToString(),
        //                DependCuadrante = reader["DependCuadrante"]?.ToString(),
        //                Estacioncuadrante = reader["Estacioncuadrante"]?.ToString(),
        //                Nivel1cuadrante = reader["Nivel1cuadrante"]?.ToString(),
        //                Dependencia = reader["Dependencia"]?.ToString(),

        //                // ---------- Resultados ----------
        //                IdTipoResultado = reader["IdTipoResultado"]?.ToString(),
        //                DescTipoResultado = reader["DescTipoResultado"]?.ToString(),
        //                NumeroResultado = reader["NumeroResultado"]?.ToString(),
        //                EstadoResultados = reader["EstadoResultados"]?.ToString()
        //            };


        //            retorno.Add(dto);
        //        }

        //        resp.IdRespuesta = retorno.Any() ? 1 : 0;
        //        resp.Mensaje = retorno.Any() ? "Consulta Exitosa" : "No se encontraron datos";
        //        resp.Operacion = "F_GetInfoGrillas";
        //        resp.Data = retorno;
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError("Error Ejecutando PK_CONSULTA_IRISP.F_GetInfoGrillas ", e);
        //        resp.IdRespuesta = 0;
        //        resp.Mensaje = $"{e.Message} - {e.InnerException}";
        //        resp.Operacion = "0";
        //    }
        //    finally
        //    {
        //        await Conexion.CloseAsync();
        //    }

        //    return resp;
        //}



        public async Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetCuadrantes(string V_unidadLabora, string V_unidadLabora2)
        {
            DataTable resultado = new();
            List<DtoIrispCriminalidad> retorno = new();
            DtoResultado<List<DtoIrispCriminalidad>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
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
                objCommand.Parameters.Add("P_Dependencia2", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_unidadLabora2;
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

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
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

        public async Task<DtoResultado<long>> F_ConsultarSeqIntegrante()
        {
            DtoResultado<long> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.Text; // IMPORTANTE: Es una función, no un procedimiento
                objCommand.CommandText = "SELECT PK_REGISTRO_IRIS.f_consultar_seq_integrante FROM dual";
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


        public async Task<DtoResultado<List<DtoIntegrantes>>> F_GetIntegrantes(string V_CriminalidadId)
        {
            var respuesta = new DtoResultado<List<DtoIntegrantes>>();

            try
            {
                using (var conn = new OracleConnection(_strConexionIris_Disec))
                {
                    await conn.OpenAsync();

                    using (var cmd = new OracleCommand("PK_REGISTRO_IRIS.F_GetIntegrantes", conn))
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
                                    INTEGRANTE_ID = reader["INTEGRANTE_ID"]?.ToString(),
                                    CRIMINALIDAD_ID = reader["CRIMINALIDAD_ID"]?.ToString(),
                                    ALIAS = reader["ALIAS"]?.ToString(),
                                    NOMBRE = reader["NOMBRE"]?.ToString(),
                                    APELLIDO = reader["APELLIDO"]?.ToString(),
                                    CEDULA = reader["CEDULA"] as long?,
                                    ID_TIPO_INFO = reader["ID_TIPO_INFO"] as int?,
                                    VIGENTE = reader["VIGENTE"] as int?,
                                    FECHA_CREACION = reader["FECHA_CREACION"] as DateTime?,
                                    IDENTIFICACION_CREACION = reader["IDENTIFICACION_CREACION"] as long?,
                                    MAQUINA_CREACION = reader["MAQUINA_CREACION"]?.ToString(),
                                    FECHA_MODIFICA = reader["FECHA_MODIFICA"] as DateTime?,
                                    IDENTIFICACION_MODIFICA = reader["IDENTIFICACION_MODIFICA"] as long?,
                                    MAQUINA_MODIFICA = reader["MAQUINA_MODIFICA"]?.ToString(),
                                    TIPO_DOCUMENTO = reader["TIPO_DOCUMENTO"] as int?,
                                    CELULAR = reader["CELULAR"] as long?,
                                    DIRECCION = reader["DIRECCION"]?.ToString(),
                                    ID_INTEGRANTE = reader["ID_INTEGRANTE"] as long?,
                                    ID_CRIMINALIDAD = reader["ID_CRIMINALIDAD"] as long?
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



        public async Task<DtoResultado<List<DtoIntegrantes>>> F_GetIntegrantesPreliminar(string V_CriminalidadId)
        {
            var respuesta = new DtoResultado<List<DtoIntegrantes>>();

            try
            {
                using (var conn = new OracleConnection(_strConexionIris_Disec))
                {
                    await conn.OpenAsync();

                    using (var cmd = new OracleCommand("PK_REGISTRO_IRIS.F_GetIntegrantesPreliminar", conn))
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
                                    INTEGRANTE_ID = reader["INTEGRANTE_ID"]?.ToString(),
                                    CRIMINALIDAD_ID = reader["CRIMINALIDAD_ID"]?.ToString(),
                                    ALIAS = reader["ALIAS"]?.ToString(),
                                    NOMBRE = reader["NOMBRE"]?.ToString(),
                                    APELLIDO = reader["APELLIDO"]?.ToString(),
                                    CEDULA = reader["CEDULA"] as long?,
                                    ID_TIPO_INFO = reader["ID_TIPO_INFO"] as int?,
                                    VIGENTE = reader["VIGENTE"] as int?,
                                    FECHA_CREACION = reader["FECHA_CREACION"] as DateTime?,
                                    IDENTIFICACION_CREACION = reader["IDENTIFICACION_CREACION"] as long?,
                                    MAQUINA_CREACION = reader["MAQUINA_CREACION"]?.ToString(),
                                    FECHA_MODIFICA = reader["FECHA_MODIFICA"] as DateTime?,
                                    IDENTIFICACION_MODIFICA = reader["IDENTIFICACION_MODIFICA"] as long?,
                                    MAQUINA_MODIFICA = reader["MAQUINA_MODIFICA"]?.ToString(),
                                    TIPO_DOCUMENTO = reader["TIPO_DOCUMENTO"] as int?,
                                    CELULAR = reader["CELULAR"] as long?,
                                    DIRECCION = reader["DIRECCION"]?.ToString(),
                                    //ID_INTEGRANTE = reader["ID_INTEGRANTE"] as long?,
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
                objCommand.CommandText = "PK_REGISTRO_IRIS.F_GetDelitosIris";
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

        public async Task<DtoResultado<List<DtoCriminalidadFoto>>> F_GetCriminalidadFotos(string V_CriminalidadId)
        {
            DataTable resultado = new();
            List<DtoCriminalidadFoto> retorno = new();
            DtoResultado<List<DtoCriminalidadFoto>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_REGISTRO_IRIS.F_GetCriminalidadFotos";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_Criminalidad_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_CriminalidadId;
                objCommand.Parameters.Add("P_Result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());
                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoCriminalidadFoto>(resultado);

                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetCriminalidadFotos";
                        resp.Data = retorno;
                    }
                    else
                    {
                        resp.IdRespuesta = 0;
                        resp.Mensaje = "No se encontraron fotos";
                        resp.Operacion = "0";
                    }
                }
            }
            catch (Exception e)
            {
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

        public async Task<DtoResultado<List<DtoInfoAdicional>>> F_GetInfoAdicional(string V_CriminalidadId)
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
                objCommand.CommandText = "PK_REGISTRO_IRIS.F_GetInfoAdicional";
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
                _logger.LogWarning("Error Ejecutando PK_REGISTRO_IRIS.F_GetInfoAdicional " + e);

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

        public async Task<DtoResultado<List<DtoDocumentoIris>>> F_GetDocIris(string V_CriminalidadId)
        {
            DataTable resultado = new();
            List<DtoDocumentoIris> retorno = new();
            DtoResultado<List<DtoDocumentoIris>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_REGISTRO_IRIS.F_GetDocIris";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_Criminalidad_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_CriminalidadId;
                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());
                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoDocumentoIris>(resultado);

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
                _logger.LogWarning("Error Ejecutando PK_REGISTRO_IRIS.F_GetDocIris " + e);

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



        public async Task<DtoResultado<List<DtoUbicacionIris>>> F_GetUbicacionIris(string V_CriminalidadId)
        {
            DataTable resultado = new();
            List<DtoUbicacionIris> retorno = new();
            DtoResultado<List<DtoUbicacionIris>> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_REGISTRO_IRIS.F_GetUbicacionIris";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add("P_Criminalidad_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = V_CriminalidadId;
                objCommand.Parameters.Add("RETURN_VALUE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                if (Conexion.State == ConnectionState.Open)
                {
                    resultado.Load(await objCommand.ExecuteReaderAsync());
                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoUbicacionIris>(resultado);

                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetUbicacionIris";
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
                _logger.LogWarning("Error Ejecutando PK_REGISTRO_IRIS.F_GetUbicacionIris " + e);

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



        #region Métodos de Insersión

        public async Task<DtoResultado<Int32>> P_InsIntegrantes(DtoIntegrantes Obj_Integrante, string usuario, string maquina)
        {
            DtoResultado<Int32> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_REGISTRO_IRIS.P_InsIntegrantes";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

               //objCommand.Parameters.Add("P_INTEGRANTE_ID", OracleDbType.Varchar2).Value = Obj_Integrante.INTEGRANTE_ID;
                objCommand.Parameters.Add("P_CRIMINALIDAD_ID", OracleDbType.Varchar2).Value = Obj_Integrante.CRIMINALIDAD_ID;
                objCommand.Parameters.Add("P_ALIAS", OracleDbType.Varchar2).Value = Obj_Integrante.ALIAS;
                objCommand.Parameters.Add("P_NOMBRE", OracleDbType.Varchar2).Value = Obj_Integrante.NOMBRE;
                objCommand.Parameters.Add("P_APELLIDO", OracleDbType.Varchar2).Value = Obj_Integrante.APELLIDO;
                objCommand.Parameters.Add("P_CEDULA", OracleDbType.Int64).Value = Obj_Integrante.CEDULA;
                objCommand.Parameters.Add("P_ID_TIPO_INFO", OracleDbType.Int32).Value = Obj_Integrante.ID_TIPO_INFO;
               // objCommand.Parameters.Add("P_VIGENTE", OracleDbType.Int32).Value = Obj_Integrante.VIGENTE ?? 1;
     
                objCommand.Parameters.Add("P_IDENTIFICACION_CREACION", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA_CREACION", OracleDbType.Varchar2).Value = maquina;
                //objCommand.Parameters.Add("P_FECHA_MODIFICA", OracleDbType.Date).Value = Obj_Integrante.FECHA_MODIFICA;
                //objCommand.Parameters.Add("P_IDENTIFICACION_MODIFICA", OracleDbType.Int64).Value = Obj_Integrante.IDENTIFICACION_MODIFICA;
                //objCommand.Parameters.Add("P_MAQUINA_MODIFICA", OracleDbType.Varchar2).Value = Obj_Integrante.MAQUINA_MODIFICA;
                objCommand.Parameters.Add("P_TIPO_DOCUMENTO", OracleDbType.Int32).Value = Obj_Integrante.TIPO_DOCUMENTO;
                objCommand.Parameters.Add("P_CELULAR", OracleDbType.Int64).Value = Obj_Integrante.CELULAR;
                objCommand.Parameters.Add("P_DIRECCION", OracleDbType.Varchar2).Value = Obj_Integrante.DIRECCION;
               // objCommand.Parameters.Add("P_ID_INTEGRANTE", OracleDbType.Int32).Value = Obj_Integrante.ID_INTEGRANTE;
                //objCommand.Parameters.Add("P_ID_CRIMINALIDAD", OracleDbType.Int32).Value = Obj_Integrante.ID_CRIMINALIDAD;

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


        public async Task<DtoResultado<Int32>> P_InsIntegrantesPreliminar(DtoIntegrantes Obj_Integrante, string usuario, string maquina)
        {
            DtoResultado<Int32> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_REGISTRO_IRIS.P_InsIntegrantesPreliminar";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                //objCommand.Parameters.Add("P_INTEGRANTE_ID", OracleDbType.Varchar2).Value = Obj_Integrante.INTEGRANTE_ID;
                objCommand.Parameters.Add("P_CRIMINALIDAD_ID", OracleDbType.Varchar2).Value = Obj_Integrante.CRIMINALIDAD_ID;
                objCommand.Parameters.Add("P_ALIAS", OracleDbType.Varchar2).Value = Obj_Integrante.ALIAS;
                objCommand.Parameters.Add("P_NOMBRE", OracleDbType.Varchar2).Value = Obj_Integrante.NOMBRE;
                objCommand.Parameters.Add("P_APELLIDO", OracleDbType.Varchar2).Value = Obj_Integrante.APELLIDO;
                objCommand.Parameters.Add("P_CEDULA", OracleDbType.Int64).Value = Obj_Integrante.CEDULA;
                objCommand.Parameters.Add("P_ID_TIPO_INFO", OracleDbType.Int32).Value = Obj_Integrante.ID_TIPO_INFO;
                // objCommand.Parameters.Add("P_VIGENTE", OracleDbType.Int32).Value = Obj_Integrante.VIGENTE ?? 1;

                objCommand.Parameters.Add("P_IDENTIFICACION_CREACION", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA_CREACION", OracleDbType.Varchar2).Value = maquina;
                //objCommand.Parameters.Add("P_FECHA_MODIFICA", OracleDbType.Date).Value = Obj_Integrante.FECHA_MODIFICA;
                //objCommand.Parameters.Add("P_IDENTIFICACION_MODIFICA", OracleDbType.Int64).Value = Obj_Integrante.IDENTIFICACION_MODIFICA;
                //objCommand.Parameters.Add("P_MAQUINA_MODIFICA", OracleDbType.Varchar2).Value = Obj_Integrante.MAQUINA_MODIFICA;
                objCommand.Parameters.Add("P_TIPO_DOCUMENTO", OracleDbType.Int32).Value = Obj_Integrante.TIPO_DOCUMENTO;
                objCommand.Parameters.Add("P_CELULAR", OracleDbType.Int64).Value = Obj_Integrante.CELULAR;
                objCommand.Parameters.Add("P_DIRECCION", OracleDbType.Varchar2).Value = Obj_Integrante.DIRECCION;
                // objCommand.Parameters.Add("P_ID_INTEGRANTE", OracleDbType.Int32).Value = Obj_Integrante.ID_INTEGRANTE;
                //objCommand.Parameters.Add("P_ID_CRIMINALIDAD", OracleDbType.Int32).Value = Obj_Integrante.ID_CRIMINALIDAD;

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

        public async Task<DtoResultado<string>> P_InsRegistroIrisP1(DtoIrispCriminalidad datos, string usuario, string maquina)
        {
            DtoResultado<string> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec); // Usa tu cadena correcta
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_REGISTRO_IRIS.P_InsCriminalidad";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                // Parámetros de entrada
                objCommand.Parameters.Add("P_CRIMINALIDAD_ID", OracleDbType.Varchar2).Value = datos.CriminalidadId;
                objCommand.Parameters.Add("P_ID_UNIDAD", OracleDbType.Int32).Value = datos.IdUnidad;
                objCommand.Parameters.Add("P_ID_ZONA", OracleDbType.Int32).Value = datos.IdZona ?? (object)DBNull.Value;
                objCommand.Parameters.Add("P_IDENTIFICACION_INFORMA", OracleDbType.Int64).Value = datos.IdentificacionInforma ?? (object)DBNull.Value;
                objCommand.Parameters.Add("P_CELULAR", OracleDbType.Varchar2).Value = datos.Celular ?? "";
                objCommand.Parameters.Add("P_ID_TIPO_SERVICIO", OracleDbType.Int32).Value = datos.IdTipoServicio;
                objCommand.Parameters.Add("P_ID_CUADRANTE", OracleDbType.Int32).Value = datos.IdCuadrante;
                objCommand.Parameters.Add("P_ID_CLASE", OracleDbType.Int32).Value = datos.IdClase;
                objCommand.Parameters.Add("P_NOMBRE_CLASE", OracleDbType.Varchar2).Value = datos.NombreClase ?? "";
                objCommand.Parameters.Add("P_CANTIDAD_INTEGRANTE", OracleDbType.Int32).Value = datos.CantidadIntegrantes ?? 0;
                objCommand.Parameters.Add("P_CARACTERISTICAS_GENERALES", OracleDbType.NVarchar2).Value = datos.CaracteristicasGenerales ?? "";
                objCommand.Parameters.Add("P_VIGENTE", OracleDbType.Int32).Value = datos.Vigente;
                objCommand.Parameters.Add("P_IDENTIFICACION_CREA", OracleDbType.Int64).Value = Convert.ToInt64(usuario);
                objCommand.Parameters.Add("P_MAQUINA_CREACION", OracleDbType.Varchar2).Value = maquina ?? "";
                objCommand.Parameters.Add("P_SIGLA_UNIDAD", OracleDbType.Varchar2).Value = datos.SiglaUnidad ?? "";
                objCommand.Parameters.Add("P_ID_ESTADO", OracleDbType.Int32).Value = datos.IdEstado;
                objCommand.Parameters.Add("P_ID_FUENTE", OracleDbType.Int32).Value = datos.IdFuente;
               // objCommand.Parameters.Add("P_DESCRIPCION_TRAMITE", OracleDbType.NVarchar2).Value = datos.CaracteristicasGenerales ?? ""; // o algún otro campo que represente la descripción
                objCommand.Parameters.Add("P_ENTORNO_AFECTADO", OracleDbType.Int32).Value = datos.EntornoAfectado;
                objCommand.Parameters.Add("P_ID_TIEMPO_DELITO", OracleDbType.Int32).Value = datos.IdtiempoDelito;
                objCommand.Parameters.Add("P_CLASIFICACION", OracleDbType.Int32).Value = datos.Clasificacion ?? 0;
                objCommand.Parameters.Add("P_MODALIDAD_EXPENDIO", OracleDbType.Int32).Value = datos.Modalidadexpendio ?? 0;
                objCommand.Parameters.Add("P_ORIGEN", OracleDbType.NVarchar2).Value = datos.Origen ?? "WEB";
                objCommand.Parameters.Add("P_NOMBRE_ENTORNO_AFECTADO", OracleDbType.NVarchar2).Value = datos.NombreEntornoAfectado ?? "";
                objCommand.Parameters.Add("P_ESPECIALIDAD_APORTA_INFO", OracleDbType.Int32).Value = datos.EspecialidadAporta ?? 0;
                objCommand.Parameters.Add("P_ID_CRIMINALIDAD", OracleDbType.Int64).Value = datos.IdCriminalidad;
                objCommand.Parameters.Add("P_ID_DELITO_PRIN", OracleDbType.Int64).Value = datos.IdDelitoPrincipal;
                objCommand.Parameters.Add("P_ID_DELITO_SECUNDARIO", OracleDbType.NVarchar2).Value =  string.Join(",", datos.IdDelitoSecundario);
                objCommand.Parameters.Add("P_ID_TINFO", OracleDbType.Int64).Value = datos.IdTipoInfo;

                objCommand.Parameters.Add("P_LONGITUD", OracleDbType.Varchar2).Value = datos.Longitud ?? "";
                objCommand.Parameters.Add("P_LATITUD", OracleDbType.Varchar2).Value = datos.Latitud ?? "";
                objCommand.Parameters.Add("P_MUNICIPIO", OracleDbType.Varchar2).Value = datos.MunicipioUbica ?? "";
                objCommand.Parameters.Add("P_BARRIO", OracleDbType.Varchar2).Value = datos.Barrio ?? "";
                objCommand.Parameters.Add("P_CUADRANTE", OracleDbType.Varchar2).Value = datos.Cuadrante ?? "";
                objCommand.Parameters.Add("P_RADACCION", OracleDbType.Int32).Value = datos.RadioAccion ?? 0;
                objCommand.Parameters.Add("P_DIRECCION", OracleDbType.Varchar2).Value = datos.Direccion ?? "";
                objCommand.Parameters.Add("P_COD_DANE", OracleDbType.Int32).Value = datos.CodigoDane ?? 0;
                objCommand.Parameters.Add("P_COD_ESTACION", OracleDbType.Int32).Value = datos.CodigoEstacion ?? 0;
                objCommand.Parameters.Add("P_COD_SIEDO_CTE", OracleDbType.Int32).Value = datos.CodigoSiedcoCuadrante ?? 0;





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
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_InsCriminalidad");
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

        public async Task<DtoResultado<string>> P_InsDelitosIris(DtoIrispCriminalidad datos, string usuario, string maquina)
        {
            DtoResultado<string> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec); // Usa tu cadena correcta
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_REGISTRO_IRIS.P_InsDelito";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                // Parámetros de entrada
                objCommand.Parameters.Add("P_CRIMINALIDAD_ID", OracleDbType.Varchar2).Value = datos.CriminalidadId;
                objCommand.Parameters.Add("P_ID_CRIMINALIDAD", OracleDbType.Int64).Value = datos.IdCriminalidad;
                objCommand.Parameters.Add("P_ID_DELITO_PRIN", OracleDbType.Int64).Value = datos.IdDelitoPrincipal;
                objCommand.Parameters.Add("P_ID_DELITO_SECUNDARIO", OracleDbType.NVarchar2).Value = string.Join(",", datos.IdDelitoSecundario);
                objCommand.Parameters.Add("P_IDENTIFICACION_CREA", OracleDbType.Int64).Value = Convert.ToInt64(usuario);
                objCommand.Parameters.Add("P_MAQUINA_CREACION", OracleDbType.Varchar2).Value = maquina ?? "";
                
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
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_InsDelito");
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


        public async Task<DtoResultado<string>> P_InsInfoAdicionalIris(DtoInfoAdicional datos, string usuario, string maquina)
        {
            DtoResultado<string> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec); // Usa tu cadena correcta
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_REGISTRO_IRIS.P_InsInfoAdicionalIris";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                // Parámetros de entrada
                objCommand.Parameters.Add("P_CRIMINALIDAD_ID", OracleDbType.Varchar2).Value = datos.CriminalidadId;
                objCommand.Parameters.Add("P_DESCRIPCION", OracleDbType.NVarchar2).Value = datos.Descripcion;
                objCommand.Parameters.Add("P_IDENTIFICACION_CREACION", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA_CREACION", OracleDbType.Varchar2).Value = maquina;
               

                // Parámetros de salida

                objCommand.Parameters.Add("P_RESULTADO", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;

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
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_InsDelito");
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



        public async Task<DtoResultado<string>> P_InsUbicacionIris(DtoUbicacionIris datos, string usuario, string maquina)
        {
            DtoResultado<string> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec); // Usa tu cadena correcta
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_REGISTRO_IRIS.P_InsUbicacion";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                // Parámetros de entrada
                objCommand.Parameters.Add("P_LATITUD", OracleDbType.Varchar2).Value = datos.Latitud ?? "";
                objCommand.Parameters.Add("P_LONGITUD", OracleDbType.Varchar2).Value = datos.Longitud ?? "";
                objCommand.Parameters.Add("P_MUNICIPIO", OracleDbType.Varchar2).Value = datos.MunicipioUbica ?? "";
                objCommand.Parameters.Add("P_BARRIO", OracleDbType.Varchar2).Value = datos.Barrio ?? "";
                objCommand.Parameters.Add("P_CUADRANTE", OracleDbType.Varchar2).Value = datos.CuadranteUbica ?? "";
                objCommand.Parameters.Add("P_RADIO_ACCION", OracleDbType.Varchar2).Value = datos.RadioAccion ?? "";

                objCommand.Parameters.Add("P_IDENTIFICACION_CREACION", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA_CREACION", OracleDbType.Varchar2).Value = maquina ?? "";

                objCommand.Parameters.Add("P_DIRECCION", OracleDbType.NVarchar2).Value = datos.Direccion ?? "";
               
                objCommand.Parameters.Add("P_CODIGO_DANE", OracleDbType.Int64).Value = datos.CodigoDane;
                objCommand.Parameters.Add("P_CODIGO_ESTACION", OracleDbType.Int64).Value = datos.CodigoEstacion;
                objCommand.Parameters.Add("P_CODIGO_SIEDCO_CUADRANTE", OracleDbType.Int64).Value = datos.CodigoSiedcoCuadrante;
                objCommand.Parameters.Add("P_ID_CRIMINALIDAD", OracleDbType.Int64).Value = datos.IdCriminalidad;
                objCommand.Parameters.Add("P_CRIMINALIDAD_ID", OracleDbType.Varchar2).Value = datos.CriminalidadId ?? "";

               

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
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_InsUbicacion");
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


        #endregion


        #region Métodos de Actualización
        public async Task<DtoResultado<string>> P_UpdCriminalidad(DtoIrispCriminalidad datos, string usuario, string maquina)
        {
            DtoResultado<string> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_REGISTRO_IRIS.P_UpdCriminalidad";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                // Parámetros de entrada
                objCommand.Parameters.Add("P_CRIMINALIDAD_ID", OracleDbType.Varchar2).Value = datos.CriminalidadId;
                objCommand.Parameters.Add("P_ID_CLASE", OracleDbType.Int32).Value = datos.IdClase;
                objCommand.Parameters.Add("P_NOMBRE_CLASE", OracleDbType.NVarchar2).Value = datos.NombreClase;
                objCommand.Parameters.Add("P_CANTIDAD_INTEGRANTE", OracleDbType.Int32).Value = datos.CantidadIntegrantes;
                objCommand.Parameters.Add("P_CARACTERISTICAS_GENERALES", OracleDbType.NVarchar2).Value = datos.CaracteristicasGenerales;
                objCommand.Parameters.Add("P_IDENTIFICACION_MODIFICA", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA_MODIFICA", OracleDbType.Varchar2).Value = maquina;
                objCommand.Parameters.Add("P_ID_FUENTE", OracleDbType.Int32).Value = datos.IdFuente;

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
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_UpdCriminalidad");
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

        public async Task<DtoResultado<string>> P_UpdEstadoCriminalidad(DtoIrispCriminalidad datos, string usuario, string maquina)
        {
            DtoResultado<string> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_REGISTRO_IRIS.P_UpdEstadoCriminalidad";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                // Parámetros de entrada
                objCommand.Parameters.Add("P_CRIMINALIDAD_ID", OracleDbType.Varchar2).Value = datos.CriminalidadId;
                objCommand.Parameters.Add("P_ID_ESTADO", OracleDbType.Int32).Value = datos.IdEstado;
                objCommand.Parameters.Add("P_IDENTIFICACION_MODIFICA", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA_MODIFICA", OracleDbType.Varchar2).Value = maquina;

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
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_UpdCriminalidad");
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


        public async Task<DtoResultado<string>> P_UpdExistenciaCriminalidad(DtoIrispCriminalidad datos, string usuario, string maquina)
        {
            DtoResultado<string> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_REGISTRO_IRIS.P_UpdExistenciaCriminalidad";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                // Parámetros de entrada
                objCommand.Parameters.Add("P_CRIMINALIDAD_ID", OracleDbType.Varchar2).Value = datos.CriminalidadId;
                objCommand.Parameters.Add("P_ID_ESTADO_EXISTENCIA", OracleDbType.Int32).Value = datos.IdEstadoExistencia;
                objCommand.Parameters.Add("P_IDENTIFICACION_MODIFICA", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA_MODIFICA", OracleDbType.Varchar2).Value = maquina;

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
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_UpdExistenciaCriminalidad");
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


        #endregion

        #region Métodos de Elimináción


        public async Task<DtoResultado<string>> P_DellIris(string CriminalidadId, string usuario, string maquina)
        {
            DtoResultado<string> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_REGISTRO_IRIS.P_DellIris";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                // Parámetros de entrada
                objCommand.Parameters.Add("P_CRIMINALIDAD_ID", OracleDbType.Varchar2).Value = CriminalidadId;
              
                objCommand.Parameters.Add("P_IDENTIFICACION_MODIFICA", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA_MODIFICA", OracleDbType.Varchar2).Value = maquina;

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
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_DellIris");
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

        public async Task<DtoResultado<string>> P_DelIntegranteIris(string IntegranteId, string usuario, string maquina)
        {
            DtoResultado<string> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_REGISTRO_IRIS.P_DelIntegranteIris";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                // Parámetros de entrada
                objCommand.Parameters.Add("P_INTEGRANTE_ID", OracleDbType.Varchar2).Value = IntegranteId;

                objCommand.Parameters.Add("P_IDENTIFICACION_MODIFICA", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA_MODIFICA", OracleDbType.Varchar2).Value = maquina;

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
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_DelIntegranteIris");
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


        public async Task<DtoResultado<string>> P_DelDelitosIris(string DelitoId, string usuario, string maquina)
        {
            DtoResultado<string> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_REGISTRO_IRIS.P_DelDelitosIris";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                // Parámetros de entrada
                objCommand.Parameters.Add("P_DELITO_ID", OracleDbType.Varchar2).Value = DelitoId;

                objCommand.Parameters.Add("P_IDENTIFICACION_MODIFICA", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA_MODIFICA", OracleDbType.Varchar2).Value = maquina;

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
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_DelDelitosIris");
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


        public async Task<DtoResultado<string>> P_DelDelInfoAdicionalIris(string InfoId, string usuario, string maquina)
        {
            DtoResultado<string> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_REGISTRO_IRIS.P_DelDelInfoAdicionalIris";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                // Parámetros de entrada
                objCommand.Parameters.Add("P_INFO_ID", OracleDbType.Varchar2).Value = InfoId;

                objCommand.Parameters.Add("P_IDENTIFICACION_MODIFICA", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA_MODIFICA", OracleDbType.Varchar2).Value = maquina;

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
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_DelDelInfoAdicionalIris");
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




        public async Task<DtoResultado<string>> P_DelUbicacionIris(string UbicacionId, string usuario, string maquina)
        {
            DtoResultado<string> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_REGISTRO_IRIS.P_DelUbicacionIris";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                // Parámetros de entrada
                objCommand.Parameters.Add("P_UBICACION_ID", OracleDbType.Varchar2).Value = UbicacionId;

                objCommand.Parameters.Add("P_IDENTIFICACION_MODIFICA", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA_MODIFICA", OracleDbType.Varchar2).Value = maquina;

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
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_DelUbicacionIris");
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



        public async Task<DtoResultado<string>> P_DelDocumentoIris(string DocumentoId, string usuario, string maquina)
        {
            DtoResultado<string> resp = new();

            using var Conexion = new OracleConnection(_strConexionIris_Disec);
            using var objCommand = new OracleCommand();

            try
            {
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_REGISTRO_IRIS.P_DelDocumentoIris";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();

                // Parámetros de entrada
                objCommand.Parameters.Add("P_DOCUMENTO_ID", OracleDbType.Varchar2).Value = DocumentoId;

                objCommand.Parameters.Add("P_IDENTIFICACION_MODIFICA", OracleDbType.Int64).Value = usuario;
                objCommand.Parameters.Add("P_MAQUINA_MODIFICA", OracleDbType.Varchar2).Value = maquina;

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
                _logger.LogError(e, "Error ejecutando PK_REGISTRO_IRIS.P_DelDocumentoIris");
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



        #endregion
    }
}
