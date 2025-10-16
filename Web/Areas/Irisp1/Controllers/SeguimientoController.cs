
using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Gepad.Models;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Negocio.Interfaz.Admin;
using Negocio.Interfaz.General;
using Negocio.Interfaz.Irisp1;

using System.Data;
using System.Security.Claims;


namespace Web.Areas.Irisp1.Controllers
{
    [Area("Irisp1")]
    [Authorize(Roles = "1,2")]
    public class SeguimientoController : Controller
    {
        #region Propiedades
        private readonly IDbAdministracion _iDbAdministracion;
        private readonly IDbSeguimientoIris _iDbSeguimientoIris;
        private readonly IDbFuncionarios _iDbFuncionarios;
        private readonly IConfiguration _configuration;

        private readonly IDbDominios _IDbDominios;
     


        #endregion

        #region Constructor

        public SeguimientoController(IConfiguration iConfiguration, IDbAdministracion iDbAdministracion, IDbSeguimientoIris iDbSeguimientoIris, IDbFuncionarios iDbFuncionarios, IConfiguration configuration, IDbDominios idbDominios)
        {

            _iDbAdministracion = iDbAdministracion;
            _iDbSeguimientoIris = iDbSeguimientoIris;
            _iDbFuncionarios = iDbFuncionarios;
            _configuration = configuration;
            _IDbDominios = idbDominios;
            //_strConexionIris_Test = configuration.GetConnectionString("strConexionIris_Test");
        }
        #endregion
        public async Task<ActionResult> Seguimiento()
        {
            var ddlAnioIris = (await _iDbSeguimientoIris.F_GetAniosIrisP1()).Data.ToList();
            ViewBag.ddlAnioIris = new SelectList(ddlAnioIris, "AnoIrisp1", "AnoIrisp1");
            ViewBag.ddlClase = new SelectList((await _IDbDominios.F_GetDominiosIris(12)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlModExpendio = new SelectList((await _IDbDominios.F_GetDominiosIris(74)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlClasiNarcotrafico = new SelectList((await _IDbDominios.F_GetDominiosIris(153)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlActividad = new SelectList((await _IDbDominios.F_GetDominiosIris(127)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlFuente = new SelectList((await _IDbDominios.F_GetDominiosIris(16)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlFuenteModal = new SelectList((await _IDbDominios.F_GetDominiosIris(16)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlEntono = new SelectList((await _IDbDominios.F_GetDominiosIris(155)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlZona = new SelectList((await _IDbDominios.F_GetDominiosIris(6)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlDelitoPrincipal = new SelectList((await _IDbDominios.F_GetDominiosIris(177)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlDelitoPrincipalModal = new SelectList((await _IDbDominios.F_GetDominiosIris(177)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlDelitoSecundario = new SelectList((await _IDbDominios.F_GetDominiosIris(177)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlDelitoSecundarioModal = new SelectList((await _IDbDominios.F_GetDominiosIris(177)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlTipoServicio = new SelectList((await _IDbDominios.F_GetDominiosIris(9)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlExistenciaIrisP1 = new SelectList((await _IDbDominios.F_GetDominiosIris(63)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlEstadosIrisP1 = new SelectList((await _IDbDominios.F_GetDominiosIris(1)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlEspecialidad = new SelectList((await _IDbDominios.F_GetDominiosIris(160)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlCuadrante = new SelectList(Enumerable.Empty<SelectListItem>());

            ViewBag.ddlClaseModal = new SelectList((await _IDbDominios.F_GetDominiosIris(12)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");

            ViewBag.ddlModExpendioModal = new SelectList((await _IDbDominios.F_GetDominiosIris(74)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");

            ViewBag.ddlClasiNarcotraficoModal = new SelectList((await _IDbDominios.F_GetDominiosIris(153)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlTipoUnidad = new SelectList((await _iDbSeguimientoIris.F_GetUnidadesSeguimiento()).Data?.OrderBy(x => x.DESCRIPCION_DEPENDENCIA), "SIGLA", "DESCRIPCION_DEPENDENCIA");
            ViewBag.ddlTipoUnidad2 = new SelectList((await _iDbSeguimientoIris.F_GetUnidadesSeguimiento()).Data?.OrderBy(x => x.DESCRIPCION_DEPENDENCIA), "SIGLA", "DESCRIPCION_DEPENDENCIA");
            ViewBag.ddlTipoDependencia = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.ddlTipoDependencia2 = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.ddlTipoTarea = new SelectList((await _IDbDominios.F_GetDominiosIris(51)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            var dominios = (await _IDbDominios.F_GetDominiosIris(58)).Data?
                             .Where(x => x.Descripcion == "Aceptada" || x.Descripcion == "Rechazada")
                             .OrderBy(x => x.Descripcion)
                             .ToList();

            ViewBag.ddlTipoEvalTarea = new SelectList(dominios, "IdDominio", "Descripcion");


            return View();
        }



        #region Métodos de Consulta
        [HttpGet]
        public async Task<IActionResult> F_GetInfoGrillas(Int32 V_Anio)
        {
            var resultado = await _iDbSeguimientoIris.F_GetInfoGrillas(V_Anio);

            if (resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = resultado.Data });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = resultado.Mensaje });

            }
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
                return Json(new { success = false, data = 0, message = "Error: no es posible guardar, revise " + ex });
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
                return Json(new { success = false, data = 0, message = "Error: no es posible guardar, revise " + ex });
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
                return Json(new { success = false, data = 0, message = "Error: no es posible guardar, revise " + ex });
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
                return Json(new { success = false, data = 0, message = "Error: no es posible guardar, revise " + ex });
            }

        }
    }
}
