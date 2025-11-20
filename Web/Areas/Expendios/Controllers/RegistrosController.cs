using Comun.Areas.Expendios;
using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Negocio.Gestion.Admin;
using Negocio.Gestion.General;
using Negocio.Gestion.Irisp1;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.Expendios;
using Negocio.Interfaz.General;
using Negocio.Interfaz.Irisp1;
using System.Security.Claims;
using Web.Models;

namespace Web.Areas.Expendios.Controllers
{

    [Area("Expendios")]
    [Authorize(Roles = "1,2,11")]
    public class RegistrosController : Controller
    {

    
        private readonly IConfiguration _iConfiguration;
        private readonly IDbAdministracion _iDbAdministracion;
        private readonly IDbRegistroExpendio _iDbRegistroExpendio;
        private readonly IDbSeguimientoIris _iDbSeguimientoIris;

        private readonly IDbDominios _iDbDominios;


        public RegistrosController(IConfiguration iConfiguration, IDbAdministracion dbAdministracion , IDbDominios iDbDominios, IDbRegistroExpendio iRegistroExpendio, IDbSeguimientoIris iSegimientoIris)
        {

            _iConfiguration = iConfiguration;
            _iDbAdministracion = dbAdministracion;
            _iDbDominios = iDbDominios;
            _iDbRegistroExpendio = iRegistroExpendio;
            _iDbSeguimientoIris = iSegimientoIris;
        }

