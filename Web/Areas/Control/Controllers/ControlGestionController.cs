using Comun.Areas.Control;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.Control;
using Negocio.Interfaz.Irisp1;
using System.Security.Claims;

namespace Web.Areas.Control.Controllers
{
    [Area("Control")]
    [Authorize(Roles = "1,2,8")]
    public class ControlGestionController : Controller
    {
        #region Propiedades
        private readonly IDbAdministracion _iDbAdministracion;
        private readonly IDbControlGestion _iDbControlGestion;
        private readonly IDbSeguimientoIris _iDbSeguimientoIris;
        private readonly ILogger<ControlGestionController> _logger;
        #endregion

        #region Constructor
        public ControlGestionController(IDbAdministracion iDbAdministracion, IDbControlGestion iDbControlGestion,
            IDbSeguimientoIris iDbSeguimientoIris, ILogger<ControlGestionController> logger)
        {
            _iDbAdministracion = iDbAdministracion;
            _iDbControlGestion = iDbControlGestion;
            _iDbSeguimientoIris = iDbSeguimientoIris;
            _logger = logger;
        }
        #endregion

        public async Task<ActionResult> Tablero()
        {
            await _iDbAdministracion.P_InsAuditoria(
                Convert.ToInt64(User.FindFirstValue("Identificacion")), "Ingreso Módulo", "Ingreso módulo Control/ControlGestion",
                Convert.ToInt64(User.FindFirstValue("Identificacion")), HttpContext.Session.GetString("IpMaquina"));

            var ddlAnioIris = (await _iDbSeguimientoIris.F_GetAniosIrisP1()).Data.ToList();
            var anioActual = ddlAnioIris.Any() ? ddlAnioIris.Max(x => x.AnoIrisp1) : (int?)null;

            ViewBag.ddlAnioIris = new SelectList(ddlAnioIris, "AnoIrisp1", "AnoIrisp1", anioActual);
            ViewBag.Unidad = User.FindFirstValue("Dependencia");

            return View();
        }

        #region Métodos de Consulta

        [HttpGet]
        public async Task<IActionResult> F_GetTareasControlGestion(int V_Anio)
        {
            var codigoUnidad = Convert.ToInt64(User.FindFirstValue("IdUndeLabora"));

            var rolesUsuario = string.Join(",",
                User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value));

            var resultado = await _iDbControlGestion.F_GetTareasControlGestion(V_Anio, rolesUsuario, codigoUnidad);

            if (resultado.IdRespuesta <= 0)
                return Json(new { success = false, message = resultado.Mensaje, data = new List<DtoTareaControlGestion>(), kpis = ArmarKpis(new List<DtoTareaControlGestion>()) });

            return Json(new { success = true, data = resultado.Data, kpis = ArmarKpis(resultado.Data) });
        }

        // Agrega, en memoria, los indicadores que alimentan las tarjetas y el gráfico del tablero
        // a partir de la misma grilla que ya se consultó — evita duplicar la consulta en Oracle.
        private static object ArmarKpis(List<DtoTareaControlGestion> tareas)
        {
            var porEstado = tareas
                .GroupBy(t => t.EstadoSla ?? "SIN SLA DEFINIDO")
                .Select(g => new { estado = g.Key, cantidad = g.Count() })
                .ToList();

            var promedioPorTipo = tareas
                .Where(t => t.HorasTranscurridas.HasValue)
                .GroupBy(t => t.DescListaTarea ?? "Sin tipo")
                .Select(g => new { tipo = g.Key, promedioHoras = Math.Round(g.Average(x => x.HorasTranscurridas!.Value), 1) })
                .ToList();

            var promedioPorUnidad = tareas
                .Where(t => t.HorasTranscurridas.HasValue)
                .GroupBy(t => t.Unidad ?? "Sin unidad")
                .Select(g => new { unidad = g.Key, promedioHoras = Math.Round(g.Average(x => x.HorasTranscurridas!.Value), 1) })
                .ToList();

            return new
            {
                total = tareas.Count,
                porEstado,
                promedioPorTipo,
                promedioPorUnidad
            };
        }

        #endregion
    }
}
