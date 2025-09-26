using Comun.Areas.AplicacionDTO;
using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Comun.General;
using Gepad.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Negocio.Gestion.Admin;
using Negocio.Gestion.Irisp1;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.General;
using Negocio.Interfaz.Irisp1;
using NuGet.Packaging.Signing;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Net;
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
        private readonly string _strConexionIris_Test;
        private object _IDbSeguimientoIris;


        #endregion

        #region Constructor

        public SeguimientoController(IConfiguration iConfiguration, IDbAdministracion iDbAdministracion, IDbSeguimientoIris iDbSeguimientoIris, IDbFuncionarios iDbFuncionarios, IConfiguration configuration, IDbDominios idbDominios)
        {

            _iDbAdministracion = iDbAdministracion;
            _iDbSeguimientoIris = iDbSeguimientoIris;
            _iDbFuncionarios = iDbFuncionarios;
            _configuration = configuration;
            _IDbDominios = idbDominios;
            _strConexionIris_Test = configuration.GetConnectionString("strConexionIris_Test");
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


        [HttpPost]
        public async Task<IActionResult> ConsultarAnioSeguimiento(string _anioSeguimiento)
        {
            if (string.IsNullOrWhiteSpace(_anioSeguimiento))
            {
                return BadRequest(new { msg = "el año es requerido.", ok = false });
            }

            try
            {
                var resultados = await _iDbSeguimientoIris.ConsultarSeguimientoIris(_anioSeguimiento);

                if (resultados == null || !resultados.Any())
                {
                    return NotFound(new { msg = "No se encontraron iris para el año seleccionado.", ok = false });
                }

                return Ok(resultados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msg = "Error interno del servidor", ok = false, error = ex.Message });
            }
        }


    }

    public class AnioRequest
    {
        public string _anio { get; set; }
    }

}
