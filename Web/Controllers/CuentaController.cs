using Comun.Areas.Admin;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.Interfaz.Admin;
using Newtonsoft.Json;
using System.Security.Claims;

// Usamos la librería
using Ponal.Seguridad.MfaCliente.Interfaces;
using Ponal.Seguridad.MfaCliente.Modelos.DtoMfa;

namespace Web.Controllers
{
    [Authorize]
    public class CuentaController : Controller
    {
        private readonly IHttpContextAccessor _iHttpContextAccessor;
        private readonly IDbAdministracion _iDbAdministracion;
        private readonly IDbConsultasPIP _iDbConsultasPIP;
        private readonly IConfiguration _iConfiguration;
        private readonly IDbMfaCentralWs _mfaWs;


        private const string TdLoginUserData = "LOGIN_USERDATA";
        private const string TdMfaPending = "MFA_PENDING";
        private const string CookieTrusted = "IRISP_MFA_DEVICE";

        public CuentaController(
           IHttpContextAccessor iHttpContextAccessor,
           IConfiguration iConfiguration,
           IDbAdministracion iDbAdministracion,
           IDbConsultasPIP idbConsultasPIP,
           IDbMfaCentralWs mfaWs)
        {
            _iHttpContextAccessor = iHttpContextAccessor;
            _iConfiguration = iConfiguration;
            _iDbAdministracion = iDbAdministracion;
            _iDbConsultasPIP = idbConsultasPIP;
            _mfaWs = mfaWs;
        }

