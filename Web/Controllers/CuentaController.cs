using Comun.Areas.Admin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Negocio.Interfaz.Admin;
using Newtonsoft.Json;
using System.Security.Claims;
using Web;
using Web.Models;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class CuentaController : Controller
    {
        private readonly IHttpContextAccessor _iHttpContextAccessor;
        private readonly IDbAdministracion _iDbAdministracion;
        private readonly IDbConsultasPIP _iDbConsultasPIP;
        private readonly IConfiguration _iConfiguration;

        bool Admin = false;
        public CuentaController(IHttpContextAccessor iHttpContextAccessor,
                                IConfiguration iConfiguration,
                                IDbAdministracion iDbAdministracion,
                                IDbConsultasPIP idbConsultasPIP)
        {

            _iHttpContextAccessor = iHttpContextAccessor;
            _iConfiguration = iConfiguration;
            _iDbAdministracion = iDbAdministracion;
            _iDbConsultasPIP = idbConsultasPIP;
        }


        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult InicioSesion(string returnurl = "nullhttps://sisec.policia.gov.co/IRIS/Home/Index")
        //{
        //    ViewData["ReturnUrl"] = returnurl;
        //    return View();
        //}



        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> InicioSesion(string returnurl = null, string _mensaje = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(_mensaje))
                {

                    /// PARA INICIO DESDE SISEC
                    string key = _iConfiguration["Encryption:Key"];

                    // 1. Base64 → texto
                    string mensajeRecibido = _iDbAdministracion.ConvertirBase64Bytes(_mensaje);

                    // 2. Desencriptar mensaje
                    string mensajeDesencriptado = _iDbAdministracion.Decript(
                        mensajeRecibido,
                        key
                    );

                    // 3. Pasar JSON a modelo
                    DtoCredencialesSisec loginDTO =
                        JsonConvert.DeserializeObject<DtoCredencialesSisec>(mensajeDesencriptado);

                    // 4. Desencriptar clave interna
                    string claveBase64 = _iDbAdministracion.ConvertirBase64Bytes(loginDTO.Clave);
                    string claveDesencriptada = _iDbAdministracion.Decript(claveBase64, key);

                    // 5. Construir DTO para tu login actual
                    var loginUsuario = new DtoCredenciales
                    {
                        UsuarioEmpresarial = loginDTO.Usuario,
                        ClaveEmpresarial = claveDesencriptada
                    };

                    // 6. Ejecutar login normal
                    return await InicioSesionAsync(loginUsuario, returnurl);
                }

                // SIN MENSAJE → formulario normal
                ViewData["ReturnUrl"] = returnurl;
                return View("InicioSesion");
            }
            catch
            {
                return View("ErrorGeneral");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> InicioSesionAsync(DtoCredenciales loginUsuario, string returnurl = null)



        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Action(nameof(HomeController.Index), "Home"); //Url.Content("~/Home/Index");

            if (!ModelState.IsValid)
                return View(loginUsuario);

            //Deshabilitar el OUD 
            var respuestaOud = await _iDbConsultasPIP.ObtenerOudAsync(loginUsuario);

            if (!respuestaOud.Respuesta)
            {
                ModelState.AddModelError("", "Usuario o Contraseña incorrecta, valide la información ingresada");
                return View();
            }

            //Obtener IP
            var Ip = _iHttpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            HttpContext.Session.SetString("IpMaquina", Ip);

            var Usuario = await _iDbAdministracion.P_GetValidaUser(loginUsuario.UsuarioEmpresarial, Ip);


            if (Usuario.Data.Identificacion != 0)
            {
             
                //Validar si el usuario está bloqueado

                if (Usuario.Data.Bloqueado == 1)
                {
                    ModelState.AddModelError("", "Su cuenta de usuario está DESHABILITADA, contacte al Administrador");
                    return View();
                }

                if (Usuario.Data.DtoUserRoles.Count == 0)
                {
                    ModelState.AddModelError("", "Su cuenta de usuario no tiene roles asignados para ingresar a este sistema, contacte al Administrador");
                    return View();
                }

                Admin = Usuario != null ? Usuario.Data.DtoUserRoles.Any(x => x.IdRol == 1) : false;

                if (Admin)
                {
                    //Generamos el Menú Super usuario
                    var Menu = await _iDbAdministracion.F_GetMenu("1", Usuario.Data.Identificacion);
                    HttpContext.Session.SetObject("ListaMenu", Menu.Data);
                }
                else
                {
                    var Menu = await _iDbAdministracion.F_GetMenu("0", Usuario.Data.Identificacion);
                    HttpContext.Session.SetObject("ListaMenu", Menu.Data);
                }


                //generamos los claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Usuario.Data.Usuario),
                    new Claim("Funcionario", Usuario.Data.Funcionario),
                    new Claim("GradoNombre", Usuario.Data.GradAlfabetico + " " + Usuario.Data.Nombres + " " + Usuario.Data.ApellidosNombres),
                    new Claim("Identificacion", Convert.ToString(Usuario.Data.Identificacion)),
                    new Claim("IdUsuario", Convert.ToString(Usuario.Data.IdUsuario)),
                    new Claim("Cargo", Convert.ToString(Usuario.Data.Cargo)),
                    new Claim("IdUndeLabora", Convert.ToString(Usuario.Data.IdUndeLaborando)),
                    new Claim("Dependencia", Convert.ToString(Usuario.Data.Dependencia)),
                    new Claim("Fisica", Convert.ToString(Usuario.Data.Fisica)),
                    new Claim("Grado", Convert.ToString(Usuario.Data.GradAlfabetico)),
                    new Claim("Correo", Convert.ToString(Usuario.Data.Correo)),
                    new Claim("Celular", Convert.ToString(Usuario.Data.Celular)),
                    new Claim("Usuario", Convert.ToString(Usuario.Data.Usuario)),
                    new Claim("SituacionLaboral", Convert.ToString(Usuario.Data.SituacionLaboral)),
                  //  new Claim("FotoBase64", foto_empl.Respuesta.Respuesta ?? ""),
                    //new Claim("FotoBase64", fotoBase64),


                };

                foreach (var rol in Usuario.Data.DtoUserRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, Convert.ToString(rol.IdRol)));
                    claims.Add(new Claim(ClaimTypes.Actor, Convert.ToString(rol.Descripcion)));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                //Auditoria Inicio de Sesion
                var Auditoria = _iDbAdministracion.P_InsAuditoria(Convert.ToInt64(Usuario.Data.Identificacion), "Inicio Sesion", "Inicio sesion Sistema", Convert.ToInt64(Usuario.Data.Identificacion), HttpContext.Session.GetString("IpMaquina"));

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Usuario no se encuentra registrado");
                return View();
            }
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

                await _iDbAdministracion.P_InsAuditoria(
                    identificacion,
                    "Cierre Sesión",
                    "Cierre Sesión Sistema",
                    identificacion,
                    HttpContext.Session.GetString("IpMaquina")
                );
            }
            catch { }

            // Limpieza total
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // ✅ REDIRECCIÓN CORRECTA
            return RedirectToAction("InicioSesion", "Cuenta");
        }




        public ActionResult Perfil() => View();


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

        

    }


}

