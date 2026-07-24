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

        public async Task<DtoResultado<List<DtoTareaControlGestion>>> F_GetTareasControlGestion(int anio, string rolesUsuario, long codigoUnidad)
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
                p.Add("P_Anio", anio, OracleMappingType.Int32, ParameterDirection.Input);
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
                _logger.LogError(oex, "OracleException en {Operacion} | Anio={Anio} | RolesUsuario={RolesUsuario} | CodigoUnidad={CodigoUnidad}",
                    resp.Operacion, anio, rolesUsuario, codigoUnidad);
                resp.IdRespuesta = 0;
                resp.Mensaje = "Error al consultar el tablero de control de gestión.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | Anio={Anio} | RolesUsuario={RolesUsuario} | CodigoUnidad={CodigoUnidad}",
                    resp.Operacion, anio, rolesUsuario, codigoUnidad);
                resp.IdRespuesta = 0;
                resp.Mensaje = "Error al consultar el tablero de control de gestión.";
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
