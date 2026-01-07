using Comun.Areas.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.Interfaz.Admin;
using Web;

[AllowAnonymous]
public class MfaController : Controller
{
    private readonly IMfaTotpService _totp;
    private readonly IDbMfaIris _dbMfa;
    private readonly IDbAdministracion _dbAdmin; // para auditoría, si quieres
    private readonly IConfiguration _cfg;

    private const string SessMfaPending = "MFA_PENDING";
    private const string SessEnrollSecret = "MFA_ENROLL_SECRET";
    private const string CookieTrusted = "IRISP_MFA_DEVICE";

    public MfaController(IMfaTotpService totp, IDbMfaIris dbMfa, IDbAdministracion dbAdmin, IConfiguration cfg)
    {
        _totp = totp;
        _dbMfa = dbMfa;
        _dbAdmin = dbAdmin;
        _cfg = cfg;
    }

    // Guardas el usuario pendiente de MFA en Session al terminar user/pass OK
    private MfaStateDto? GetPending()
        => HttpContext.Session.GetObject<MfaStateDto>(SessMfaPending);

    [HttpGet]
    public async Task<IActionResult> Enroll()
    {
        var pending = GetPending();
        if (pending is null) return RedirectToAction("InicioSesion", "Cuenta");

        var mfa = await _dbMfa.GetMfaAsync(pending.IdUsuario);

        // Si ya tiene MFA y no requiere re-enroll, no se enrolla
        if (mfa.MfaHabilitado == 1 && mfa.RequireReenroll == 0)
            return RedirectToAction(nameof(Verify));

        var issuer = "IRIS-P1";
        var account = pending.Usuario; // o pending.Identificacion.ToString()

        var (secretBase32, qrPngBase64) = _totp.GenerateEnrollmentQr(issuer, account);

        HttpContext.Session.SetString(SessEnrollSecret, secretBase32);

        return View(new MfaEnrollViewModel
        {
            QrCodeBase64Png = qrPngBase64,
            ManualKey = secretBase32
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(MfaEnrollViewModel vm)
    {
        var pending = GetPending();
        if (pending is null) return RedirectToAction("InicioSesion", "Cuenta");

        var secretBase32 = HttpContext.Session.GetString(SessEnrollSecret);
        if (string.IsNullOrWhiteSpace(secretBase32))
        {
            ModelState.AddModelError("", "Sesión de enrolamiento expirada. Intente nuevamente.");
            return RedirectToAction(nameof(Enroll));
        }

        if (!_totp.ValidateCode(secretBase32, vm.Code))
        {
            ModelState.AddModelError("", "Código inválido. Verifique la hora del celular y vuelva a intentar.");
            return RedirectToAction(nameof(Enroll));
        }


        var secretEnc = _totp.ProtectSecret(secretBase32);
        await _dbMfa.P_Guardar_LlaveSecreta(pending.IdUsuario, secretEnc, pending.Ip, pending.Identificacion);

        // Auditoría opcional
        await _dbAdmin.P_InsAuditoria(pending.Identificacion, "MFA Enroll OK", "Usuario enroló TOTP", pending.Identificacion, pending.Ip);

        HttpContext.Session.Remove(SessEnrollSecret);
        return RedirectToAction(nameof(Verify));
    }

    [HttpGet]
    public async Task<IActionResult> Verify()
    {
        var pending = GetPending();
        if (pending is null) return RedirectToAction("InicioSesion", "Cuenta");

        var mfa = await _dbMfa.GetMfaAsync(pending.IdUsuario);

        // Bloqueo temporal por intentos
        if (mfa.BloqueoHasta.HasValue && mfa.BloqueoHasta.Value > DateTime.Now)
        {
            ViewBag.BloqueoHasta = mfa.BloqueoHasta.Value;
            return View("Blocked");
        }

        // Si dispositivo confiable, saltar MFA
        var deviceId = Request.Cookies[CookieTrusted];
        if (!string.IsNullOrWhiteSpace(deviceId))
        {
            var hash = _totp.HashDeviceId(deviceId);
            var trusted = await _dbMfa.IsTrustedDeviceAsync(pending.IdUsuario, hash);
            if (trusted == 1)
            {
                // marca OK y finaliza login
                await _dbMfa.P_Validacion_exitosa(pending.IdUsuario, pending.Ip, pending.Identificacion);
                return await FinalizeLogin(pending);
            }
        }

        // Si no está habilitado, forzar enrolamiento
        if (mfa.MfaHabilitado != 1 || mfa.RequireReenroll == 1 || string.IsNullOrWhiteSpace(mfa.TotpSecretEnc))
            return RedirectToAction(nameof(Enroll));

        return View(new MfaVerifyViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Verify(MfaVerifyViewModel vm)
    {
        var pending = GetPending();
        if (pending is null) return RedirectToAction("InicioSesion", "Cuenta");

        var mfa = await _dbMfa.GetMfaAsync(pending.IdUsuario);

        if (mfa.BloqueoHasta.HasValue && mfa.BloqueoHasta.Value > DateTime.Now)
        {
            ViewBag.BloqueoHasta = mfa.BloqueoHasta.Value;
            return View("Blocked");
        }

        if (mfa.MfaHabilitado != 1 || string.IsNullOrWhiteSpace(mfa.TotpSecretEnc))
            return RedirectToAction(nameof(Enroll));

        var secretBase32 = _totp.UnprotectSecret(mfa.TotpSecretEnc);

        if (!_totp.ValidateCode(secretBase32, vm.Code))
        {
            await _dbMfa.P_Intentos_Fallidos(pending.IdUsuario, pending.Ip, pending.Identificacion);
            await _dbAdmin.P_InsAuditoria(pending.Identificacion, "MFA Fail", "Código TOTP inválido", pending.Identificacion, pending.Ip);

            ModelState.AddModelError("", "Código inválido.");
            return View(vm);
        }

        await _dbMfa.P_Validacion_exitosa(pending.IdUsuario, pending.Ip, pending.Identificacion);
        await _dbAdmin.P_InsAuditoria(pending.Identificacion, "MFA OK", "Validación TOTP exitosa", pending.Identificacion, pending.Ip);

        if (vm.RememberDevice)
        {
            var deviceId = Guid.NewGuid().ToString("N");
            var hash = _totp.HashDeviceId(deviceId);

            await _dbMfa.SaveTrustedDeviceAsync(pending.IdUsuario, hash, expiraDias: 30, pending.Ip, pending.Identificacion);

            Response.Cookies.Append(CookieTrusted, deviceId, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.Now.AddDays(30)
            });
        }

        return await FinalizeLogin(pending);
    }

    private async Task<IActionResult> FinalizeLogin(MfaStateDto pending)
    {
        // Aquí NO vuelves a validar contraseña.
        // Solo terminas el login creando cookie de auth y menú.

        // Recupera el usuario completo de nuevo si lo necesitas (o guarda datos del Usuario.Data en Session al inicio).
        // Lo más simple: en la etapa user/pass OK, guardas un objeto Usuario.Data en sesión "LOGIN_USERDATA".
        var usuario = HttpContext.Session.GetObject<dynamic>("LOGIN_USERDATA"); // cambia a tu tipo real

        // ⚠️ Recomendación: aquí crea claims y SignInAsync (exactamente como ya lo haces)
        // Para mantener el ejemplo corto, delega a un método en CuentaController o crea un AuthService.
        // Como mínimo: redirige a una acción en Cuenta que finalice el sign-in leyendo "LOGIN_USERDATA".
        return RedirectToAction("FinalizeMfaLogin", "Cuenta");
    }
}
