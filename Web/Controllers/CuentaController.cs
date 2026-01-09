using Comun.Areas.Admin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Negocio.Gestion.Admin;
using Negocio.Interfaz.Admin;
using Newtonsoft.Json;
using System.Security.Claims;
using Web;

namespace Web.Controllers
{
    [Authorize]
    public class CuentaController : Controller
    {
        private readonly IHttpContextAccessor _iHttpContextAccessor;
        private readonly IDbAdministracion _iDbAdministracion;
        private readonly IDbConsultasPIP _iDbConsultasPIP;
        private readonly IConfiguration _iConfiguration;

        // ✅ MFA
        private readonly IDbMfaIris _dbMfa;
        private readonly IMfaTotpService _totp;

        private bool Admin = false;

        // ===== TempData keys (cookie-based) =====
        private const string TdLoginUserData = "LOGIN_USERDATA";
        private const string TdMfaPending = "MFA_PENDING";

        // ===== Session keys (solo después de login OK) =====
        private const string SessEnrollSecret = "MFA_ENROLL_SECRET";
        private const string SessEnrollQr = "MFA_ENROLL_QR";

        // ===== Cookies =====
        private const string CookieTrusted = "IRISP_MFA_DEVICE";

        public CuentaController(
            IHttpContextAccessor iHttpContextAccessor,
            IConfiguration iConfiguration,
            IDbAdministracion iDbAdministracion,
            IDbConsultasPIP idbConsultasPIP,
            IDbMfaIris dbMfa,
            IMfaTotpService totp
        )
        {
            _iHttpContextAccessor = iHttpContextAccessor;
            _iConfiguration = iConfiguration;
            _iDbAdministracion = iDbAdministracion;
            _iDbConsultasPIP = idbConsultasPIP;

            _dbMfa = dbMfa;
            _totp = totp;
        }

        // ============================================================
        // LOGIN GET
        // ============================================================
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> InicioSesion(string returnurl = null, string _mensaje = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(_mensaje))
                {
                    // PARA INICIO DESDE SISEC
                    string key = _iConfiguration["Encryption:Key"];

                    string mensajeRecibido = _iDbAdministracion.ConvertirBase64Bytes(_mensaje);
                    string mensajeDesencriptado = _iDbAdministracion.Decript(mensajeRecibido, key);

                    DtoCredencialesSisec loginDTO =
                        JsonConvert.DeserializeObject<DtoCredencialesSisec>(mensajeDesencriptado);

                    string claveBase64 = _iDbAdministracion.ConvertirBase64Bytes(loginDTO.Clave);
                    string claveDesencriptada = _iDbAdministracion.Decript(claveBase64, key);

                    var loginUsuario = new DtoCredenciales
                    {
                        UsuarioEmpresarial = loginDTO.Usuario,
                        ClaveEmpresarial = claveDesencriptada
                    };

                    return await InicioSesionAsync(loginUsuario, returnurl);
                }

