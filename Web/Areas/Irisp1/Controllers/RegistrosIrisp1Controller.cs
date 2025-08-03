using Comun.Areas.Integrantes;
using Gepad.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Negocio.Gestion.Admin;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.General;
using Negocio.Interfaz.Irisp1;
using NuGet.Packaging.Signing;
using Oracle.ManagedDataAccess.Client;
using System.Security.Claims;


namespace Web.Areas.Irisp1.Controllers
{
    [Area("Irisp1")]
    [Authorize(Roles = "1,2")]
    public class RegistrosIrisp1Controller : Controller
    {
        #region Propiedades

        private readonly IDbAdministracion _iDbAdministracion;
        private readonly IDbIrisp1 _iDbIrisp1;
        private readonly IDbFuncionarios _iDbFuncionarios;
        private readonly IConfiguration _configuration;
        private readonly IDbDominios _IDbDominios;

        #endregion

        #region Constructor

        public RegistrosIrisp1Controller(IDbAdministracion iDbAdministracion, IDbIrisp1 iDbIrisp1, IDbFuncionarios iDbFuncionarios, IConfiguration configuration, IDbDominios idbDominios)
        {
            _iDbAdministracion = iDbAdministracion;
            _iDbIrisp1 = iDbIrisp1;
            _iDbFuncionarios = iDbFuncionarios;
            _configuration = configuration;
            _IDbDominios = idbDominios;
        }

        #endregion

        public async Task<ActionResult> RegistrosIrisp1()
        {
            var ddlAnioIris = (await _iDbIrisp1.F_GetAniosIrisP1()).Data.ToList();
            ViewBag.ddlAnioIris = new SelectList(ddlAnioIris, "AnoIrisp1", "AnoIrisp1");
            ViewBag.ddlClase = new SelectList((await _IDbDominios.F_GetDominiosIris(12)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlModExpendio = new SelectList((await _IDbDominios.F_GetDominiosIris(74)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlClasiNarcotrafico = new SelectList((await _IDbDominios.F_GetDominiosIris(153)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlActividad = new SelectList((await _IDbDominios.F_GetDominiosIris(127)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlFuente = new SelectList((await _IDbDominios.F_GetDominiosIris(16)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlEntono = new SelectList((await _IDbDominios.F_GetDominiosIris(155)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlZona = new SelectList((await _IDbDominios.F_GetDominiosIris(6)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlDelitoPrincipal = new SelectList((await _IDbDominios.F_GetDominiosIris(6)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlDelitoSecundario = new SelectList((await _IDbDominios.F_GetDominiosIris(6)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            
            return View();
        }

        #region Métodos de Consulta

        [HttpGet]
        public async Task<IActionResult> F_GetEstadosIrisP1()
        {
            var resultado = await _iDbIrisp1.F_GetEstadosIrisP1();

            if (resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = resultado.Data });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [HttpGet]
        public async Task<IActionResult> F_GetInfoGrillas(Int32 V_Anio)
        {
            var resultado = await _iDbIrisp1.F_GetInfoGrillas(V_Anio);

            if (resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = resultado.Data });
            }
            else
            {
                return Json(new { success = false });
            }
        }



        [HttpGet]
        public async Task<IActionResult> F_GetCuadrantes(string V_unidadLabora)
        {
            var resultado = await _iDbIrisp1.F_GetCuadrantes(V_unidadLabora);

            if (resultado.IdRespuesta > 0)
            {
                return Json(resultado.Data); // Solo la lista
            }
            else
            {
                return Json(new { success = false });
            }
        }



        [HttpPost]
        public async Task<IActionResult> F_ConsultarSeqIris()
        {
            var resultado = await _iDbIrisp1.F_ConsultarSeqIris();
           

            if (resultado.IdRespuesta > 0)
            {
                var consecutivo = resultado.Data.ToString();

                consecutivo = ClsEncriptar.Encriptar(consecutivo);
                return Json(new { success = true, data = consecutivo, message = resultado.Mensaje });
            }
            else
            {
                return Json(new { success = false, data = resultado.Data, message = resultado.Mensaje });
            }
        }



        #endregion


        #region Métodos de Insersión

        [HttpPost]
        public async Task<IActionResult> GuardarFoto(IFormFile foto)
        {
            if (foto == null || foto.Length == 0)
                return Json(new { exito = false, mensaje = "Archivo inválido" });

            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(foto.FileName).ToLowerInvariant();

            if (!extensionesPermitidas.Contains(extension))
                return Json(new { exito = false, mensaje = "Formato no permitido" });

            if (foto.Length > 5 * 1024 * 1024)
                return Json(new { exito = false, mensaje = "Tamaño excedido" });

            try
            {
                // Construir nombre único
                var nombreArchivo = Path.GetFileNameWithoutExtension(foto.FileName);
                var nuevoNombre = $"{nombreArchivo}_{DateTime.Now:yyyyMMddHHmmss}{extension}";

                // Ruta UNC de red
                var rutaRed = @"\\srvfilesponal3\OFITE\AITEC\GRUDE\TE KEHILOR MARTINEZ\Fotos_Iris";
                var rutaArchivoCompleta = Path.Combine(rutaRed, nuevoNombre);

                // Guardar archivo en la carpeta de red
                using (var stream = new FileStream(rutaArchivoCompleta, FileMode.Create))
                {
                    await foto.CopyToAsync(stream);
                }

                return Json(new { exito = true, mensaje = "Imagen guardada correctamente" });
            }
            catch (Exception ex)
            {
                // Puedes loguear el error si usas Serilog, NLog, etc.
                return Json(new { exito = false, mensaje = $"Error al guardar imagen: {ex.Message}" });
            }
        }



        [HttpGet]
        public async Task<IActionResult> P_InsIntegrantes(DtoIntegrantes Obj_Integrante)
        {
            

            try
            {
                var Resultado = await _iDbIrisp1.P_InsIntegrantes(Obj_Integrante);

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