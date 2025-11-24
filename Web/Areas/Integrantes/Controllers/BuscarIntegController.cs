using ClosedXML.Excel;
using Comun.Areas.Integrantes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.Gestion.General;
using Negocio.Gestion.Integrantes;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.General;
using Negocio.Interfaz.Integrantes;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Security.Claims;

namespace Web.Areas.Integrantes.Controllers
{


    [Area("Integrantes")]
    [Authorize(Roles = "1,2,3,4,5,6,7,8,11")]
    public class BuscarIntegController : Controller
    {

        private readonly IConfiguration _iConfiguration;
        private readonly IDbAdministracion _iDbAdministracion;
       private readonly IDbBuscarIntegrantes _iDbBuscarIntegrantes;
        private readonly IDbDominios _iDbDominios;



        public BuscarIntegController(IConfiguration iConfiguration, IDbAdministracion dbAdministracion, 
            IDbBuscarIntegrantes dbBuscarIntegrantes,
            IDbDominios iDbDominios)
        {

            _iConfiguration = iConfiguration;
            _iDbAdministracion = dbAdministracion;
            _iDbBuscarIntegrantes = dbBuscarIntegrantes;
            _iDbDominios = iDbDominios;
        }



        public IActionResult BuscarIntegrantes()
        {
            return View();
        }





