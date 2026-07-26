using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.Interfaz.General;

namespace Web.Controllers
{
    [Authorize]
    public class DominiosController : Controller
    {
        #region Propiedades
        private readonly IDbDominios _IDbDominios;
        #endregion
        #region Constructor
        public DominiosController(IDbDominios iDbDominios)
        {
            _IDbDominios = iDbDominios;
        }
        #endregion

        #region Métodos de Consulta
        public async Task<IActionResult> F_GetDominios(Int32 V_Id)
        {
            var Resultado = await _IDbDominios.F_GetDominios(V_Id);
            if (Resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = Resultado.Data, message = Resultado.Mensaje });
            }
            else
            {
                return Json(new { success = false, data = Resultado.Data, message = Resultado.Mensaje });
            }
        }

        public async Task<IActionResult> F_GetDepartamentos(Int32 V_Id)
        {
            var Resultado = await _IDbDominios.F_GetDepartamentos(V_Id);
            if (Resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = Resultado.Data, message = Resultado.Mensaje });
            }
            else
            {
                return Json(new { success = false, data = Resultado.Data, message = Resultado.Mensaje });
            }
        }

        public async Task<IActionResult> F_GetMunicipios(Int32 V_Id)
        {
            var Resultado = await _IDbDominios.F_GetMunicipios(V_Id);
            if (Resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = Resultado.Data, message = Resultado.Mensaje });
            }
            else
            {
                return Json(new { success = false, data = Resultado.Data, message = Resultado.Mensaje });
            }
        }

        public async Task<IActionResult> F_GetUnidadesPoliciales(Int32 V_Id)
        {
            var Resultado = await _IDbDominios.F_GetUnidadesPoliciales(V_Id);
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

        #region Métodos de Inserción


        #endregion

        #region Métodos de Actualización

        #endregion

    }
}