        public async Task<ActionResult> Registros()
        {

            var Auditoria = await _iDbAdministracion.P_InsAuditoria(Convert.ToInt64(User.FindFirstValue("Identificacion")), "VwRegistrosExpendios", "Ingreso Módulo", "0", HttpContext.Session.GetString("IpMaquina"));

            ViewBag.ddlUnidadExpendio = new SelectList((await _iDbSeguimientoIris.F_GetUnidadesSeguimiento()).Data?.OrderBy(x => x.DESCRIPCION_DEPENDENCIA), "SIGLA", "DESCRIPCION_DEPENDENCIA");

            var ddlAnioIris = (await _iDbRegistroExpendio.F_GetAniosIrisP1()).Data.ToList();
           // var anioActual = ddlAnioIris.Max(x => x.AnoIrisp1);
           // ViewBag.ddlAnioIris = new SelectList(ddlAnioIris, "AnoIrisp1", "AnoIrisp1", anioActual);
            ViewBag.ddlAnioIris = new SelectList(ddlAnioIris, "AnoIrisp1", "AnoIrisp1");
            ViewBag.ddlDelitoModal = new SelectList((await _iDbDominios.F_GetDominiosIris(177)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlTipoResultado = new SelectList((await _iDbDominios.F_GetDominiosIris(76)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");           
            ViewBag.ddlZonaExpendio = new SelectList((await _iDbDominios.F_GetDominiosIris(6)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");           
            ViewBag.ddlSubTipoResultado = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.ddlEstacionExpendio = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.ddlunidadInformaExpendio = new SelectList(Enumerable.Empty<SelectListItem>());
            var ddlTipoEstado = (await _iDbDominios.F_GetDominiosIris(92)).Data?.Where(x => x.Descripcion == "Investigación" || x.Descripcion == "Descartado" || x.Descripcion == "Finalizado").OrderBy(x => x.Descripcion).ToList();
            ViewBag.ddlTipoEstado = new SelectList(ddlTipoEstado, "IdDominio", "Descripcion");

            var ddlExpendio = (await _iDbDominios.F_GetDominiosIris(12)).Data?.Where(x => x.Descripcion == "Trafico De Estupefacientes").OrderBy(x => x.Descripcion).ToList();
            ViewBag.ddlExpendio = new SelectList(ddlExpendio, "IdDominio", "Descripcion");
            ViewBag.ddlTipoExpendio = new SelectList((await _iDbDominios.F_GetDominiosIris(74)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlFuente = new SelectList((await _iDbDominios.F_GetDominiosIris(16)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlCategoria = new SelectList((await _iDbDominios.F_GetDominiosIris(96)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlDelitosRelacionados = new SelectList((await _iDbDominios.F_GetDominiosIris(177)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");


            return View();
        }


        #region Métodos de Consulta



        [HttpPost]
        public async Task<IActionResult> F_ConsultarSeqIris()
        {
            var resultado = await _iDbRegistroExpendio.F_ConsultarSeqIris();


            if (resultado.IdRespuesta > 0)
            {
                var consecutivo = resultado.Data.ToString();

                consecutivo = ClsEncriptar.Encriptar(consecutivo);
                return Json(new { success = true, data = resultado, message = resultado.Mensaje });
            }
            else
            {
                return Json(new { success = false, data = resultado.Data, message = resultado.Mensaje });
            }
        }




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


        [HttpGet]
        public async Task<IActionResult> F_GetEstaciones(string V_Sigla)
        {
            var resultado = await _iDbRegistroExpendio.F_GetEstaciones(V_Sigla);

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
        public async Task<IActionResult> F_GetEspecialidad(string V_Sigla)
        {
            var resultado = await _iDbRegistroExpendio.F_GetEspecialidad(V_Sigla);

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
        public async Task<IActionResult> F_GetIntegrantes(string V_CriminalidadId)
        {
            var resultado = await _iDbRegistroExpendio.F_GetIntegrantes(V_CriminalidadId);

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
        public async Task<IActionResult> F_GetIntegrantesPreliminar(string V_CriminalidadId)
        {
            var resultado = await _iDbRegistroExpendio.F_GetIntegrantesPreliminar(V_CriminalidadId);

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
        public async Task<IActionResult> F_GetDelitosIris(string V_CriminalidadId)
        {
            var resultado = await _iDbRegistroExpendio.F_GetDelitosIris(V_CriminalidadId);

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
        public async Task<IActionResult> F_GetBitacora(string V_CriminalidadId)
        {
            var resultado = await _iDbRegistroExpendio.F_GetBitacora(V_CriminalidadId);

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
        public async Task<IActionResult> F_GetResultados(string V_CriminalidadId)
        {
            var resultado = await _iDbRegistroExpendio.F_GetResultados(V_CriminalidadId);

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
        public async Task<IActionResult> F_GetIntegranteAll(Int64 V_Identificacion)
        {
            var resultado = await _iDbRegistroExpendio.F_GetIntegranteAll(V_Identificacion);

            if (resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = resultado.Data });
            }
            else
            {
                return Json(new { success = false, message = resultado.Mensaje });


            }
        }


        #endregion



        #region Métodos de Insersión

        [HttpPost]
        public async Task<IActionResult> P_InsRegistroExpendio(DtoInsExpendios Obj_NuevoExpendio)
        {

            
            try
            {
                var Resultado = await _iDbRegistroExpendio.P_InsRegistroExpendio(Obj_NuevoExpendio, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina"));

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
        public async Task<IActionResult> P_InsIntegrante(DtoIntegrantes Obj_Integrante)
        {
            try
            {
                var Resultado = await _iDbRegistroExpendio.P_InsIntegrante(Obj_Integrante, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina")
                );

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
        public async Task<IActionResult> P_InsIntegrantePreliminar(DtoIntegrantes Obj_Integrante)
        {
            try
            {
                var Resultado = await _iDbRegistroExpendio.P_InsIntegrantePreliminar(Obj_Integrante, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina")
                );

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
        public async Task<IActionResult> P_InsDelito(DtoDelitosIris Obj_Delito)
        {
            try
            {
                var Resultado = await _iDbRegistroExpendio.P_InsDelito(Obj_Delito, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina")
                );

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
        public async Task<IActionResult> P_InsBitacora(DtoInfoAdicional Obj_Bitacora)
        {
            try
            {
                var Resultado = await _iDbRegistroExpendio.P_InsBitacora(Obj_Bitacora, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina")
                );

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
        public async Task<IActionResult> P_InsResultados(DtoResultadosExpendio Obj_Resultados)
        {
            try
            {
                var Resultado = await _iDbRegistroExpendio.P_InsResultados(Obj_Resultados, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina")
                );

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


        #endregion


        #region Métodos de Actualización

        [HttpPost]
        public async Task<IActionResult> P_UpdExpendio(DtoExpendios Obj_UpdExpendio)
        {
            try
            {
                var Resultado = await _iDbRegistroExpendio.P_UpdExpendio(Obj_UpdExpendio, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina")
                );

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
        public async Task<IActionResult> P_UpdIntegrante(DtoIntegrantes Obj_Integrante)
        {
            try
            {
                var Resultado = await _iDbRegistroExpendio.P_UpdIntegrante(Obj_Integrante, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina")
                );

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


        #endregion
    }
}



