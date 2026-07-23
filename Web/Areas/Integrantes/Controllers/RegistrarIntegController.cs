using ClosedXML.Excel;
using Comun.Areas.Expendios;
using Comun.Areas.Integrantes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.General;
using Negocio.Interfaz.Integrantes;
using NuGet.Packaging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Security.Claims;


namespace Web.Areas.Integrantes.Controllers
{
    [Area("Integrantes")]
    [Authorize(Roles = "1,2,3,4,5,6,7,8,11")]
    public class RegistrarIntegController : Controller

    {
        private readonly IConfiguration _iConfiguration;
        private readonly IDbAdministracion _iDbAdministracion;
        private readonly IDbRegistroInteg _iDbRegistroInteg;
        private readonly IDbDominios _iDbDominios;
        private readonly ILogger<RegistrarIntegController> _logger;


        public RegistrarIntegController(IConfiguration iConfiguration, IDbAdministracion dbAdministracion,IDbRegistroInteg dbRegistroInteg, IDbDominios iDbDominios, ILogger<RegistrarIntegController> logger)
        {
            _iConfiguration = iConfiguration;
            _iDbAdministracion = dbAdministracion;
            _iDbRegistroInteg = dbRegistroInteg;
            _iDbDominios = iDbDominios;
            _logger = logger;
        }


        public async Task<ActionResult> RegistrarInteg()
        {
            var Auditoria = await _iDbAdministracion.P_InsAuditoria(Convert.ToInt64(User.FindFirstValue("Identificacion")), "Ingreso Módulo", "Ingreso módulo Integrantes/Registrar", Convert.ToInt64(User.FindFirstValue("Identificacion")), HttpContext.Session.GetString("IpMaquina"));
            ViewBag.ddlTipoReincidencia = new SelectList((await _iDbDominios.F_GetDominiosIris(110)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");
            ViewBag.ddlTipoReincidencia2 = new SelectList((await _iDbDominios.F_GetDominiosIris(110)).Data?.OrderBy(x => x.Descripcion), "IdDominio", "Descripcion");


            return View();
        }


        [HttpGet]
        public async Task<IActionResult> F_GetReincidentes()
        {
            var resultado = await _iDbRegistroInteg.F_GetReincidentes();

            if (resultado.IdRespuesta > 0)
            {
                return Json(new{  success = true, data = resultado.Data });
            }
            else
            {
                return StatusCode(500, new{ success = false, message = resultado.Mensaje});
            }
        }




        [HttpGet]
        public async Task<IActionResult> F_GetReincidentesPorId(Int64 V_Identificacion)
        {
            var resultado = await _iDbRegistroInteg.F_GetReincidentesPorId(V_Identificacion);

            if (resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = resultado.Data });
            }
            else
            {
                return Json(new { success = false, message = resultado.Mensaje });


            }
        }




