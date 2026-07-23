
using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Web.Models;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Negocio.Interfaz.Admin;
using Negocio.Interfaz.General;
using Negocio.Interfaz.Irisp1;

using System.Data;
using System.Security.Claims;
using Negocio.Gestion.Irisp1;


namespace Web.Areas.Irisp1.Controllers
{
    [Area("Irisp1")]
    [Authorize(Roles = "1,2,7")]
    public class SeguimientoController : Controller
    {
        #region Propiedades
        private readonly IDbAdministracion _iDbAdministracion;
        private readonly IDbSeguimientoIris _iDbSeguimientoIris;
        private readonly IDbFuncionarios _iDbFuncionarios;
        private readonly IConfiguration _configuration;
        private readonly IDbDominios _IDbDominios;
        private readonly ILogger<SeguimientoController> _logger;



        #endregion

        #region Constructor
        public SeguimientoController(IConfiguration iConfiguration, IDbAdministracion iDbAdministracion, IDbSeguimientoIris iDbSeguimientoIris, IDbFuncionarios iDbFuncionarios, IDbDominios idbDominios, ILogger<SeguimientoController> logger)
        {

            _iDbAdministracion = iDbAdministracion;
            _iDbSeguimientoIris = iDbSeguimientoIris;
            _iDbFuncionarios = iDbFuncionarios;
            _configuration = iConfiguration;
            _IDbDominios = idbDominios;
            _logger = logger;

        }
        #endregion
        public async Task<ActionResult> Seguimiento()
        {

            var Auditoria = await _iDbAdministracion.P_InsAuditoria(Convert.ToInt64(User.FindFirstValue("Identificacion")), "Ingreso Módulo", "Ingreso módulo IrisP1/Seguimiento", Convert.ToInt64(User.FindFirstValue("Identificacion")), HttpContext.Session.GetString("IpMaquina"));
            var ddlAnioIris = (await _iDbSeguimientoIris.F_GetAniosIrisP1()).Data.ToList();
            var anioActual = ddlAnioIris.Any() ? ddlAnioIris.Max(x => x.AnoIrisp1) : (int?)null;

            //  Crea el SelectList con el año actual seleccionado por defecto
            ViewBag.ddlAnioIris = new SelectList(ddlAnioIris, "AnoIrisp1", "AnoIrisp1", anioActual);

            ViewBag.ddlTipoUnidad = new SelectList((await _iDbSeguimientoIris.F_GetUnidadesSeguimiento()).Data?.OrderBy(x => x.DESCRIPCION_DEPENDENCIA), "SIGLA", "DESCRIPCION_DEPENDENCIA");
            ViewBag.ddlTipoUnidad2 = new SelectList((await _iDbSeguimientoIris.F_GetUnidadesSeguimiento()).Data?.OrderBy(x => x.DESCRIPCION_DEPENDENCIA), "SIGLA", "DESCRIPCION_DEPENDENCIA");
            ViewBag.ddlTipoDependencia = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.ddlTipoDependencia2 = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.ddlTipoTarea = new SelectList((await _IDbDominios.F_GetDominiosIris(51)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            var dominios = (await _IDbDominios.F_GetDominiosIris(58)).Data?.Where(x => x.Descripcion == "Aceptada" || x.Descripcion == "Rechazada").OrderBy(x => x.Descripcion).ToList();


            ViewBag.ddlTipoEvalTarea = new SelectList(dominios, "IdDominio", "Descripcion");


            return View();
        }



        #region Métodos de Consulta
        [HttpGet]
        public async Task<IActionResult> F_GetInfoGrillas(Int32 V_Anio)
        {
            var codigoUnidad = Convert.ToInt64(User.FindFirstValue("IdUndeLabora"));

            // 🔹 Obtener todos los roles del usuario separados por coma
            var rolesUsuario = string.Join(",",
                User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
            );

            var resultado = await _iDbSeguimientoIris.F_GetInfoGrillas(V_Anio, rolesUsuario, codigoUnidad);

            if (resultado.IdRespuesta > 0)
                return Json(new { success = true, data = resultado.Data });
            else
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = resultado.Mensaje });
        }


