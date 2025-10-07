using Comun.Areas.Admin;
using Gepad.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.Interfaz.Admin;
using System.Security.Claims;
using Web;

namespace Gepad.Controllers
{
    [Authorize]
    public class CuentaController : Controller
    {
        private readonly IHttpContextAccessor _iHttpContextAccessor;
        private readonly IGestionToken _iGestionToken;
        private readonly IGestionOUD _iGestionOUD;
        private readonly IDbAdministracion _iDbAdministracion;

        bool cotiene = false;
        public CuentaController(IGestionToken iGestionToken,
                                IGestionOUD iGestionOUD,
                                IHttpContextAccessor iHttpContextAccessor,
                                IDbAdministracion iDbAdministracion
                                )
        {
            _iGestionToken = iGestionToken;
            _iGestionOUD = iGestionOUD;
            _iHttpContextAccessor = iHttpContextAccessor;
            _iDbAdministracion = iDbAdministracion;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult InicioSesion(string returnurl ="nullhttps://disec.policia.gov.co/Irisp1/Home/Indexl") 
        {
            ViewData["ReturnUrl"] = returnurl;
            return View();
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
          //var respuestaOud = await _iGestionOUD.ObtenerOudAsync(loginUsuario);

          //  if (!respuestaOud.Respuesta)
          //  {
          //      ModelState.AddModelError("", "Usuario o Contraseña incorrecta, revise");
          //      return View();
          //  }

            //Obtener IP
            var Ip = _iHttpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            HttpContext.Session.SetString("IpMaquina", Ip);

            var Usuario = await _iDbAdministracion.P_GetValidaUser(loginUsuario.UsuarioEmpresarial, Ip);


            if (Usuario.Data.Identificacion != 0)
            {
                //Cargar Foto
                var foto_empl = "https://sinac.policia.gov.co:8443" + "/SinacPicture/picture.aspx?DocID=" + ClsEncriptar.Encriptar(Convert.ToString(Usuario.Data.Identificacion)) + "&Token=Mxl7995Julabdfjughyts1*_58$$";

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

                cotiene = Usuario != null ? Usuario.Data.DtoUserRoles.Any(x => x.IdRol == 1) : false;

                if (cotiene)
                {
                    //Generamos el Menú Administrador
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
                };

                foreach (var rol in Usuario.Data.DtoUserRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, Convert.ToString(rol.IdRol)));
                    claims.Add(new Claim(ClaimTypes.Actor, Convert.ToString(rol.Descripcion)));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                //Auditoria Inicio de Sesion
                var Auditoria = _iDbAdministracion.P_InsAuditoria(Convert.ToInt64(Usuario.Data.Identificacion), "InicioSesion", "Inicio sesion Sistema", Convert.ToString(Usuario.Data.Identificacion), HttpContext.Session.GetString("IpMaquina"));

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Usuario no se encuentra registrado");
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> CerrarSesion()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(InicioSesion));
        }

        public ActionResult Perfil() => View();

    }
}
