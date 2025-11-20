using Comun.Areas.AplicacionDTO;
using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Negocio.Gestion.Admin;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.General;
using Negocio.Interfaz.Irisp1;
using NuGet.Packaging.Signing;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Net;
using System.Security.Claims;
using Web.Models;


namespace Web.Areas.Irisp1.Controllers
{
    [Area("Irisp1")]
    [Authorize(Roles = "1,2,3,7")]
    public class RegistrosIrisp1Controller : Controller
    {
        #region Propiedades

        private readonly IDbAdministracion _iDbAdministracion;
        private readonly IDbIrisp1 _iDbIrisp1;
        private readonly IDbFuncionarios _iDbFuncionarios;
        private readonly IConfiguration _configuration;
       
        private readonly IDbDominios _IDbDominios;
        private readonly string _strConexionIris_Test;
      

        #endregion

        #region Constructor

        public RegistrosIrisp1Controller(IConfiguration iConfiguration, IDbAdministracion iDbAdministracion, IDbIrisp1 iDbIrisp1, IDbFuncionarios iDbFuncionarios, IConfiguration configuration, IDbDominios idbDominios)
        {
          
            _iDbAdministracion = iDbAdministracion;
            _iDbIrisp1 = iDbIrisp1;
            _iDbFuncionarios = iDbFuncionarios;
            _configuration = configuration;
            _IDbDominios = idbDominios;
            _strConexionIris_Test = configuration.GetConnectionString("strConexionIris_Test");
        }

        #endregion

