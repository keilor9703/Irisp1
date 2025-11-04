using Comun.Areas.Clientes;
using Comun.Areas.Irisp1;
using Comun.Areas.Mod_Uno;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Negocio.Gestion.Irisp1;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.Clientes;
using Negocio.Interfaz.General;
using Negocio.Interfaz.Irisp1;
using Negocio.Interfaz.Modulo1;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Policy;
using System.Threading;
using Negocio.Gestion.General;

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
        private readonly string _strConexionIris_Disec;


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
            _strConexionIris_Disec = configuration.GetConnectionString("strConexionIris_Disec");
        }

        #endregion

        public async Task<ActionResult> Verificacion()
        {
            var ddlAnioIris = (await _iDbVerificacionIris.F_GetAniosIrisP1()).Data.ToList();
           

            var anioActual = ddlAnioIris.Max(x => x.AnoIrisp1);

            //  Crea el SelectList con el año actual seleccionado por defecto
            ViewBag.ddlAnioIris = new SelectList(ddlAnioIris, "AnoIrisp1", "AnoIrisp1", anioActual);

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
            ViewBag.ddlTipoResultado = new SelectList((await _IDbDominios.F_GetDominiosIris(68)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlDelitoSecundario = new SelectList((await _IDbDominios.F_GetDominiosIris(177)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlDelitoSecundarioModal = new SelectList((await _IDbDominios.F_GetDominiosIris(177)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlTipoServicio = new SelectList((await _IDbDominios.F_GetDominiosIris(9)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlExistenciaIrisP1 = new SelectList((await _IDbDominios.F_GetDominiosIris(63)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlEstadosIrisP1 = new SelectList((await _IDbDominios.F_GetDominiosIris(1)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlEspecialidad = new SelectList((await _IDbDominios.F_GetDominiosIris(160)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlTipoExiste = new SelectList((await _IDbDominios.F_GetDominiosIris(63)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
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
        public async Task<IActionResult> F_GetResultados(string V_Criminalidad)//, string V_ResponsableId)
        {
            var resultado = await _iDbVerificacionIris.F_GetResultados(V_Criminalidad);//,V_ResponsableId);

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
        public async Task<IActionResult> F_GetResponsablesTareasIris(string V_Criminalidad)
        {
            var resultado = await _iDbVerificacionIris.F_GetResponsablesTareasIris(V_Criminalidad);

            if (resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = resultado.Data });
            }
            else
            {
                return Json(new { success = false, data = resultado.Data });
            }
        }


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


        #endregion





        #region Métodos de Insersión


        [HttpPost]
        public async Task<IActionResult> P_InsResultadoTareasIris(DtoIrisResultado Obj_Resultado)
        {
            try
            {
                var Resultado = await _iDbVerificacionIris.P_InsResultadoIris(Obj_Resultado,User.FindFirstValue("Identificacion"),HttpContext.Session.GetString("IpMaquina")
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
        public async Task<IActionResult> P_InsTareaRespuesta(DtoTareasIris Obj_RespuestaTarea)
        {
            try
            {
                var Resultado = await _iDbVerificacionIris.P_InsTareaRespuesta(Obj_RespuestaTarea, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina")
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
        public async Task<IActionResult> GuardarDocumentoTareaConRegistro(IFormFile file, string tareaId)
        {

            var usuario = User.FindFirstValue("Identificacion");
            var maquina = HttpContext.Session.GetString("IpMaquina");

            if (file == null || file.Length == 0)
                return Json(new { exito = false, mensaje = "Archivo inválido" });

            try
            {
                // 1. Guardar archivo en red
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var nombreArchivoOriginal = Path.GetFileNameWithoutExtension(file.FileName);
                var nuevoNombre = $"{nombreArchivoOriginal}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                var rutaRed = @"\\srvfilesponal3\OFITE\AITEC\GRUDE\TE KEHILOR MARTINEZ\Documentos Iris";
                var rutaArchivoCompleta = Path.Combine(rutaRed, nuevoNombre);

                using (var stream = new FileStream(rutaArchivoCompleta, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // 2. Guardar registro en BD
                using (var conexion = new OracleConnection(_strConexionIris_Disec))
                using (var command = new OracleCommand("PK_VERIFICACION_IRIS.P_GuardarDocumentoTarea", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("P_TAREA_ID", OracleDbType.Varchar2).Value = tareaId;
                    command.Parameters.Add("P_NOMBRE", OracleDbType.NVarchar2).Value = nombreArchivoOriginal;
                    command.Parameters.Add("P_URL", OracleDbType.NVarchar2).Value = rutaArchivoCompleta;


                    command.Parameters.Add("P_IDENTIFICACION_CREACION", OracleDbType.Int64).Value = usuario;
                    command.Parameters.Add("P_MAQUINA_CREACION", OracleDbType.Varchar2).Value = maquina;

                    command.Parameters.Add("P_RESULTADO", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    command.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;



                    await conexion.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    var resultado = Convert.ToInt32(((Oracle.ManagedDataAccess.Types.OracleDecimal)command.Parameters["P_RESULTADO"].Value).ToInt32());
                    var mensaje = command.Parameters["SRV_Message"].Value.ToString();

                    if (resultado == 1)
                        return Json(new { success = true, message = "Documento guardado correctamente" });

                    else
                        return Json(new { exito = false, mensaje = $"Error al insertar en BD: {mensaje}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, mensaje = $"Error al guardar documento: {ex.Message}" });
            }
        }




        #endregion

    }
}
