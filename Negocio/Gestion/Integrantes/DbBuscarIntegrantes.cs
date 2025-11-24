using Comun.Areas.Integrantes;
using Comun.General;
using Dapper;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Interfaz.Integrantes;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Negocio.Gestion.Integrantes
{
    public class DbBuscarIntegrantes : IDbBuscarIntegrantes
    {
        private readonly IConfiguration _iConfiguration;
        private readonly string _strConexionIris_Disec;
        private readonly ILogger _logger;

        public DbBuscarIntegrantes(IConfiguration iConfiguration, ILogger<IDbRegistroInteg> logger)
        {
            _iConfiguration = iConfiguration;
            _strConexionIris_Disec = _iConfiguration.GetConnectionString("strConexionIris_Disec");
            _logger = logger;
        }

        // ================================================================
        // 1. CONSULTA BÁSICA DE INTEGRANTES (RETORNA 1 REGISTRO)
        // ================================================================
        public async Task<DtoResultado<List<DtoDatosBasicos>>> F_GetIntegrantesPorId(long identificacion)
        {
            var resultado = new DtoResultado<List<DtoDatosBasicos>>();

            try
            {
                using var conexion = new OracleConnection(_strConexionIris_Disec);

                var parametros = new OracleDynamicParameters();
                parametros.Add("P_IDENTIFICACION", identificacion, OracleMappingType.Decimal, ParameterDirection.Input);
                parametros.Add("V_CONSULTA", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

                var lista = (await conexion.QueryAsync<DtoDatosBasicos>(
                    "PK_INTEGRANTES_IRIS.F_GetIntegrantesPorId",
                    parametros,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                resultado.IdRespuesta = 1;
                resultado.Data = lista;
                resultado.Mensaje = "Consulta realizada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en F_GetIntegrantesPorId");
                resultado.IdRespuesta = 0;
                resultado.Mensaje = "No es posible completar la consulta";
            }

            return resultado;
        }

        // ================================================================
        // 2. CONSULTAR LISTA IRIS GENERAL
        // ================================================================
        public async Task<DtoResultado<List<DtoListaIrisGeneral>>> F_GetListaIris(long identificacion)
        {
            var resp = new DtoResultado<List<DtoListaIrisGeneral>>();

            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);

                var parameters = new OracleDynamicParameters();
                parameters.Add("P_Identificacion", identificacion, OracleMappingType.Int64, ParameterDirection.Input);
                parameters.Add("V_CONSULTA", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

                var lista = (await connection.QueryAsync<DtoListaIrisGeneral>(
                    "PK_INTEGRANTES_IRIS.F_GetListaIris",
                    parameters,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                resp.Data = lista;
                resp.IdRespuesta = lista.Count > 0 ? 1 : 0;
                resp.Mensaje = lista.Count > 0 ? "Consulta exitosa" : "No se encontraron datos";
                resp.Operacion = "F_GetListaIris";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando F_GetListaIris");

                resp.IdRespuesta = 0;
                resp.Mensaje = ex.Message;
                resp.Data = new List<DtoListaIrisGeneral>();
                resp.Operacion = "ERROR";
            }

            return resp;
        }

        // ================================================================
        // 3. CONSULTAR LOGS — ESTÁNDAR CORRECTO
        // ================================================================
        public async Task<DtoResultado<List<DtoAntecedentesLogs>>> F_GetLogPorIdentificacion(long identificacion)
        {
            var resp = new DtoResultado<List<DtoAntecedentesLogs>>();

            try
            {
                using var cnn = new OracleConnection(_strConexionIris_Disec);

                var p = new OracleDynamicParameters();
                p.Add("P_Identificacion", identificacion, OracleMappingType.Decimal, ParameterDirection.Input);
                p.Add("V_Consulta", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);

                var lista = (await cnn.QueryAsync<DtoAntecedentesLogs>(
                    "PK_INTEGRANTES_IRIS.F_GetLogPorIdentificacion",
                    p,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                resp.IdRespuesta = 1;
                resp.Data = lista;
            }
            catch (Exception ex)
            {
                resp.IdRespuesta = 0;
                resp.Mensaje = "Error consultando el log: " + ex.Message;
            }

            return resp;
        }

    }
}