        [HttpGet]
        public async Task<IActionResult> F_GetIntegrantesPorId(long V_Identificacion)
        {
            var resultado = await _iDbBuscarIntegrantes.F_GetIntegrantesPorId(V_Identificacion);

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
        public async Task<IActionResult> F_GetListaIris(long V_Identificacion)
        {
            var resultado = await _iDbBuscarIntegrantes.F_GetListaIris(V_Identificacion);

            if (resultado.IdRespuesta > 0)
                return Json(new { success = true, data = resultado.Data });

            return Json(new { success = false, message = resultado.Mensaje });
        }

        [HttpGet]
        public async Task<IActionResult> F_GetLogPorIdentificacion(long V_Identificacion)
        {
            var res = await _iDbBuscarIntegrantes.F_GetLogPorIdentificacion(V_Identificacion);

            if (res.IdRespuesta > 0)
                return Json(new { success = true, data = res.Data });
            else
                return Json(new { success = false, message = res.Mensaje });
        }


        [HttpGet]
        public async Task<IActionResult> ExportarExcelListaIris(Int64 V_Identificacion)
        {


            // Auditoría institucional
            await _iDbAdministracion.P_InsAuditoria(
                Convert.ToInt64(User.FindFirstValue("Identificacion")),
                "Exportar Reporte",
                "EXCEL Lista Iris por Integrante",
                Convert.ToInt64(User.FindFirstValue("Identificacion")),
                HttpContext.Session.GetString("IpMaquina")
            );

            var resultado = await _iDbBuscarIntegrantes.F_GetListaIris(V_Identificacion); // ID se envía por filtro

            if (resultado.IdRespuesta == 0)
                return StatusCode(500, new { success = false, message = resultado.Mensaje });

            var lista = resultado.Data;

          
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Listado IRIS");

            // -------------------------
            // ENCABEZADO INSTITUCIONAL
            // -------------------------
            ws.Cell("A1").Value = "POLICÍA NACIONAL DE COLOMBIA";
            ws.Range("A1:O1").Merge().Style
                .Font.SetBold()
                .Font.SetFontSize(16)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell("A2").Value = "Sistema de Información IRIS-P1 – Reporte de Iris Integrante";
            ws.Range("A2:O2").Merge().Style
                .Font.SetBold()
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell("A3").Value = "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            ws.Range("A3:O3").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

            ws.Row(4).Height = 8;

            // -------------------------
            // ENCABEZADOS TABLA
            // -------------------------
            ws.Cell(5, 1).Value = "Código";
            ws.Cell(5, 2).Value = "Unidad";
            ws.Cell(5, 3).Value = "Municipio";
            ws.Cell(5, 4).Value = "Zona";
            ws.Cell(5, 5).Value = "Clase";
            ws.Cell(5, 6).Value = "Fuente";
            ws.Cell(5, 7).Value = "Nombre";
            ws.Cell(5, 8).Value = "Fecha Inicio";
            ws.Cell(5, 9).Value = "Cant. Integrantes";
            ws.Cell(5, 10).Value = "Características";
            ws.Cell(5, 11).Value = "Tipo Servicio";
            ws.Cell(5, 12).Value = "Unidad Verificación";
            ws.Cell(5, 13).Value = "Fecha Asig. Ver.";
            ws.Cell(5, 14).Value = "Fecha Resp. Ver.";
            ws.Cell(5, 15).Value = "Resultado";

            ws.Range("A5:O5").Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.FromHtml("#D9E1F2"));

            // -------------------------
            // CONTENIDO
            // -------------------------
            int fila = 6;
            foreach (var x in lista)
            {
                ws.Cell(fila, 1).Value = x.Codigo;
                ws.Cell(fila, 2).Value = x.Dependencia;
                ws.Cell(fila, 3).Value = x.Municipio;
                ws.Cell(fila, 4).Value = x.Zona;
                ws.Cell(fila, 5).Value = x.Clase;
                ws.Cell(fila, 6).Value = x.Fuente;
                ws.Cell(fila, 7).Value = x.NombreClase;
                ws.Cell(fila, 8).Value = x.FechaInicioExistencia;
                ws.Cell(fila, 9).Value = x.CantidadIntegrantes;
                ws.Cell(fila, 10).Value = x.CaracteristicasGenerales;
                ws.Cell(fila, 11).Value = x.TipoServicio;
                ws.Cell(fila, 12).Value = x.UnidadResponsable;
                ws.Cell(fila, 13).Value = x.FechaVerificacionExistencia;
                ws.Cell(fila, 14).Value = x.FechaRespuestaVerificacion;
                ws.Cell(fila, 15).Value = x.Resultados;
                fila++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Listado_IRISP1.xlsx");
        }



        [HttpGet]
        public async Task<IActionResult> ExportarExcelUbicaciones(Int64 V_Identificacion)
        {

            // Auditoría institucional
            await _iDbAdministracion.P_InsAuditoria(
                Convert.ToInt64(User.FindFirstValue("Identificacion")),
                "Exportar Reporte",
                "EXCEL Ubicación Anecedentes por Integrante",
                Convert.ToInt64(User.FindFirstValue("Identificacion")),
                HttpContext.Session.GetString("IpMaquina")
            );



            var resultado = await _iDbBuscarIntegrantes.F_GetLogPorIdentificacion(V_Identificacion);

            if (resultado.IdRespuesta == 0)
                return StatusCode(500, new { success = false, message = resultado.Mensaje });

            var lista = resultado.Data;


            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Ubicaciones PDA");

            ws.Cell("A1").Value = "POLICÍA NACIONAL DE COLOMBIA";
            ws.Range("A1:H1").Merge().Style.Font.SetBold().Font.SetFontSize(16)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell("A2").Value = "Georreferenciación de Consultas POR PDA";
            ws.Range("A2:H2").Merge().Style.Font.SetBold()
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell("A3").Value = "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            ws.Cell(5, 1).Value = "Tipo";
            ws.Cell(5, 2).Value = "Nombres";
            ws.Cell(5, 3).Value = "Apellidos";
            ws.Cell(5, 4).Value = "Identificación";
            ws.Cell(5, 5).Value = "Latitud";
            ws.Cell(5, 6).Value = "Longitud";
            ws.Cell(5, 7).Value = "Fecha Consulta";

            int fila = 6;
            foreach (var item in lista)
            {
                ws.Cell(fila, 1).Value = item.Clase;
                ws.Cell(fila, 2).Value = item.Nombres;
                ws.Cell(fila, 3).Value = item.Apellidos;
                ws.Cell(fila, 4).Value = item.Identificacion;
                ws.Cell(fila, 5).Value = item.Latitud;
                ws.Cell(fila, 6).Value = item.Longitud;
                ws.Cell(fila, 7).Value = item.FechaCreacion;
                fila++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Ubicaciones_PDA.xlsx");
        }



        [HttpGet]
        public async Task<IActionResult> ExportarPdfListaIris(Int32 V_Identificacion)
        {
            // Auditoría institucional
            await _iDbAdministracion.P_InsAuditoria(
                Convert.ToInt64(User.FindFirstValue("Identificacion")),
                "Exportar Reporte",
                "PDF Lista Iris por Integrante",
                Convert.ToInt64(User.FindFirstValue("Identificacion")),
                HttpContext.Session.GetString("IpMaquina")
            );

            var resultado = await _iDbBuscarIntegrantes.F_GetListaIris(V_Identificacion);

            if (resultado.IdRespuesta == 0)
                return StatusCode(500, new { success = false, message = resultado.Mensaje });

            var lista = resultado.Data;

            // Generación del PDF (nuevo helper)
            byte[] pdfBytes = GeneratePdfListaIris(lista);

            return File(pdfBytes, "application/pdf", "Listado_IRISP1.pdf");
        }



        private byte[] GeneratePdfListaIris(List<DtoListaIrisGeneral> data)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.Letter);

                    // === HEADER UNIFICADO ===
                    page.Header().Column(column =>
                    {
                        column.Item().AlignCenter()
                            .Text("POLICÍA NACIONAL DE COLOMBIA")
                            .FontSize(16).Bold();

                        column.Item().AlignCenter()
                            .Text("Sistema de Información IRIS-P1 – Reporte de Ubicación del Integrante")
                            .FontSize(11).Bold();

                        column.Item().AlignLeft()
                            .Text($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    });

                    // === TABLA ===
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(60);  // Código
                            columns.RelativeColumn();    // Unidad
                            columns.RelativeColumn();    // Municipio
                            columns.RelativeColumn();    // Zona
                            columns.RelativeColumn();    // Clase
                            columns.RelativeColumn();    // Resultado
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background("#D9E1F2").Text("Código").Bold();
                            header.Cell().Background("#D9E1F2").Text("Unidad").Bold();
                            header.Cell().Background("#D9E1F2").Text("Municipio").Bold();
                            header.Cell().Background("#D9E1F2").Text("Zona").Bold();
                            header.Cell().Background("#D9E1F2").Text("Clase").Bold();
                            header.Cell().Background("#D9E1F2").Text("Resultado").Bold();
                        });

