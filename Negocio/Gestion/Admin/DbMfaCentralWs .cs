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
    /// <summary>
    /// Clase encargada de gestionar la integración con el servicio central de MFA (Doble Autenticación).
    /// Maneja autenticación, cache de tokens y consumo de servicios externos.
    /// </summary>
    public class DbMfaCentralWs : IDbMfaCentralWs
    {
        // Cliente para consumir los servicios web de MFA
        private readonly IMfaWebServices _ws;

        // Configuración de la aplicación (appsettings, variables, etc.)
        private readonly IConfiguration _cfg;

        // Cache en memoria para almacenar tokens y evitar llamadas repetidas
        private readonly IMemoryCache _cache;

        // Logger para registrar errores y eventos
        private readonly ILogger<DbMfaCentralWs> _logger;

        // Nombre del sistema que consume el servicio MFA
        private readonly string _sistema;

        // Llave secreta para generar firmas HMAC
        private readonly string _hmacSecret;

        /// <summary>
        /// Constructor con inyección de dependencias
        /// </summary>
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

            // Obtiene el nombre del sistema desde configuración o usa valor por defecto
            _sistema = _cfg["MfaCentral:Sistema"] ?? "IRIS-P1";

            // Obtiene el secreto HMAC (obligatorio)
            _hmacSecret = _cfg["MfaCentral:HmacSecret"] ?? throw new InvalidOperationException("Falta MfaCentral:HmacSecret");
        }

        /// <summary>
        /// Genera la clave única para almacenar el token en cache
        /// </summary>
        private string CacheKey(string usuario, long identificacion)
        {
            // Normaliza el usuario (trim + mayúsculas)
            usuario = (usuario ?? "").Trim().ToUpperInvariant();

            // Construye una clave única por sistema + usuario + identificación
            return $"MFA_CENTRAL_BEARER::{_sistema}::{identificacion}::{usuario}";
        }

        /// <summary>
        /// Determina si una excepción es de conectividad (red, timeout, etc.)
        /// </summary>
        private static bool IsConnectivityException(Exception ex)
        {
            return ex is HttpRequestException // errores HTTP
                || ex is TaskCanceledException // timeouts
                || (ex.InnerException != null && IsConnectivityException(ex.InnerException)); // evalúa excepciones internas
        }

        /// <summary>
        /// Construye una respuesta estándar cuando el servicio MFA no está disponible
        /// </summary>
        private DtoResultado<T> FailMfa<T>(string mensajeUsuario, Exception? ex = null)
        {
            // Log del error si existe excepción
            if (ex != null)
                _logger.LogError(ex, "MFA CENTRAL no disponible / error de conectividad.");

            // Retorna respuesta controlada para el cliente
            return new DtoResultado<T>
            {
                CodigoExito = 0,
                Mensaje = $"MFA_SVC_DOWN|{mensajeUsuario}",
                Data = default
            };
        }


        #region Token
        /// <summary>
        /// Intenta obtener el token bearer manejando errores de forma controlada
        /// </summary>
        private async Task<(bool ok, string? bearer, DtoResultado<string>? error)> TryGetBearerAsync(string usuario, long identificacion)
        {
            try
            {
                // Intenta obtener el token
                var token = await GetBearerAsync(usuario, identificacion);
                return (true, token, null);
            }
            catch (Exception ex) when (IsConnectivityException(ex))
            {
                // Manejo específico de errores de conectividad
                return (false, null, FailMfa<string>(
                    "El servicio de Doble Autenticación (MFA) está presentando fallas. No es posible continuar.",
                    ex));
            }
            catch (Exception ex)
            {
                // Otros errores (firma, configuración, etc.)
                _logger.LogError(ex, "Error interno al obtener token MFA central.");
                return (false, null, new DtoResultado<string>
                {
                    CodigoExito = 0,
                    Mensaje = $"MFA_ERROR|No fue posible validar MFA en este momento: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Obtiene el token Bearer desde cache o desde el servicio externo
        /// </summary>
        private async Task<string> GetBearerAsync(string usuario, long identificacion)
        {
            var key = CacheKey(usuario, identificacion);

            // Intenta obtener el token desde cache
            if (_cache.TryGetValue(key, out string bearer) && !string.IsNullOrWhiteSpace(bearer))
                return bearer;

            // Genera datos de seguridad
            var fecha = DateTimeOffset.UtcNow;
            var nonce = Guid.NewGuid().ToString("N");

            // Construye firma HMAC
            var signature = BuildSignature(usuario, identificacion, _sistema, fecha, nonce);

            // Construye request
            var tokenReq = new DtoTokenSistemaReq
            {
                Usuario = usuario,
                Identificacion = identificacion,
                Sistema = _sistema,
                FechaUtc = fecha,
                Nonce = nonce,
                Signature = signature
            };

            // Llama al servicio externo
            var resp = await _ws.TokenSistemaAsync(tokenReq);

            // Valida respuesta
            if (resp.CodigoExito != 1 || string.IsNullOrWhiteSpace(resp.Data))
                throw new Exception($"No se pudo obtener token MFA central: {resp.Mensaje}");

            // Guarda en cache por 10 minutos
            _cache.Set(key, resp.Data, TimeSpan.FromMinutes(10));

            return resp.Data;
        }

        /// <summary>
        /// Construye la firma HMAC para autenticación segura
        /// </summary>
        private string BuildSignature(string usuario, long identificacion, string sistema, DateTimeOffset fechaUtc, string nonce)
        {
            // Cadena base (canonical)
            var canonical = $"{usuario}|{identificacion}|{sistema}|{fechaUtc:O}|{nonce}";

            // Genera hash HMAC SHA256 usando el secreto
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_hmacSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(canonical));

            // Retorna en formato hexadecimal
            return Convert.ToHexString(hash);
        }



        #endregion

        /// <summary>
        /// Consulta el estado MFA del usuario
        /// </summary>
        public async Task<DtoResultado<DtoMfaState>> StateAsync(long identificacion, string usuario)
        {
            var (ok, bearer, err) = await TryGetBearerAsync(usuario, identificacion);

            // Si no se pudo obtener token
            if (!ok)
                return new DtoResultado<DtoMfaState> { CodigoExito = err!.CodigoExito, Mensaje = err.Mensaje, Data = null };

            try
            {
                return await _ws.StateAsync(identificacion, usuario, bearer!);
            }
            catch (Exception ex) when (IsConnectivityException(ex))
            {
                return FailMfa<DtoMfaState>(
                    "El servicio de Doble Autenticación (MFA) está presentando fallas. No es posible validar.",
                    ex);
            }
        }

        /// <summary>
        /// Inicia el proceso de enrolamiento MFA
        /// </summary>
        public async Task<DtoResultado<DtoMfaEnrollStartResp>> EnrollStartAsync(long identificacion, string usuario)
        {
            var (ok, bearer, err) = await TryGetBearerAsync(usuario, identificacion);

            if (!ok)
                return new DtoResultado<DtoMfaEnrollStartResp> { CodigoExito = err!.CodigoExito, Mensaje = err.Mensaje, Data = null };

            try
            {
                return await _ws.EnrollStartAsync(new DtoMfaEnrollStartReq { Identificacion = identificacion, Usuario = usuario }, bearer);
            }
            catch (Exception ex) when (IsConnectivityException(ex))
            {
                return FailMfa<DtoMfaEnrollStartResp>(
                    "El servicio de Doble Autenticación (MFA) está presentando fallas. No es posible iniciar sesión.",
                    ex);
            }
        }

        /// <summary>
        /// Confirma el enrolamiento MFA
        /// </summary>
        public async Task<DtoResultado<DtoMfaEnrrollConfirmResp>> EnrollConfirmAsync(long identificacion, string usuario, string enrollToken, string code, string ip, long userAudit)
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

        /// <summary>
        /// Verifica el código MFA
        /// </summary>
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

        /// <summary>
        /// Verifica si un dispositivo es confiable
        /// </summary>
        public async Task<DtoResultado<int>> IsTrustedAsync(long identificacion, string usuario, string deviceId)
        {
            var bearer = await GetBearerAsync(usuario, identificacion);
            return await _ws.IsTrustedAsync(identificacion, usuario, deviceId, bearer);
        }

        /// <summary>
        /// Resetea MFA del usuario
        /// </summary>
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

        /// <summary>
        /// Cambia el estado del MFA (activar/desactivar)
        /// </summary>
        public async Task<DtoResultado<int>> ChangeMfaAsync(long identificacion, string usuario, int? estado2Fa, string ip, long userAudit)
        {
            var bearer = await GetBearerAsync(usuario, identificacion);

            return await _ws.ChangeMfaAsync(new DtoMfaResetReq
            {
                Identificacion = identificacion,
                Usuario = usuario,
                Estado2FA = estado2Fa,
                IpMaquina = ip,
                UserAudit = userAudit,
                Sistema = _sistema
            }, bearer);
        }

        /// <summary>
        /// Limpia dispositivos confiables del usuario
        /// </summary>
        public async Task<DtoResultado<int>> TrustClearUserAsync(long identificacion, string usuario, string ip, long usuarioAudita)
        {
            var bearer = await GetBearerAsync(usuario, usuarioAudita);

            return await _ws.TrustClearUserAsync(new DtoMfaTrustClearReq
            {
                Identificacion = identificacion,
                Usuario = usuario,
                IpMaquina = ip,
                UserAudit = usuarioAudita,
                Sistema = _sistema
            }, bearer);
        }

        /// <summary>
        /// Solicita reset de MFA
        /// </summary>
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

        /// <summary>
        /// Confirma reset de MFA
        /// </summary>
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

        /// <summary>
        /// Ejecuta el reset definitivo de MFA
        /// </summary>
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