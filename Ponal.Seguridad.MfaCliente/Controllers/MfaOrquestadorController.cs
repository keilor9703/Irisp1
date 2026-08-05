using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ponal.Seguridad.MfaCliente.Interfaces; // Ajusta al namespace de IDbMfaCentralWs
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Ponal.Seguridad.MfaCliente.Modelos.DtoMfa;

namespace Ponal.Seguridad.MfaCliente.Controllers
{
    [AllowAnonymous]
    [Route("MfaOrquestador")]
    public class MfaOrquestadorController : Controller
    {
        private readonly IDbMfaCentralWs _mfaWs;
        private const string TdMfaPending = "MFA_PENDING";
        private const string TdEnrollToken = "MFA_ENROLL_TOKEN";

        public MfaOrquestadorController(IDbMfaCentralWs mfaWs)
        {
            _mfaWs = mfaWs;
        }

        private MfaPendingDto? GetPendingData()
        {
            var json = TempData.Peek(TdMfaPending) as string;
            if (string.IsNullOrWhiteSpace(json)) return null;
            try { return JsonConvert.DeserializeObject<MfaPendingDto>(json); }
            catch { return null; }
        }

        [HttpPost("VerifyAjax")]
        public async Task<IActionResult> VerifyAjax([FromForm] string Code, [FromForm] bool RememberDevice)
        {
            var pending = GetPendingData();
            if (pending == null) return Json(new { success = false, message = "Sesión expirada. Recargue la página." });

            // ✅ PUNTO DE CONTROL
            if (string.IsNullOrWhiteSpace(Code))
                return Json(new { success = false, message = "Ingrese el código de 6 dígitos." });

            string? deviceId = RememberDevice ? (Request.Cookies["IRISP_MFA_DEVICE"] ?? Guid.NewGuid().ToString("N")) : null;

            var verify = await _mfaWs.VerifyAsync(
                pending.Identificacion, pending.Usuario, Code, RememberDevice, deviceId, pending.Ip ?? "0.0.0.0"
            );

            if (verify.CodigoError == 9 || (verify.Data?.BloqueoHasta.HasValue == true && verify.Data.BloqueoHasta.Value > DateTime.Now))
                return Json(new { success = false, blocked = true, bloqueoHasta = verify.Data?.BloqueoHasta?.ToString("yyyy-MM-dd HH:mm:ss") });

            if (verify.CodigoExito != 1 || verify.Data?.Ok != true)
                return Json(new { success = false, message = verify.Mensaje ?? "Código inválido." });

            if (RememberDevice && !string.IsNullOrWhiteSpace(deviceId))
            {
                Response.Cookies.Append("IRISP_MFA_DEVICE", deviceId, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.Now.AddDays(30)
                });
            }

            HttpContext.Session.SetString($"MFA_PASSED_{pending.Identificacion}", "true");
            // Url.Content("~/...") respeta el PathBase / directorio virtual (p.ej. "/iris2fa"),
            // evitando que la redirección apunte a la raíz del dominio.
            return Json(new { success = true, redirectUrl = Url.Content("~/Cuenta/FinalizeMfaLogin") });
        }

        [HttpPost("EnrollConfirmAjax")]
        public async Task<IActionResult> EnrollConfirmAjax([FromForm] string Code)
        {
            var pending = GetPendingData();
            var enrollToken = TempData.Peek(TdEnrollToken) as string;

            if (pending == null || string.IsNullOrWhiteSpace(enrollToken))
                return Json(new { success = false, message = "Sesión expirada. Recargue la página." });

            // ✅ PUNTO DE CONTROL
            if (string.IsNullOrWhiteSpace(Code))
                return Json(new { success = false, message = "Ingrese el código de 6 dígitos." });

            var resp = await _mfaWs.EnrollConfirmAsync(
                pending.Identificacion, pending.Usuario, enrollToken, Code, pending.Ip ?? "0.0.0.0"
            );

            if (resp.CodigoError == 9 || (resp.Data?.BloqueoHasta.HasValue == true && resp.Data.BloqueoHasta.Value > DateTime.Now))
                return Json(new { success = false, blocked = true, bloqueoHasta = resp.Data?.BloqueoHasta?.ToString("yyyy-MM-dd HH:mm:ss") });

            if (resp.CodigoExito != 1 || resp.Data?.Ok != true)
                return Json(new { success = false, message = resp.Mensaje ?? "Código inválido." });

            HttpContext.Session.SetString($"MFA_PASSED_{pending.Identificacion}", "true");
            // Url.Content("~/...") respeta el PathBase / directorio virtual (p.ej. "/iris2fa"),
            // evitando que la redirección apunte a la raíz del dominio.
            return Json(new { success = true, redirectUrl = Url.Content("~/Cuenta/FinalizeMfaLogin") });
        }

