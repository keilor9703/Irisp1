using Comun.Areas.Admin;
using Dapper;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Negocio.Gestion.Admin;

public class DbMfaIris : IDbMfaIris
{
    private readonly IConfiguration _cfg;
    private readonly ILogger<DbMfaIris> _logger;
    private readonly string _connStr;

    public DbMfaIris(IConfiguration cfg, ILogger<DbMfaIris> logger)
    {
        _cfg = cfg;
        _logger = logger;
        _connStr = _cfg.GetConnectionString("strConexionIris_Disec")
                   ?? throw new InvalidOperationException("No existe connection string 'strConexionIris_Disec'.");
    }

    private OracleConnection CreateConnection() => new OracleConnection(_connStr);

    public async Task<MfaDbDto> GetMfaAsync(long idUsuario)
    {
        const string sp = "PK_IRIS_MFA.P_GET_MFA";

        try
        {
            using var con = CreateConnection();
            await con.OpenAsync();

            var p = new OracleDynamicParameters();
            p.Add("P_ID_USUARIO", idUsuario, OracleMappingType.Int64, ParameterDirection.Input);

            p.Add("O_MFA_HABILITADO", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            p.Add("O_TOTP_SECRET_ENC", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 4000);
            p.Add("O_REQUIRE_REENROLL", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            p.Add("O_BLOQUEO_HASTA", dbType: OracleMappingType.Date, direction: ParameterDirection.Output);
            p.Add("O_INTENTOS_FALLIDOS", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

            await con.ExecuteAsync(sp, p, commandType: CommandType.StoredProcedure);

            // OJO: si un OUT viene NULL, Get<T> puede fallar; por eso usamos object y convertimos seguro.
            int mfaHab = SafeToInt(p.Get<dynamic>("O_MFA_HABILITADO"));
            string? secretEnc = SafeToString(p.Get<dynamic>("O_TOTP_SECRET_ENC"));
            int reenroll = SafeToInt(p.Get<dynamic>("O_REQUIRE_REENROLL"));
            DateTime? bloqueoHasta = SafeToDateTime(p.Get<dynamic>("O_BLOQUEO_HASTA"));
            int intentos = SafeToInt(p.Get<dynamic>("O_INTENTOS_FALLIDOS"));

            return new MfaDbDto
            {
                MfaHabilitado = mfaHab,
                TotpSecretEnc = secretEnc,
                RequireReenroll = reenroll,
                BloqueoHasta = bloqueoHasta,
                IntentosFallidos = intentos
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetMfaAsync (idUsuario={IdUsuario})", idUsuario);
            throw;
        }
    }

    public async Task P_Guardar_LlaveSecreta(long idUsuario, string secretEnc, string ip, long userAudit)
    {
        const string sp = "PK_IRIS_MFA.P_Guardar_LlaveSecreta";

        try
        {
            using var con = CreateConnection();
            await con.OpenAsync();

            var p = new OracleDynamicParameters();
            p.Add("P_ID_USUARIO", idUsuario, OracleMappingType.Int64, ParameterDirection.Input);
            p.Add("P_TOTP_SECRET_ENC", secretEnc, OracleMappingType.Varchar2, ParameterDirection.Input, size: 4000);
            p.Add("P_MAQUINA", ip ?? "0.0.0.0", OracleMappingType.Varchar2, ParameterDirection.Input, size: 255);
            p.Add("P_USUARIO", userAudit, OracleMappingType.Int64, ParameterDirection.Input);

            await con.ExecuteAsync(sp, p, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en P_Guardar_LlaveSecreta (idUsuario={IdUsuario})", idUsuario);
            throw;
        }
    }

    public async Task P_Validacion_exitosa(long idUsuario, string ip, long userAudit)
    {
        const string sp = "PK_IRIS_MFA.P_Validacion_exitosa";

        try
        {
            using var con = CreateConnection();
            await con.OpenAsync();

            var p = new OracleDynamicParameters();
            p.Add("P_ID_USUARIO", idUsuario, OracleMappingType.Int64, ParameterDirection.Input);
            p.Add("P_MAQUINA", ip ?? "0.0.0.0", OracleMappingType.Varchar2, ParameterDirection.Input, size: 255);
            p.Add("P_USUARIO", userAudit, OracleMappingType.Int64, ParameterDirection.Input);

            await con.ExecuteAsync(sp, p, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en P_Validacion_exitosa (idUsuario={IdUsuario})", idUsuario);
            throw;
        }
    }

    public async Task P_Intentos_Fallidos(long idUsuario, string ip, long userAudit, int bloqueoMinutos = 5, int maxIntentos = 5)
    {
        const string sp = "PK_IRIS_MFA.P_Intentos_Fallidos";

        try
        {
            using var con = CreateConnection();
            await con.OpenAsync();

            var p = new OracleDynamicParameters();
            p.Add("P_ID_USUARIO", idUsuario, OracleMappingType.Int64, ParameterDirection.Input);
            p.Add("P_MAQUINA", ip ?? "0.0.0.0", OracleMappingType.Varchar2, ParameterDirection.Input, size: 255);
            p.Add("P_USUARIO", userAudit, OracleMappingType.Int64, ParameterDirection.Input);
            p.Add("P_BLOQUEO_MINUTOS", bloqueoMinutos, OracleMappingType.Int32, ParameterDirection.Input);
            p.Add("P_MAX_INTENTOS", maxIntentos, OracleMappingType.Int32, ParameterDirection.Input);

            await con.ExecuteAsync(sp, p, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en P_Intentos_Fallidos (idUsuario={IdUsuario})", idUsuario);
            throw;
        }
    }

    public async Task<int> IsTrustedDeviceAsync(long idUsuario, string deviceHash)
    {
        const string sql = "SELECT PK_IRIS_MFA.F_IS_DEVICE_TRUSTED(:P_ID_USUARIO, :P_DEVICE_ID_HASH) FROM DUAL";

        try
        {
            using var con = CreateConnection();
            await con.OpenAsync();

            var p = new OracleDynamicParameters();
            p.Add("P_ID_USUARIO", idUsuario, OracleMappingType.Int64, ParameterDirection.Input);
            p.Add("P_DEVICE_ID_HASH", deviceHash, OracleMappingType.Varchar2, ParameterDirection.Input, size: 128);

            int result = await con.ExecuteScalarAsync<int>(sql, p);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en IsTrustedDeviceAsync (idUsuario={IdUsuario})", idUsuario);
            throw;
        }
    }

    public async Task SaveTrustedDeviceAsync(long idUsuario, string deviceHash, int expiraDias, string ip, long userAudit)
    {
        const string sp = "PK_IRIS_MFA.P_SAVE_TRUSTED_DEVICE";

        try
        {
            using var con = CreateConnection();
            await con.OpenAsync();

            var p = new OracleDynamicParameters();
            p.Add("P_ID_USUARIO", idUsuario, OracleMappingType.Int64, ParameterDirection.Input);
            p.Add("P_DEVICE_ID_HASH", deviceHash, OracleMappingType.Varchar2, ParameterDirection.Input, size: 128);
            p.Add("P_EXPIRA_DIAS", expiraDias, OracleMappingType.Int32, ParameterDirection.Input);
            p.Add("P_MAQUINA", ip ?? "0.0.0.0", OracleMappingType.Varchar2, ParameterDirection.Input, size: 255);
            p.Add("P_USUARIO", userAudit, OracleMappingType.Int64, ParameterDirection.Input);

            await con.ExecuteAsync(sp, p, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en SaveTrustedDeviceAsync (idUsuario={IdUsuario})", idUsuario);
            throw;
        }
    }

    public async Task ResetMfaAsync(long idUsuario, string ip, long userAudit)
    {
        const string sp = "PK_IRIS_MFA.P_RESET_MFA";

        try
        {
            using var con = CreateConnection();
            await con.OpenAsync();

            var p = new OracleDynamicParameters();
            p.Add("P_ID_USUARIO", idUsuario, OracleMappingType.Int64, ParameterDirection.Input);
            p.Add("P_MAQUINA", ip ?? "0.0.0.0", OracleMappingType.Varchar2, ParameterDirection.Input, size: 255);
            p.Add("P_USUARIO", userAudit, OracleMappingType.Int64, ParameterDirection.Input);

            await con.ExecuteAsync(sp, p, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ResetMfaAsync (idUsuario={IdUsuario})", idUsuario);
            throw;
        }
    }

    // ===== helpers OUT NULL safe =====
    private static int SafeToInt(object? v)
    {
        if (v is null) return 0;
        if (v is int i) return i;
        if (v is long l) return (int)l;
        if (v is decimal d) return (int)d;
        if (int.TryParse(v.ToString(), out var r)) return r;
        return 0;
    }

    private static string? SafeToString(object? v)
    {
        if (v is null) return null;
        var s = v.ToString();
        return string.IsNullOrWhiteSpace(s) ? null : s;
    }

    private static DateTime? SafeToDateTime(object? v)
    {
        if (v is null) return null;
        if (v is DateTime dt) return dt;
        if (DateTime.TryParse(v.ToString(), out var r)) return r;
        return null;
    }
}