        public async Task<ActionResult> RegistrosIrisp1()
        {


			var Auditoria = await _iDbAdministracion.P_InsAuditoria(Convert.ToInt64(User.FindFirstValue("Identificacion")), "VwRegistrosIrisp1", "Ingreso Módulo", "0", HttpContext.Session.GetString("IpMaquina"));

			var ddlAnioIris = (await _iDbIrisp1.F_GetAniosIrisP1()).Data.ToList();
          

            //  var anioActual = ddlAnioIris.Max(x => x.AnoIrisp1);

            //  Crea el SelectList con el año actual seleccionado por defecto
            //ViewBag.ddlAnioIris = new SelectList(ddlAnioIris, "AnoIrisp1", "AnoIrisp1", anioActual);
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



		//[HttpGet]
		//public async Task<IActionResult> F_GetInfoGrillas(Int32 V_Anio)
		//{


		//    var resultado = await _iDbIrisp1.F_GetInfoGrillas(V_Anio);

		//    if (resultado.IdRespuesta > 0)
		//    {
		//        return Json(new { success = true, data = resultado.Data });
		//    }
		//    else
		//    {
		//        return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = resultado.Mensaje });

		//    }
		//}



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
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = resultado.Mensaje });

            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> F_GetDocIris(string V_CriminalidadId)
        {
            var resultado = await _iDbIrisp1.F_GetDocIris(V_CriminalidadId);

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
        public async Task<IActionResult> f_GetFotosCriminalidad(string V_CriminalidadId)
        {
            var resultado = await _iDbIrisp1.F_GetCriminalidadFotos(V_CriminalidadId);
          


            if (resultado.IdRespuesta > 0)
            {
                return Json(new { data = resultado.Data, exito = resultado.IdRespuesta == 1, mensaje = resultado.Mensaje });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = resultado.Mensaje });

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
        public async Task<IActionResult> GuardarFotoConRegistro(IFormFile foto, string idCriminalidad)
        {

            var CriminalidadId_Desencp = Convert.ToInt64(ClsEncriptar.Desencriptar(idCriminalidad));

            var usuario = User.FindFirstValue("Identificacion");
            var maquina = HttpContext.Session.GetString("IpMaquina");
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
                // 1. Guardar archivo en red
                var nombreArchivo = Path.GetFileNameWithoutExtension(foto.FileName);
                var nuevoNombre = $"{nombreArchivo}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                var rutaRed = @"\\srvfilesponal3\OFITE\AITEC\GRUDE\TE KEHILOR MARTINEZ\Fotos_Iris";
                var rutaArchivoCompleta = Path.Combine(rutaRed, nuevoNombre);

                using (var stream = new FileStream(rutaArchivoCompleta, FileMode.Create))
                {
                    await foto.CopyToAsync(stream);
                }

                // 2. Llamar procedimiento almacenado
                using (var Conexion = new OracleConnection(_strConexionIris_Test))
                    
                using (var command = new OracleCommand("PK_REGISTRO_IRIS.P_InsCriminalidadFotos", Conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;

                  
                    command.Parameters.Add("P_ID_CRIMINALIDAD", OracleDbType.Varchar2).Value = idCriminalidad;
                    command.Parameters.Add("P_SERVIDOR", OracleDbType.NVarchar2).Value = Environment.MachineName;
                    command.Parameters.Add("P_TIPO_DOC", OracleDbType.NVarchar2).Value = extension.TrimStart('.'); // jpg, png, etc.
                    command.Parameters.Add("P_NAME_FILE", OracleDbType.NVarchar2).Value = nuevoNombre;
                    command.Parameters.Add("P_RUTA", OracleDbType.NVarchar2).Value = rutaArchivoCompleta;
                    command.Parameters.Add("P_USUARIO_CREACION", OracleDbType.Int32).Value = usuario;
                    command.Parameters.Add("P_FECHA_CREACION", OracleDbType.Date).Value = DateTime.Now;
                    command.Parameters.Add("P_MAQUINA_CREACION", OracleDbType.NVarchar2).Value = maquina;
                    command.Parameters.Add("P_VIGENTE", OracleDbType.Int32).Value = 1;
                    command.Parameters.Add("P_USUARIO_MODIFICA", OracleDbType.Int32).Value = usuario;
                    command.Parameters.Add("P_MAQUINA_MODIFICA", OracleDbType.NVarchar2).Value = maquina;
                    command.Parameters.Add("P_FECHA_MODIFICA", OracleDbType.Date).Value = DateTime.Now;
                    command.Parameters.Add("P_ID_CRIMINALIDA", OracleDbType.Int32).Value = CriminalidadId_Desencp;

                 

                    command.Parameters.Add("P_RESULTADO", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    command.Parameters.Add("SRV_Message", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;

                    await Conexion.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    var resultado = ((Oracle.ManagedDataAccess.Types.OracleDecimal)command.Parameters["P_RESULTADO"].Value).ToInt32();
                    var mensaje = command.Parameters["SRV_Message"].Value.ToString();


                    if (resultado == 1)
                    {
                        return Json(new { exito = true, mensaje = "Foto guardada y registro insertado correctamente" });
                    }
                    else
                    {
                        return Json(new { exito = false, mensaje = $"Fallo al insertar en base de datos: {mensaje}" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, mensaje = $"Error al guardar imagen o registrar: {ex.Message}" });
            }
        }


        [HttpPost]
        public async Task<IActionResult> GuardarDocumentoConRegistro(IFormFile file, string idCriminalidad)
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
                using (var conexion = new OracleConnection(_strConexionIris_Test))
                using (var command = new OracleCommand("PK_REGISTRO_IRIS.P_InsCriminalidadDocumentos", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("P_CRIMINALIDAD_ID", OracleDbType.Varchar2).Value = idCriminalidad;
                    command.Parameters.Add("P_NOMBRE", OracleDbType.NVarchar2).Value = nombreArchivoOriginal;
                    command.Parameters.Add("P_URL", OracleDbType.NVarchar2).Value = rutaArchivoCompleta;
                 
                   
                    command.Parameters.Add("P_IDENTIFICACION_CREACION", OracleDbType.Int64).Value = usuario;
                    command.Parameters.Add("P_MAQUINA_CREACION", OracleDbType.NVarchar2).Value = maquina;
                   

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




        [HttpPost]
        public async Task<IActionResult> P_InsIntegrantes(DtoIntegrantes Obj_Integrante)
        {

            //Obj_Integrante.ID_CRIMINALIDAD = Convert.ToInt64(ClsEncriptar.Desencriptar(Obj_Integrante.CRIMINALIDAD_ID));
           // Obj_Integrante.ID_INTEGRANTE = Convert.ToInt64(ClsEncriptar.Desencriptar(Obj_Integrante.INTEGRANTE_ID));
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
                return Json(new { success = false, data = 0, message = "Error: no es posible guardar, revise " + ex });
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
                return Json(new { success = false, data = 0, message = "Error: no es posible guardar, revise " + ex });
            }

        }



        [HttpPost]
        public async Task<IActionResult> P_InsRegistroIrisP1(DtoIrispCriminalidad Obj_NuevoIrisP1)
        {

            Obj_NuevoIrisP1.IdCriminalidad = Convert.ToInt64(ClsEncriptar.Desencriptar(Obj_NuevoIrisP1.CriminalidadId));
           
            try
            {
                var Resultado = await _iDbIrisp1.P_InsRegistroIrisP1(Obj_NuevoIrisP1, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina"));

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
                return Json(new { success = false, data = 0, message = "Error: no es posible guardar, revise " + ex });
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
                return Json(new { success = false, data = 0, message = "Error: no es posible guardar, revise " + ex });
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
                return Json(new { success = false, message = "Error: no es posible actualizar. " + ex.Message });
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
                return Json(new { success = false, message = "Error: no es posible actualizar. " + ex.Message });
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
                return Json(new { success = false, message = "Error: no es posible actualizar. " + ex.Message });
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
                return Json(new { success = false, message = "Error: no es posible actualizar. " + ex.Message });
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
                return Json(new { success = false, message = "Error: no es posible actualizar. " + ex.Message });
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
                return Json(new { success = false, message = "Error: no es posible actualizar. " + ex.Message });
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
                return Json(new { success = false, message = "Error: no es posible actualizar. " + ex.Message });
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
                return Json(new { success = false, message = "Error: no es posible actualizar. " + ex.Message });
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
                return Json(new { success = false, message = "Error: no es posible actualizar. " + ex.Message });
            }
        }


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
                return Json(new { success = false, message = "Error: no es posible actualizar. " + ex.Message });
            }
        }





        #endregion


    }
}