        [HttpPost("ResetRequestAjax")]
        public async Task<IActionResult> ResetRequestAjax()
        {
            var pending = GetPendingData();
            if (pending == null) return Json(new { success = false, message = "Sesión expirada." });

            var resp = await _mfaWs.ResetRequestAsync(pending.Identificacion, pending.Usuario, pending.Ip ?? "0.0.0.0");

            if (resp.CodigoExito != 1) return Json(new { success = false, message = resp.Mensaje ?? "Error enviando código." });

            return Json(new { success = true, isInfo = true, message = resp.Mensaje });
        }

        [HttpPost("ResetConfirmAjax")]
        public async Task<IActionResult> ResetConfirmAjax([FromForm] string Code)
        {
            var pending = GetPendingData();
            if (pending == null) return Json(new { success = false, message = "Sesión expirada." });

            // ✅ PUNTO DE CONTROL
            if (string.IsNullOrWhiteSpace(Code))
                return Json(new { success = false, message = "Ingrese el código que enviamos a su correo." });

            // 1. Confirmar el código enviado al correo
            var resp = await _mfaWs.ResetConfirmAsync(pending.Identificacion, pending.Usuario, Code, pending.Ip ?? "0.0.0.0");
            if (resp.CodigoExito != 1) return Json(new { success = false, message = resp.Mensaje ?? "Código inválido." });

            // 2. Ejecutar el reset en la base de datos
            var exec = await _mfaWs.ResetExecuteAsync(pending.Identificacion, pending.Usuario, pending.Ip ?? "0.0.0.0");
            if (exec.CodigoExito != 1) return Json(new { success = false, message = exec.Mensaje ?? "Aún no disponible." });

            // 3. ✨ INICIAR EL ENROLAMIENTO AQUÍ MISMO ✨
            var enrollStart = await _mfaWs.EnrollStartAsync(pending.Identificacion, pending.Usuario);

            if (enrollStart.CodigoExito != 1 || enrollStart.Data == null)
                return Json(new { success = false, message = "Se restableció el MFA, pero hubo un error al generar el nuevo código QR. Inicie sesión nuevamente." });

            // Guardar el nuevo token de enrolamiento en TempData
            TempData[TdEnrollToken] = enrollStart.Data.EnrollToken;
            TempData.Keep(TdEnrollToken);
            TempData.Keep(TdMfaPending); // Mantener el pending vivo

            // Devolver la orden de cambiar de modal junto con el QR y la llave
            return Json(new
            {
                success = true,
                switchToEnroll = true,
                qrBase64 = enrollStart.Data.QrBase64,
                manualKey = enrollStart.Data.ManualKey
            });
        }

        [HttpPost("CancelAjax")]
        public IActionResult CancelAjax()
        {
            var pending = GetPendingData();
            if (pending != null)
            {
                HttpContext.Session.Remove($"MFA_PASSED_{pending.Identificacion}");
            }

            TempData.Remove("LOGIN_USERDATA"); // Constante TdLoginUserData
            TempData.Remove(TdMfaPending);
            TempData.Remove(TdEnrollToken);

            return Json(new { success = true });
        }
    }
}