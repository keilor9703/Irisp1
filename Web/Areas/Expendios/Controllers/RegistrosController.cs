using ClosedXML.Excel;
using Comun.Areas.Expendios;
using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Comun.General;
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
    [Authorize(Roles = "1,2,3,4,8,11")]
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

            var Auditoria = await _iDbAdministracion.P_InsAuditoria(Convert.ToInt64(User.FindFirstValue("Identificacion")), "Ingreso Módulo", "Ingreso módulo Expendios/Registros", Convert.ToInt64(User.FindFirstValue("Identificacion")), HttpContext.Session.GetString("IpMaquina"));

            //ViewBag.ddlUnidadExpendio = new SelectList((await _iDbSeguimientoIris.F_GetUnidadesSeguimiento()).Data?.OrderBy(x => x.DESCRIPCION_DEPENDENCIA), "SIGLA", "DESCRIPCION_DEPENDENCIA");

            // Obtener todos los roles del usuario desde los claims
            var rolesUsuario = User.Claims
                                   .Where(c => c.Type == ClaimTypes.Role)
                                   .Select(c => c.Value)
                                   .ToList();

            // Obtener la sigla física del usuario desde los claims
            var siglaFisicaUsuario = User.Claims.FirstOrDefault(c => c.Type == "Fisica")?.Value;

            IEnumerable<DtoDominios> unidades;

            // 🔑 Si el usuario tiene el rol 8, aplica el filtro
            if (rolesUsuario.Contains("8"))
            {
                unidades = (await _iDbSeguimientoIris.F_GetUnidadesSeguimiento())
                                .Data?
                                .Where(x => x.SIGLA == siglaFisicaUsuario)
                                .OrderBy(x => x.DESCRIPCION_DEPENDENCIA);
            }
            else
            {
                // 🔑 Si no tiene el rol 8, carga todos los registros
                unidades = (await _iDbSeguimientoIris.F_GetUnidadesSeguimiento())
                                .Data?
                                .OrderBy(x => x.DESCRIPCION_DEPENDENCIA);
            }

            // Asignar al ViewBag
            ViewBag.ddlUnidadExpendio = new SelectList(unidades, "SIGLA", "DESCRIPCION_DEPENDENCIA");

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
                //var consecutivo = resultado.Data.ToString();

                //consecutivo = ClsEncriptar.Encriptar(consecutivo);
                return Json(new { success = true, data = resultado.Data, message = resultado.Mensaje });
            }
            else
            {
                return Json(new { success = false, data = resultado.Data, message = resultado.Mensaje });
            }
        }




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

            var resultado = await _iDbRegistroExpendio.F_GetInfoGrillas(V_Anio, rolesUsuario, codigoUnidad);

            // Caso 1: Hay datos
            if (resultado.IdRespuesta == 1)
            {
                return Json(new { success = true, data = resultado.Data });
            }

            // Caso 2: No hay datos (NO ES ERROR)
            if (resultado.IdRespuesta == 0 && resultado.Mensaje == "No se encontraron datos")
            {
                return Json(new { success = true, data = new List<object>() });
            }

            // Caso 3: Error real
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { success = false, message = resultado.Mensaje });
        }


        [HttpGet]
        public async Task<IActionResult> ExportarExcel(string filtro, int anio)
        {

            var Auditoria = await _iDbAdministracion.P_InsAuditoria(Convert.ToInt64(User.FindFirstValue("Identificacion")), "Descargar Reporte", "Excel Expendios", Convert.ToInt64(User.FindFirstValue("Identificacion")), HttpContext.Session.GetString("IpMaquina"));


            var codigoUnidad = Convert.ToInt64(User.FindFirstValue("IdUndeLabora"));

            // Obtener roles del usuario
            var rolesUsuario = string.Join(",",
                User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
            );

            // Obtener datos desde la BD
            var resultado = await _iDbRegistroExpendio.F_GetInfoGrillas(anio, rolesUsuario, codigoUnidad);

            if (resultado.IdRespuesta == 0)
                return StatusCode(500, new { success = false, message = resultado.Mensaje });

            var lista = resultado.Data;

            // Aplicar filtro si existe
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtro = filtro.ToUpper().Trim();

                lista = lista.Where(x =>
                    (x.Estado?.ToUpper().Contains(filtro) ?? false) ||
                    (x.Codigo?.ToUpper().Contains(filtro) ?? false) ||
                    (x.UnidadInformaDescripcion?.ToUpper().Contains(filtro) ?? false) ||
                    (x.SiglaUnidadInforma?.ToUpper().Contains(filtro) ?? false) ||
                    (x.RegionDescripcion?.ToUpper().Contains(filtro) ?? false) ||
                    (x.Unidad?.ToUpper().Contains(filtro) ?? false) ||
                    (x.Zona?.ToUpper().Contains(filtro) ?? false) ||
                    (x.Clase?.ToUpper().Contains(filtro) ?? false) ||
                    (x.Expendio?.ToUpper().Contains(filtro) ?? false) ||
                    (x.Fuente?.ToUpper().Contains(filtro) ?? false) ||
                    (x.Categoria?.ToUpper().Contains(filtro) ?? false) ||
                    (x.OtraCategoria?.ToUpper().Contains(filtro) ?? false) ||
                    (x.NombreMored?.ToUpper().Contains(filtro) ?? false) ||
                    (x.Barrio?.ToUpper().Contains(filtro) ?? false) ||
                    (x.Direccion?.ToUpper().Contains(filtro) ?? false) ||
                    (x.Municipio?.ToUpper().Contains(filtro) ?? false)
                ).ToList();
            }


            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Expendios");

            // ------------------------------------------
            // ENCABEZADO INSTITUCIONAL
            // ------------------------------------------
            ws.Cell(1, 1).Value = "POLICÍA NACIONAL DE COLOMBIA";
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 16;
            ws.Range(1, 1, 1, 26).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell(2, 1).Value = "Reporte de Expendios";
            ws.Cell(2, 1).Style.Font.Bold = true;
            ws.Cell(2, 1).Style.Font.FontSize = 14;
            ws.Range(2, 1, 2, 26).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell(3, 1).Value = $"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}";
            ws.Range(3, 1, 3, 26).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Range(4, 1, 4, 26).Merge(); // Fila vacía estética


            // ------------------------------------------
            // ENCABEZADOS DE LA TABLA
            // ------------------------------------------
            string[] headers = new[]
            {
        "Estado", "Codigo", "Unidad Informa", "Sigla", "Region", "Unidad Hecho", "Zona", "Clase",
        "Tipo Expendio", "Fuente", "Fecha Inicio Existencia", "Características Generales",
        "Categoría", "Otra Categoría", "Código Operación", "Nombre Operación",
        "NUNC", "SIEDCO", "Erradicado ?", "Barrio", "Direccion",
        "Latitud", "Longitud", "Cuadrante", "Municipio", "Fecha Creacion"
    };

            for (int i = 0; i < headers.Length; i++)
                ws.Cell(5, i + 1).Value = headers[i];

            ws.Range(5, 1, 5, headers.Length).Style.Font.Bold = true;


            // ------------------------------------------
            // ESCRIBIR DATOS
            // ------------------------------------------
            int fila = 6; // datos comienzan aquí

            foreach (var item in lista)
            {
                ws.Cell(fila, 1).Value = item.Estado;
                ws.Cell(fila, 2).Value = item.Codigo;
                ws.Cell(fila, 3).Value = item.UnidadInformaDescripcion;
                ws.Cell(fila, 4).Value = item.SiglaUnidadInforma;
                ws.Cell(fila, 5).Value = item.RegionDescripcion;
                ws.Cell(fila, 6).Value = item.Unidad;
                ws.Cell(fila, 7).Value = item.Zona;
                ws.Cell(fila, 8).Value = item.Clase;
                ws.Cell(fila, 9).Value = item.Expendio;
                ws.Cell(fila, 10).Value = item.Fuente;
                ws.Cell(fila, 11).Value = item.FechaInicioExistencia.ToString("dd/MM/yyyy HH:mm");
                ws.Cell(fila, 12).Value = item.CaracteristicasGenerales;
                ws.Cell(fila, 13).Value = item.Categoria;
                ws.Cell(fila, 14).Value = item.OtraCategoria;
                ws.Cell(fila, 15).Value = item.CodigoMored;
                ws.Cell(fila, 16).Value = item.NombreMored;
                ws.Cell(fila, 17).Value = item.Nunc;
                ws.Cell(fila, 18).Value = item.Siedco;
                ws.Cell(fila, 19).Value = item.Erradicado;
                ws.Cell(fila, 20).Value = item.Barrio;
                ws.Cell(fila, 21).Value = item.Direccion;
                ws.Cell(fila, 22).Value = item.Latitud;
                ws.Cell(fila, 23).Value = item.Longitud;
                ws.Cell(fila, 24).Value = item.Cuadrante;
                ws.Cell(fila, 25).Value = item.Municipio;
                ws.Cell(fila, 26).Value = item.FechaCreacion.ToString("dd/MM/yyyy HH:mm");

                fila++;
            }


            // ------------------------------------------
            // AJUSTE DE ANCHO COLUMNAS
            // ------------------------------------------
            ws.Columns().AdjustToContents();


            // ------------------------------------------
            // RETORNO
            // ------------------------------------------
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Expendios_{anio}.xlsx");
        }



        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> F_GetEstaciones(string V_Sigla)
        {
            var resultado = await _iDbRegistroExpendio.F_GetEstaciones(V_Sigla);

            // Caso 1: Hay datos
            if (resultado.IdRespuesta == 1)
            {
                return Json(new { success = true, data = resultado.Data });
            }

            // Caso 2: No hay datos (NO ES ERROR)
            if (resultado.IdRespuesta == 0 && resultado.Mensaje == "No se encontraron datos")
            {
                return Json(new { success = true, data = new List<object>() });
            }

            // Caso 3: Error real
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { success = false, message = resultado.Mensaje });
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> F_GetEspecialidad(string V_Sigla)
        {
            var resultado = await _iDbRegistroExpendio.F_GetEspecialidad(V_Sigla);

            // Caso 1: Hay datos
            if (resultado.IdRespuesta == 1)
            {
                return Json(new { success = true, data = resultado.Data });
            }

            // Caso 2: No hay datos (NO ES ERROR)
            if (resultado.IdRespuesta == 0 && resultado.Mensaje == "No se encontraron datos")
            {
                return Json(new { success = true, data = new List<object>() });
            }

            // Caso 3: Error real
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { success = false, message = resultado.Mensaje });
        }


        [HttpGet]
        public async Task<IActionResult> P_GetIntegrantes(string V_CriminalidadId)
        {
            var resultado = await _iDbRegistroExpendio.P_GetIntegrantes(V_CriminalidadId);

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

            // Caso 1: Hay datos
            if (resultado.IdRespuesta == 1)
            {
                return Json(new { success = true, data = resultado.Data });
            }

            // Caso 2: No hay datos (NO ES ERROR)
            if (resultado.IdRespuesta == 0 && resultado.Mensaje == "No se encontraron datos")
            {
                return Json(new { success = true, data = new List<object>() });
            }

            // Caso 3: Error real
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { success = false, message = resultado.Mensaje });
        }


        [HttpGet]
        public async Task<IActionResult> F_GetBitacora(string V_CriminalidadId)
        {
            var resultado = await _iDbRegistroExpendio.F_GetBitacora(V_CriminalidadId);

            // Caso 1: Hay datos
            if (resultado.IdRespuesta == 1)
            {
                return Json(new { success = true, data = resultado.Data });
            }

            // Caso 2: No hay datos (NO ES ERROR)
            if (resultado.IdRespuesta == 0 && resultado.Mensaje == "No se encontraron datos")
            {
                return Json(new { success = true, data = new List<object>() });
            }

            // Caso 3: Error real
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { success = false, message = resultado.Mensaje });
        }



        [HttpGet]
        public async Task<IActionResult> F_GetResultados(string V_CriminalidadId)
        {
            var resultado = await _iDbRegistroExpendio.F_GetResultados(V_CriminalidadId);

            // Caso 1: Hay datos
            if (resultado.IdRespuesta == 1)
            {
                return Json(new { success = true, data = resultado.Data });
            }

            // Caso 2: No hay datos (NO ES ERROR)
            if (resultado.IdRespuesta == 0 && resultado.Mensaje == "No se encontraron datos")
            {
                return Json(new { success = true, data = new List<object>() });
            }

            // Caso 3: Error real
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { success = false, message = resultado.Mensaje });
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



        [HttpGet]
        public async Task<IActionResult> F_GetDominiosIris(Int32 V_Id)
        {
            //var resultado = await _iDbRegistroExpendio.F_GetIntegranteAll(V_Identificacion);
            var resultado = await _iDbDominios.F_GetDominiosIris(V_Id);

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