                        foreach (var x in data)
                        {
                            table.Cell().Text(x.Codigo);
                            table.Cell().Text(x.Dependencia);
                            table.Cell().Text(x.Municipio);
                            table.Cell().Text(x.Zona);
                            table.Cell().Text(x.Clase);
                            table.Cell().Text(x.Resultados);
                        }
                    });
                });
            });

            return document.GeneratePdf();
        }

        [HttpGet]
        public async Task<IActionResult> ExportarPdfUbicaciones(Int32 V_Identificacion)
        {
            await _iDbAdministracion.P_InsAuditoria(
                Convert.ToInt64(User.FindFirstValue("Identificacion")),
                "Exportar Reporte",
                "PDF Ubicaciones Antecedentes por Integrante",
                Convert.ToInt64(User.FindFirstValue("Identificacion")),
                HttpContext.Session.GetString("IpMaquina")
            );

            var resultado = await _iDbBuscarIntegrantes.F_GetLogPorIdentificacion(V_Identificacion);

            if (resultado.IdRespuesta == 0)
                return StatusCode(500, new { success = false, message = resultado.Mensaje });

            var lista = resultado.Data;

            byte[] pdfBytes = GeneratePdfUbicaciones(lista);

            return File(pdfBytes, "application/pdf", "Georeferenciacion_PDA.pdf");
        }


        private byte[] GeneratePdfUbicaciones(List<DtoAntecedentesLogs> data)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.Letter);

                    // ===============================
                    //       HEADER ÚNICO
                    // ===============================
                    page.Header().Column(column =>
                    {
                        column.Item().AlignCenter()
                            .Text("POLICÍA NACIONAL DE COLOMBIA")
                            .FontSize(16).Bold();

                        column.Item().AlignCenter()
                            .Text("Georreferenciación de Consultas PDA – IRISP1")
                            .FontSize(11).Bold();

                        column.Item().AlignLeft()
                            .Text($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    });

                    // ===============================
                    //            TABLA
                    // ===============================
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(60);  // Tipo
                            columns.RelativeColumn();    // Nombres
                            columns.RelativeColumn();    // Apellidos
                            columns.ConstantColumn(80);  // Identificación
                            columns.ConstantColumn(60);  // Latitud
                            columns.ConstantColumn(60);  // Longitud
                            columns.RelativeColumn();    // Fecha Consulta
                        });

                        // ---- ENCABEZADOS ----
                        table.Header(header =>
                        {
                            header.Cell().Background("#D9E1F2").Padding(4).Text("Tipo").Bold();
                            header.Cell().Background("#D9E1F2").Padding(4).Text("Nombres").Bold();
                            header.Cell().Background("#D9E1F2").Padding(4).Text("Apellidos").Bold();
                            header.Cell().Background("#D9E1F2").Padding(4).Text("ID").Bold();
                            header.Cell().Background("#D9E1F2").Padding(4).Text("Lat").Bold();
                            header.Cell().Background("#D9E1F2").Padding(4).Text("Lon").Bold();
                            header.Cell().Background("#D9E1F2").Padding(4).Text("Fecha Consulta").Bold();
                        });

                        // ---- FILAS ----
                        foreach (var x in data)
                        {
                            table.Cell().Padding(2).Text(x.Clase);
                            table.Cell().Padding(2).Text(x.Nombres);
                            table.Cell().Padding(2).Text(x.Apellidos);
                            table.Cell().Padding(2).Text(x.Identificacion);
                            table.Cell().Padding(2).Text(x.Latitud);
                            table.Cell().Padding(2).Text(x.Longitud);
                            table.Cell().Padding(2).Text(x.FechaCreacion);
                        }
                    });

                    // ===============================
                    //        FOOTER (opcional)
                    // ===============================
                    page.Footer().AlignCenter().Text(txt =>
                    {
                        txt.Span("Página ").FontSize(10);
                        txt.CurrentPageNumber().FontSize(10);
                        txt.Span(" de ").FontSize(10);
                        txt.TotalPages().FontSize(10);
                    });
                });
            });

            return document.GeneratePdf();
        }


    }
}
