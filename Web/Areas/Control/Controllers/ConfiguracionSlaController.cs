using Comun.Areas.Control;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.Control;
using Negocio.Interfaz.General;
using System.Security.Claims;

namespace Web.Areas.Control.Controllers
{
    [Area("Control")]
    [Authorize(Roles = "1,2")]
    [AutoValidateAntiforgeryToken]
    public class ConfiguracionSlaController : Controller
    {
        #region Propiedades
        private readonly IDbAdministracion _iDbAdministracion;
        private readonly IDbControlGestion _iDbControlGestion;
        private readonly IDbDominios _iDbDominios;
        private readonly ILogger<ConfiguracionSlaController> _logger;
        #endregion

        #region Constructor
        public ConfiguracionSlaController(IDbAdministracion iDbAdministracion, IDbControlGestion iDbControlGestion,
            IDbDominios iDbDominios, ILogger<ConfiguracionSlaController> logger)
        {
            _iDbAdministracion = iDbAdministracion;
            _iDbControlGestion = iDbControlGestion;
            _iDbDominios = iDbDominios;
            _logger = logger;
        }
        #endregion

        public async Task<ActionResult> VwConfiguracionSla()
        {
            await _iDbAdministracion.P_InsAuditoria(
                Convert.ToInt64(User.FindFirstValue("Identificacion")), "Ingreso Módulo", "Ingreso módulo Control/ConfiguracionSla",
                Convert.ToInt64(User.FindFirstValue("Identificacion")), HttpContext.Session.GetString("IpMaquina"));

            // Dominio 51 = Lista de tareas (mismo catálogo que usa SeguimientoController.ddlTipoTarea)
            ViewBag.ddlTipoTarea = new SelectList((await _iDbDominios.F_GetDominiosIris(51)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");

            return View();
        }

        #region Métodos de Consulta

        [HttpGet]
        public async Task<IActionResult> F_GetSlaConfig()
        {
            var resultado = await _iDbControlGestion.F_GetSlaConfig();

            if (resultado.IdRespuesta > 0)
                return Json(new { success = true, data = resultado.Data });

            return Json(new { success = false, data = resultado.Data, message = resultado.Mensaje });
        }

        #endregion

        #region Métodos de Inserción/Actualización/Eliminación

        [HttpPost]
        public async Task<IActionResult> P_InsUpdSlaConfig(DtoSlaConfig obj)
        {
            if (obj.IdListaTareas == 0)
                return Json(new { success = false, message = "Debe seleccionar el tipo de tarea, revise" });

            if (obj.TiempoAlertaHoras <= 0 || obj.TiempoMaximoHoras <= 0)
                return Json(new { success = false, message = "Debe registrar tiempos de alerta y máximo mayores a cero, revise" });

            if (obj.TiempoAlertaHoras >= obj.TiempoMaximoHoras)
                return Json(new { success = false, message = "El tiempo de alerta debe ser menor al tiempo máximo, revise" });

            try
            {
                var resultado = await _iDbControlGestion.P_InsUpdSlaConfig(
                    obj.IdListaTareas, obj.TiempoAlertaHoras, obj.TiempoMaximoHoras, obj.Justificacion,
                    Convert.ToInt64(User.FindFirstValue("Identificacion")), HttpContext.Session.GetString("IpMaquina") ?? "");

                if (resultado.IdRespuesta > 0)
                    return Json(new { success = true, data = resultado.Data, message = resultado.Mensaje });

                return Json(new { success = false, data = resultado.Data, message = resultado.Mensaje });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en P_InsUpdSlaConfig");
                return Json(new { success = false, data = 0, message = "Error: no fue posible guardar, intente nuevamente." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> P_DelSlaConfig(string slaConfigId)
        {
            if (string.IsNullOrWhiteSpace(slaConfigId))
                return Json(new { success = false, message = "Debe seleccionar un registro, revise" });

            try
            {
                var resultado = await _iDbControlGestion.P_DelSlaConfig(
                    slaConfigId, Convert.ToInt64(User.FindFirstValue("Identificacion")), HttpContext.Session.GetString("IpMaquina") ?? "");

                if (resultado.IdRespuesta > 0)
                    return Json(new { success = true, data = resultado.Data, message = resultado.Mensaje });

                return Json(new { success = false, data = resultado.Data, message = resultado.Mensaje });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en P_DelSlaConfig");
                return Json(new { success = false, data = 0, message = "Error: no fue posible eliminar, intente nuevamente." });
            }
        }

        #endregion
    }
}
