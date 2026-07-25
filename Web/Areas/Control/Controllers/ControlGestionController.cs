using Comun.Areas.Control;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.Control;
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
        private readonly ILogger<ControlGestionController> _logger;
        #endregion

        #region Constructor
        public ControlGestionController(IDbAdministracion iDbAdministracion, IDbControlGestion iDbControlGestion,
            ILogger<ControlGestionController> logger)
        {
            _iDbAdministracion = iDbAdministracion;
            _iDbControlGestion = iDbControlGestion;
            _logger = logger;
        }
        #endregion

        public async Task<ActionResult> Tablero()
        {
            await _iDbAdministracion.P_InsAuditoria(
                Convert.ToInt64(User.FindFirstValue("Identificacion")), "Ingreso Módulo", "Ingreso módulo Control/ControlGestion",
                Convert.ToInt64(User.FindFirstValue("Identificacion")), HttpContext.Session.GetString("IpMaquina"));

            await PoblarDdlRegionYSiglaUnidad();
            ViewBag.Unidad = User.FindFirstValue("Dependencia");

            return View();
        }

        // Rango de fechas por defecto (último año hasta hoy) cuando el usuario no ha filtrado
        // aún — mismo criterio que usa F_GetMapaIrisp1, para que el tablero y el mapa se
        // comporten igual la primera vez que se abren.
        private static (DateTime FechaInicio, DateTime FechaFin) ResolverRangoFechas(DateTime? fechaInicio, DateTime? fechaFin)
        {
            var fin = fechaFin ?? DateTime.Today;
            var inicio = fechaInicio ?? fin.AddMonths(-12);
            return (inicio, fin);
        }

        // Llena ViewBag.ddlRegion y ViewBag.ddlSiglaUnidad (combos de filtro, iguales en Tablero
        // y Mapa; la sigla arranca sin filtrar por región — el cascadeo real ocurre en el cliente
        // vía F_GetSiglasUnidad) y ViewBag.SiglasCargadasError — IdRespuesta == 0 puede significar
        // "no hay datos" o "el paquete Oracle PK_CONTROL_GESTION_IRIS aún no se recompiló con las
        // funciones nuevas"; se avisa en la vista para no confundir ambos casos.
        private async Task PoblarDdlRegionYSiglaUnidad()
        {
            var resultadoRegiones = await _iDbControlGestion.F_GetRegionesIrisp1();
            var regiones = resultadoRegiones.Data
                .Where(r => r.RegionCodigo.HasValue)
                .ToList();
            ViewBag.ddlRegion = new SelectList(regiones, "RegionCodigo", "RegionDescripcion");

            var resultadoSiglas = await _iDbControlGestion.F_GetSiglasUnidadIrisp1(null);
            var siglas = resultadoSiglas.Data
                .Where(s => !string.IsNullOrWhiteSpace(s.SiglaUnidad))
                .ToList();

            ViewBag.ddlSiglaUnidad = new SelectList(siglas, "SiglaUnidad", "SiglaUnidad");
            ViewBag.SiglasCargadasError = resultadoSiglas.IdRespuesta <= 0;
        }

        public async Task<ActionResult> Mapa()
        {
            await _iDbAdministracion.P_InsAuditoria(
                Convert.ToInt64(User.FindFirstValue("Identificacion")), "Ingreso Módulo", "Ingreso módulo Control/Mapa",
                Convert.ToInt64(User.FindFirstValue("Identificacion")), HttpContext.Session.GetString("IpMaquina"));

            await PoblarDdlRegionYSiglaUnidad();
            ViewBag.Unidad = User.FindFirstValue("Dependencia");

            return View();
        }

        #region Métodos de Consulta

        [HttpGet]
        public async Task<IActionResult> F_GetTareasControlGestion(DateTime? V_FechaInicio, DateTime? V_FechaFin, int? V_RegionCodigo, string? V_SiglaUnidad)
        {
            var codigoUnidad = Convert.ToInt64(User.FindFirstValue("IdUndeLabora"));

            var rolesUsuario = string.Join(",",
                User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value));

            var (fechaInicio, fechaFin) = ResolverRangoFechas(V_FechaInicio, V_FechaFin);

            var resultado = await _iDbControlGestion.F_GetTareasControlGestion(fechaInicio, fechaFin, V_RegionCodigo, V_SiglaUnidad, rolesUsuario, codigoUnidad);

            if (resultado.IdRespuesta <= 0)
                return Json(new { success = false, message = resultado.Mensaje, data = new List<DtoTareaControlGestion>(), kpis = ArmarKpis(new List<DtoTareaControlGestion>()) });

            return Json(new { success = true, data = resultado.Data, kpis = ArmarKpis(resultado.Data) });
        }

        // KPIs a nivel de caso (no de tarea): tiempo total de creación a finalización, y tiempo
        // por etapa (Verificación/Investigación). Se consulta aparte de F_GetTareasControlGestion
        // porque es un dataset distinto (un caso IRISP1 puede tener varias tareas).
        [HttpGet]
        public async Task<IActionResult> F_GetKpisTiempoGestion(DateTime? V_FechaInicio, DateTime? V_FechaFin, int? V_RegionCodigo, string? V_SiglaUnidad)
        {
            var codigoUnidad = Convert.ToInt64(User.FindFirstValue("IdUndeLabora"));

            var rolesUsuario = string.Join(",",
                User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value));

            var (fechaInicio, fechaFin) = ResolverRangoFechas(V_FechaInicio, V_FechaFin);

            var resultado = await _iDbControlGestion.F_GetCasosControlGestion(fechaInicio, fechaFin, V_RegionCodigo, V_SiglaUnidad, rolesUsuario, codigoUnidad);

            if (resultado.IdRespuesta <= 0)
                return Json(new { success = false, message = resultado.Mensaje, data = new List<DtoCasoControlGestion>(), kpis = ArmarKpisCasos(new List<DtoCasoControlGestion>()) });

            return Json(new { success = true, data = resultado.Data, kpis = ArmarKpisCasos(resultado.Data) });
        }

        // Resultado/efectividad de cada caso IRISP1 del rango de fechas: cuántos se finalizaron,
        // cuántos resultaron con existencia confirmada/descartada, cuántos siguen abiertos, y el
        // ranking de unidades por efectividad — responde a "qué unidades sí están tramitando lo
        // que se les asigna" y no solo a los tiempos de SLA.
        [HttpGet]
        public async Task<IActionResult> F_GetResultadosCasos(DateTime? V_FechaInicio, DateTime? V_FechaFin, int? V_RegionCodigo, string? V_SiglaUnidad)
        {
            var codigoUnidad = Convert.ToInt64(User.FindFirstValue("IdUndeLabora"));

            var rolesUsuario = string.Join(",",
                User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value));

            var (fechaInicio, fechaFin) = ResolverRangoFechas(V_FechaInicio, V_FechaFin);

            var resultado = await _iDbControlGestion.F_GetResultadosCasosIrisp1(fechaInicio, fechaFin, V_RegionCodigo, V_SiglaUnidad, rolesUsuario, codigoUnidad);

            if (resultado.IdRespuesta <= 0)
                return Json(new { success = false, message = resultado.Mensaje, data = new List<DtoResultadoCasoIrisp1>(), kpis = ArmarKpisResultados(new List<DtoResultadoCasoIrisp1>()) });

            return Json(new { success = true, data = resultado.Data, kpis = ArmarKpisResultados(resultado.Data) });
        }

        // Cascada Región -> Sigla de unidad: cuando el usuario elige una región en el filtro, el
        // JS vuelve a pedir el catálogo de siglas ya acotado a esa región.
        [HttpGet]
        public async Task<IActionResult> F_GetSiglasUnidad(int? regionCodigo)
        {
            var resultado = await _iDbControlGestion.F_GetSiglasUnidadIrisp1(regionCodigo);

            if (resultado.IdRespuesta <= 0)
                return Json(new { success = false, message = resultado.Mensaje, data = new List<DtoSiglaUnidad>() });

            return Json(new { success = true, data = resultado.Data });
        }

        // ID_ESTADO = 5 ("Finalizado") está confirmado en PK_SEGUIMIENTO_IRIS.P_FinalizarIris.
        // La clasificación de existencia se basa en el texto real del dominio (PADRE_ID=63), ya
        // que es el mismo texto que usan las grillas existentes ("SI EXISTE"/"NO EXISTE").
        private const int ID_ESTADO_FINALIZADO = 5;

        private static bool EsCasoConExistencia(DtoResultadoCasoIrisp1 caso) =>
            !string.IsNullOrWhiteSpace(caso.DescEstadoExistencia) &&
            caso.DescEstadoExistencia!.Trim().ToUpperInvariant().Contains("SI EXISTE");

        private static bool EsCasoSinExistencia(DtoResultadoCasoIrisp1 caso) =>
            !string.IsNullOrWhiteSpace(caso.DescEstadoExistencia) &&
            caso.DescEstadoExistencia!.Trim().ToUpperInvariant().Contains("NO EXISTE");

        private static bool EsCasoFinalizado(DtoResultadoCasoIrisp1 caso) =>
            caso.IdEstado == ID_ESTADO_FINALIZADO;

        private static object ArmarKpisResultados(List<DtoResultadoCasoIrisp1> casos)
        {
            var total = casos.Count;
            var finalizados = casos.Count(EsCasoFinalizado);
            var existe = casos.Count(EsCasoConExistencia);
            var noExiste = casos.Count(EsCasoSinExistencia);
            var pendienteExistencia = total - existe - noExiste;
            var abiertos = total - finalizados;

            // Inconcluso: el caso se cerró (finalizado) sin que quedara registrada una
            // definición de existencia — un resultado "a medias" distinto de un caso exitoso
            // (existe confirmado) o descartado (no existe).
            var inconclusos = casos.Count(c => EsCasoFinalizado(c) && !EsCasoConExistencia(c) && !EsCasoSinExistencia(c));

            var porEstado = casos
                .GroupBy(c => string.IsNullOrWhiteSpace(c.DescEstado) ? "Sin estado" : c.DescEstado!)
                .Select(g => new { estado = g.Key, cantidad = g.Count() })
                .OrderByDescending(g => g.cantidad)
                .ToList();

            var porExistencia = new[]
            {
                new { resultado = "Existe", cantidad = existe },
                new { resultado = "No existe", cantidad = noExiste },
                new { resultado = "Sin determinar", cantidad = pendienteExistencia }
            };

            var rankingUnidades = casos
                .GroupBy(c => !string.IsNullOrWhiteSpace(c.UnidadSigla) ? c.UnidadSigla! : (c.Unidad ?? "Sin unidad"))
                .Select(g =>
                {
                    var totalUnidad = g.Count();
                    var finalizadosUnidad = g.Count(EsCasoFinalizado);
                    var existeUnidad = g.Count(EsCasoConExistencia);

                    return new
                    {
                        unidad = g.Key,
                        total = totalUnidad,
                        finalizados = finalizadosUnidad,
                        existeConfirmado = existeUnidad,
                        abiertos = totalUnidad - finalizadosUnidad,
                        efectividadPct = totalUnidad > 0 ? Math.Round((decimal)existeUnidad * 100 / totalUnidad, 1) : 0m
                    };
                })
                .OrderByDescending(u => u.efectividadPct)
                .ThenByDescending(u => u.total)
                .ToList();

            return new
            {
                total,
                finalizados,
                existe,
                noExiste,
                pendienteExistencia,
                abiertos,
                inconclusos,
                porEstado,
                porExistencia,
                rankingUnidades
            };
        }

        private static object ArmarKpisCasos(List<DtoCasoControlGestion> casos)
        {
            var finalizados = casos.Where(c => c.HorasTotalCaso.HasValue).ToList();
            var verificacion = casos.Where(c => c.HorasVerificacion.HasValue).ToList();
            var investigacion = casos.Where(c => c.HorasInvestigacion.HasValue).ToList();

            return new
            {
                totalCasos = casos.Count,
                casosFinalizados = finalizados.Count,
                promedioTotalHoras = finalizados.Count > 0 ? Math.Round(finalizados.Average(c => c.HorasTotalCaso!.Value), 1) : (decimal?)null,
                casosVerificacion = verificacion.Count,
                promedioVerificacionHoras = verificacion.Count > 0 ? Math.Round(verificacion.Average(c => c.HorasVerificacion!.Value), 1) : (decimal?)null,
                casosInvestigacion = investigacion.Count,
                promedioInvestigacionHoras = investigacion.Count > 0 ? Math.Round(investigacion.Average(c => c.HorasInvestigacion!.Value), 1) : (decimal?)null
            };
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
                .GroupBy(t => !string.IsNullOrWhiteSpace(t.UnidadSigla) ? t.UnidadSigla! : (t.Unidad ?? "Sin unidad"))
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

        [HttpGet]
        public async Task<IActionResult> F_GetMapaIrisp1(DateTime? V_FechaInicio, DateTime? V_FechaFin, int? V_RegionCodigo, string? V_SiglaUnidad)
        {
            var codigoUnidad = Convert.ToInt64(User.FindFirstValue("IdUndeLabora"));

            var rolesUsuario = string.Join(",",
                User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value));

            var (fechaInicio, fechaFin) = ResolverRangoFechas(V_FechaInicio, V_FechaFin);

            var resultado = await _iDbControlGestion.F_GetMapaIrisp1(fechaInicio, fechaFin, V_RegionCodigo, V_SiglaUnidad, rolesUsuario, codigoUnidad);

            if (resultado.IdRespuesta <= 0)
                return Json(new { success = false, message = resultado.Mensaje, data = new List<DtoMapaIrisp1>() });

            return Json(new { success = true, data = resultado.Data, fechaInicio, fechaFin });
        }

        #endregion
    }
}
