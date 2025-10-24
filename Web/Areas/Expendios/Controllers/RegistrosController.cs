using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Negocio.Gestion.Admin;
using Negocio.Gestion.Irisp1;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.Expendios;
using Negocio.Interfaz.General;
using Negocio.Interfaz.Irisp1;

namespace Web.Areas.Expendios.Controllers
{

    [Area("Expendios")]
    [Authorize(Roles = "1,2")]
    public class RegistrosController : Controller
    {


        private readonly IConfiguration _iConfiguration;
        private readonly IDbRegistroExpendio _iDbRegistroExpendio;

        private readonly IDbDominios _iDbDominios;


        public RegistrosController(IConfiguration iConfiguration,  IDbDominios iDbDominios, IDbRegistroExpendio iRegistroExpendio)
        {

            _iConfiguration = iConfiguration;
            _iDbDominios = iDbDominios;
            _iDbRegistroExpendio = iRegistroExpendio;
        }

        public async Task<ActionResult> Registros()
        {


            var ddlAnioIris = (await _iDbRegistroExpendio.F_GetAniosIrisP1()).Data.ToList();

            var anioActual = ddlAnioIris.Max(x => x.AnoIrisp1);

            //  Crea el SelectList con el año actual seleccionado por defecto
            ViewBag.ddlAnioIris = new SelectList(ddlAnioIris, "AnoIrisp1", "AnoIrisp1", anioActual);
            return View();
        }


        #region Métodos de Consulta
        [HttpGet]
        public async Task<IActionResult> F_GetInfoGrillas(Int32 V_Anio)
        {
            var resultado = await _iDbRegistroExpendio.F_GetInfoGrillas(V_Anio);

            if (resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = resultado.Data });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = resultado.Mensaje });

            }
        }

        #endregion

    }
}



