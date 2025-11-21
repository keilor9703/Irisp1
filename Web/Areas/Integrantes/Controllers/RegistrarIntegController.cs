using Comun.Areas.Expendios;
using Comun.Areas.Integrantes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Negocio.Interfaz.General;
using Negocio.Interfaz.Integrantes;
using System.Security.Claims;

namespace Web.Areas.Integrantes.Controllers
{
    [Area("Integrantes")]
    [Authorize(Roles = "1,2,3,4,8,11")]
    public class RegistrarIntegController : Controller

    {
        private readonly IConfiguration _iConfiguration;
        private readonly IDbRegistroInteg _iDbRegistroInteg;
        private readonly IDbDominios _iDbDominios;


        public RegistrarIntegController(IConfiguration iConfiguration, IDbRegistroInteg dbRegistroInteg, IDbDominios iDbDominios)
        {
            _iConfiguration = iConfiguration;
            _iDbRegistroInteg = dbRegistroInteg;
            _iDbDominios = iDbDominios;
        }


        public async Task<ActionResult> RegistrarInteg()
        {

            ViewBag.ddlTipoReincidencia = new SelectList((await _iDbDominios.F_GetDominiosIris(110)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");


            return View();
        }


        [HttpPost]
        public async Task<IActionResult> F_GetReincidentes()
        {
            var resultado = await _iDbRegistroInteg.F_GetReincidentes();

            if (resultado.IdRespuesta > 0)
            {
                return Json(new
                {
                    success = true,
                    data = resultado.Data
                });
            }
            else
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = resultado.Mensaje
                });
            }
        }




        [HttpGet]
        public async Task<IActionResult> F_GetReincidentesPorId(Int64 V_Identificacion)
        {
            var resultado = await _iDbRegistroInteg.F_GetReincidentesPorId(V_Identificacion);

            if (resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = resultado.Data });
            }
            else
            {
                return Json(new { success = false, message = resultado.Mensaje });


            }
        }




        [HttpPost]
        public async Task<IActionResult> P_InsOrUpdReincidente(DtoReincidentes Obj_Reincidente)
        {

            try
            {
                var Resultado = await _iDbRegistroInteg.P_InsOrUpdReincidente(Obj_Reincidente, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina"));

                if (Resultado.IdRespuesta > 0)
                {
                    return Json(new { success = true, data = Resultado.Data, message = Resultado.Mensaje });
                }
                else
                {
                    return Json(new { success = false, data = Resultado.Data, message = Resultado.Mensaje });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = 0, message = "Error: no es posible guardar, revise " + ex });
            }

        }





    }
}
