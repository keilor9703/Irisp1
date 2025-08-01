using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.Interfaz.Irisp1;
using System.Security.Claims;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "1,2")]
    public class FormsBaseController : Controller
    {
        #region Propiedades
        private readonly IDbIrisp1 _iDbAdministracion;
        #endregion

        public FormsBaseController(IDbIrisp1 iDbAdministracion)
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
    }
}
