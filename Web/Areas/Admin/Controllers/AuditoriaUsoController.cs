using System.Globalization;
using System.Security.Claims;
using Comun.Areas.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.Interfaz.Admin;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "1,2")] // Super Usuario / Administrador
    public class AuditoriaUsoController : Controller
    {
        private readonly IDbAuditoriaUso _iDbAuditoriaUso;
        private readonly IDbAdministracion _iDbAdministracion;
        private readonly ILogger<AuditoriaUsoController> _logger;

        public AuditoriaUsoController(IDbAuditoriaUso iDbAuditoriaUso, IDbAdministracion iDbAdministracion,
            ILogger<AuditoriaUsoController> logger)
        {
            _iDbAuditoriaUso = iDbAuditoriaUso;
            _iDbAdministracion = iDbAdministracion;
            _logger = logger;
        }

        public async Task<IActionResult> VwDashboardUso()
        {
            await _iDbAdministracion.P_InsAuditoria(
                Convert.ToInt64(User.FindFirstValue("Identificacion")), "Ingreso Módulo",
                "Ingreso módulo Administracion/AuditoriaUso",
                Convert.ToInt64(User.FindFirstValue("Identificacion")), HttpContext.Session.GetString("IpMaquina"));

            // Rango por defecto: últimos 30 días.
            ViewBag.FechaFin = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.FechaIni = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard(string? fechaIni, string? fechaFin, string? sigla, string? usuario)
        {
            try
            {
                var hoy = DateTime.Now.Date;
                var fIni = ParseFecha(fechaIni) ?? hoy.AddDays(-30);
                var fFin = ParseFecha(fechaFin) ?? hoy;
                if (fFin < fIni) (fIni, fFin) = (fFin, fIni);

                long? cedula = long.TryParse((usuario ?? "").Trim(), out var c) ? c : (long?)null;
                string? siglaF = string.IsNullOrWhiteSpace(sigla) ? null : sigla.Trim();

                // Todas las consultas en paralelo (independientes).
                var tResumen = _iDbAuditoriaUso.F_GetResumen(fIni, fFin, siglaF, cedula);
                var tModulo = _iDbAuditoriaUso.F_GetPorModulo(fIni, fFin, siglaF, cedula);
                var tFecha = _iDbAuditoriaUso.F_GetPorFecha(fIni, fFin, siglaF, cedula);
                var tUnidad = _iDbAuditoriaUso.F_GetPorUnidad(fIni, fFin, siglaF, cedula);
                var tUsuario = _iDbAuditoriaUso.F_GetPorUsuario(fIni, fFin, siglaF, cedula);
                var tCreados = _iDbAdministracion.F_GetListUsuarios();

                await Task.WhenAll(tResumen, tModulo, tFecha, tUnidad, tUsuario, tCreados);

                var resumen = (await tResumen).Data ?? new DtoAudResumen();
                var porModulo = (await tModulo).Data ?? new List<DtoAudPorModulo>();
                var porFecha = (await tFecha).Data ?? new List<DtoAudPorFecha>();
                var porUnidad = (await tUnidad).Data ?? new List<DtoAudPorUnidad>();
                var porUsuario = (await tUsuario).Data ?? new List<DtoAudPorUsuario>();
                var creados = (await tCreados).Data ?? new List<DtoUsuario>();

                // Índice de actividad por cédula.
                var actividad = porUsuario
                    .GroupBy(u => u.Identificacion)
                    .ToDictionary(g => g.Key, g => g.First());

                // Tabla de depuración: TODOS los usuarios creados + su actividad (incluye "Nunca ingresó").
                var depuracion = new List<DtoAudUsuarioDepuracion>();
                foreach (var u in creados)
                {
                    actividad.TryGetValue(u.Identificacion, out var act);

                    // Filtro por unidad (sigla): se conserva si la unidad del funcionario (física/dependencia)
                    // o su unidad de actividad coincide con la sigla seleccionada.
                    if (siglaF != null)
                    {
                        var coincide =
                            string.Equals(u.Fisica, siglaF, StringComparison.OrdinalIgnoreCase)
                            || (u.Dependencia ?? "").IndexOf(siglaF, StringComparison.OrdinalIgnoreCase) >= 0
                            || (act != null && string.Equals(act.Sigla, siglaF, StringComparison.OrdinalIgnoreCase));
                        if (!coincide) continue;
                    }
                    if (cedula.HasValue && u.Identificacion != cedula.Value) continue;

                    long ingresos = act?.TotalIngresos ?? 0;
                    DateTime? ultimo = act?.UltimoIngreso;
                    int? dias = ultimo.HasValue ? (int?)(hoy - ultimo.Value.Date).Days : null;

                    string estado;
                    if (ingresos == 0 || ultimo == null) estado = "Nunca ingresó";
                    else if (dias > 90) estado = "Inactivo (+90 días)";
                    else estado = "Activo";

                    depuracion.Add(new DtoAudUsuarioDepuracion
                    {
                        Identificacion = u.Identificacion,
                        Funcionario = string.IsNullOrWhiteSpace(u.ApellidosNombres) ? u.Funcionario : u.ApellidosNombres,
                        Usuario = u.Usuario,
                        Unidad = act?.Unidad ?? u.Dependencia ?? u.Fisica,
                        Bloqueado = u.Bloqueado,
                        TotalIngresos = ingresos,
                        ModulosDistintos = act?.ModulosDistintos ?? 0,
                        UltimoIngreso = ultimo,
                        DiasSinIngresar = dias,
                        Estado = estado
                    });
                }

                int usuariosCreados = depuracion.Count;
                int usuariosActivos = depuracion.Count(d => d.TotalIngresos > 0);
                int nuncaIngresaron = usuariosCreados - usuariosActivos;
                double adopcionPct = usuariosCreados > 0 ? Math.Round(usuariosActivos * 100.0 / usuariosCreados, 1) : 0;

                var kpis = new
                {
                    usuariosCreados,
                    usuariosActivos,
                    nuncaIngresaron,
                    adopcionPct,
                    totalIngresos = resumen.TotalIngresos,
                    totalEventos = resumen.TotalEventos,
                    modulosDistintos = resumen.ModulosDistintos,
                    exportaciones = resumen.Exportaciones,
                    descargas = resumen.Descargas
                };

                var topUsuarios = porUsuario.Take(15).ToList();

                return Json(new
                {
                    success = true,
                    kpis,
                    porModulo,
                    porFecha,
                    porUnidad,
                    topUsuarios,
                    depuracion = depuracion.OrderBy(d => d.TotalIngresos).ThenBy(d => d.Funcionario).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetDashboard de Auditoría de Uso");
                return Json(new { success = false, message = "No fue posible cargar el dashboard de uso y auditoría." });
            }
        }

        private static DateTime? ParseFecha(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;
            if (DateTime.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d)) return d.Date;
            if (DateTime.TryParse(valor, out d)) return d.Date;
            return null;
        }
    }
}
