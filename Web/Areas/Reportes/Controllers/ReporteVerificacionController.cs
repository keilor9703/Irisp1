using ClosedXML.Excel;
using Comun.Areas.Reportes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Negocio.Gestion.Reportes;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.General;
using Negocio.Interfaz.Irisp1;
using Negocio.Interfaz.Reportes;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Security.Claims;

namespace Web.Areas.Reportes.Controllers
{
    [Area("Reportes")]
    [Authorize(Roles = "1,2,7")]
    public class ReporteVerificacionController : Controller
    {


        private readonly IDbAdministracion _iDbAdministracion;
        private readonly IConfiguration _configuration;

        private readonly IDbReporteVerificacion _IDbReporteVerificacion;
        private readonly IDbReportesGeneral _iDbReportesGeneral;


        public ReporteVerificacionController(IConfiguration iConfiguration, IDbAdministracion iDbAdministracion,
            IDbReporteVerificacion dbReporteVerificacion, IDbReportesGeneral iDbReportesGeneral)
        {
            _iDbAdministracion = iDbAdministracion;
            _configuration = iConfiguration;
            _IDbReporteVerificacion = dbReporteVerificacion;
            _iDbReportesGeneral = iDbReportesGeneral;


        }

        public async Task<IActionResult> ReporteVerificacion()
        {
            var ddlAnioIris = (await _iDbReportesGeneral.F_GetAniosIrisP1()).Data?.ToList() ?? new List<DtoAnioR>();
            var anioActual = ddlAnioIris.Any() ? ddlAnioIris.Max(x => x.AnoIrisp1) : (int?)null;

            ViewBag.ddlAnioIris = new SelectList(ddlAnioIris, "AnoIrisp1", "AnoIrisp1", anioActual);

            return View();
        }

        private string RolesUsuario() => string.Join(",",
            User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value));

        private long CodigoUnidad() => Convert.ToInt64(User.FindFirstValue("IdUndeLabora"));

        [HttpGet]
        public async Task<IActionResult> F_GetReporteVerificacion(int? V_Anio)
        {
            var res = await _IDbReporteVerificacion.F_GetReporteVerificacion(V_Anio, RolesUsuario(), CodigoUnidad());

            if (res.IdRespuesta > 0)
                return Json(new { success = true, data = res.Data });
            else
                return Json(new { success = false, message = res.Mensaje });
        }



        [HttpGet]
        public async Task<IActionResult> ExportarExcelReporteVerificacion(int? V_Anio, string filtro)
        {
            // Auditoría
            await _iDbAdministracion.P_InsAuditoria(
                Convert.ToInt64(User.FindFirstValue("Identificacion")),
                "Exportar Reporte",
                "EXCEL Reporte Verificación IRIS-P1",
                Convert.ToInt64(User.FindFirstValue("Identificacion")),
                HttpContext.Session.GetString("IpMaquina")
            );

            var resultado = await _IDbReporteVerificacion.F_GetReporteVerificacion(V_Anio, RolesUsuario(), CodigoUnidad());

            if (resultado.IdRespuesta == 0)
                return StatusCode(500, new { success = false, message = resultado.Mensaje });

            var lista = resultado.Data;

            // -----------------------------
            // APLICAR FILTRO SI EXISTE
            // -----------------------------
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtro = filtro.ToUpper().Trim();

                lista = lista.Where(x =>
                    (x.CodigoIrisp?.ToUpper().Contains(filtro) ?? false) ||
                    x.CuentaIntegrantes.ToString().Contains(filtro) ||
                    x.CuentaUbicaciones.ToString().Contains(filtro) ||
                    x.CuentaDelitosP.ToString().Contains(filtro) ||
                    x.CuentaDelitosC.ToString().Contains(filtro) ||
                    x.CuentaInformacion.ToString().Contains(filtro) ||
                    x.CuentaResponsable.ToString().Contains(filtro) ||
                    x.CuentaDocumentos.ToString().Contains(filtro) ||
                    x.CuentaUnidadResponsable.ToString().Contains(filtro)
                ).ToList();
            }

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Reporte Verificación");

            // ENCABEZADO
            ws.Cell("A1").Value = "POLICÍA NACIONAL DE COLOMBIA";
            ws.Range("A1:I1").Merge().Style
                .Font.SetBold().Font.SetFontSize(16)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell("A2").Value = "Sistema de Información IRIS-P1 – Reporte Verificación";
            ws.Range("A2:I2").Merge().Style
                .Font.SetBold()
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell("A3").Value = $"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}";
            ws.Range("A3:I3").Merge()
                .Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

            ws.Row(4).Height = 8;

            // ENCABEZADOS
            string[] headers = {
        "Código IRISP1",
        "# Integrantes",
        "# Ubicaciones",
        "# Delitos Principales",
        "# Delitos Conexos",
        "# Información",
        "# Responsables",
        "# Documentos",
        "# Unidad Responsable"
    };

            for (int i = 0; i < headers.Length; i++)
                ws.Cell(5, i + 1).Value = headers[i];

