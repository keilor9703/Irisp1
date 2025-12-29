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
    public class DbBuscarIntegrantes : IDbBuscarIntegrantes
    {
        private readonly IConfiguration _iConfiguration;
        private readonly string _strConexionIris_Disec;
        private readonly ILogger<DbBuscarIntegrantes> _logger;

        public DbBuscarIntegrantes(IConfiguration iConfiguration, ILogger<DbBuscarIntegrantes> logger)
        {
            _iConfiguration = iConfiguration;
            _strConexionIris_Disec = _iConfiguration.GetConnectionString("strConexionIris_Disec");
            _logger = logger;
        }

        // ================================================================
        // 1. CONSULTA BÁSICA DE INTEGRANTES (RETORNA LISTA)
        // ================================================================
        public async Task<DtoResultado<List<DtoDatosBasicos>>> F_GetIntegrantesPorId(long identificacion)
        {
            var resp = new DtoResultado<List<DtoDatosBasicos>>
            {
                Operacion = "F_GetIntegrantesPorId",
                Data = new List<DtoDatosBasicos>()
            };

            try
            {
                using var cnn = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_IDENTIFICACION", identificacion, OracleMappingType.Int64, ParameterDirection.Input);

                // Cursor de salida (más estable en Dapper.Oracle)
                p.Add("V_CONSULTA", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                await cnn.OpenAsync();

                var lista = (await cnn.QueryAsync<DtoDatosBasicos>(
                    "PK_INTEGRANTES_IRIS.F_GetIntegrantesPorId",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoDatosBasicos>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta realizada correctamente" : "No se encontraron datos";
                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | Identificacion={Identificacion}",
                    resp.Operacion, identificacion);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = new List<DtoDatosBasicos>();
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | Identificacion={Identificacion}",
                    resp.Operacion, identificacion);

                resp.IdRespuesta = 0;
                resp.Mensaje = "No es posible completar la consulta";
                resp.Data = new List<DtoDatosBasicos>();
                return resp;
            }
        }

        // ================================================================
        // 2. CONSULTAR LISTA IRIS GENERAL
        // ================================================================
        public async Task<DtoResultado<List<DtoListaIrisGeneral>>> F_GetListaIris(long identificacion)
        {
            var resp = new DtoResultado<List<DtoListaIrisGeneral>>
            {
                Operacion = "F_GetListaIris",
                Data = new List<DtoListaIrisGeneral>()
            };

            try
            {
                using var cnn = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_Identificacion", identificacion, OracleMappingType.Int64, ParameterDirection.Input);

                // Cursor salida
                p.Add("V_CONSULTA", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                await cnn.OpenAsync();

                var lista = (await cnn.QueryAsync<DtoListaIrisGeneral>(
                    "PK_INTEGRANTES_IRIS.F_GetListaIris",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoListaIrisGeneral>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";
                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | Identificacion={Identificacion}",
                    resp.Operacion, identificacion);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = new List<DtoListaIrisGeneral>();
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | Identificacion={Identificacion}",
                    resp.Operacion, identificacion);

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                resp.Data = new List<DtoListaIrisGeneral>();
                return resp;
            }
        }

        // ================================================================
        // 3. CONSULTAR LOGS (ANTECEDENTES)
        // ================================================================
        public async Task<DtoResultado<List<DtoAntecedentesLogs>>> F_GetLogPorIdentificacion(long identificacion)
        {
            var resp = new DtoResultado<List<DtoAntecedentesLogs>>
            {
                Operacion = "F_GetLogPorIdentificacion",
                Data = new List<DtoAntecedentesLogs>()
            };

            try
            {
                using var cnn = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_Identificacion", identificacion, OracleMappingType.Int64, ParameterDirection.Input);

                // Cursor salida
                p.Add("V_Consulta", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                await cnn.OpenAsync();

                var lista = (await cnn.QueryAsync<DtoAntecedentesLogs>(
                    "PK_INTEGRANTES_IRIS.F_GetLogPorIdentificacion",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                )).AsList();

                resp.Data = lista ?? new List<DtoAntecedentesLogs>();
                resp.IdRespuesta = 1;
                resp.Mensaje = "Consulta exitosa";
                return resp;
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex,
                    "OracleException en {Operacion} | Identificacion={Identificacion}",
                    resp.Operacion, identificacion);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"OracleException: {oex.Message}";
                resp.Data = new List<DtoAntecedentesLogs>();
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error Dapper en {Operacion} | Identificacion={Identificacion}",
                    resp.Operacion, identificacion);

                resp.IdRespuesta = 0;
                resp.Mensaje = "Error consultando el log: " + ex.Message;
                resp.Data = new List<DtoAntecedentesLogs>();
                return resp;
            }
        }
    }
}
