using Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.Interfaz.Admin;

namespace Web.Controllers
{
    [Authorize]
    public class EmpleadosController : Controller
    {
        #region Propiedades
        private readonly IDbFuncionarios _iDbFuncionarios;
        #endregion
        #region Constructor
        public EmpleadosController(IDbFuncionarios iDbFuncionarios)
        {
            _iDbFuncionarios = iDbFuncionarios;
        }
        #endregion

        #region Métodos de Consulta
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> F_GetFuncionarios(Int64 V_Identificacion)
        {
            var Resultado = await _iDbFuncionarios.F_GetFuncionarios(V_Identificacion);
            if (Resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = Resultado.Data, message = Resultado.Mensaje, IdEncry = ClsEncriptar.Encriptar(Convert.ToString(V_Identificacion)) });
            }
            else
            {
                return Json(new { success = false, data = Resultado.Data, message = Resultado.Mensaje });
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> F_GetEmpleadoIntel(string V_Busqueda)
        {
            var Resultado = await _iDbFuncionarios.F_GetEmpleadoIntel(V_Busqueda);
            if (Resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = Resultado.Data, message = Resultado.Mensaje });
            }
            else
            {
                return Json(new { success = false, data = Resultado.Data, message = Resultado.Mensaje });
            }
        }
        #endregion

    }
}
