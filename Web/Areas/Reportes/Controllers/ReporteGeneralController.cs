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
   
        private readonly IDbReportesGeneral _IDbReportesGeneral;




        public ReporteGeneralController(IConfiguration iConfiguration, IDbAdministracion iDbAdministracion,  IDbReportesGeneral dbReportesGeneral)
        {
            _iDbAdministracion = iDbAdministracion;
            _configuration = iConfiguration;
             _IDbReportesGeneral  = dbReportesGeneral;


        }
        public async Task<ActionResult> ReporteGeneral()
        {

            var Auditoria = await _iDbAdministracion.P_InsAuditoria(Convert.ToInt64(User.FindFirstValue("Identificacion")), "Ingreso Módulo", "Ingreso módulo Reportes/ReportesGeneral", Convert.ToInt64(User.FindFirstValue("Identificacion")), HttpContext.Session.GetString("IpMaquina"));
            var ddlAnioIris = (await _IDbReportesGeneral.F_GetAniosIrisP1()).Data.ToList();
            var anioActual = ddlAnioIris.Any() ? ddlAnioIris.Max(x => x.AnoIrisp1) : (int?)null;

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
        "Municipio 2","Barrio","Dirección","Cantidad SPOA","NUNC","Cantidad SIEDCO","Origen"
    };

            for (int i = 0; i < headers.Length; i++)
                ws.Cell(5, i + 1).Value = headers[i];

            ws.Range("A5:AK5").Style
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
                ws.Cell(fila, 37).Value = x.Origen;

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


       

    }
}
