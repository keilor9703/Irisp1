using Comun.Areas.Clientes;
using Comun.Areas.Mod_Uno;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Negocio.Gestion.Irisp1;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.Clientes;
using Negocio.Interfaz.General;
using Negocio.Interfaz.Irisp1;
using Negocio.Interfaz.Modulo1;
using System.Security.Claims;

namespace Web.Areas.Irisp1.Controllers
{
    [Area("Irisp1")]
    [Authorize(Roles = "1,2")]
    public class VerificacionController : Controller
    {

        #region Propiedades

        private readonly IDbAdministracion _iDbAdministracion;
        private readonly IDbVerificacionIris _iDbVerificacionIris;
        private readonly IDbFuncionarios _iDbFuncionarios;
        private readonly IConfiguration _configuration;

        private readonly IDbDominios _IDbDominios;
        private readonly string _strConexionIris_Test;


        #endregion

        #region Constructor

        public VerificacionController(IConfiguration iConfiguration, IDbAdministracion iDbAdministracion, IDbVerificacionIris iDbVerificacionIris, IDbFuncionarios iDbFuncionarios, IConfiguration configuration, IDbDominios idbDominios)
        {

            _iDbAdministracion = iDbAdministracion;
            _iDbVerificacionIris = iDbVerificacionIris;
            _iDbFuncionarios = iDbFuncionarios;
            _configuration = configuration;
            _IDbDominios = idbDominios;
            _strConexionIris_Test = configuration.GetConnectionString("strConexionIris_Test");
        }

        #endregion

        public async Task<ActionResult> Verificacion()
        {
            var ddlAnioIris = (await _iDbVerificacionIris.F_GetAniosIrisP1()).Data.ToList();
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
            var resultado = await _iDbVerificacionIris.F_GetInfoGrillas(V_Anio);

            if (resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = resultado.Data });
            }
            else
            {
                // return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = resultado.Mensaje });
                return Json(new { success = false, data = resultado.Data });
            }
        }



        [HttpGet]
        public async Task<IActionResult> F_GetTareas(string V_ResponsableId)
        {
            var resultado = await _iDbVerificacionIris.F_GetTareas(V_ResponsableId);

            if (resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = resultado.Data });
            }
            else
            {
              //  return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = resultado.Mensaje });
                return Json(new { success = false, data = resultado.Data });

            }
        }


        [HttpGet]
        public async Task<IActionResult> F_GetResultados(string V_Criminalidad, string V_ResponsableId)
        {
            var resultado = await _iDbVerificacionIris.F_GetResultados(V_Criminalidad,V_ResponsableId);

            if (resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = resultado.Data });
            }
            else
            {
                //  return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = resultado.Mensaje });
                return Json(new { success = false, data = resultado.Data });

            }
        }



        #endregion



        [HttpGet]
        [Route("Irisp1/Verificacion/descargar")]
        public IActionResult DescargarArchivo(string ruta)
        {
            Console.WriteLine($"Ruta solicitada: {ruta}");

            if (!System.IO.File.Exists(ruta))
                return NotFound("Archivo no encontrado");

            var nombreArchivo = Path.GetFileName(ruta);
            var bytes = System.IO.File.ReadAllBytes(ruta);
            return File(bytes, "application/octet-stream", nombreArchivo);
        }



    }
}
