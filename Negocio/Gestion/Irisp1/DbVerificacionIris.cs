using Comun.Areas.Irisp1;
using Comun.General;
using Dapper;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Interfaz.Irisp1;
using Oracle.ManagedDataAccess.Client;

using System.Data;

namespace Negocio.Gestion.Irisp1
{
    public class DbVerificacionIris : IDbVerificacionIris
    {
        #region Propiedades
        private readonly IConfiguration _iConfiguration;
        private readonly string _strConexionIris_Disec;
        private readonly ILogger<DbVerificacionIris> _logger;
        #endregion

        #region Constructor
        // ✅ Logger correcto: ILogger<DbVerificacionIris>
        public DbVerificacionIris(IConfiguration iConfiguration, ILogger<DbVerificacionIris> logger)
        {
            _iConfiguration = iConfiguration;
            _strConexionIris_Disec = _iConfiguration.GetConnectionString("strConexionIris_Disec");
            _logger = logger;
        }
        #endregion

        // ================================================================
        // 1) PK_CONSULTA_IRISP.F_GetAniosIrisP1  (FUNCTION -> RETURN SYS_REFCURSOR)
        // ================================================================
        public async Task<DtoResultado<List<DtoAnio>>> F_GetAniosIrisP1()
        {
            var resp = new DtoResultado<List<DtoAnio>>
            {
                Operacion = "F_GetAniosIrisP1",
                Data = new List<DtoAnio>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                // FUNCTION -> ReturnValue RefCursor
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

                var lista = (await connection.QueryAsync<DtoAnio>(
                    "PK_CONSULTA_IRISP.F_GetAniosIrisP1",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).ToList();

                resp.Data = lista ?? new List<DtoAnio>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta exitosa" : "No se encuentran registros en base de datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = new List<DtoAnio>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                resp.Data = new List<DtoAnio>();
            }

            return resp;
        }

        // ================================================================
        // 2) PK_VERIFICACION_IRIS.F_GetInfoGrillas (FUNCTION con parámetros -> RETURN SYS_REFCURSOR)
        // OJO: es un paquete PL/SQL independiente de PK_CONSULTA_IRISP.F_GetInfoGrillas (usado por DbIrisp1 y
        // DbSeguimientoIris) con el mismo nombre y propósito. Mantener las reglas de filtrado sincronizadas
        // manualmente entre los tres paquetes Oracle hasta que se unifiquen en uno solo.
        // ================================================================
        public async Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetInfoGrillas(int V_Anio, string RolesUsuario, long CodigoUnidad)
        {
            var resp = new DtoResultado<List<DtoIrispCriminalidad>>
            {
                Operacion = "F_GetInfoGrillas",
                Data = new List<DtoIrispCriminalidad>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_Anio", V_Anio, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_Roles", RolesUsuario ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_CodigoUnidad", CodigoUnidad, OracleMappingType.Int64, ParameterDirection.Input);

                // FUNCTION -> ReturnValue RefCursor (lo más correcto)
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

                var lista = (await connection.QueryAsync<DtoIrispCriminalidad>(
                    "PK_VERIFICACION_IRIS.F_GetInfoGrillas",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).ToList();

                resp.Data = lista ?? new List<DtoIrispCriminalidad>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = new List<DtoIrispCriminalidad>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                resp.Data = new List<DtoIrispCriminalidad>();
            }

            return resp;
        }

        // ================================================================
        // 3) PK_VERIFICACION_IRIS.F_GetTareas (FUNCTION -> RETURN SYS_REFCURSOR)
        // ================================================================
        public async Task<DtoResultado<List<DtoTareasIris>>> F_GetTareas(string V_Criminalidad)
        {
            var resp = new DtoResultado<List<DtoTareasIris>>
            {
                Operacion = "F_GetTareas",
                Data = new List<DtoTareasIris>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_Criminalidad_id", V_Criminalidad ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

                var lista = (await connection.QueryAsync<DtoTareasIris>(
                    "PK_VERIFICACION_IRIS.F_GetTareas",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).ToList();





                resp.Data = lista ?? new List<DtoTareasIris>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = new List<DtoTareasIris>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                resp.Data = new List<DtoTareasIris>();
            }

            return resp;
        }

        // ================================================================
        // 4) PK_VERIFICACION_IRIS.F_GetResultados (FUNCTION -> RETURN SYS_REFCURSOR)
        // ================================================================
        public async Task<DtoResultado<List<DtoTareasIris>>> F_GetResultados(string V_Criminalidad)
        {
            var resp = new DtoResultado<List<DtoTareasIris>>
            {
                Operacion = "F_GetResultados",
                Data = new List<DtoTareasIris>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                // OJO: en tu código estaba "P_Ciminalidad_id" (sin 'r'), lo mantengo tal cual por compatibilidad
                p.Add("P_Ciminalidad_id", V_Criminalidad ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

                var lista = (await connection.QueryAsync<DtoTareasIris>(
                    "PK_VERIFICACION_IRIS.F_GetResultados",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).ToList();

                resp.Data = lista ?? new List<DtoTareasIris>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = new List<DtoTareasIris>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                resp.Data = new List<DtoTareasIris>();
            }

            return resp;
        }

        // ================================================================
        // 5) PK_VERIFICACION_IRIS.F_GetResponsablesTareasIris (FUNCTION con CLOB)
        //   
        // ================================================================
        public async Task<DtoResultado<List<DtoTareasIris>>> F_GetResponsablesTareasIris(string V_Criminalidad)
        {
            var resp = new DtoResultado<List<DtoTareasIris>>
            {
                Operacion = "F_GetResponsablesTareasIris",
                Data = new List<DtoTareasIris>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_Criminalidad_id", V_Criminalidad ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);

                // Function -> ReturnValue RefCursor
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

                // ✅ 1) Intento directo con Dapper
                // Si tu columna SEGUIMIENTO es CLOB, usualmente Dapper la trae como string.
                var lista = (await connection.QueryAsync<DtoTareasIris>(
                    "PK_VERIFICACION_IRIS.F_GetResponsablesTareasIris",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).ToList();

                // ✅ 2) Fallback: si por alguna razón llega OracleClob, lo normalizamos (poco común con Dapper)
                foreach (var item in lista)
                {
                    // Si tu DTO ya tiene Seguimiento string, no necesitas nada aquí.
                    // Dejo el bloque vacío a propósito.
                }

                resp.Data = lista ?? new List<DtoTareasIris>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = new List<DtoTareasIris>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                resp.Data = new List<DtoTareasIris>();
            }

            return resp;
        }

        // ================================================================
        // 6) PK_VERIFICACION_IRIS.P_InsResultado  (PROCEDURE con OUT params)
        // ================================================================
        public async Task<DtoResultado<string>> P_InsResultadoIris(DtoIrisResultado datos, string usuario, string maquina)
        {
            var resp = new DtoResultado<string>
            {
                Operacion = "P_InsResultadoIris",
                Data = ""
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();

                p.Add("P_CRIMINALIDAD_ID", datos?.CriminalidadId ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_ID_TIPO", datos?.IdTipo ?? 0, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_NRO_SPOA_SIEDCO", datos?.Numero ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_OBSERVACION", datos?.Observacion ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_FECHA", datos?.Fecha, OracleMappingType.Date, ParameterDirection.Input);

                // Auditoría
                p.Add("P_IDENTIFICACION_CREA", Convert.ToInt64(usuario), OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_MAQUINA_CREACION", maquina ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);

                // Out
                p.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.ExecuteAsync(
                    "PK_VERIFICACION_IRIS.P_InsResultado",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = p.Get<int?>("P_RESULTADO") ?? 0;
                var mensaje = p.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? "OK" : "";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                resp.Data = "";
            }

            return resp;
        }

        // ================================================================
        // 7) PK_VERIFICACION_IRIS.P_InsTareaRespuesta (PROCEDURE con OUT params)
        // ================================================================
        public async Task<DtoResultado<string>> P_InsTareaRespuesta(DtoTareasIris obj, string usuario, string maquina)
        {
            var resp = new DtoResultado<string>
            {
                Operacion = "P_InsTareaRespuesta",
                Data = ""
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();

                p.Add("P_CRIMINALIDAD_ID", obj?.CriminalidadId ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_TAREA_ID", obj?.TareaId ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_ESTADO_EXISTENCIA", obj?.EstadoExistencia ?? 0, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_JUSTIFICACION", obj?.Justificacion ?? "", OracleMappingType.NVarchar2, ParameterDirection.Input);

                // Date nullable
                if (obj?.FechaVerifica.HasValue == true)
                    p.Add("P_FECHA_VERIFICA", obj.FechaVerifica.Value, OracleMappingType.Date, ParameterDirection.Input);
                else
                    p.Add("P_FECHA_VERIFICA", null, OracleMappingType.Date, ParameterDirection.Input);

                // Auditoría (en tu código original estaba int.Parse(usuario); aquí lo manejo como Int64)
                p.Add("P_IDENTIFICACION_MODIFICA", Convert.ToInt64(usuario), OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_MAQUINA_MODIFICA", maquina ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);

                // Out
                p.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.ExecuteAsync(
                    "PK_VERIFICACION_IRIS.P_InsTareaRespuesta",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = p.Get<int?>("P_RESULTADO") ?? 0;
                var mensaje = p.Get<string>("SRV_Message") ?? "";

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? "OK" : "";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                resp.Data = "";
            }

            return resp;
        }
    }
}
