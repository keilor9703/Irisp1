//using Comun.Areas.AplicacionDTO;
using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Comun.General;
using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Negocio.Gestion.Admin;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.General;
using Negocio.Interfaz.Irisp1;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using Oracle.ManagedDataAccess.Client;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Data;
using System.Net;
using System.Security.Claims;
using Web.Models;


namespace Web.Areas.Irisp1.Controllers
{
    [Area("Irisp1")]
    [Authorize(Roles = "1,2,3,7")]
    [AutoValidateAntiforgeryToken]
    public class RegistrosIrisp1Controller : Controller
    {
        #region Propiedades

        private readonly IDbAdministracion _iDbAdministracion;
        private readonly IDbIrisp1 _iDbIrisp1;
        private readonly IDbFuncionarios _iDbFuncionarios;
        private readonly IConfiguration _configuration;
       
        private readonly IDbDominios _IDbDominios;
        private readonly string _strConexionIris_Test;
        private readonly string _strConexionIris_Disec;
        private readonly ILogger<RegistrosIrisp1Controller> _logger;


        #endregion

        #region Constructor

        public RegistrosIrisp1Controller(IConfiguration iConfiguration, IDbAdministracion iDbAdministracion, IDbIrisp1 iDbIrisp1, IDbFuncionarios iDbFuncionarios, IConfiguration configuration, IDbDominios idbDominios, ILogger<RegistrosIrisp1Controller> logger)
        {

            _iDbAdministracion = iDbAdministracion;
            _iDbIrisp1 = iDbIrisp1;
            _iDbFuncionarios = iDbFuncionarios;
            _configuration = configuration;
            _IDbDominios = idbDominios;
            _logger = logger;
            _strConexionIris_Test = configuration.GetConnectionString("strConexionIris_Test");
            _strConexionIris_Disec = configuration.GetConnectionString("strConexionIris_Disec");
        }

        #endregion

