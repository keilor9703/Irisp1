using Comun.Areas.Admin;
using Comun.DtoMfa;
using Comun.General;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Negocio.Interfaz.Admin;
using Servicios.ApiInterfaz;
using System.Security.Cryptography;
using System.Text;

namespace Negocio.Gestion.Admin
{

    public class DbMfaCentralWs : IDbMfaCentralWs
    {
        private readonly IMfaWebServices _ws;
        private readonly IConfiguration _cfg;
        private readonly IMemoryCache _cache;
        private readonly ILogger<DbMfaCentralWs> _logger;

        private readonly string _sistema;
        private readonly string _hmacSecret;

        //private const string CacheTokenKey = "MFA_CENTRAL_BEARER";

        public DbMfaCentralWs(
            IMfaWebServices ws,
            IConfiguration cfg,
            IMemoryCache cache,
            ILogger<DbMfaCentralWs> logger)
        {
            _ws = ws;
            _cfg = cfg;
            _cache = cache;
            _logger = logger;

            _sistema = _cfg["MfaCentral:Sistema"] ?? "IRIS-P1";
            _hmacSecret = _cfg["MfaCentral:HmacSecret"] ?? throw new InvalidOperationException("Falta MfaCentral:HmacSecret");
        }



        private string CacheKey(string usuario, long identificacion)
        {
            usuario = (usuario ?? "").Trim().ToUpperInvariant();
            return $"MFA_CENTRAL_BEARER::{_sistema}::{identificacion}::{usuario}";
        }



        private async Task<string> GetBearerAsync(string usuario, long identificacion)
        {
            var key = CacheKey(usuario, identificacion);

            if (_cache.TryGetValue(key, out string bearer) && !string.IsNullOrWhiteSpace(bearer))
                return bearer;

            var fecha = DateTimeOffset.UtcNow;
            var nonce = Guid.NewGuid().ToString("N");
            var signature = BuildSignature(usuario, identificacion, _sistema, fecha, nonce);

            var tokenReq = new DtoTokenSistemaReq
            {
                Usuario = usuario,
                Identificacion = identificacion,
                Sistema = _sistema,
                FechaUtc = fecha,
                Nonce = nonce,
                Signature = signature
            };

            var resp = await _ws.TokenSistemaAsync(tokenReq);

            if (resp.CodigoExito != 1 || string.IsNullOrWhiteSpace(resp.Data))
                throw new Exception($"No se pudo obtener token MFA central: {resp.Mensaje}");

            // Cache conservador (15 min)
            _cache.Set(key, resp.Data, TimeSpan.FromMinutes(10));
            return resp.Data;
        }


        private string BuildSignature(string usuario, long identificacion, string sistema, DateTimeOffset fechaUtc, string nonce)
        {
            var canonical = $"{usuario}|{identificacion}|{sistema}|{fechaUtc:O}|{nonce}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_hmacSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(canonical));
            return Convert.ToHexString(hash);
        }

        public async Task<DtoResultado<DtoMfaState>> StateAsync(long identificacion, string usuario)
        {
            var bearer = await GetBearerAsync(usuario, identificacion);
            return await _ws.StateAsync(identificacion, usuario, bearer);
        }

        public async Task<DtoResultado<DtoMfaEnrollStartResp>> EnrollStartAsync(long identificacion, string usuario)
        {
            var bearer = await GetBearerAsync(usuario, identificacion);
            return await _ws.EnrollStartAsync(new DtoMfaEnrollStartReq
            {
                Identificacion = identificacion,
                Usuario = usuario
            }, bearer);
        }

        public async Task<DtoResultado<bool>> EnrollConfirmAsync(long identificacion, string usuario, string enrollToken, string code, string ip, long userAudit)
        {
            var bearer = await GetBearerAsync(usuario, identificacion);
            return await _ws.EnrollConfirmAsync(new DtoMfaEnrollConfirmReq
            {
                Identificacion = identificacion,
                Usuario = usuario,
                EnrollToken = enrollToken,
                Code = code,
                IpMaquina = ip,
                UserAudit = userAudit
            }, bearer);
        }

        public async Task<DtoResultado<DtoMfaVerifyResp>> VerifyAsync(long identificacion, string usuario, string code, bool rememberDevice, string? deviceId, string ip, long userAudit)
        {
            var bearer = await GetBearerAsync(usuario, identificacion);
            return await _ws.VerifyAsync(new DtoMfaVerifyReq
            {
                Identificacion = identificacion,
                Usuario = usuario,
                Code = code,
                RememberDevice = rememberDevice,
                DeviceId = deviceId,
                IpMaquina = ip,
                UserAudit = userAudit
            }, bearer);
        }

        public async Task<DtoResultado<int>> IsTrustedAsync(long identificacion, string usuario, string deviceId)
        {
            var bearer = await GetBearerAsync(usuario, identificacion);
            return await _ws.IsTrustedAsync(identificacion, usuario, deviceId, bearer);
        }

        public async Task<DtoResultado<int>> ResetAsync(long identificacion, string usuario, string ip, long userAudit)
        {
            var bearer = await GetBearerAsync(usuario, identificacion);
            return await _ws.ResetAsync(new DtoMfaResetReq
            {
                Identificacion = identificacion,
                Usuario = usuario,
                IpMaquina = ip,
                UserAudit = userAudit,
                Sistema = _sistema
            }, bearer);
        }


        public async Task<DtoResultado<int>> ResetRequestAsync(long identificacion, string usuario, string ip, long userAudit)
        {
            var bearer = await GetBearerAsync(usuario, identificacion);
            return await _ws.ResetRequestAsync(new DtoMfaResetRequestReq
            {
                Identificacion = identificacion,
                Usuario = usuario,
                IpMaquina = ip,
                UserAudit = userAudit,
                Sistema = _sistema
            }, bearer);
        }

        public async Task<DtoResultado<DtoMfaResetConfirmResp>> ResetConfirmAsync(long identificacion, string usuario, string code, string ip, long userAudit)
        {
            var bearer = await GetBearerAsync(usuario, identificacion);
            return await _ws.ResetConfirmAsync(new DtoMfaResetConfirmReq
            {
                Identificacion = identificacion,
                Usuario = usuario,
                Code = code,
                IpMaquina = ip,
                UserAudit = userAudit,
                Sistema = _sistema
            }, bearer);
        }

        public async Task<DtoResultado<int>> ResetExecuteAsync(long identificacion, string usuario, string ip, long userAudit)
        {
            var bearer = await GetBearerAsync(usuario, identificacion);
            return await _ws.ResetExecuteAsync(new DtoMfaResetExecuteReq
            {
                Identificacion = identificacion,
                Usuario = usuario,
                IpMaquina = ip,
                UserAudit = userAudit,
                Sistema = _sistema
            }, bearer);
        }




    }
}
