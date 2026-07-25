using Comun.Areas.Control;
using Comun.General;
using Dapper;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Interfaz.Control;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Negocio.Gestion.Control
{
    public class DbControlGestion : IDbControlGestion
    {
        #region Propiedades
        private readonly IConfiguration _iConfiguration;
        private readonly string _strConexionIris_Disec;
        private readonly ILogger<DbControlGestion> _logger;
        #endregion

        #region Constructor
        public DbControlGestion(IConfiguration iConfiguration, ILogger<DbControlGestion> logger)
        {
            _iConfiguration = iConfiguration;
            _strConexionIris_Disec = _iConfiguration.GetConnectionString("strConexionIris_Disec");
            _logger = logger;
        }
        #endregion

        #region Métodos de Consulta

        public async Task<DtoResultado<List<DtoSlaConfig>>> F_GetSlaConfig()
        {
            var resp = new DtoResultado<List<DtoSlaConfig>>
            {
                Operacion = "F_GetSlaConfig",
                Data = new List<DtoSlaConfig>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoSlaConfig>(
                    "PK_CONTROL_GESTION_IRIS.F_GetSlaConfig",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoSlaConfig>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = "Error al consultar la configuración de SLA.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = "Error al consultar la configuración de SLA.";
            }

            return resp;
        }

        public async Task<DtoResultado<List<DtoTareaControlGestion>>> F_GetTareasControlGestion(DateTime fechaInicio, DateTime fechaFin, string? siglaUnidad, string rolesUsuario, long codigoUnidad)
        {
            var resp = new DtoResultado<List<DtoTareaControlGestion>>
            {
                Operacion = "F_GetTareasControlGestion",
                Data = new List<DtoTareaControlGestion>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);
                p.Add("P_FechaInicio", fechaInicio.Date, OracleMappingType.Date, ParameterDirection.Input);
                p.Add("P_FechaFin", fechaFin.Date, OracleMappingType.Date, ParameterDirection.Input);
                p.Add("P_SiglaUnidad", string.IsNullOrWhiteSpace(siglaUnidad) ? (object)DBNull.Value : siglaUnidad.Trim(), OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_Roles", rolesUsuario ?? string.Empty, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_CodigoUnidad", codigoUnidad, OracleMappingType.Int64, ParameterDirection.Input);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoTareaControlGestion>(
                    "PK_CONTROL_GESTION_IRIS.F_GetTareasControlGestion",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoTareaControlGestion>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | FechaInicio={FechaInicio} | FechaFin={FechaFin} | SiglaUnidad={SiglaUnidad} | RolesUsuario={RolesUsuario} | CodigoUnidad={CodigoUnidad}",
                    resp.Operacion, fechaInicio, fechaFin, siglaUnidad, rolesUsuario, codigoUnidad);
                resp.IdRespuesta = 0;
                resp.Mensaje = "Error al consultar el tablero de control de gestión.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | FechaInicio={FechaInicio} | FechaFin={FechaFin} | SiglaUnidad={SiglaUnidad} | RolesUsuario={RolesUsuario} | CodigoUnidad={CodigoUnidad}",
                    resp.Operacion, fechaInicio, fechaFin, siglaUnidad, rolesUsuario, codigoUnidad);
                resp.IdRespuesta = 0;
                resp.Mensaje = "Error al consultar el tablero de control de gestión.";
            }

            return resp;
        }

        public async Task<DtoResultado<List<DtoCasoControlGestion>>> F_GetCasosControlGestion(DateTime fechaInicio, DateTime fechaFin, string? siglaUnidad, string rolesUsuario, long codigoUnidad)
        {
            var resp = new DtoResultado<List<DtoCasoControlGestion>>
            {
                Operacion = "F_GetCasosControlGestion",
                Data = new List<DtoCasoControlGestion>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);
                p.Add("P_FechaInicio", fechaInicio.Date, OracleMappingType.Date, ParameterDirection.Input);
                p.Add("P_FechaFin", fechaFin.Date, OracleMappingType.Date, ParameterDirection.Input);
                p.Add("P_SiglaUnidad", string.IsNullOrWhiteSpace(siglaUnidad) ? (object)DBNull.Value : siglaUnidad.Trim(), OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_Roles", rolesUsuario ?? string.Empty, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_CodigoUnidad", codigoUnidad, OracleMappingType.Int64, ParameterDirection.Input);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoCasoControlGestion>(
                    "PK_CONTROL_GESTION_IRIS.F_GetCasosControlGestion",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoCasoControlGestion>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | FechaInicio={FechaInicio} | FechaFin={FechaFin} | SiglaUnidad={SiglaUnidad} | RolesUsuario={RolesUsuario} | CodigoUnidad={CodigoUnidad}",
                    resp.Operacion, fechaInicio, fechaFin, siglaUnidad, rolesUsuario, codigoUnidad);
                resp.IdRespuesta = 0;
                resp.Mensaje = "Error al consultar los tiempos de gestión por caso.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | FechaInicio={FechaInicio} | FechaFin={FechaFin} | SiglaUnidad={SiglaUnidad} | RolesUsuario={RolesUsuario} | CodigoUnidad={CodigoUnidad}",
                    resp.Operacion, fechaInicio, fechaFin, siglaUnidad, rolesUsuario, codigoUnidad);
                resp.IdRespuesta = 0;
                resp.Mensaje = "Error al consultar los tiempos de gestión por caso.";
            }

            return resp;
        }

        public async Task<DtoResultado<List<DtoSiglaUnidad>>> F_GetSiglasUnidadIrisp1()
        {
            var resp = new DtoResultado<List<DtoSiglaUnidad>>
            {
                Operacion = "F_GetSiglasUnidadIrisp1",
                Data = new List<DtoSiglaUnidad>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoSiglaUnidad>(
                    "PK_CONTROL_GESTION_IRIS.F_GetSiglasUnidadIrisp1",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoSiglaUnidad>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = "Error al consultar las siglas de unidad.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = "Error al consultar las siglas de unidad.";
            }

            return resp;
        }

        public async Task<DtoResultado<List<DtoMapaIrisp1>>> F_GetMapaIrisp1(DateTime fechaInicio, DateTime fechaFin, string? siglaUnidad, string rolesUsuario, long codigoUnidad)
        {
            var resp = new DtoResultado<List<DtoMapaIrisp1>>
            {
                Operacion = "F_GetMapaIrisp1",
                Data = new List<DtoMapaIrisp1>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);
                p.Add("P_FechaInicio", fechaInicio.Date, OracleMappingType.Date, ParameterDirection.Input);
                p.Add("P_FechaFin", fechaFin.Date, OracleMappingType.Date, ParameterDirection.Input);
                p.Add("P_SiglaUnidad", string.IsNullOrWhiteSpace(siglaUnidad) ? (object)DBNull.Value : siglaUnidad.Trim(), OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_Roles", rolesUsuario ?? string.Empty, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_CodigoUnidad", codigoUnidad, OracleMappingType.Int64, ParameterDirection.Input);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoMapaIrisp1>(
                    "PK_CONTROL_GESTION_IRIS.F_GetMapaIrisp1",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoMapaIrisp1>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | FechaInicio={FechaInicio} | FechaFin={FechaFin} | SiglaUnidad={SiglaUnidad}",
                    resp.Operacion, fechaInicio, fechaFin, siglaUnidad);
                resp.IdRespuesta = 0;
                resp.Mensaje = "Error al consultar los puntos del mapa.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | FechaInicio={FechaInicio} | FechaFin={FechaFin} | SiglaUnidad={SiglaUnidad}",
                    resp.Operacion, fechaInicio, fechaFin, siglaUnidad);
                resp.IdRespuesta = 0;
                resp.Mensaje = "Error al consultar los puntos del mapa.";
            }

            return resp;
        }

        public async Task<DtoResultado<List<DtoResultadoCasoIrisp1>>> F_GetResultadosCasosIrisp1(DateTime fechaInicio, DateTime fechaFin, string? siglaUnidad, string rolesUsuario, long codigoUnidad)
        {
            var resp = new DtoResultado<List<DtoResultadoCasoIrisp1>>
            {
                Operacion = "F_GetResultadosCasosIrisp1",
                Data = new List<DtoResultadoCasoIrisp1>()
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);
                p.Add("P_FechaInicio", fechaInicio.Date, OracleMappingType.Date, ParameterDirection.Input);
                p.Add("P_FechaFin", fechaFin.Date, OracleMappingType.Date, ParameterDirection.Input);
                p.Add("P_SiglaUnidad", string.IsNullOrWhiteSpace(siglaUnidad) ? (object)DBNull.Value : siglaUnidad.Trim(), OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_Roles", rolesUsuario ?? string.Empty, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_CodigoUnidad", codigoUnidad, OracleMappingType.Int64, ParameterDirection.Input);

                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<DtoResultadoCasoIrisp1>(
                    "PK_CONTROL_GESTION_IRIS.F_GetResultadosCasosIrisp1",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoResultadoCasoIrisp1>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | FechaInicio={FechaInicio} | FechaFin={FechaFin} | SiglaUnidad={SiglaUnidad} | RolesUsuario={RolesUsuario} | CodigoUnidad={CodigoUnidad}",
                    resp.Operacion, fechaInicio, fechaFin, siglaUnidad, rolesUsuario, codigoUnidad);
                resp.IdRespuesta = 0;
                resp.Mensaje = "Error al consultar los resultados de los casos.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | FechaInicio={FechaInicio} | FechaFin={FechaFin} | SiglaUnidad={SiglaUnidad} | RolesUsuario={RolesUsuario} | CodigoUnidad={CodigoUnidad}",
                    resp.Operacion, fechaInicio, fechaFin, siglaUnidad, rolesUsuario, codigoUnidad);
                resp.IdRespuesta = 0;
                resp.Mensaje = "Error al consultar los resultados de los casos.";
            }

            return resp;
        }

        #endregion

        #region Métodos de Inserción/Actualización/Eliminación

        public async Task<DtoResultado<int>> P_InsUpdSlaConfig(int idListaTareas, int tiempoAlertaHoras, int tiempoMaximoHoras,
            string? justificacion, long usuario, string maquina)
        {
            var resp = new DtoResultado<int>
            {
                Operacion = "P_InsUpdSlaConfig",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_IdListaTareas", idListaTareas, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_TiempoAlertaHoras", tiempoAlertaHoras, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_TiempoMaximoHoras", tiempoMaximoHoras, OracleMappingType.Int32, ParameterDirection.Input);
                p.Add("P_Justificacion", (justificacion ?? string.Empty).Trim(), OracleMappingType.Varchar2, ParameterDirection.Input, size: 2000);
                p.Add("P_Usuario", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_Maquina", (maquina ?? string.Empty).Trim(), OracleMappingType.Varchar2, ParameterDirection.Input, size: 200);

                p.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 2000);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_CONTROL_GESTION_IRIS.P_InsUpdSlaConfig",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = p.Get<int?>("P_RESULTADO") ?? 0;
                var mensajeSrv = p.Get<string>("SRV_Message");

                resp.IdRespuesta = resultado;
                resp.Data = resultado;
                resp.Mensaje = !string.IsNullOrWhiteSpace(mensajeSrv)
                    ? mensajeSrv
                    : (resultado == 1 ? "Registro grabado exitosamente" : "No fue posible registrar la configuración de SLA.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error en {Operacion} | IdListaTareas={IdListaTareas} | Usuario={Usuario}",
                    resp.Operacion, idListaTareas, usuario);

                resp.IdRespuesta = 0;
                resp.Data = 0;
                resp.Mensaje = "No fue posible guardar, intente nuevamente.";
            }

            return resp;
        }

        public async Task<DtoResultado<int>> P_DelSlaConfig(string slaConfigId, long usuario, string maquina)
        {
            var resp = new DtoResultado<int>
            {
                Operacion = "P_DelSlaConfig",
                Data = 0
            };

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_SlaConfigId", slaConfigId, OracleMappingType.Varchar2, ParameterDirection.Input);
                p.Add("P_Usuario", usuario, OracleMappingType.Int64, ParameterDirection.Input);
                p.Add("P_Maquina", (maquina ?? string.Empty).Trim(), OracleMappingType.Varchar2, ParameterDirection.Input, size: 200);

                p.Add("P_RESULTADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
                p.Add("SRV_Message", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 2000);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    "PK_CONTROL_GESTION_IRIS.P_DelSlaConfig",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var resultado = p.Get<int?>("P_RESULTADO") ?? 0;
                var mensajeSrv = p.Get<string>("SRV_Message");

                resp.IdRespuesta = resultado;
                resp.Data = resultado;
                resp.Mensaje = !string.IsNullOrWhiteSpace(mensajeSrv)
                    ? mensajeSrv
                    : (resultado == 1 ? "Registro Eliminado correctamente" : "No se encontró el registro especificado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en {Operacion} | SlaConfigId={SlaConfigId} | Usuario={Usuario}",
                    resp.Operacion, slaConfigId, usuario);

                resp.IdRespuesta = 0;
                resp.Data = 0;
                resp.Mensaje = "No fue posible eliminar, intente nuevamente.";
            }

            return resp;
        }

        #endregion
    }
}
