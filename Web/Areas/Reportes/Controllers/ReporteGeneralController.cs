using ClosedXML.Excel;
using Comun.Areas.Integrantes;
using Comun.Areas.Reportes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class ReporteGeneralController : Controller
    {

        private readonly IDbAdministracion _iDbAdministracion;
        private readonly IConfiguration _configuration;
        private readonly IDbDominios _IDbDominios;
        private readonly IDbReportesGeneral _IDbReportesGeneral;




        public ReporteGeneralController(IConfiguration iConfiguration, IDbAdministracion iDbAdministracion, IDbVerificacionIris iDbVerificacionIris, IDbDominios idbDominios, IDbReportesGeneral dbReportesGeneral)
        {
            _iDbAdministracion = iDbAdministracion;
            _configuration = iConfiguration;
            _IDbDominios = idbDominios;
            _IDbDominios = idbDominios;
            _IDbReportesGeneral  = dbReportesGeneral;


        }
        public async Task<ActionResult> ReporteGeneral()
        {

            var Auditoria = await _iDbAdministracion.P_InsAuditoria(Convert.ToInt64(User.FindFirstValue("Identificacion")), "Ingreso Módulo", "Ingreso módulo Reportes/ReportesGeneral", Convert.ToInt64(User.FindFirstValue("Identificacion")), HttpContext.Session.GetString("IpMaquina"));
            var ddlAnioIris = (await _IDbReportesGeneral.F_GetAniosIrisP1()).Data.ToList();
            var anioActual = ddlAnioIris.Max(x => x.AnoIrisp1);

            //  Crea el SelectList con el año actual seleccionado por defecto
            ViewBag.ddlAnioIris = new SelectList(ddlAnioIris, "AnoIrisp1", "AnoIrisp1", anioActual);


            return View();
        }


        [HttpGet]
        public async Task<IActionResult> F_GetReporteGeneral(int anio)
        {

            var codigoUnidad = Convert.ToInt32(User.FindFirstValue("IdUndeLabora"));

            // 🔹 Obtener todos los roles del usuario separados por coma
            var rolesUsuario = string.Join(",",
                User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
            );


            var res = await _IDbReportesGeneral.F_GetReporteGeneral(rolesUsuario, codigoUnidad, anio);

            if (res.IdRespuesta > 0)
                return Json(new { success = true, data = res.Data });
            else
                return Json(new { success = false, message = res.Mensaje });
        }

        [HttpGet]
        public async Task<IActionResult> ExportarExcelReporteGeneral(int anio)
        {
            // Auditoría
            await _iDbAdministracion.P_InsAuditoria(
                Convert.ToInt64(User.FindFirstValue("Identificacion")),
                "Exportar Reporte",
                "EXCEL Reporte General IRIS-P1",
                Convert.ToInt64(User.FindFirstValue("Identificacion")),
                HttpContext.Session.GetString("IpMaquina")
            );

            var codigoUnidad = Convert.ToInt32(User.FindFirstValue("IdUndeLabora"));

            var rolesUsuario = string.Join(",",
                User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
            );

            var resultado = await _IDbReportesGeneral.F_GetReporteGeneral(rolesUsuario, codigoUnidad, anio);

            if (resultado.IdRespuesta == 0)
                return StatusCode(500, new { success = false, message = resultado.Mensaje });

            var lista = resultado.Data;


            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Reporte General");

            // --------------------------------------------------
            // ENCABEZADO
            // --------------------------------------------------
            ws.Cell("A1").Value = "POLICÍA NACIONAL DE COLOMBIA";
            ws.Range("A1:AI1").Merge().Style
                .Font.SetBold().Font.SetFontSize(16)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell("A2").Value = "Sistema de Información IRIS-P1 – Reporte General";
            ws.Range("A2:AI2").Merge().Style
                .Font.SetBold()
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell("A3").Value = $"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}";
            ws.Range("A3:AI3").Merge()
                .Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

            ws.Row(4).Height = 8;

            // --------------------------------------------------
            // ENCABEZADOS (todos los de la grilla JS)
            // --------------------------------------------------
            string[] headers = {
        "Estado","Estado Existencia","Código","Delito Principal","Región","Unidad",
        "Dependencia","Cuadrante","Municipio","Zona","Clase","Fuente","Tipo Servicio",
        "Nombre Clase","Fecha Inicio","Cantidad Integrantes","Características",
        "Fecha Creación","Funcionario Informa","Unidad Funcionario","Identificación Func.",
        "Descripción Trámite","Unidad Verificación","Fecha Asig. Ver.","Fecha Resp. Ver.",
        "Unidad Investigación","Fecha Asig. Inv.","Fecha Resp. Inv.","Longitud","Latitud",
        "Municipio 2","Barrio","Dirección","Cantidad SPOA","NUNC","Cantidad SIEDCO"
    };

            for (int i = 0; i < headers.Length; i++)
                ws.Cell(5, i + 1).Value = headers[i];

            ws.Range("A5:AI5").Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.FromHtml("#D9E1F2"));

            // --------------------------------------------------
            // CONTENIDO
            // --------------------------------------------------
            int fila = 6;
            foreach (var x in lista)
            {
                ws.Cell(fila, 1).Value = x.Estado;
                ws.Cell(fila, 2).Value = x.EstadoExistencia;
                ws.Cell(fila, 3).Value = x.Codigo;
                ws.Cell(fila, 4).Value = x.DelitoPrincipal;
                ws.Cell(fila, 5).Value = x.RegionP;
                ws.Cell(fila, 6).Value = x.SiglaUnidad;
                ws.Cell(fila, 7).Value = x.Dependencia;
                ws.Cell(fila, 8).Value = x.NroCuadrante;
                ws.Cell(fila, 9).Value = x.Municipio;
                ws.Cell(fila, 10).Value = x.Zona;
                ws.Cell(fila, 11).Value = x.Clase;
                ws.Cell(fila, 12).Value = x.Fuente;
                ws.Cell(fila, 13).Value = x.TipoServicio;
                ws.Cell(fila, 14).Value = x.NombreClase;
                ws.Cell(fila, 15).Value = x.FechaInicioExistenciaStr;
                ws.Cell(fila, 16).Value = x.CantidadIntegrante;
                ws.Cell(fila, 17).Value = x.CaracteristicasGenerales;
                ws.Cell(fila, 18).Value = x.FechaCreacionIrisp1Str;
                ws.Cell(fila, 19).Value = x.FuncionarioInforma;
                ws.Cell(fila, 20).Value = x.UnidadFuncionarioInforma;
                ws.Cell(fila, 21).Value = x.IdentificacionInforma;
                ws.Cell(fila, 22).Value = x.DescripcionTramite;
                ws.Cell(fila, 23).Value = x.UnidadVerifica;
                ws.Cell(fila, 24).Value = x.FechaAsigVerificaStr;
                ws.Cell(fila, 25).Value = x.FechaRespVerificaStr;
                ws.Cell(fila, 26).Value = x.UnidadAsignacionInves;
                ws.Cell(fila, 27).Value = x.FechaAsigInvesStr;
                ws.Cell(fila, 28).Value = x.FechaRespInvesStr;
                ws.Cell(fila, 29).Value = x.Longitud;
                ws.Cell(fila, 30).Value = x.Latitud;
                ws.Cell(fila, 31).Value = x.Municipio2;
                ws.Cell(fila, 32).Value = x.Barrio;
                ws.Cell(fila, 33).Value = x.Direccion;
                ws.Cell(fila, 34).Value = x.CantidadSpoa;
                ws.Cell(fila, 35).Value = x.Nunc;
                ws.Cell(fila, 36).Value = x.CantidadSiedco;

                fila++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);

            return File(
                ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Reporte_General_IRISP1.xlsx"
            );
        }


        [HttpGet]
        public async Task<IActionResult> ExportarPdfReporteGeneral(int anio)
        {
            await _iDbAdministracion.P_InsAuditoria(
                Convert.ToInt64(User.FindFirstValue("Identificacion")),
                "Exportar Reporte",
                "PDF Reporte General IRIS-P1",
                Convert.ToInt64(User.FindFirstValue("Identificacion")),
                HttpContext.Session.GetString("IpMaquina")
            );

            var codigoUnidad = Convert.ToInt32(User.FindFirstValue("IdUndeLabora"));

            var rolesUsuario = string.Join(",",
                User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value)
            );

            var resultado = await _IDbReportesGeneral.F_GetReporteGeneral(rolesUsuario, codigoUnidad, anio);

            if (resultado.IdRespuesta == 0)
                return StatusCode(500, new { success = false, message = resultado.Mensaje });

            byte[] pdf = GeneratePdfReporteGeneral(resultado.Data);

            return File(pdf, "application/pdf", "Reporte_General_IRISP1.pdf");
        }

        private string CleanText(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return input
                .Replace("•", "-")
                .Replace("●", "-")
                .Replace("▪", "-")
                .Replace("", "-")
                .Replace("\uF0B7", "-")
                .Replace("\u2022", "-") // bullet unicode
                .Trim();
        }

        private byte[] GeneratePdfReporteGeneral(List<DtoGeneralIrisp> data)
        {
            // Permite que QuestPDF no falle cuando el contenido se sale del ancho
            QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = false;
            QuestPDF.Settings.DocumentLayoutExceptionThreshold = 0;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Página horizontal con márgenes pequeños
                    page.Size(PageSizes.Letter.Landscape());
                    page.Margin(10);

                    // --------- ENCABEZADO ---------
                    page.Header().Column(col =>
                    {
                        col.Item().AlignCenter()
                            .Text("POLICÍA NACIONAL DE COLOMBIA")
                            .FontSize(14).Bold();

                        col.Item().AlignCenter()
                            .Text("Reporte General – IRIS-P1")
                            .FontSize(10).Bold();

                        col.Item().AlignLeft()
                            .Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    });

                    // --------- TABLA PRINCIPAL ---------
                    page.Content().Padding(2).Table(table =>
                    {
                        // Para evitar el error de overflow
                        table.ExtendHorizontal();

                        // Definir ancho fijo de columnas (ajustado para que TODO quepa)
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(55);  // Estado
                            columns.ConstantColumn(55);  // Estado Existencia
                            columns.ConstantColumn(60);  // Código
                            columns.ConstantColumn(60);  // Delito
                            columns.ConstantColumn(50);  // Región
                            columns.ConstantColumn(65);  // Unidad
                            columns.ConstantColumn(70);  // Dependencia
                            columns.ConstantColumn(50);  // Cuadrante
                            columns.ConstantColumn(60);  // Municipio
                            columns.ConstantColumn(50);  // Zona
                            columns.ConstantColumn(55);  // Clase
                            columns.ConstantColumn(55);  // Fuente
                            columns.ConstantColumn(55);  // Tipo servicio
                            columns.ConstantColumn(70);  // Nombre clase
                            columns.ConstantColumn(70);  // Fecha inicio
                            columns.ConstantColumn(50);  // Integrantes
                            columns.ConstantColumn(120); // Características
                            columns.ConstantColumn(70);  // Fecha creación
                            columns.ConstantColumn(80);  // Funcionario informa
                            columns.ConstantColumn(70);  // Unidad funcionario
                            columns.ConstantColumn(70);  // Identificación
                            columns.ConstantColumn(90);  // Descripción trámite
                            columns.ConstantColumn(80);  // Unidad Verificación
                            columns.ConstantColumn(80);  // Asig Verifica
                            columns.ConstantColumn(90);  // Resp Verifica
                            columns.ConstantColumn(85);  // Unidad investigativa
                            columns.ConstantColumn(80);  // Asig inves
                            columns.ConstantColumn(90);  // Resp inves
                            columns.ConstantColumn(60);  // Longitud
                            columns.ConstantColumn(60);  // Latitud
                            columns.ConstantColumn(60);  // Municipio 2
                            columns.ConstantColumn(60);  // Barrio
                            columns.ConstantColumn(90);  // Dirección
                            columns.ConstantColumn(60);  // SPOA
                            columns.ConstantColumn(60);  // NUNC
                            columns.ConstantColumn(60);  // Siedco
                        });

                        // --------- ENCABEZADOS ---------
                        table.Header(header =>
                        {
                            void Cell(string t) =>
                                header.Cell().Background("#D9E1F2").Padding(2).Text(t).FontSize(8).Bold();

                            Cell("Estado");
                            Cell("Exist.");
                            Cell("Código");
                            Cell("Delito");
                            Cell("Región");
                            Cell("Unidad");
                            Cell("Dependencia");
                            Cell("Cuadrante");
                            Cell("Municipio");
                            Cell("Zona");
                            Cell("Clase");
                            Cell("Fuente");
                            Cell("Tipo Serv.");
                            Cell("Nombre Clase");
                            Cell("Fecha Activ.");
                            Cell("Integrantes");
                            Cell("Características");
                            Cell("Fecha Creación");
                            Cell("Funcionario");
                            Cell("Unidad Func.");
                            Cell("Identificación");
                            Cell("Trámite");
                            Cell("Unidad Verifica");
                            Cell("Asig Ver.");
                            Cell("Resp Ver.");
                            Cell("Unidad Inves");
                            Cell("Asig Inv.");
                            Cell("Resp Inv.");
                            Cell("Long.");
                            Cell("Lat.");
                            Cell("Municipio 2");
                            Cell("Barrio");
                            Cell("Dirección");
                            Cell("SPOA");
                            Cell("NUNC");
                            Cell("SIEDCO");
                        });

                        // --------- CONTENIDO ---------
                        foreach (var x in data)
                        {
                            void C(object v) =>
                                table.Cell().Padding(1).Text(v?.ToString() ?? "").FontSize(7);

                            C(x.Estado);
                            C(x.EstadoExistencia);
                            C(x.Codigo);
                            C(x.DelitoPrincipal);
                            C(x.RegionP);
                            C(x.SiglaUnidad);
                            C(x.Dependencia);
                            C(x.NroCuadrante);
                            C(x.Municipio);
                            C(x.Zona);
                            C(x.Clase);
                            C(x.Fuente);
                            C(x.TipoServicio);
                            C(x.NombreClase);
                            C(x.FechaInicioExistenciaStr);
                            C(x.CantidadIntegrante);
                            C(x.CaracteristicasGenerales);
                            C(x.FechaCreacionIrisp1Str);
                            C(x.FuncionarioInforma);
                            C(x.UnidadFuncionarioInforma);
                            C(x.IdentificacionInforma);
                            C(x.DescripcionTramite);
                            C(x.UnidadVerifica);
                            C(x.FechaAsigVerificaStr);
                            C(x.FechaRespVerificaStr);
                            C(x.UnidadAsignacionInves);
                            C(x.FechaAsigInvesStr);
                            C(x.FechaRespInvesStr);
                            C(x.Longitud);
                            C(x.Latitud);
                            C(x.Municipio2);
                            C(x.Barrio);
                            C(x.Direccion);
                            C(x.CantidadSpoa);
                            C(x.Nunc);
                            C(x.CantidadSiedco);
                        }
                    });

                    // Pie de página
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ").FontSize(8);
                            x.CurrentPageNumber().FontSize(8);
                            x.Span(" de ").FontSize(8);
                            x.TotalPages().FontSize(8);
                        });
                });
            });

            return document.GeneratePdf();
        }




    }
}