        private bool MfaEnabled() =>
            _iConfiguration.GetValue<bool>("MfaCentral:Enabled");


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> InicioSesion(string? returnurl = null, string? _mensaje = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(_mensaje))
                {
                    string? key = _iConfiguration["Encryption:Key"];
                    if (string.IsNullOrEmpty(key)) return View("ErrorGeneral");

                    string mensajeRecibido = _iDbAdministracion.ConvertirBase64Bytes(_mensaje);
                    string mensajeDesencriptado = _iDbAdministracion.Decript(mensajeRecibido, key);

                    DtoCredencialesSisec? loginDTO = JsonConvert.DeserializeObject<DtoCredencialesSisec>(mensajeDesencriptado);
                    if (loginDTO == null) return View("ErrorGeneral");

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> InicioSesionAsync(DtoCredenciales loginUsuario, string? returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl ??= Url.Action(nameof(HomeController.Index), "Home") ?? "/Home";

            if (!ModelState.IsValid) return View("InicioSesion", loginUsuario);

            // Validación de credenciales contra OUD. OJO: respuestaOud.Respuesta == true significa
            // FALLO de autenticación (booleano invertido por el contrato del servicio PIP/OUD).
            var respuestaOud = await _iDbConsultasPIP.ObtenerOudAsync(loginUsuario);

            //if (!respuestaOud.Respuesta)
            //{
            //    ModelState.AddModelError("", "Usuario o Contraseña incorrecta, valide la información ingresada");
            //    return View("InicioSesion", loginUsuario);
            //}

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

            bool superUsuario = Usuario.Data.DtoUserRoles.Any(x => x.IdRol == 1);

            // ============================================================
            // ✅ MFA CENTRALIZADO 
            // ============================================================

            bool skipMfa = !MfaEnabled();

            if (!skipMfa)
            {
                var mfaState = await _mfaWs.StateAsync(Usuario.Data.Identificacion, Usuario.Data.Usuario ?? "");

                if (mfaState.CodigoExito != 1)
                {
                    var msg = mfaState.Mensaje ?? "No fue posible validar MFA en este momento.";
                    if (msg.StartsWith("MFA_SVC_DOWN|", StringComparison.OrdinalIgnoreCase))
                    {
                        ViewBag.MfaShow = true;
                        ViewBag.MfaMode = "svcdown";
                        ViewBag.MfaSvcMsg = msg.Replace("MFA_SVC_DOWN|", "");
                        return View("InicioSesion", loginUsuario);
                    }

                    ModelState.AddModelError("", $"No fue posible consultar estado MFA: {msg}");
                    return View("InicioSesion", loginUsuario);
                }

                // MFA es OPCIONAL por usuario (comportamiento por diseño de este sistema): solo se
                // aplica a quienes tienen MFA habilitado (MfaHabilitado == 1). Los usuarios NO
                // enrolados continúan al login normal, sin segundo factor.
                if (mfaState.Data?.MfaHabilitado == 1)
                {
                    // Datos que necesitan por igual el paso de verificación y el de re-enrolamiento.
                    TempData[TdLoginUserData] = JsonConvert.SerializeObject(Usuario.Data);
                    TempData[TdMfaPending] = JsonConvert.SerializeObject(new MfaPendingDto
                    {
                        IdUsuario = Usuario.Data.IdUsuario,
                        Identificacion = Usuario.Data.Identificacion,
                        Usuario = Usuario.Data.Usuario ?? Usuario.Data.Identificacion.ToString(),
                        Funcionario = Usuario.Data.Funcionario ?? "",
                        Ip = ip
                    });
                    TempData.Keep(TdLoginUserData);
                    TempData.Keep(TdMfaPending);

                    if (mfaState.Data?.BloqueoHasta.HasValue == true && mfaState.Data.BloqueoHasta.Value > DateTime.Now)
                    {
                        ViewBag.MfaShow = true;
                        ViewBag.MfaMode = "blocked";
                        ViewBag.BloqueoHasta = mfaState.Data.BloqueoHasta.Value;
                        return View("InicioSesion", loginUsuario);
                    }

                    var deviceId = Request.Cookies[CookieTrusted];
                    if (!string.IsNullOrWhiteSpace(deviceId))
                    {
                        var trustedResp = await _mfaWs.IsTrustedAsync(Usuario.Data.Identificacion, Usuario.Data.Usuario ?? "", deviceId);

                        if (trustedResp.CodigoExito != 1)
                        {
                            var tmsg = trustedResp.Mensaje ?? "";
                            if (tmsg.StartsWith("MFA_SVC_DOWN|", StringComparison.OrdinalIgnoreCase))
                            {
                                ViewBag.MfaShow = true; ViewBag.MfaMode = "svcdown"; ViewBag.MfaSvcMsg = tmsg.Replace("MFA_SVC_DOWN|", "");
                                return View("InicioSesion", loginUsuario);
                            }
                            ModelState.AddModelError("", $"No fue posible validar equipo confiable: {tmsg}");
                            return View("InicioSesion", loginUsuario);
                        }

                        if (trustedResp.CodigoExito == 1 && trustedResp.Data == 1)
                        {
                            return await FinalizeMfaLoginInternal();
                        }
                    }

                    ViewBag.MfaShow = true;

                    // Usuario enrolado: normalmente verifica su código TOTP; si el servicio pide
                    // re-enrolar (RequireReenroll), se muestra el QR de enrolamiento.
                    bool debeReenrolar = mfaState.Data?.RequireReenroll == 1;

                    if (debeReenrolar)
                    {
                        var enrollStart = await _mfaWs.EnrollStartAsync(Usuario.Data.Identificacion, Usuario.Data.Usuario ?? "");

                        if (enrollStart.CodigoExito != 1 || enrollStart.Data == null)
                        {
                            var emsg = enrollStart.Mensaje ?? "";
                            if (emsg.StartsWith("MFA_SVC_DOWN|", StringComparison.OrdinalIgnoreCase))
                            {
                                ViewBag.MfaMode = "svcdown"; ViewBag.MfaSvcMsg = emsg.Replace("MFA_SVC_DOWN|", "");
                                return View("InicioSesion", loginUsuario);
                            }

                            ModelState.AddModelError("", $"No fue posible iniciar enrolamiento MFA: {enrollStart.Mensaje}");
                            return View("InicioSesion", loginUsuario);
                        }

                        ViewBag.MfaMode = "enroll";
                        ViewBag.MfaQrBase64 = enrollStart.Data.QrBase64;
                        ViewBag.MfaManualKey = enrollStart.Data.ManualKey;

                        TempData["MFA_ENROLL_TOKEN"] = enrollStart.Data.EnrollToken;
                        TempData.Keep("MFA_ENROLL_TOKEN");
                    }
                    else
                    {
                        ViewBag.MfaMode = "verify";
                    }

                    return View("InicioSesion", loginUsuario);
                }
                // Usuario sin MFA habilitado -> continúa al login normal (MFA opcional por usuario).
            }
            // Flujo normal sin MFA (solo cuando MFA global está deshabilitado)
            var menuNormal = superUsuario
                ? await _iDbAdministracion.F_GetMenu(1, Usuario.Data.Identificacion)
                : await _iDbAdministracion.F_GetMenu(0, Usuario.Data.Identificacion);

            HttpContext.Session.SetObject("ListaMenu", menuNormal.Data);
            var claims = BuildClaims(Usuario.Data);
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            await _iDbAdministracion.P_InsAuditoria(
                Convert.ToInt64(Usuario.Data.Identificacion), "Inicio Sesion", "Inicio sesion Sistema", Convert.ToInt64(Usuario.Data.Identificacion), HttpContext.Session.GetString("IpMaquina")
            );

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> FinalizeMfaLogin()
        {
            var pending = GetPendingFromTempData(keep: false);
            var userData = GetUserFromTempData(keep: false);

            if (pending == null || userData == null)
            {
                TempData["MfaError"] = "La sesión expiró. Inicie sesión nuevamente.";
                return RedirectToAction("InicioSesion");
            }

            var mfaPassed = HttpContext.Session.GetString($"MFA_PASSED_{pending.Identificacion}");

            if (mfaPassed == "true")
            {
                HttpContext.Session.Remove($"MFA_PASSED_{pending.Identificacion}");
            }
            else
            {
                // Protección extra: Si intenta entrar aquí sin la bandera del orquestador, lo sacamos.
                return RedirectToAction("InicioSesion");
            }

            return await FinalizeMfaLoginInternal(pending, userData);
        }

        private async Task<IActionResult> FinalizeMfaLoginInternal(MfaPendingDto? pending = null, DtoUsuario? userData = null)
        {
            if (pending == null) pending = GetPendingFromTempData(keep: false);
            if (userData == null) userData = GetUserFromTempData(keep: false);

            if (pending == null || userData == null)
                return RedirectToAction("InicioSesion");

            HttpContext.Session.SetString("IpMaquina", pending.Ip ?? "0.0.0.0");

            bool superUsuario = SuperUsuarioOrFromUser(userData);

            var menu = superUsuario
                ? await _iDbAdministracion.F_GetMenu(1, userData.Identificacion)
                : await _iDbAdministracion.F_GetMenu(0, userData.Identificacion);

            HttpContext.Session.SetObject("ListaMenu", menu.Data);

            var claims = BuildClaims(userData);
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            string ipMaquina = pending.Ip ?? "0.0.0.0";
            await _iDbAdministracion.P_InsAuditoria(
                Convert.ToInt64(userData.Identificacion), "Inicio Sesion", "Inicio sesión Sistema (MFA OK)", Convert.ToInt64(userData.Identificacion), ipMaquina
            );

            TempData.Remove(TdLoginUserData);
            TempData.Remove(TdMfaPending);

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public async Task<IActionResult> CerrarSesion()
        {
            try
            {
                long identificacion = 0;
                var idClaim = User.FindFirstValue("Identificacion");

                if (!string.IsNullOrEmpty(idClaim))
                    long.TryParse(idClaim, out identificacion);

                string ipMaquina = HttpContext.Session.GetString("IpMaquina") ?? "0.0.0.0";
                await _iDbAdministracion.P_InsAuditoria(identificacion, "Cierre Sesión", "Cierre Sesión Sistema", identificacion, ipMaquina);
            }
            catch { }

            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("InicioSesion", "Cuenta");
        }

        [AllowAnonymous]
        public IActionResult SesionExpirada()
        {
            return RedirectToAction("InicioSesion", "Cuenta", new { _mensaje = "" });
        }

        public ActionResult Perfil() => View();

        [Authorize]
        public async Task<IActionResult> FotoPerfil()
        {
            try
            {
                var identificacionClaim = User.FindFirst("Identificacion")?.Value;
                if (string.IsNullOrWhiteSpace(identificacionClaim) || !long.TryParse(identificacionClaim, out long identificacion))
                    return FotoPorDefecto();

                var foto_empl = await _iDbConsultasPIP.ObtenerFotoFuncinarioAsync(identificacion);

                if (!foto_empl.Estado || string.IsNullOrWhiteSpace(foto_empl.Respuesta))
                    return FotoPorDefecto();

                byte[] bytes;
                try { bytes = Convert.FromBase64String(foto_empl.Respuesta); }
                catch { return FotoPorDefecto(); }

                return File(bytes, "image/jpeg");
            }
            catch { return FotoPorDefecto(); }
        }

        private IActionResult FotoPorDefecto()
        {
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
                    return FotoPorDefecto();

                byte[] bytes;
                try { bytes = Convert.FromBase64String(foto_empl.Respuesta); }
                catch { return FotoPorDefecto(); }

                return File(bytes, "image/jpeg");
            }
            catch { return FotoPorDefecto(); }
        }

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

        private static bool SuperUsuarioOrFromUser(DtoUsuario userData)
            => userData.DtoUserRoles?.Any(x => x.IdRol == 1) == true;

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
    }
}