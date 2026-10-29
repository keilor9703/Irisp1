using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Comun.Areas.Admin;
using Comun.General;
using Dapper;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Interfaz.Admin;
using Oracle.ManagedDataAccess.Client;

namespace Negocio.Gestion.Admin
{
    public class DbAuditoriaUso : IDbAuditoriaUso
    {
        private readonly string _strConexionIris_Disec;
        private readonly ILogger<DbAuditoriaUso> _logger;

        public DbAuditoriaUso(IConfiguration iConfiguration, ILogger<DbAuditoriaUso> logger)
        {
            _logger = logger;
            _strConexionIris_Disec = iConfiguration.GetConnectionString("strConexionIris_Disec");
        }

        // Parámetros comunes a todas las funciones de PK_AUDITORIA_USO.
        private static OracleDynamicParameters ParametrosBase(DateTime fechaIni, DateTime fechaFin, string? sigla, long? usuario)
        {
            var p = new OracleDynamicParameters();
            p.Add("RETURN_VALUE", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.ReturnValue);
            p.Add("P_FechaIni", fechaIni.Date, OracleMappingType.Date, ParameterDirection.Input);
            p.Add("P_FechaFin", fechaFin.Date, OracleMappingType.Date, ParameterDirection.Input);
            p.Add("P_Sigla", string.IsNullOrWhiteSpace(sigla) ? (object)DBNull.Value : sigla.Trim(), OracleMappingType.Varchar2, ParameterDirection.Input);
            p.Add("P_Usuario", usuario.HasValue ? (object)usuario.Value : DBNull.Value, OracleMappingType.Int64, ParameterDirection.Input);
            return p;
        }

        // Ejecutor genérico para las funciones que devuelven una lista.
        private async Task<DtoResultado<List<T>>> EjecutarListaAsync<T>(string operacion, string funcion,
            DateTime fechaIni, DateTime fechaFin, string? sigla, long? usuario)
        {
            var resp = new DtoResultado<List<T>> { Operacion = operacion, Data = new List<T>() };
            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);
                var p = ParametrosBase(fechaIni, fechaFin, sigla, usuario);
                await connection.OpenAsync();

                var lista = (await connection.QueryAsync<T>(
                    funcion, p, commandType: CommandType.StoredProcedure, commandTimeout: 120)).AsList();

                resp.Data = lista ?? new List<T>();
                resp.IdRespuesta = resp.Data.Count > 0 ? 1 : 0;
                resp.Mensaje = resp.Data.Count > 0 ? "Consulta Exitosa" : "No se encontraron datos";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion} | FechaIni={FechaIni} | FechaFin={FechaFin} | Sigla={Sigla} | Usuario={Usuario}",
                    operacion, fechaIni, fechaFin, sigla, usuario);
                resp.IdRespuesta = 0;
                resp.Mensaje = "Error al consultar la auditoría de uso.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion} | FechaIni={FechaIni} | FechaFin={FechaFin} | Sigla={Sigla} | Usuario={Usuario}",
                    operacion, fechaIni, fechaFin, sigla, usuario);
                resp.IdRespuesta = 0;
                resp.Mensaje = "Error al consultar la auditoría de uso.";
            }
            return resp;
        }

        public async Task<DtoResultado<DtoAudResumen>> F_GetResumen(DateTime fechaIni, DateTime fechaFin, string? sigla, long? usuario)
        {
            var resp = new DtoResultado<DtoAudResumen> { Operacion = "F_GetResumen", Data = new DtoAudResumen() };
            try
            {
                using var connection = new OracleConnection(_strConexionIris_Disec);
                var p = ParametrosBase(fechaIni, fechaFin, sigla, usuario);
                await connection.OpenAsync();

                var fila = (await connection.QueryAsync<DtoAudResumen>(
                    "PK_AUDITORIA_USO.F_GetResumen", p, commandType: CommandType.StoredProcedure, commandTimeout: 120)).FirstOrDefault();

                resp.Data = fila ?? new DtoAudResumen();
                resp.IdRespuesta = 1;
                resp.Mensaje = "Consulta Exitosa";
            }
            catch (OracleException oex)
            {
                _logger.LogError(oex, "OracleException en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = "Error al consultar el resumen de auditoría.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Dapper en {Operacion}", resp.Operacion);
                resp.IdRespuesta = 0;
                resp.Mensaje = "Error al consultar el resumen de auditoría.";
            }
            return resp;
        }

        public Task<DtoResultado<List<DtoAudPorModulo>>> F_GetPorModulo(DateTime fechaIni, DateTime fechaFin, string? sigla, long? usuario)
            => EjecutarListaAsync<DtoAudPorModulo>("F_GetPorModulo", "PK_AUDITORIA_USO.F_GetPorModulo", fechaIni, fechaFin, sigla, usuario);

        public Task<DtoResultado<List<DtoAudPorFecha>>> F_GetPorFecha(DateTime fechaIni, DateTime fechaFin, string? sigla, long? usuario)
            => EjecutarListaAsync<DtoAudPorFecha>("F_GetPorFecha", "PK_AUDITORIA_USO.F_GetPorFecha", fechaIni, fechaFin, sigla, usuario);

        public Task<DtoResultado<List<DtoAudPorUnidad>>> F_GetPorUnidad(DateTime fechaIni, DateTime fechaFin, string? sigla, long? usuario)
            => EjecutarListaAsync<DtoAudPorUnidad>("F_GetPorUnidad", "PK_AUDITORIA_USO.F_GetPorUnidad", fechaIni, fechaFin, sigla, usuario);

        public Task<DtoResultado<List<DtoAudPorUsuario>>> F_GetPorUsuario(DateTime fechaIni, DateTime fechaFin, string? sigla, long? usuario)
            => EjecutarListaAsync<DtoAudPorUsuario>("F_GetPorUsuario", "PK_AUDITORIA_USO.F_GetPorUsuario", fechaIni, fechaFin, sigla, usuario);
    }
}
