
using Comun.Areas.Integrantes;
using Comun.General;
using Dapper;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

using System.Data;

using Negocio.Interfaz.Integrantes;

namespace Negocio.Gestion.Integrantes
{
    public class DbRegistroInteg : IDbRegistroInteg
    {
        private readonly IConfiguration _iConfiguration;
        private readonly string _strConexionIris_Disec;
        private readonly ILogger<DbRegistroInteg> _logger;

        public DbRegistroInteg(IConfiguration iConfiguration, ILogger<DbRegistroInteg> logger)
        {
            _iConfiguration = iConfiguration;
            _strConexionIris_Disec = _iConfiguration.GetConnectionString("strConexionIris_Disec");
            _logger = logger;
        }

        // ================================================================
        // F_GetReincidentes
        // ================================================================
        public async Task<DtoResultado<List<DtoReincidentes>>> F_GetReincidentes()
        {
            var resp = new DtoResultado<List<DtoReincidentes>>
            {
                Operacion = "F_GetReincidentes",
                Data = new List<DtoReincidentes>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);


                var p = new OracleDynamicParameters();
                p.Add("RESULT", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                var sql = @"BEGIN 
                                :RESULT := PK_INTEGRANTES_IRIS.F_GetReincidentes; 
                            END;";

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoReincidentes>(
                    sql,
                    p,
                    commandType: CommandType.Text,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoReincidentes>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta exitosa" : "No se encontraron registros";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = new List<DtoReincidentes>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = "Error consultando reincidentes: " + ex.Message;
                resp.Data = new List<DtoReincidentes>();
            }

            return resp;
        }

        // ================================================================
        // F_GetReincidentesPorId
        // ================================================================
        public async Task<DtoResultado<List<DtoReincidentes>>> F_GetReincidentesPorId(long V_Identificacion)
        {
            var resp = new DtoResultado<List<DtoReincidentes>>
            {
                Operacion = "F_GetReincidentesPorId",
                Data = new List<DtoReincidentes>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                // En tu código viejo lo mandabas Varchar2. Si en BD es number, cambia a Int64.
                p.Add("P_Identificacion", V_Identificacion, OracleMappingType.Int64, ParameterDirection.Input);

                // Cursor salida (si tu función exige ReturnValue, dime y lo cambiamos a ReturnValue)
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoReincidentes>(
                    "PK_INTEGRANTES_IRIS.F_GetReincidentesPorId",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoReincidentes>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | Identificacion={Identificacion}",
                    resp.Operacion, V_Identificacion);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = new List<DtoReincidentes>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | Identificacion={Identificacion}",
                    resp.Operacion, V_Identificacion);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                resp.Data = new List<DtoReincidentes>();
            }

            return resp;
        }

        // ================================================================
        // P_InsOrUpdReincidente
        // ================================================================
        public async Task<DtoResultado<string>> P_InsOrUpdReincidente(DtoReincidentes Obj_Reincidente, string usuario, string maquina)
        {
            var resp = new DtoResultado<string>
            {
                Operacion = "P_InsOrUpdReincidente",
                Data = string.Empty
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();

                p.Add("P_IDENTIFICACION", Obj_Reincidente.Identificacion, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_ALIAS", Obj_Reincidente.Alias, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_NOMBRE", Obj_Reincidente.Nombre, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_APELLIDO", Obj_Reincidente.Apellido, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_OBSERVACION", Obj_Reincidente.Observacion, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_ID_TIPO", Obj_Reincidente.IdTipo, OracleMappingType.Varchar2, ParameterDirection.Input);

                p.Add("P_USUARIO", usuario, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_MAQUINA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                p.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_INTEGRANTES_IRIS.P_InsOrUpdReincidente",
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
                _logger.LogError(oex,
                    "OracleException en {Operacion} | Identificacion={Identificacion}",
                    resp.Operacion, Obj_Reincidente?.Identificacion);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | Identificacion={Identificacion}",
                    resp.Operacion, Obj_Reincidente?.Identificacion);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = "";
            }

            return resp;
        }

        // ================================================================
        // P_UpdReincidente
        // ================================================================
        public async Task<DtoResultado<string>> P_UpdReincidente(DtoReincidentes Obj_Reincidente, string usuario, string maquina)
        {
            var resp = new DtoResultado<string>
            {
                Operacion = "P_UpdReincidente",
                Data = string.Empty
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();

                p.Add("P_REINCIDENTE_ID", Obj_Reincidente.ReincidenteId, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_ALIAS", Obj_Reincidente.Alias, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_OBSERVACION", Obj_Reincidente.Observacion, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_ID_TIPO", Obj_Reincidente.IdTipo, OracleMappingType.Varchar2, ParameterDirection.Input);

                p.Add("P_USUARIO", usuario, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_MAQUINA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                p.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_INTEGRANTES_IRIS.P_UpdReincidente",
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
                _logger.LogError(oex,
                    "OracleException en {Operacion} | ReincidenteId={ReincidenteId}",
                    resp.Operacion, Obj_Reincidente?.ReincidenteId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | ReincidenteId={ReincidenteId}",
                    resp.Operacion, Obj_Reincidente?.ReincidenteId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = "";
            }

            return resp;
        }

        // ================================================================
        // P_DellReincidente
        // ================================================================
        public async Task<DtoResultado<string>> P_DellReincidente(DtoReincidentes Obj_Reincidente, string usuario, string maquina)
        {
            var resp = new DtoResultado<string>
            {
                Operacion = "P_DellReincidente",
                Data = string.Empty
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();

                p.Add("P_REINCIDENTE_ID", Obj_Reincidente.ReincidenteId, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_USUARIO", usuario, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_MAQUINA", maquina, OracleMappingType.Varchar2, ParameterDirection.Input);

                p.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 500);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_INTEGRANTES_IRIS.P_DellReincidente",
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
                _logger.LogError(oex,
                    "OracleException en {Operacion} | ReincidenteId={ReincidenteId}",
                    resp.Operacion, Obj_Reincidente?.ReincidenteId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | ReincidenteId={ReincidenteId}",
                    resp.Operacion, Obj_Reincidente?.ReincidenteId);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"Error: {ex.Message}";
                resp.Data = "";
            }

            return resp;
        }
    }
}