        [HttpPost]
        public async Task<IActionResult> P_InsOrUpdReincidente(DtoReincidentes Obj_Reincidente)
        {

            try
            {
                var Resultado = await _iDbRegistroInteg.P_InsOrUpdReincidente(Obj_Reincidente, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina"));

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
                _logger.LogError(ex, "Error en P_InsOrUpdReincidente");
                return Json(new { success = false, data = 0, message = "Error: no fue posible guardar, intente nuevamente." });
            }

        }


        [HttpPost]
        public async Task<IActionResult> P_UpdReincidente(DtoReincidentes Obj_Reincidente)
        {

            try
            {
                var Resultado = await _iDbRegistroInteg.P_UpdReincidente(Obj_Reincidente, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina"));

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
                _logger.LogError(ex, "Error en P_UpdReincidente");
                return Json(new { success = false, data = 0, message = "Error: no fue posible guardar, intente nuevamente." });
            }

        }




        [HttpPost]
        public async Task<IActionResult> P_DellReincidente(DtoReincidentes Obj_Reincidente)
        {

            try
            {
                var Resultado = await _iDbRegistroInteg.P_DellReincidente(Obj_Reincidente, User.FindFirstValue("Identificacion"), HttpContext.Session.GetString("IpMaquina"));

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
                _logger.LogError(ex, "Error en P_DellReincidente");
                return Json(new { success = false, data = 0, message = "Error: no fue posible guardar, intente nuevamente." });
            }

        }





        [HttpGet]
        public async Task<IActionResult> ExportarExcel(string filtro)
        {

            var Auditoria = await _iDbAdministracion.P_InsAuditoria(Convert.ToInt64(User.FindFirstValue("Identificacion")), "Descargar Reporte", "Excel Reincidentes", Convert.ToInt64(User.FindFirstValue("Identificacion")), HttpContext.Session.GetString("IpMaquina"));

            var resultado = await _iDbRegistroInteg.F_GetReincidentes();

            if (resultado.IdRespuesta == 0)
                return StatusCode(500, new { success = false, message = resultado.Mensaje });

            var lista = resultado.Data;

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtro = filtro.ToUpper().Trim();
                lista = lista.Where(x =>
                    (x.TipoId?.ToUpper().Contains(filtro) ?? false) ||
                    (x.Nombre?.ToUpper().Contains(filtro) ?? false) ||
                    (x.Apellido?.ToUpper().Contains(filtro) ?? false) ||
                    x.Identificacion.ToString().Contains(filtro) ||
                    (x.Alias?.ToUpper().Contains(filtro) ?? false) ||
                    (x.Observacion?.ToUpper().Contains(filtro) ?? false)
                ).ToList();
            }

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Reincidentes");

            // =============================
            //  ENCABEZADO INSTITUCIONAL
            // =============================

            // Fila 1: Título general
            worksheet.Cell("A1").Value = "POLICÍA NACIONAL DE COLOMBIA";
            worksheet.Range("A1:F1").Merge().Style
                .Font.SetBold()
                .Font.SetFontSize(16)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            

            // Fila 3: Sistema
            worksheet.Cell("A2").Value = "Sistema de Información IRIS-P1 – Reporte de Personas Reincidentes";
            worksheet.Range("A2:F2").Merge().Style
                .Font.SetBold()
                .Font.SetFontSize(11)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // Fila 4: Fecha
            worksheet.Cell("A3").Value = "Fecha de generación: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            worksheet.Range("A3:F3").Merge().Style
                .Font.SetFontSize(10)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

            // Fila 5: Espacio
            worksheet.Row(4).Height = 8;

            // =============================
            // ENCABEZADOS DE LA TABLA
            // =============================

            worksheet.Cell(5, 1).Value = "Tipo reincidencia";
            worksheet.Cell(5, 2).Value = "Nombre";
            worksheet.Cell(5, 3).Value = "Apellido";
            worksheet.Cell(5, 4).Value = "Identificación";
            worksheet.Cell(5, 5).Value = "Alias";
            worksheet.Cell(5, 6).Value = "Observación";

            // Estilo encabezados
            worksheet.Range("A5:F5").Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.FromHtml("#D9E1F2"))
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // =============================
            // DETALLE DE LOS REGISTROS
            // =============================
            int fila = 6;
            foreach (var item in lista)
            {
                worksheet.Cell(fila, 1).Value = item.TipoId;
                worksheet.Cell(fila, 2).Value = item.Nombre;
                worksheet.Cell(fila, 3).Value = item.Apellido;
                worksheet.Cell(fila, 4).Value = item.Identificacion;
                worksheet.Cell(fila, 5).Value = item.Alias;
                worksheet.Cell(fila, 6).Value = item.Observacion;
                fila++;
            }

            // Ajustar automáticamente el ancho de las columnas
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reincidentes_IrisP1.xlsx");
        }