        [HttpGet]
        public async Task<IActionResult> F_GetResponsables(string V_CriminalidadId)
        {
            var resultado = await _iDbSeguimientoIris.F_GetResponsables(V_CriminalidadId);

            if (resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = resultado.Data, message = resultado.Mensaje });
            }
            else
            {
                return Json(new { success = false, data = new List<DtoIntegrantes>(), message = resultado.Mensaje });
            }
        }

        [HttpGet]
        public async Task<IActionResult> F_GetUnidadesPorSigla(string V_Sigla)
        {
            var dependencias = await _iDbSeguimientoIris.F_GetUnidadesPorSigla(V_Sigla);

            if (dependencias.IdRespuesta == 1 && dependencias.Data != null)
            {
                var resultado = dependencias.Data.Select(x => new
                {
                    Codigo = x.CONSECUTIVO,
                    Descripcion = x.DESCRIPCION_DEPENDENCIA,
                    Sigla = x.SIGLA
                }).ToList();

                return Json(resultado);
            }
            else
            {
                return Json(new List<object>());
            }
        }


        #endregion



        [HttpPost]
        public async Task<IActionResult> P_InsResponsable(DtoIrispCriminalidad Obj_Responsable)
        {


            try
            {
                var Resultado = await _iDbSeguimientoIris.P_InsResponsable(Obj_Responsable, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina"));

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
                _logger.LogError(ex, "Error en P_InsResponsable");
                return Json(new { success = false, data = 0, message = "Error: no fue posible guardar, intente nuevamente." });
            }

        }





        [HttpPost]
        public async Task<IActionResult> P_UpdUnidadResponsable(DtoIrispCriminalidad obj_responsableUpd)
        {


            try
            {
                var Resultado = await _iDbSeguimientoIris.P_UpdUnidadResponsable(obj_responsableUpd, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina"));

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
                _logger.LogError(ex, "Error en P_UpdUnidadResponsable");
                return Json(new { success = false, data = 0, message = "Error: no fue posible guardar, intente nuevamente." });
            }

        }


        [HttpPost]
        public async Task<IActionResult> P_FinalizarIris(DtoIrispCriminalidad obj)
        {
            try
            {
                var resultado = await _iDbSeguimientoIris.P_FinalizarIris(obj, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina"));

                if (resultado.IdRespuesta > 0)
                    return Json(new { success = true, message = resultado.Mensaje });
                else
                    return Json(new { success = false, message = resultado.Mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: no es posible actualizar. " + ex.Message });
            }
        }



        [HttpGet]
        public async Task<IActionResult> F_GetResponsablesTareasIris(string V_Criminalidad)
        {
            var resultado = await _iDbSeguimientoIris.F_GetResponsablesTareasIris(V_Criminalidad);

            if (resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = resultado.Data });
            }
            else
            {
                return Json(new { success = false, data = resultado.Data });
            }
        }



        [HttpPost]
        public async Task<IActionResult> P_DelUnidadResponsable(DtoIrispCriminalidad obj_DelResponsable)
        {


            try
            {
                var Resultado = await _iDbSeguimientoIris.P_DelUnidadResponsable(obj_DelResponsable, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina"));

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
                _logger.LogError(ex, "Error en P_DelUnidadResponsable");
                return Json(new { success = false, data = 0, message = "Error: no fue posible guardar, intente nuevamente." });
            }

        }




        [HttpPost]
        public async Task<IActionResult> P_EvalTarea(DtoIrispCriminalidad obj_EvalTarea)
        {


            try
            {
                var Resultado = await _iDbSeguimientoIris.P_EvalTarea(obj_EvalTarea, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina"));

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
                _logger.LogError(ex, "Error en P_EvalTarea");
                return Json(new { success = false, data = 0, message = "Error: no fue posible guardar, intente nuevamente." });
            }

        }


        [HttpPost]
        public async Task<IActionResult> P_ReasignarTarea(DtoTareasIris obj_ReasignarTarea)
        {


            try
            {
                var Resultado = await _iDbSeguimientoIris.P_ReasignarTarea(obj_ReasignarTarea, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina"));

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
                _logger.LogError(ex, "Error en P_ReasignarTarea");
                return Json(new { success = false, data = 0, message = "Error: no fue posible guardar, intente nuevamente." });
            }

        }
    }
}
