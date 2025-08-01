using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.Interfaz.Admin;
using System.Security.Claims;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "1,2")]
    public class FormsBaseController : Controller
    {
        #region Propiedades
        private readonly IDbAdministracion _iDbAdministracion;
        #endregion

        public FormsBaseController(IDbAdministracion iDbAdministracion)
        {
            _iDbAdministracion = iDbAdministracion;
        }

        public IActionResult Formularios()
        {
            var Funcionario = User.FindFirstValue("Funcionario");
            var IpMaquina = HttpContext.Session.GetString("IpMaquina");

            var roles = ((ClaimsIdentity)User.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);

            return View();
        }
        public async Task<IActionResult> F_GetDatos(Int64 V_Identificacion)
        {
            var retorno = await _iDbAdministracion.F_GetSilerSuperior();

            if (retorno != null)
            {
                return Json(new { success = true, data = retorno.Data });
            }
            else
            {
                return Json(new { success = false, data = retorno.Data });
            }
        }
    }
}
