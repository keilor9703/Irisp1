using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.Clientes;
using Negocio.Interfaz.General;
using System.Security.Claims;

namespace Web.Areas.Mod_Uno.Controllers
{
    [Area("Mod_Uno")]
    [Authorize(Roles = "1,2,3")]

    public class RegistrosController : Controller
    {
        #region Propiedades
        private readonly IDbAdministracion _iDbAdministracion;
        private readonly IDbDominios _IDbDominios;
        private readonly IDbClientes _IDbClientes;
        #endregion

        #region Constructor

        public RegistrosController(IDbClientes iDbClientes, IDbAdministracion iDbAdministracion, IDbDominios iDbDominios)
        {
            _iDbAdministracion = iDbAdministracion;
            _IDbClientes = iDbClientes;
            _IDbDominios = iDbDominios;
        }

        #endregion
        public IActionResult RegistrosIris()

        {

            var Funcionario = User.FindFirstValue("Funcionario");
            var IpMaquina = HttpContext.Session.GetString("IpMaquina");

            var roles = ((ClaimsIdentity)User.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);

            return View();
        }
        public async Task<IActionResult> F_GetDatos(long V_Identificacion)
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