
using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Negocio.Interfaz.Admin;
using Negocio.Interfaz.General;
using Negocio.Interfaz.Irisp1;

using System.Data;


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




        #endregion
    }
}
