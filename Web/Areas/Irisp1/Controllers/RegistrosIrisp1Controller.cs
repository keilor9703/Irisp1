using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Negocio.Gestion.Admin;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.Irisp1;
using Oracle.ManagedDataAccess.Client;


namespace Web.Areas.Irisp1.Controllers
{
    [Area("Irisp1")]
    [Authorize(Roles = "1,2")]
    public class RegistrosIrisp1Controller : Controller
    {
        #region Propiedades

        private readonly IDbAdministracion _iDbAdministracion;
        private readonly IDbIrisp1 _iDbIrisp1;
        private readonly IDbFuncionarios _iDbFuncionarios;
        private readonly IConfiguration _configuration;

        #endregion

        #region Constructor

        public RegistrosIrisp1Controller(IDbAdministracion iDbAdministracion, IDbIrisp1 iDbIrisp1, IDbFuncionarios iDbFuncionarios, IConfiguration configuration)
        {
            _iDbAdministracion = iDbAdministracion;
            _iDbIrisp1 = iDbIrisp1;
            _iDbFuncionarios = iDbFuncionarios;
            _configuration = configuration;
        }

        #endregion

        public async Task<ActionResult> RegistrosIrisp1()
        {
            var ddlAnioIris = (await _iDbIrisp1.F_GetAniosIrisP1()).Data.ToList();
            ViewBag.ddlAnioIris = new SelectList(ddlAnioIris, "AnoIrisp1", "AnoIrisp1");
            ViewBag.ddlCanales = new SelectList(Enumerable.Empty<SelectListItem>());
            return View();
        }

        #region Métodos de Consulta

        [HttpGet]
        public async Task<IActionResult> F_GetEstadosIrisP1()
        {
            var resultado = await _iDbIrisp1.F_GetEstadosIrisP1();

            if (resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = resultado.Data });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [HttpGet]
        public async Task<IActionResult> F_GetInfoGrillas(Int32 V_Anio)
        {
            var resultado = await _iDbIrisp1.F_GetInfoGrillas(V_Anio);

            if (resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = resultado.Data });
            }
            else
            {
                return Json(new { success = false });
            }
        }



        [HttpGet]
        public async Task<IActionResult> F_GetCuadrantes(string V_unidadLabora)
        {
            var resultado = await _iDbIrisp1.F_GetCuadrantes(V_unidadLabora);

            if (resultado.IdRespuesta > 0)
            {
                return Json(resultado.Data); // Solo la lista
            }
            else
            {
                return Json(new { success = false });
            }
        }


        #endregion
    }
}