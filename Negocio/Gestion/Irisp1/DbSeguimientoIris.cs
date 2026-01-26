using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Comun.General;
using Dapper;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Interfaz.Irisp1;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Negocio.Gestion.Irisp1
{
    public class DbSeguimientoIris : IDbSeguimientoIris
    {
        #region Propiedades
        private readonly IConfiguration _iConfiguration;
        private readonly string _strConexionIris_Disec;
        private readonly ILogger<DbSeguimientoIris> _logger;
        #endregion

        #region Constructor
        public DbSeguimientoIris(IConfiguration iConfiguration, ILogger<DbSeguimientoIris> logger)
        {
            _iConfiguration = iConfiguration;
            _strConexionIris_Disec = _iConfiguration.GetConnectionString("strConexionIris_Disec");
            _logger = logger;
        }
        #endregion

        #region Métodos de Consulta

        // ================================================================
        // F_GetAniosIrisP1
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
                p.Add("RESULT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                var sql = @"BEGIN 
                                :RESULT := PK_CONSULTA_IRISP.F_GetAniosIrisP1; 
                            END;";

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoAnio>(
                    sql,
                    p,
                    commandType: CommandType.Text,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoAnio>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encuentran registros en base de datos";
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
                _logger.LogError(ex, "Error Dapper en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                resp.Data = new List<DtoAnio>();
            }

            return resp;
        }

        // ================================================================
        // F_GetInfoGrillas
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
                p.Add("P_Roles", RolesUsuario, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_CodigoUnidad", CodigoUnidad, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("RESULT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                var sql = @"
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
                    p,
                    commandType: CommandType.Text,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoIrispCriminalidad>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | Anio={Anio} Roles={Roles} Unidad={Unidad}",
                    resp.Operacion, V_Anio, RolesUsuario, CodigoUnidad);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = new List<DtoIrispCriminalidad>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | Anio={Anio} Roles={Roles} Unidad={Unidad}",
                    resp.Operacion, V_Anio, RolesUsuario, CodigoUnidad);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                resp.Data = new List<DtoIrispCriminalidad>();
            }

            return resp;
        }

        // ================================================================
        // F_GetResponsablesTareasIris  (manejo CLOB)
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
                p.Add("P_Criminalidad_id", V_Criminalidad, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

              

                await connection.OpenAsync();

                
                var lista = (await connection.QueryAsync<DtoTareasIris>(
                    "PK_SEGUIMIENTO_IRIS.F_GetResponsablesTareasIris",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoTareasIris>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | Criminalidad={Criminalidad}",
                    resp.Operacion, V_Criminalidad);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = new List<DtoTareasIris>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | Criminalidad={Criminalidad}",
                    resp.Operacion, V_Criminalidad);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                resp.Data = new List<DtoTareasIris>();
            }

            return resp;
        }

        // ================================================================
        // F_GetResponsables 
        // ================================================================
        public async Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetResponsables(string V_CriminalidadId)
        {
            var resp = new DtoResultado<List<DtoIrispCriminalidad>>
            {
                Operacion = "F_GetResponsables",
                Data = new List<DtoIrispCriminalidad>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_Criminalidad_Id", V_CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

           

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoIrispCriminalidad>(
                    "PK_SEGUIMIENTO_IRIS.F_GetResponsables",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoIrispCriminalidad>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | Criminalidad={Criminalidad}",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = new List<DtoIrispCriminalidad>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | Criminalidad={Criminalidad}",
                    resp.Operacion, V_CriminalidadId);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                resp.Data = new List<DtoIrispCriminalidad>();
            }

            return resp;
        }

        // ================================================================
        // F_GetUnidadesSeguimiento
        // ================================================================
        public async Task<DtoResultado<List<DtoDominios>>> F_GetUnidadesSeguimiento()
        {
            var resp = new DtoResultado<List<DtoDominios>>
            {
                Operacion = "F_GetUnidadesSeguimiento",
                Data = new List<DtoDominios>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();

                // 👇 IMPORTANTE: una FUNCTION se lee por RETURN VALUE (ReturnValue)
                // El nombre puede ser "RETURN_VALUE" o el que tú quieras,
                // lo clave es direction: ReturnValue
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

                var lista = (await connection.QueryAsync<DtoDominios>(
                    "PK_SEGUIMIENTO_IRIS.f_GetUnidadesSeguimiento",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).ToList();

                resp.Data = lista ?? new List<DtoDominios>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = new List<DtoDominios>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                resp.Data = new List<DtoDominios>();
            }

            return resp;
        }


        // ================================================================
        // F_GetUnidadesPorSigla
        // ================================================================
        public async Task<DtoResultado<List<DtoDominios>>> F_GetUnidadesPorSigla(string V_Sigla)
        {
            var resp = new DtoResultado<List<DtoDominios>>
            {
                Operacion = "F_GetUnidadesPorSigla",
                Data = new List<DtoDominios>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("p_sigla", V_Sigla, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

                

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoDominios>(
                    "PK_SEGUIMIENTO_IRIS.F_GetUnidadesPorSigla",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoDominios>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | Sigla={Sigla}",
                    resp.Operacion, V_Sigla);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = new List<DtoDominios>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | Sigla={Sigla}",
                    resp.Operacion, V_Sigla);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                resp.Data = new List<DtoDominios>();
            }

            return resp;
        }

        #endregion

        #region Métodos de Actualización / Inserción / Eliminación

        // ================================================================
        // P_InsResponsable
        // ================================================================
        public async Task<DtoResultado<int>> P_InsResponsable(DtoIrispCriminalidad Obj_Responsable, string usuario, string maquina)
        {
            var resp = new DtoResultado<int>
            {
                Operacion = "P_InsResponsable",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_CRIMINALIDAD_ID", Obj_Responsable.CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_ID_UNIDAD", Obj_Responsable.IdUnidad ?? 0, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_TAREA", Obj_Responsable.IdTareai ?? 1, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_OBSERVACION", Obj_Responsable.Observacion, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_USUARIO", usuario, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_MAQUINA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                p.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_SEGUIMIENTO_IRIS.P_InsResponsable",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = p.Get<int>("P_RESULTADO");
                var mensaje = p.Get<string>("SRV_Message") ?? string.Empty;

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | Criminalidad={Criminalidad}", resp.Operacion, Obj_Responsable?.CriminalidadId);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | Criminalidad={Criminalidad}", resp.Operacion, Obj_Responsable?.CriminalidadId);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
            }

            return resp;
        }

        // ================================================================
        // P_UpdUnidadResponsable
        // ================================================================
        public async Task<DtoResultado<int>> P_UpdUnidadResponsable(DtoIrispCriminalidad obj_responsableUpd, string usuario, string maquina)
        {
            var resp = new DtoResultado<int>
            {
                Operacion = "P_UpdUnidadResponsable",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_RESPON_VALIDACION_ID", obj_responsableUpd.IdResponsable, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_ID_UNIDAD_NUEVA", obj_responsableUpd.IdUnidad ?? 0, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_IDENTIFICACION_MODIFICA", usuario, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_MAQUINA_MODIFICA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                p.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_SEGUIMIENTO_IRIS.P_UpdUnidadResponsable",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = p.Get<int>("P_RESULTADO");
                var mensaje = p.Get<string>("SRV_Message") ?? string.Empty;

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | ResponId={ResponId}", resp.Operacion, obj_responsableUpd?.IdResponsable);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | ResponId={ResponId}", resp.Operacion, obj_responsableUpd?.IdResponsable);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
            }

            return resp;
        }

        // ================================================================
        // P_FinalizarIris
        // ================================================================
        public async Task<DtoResultado<string>> P_FinalizarIris(DtoIrispCriminalidad datos, string usuario, string maquina)
        {
            var resp = new DtoResultado<string>
            {
                Operacion = "P_FinalizarIris",
                Data = string.Empty
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_CRIMINALIDAD_ID", datos.CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_IDENTIFICACION_MODIFICA", usuario, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_MAQUINA_MODIFICA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_JUSTIFICACION", datos.Justificacion, OracleMappingType.Varchar2, ParameterDirection.Input);

                p.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 4000);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_SEGUIMIENTO_IRIS.P_FinalizarIris",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = p.Get<int>("P_RESULTADO");
                var mensaje = p.Get<string>("SRV_Message") ?? string.Empty;

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? "OK" : "";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | Criminalidad={Criminalidad}", resp.Operacion, datos?.CriminalidadId);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | Criminalidad={Criminalidad}", resp.Operacion, datos?.CriminalidadId);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = "";
            }

            return resp;
        }

        // ================================================================
        // P_DelUnidadResponsable
        // ================================================================
        public async Task<DtoResultado<int>> P_DelUnidadResponsable(DtoIrispCriminalidad obj_responsableUpd, string usuario, string maquina)
        {
            var resp = new DtoResultado<int>
            {
                Operacion = "P_DelUnidadResponsable",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_RESPON_VALIDACION_ID", obj_responsableUpd.IdResponsable, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_ID_UNIDAD", obj_responsableUpd.IdUnidad ?? 0, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_IDENTIFICACION_MODIFICA", usuario, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_MAQUINA_MODIFICA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                p.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_SEGUIMIENTO_IRIS.P_DelUnidadResponsable",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = p.Get<int>("P_RESULTADO");
                var mensaje = p.Get<string>("SRV_Message") ?? string.Empty;

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | ResponId={ResponId}", resp.Operacion, obj_responsableUpd?.IdResponsable);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | ResponId={ResponId}", resp.Operacion, obj_responsableUpd?.IdResponsable);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
            }

            return resp;
        }

        // ================================================================
        // P_EvalTarea
        // ================================================================
        public async Task<DtoResultado<int>> P_EvalTarea(DtoIrispCriminalidad obj_EvalTarea, string usuario, string maquina)
        {
            var resp = new DtoResultado<int>
            {
                Operacion = "P_EvalTarea",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_TAREA_ID", obj_EvalTarea.IdTarea, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_EVAL_TAREA_ID", obj_EvalTarea.IdEstado ?? 0, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_IDENTIFICACION_MODIFICA", usuario, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_MAQUINA_MODIFICA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                p.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_SEGUIMIENTO_IRIS.P_EvalTarea",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = p.Get<int>("P_RESULTADO");
                var mensaje = p.Get<string>("SRV_Message") ?? string.Empty;

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | TareaId={TareaId}", resp.Operacion, obj_EvalTarea?.IdTarea);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | TareaId={TareaId}", resp.Operacion, obj_EvalTarea?.IdTarea);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
            }

            return resp;
        }

        // ================================================================
        // P_ReasignarTarea
        // ================================================================
        public async Task<DtoResultado<int>> P_ReasignarTarea(DtoTareasIris obj_ReasignarTarea, string usuario, string maquina)
        {
            var resp = new DtoResultado<int>
            {
                Operacion = "P_ReasignarTarea",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("p_CRIMINALIDAD_ID", obj_ReasignarTarea.CriminalidadId, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("p_RESPON_VALIDACION_ID", obj_ReasignarTarea.ResponValidacionId, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("p_OBSERVACION", obj_ReasignarTarea.Observacion ?? "", OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("p_IDENTIFICACION_CREACION", usuario, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("p_MAQUINA_CREACION", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                p.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_SEGUIMIENTO_IRIS.P_ReasignarTarea",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = p.Get<int>("P_RESULTADO");
                var mensaje = p.Get<string>("SRV_Message") ?? string.Empty;

                resp.IdRespuesta = resultado > 0 ? 1 : 0;
                resp.Mensaje = mensaje;
                resp.Data = resultado > 0 ? 1 : 0;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | Criminalidad={Criminalidad} Respon={Respon}",
                    resp.Operacion, obj_ReasignarTarea?.CriminalidadId, obj_ReasignarTarea?.ResponValidacionId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | Criminalidad={Criminalidad} Respon={Respon}",
                    resp.Operacion, obj_ReasignarTarea?.CriminalidadId, obj_ReasignarTarea?.ResponValidacionId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = 0;
            }

            return resp;
        }

        #endregion
    }
}