                ViewData["ReturnUrl"] = returnurl;
                return View("InicioSesion", new DtoCredenciales());
            }
            catch
            {
                return View("ErrorGeneral");
            }
        }

        // ============================================================
        // LOGIN POST (credenciales)
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> InicioSesionAsync(DtoCredenciales loginUsuario, string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Action(nameof(HomeController.Index), "Home");

            if (!ModelState.IsValid)
                return View("InicioSesion", loginUsuario);

            // OUD
            var respuestaOud = await _iDbConsultasPIP.ObtenerOudAsync(loginUsuario);
            if (!respuestaOud.Respuesta)
            {
                ModelState.AddModelError("", "Usuario o Contraseña incorrecta, valide la información ingresada");
                return View("InicioSesion", loginUsuario);
            }

            // IP
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrWhiteSpace(ip)) ip = "0.0.0.0";

            
            HttpContext.Session.SetString("IpMaquina", ip);

            var Usuario = await _iDbAdministracion.P_GetValidaUser(loginUsuario.UsuarioEmpresarial, ip);

            if (Usuario.Data.Identificacion == 0)
            {
                ModelState.AddModelError("", "Usuario no se encuentra registrado");
                return View("InicioSesion", loginUsuario);
            }

            if (Usuario.Data.Bloqueado == 1)
            {
                ModelState.AddModelError("", "Su cuenta de usuario está DESHABILITADA, contacte al Administrador");
                return View("InicioSesion", loginUsuario);
            }

            if (Usuario.Data.DtoUserRoles == null || Usuario.Data.DtoUserRoles.Count == 0)
            {
                ModelState.AddModelError("", "Su cuenta de usuario no tiene roles asignados para ingresar a este sistema, contacte al Administrador");
                return View("InicioSesion", loginUsuario);
            }

            Admin = Usuario.Data.DtoUserRoles.Any(x => x.IdRol == 1);

            // ============================================================
            // ✅ MFA (NO USAR SESSION para el estado del MFA)
            // ============================================================
            bool requiereMfa = Admin; 

            if (!requiereMfa)
            {
                // ✅ Guardar estado en TempData 
                TempData[TdLoginUserData] = JsonConvert.SerializeObject(Usuario.Data);

                TempData[TdMfaPending] = JsonConvert.SerializeObject(new MfaPendingDto
                {
                    IdUsuario = Usuario.Data.IdUsuario,
                    Identificacion = Usuario.Data.Identificacion,
                    Usuario = Usuario.Data.Usuario ?? Usuario.Data.Identificacion.ToString(),
                    Funcionario = Usuario.Data.Funcionario ?? "",
                    Ip = ip
                });

                // 🔁Mantener TempData vivo para el siguiente POST (MFA)
                TempData.Keep(TdLoginUserData);
                TempData.Keep(TdMfaPending);

                await _iDbAdministracion.P_InsAuditoria(
                    Convert.ToInt64(Usuario.Data.Identificacion),
                    "MFA Required",
                    "Usuario requiere validación TOTP para completar el inicio de sesión",
                    Convert.ToInt64(Usuario.Data.Identificacion),
                    ip
                );

                // Consultar estado MFA en BD
                var mfa = await _dbMfa.GetMfaAsync(Usuario.Data.IdUsuario);

                // Bloqueo temporal
                if (mfa.BloqueoHasta.HasValue && mfa.BloqueoHasta.Value > DateTime.Now)
                {
                    ViewBag.MfaShow = true;
                    ViewBag.MfaMode = "blocked";
                    ViewBag.BloqueoHasta = mfa.BloqueoHasta.Value;
                    return View("InicioSesion", loginUsuario);
                }

                // Dispositivo confiable?
                var deviceId = Request.Cookies[CookieTrusted];
                if (!string.IsNullOrWhiteSpace(deviceId))
                {
                    var hash = _totp.HashDeviceId(deviceId);
                    var trusted = await _dbMfa.IsTrustedDeviceAsync(Usuario.Data.IdUsuario, hash);

                    if (trusted == 1)
                    {
                        await _dbMfa.P_Validacion_exitosa(Usuario.Data.IdUsuario, ip, Usuario.Data.Identificacion);

                        // ✅ finalizar login sin session intermedia
                        return await FinalizeMfaLoginInternal();
                    }
                }

                // Decide modal: enroll o verify
                ViewBag.MfaShow = true;

                bool debeEnrollar = (mfa.MfaHabilitado != 1) ||
                                    (mfa.RequireReenroll == 1) ||
                                    string.IsNullOrWhiteSpace(mfa.TotpSecretEnc);

                if (debeEnrollar)
                {
                    var issuer = "IRIS-P1";
                    var account = Usuario.Data.Usuario ?? Usuario.Data.Identificacion.ToString();

                    var (secretBase32, qrBase64) = _totp.GenerateEnrollmentQr(issuer, account);

                    // ✅ estos dos sí pueden ir en Session (solo para render del modal en esta misma navegación)
                    //HttpContext.Session.SetString(SessEnrollSecret, secretBase32);
                    //HttpContext.Session.SetString(SessEnrollQr, qrBase64);


                    TempData["MFA_ENROLL_SECRET"] = secretBase32;
                    TempData["MFA_ENROLL_QR"] = qrBase64;
                    TempData.Keep("MFA_ENROLL_SECRET");
                    TempData.Keep("MFA_ENROLL_QR");


                    ViewBag.MfaMode = "enroll";
                    ViewBag.MfaQrBase64 = qrBase64;
                    ViewBag.MfaManualKey = secretBase32;
                }
                else
                {
                    ViewBag.MfaMode = "verify";
                }

                return View("InicioSesion", loginUsuario);
            }

            // ============================================================
            // NO MFA -> flujo normal (menú, claims, SignIn)
            // ============================================================
            var menuNormal = Admin
                ? await _iDbAdministracion.F_GetMenu(1, Usuario.Data.Identificacion)
                : await _iDbAdministracion.F_GetMenu(0, Usuario.Data.Identificacion);

            HttpContext.Session.SetObject("ListaMenu", menuNormal.Data);

            var claims = BuildClaims(Usuario.Data);

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            await _iDbAdministracion.P_InsAuditoria(
                Convert.ToInt64(Usuario.Data.Identificacion),
                "Inicio Sesion",
                "Inicio sesion Sistema",
                Convert.ToInt64(Usuario.Data.Identificacion),
                HttpContext.Session.GetString("IpMaquina")
            );

            return RedirectToAction("Index", "Home");
        }

        // ============================================================
        // MFA - ENROLL CONFIRM (POST desde MODAL en el LOGIN)
        // ============================================================
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MfaEnrollConfirm(DtoCredenciales loginUsuario, string Code)
            {
            // Recuperar de TempData
            var pending = GetPendingFromTempData(keep: true);
            if (pending == null)
                return RedirectToAction("InicioSesion");

            //var secret = HttpContext.Session.GetString(SessEnrollSecret);
            //var qr = HttpContext.Session.GetString(SessEnrollQr);


            var secret = TempData["MFA_ENROLL_SECRET"] as string;
            var qr = TempData["MFA_ENROLL_QR"] as string;

            // Mantener vivos para reintentos
            TempData.Keep("MFA_ENROLL_SECRET");
            TempData.Keep("MFA_ENROLL_QR");


            // Mantener modal abierto
            ViewBag.MfaShow = true;
            ViewBag.MfaMode = "enroll";
            ViewBag.MfaManualKey = secret;
            ViewBag.MfaQrBase64 = qr;

            if (string.IsNullOrWhiteSpace(secret))
            {
                TempData["MfaError"] = "La sesión de enrolamiento expiró. Inicie sesión nuevamente.";
                return View("InicioSesion", loginUsuario);
            }

            if (string.IsNullOrWhiteSpace(Code))
            {
                TempData["MfaError"] = "Ingrese el código de 6 dígitos.";
                return View("InicioSesion", loginUsuario);
            }

            if (!_totp.ValidateCode(secret, Code))
            {
                TempData["MfaError"] = "Código inválido. Verifique la hora automática del celular y vuelva a intentar.";
                return View("InicioSesion", loginUsuario);
            }

            var enc = _totp.ProtectSecret(secret);
            await _dbMfa.P_Guardar_LlaveSecreta(pending.IdUsuario, enc, pending.Ip, pending.Identificacion);

            await _iDbAdministracion.P_InsAuditoria(
                pending.Identificacion,
                "MFA Enroll OK",
                "Usuario enroló TOTP",
                pending.Identificacion,
                pending.Ip
            );

            //HttpContext.Session.Remove(SessEnrollSecret);
            //HttpContext.Session.Remove(SessEnrollQr);

            TempData.Remove("MFA_ENROLL_SECRET");
            TempData.Remove("MFA_ENROLL_QR");


            // Luego de enrolar -> pasa a verify modal
            ViewBag.MfaShow = true;
            ViewBag.MfaMode = "verify";

            // Importante: TempData debe seguir vivo para Verify
            TempData.Keep(TdLoginUserData);
            TempData.Keep(TdMfaPending);

            return View("InicioSesion", loginUsuario);
        }

        // ============================================================
        // MFA - VERIFY CONFIRM (POST desde MODAL en el LOGIN)
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> MfaVerifyConfirm(string Code, bool RememberDevice)
        {
            var pending = GetPendingFromTempData(keep: true);
            var userData = GetUserFromTempData(keep: true);

            if (pending == null || userData == null)
            {
                ModelState.AddModelError("", "La sesión de verificación expiró. Inicie sesión nuevamente.");
                return View("InicioSesion", new DtoCredenciales());
            }

            // 2) Consultar MFA en BD
            var mfa = await _dbMfa.GetMfaAsync(pending.IdUsuario);

            if (mfa.BloqueoHasta.HasValue && mfa.BloqueoHasta.Value > DateTime.Now)
            {
                ModelState.AddModelError("", $"Usuario bloqueado temporalmente hasta {mfa.BloqueoHasta:yyyy-MM-dd HH:mm}.");
                return View("InicioSesion", new DtoCredenciales());
            }

            if (mfa.MfaHabilitado != 1 || string.IsNullOrWhiteSpace(mfa.TotpSecretEnc))
            {
                ModelState.AddModelError("", "El usuario no tiene MFA configurado. Debe enrolar primero.");
                ViewBag.MfaShow = true;
                ViewBag.MfaMode = "enroll";
                return View("InicioSesion", new DtoCredenciales());
            }

            // 3) Validar código
            var secretBase32 = _totp.UnprotectSecret(mfa.TotpSecretEnc);

            if (!_totp.ValidateCode(secretBase32, Code))
            {
                await _dbMfa.P_Intentos_Fallidos(pending.IdUsuario, pending.Ip, pending.Identificacion);
                await _iDbAdministracion.P_InsAuditoria(pending.Identificacion, "MFA Fail", "Código TOTP inválido", pending.Identificacion, pending.Ip);

                ModelState.AddModelError("", "Código inválido.");
                ViewBag.MfaShow = true;
                ViewBag.MfaMode = "verify";

                // mantener TempData para reintentos
                TempData.Keep(TdLoginUserData);
                TempData.Keep(TdMfaPending);

                return View("InicioSesion", new DtoCredenciales());
            }

            // 4) OK: marcar OK
            await _dbMfa.P_Validacion_exitosa(pending.IdUsuario, pending.Ip, pending.Identificacion);
            await _iDbAdministracion.P_InsAuditoria(pending.Identificacion, "MFA OK", "Validación TOTP exitosa", pending.Identificacion, pending.Ip);

            // 5) Recordar equipo (cookie + DB)
            if (RememberDevice)
            {
                var deviceId = Guid.NewGuid().ToString("N");
                var hash = _totp.HashDeviceId(deviceId);

                await _dbMfa.SaveTrustedDeviceAsync(pending.IdUsuario, hash, expiraDias: 15, pending.Ip, pending.Identificacion);

                Response.Cookies.Append(CookieTrusted, deviceId, new CookieOptions
                {
                    HttpOnly = true,
                    // En localhost http puede ser false. En prod https debería ser true.
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.Now.AddDays(15)
                });
            }

            // 6) Finalizar login (menú + claims + SignIn)
            HttpContext.Session.SetString("IpMaquina", pending.Ip ?? "0.0.0.0");

            var menu = AdminOrFromUser(userData)
                ? await _iDbAdministracion.F_GetMenu(1, userData.Identificacion)
                : await _iDbAdministracion.F_GetMenu(0, userData.Identificacion);

            HttpContext.Session.SetObject("ListaMenu", menu.Data);

            var claims = BuildClaims(userData);

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // 7) Limpieza TempData (ya no se necesita)
            TempData.Remove(TdLoginUserData);
            TempData.Remove(TdMfaPending);

            // 8) Entrar al sistema
            return RedirectToAction("Index", "Home");
        }

        // ============================================================
        // MFA - CANCEL
        // ============================================================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult MfaCancel()
        {
            TempData.Remove(TdLoginUserData);
            TempData.Remove(TdMfaPending);

            HttpContext.Session.Remove(SessEnrollSecret);
            HttpContext.Session.Remove(SessEnrollQr);

            return RedirectToAction("InicioSesion");
        }

        // ============================================================
        // (Opcional) Endpoint, pero ya no es necesario.
        // Si lo llamas desde trusted device, funciona también.
        // ============================================================
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> FinalizeMfaLogin()
        {
            var result = await FinalizeMfaLoginInternal();
            return result;
        }

        private async Task<IActionResult> FinalizeMfaLoginInternal()
        {
            var pending = GetPendingFromTempData(keep: false);
            var userData = GetUserFromTempData(keep: false);

            if (pending == null || userData == null)
                return RedirectToAction("InicioSesion");

            HttpContext.Session.SetString("IpMaquina", pending.Ip ?? "0.0.0.0");

            bool admin = AdminOrFromUser(userData);

            var menu = admin
                ? await _iDbAdministracion.F_GetMenu(1, userData.Identificacion)
                : await _iDbAdministracion.F_GetMenu(0, userData.Identificacion);

            HttpContext.Session.SetObject("ListaMenu", menu.Data);

            var claims = BuildClaims(userData);

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity)
            );

            await _iDbAdministracion.P_InsAuditoria(
                Convert.ToInt64(userData.Identificacion),
                "Inicio Sesion",
                "Inicio sesión Sistema (MFA OK)",
                Convert.ToInt64(userData.Identificacion),
                pending.Ip ?? "0.0.0.0"
            );

            TempData.Remove(TdLoginUserData);
            TempData.Remove(TdMfaPending);

            return RedirectToAction("Index", "Home");
        }

        // ============================================================
        // CERRAR SESIÓN
        // ============================================================
        [AllowAnonymous]
        public async Task<IActionResult> CerrarSesion()
        {
            try
            {
                long identificacion = 0;
                var idClaim = User.FindFirstValue("Identificacion");

                if (!string.IsNullOrEmpty(idClaim))
                    long.TryParse(idClaim, out identificacion);

                await _iDbAdministracion.P_InsAuditoria(
                    identificacion,
                    "Cierre Sesión",
                    "Cierre Sesión Sistema",
                    identificacion,
                    HttpContext.Session.GetString("IpMaquina")
                );
            }
            catch { }

            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("InicioSesion", "Cuenta");
        }

        [AllowAnonymous]
        public async Task<IActionResult> SesionExpirada()
        {
            return RedirectToAction("InicioSesion", "Cuenta", new { _mensaje = "" });
        }

        public ActionResult Perfil() => View();

        // ============================================================
        // HELPERS TempData
        // ============================================================
        private MfaPendingDto? GetPendingFromTempData(bool keep)
        {
            var json = TempData[TdMfaPending] as string;
            if (string.IsNullOrWhiteSpace(json)) return null;

            if (keep) TempData.Keep(TdMfaPending);

            try { return JsonConvert.DeserializeObject<MfaPendingDto>(json); }
            catch { return null; }
        }

        private DtoUsuario? GetUserFromTempData(bool keep)
        {
            var json = TempData[TdLoginUserData] as string;
            if (string.IsNullOrWhiteSpace(json)) return null;

            if (keep) TempData.Keep(TdLoginUserData);

            try { return JsonConvert.DeserializeObject<DtoUsuario>(json); }
            catch { return null; }
        }

        private static bool AdminOrFromUser(DtoUsuario userData)
            => userData.DtoUserRoles?.Any(x => x.IdRol == 1) == true;

        // ============================================================
        // HELPERS Claims
        // ============================================================
        private static List<Claim> BuildClaims(DtoUsuario userData)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userData.Usuario ?? ""),
                new Claim("Funcionario", userData.Funcionario ?? ""),
                new Claim("GradoNombre", (userData.GradAlfabetico ?? "") + " " + (userData.Nombres ?? "") + " " + (userData.ApellidosNombres ?? "")),
                new Claim("Identificacion", Convert.ToString(userData.Identificacion)),
                new Claim("IdUsuario", Convert.ToString(userData.IdUsuario)),
                new Claim("Cargo", Convert.ToString(userData.Cargo ?? "")),
                new Claim("IdUndeLabora", Convert.ToString(userData.IdUndeLaborando)),
                new Claim("Dependencia", Convert.ToString(userData.Dependencia ?? "")),
                new Claim("Fisica", Convert.ToString(userData.Fisica ?? "")),
                new Claim("Grado", Convert.ToString(userData.GradAlfabetico ?? "")),
                new Claim("Correo", Convert.ToString(userData.Correo ?? "")),
                new Claim("Celular", Convert.ToString(userData.Celular)),
                new Claim("Usuario", Convert.ToString(userData.Usuario ?? "")),
                new Claim("SituacionLaboral", Convert.ToString(userData.SituacionLaboral ?? "")),
            };

            if (userData.DtoUserRoles != null)
            {
                foreach (var rol in userData.DtoUserRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, Convert.ToString(rol.IdRol)));
                    claims.Add(new Claim(ClaimTypes.Actor, Convert.ToString(rol.Descripcion)));
                }
            }

            return claims;
        }



        [Authorize]
        public async Task<IActionResult> FotoPerfil()
        {
            try
            {
                // 1. Obtener identificación desde el claim
                var identificacionClaim = User.FindFirst("Identificacion")?.Value;

                if (string.IsNullOrWhiteSpace(identificacionClaim))
                {
                    return FotoPorDefecto();
                }

                if (!long.TryParse(identificacionClaim, out long identificacion))
                {
                    return FotoPorDefecto();
                }

                // 2. Llamar el servicio que trae la foto
                var foto_empl = await _iDbConsultasPIP.ObtenerFotoFuncinarioAsync(identificacion);

                if (!foto_empl.Estado || string.IsNullOrWhiteSpace(foto_empl.Respuesta))
                {
                    return FotoPorDefecto();
                }

                // 3. Convertir Base64 a bytes
                byte[] bytes;
                try
                {
                    bytes = Convert.FromBase64String(foto_empl.Respuesta);
                }
                catch
                {
                    return FotoPorDefecto();
                }

                // 4. Devolver como archivo de imagen
                return File(bytes, "image/jpeg");
            }
            catch
            {
                return FotoPorDefecto();
            }
        }

        private IActionResult FotoPorDefecto()
        {
            // Ruta a una imagen por defecto en wwwroot/images
            var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "Avatar.png");
            var bytes = System.IO.File.ReadAllBytes(ruta);
            return File(bytes, "image/png");
        }




        [Authorize]
        [HttpGet]
        public async Task<IActionResult> FotoFuncionario(long identificacion)
        {
            try
            {
                var foto_empl = await _iDbConsultasPIP.ObtenerFotoFuncinarioAsync(identificacion);

                if (!foto_empl.Estado || string.IsNullOrWhiteSpace(foto_empl.Respuesta))
                {
                    return FotoPorDefecto();
                }

                byte[] bytes;
                try
                {
                    bytes = Convert.FromBase64String(foto_empl.Respuesta);
                }
                catch
                {
                    return FotoPorDefecto();
                }

                return File(bytes, "image/jpeg");
            }
            catch
            {
                return FotoPorDefecto();
            }
        }



        // ============================================================
        // MFA - PERDÍ MI AUTENTICADOR
        // ============================================================
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MfaLostDevice()
        {
            //var pending = HttpContext.Session.GetObject<MfaStateDto>("MFA_PENDING");
            var pending = GetPendingFromTempData(keep: true);

            if (pending == null)
            {
                TempData["MfaError"] = "La sesión expiró. Inicie sesión nuevamente.";
                return RedirectToAction("InicioSesion");
            }

            // 🔐 Reset MFA controlado
            await _dbMfa.ResetMfaAsync(
                pending.IdUsuario,
                pending.Ip,
                pending.Identificacion
            );

            await _iDbAdministracion.P_InsAuditoria(
                pending.Identificacion,
                "MFA RESET",
                "Usuario reporta pérdida del autenticador. Se fuerza re-enrolamiento.",
                pending.Identificacion,
                pending.Ip
            );

            // Limpieza de sesión MFA
            HttpContext.Session.Remove("MFA_PENDING");
            HttpContext.Session.Remove("LOGIN_USERDATA");
            HttpContext.Session.Remove("MFA_ENROLL_SECRET");
            HttpContext.Session.Remove("MFA_ENROLL_QR");

            TempData["MfaInfo"] =
                "Se restableció la verificación en dos pasos. Inicie sesión nuevamente para activar su nuevo autenticador.";

            return RedirectToAction("InicioSesion");
        }




    }






}