        [HttpGet]
        public async Task<IActionResult> ExportarPdf(string filtro)
        {

            var Auditoria = await _iDbAdministracion.P_InsAuditoria(Convert.ToInt64(User.FindFirstValue("Identificacion")), "Exportar Reporte", "PDF Reincidentes", Convert.ToInt64(User.FindFirstValue("Identificacion")), HttpContext.Session.GetString("IpMaquina"));

            var resultado = await _iDbRegistroInteg.F_GetReincidentes();

            if (resultado.IdRespuesta == 0)
                return StatusCode(500, new { success = false, message = resultado.Mensaje });

            var lista = resultado.Data;

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtro = filtro.ToUpper().Trim();
                lista = lista.Where(x =>
                    (x.TipoId?.ToUpper().Contains(filtro) ?? false) ||
                    (x.Nombre?.ToUpper().Contains(filtro) ?? false) ||
                    (x.Apellido?.ToUpper().Contains(filtro) ?? false) ||
                    x.Identificacion.ToString().Contains(filtro) ||
                    (x.Alias?.ToUpper().Contains(filtro) ?? false) ||
                    (x.Observacion?.ToUpper().Contains(filtro) ?? false)
                ).ToList();
            }

            var pdfBytes = GeneratePdf(lista);

            return File(pdfBytes, "application/pdf", "Reincidentes.pdf");
        }



        //[HttpGet]
        //public async Task<IActionResult> Imprimir(string filtro = "")
        //{
        //    var resultado = await _iDbRegistroInteg.F_GetReincidentes();

        //    var datos = resultado.Data;

        //    // aplicar filtro igual que arriba
        //    if (!string.IsNullOrWhiteSpace(filtro))
        //    {
        //        filtro = filtro.ToLower();

        //        datos = datos.Where(x =>
        //            (x.TipoId?.ToLower().Contains(filtro) ?? false) ||
        //            (x.Nombre?.ToLower().Contains(filtro) ?? false) ||
        //            (x.Apellido?.ToLower().Contains(filtro) ?? false) ||
        //            (x.Alias?.ToLower().Contains(filtro) ?? false) ||
        //            (x.Observacion?.ToLower().Contains(filtro) ?? false) ||
        //            x.Identificacion.ToString().Contains(filtro)
        //        ).ToList();
        //    }

        //    // ✔ ✔ ✔ AQUÍ REEMPLAZA EL ERROR
        //    var pdfBytes = GeneratePdf(datos);

        //    return File(pdfBytes, "application/pdf", "Reincidentes_Imprimir.pdf");
        //}




        public static byte[] GeneratePdf(List<DtoReincidentes> data)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;


            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // ENCABEZADO
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("POLICÍA NACIONAL DE COLOMBIA - IRIS-P1").Bold().FontSize(14);
                            col.Item().Text("Reporte de Reincidentes").FontSize(12);
                            col.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
                        });
                    });

                    // TABLA
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Tipo
                            columns.RelativeColumn(2); // Nombre
                            columns.RelativeColumn(2); // Apellido
                            columns.RelativeColumn(2); // Identificación
                            columns.RelativeColumn(2); // Alias
                            columns.RelativeColumn(3); // Observación
                        });

                        // ENCABEZADOS
                        table.Header(header =>
                        {
                            header.Cell().Text("Tipo reincidencia").Bold();
                            header.Cell().Text("Nombre").Bold();
                            header.Cell().Text("Apellido").Bold();
                            header.Cell().Text("Identificación").Bold();
                            header.Cell().Text("Alias").Bold();
                            header.Cell().Text("Observación").Bold();
                        });

                        // FILAS
                        foreach (var item in data)
                        {
                            table.Cell().Text(item.TipoId);
                            table.Cell().Text(item.Nombre);
                            table.Cell().Text(item.Apellido);
                            table.Cell().Text(item.Identificacion.ToString());
                            table.Cell().Text(item.Alias);
                            table.Cell().Text(item.Observacion ?? "");
                        }
                    });

                    // PIE DE PÁGINA
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ").FontSize(10);
                        x.CurrentPageNumber().FontSize(10);
                        x.Span(" de ").FontSize(10);
                        x.TotalPages().FontSize(10);
                    });
                });
            });

            return document.GeneratePdf();
        }



    }
}
