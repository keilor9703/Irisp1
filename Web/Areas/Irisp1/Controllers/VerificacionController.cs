using Comun.Areas.Clientes;
using Comun.Areas.Mod_Uno;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.Clientes;
using Negocio.Interfaz.General;
using Negocio.Interfaz.Modulo1;
using System.Security.Claims;

namespace Web.Areas.Irisp1.Controllers
{
    [Area("Irisp1")]
    [Authorize(Roles = "1,2")]
    public class VerificacionController : Controller
    {
        public IActionResult Verificacion()
        {
            return View();
        }
    }
}