        public async Task<ActionResult> RegistrosIrisp1()
        {
            var auditoriaTask = _iDbAdministracion.P_InsAuditoria(Convert.ToInt64(User.FindFirstValue("Identificacion")), "Ingreso Módulo", "Ingreso módulo IrisP1/Registros", Convert.ToInt64(User.FindFirstValue("Identificacion")), HttpContext.Session.GetString("IpMaquina"));
            var aniosTask = _iDbIrisp1.F_GetAniosIrisP1();

            // Dominios únicos que necesita esta vista (varios combos "...Modal" reutilizan el mismo dominio
            // que su combo hermano, así que se consultan una sola vez cada uno).
            var claseTask = _IDbDominios.F_GetDominiosIris(12);
            var modExpendioTask = _IDbDominios.F_GetDominiosIris(74);
            var clasiNarcotraficoTask = _IDbDominios.F_GetDominiosIris(153);
            var actividadTask = _IDbDominios.F_GetDominiosIris(127);
            var fuenteTask = _IDbDominios.F_GetDominiosIris(16);
            var entornoTask = _IDbDominios.F_GetDominiosIris(155);
            var zonaTask = _IDbDominios.F_GetDominiosIris(6);
            var delitoTask = _IDbDominios.F_GetDominiosIris(177);
            var tipoServicioTask = _IDbDominios.F_GetDominiosIris(9);
            var existenciaTask = _IDbDominios.F_GetDominiosIris(63);
            var estadosTask = _IDbDominios.F_GetDominiosIris(1);
            var especialidadTask = _IDbDominios.F_GetDominiosIris(160);

            await Task.WhenAll(auditoriaTask, aniosTask, claseTask, modExpendioTask, clasiNarcotraficoTask, actividadTask,
                fuenteTask, entornoTask, zonaTask, delitoTask, tipoServicioTask, existenciaTask, estadosTask, especialidadTask);

            var ddlAnioIris = (await aniosTask).Data.ToList();
            ViewBag.ddlAnioIris = new SelectList(ddlAnioIris, "AnoIrisp1", "AnoIrisp1");

            var claseData = (await claseTask).Data?.OrderBy(x => x.Descripcion).ToList();
            var modExpendioData = (await modExpendioTask).Data?.OrderBy(x => x.Descripcion).ToList();
            var clasiNarcotraficoData = (await clasiNarcotraficoTask).Data?.OrderBy(x => x.Descripcion).ToList();
            var fuenteData = (await fuenteTask).Data?.OrderBy(x => x.Descripcion).ToList();
            var delitoData = (await delitoTask).Data?.OrderBy(x => x.Descripcion).ToList();

            ViewBag.ddlClase = new SelectList(claseData, "IdDominio", "Descripcion");
            ViewBag.ddlModExpendio = new SelectList(modExpendioData, "IdDominio", "Descripcion");
            ViewBag.ddlClasiNarcotrafico = new SelectList(clasiNarcotraficoData, "IdDominio", "Descripcion");
            ViewBag.ddlActividad = new SelectList((await actividadTask).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlFuente = new SelectList(fuenteData, "IdDominio", "Descripcion");
            ViewBag.ddlFuenteModal = new SelectList(fuenteData, "IdDominio", "Descripcion");
            ViewBag.ddlEntono = new SelectList((await entornoTask).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlZona = new SelectList((await zonaTask).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlDelitoPrincipal = new SelectList(delitoData, "IdDominio", "Descripcion");
            ViewBag.ddlDelitoPrincipalModal = new SelectList(delitoData, "IdDominio", "Descripcion");
            ViewBag.ddlDelitoSecundario = new SelectList(delitoData, "IdDominio", "Descripcion");
            ViewBag.ddlDelitoSecundarioModal = new SelectList(delitoData, "IdDominio", "Descripcion");
            ViewBag.ddlTipoServicio = new SelectList((await tipoServicioTask).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlExistenciaIrisP1 = new SelectList((await existenciaTask).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlEstadosIrisP1 = new SelectList((await estadosTask).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlEspecialidad = new SelectList((await especialidadTask).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlCuadrante = new SelectList(Enumerable.Empty<SelectListItem>());

            ViewBag.ddlClaseModal = new SelectList(claseData, "IdDominio", "Descripcion");
            ViewBag.ddlModExpendioModal = new SelectList(modExpendioData, "IdDominio", "Descripcion");
            ViewBag.ddlClasiNarcotraficoModal = new SelectList(clasiNarcotraficoData, "IdDominio", "Descripcion");

            return View();
        }

		#region Métodos de Consulta




		[HttpGet]
		public async Task<IActionResult> F_GetInfoGrillas(Int32 V_Anio)
		{
			var codigoUnidad = Convert.ToInt64(User.FindFirstValue("IdUndeLabora"));

			// 🔹 Obtener todos los roles del usuario separados por coma
			var rolesUsuario = string.Join(",",
				User.Claims
					.Where(c => c.Type == ClaimTypes.Role)
					.Select(c => c.Value)
			);

			var resultado = await _iDbIrisp1.F_GetInfoGrillas(V_Anio, rolesUsuario, codigoUnidad);

			if (resultado.IdRespuesta > 0)
				return Json(new { success = true, data = resultado.Data });
			else
				return StatusCode(StatusCodes.Status500InternalServerError,
					new { success = false, message = resultado.Mensaje });
		}




		[HttpGet]
        public async Task<IActionResult> F_GetCuadrantes(string V_unidadLabora, string V_unidadLabora2)
        {
            var cuadrantes = await _iDbIrisp1.F_GetCuadrantes(V_unidadLabora, V_unidadLabora2);



            if (cuadrantes.IdRespuesta > 0)
            {
                

                // Seleccionar solo los campos necesarios: Consecutivo y Descripcion
                var resultado = cuadrantes.Data.Select(x => new
                {
                    Codigo = x.CODIGOC,
                    Descripcion = x.DESCRIPCION   // Accediendo a la propiedad 'DESCRIPCION'
                }).ToList();

                // Devolver la lista de resultados como JSON
                return Json(resultado);



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


        [HttpPost]
        public async Task<IActionResult> F_ConsultarSeqIntegrante()
        {
            var resultado = await _iDbIrisp1.F_ConsultarSeqIntegrante();


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



        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> F_GetIntegrantes( string V_CriminalidadId)
        {
            var resultado = await _iDbIrisp1.F_GetIntegrantes(V_CriminalidadId);

            if (resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = resultado.Data, message = resultado.Mensaje });
            }
            else
            {
                return Json(new { success = false, data = new List<DtoIntegrantes>(), message = resultado.Mensaje });
            }
        }




        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> F_GetIntegrantesPreliminar(string V_CriminalidadId)
        {
            var resultado = await _iDbIrisp1.F_GetIntegrantesPreliminar(V_CriminalidadId);

            if (resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = resultado.Data, message = resultado.Mensaje });
            }
            else
            {
                return Json(new { success = false, data = new List<DtoIntegrantes>(), message = resultado.Mensaje });
            }
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> F_GetDelitosIris(string V_CriminalidadId)
        {
            var resultado = await _iDbIrisp1.F_GetDelitosIris(V_CriminalidadId);

            if (resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = resultado.Data });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = resultado.Mensaje });

            }
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> F_GetInfoAdicional(string V_CriminalidadId)
        {
            var resultado = await _iDbIrisp1.F_GetInfoAdicional(V_CriminalidadId);

            if (resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = resultado.Data });
            }
            else
            {
                return Json(new { success = true, data = resultado.Data });

            }
        }

        [HttpGet]
        public async Task<IActionResult> F_GetDocIris(string V_CriminalidadId)
        {
            //var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var resultado = await _iDbIrisp1.F_GetDocIris(V_CriminalidadId);

            //if (resultado.IdRespuesta == 1)
            //    return Json(new { success = true, data = resultado.Data });

            //return Json(new { success = false, message = resultado.Mensaje });



            if (resultado.IdRespuesta > 0)
            {
                return Json(new { data = resultado.Data, exito = resultado.IdRespuesta == 1, mensaje = resultado.Mensaje });
            }
            else
            {
                //  return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = resultado.Mensaje });
                return Json(new { success = false, message = resultado.Mensaje });

            }
        }



        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> F_GetCriminalidadFotos(string V_CriminalidadId)
        {
            var resultado = await _iDbIrisp1.F_GetCriminalidadFotos(V_CriminalidadId);
          


            if (resultado.IdRespuesta > 0)
            {
                return Json(new { data = resultado.Data, exito = resultado.IdRespuesta == 1, mensaje = resultado.Mensaje });
            }
            else
            {
              //  return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = resultado.Mensaje });
                return Json(new { success = false, message = resultado.Mensaje });

            }
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> F_GetUbicacionIris(string V_CriminalidadId)
        {
           
            var resultado = await _iDbIrisp1.F_GetUbicacionIris(V_CriminalidadId);


            if (resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = resultado.Data, message = resultado.Mensaje });
            }
            else
            {
                return Json(new { success = false, data = new List<DtoUbicacionIris>(), message = resultado.Mensaje });
            }


        }

        [HttpGet]
        [Route("Irisp1/RegistroIrisp1/descargar")]
        public async Task<IActionResult> DescargarArchivo(string ruta, int tipoRuta)
        {
            if (string.IsNullOrWhiteSpace(ruta))
                return BadRequest("Ruta inválida.");

            try
            {
                // Base UNC desde configuración
               string uncBase ;


                if (tipoRuta == 1)
                { uncBase = _configuration["RutasArchivosIris:RutaDocumentos"]; }
                else
                { uncBase = _configuration["RutasArchivosIris:RutaDocumentosAnterior"]; }

                // Confinar la ruta dentro de la base para evitar path traversal (..\..\)
                string rutaCompleta = RutaArchivoSegura.Resolver(uncBase, ruta);
                if (rutaCompleta == null)
                    return BadRequest("Ruta inválida.");

                if (!System.IO.File.Exists(rutaCompleta))
                    return NotFound("Archivo no encontrado.");

                var bytes = System.IO.File.ReadAllBytes(rutaCompleta);
                var nombreArchivo = Path.GetFileName(rutaCompleta);

                // Auditoría (usando interpolación de cadenas)
                await _iDbAdministracion.P_InsAuditoria(
                    Convert.ToInt64(User.FindFirstValue("Identificacion")),
                    "Descarga Documento IRIS",
                    $"Descarga documento: {nombreArchivo}",   // <-- Ajuste aquí
                    Convert.ToInt64(User.FindFirstValue("Identificacion")),
                    HttpContext.Session.GetString("IpMaquina")
                );

                return File(bytes, "application/octet-stream", nombreArchivo);
            }
            catch (Exception ex)
            {
                // Opcional: loguear el error con más detalle
                _logger.LogError(ex, "Error al descargar el archivo");
                return StatusCode(500, "Error al descargar el archivo.");
            }
        }


        #endregion



        #region Métodos de Insersión

      
        [HttpPost]
        public async Task<IActionResult> GuardarDocumentoConRegistro(IFormFile documento, string idCriminalidad)
        {
            if (string.IsNullOrWhiteSpace(idCriminalidad))
                return BadRequest(new { exito = false, mensaje = "IdCriminalidad no proporcionado." });

            if (documento == null || documento.Length == 0)
                return BadRequest(new { exito = false, mensaje = "No se envió ningún archivo válido." });

            var CriminalidadId_Desc = Convert.ToInt64(ClsEncriptar.Desencriptar(idCriminalidad));
            var usuario = User.FindFirstValue("Identificacion");
            var maquina = HttpContext.Session.GetString("IpMaquina");

            try
            {
                var resultado = await ProcesarYRegistrarDocumentoAsync(
                    idCriminalidad,
                    CriminalidadId_Desc,
                    documento,
                    usuario,
                    maquina
                );

                if (!resultado.Exito)
                    return Json(new { exito = false, mensaje = resultado.Mensaje });

                return Json(new { exito = true, mensaje = "Documento guardado y registro insertado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { exito = false, mensaje = "Error interno al guardar el documento." });
            }
        }


        private async Task<DtoResultadoFoto> ProcesarYRegistrarDocumentoAsync(
                                            string criminalidadIdCadena,
                                            long criminalidadIdNumero,
                                            IFormFile documento,
                                            string usuario,
                                            string maquina)
        {
            var resultado = new DtoResultadoFoto(); // mismo DTO reutilizable

            try
            {
                // 1️⃣ Validar extensión permitida
                var extension = Path.GetExtension(documento.FileName).ToLowerInvariant();
                var extensionesPermitidas = new[] { ".pdf", ".doc", ".docx", ".xlsx", ".txt" };

                if (!extensionesPermitidas.Contains(extension))
                {
                    resultado.Exito = false;
                    resultado.Mensaje = "Formato de documento no permitido.";
                    return resultado;
                }

                // 2️⃣ Validar tamaño (máximo 24MB igual que fotos)
                if (documento.Length > 24 * 1024 * 1024)
                {
                    resultado.Exito = false;
                    resultado.Mensaje = "El documento supera los 24MB permitidos.";
                    return resultado;
                }

                // 2️⃣.b Validar la firma real del archivo (magic bytes) contra la extensión
                // declarada, para que un ejecutable no pueda subirse renombrado como .pdf/.docx.
                byte[] encabezado = new byte[8];
                await using (var lectura = documento.OpenReadStream())
                {
                    int leidos = await lectura.ReadAsync(encabezado, 0, encabezado.Length);
                    if (leidos < encabezado.Length) Array.Resize(ref encabezado, leidos);
                }

                if (!FirmaArchivo.CoincideConExtension(encabezado, extension))
                {
                    resultado.Exito = false;
                    resultado.Mensaje = "El contenido del documento no corresponde a su extensión.";
                    return resultado;
                }

                // 3️⃣ Ruta base para documentos
                var rutaBase = _configuration["RutasArchivosIris:RutaDocumentos"];
                if (string.IsNullOrWhiteSpace(rutaBase))
                {
                    resultado.Exito = false;
                    resultado.Mensaje = "La ruta base de documentos no está configurada.";
                    return resultado;
                }

                // 4️⃣ Construcción de carpetas Año/Mes
                string anio = DateTime.Now.ToString("yyyy");
                string mes = DateTime.Now.ToString("MM");

                string rutaAnio = Path.Combine(rutaBase, anio);
                string rutaMes = Path.Combine(rutaAnio, mes);

                if (!Directory.Exists(rutaAnio)) Directory.CreateDirectory(rutaAnio);
                if (!Directory.Exists(rutaMes)) Directory.CreateDirectory(rutaMes);

                // 5️⃣ Nombre del archivo
                var nombreOriginal = Path.GetFileNameWithoutExtension(documento.FileName);
                string nuevoNombre = $"{nombreOriginal}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                string rutaFinal = Path.Combine(rutaMes, nuevoNombre);

                // 6️⃣ Guardar archivo físico
                await using (var stream = new FileStream(rutaFinal, FileMode.Create))
                {
                    await documento.CopyToAsync(stream);
                }

                // Ruta relativa para BD
                string rutaRelativa = Path.Combine(anio, mes, nuevoNombre);

                // 7️⃣ Insertar en BD vía Dapper
                using var conexion = new OracleConnection(_strConexionIris_Disec);
                var parametros = new DynamicParameters();

                parametros.Add("P_CRIMINALIDAD_ID", criminalidadIdCadena);
                //parametros.Add("P_SERVIDOR", rutaBase);
               // parametros.Add("P_TIPO_DOC", extension.Replace(".", ""));
                parametros.Add("P_NOMBRE", nombreOriginal);
                parametros.Add("P_URL", rutaFinal);
                parametros.Add("P_IDENTIFICACION_CREACION", Convert.ToInt64(usuario));
                //parametros.Add("P_FECHA_CREACION", DateTime.Now);
                parametros.Add("P_MAQUINA_CREACION", maquina ?? string.Empty);
               // parametros.Add("P_VIGENTE", 1);
               // parametros.Add("P_ID_CRIMINALIDA", criminalidadIdNumero);

                parametros.Add("P_RESULTADO", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parametros.Add("SRV_Message", dbType: DbType.String, size: 4000, direction: ParameterDirection.Output);

                await conexion.ExecuteAsync(
                    "PK_REGISTRO_IRIS.P_InsCriminalidadDocumentos",
                    parametros,
                    commandType: CommandType.StoredProcedure
                );

                int resultadoSp = parametros.Get<int>("P_RESULTADO");
                string mensajeSp = parametros.Get<string>("SRV_Message");

                resultado.Exito = resultadoSp == 1;
                resultado.Mensaje = mensajeSp;

                return resultado;
            }
            catch (Exception ex)
            {
                resultado.Exito = false;
                resultado.Mensaje = "Error al procesar o registrar el documento: " + ex.Message;
                return resultado;
            }
        }



        
        [HttpPost]
        public async Task<IActionResult> GuardarFotoConRegistro(IFormFile foto, string idCriminalidad)
        {
            if (string.IsNullOrWhiteSpace(idCriminalidad))
                return BadRequest(new { exito = false, mensaje = "IdCriminalidad no proporcionado." });

            var CriminalidadId_Desencp = Convert.ToInt64(ClsEncriptar.Desencriptar(idCriminalidad));
            var usuario = User.FindFirstValue("Identificacion");
            var maquina = HttpContext.Session.GetString("IpMaquina");

            try
            {
                if (foto == null || foto.Length == 0)
                {
                    return BadRequest(new { exito = false, mensaje = "No se envió ninguna foto válida." });
                }

                var resultadoFoto = await ProcesarYRegistrarFotoAsync(
                    idCriminalidad,            // VARCHAR2 (GUID/encriptado)
                    CriminalidadId_Desencp,    // NUMBER (desencriptado)
                    foto,
                    usuario,
                    maquina
                );

                if (!resultadoFoto.Exito)
                {
                    // IRIS se guardó, pero la foto falló
                    return Json(new { exito = false, mensaje = $"Fallo al insertar en base de datos: {resultadoFoto.Mensaje}" });
                }

                return Json(new { exito = true, mensaje = "Foto guardada y registro insertado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GuardarFotoConRegistro");
                return StatusCode(500, new { exito = false, mensaje = "Error interno al guardar la foto." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> P_InsIntegrantes(DtoIntegrantes Obj_Integrante)
        {

           
            try
            {
                var Resultado = await _iDbIrisp1.P_InsIntegrantes(Obj_Integrante, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina"));

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
                _logger.LogError(ex, "Error en P_InsIntegrantes");
                return Json(new { success = false, data = 0, message = "Error: no fue posible guardar, intente nuevamente." });
            }

        }


        [HttpPost]
        public async Task<IActionResult> P_InsIntegrantesPreliminar(DtoIntegrantes Obj_Integrante)
        {


            try
            {
                var Resultado = await _iDbIrisp1.P_InsIntegrantesPreliminar(Obj_Integrante, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina"));

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
                _logger.LogError(ex, "Error en P_InsIntegrantesPreliminar");
                return Json(new { success = false, data = 0, message = "Error: no fue posible guardar, intente nuevamente." });
            }

        }




        [HttpPost]
        public async Task<IActionResult> P_InsRegistroIrisP1(IFormFile foto, string datosJson)
        {
            if (string.IsNullOrWhiteSpace(datosJson))
                return Json(new { success = false, message = "Datos del IRIS vacíos o inválidos." });

            DtoIrispCriminalidad Obj_NuevoIrisP1;
            try
            {
                Obj_NuevoIrisP1 = JsonConvert.DeserializeObject<DtoIrispCriminalidad>(datosJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interpretando los datos del IRIS");
                return Json(new { success = false, message = "Error interpretando los datos del IRIS." });
            }

            if (string.IsNullOrWhiteSpace(Obj_NuevoIrisP1.CriminalidadId))
                return Json(new { success = false, message = "No se recibió el identificador de criminalidad." });

            // Desencriptar Id numérico
            Obj_NuevoIrisP1.IdCriminalidad = Convert.ToInt64(ClsEncriptar.Desencriptar(Obj_NuevoIrisP1.CriminalidadId));

            var usuario = User.FindFirstValue("Identificacion");
            var maquina = HttpContext.Session.GetString("IpMaquina");

            if (string.IsNullOrWhiteSpace(usuario))
                return Json(new { success = false, message = "No se pudo determinar el usuario autenticado." });

            try
            {
                // 1️⃣ PRIMERO: insertar el IRIS
                var resultadoRegistro = await _iDbIrisp1.P_InsRegistroIrisP1(Obj_NuevoIrisP1, usuario, maquina);

                if (resultadoRegistro.IdRespuesta <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = resultadoRegistro.Mensaje ?? "No fue posible guardar el registro IRISP."
                    });
                }

                // 2️⃣ SEGUNDO (opcional): procesar y asociar la foto
                if (foto != null && foto.Length > 0)
                {
                    var resultadoFoto = await ProcesarYRegistrarFotoAsync(
                        Obj_NuevoIrisP1.CriminalidadId,       // VARCHAR2 (GUID/encriptado)
                        Obj_NuevoIrisP1.IdCriminalidad,       // NUMBER (desencriptado)
                        foto,
                        usuario,
                        maquina
                    );

                    if (!resultadoFoto.Exito)
                    {
                        // OJO: el IRIS ya está insertado (P_InsCriminalidad hace COMMIT)
                        // aquí reportamos que el IRIS se guardó, pero la foto falló.
                        return Json(new
                        {
                            success = false,
                            message = "IRIS creado, pero la fotografía no se pudo asociar: " + resultadoFoto.Mensaje
                        });
                    }
                }

                // 3️⃣ Todo OK
                return Json(new
                {
                    success = true,
                    message = resultadoRegistro.Mensaje ?? "Registro IRISP creado correctamente."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error inesperado al guardar el IRIS y la fotografía: " + ex.Message
                });
            }
        }


        [AllowAnonymous]
        private async Task<DtoResultadoFoto> ProcesarYRegistrarFotoAsync(
            string criminalidadIdCadena,   // P_ID_CRIMINALIDAD (VARCHAR2)
            long? criminalidadIdNumero,     // P_ID_CRIMINALIDA (NUMBER)
            IFormFile foto,
            string usuario,
            string maquina)
        {
            var resultado = new DtoResultadoFoto();

            if (foto == null || foto.Length == 0)
            {
                resultado.Exito = false;
                resultado.Mensaje = "Archivo de imagen vacío o no enviado.";
                return resultado;
            }

            var extension = Path.GetExtension(foto.FileName).ToLowerInvariant();

            // Tipos permitidos de entrada (lo convertimos a WebP internamente)
            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            {
                resultado.Exito = false;
                resultado.Mensaje = "Formato de imagen no permitido. Solo JPG o PNG.";
                return resultado;
            }

            if (foto.Length > 24 * 1024 * 1024)
            {
                resultado.Exito = false;
                resultado.Mensaje = "La imagen supera el tamaño máximo permitido de 24MB.";
                return resultado;
            }

            try
            {
                // 1️⃣ Ruta base y calidad desde appsettings
                var rutaBase = _configuration["RutasArchivosIris:RutaFotos"];
                var calidadStr = _configuration["RutasArchivosIris:CalidadWebP"];
                int calidad = 80;

                if (!string.IsNullOrWhiteSpace(calidadStr) && int.TryParse(calidadStr, out var q))
                    calidad = q;

                if (string.IsNullOrWhiteSpace(rutaBase))
                {
                    resultado.Exito = false;
                    resultado.Mensaje = "La ruta base para almacenamiento de fotos no está configurada.";
                    return resultado;
                }

                // 2️⃣ Subcarpetas por AÑO / MES
                string anio = DateTime.Now.ToString("yyyy");
                string mes = DateTime.Now.ToString("MM");

                string rutaAnio = Path.Combine(rutaBase, anio);
                string rutaMes = Path.Combine(rutaAnio, mes);

                if (!Directory.Exists(rutaAnio)) Directory.CreateDirectory(rutaAnio);
                if (!Directory.Exists(rutaMes)) Directory.CreateDirectory(rutaMes);

                // 3️⃣ Nombre final .webp
                var nombreBase = Path.GetFileNameWithoutExtension(foto.FileName);
                string nuevoNombre = $"{nombreBase}_{DateTime.Now:yyyyMMddHHmmss}.webp";
                string rutaFinal = Path.Combine(rutaMes, nuevoNombre);

                // 4️⃣ Cargar imagen y convertir a WebP comprimido
                using (var imagen = Image.Load(foto.OpenReadStream()))
                {
                    imagen.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(1600, 1600) // recorta solo si es muy grande
                    }));

                    var encoder = new WebpEncoder
                    {
                        Quality = calidad,
                        FileFormat = WebpFileFormatType.Lossy
                    };

                    await using (var output = new FileStream(rutaFinal, FileMode.Create))
                    {
                        imagen.Save(output, encoder);
                    }
                }


                // ⚡ Aquí calculamos la ruta relativa (lo que quieres guardar en BD)
                string rutaRelativa = Path.Combine(anio, mes, nuevoNombre);

                // 5️⃣ Insertar registro en IRISP_CRIMINALIDAD_FOTOS via Dapper
                using var conexion = new OracleConnection(_strConexionIris_Disec);
                var parametros = new DynamicParameters();

                parametros.Add("P_ID_CRIMINALIDAD", criminalidadIdCadena);
                parametros.Add("P_SERVIDOR", rutaBase);
                parametros.Add("P_TIPO_DOC", "webp");
                parametros.Add("P_NAME_FILE", nuevoNombre);
                parametros.Add("P_RUTA", rutaRelativa);
                parametros.Add("P_USUARIO_CREACION", Convert.ToInt64(usuario));
                parametros.Add("P_FECHA_CREACION", DateTime.Now);
                parametros.Add("P_MAQUINA_CREACION", maquina ?? string.Empty);
                parametros.Add("P_VIGENTE", 1);
                parametros.Add("P_ID_CRIMINALIDA", criminalidadIdNumero);
                parametros.Add("P_RESULTADO", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                parametros.Add("SRV_Message", dbType: System.Data.DbType.String, size: 4000, direction: System.Data.ParameterDirection.Output);

                await conexion.ExecuteAsync(
                    "PK_REGISTRO_IRIS.P_InsCriminalidadFotos",
                    parametros,
                    commandType: System.Data.CommandType.StoredProcedure
                );

                int resultadoSp = parametros.Get<int>("P_RESULTADO");
                string mensajeSp = parametros.Get<string>("SRV_Message");

                resultado.Exito = resultadoSp == 1;
                resultado.Mensaje = mensajeSp;

                return resultado;
            }
            catch (Exception ex)
            {
                resultado.Exito = false;
                resultado.Mensaje = "Error al procesar o registrar la fotografía: " + ex.Message;
                return resultado;
            }
        }



      
        [HttpGet]
        public IActionResult Fotos(string V_Ruta)
        {
            //Console.WriteLine($"ENTRÓ A Fotos, V_Ruta={V_Ruta}");
            if (string.IsNullOrWhiteSpace(V_Ruta))
                return NotFound();

            try
            {
                // Ruta base física
                string rutaBase = _configuration["RutasArchivosIris:RutaFotos"];

                // Confinar la ruta dentro de la base para evitar path traversal (..\..\)
                string rutaCompleta = RutaArchivoSegura.Resolver(rutaBase, V_Ruta);
                if (rutaCompleta == null)
                    return BadRequest();

                if (!System.IO.File.Exists(rutaCompleta))
                    return NotFound("Archivo no encontrado.");

                string extension = Path.GetExtension(rutaCompleta).ToLower();
                string contentType = extension switch
                {
                    ".webp" => "image/webp",
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    _ => "application/octet-stream"
                };

                var bytes = System.IO.File.ReadAllBytes(rutaCompleta);
                return File(bytes, contentType);
            }
            catch (Exception ex)
            {
                // También puedes loguear el error
                Console.WriteLine($"Error cargando la foto: {ex.Message}");
                //_logger.LogError(ex, "Error cargando la foto");

                return StatusCode(500, "Error cargando la foto.");
            }
        }



        [HttpPost]
        public async Task<IActionResult> P_InsDelitosIris(DtoIrispCriminalidad Obj_DelitosIris)
        {

            Obj_DelitosIris.IdCriminalidad = Convert.ToInt64(ClsEncriptar.Desencriptar(Obj_DelitosIris.CriminalidadId));

            try
            {
                var Resultado = await _iDbIrisp1.P_InsDelitosIris(Obj_DelitosIris, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina"));

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
                _logger.LogError(ex, "Error en P_InsDelitosIris");
                return Json(new { success = false, data = 0, message = "Error: no fue posible guardar, intente nuevamente." });
            }

        }




        [HttpPost]
        public async Task<IActionResult> P_InsInfoAdicionalIris(DtoInfoAdicional Obj_InfoAdicional)
        {

        
            try
            {
                var Resultado = await _iDbIrisp1.P_InsInfoAdicionalIris(Obj_InfoAdicional, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina"));

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
                _logger.LogError(ex, "Error en P_InsInfoAdicionalIris");
                return Json(new { success = false, data = 0, message = "Error: no fue posible guardar, intente nuevamente." });
            }

        }



        [HttpPost]
        public async Task<IActionResult> P_UpdCriminalidad(DtoIrispCriminalidad data)
        {
            try
            {
                var resultado = await _iDbIrisp1.P_UpdCriminalidad(
                    data,
                    User.FindFirstValue("Identificacion"),
                    HttpContext.Session.GetString("IpMaquina")
                );

                if (resultado.IdRespuesta > 0)
                    return Json(new { success = true, message = resultado.Mensaje });
                else
                    return Json(new { success = false, message = resultado.Mensaje });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar registro IRIS");
                return Json(new { success = false, message = "No es posible actualizar en este momento." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> P_UpdEstadoCriminalidad(DtoIrispCriminalidad data)
        {
            try
            {
                var resultado = await _iDbIrisp1.P_UpdEstadoCriminalidad(
                    data,
                    User.FindFirstValue("Identificacion"),
                    HttpContext.Session.GetString("IpMaquina")
                );

                if (resultado.IdRespuesta > 0)
                    return Json(new { success = true, message = resultado.Mensaje });
                else
                    return Json(new { success = false, message = resultado.Mensaje });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar registro IRIS");
                return Json(new { success = false, message = "No es posible actualizar en este momento." });
            }
        }



        [HttpPost]
        public async Task<IActionResult> P_UpdExistenciaCriminalidad(DtoIrispCriminalidad data)
        {
            try
            {
                var resultado = await _iDbIrisp1.P_UpdExistenciaCriminalidad(
                    data,
                    User.FindFirstValue("Identificacion"),
                    HttpContext.Session.GetString("IpMaquina")
                );

                if (resultado.IdRespuesta > 0)
                    return Json(new { success = true, message = resultado.Mensaje });
                else
                    return Json(new { success = false, message = resultado.Mensaje });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar registro IRIS");
                return Json(new { success = false, message = "No es posible actualizar en este momento." });
            }
        }




        [HttpPost]
        public async Task<IActionResult> P_InsUbicacionIris(DtoUbicacionIris Obj_Ubicacion)
        {


            Obj_Ubicacion.IdCriminalidad = Convert.ToInt64(ClsEncriptar.Desencriptar(Obj_Ubicacion.CriminalidadId));

            try
            {
                var resultado = await _iDbIrisp1.P_InsUbicacionIris(
                    Obj_Ubicacion,
                    User.FindFirstValue("Identificacion"),
                    HttpContext.Session.GetString("IpMaquina")
                );

                if (resultado.IdRespuesta > 0)
                    return Json(new { success = true, message = resultado.Mensaje });
                else
                    return Json(new { success = false, message = resultado.Mensaje });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar registro IRIS");
                return Json(new { success = false, message = "No es posible actualizar en este momento." });
            }
        }


       




        #endregion



        #region Métodos de Eliminación


        [HttpPost]
        public async Task<IActionResult> P_DellIris(string CriminalidadId)
        {
            try
            {
                var resultado = await _iDbIrisp1.P_DellIris(
                    CriminalidadId,
                    User.FindFirstValue("Identificacion"),
                    HttpContext.Session.GetString("IpMaquina")
                );

                if (resultado.IdRespuesta > 0)
                    return Json(new { success = true, message = resultado.Mensaje });
                else
                    return Json(new { success = false, message = resultado.Mensaje });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar registro IRIS");
                return Json(new { success = false, message = "No es posible actualizar en este momento." });
            }
        }



        [HttpPost]
        public async Task<IActionResult> P_DelIntegranteIris(string IntegranteId)
        {
            try
            {
                var resultado = await _iDbIrisp1.P_DelIntegranteIris(IntegranteId, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina")
                );

                if (resultado.IdRespuesta > 0)
                    return Json(new { success = true, message = resultado.Mensaje });
                else
                    return Json(new { success = false, message = resultado.Mensaje });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar registro IRIS");
                return Json(new { success = false, message = "No es posible actualizar en este momento." });
            }
        }



        [HttpPost]
        public async Task<IActionResult> P_DelDelitosIris(string DelitoId)
        {
            try
            {
                var resultado = await _iDbIrisp1.P_DelDelitosIris(DelitoId, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina")
                );

                if (resultado.IdRespuesta > 0)
                    return Json(new { success = true, message = resultado.Mensaje });
                else
                    return Json(new { success = false, message = resultado.Mensaje });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar registro IRIS");
                return Json(new { success = false, message = "No es posible actualizar en este momento." });
            }
        }



        [HttpPost]
        public async Task<IActionResult> P_DelDelInfoAdicionalIris(string InfoId)
        {
            try
            {
                var resultado = await _iDbIrisp1.P_DelDelInfoAdicionalIris(InfoId, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina")
                );

                if (resultado.IdRespuesta > 0)
                    return Json(new { success = true, message = resultado.Mensaje });
                else
                    return Json(new { success = false, message = resultado.Mensaje });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar registro IRIS");
                return Json(new { success = false, message = "No es posible actualizar en este momento." });
            }
        }



        [HttpPost]
        public async Task<IActionResult> P_DelUbicacionIris(string UbicacionId)
        {
            try
            {
                var resultado = await _iDbIrisp1.P_DelUbicacionIris(UbicacionId, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina")
                );

                if (resultado.IdRespuesta > 0)
                    return Json(new { success = true, message = resultado.Mensaje });
                else
                    return Json(new { success = false, message = resultado.Mensaje });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar registro IRIS");
                return Json(new { success = false, message = "No es posible actualizar en este momento." });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> P_DelDocumentoIris(string DocumentoId)
        {
            try
            {
                var resultado = await _iDbIrisp1.P_DelDocumentoIris(DocumentoId, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina")
                );

                if (resultado.IdRespuesta > 0)
                    return Json(new { success = true, message = resultado.Mensaje });
                else
                    return Json(new { success = false, message = resultado.Mensaje });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar registro IRIS");
                return Json(new { success = false, message = "No es posible actualizar en este momento." });
            }
        }





        #endregion


    }
}