            ws.Range("A5:I5").Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.FromHtml("#D9E1F2"));

            // DETALLE
            int fila = 6;
            foreach (var x in lista)
            {
                ws.Cell(fila, 1).Value = x.CodigoIrisp;
                ws.Cell(fila, 2).Value = x.CuentaIntegrantes;
                ws.Cell(fila, 3).Value = x.CuentaUbicaciones;
                ws.Cell(fila, 4).Value = x.CuentaDelitosP;
                ws.Cell(fila, 5).Value = x.CuentaDelitosC;
                ws.Cell(fila, 6).Value = x.CuentaInformacion;
                ws.Cell(fila, 7).Value = x.CuentaResponsable;
                ws.Cell(fila, 8).Value = x.CuentaDocumentos;
                ws.Cell(fila, 9).Value = x.CuentaUnidadResponsable;
                fila++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);

            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Reporte_Verificacion_IRISP1.xlsx");
        }


        [HttpGet]
        public async Task<IActionResult> ExportarPdfReporteVerificacion(int? V_Anio, string filtro)
        {
            await _iDbAdministracion.P_InsAuditoria(
                Convert.ToInt64(User.FindFirstValue("Identificacion")),
                "Exportar Reporte",
                "PDF Reporte Verificación IRIS-P1",
                Convert.ToInt64(User.FindFirstValue("Identificacion")),
                HttpContext.Session.GetString("IpMaquina")
            );

            var resultado = await _IDbReporteVerificacion.F_GetReporteVerificacion(V_Anio, RolesUsuario(), CodigoUnidad());

            if (resultado.IdRespuesta == 0)
                return StatusCode(500, new { success = false, message = resultado.Mensaje });

            var lista = resultado.Data;

            // APLICAR FILTRO
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtro = filtro.ToUpper().Trim();

                lista = lista.Where(x =>
                    (x.CodigoIrisp?.ToUpper().Contains(filtro) ?? false) ||
                    x.CuentaIntegrantes.ToString().Contains(filtro) ||
                    x.CuentaUbicaciones.ToString().Contains(filtro) ||
                    x.CuentaDelitosP.ToString().Contains(filtro) ||
                    x.CuentaDelitosC.ToString().Contains(filtro) ||
                    x.CuentaDocumentos.ToString().Contains(filtro) ||
                    x.CuentaResponsable.ToString().Contains(filtro) ||
                    x.CuentaInformacion.ToString().Contains(filtro) ||
                    x.CuentaUnidadResponsable.ToString().Contains(filtro)
                ).ToList();
            }

            var pdfBytes = GeneratePdfReporteVerificacion(lista);

            return File(pdfBytes, "application/pdf", "Reporte_Verificacion.pdf");
        }

        public static byte[] GeneratePdfReporteVerificacion(List<DtoReporteVerificacion> data)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // ============================
                    // ENCABEZADO
                    // ============================
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("POLICÍA NACIONAL DE COLOMBIA - IRIS-P1")
                                .Bold().FontSize(14);
                            col.Item().Text("Reporte de Verificación de IRISP1").FontSize(12);
                            col.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
                        });
                    });

                    // ============================
                    // TABLA PRINCIPAL
                    // ============================
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Código
                            columns.RelativeColumn(2); // Integrantes
                            columns.RelativeColumn(2); // Ubicaciones
                            columns.RelativeColumn(2); // Delitos P
                            columns.RelativeColumn(2); // Delitos C
                            columns.RelativeColumn(2); // Información
                            columns.RelativeColumn(2); // Responsables
                            columns.RelativeColumn(2); // Documentos
                            columns.RelativeColumn(3); // Unidad Responsable
                        });

                        // ENCABEZADOS
                        table.Header(header =>
                        {
                            header.Cell().Text("Código IRISP1").Bold();
                            header.Cell().Text("# Integrantes").Bold();
                            header.Cell().Text("# Ubicaciones").Bold();
                            header.Cell().Text("# Delitos P").Bold();
                            header.Cell().Text("# Delitos C").Bold();
                            header.Cell().Text("# Información").Bold();
                            header.Cell().Text("# Responsables").Bold();
                            header.Cell().Text("# Documentos").Bold();
                            header.Cell().Text("# Unidad Responsable").Bold();
                        });

                        // FILAS
                        foreach (var item in data)
                        {
                            table.Cell().Text(item.CodigoIrisp);
                            table.Cell().Text(item.CuentaIntegrantes.ToString());
                            table.Cell().Text(item.CuentaUbicaciones.ToString());
                            table.Cell().Text(item.CuentaDelitosP.ToString());
                            table.Cell().Text(item.CuentaDelitosC.ToString());
                            table.Cell().Text(item.CuentaInformacion.ToString());
                            table.Cell().Text(item.CuentaResponsable.ToString());
                            table.Cell().Text(item.CuentaDocumentos.ToString());
                            table.Cell().Text(item.CuentaUnidadResponsable.ToString());
                        }
                    });

                    // ============================
                    // PIE DE PÁGINA
                    // ============================
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
