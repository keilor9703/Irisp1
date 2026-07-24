$(document).ready(function () {

    $("#ddlAnioIris").on("change", function () {
        F_GetReporteVerificacion();
    });

    F_GetReporteVerificacion();


});



function F_GetReporteVerificacion() {
    $.ajax({
        type: 'GET',
        url: AppRoutes.ReportesGeneral.UrlGetReporteVerificacion,
        data: { V_Anio: $("#ddlAnioIris").val() },
        dataType: 'json',
        success: function (response) {
            if (response && response.success === true) {
                renderReporteVerificacion(response.data);
            } else {
                renderReporteVerificacion([]);
            }
        },
        error: function () {
            renderReporteVerificacion([]);
        }
    });
}

function renderReporteVerificacion(data) {

    $("#pn_GrillaVerificacionRep").removeClass('hidden');

    renderDataTable("#tbGrillaVerificacionRep", data, [

        { title: "Código IRISP1", data: "codigo_irisp" , class: "celdaCenter"},
        { title: "# Integrantes", data: "cuenta_integrantes", class: "celdaCenter" },
        { title: "# Ubicaciones", data: "cuenta_ubicaciones", class: "celdaCenter" },
        { title: "# Delitos Principales", data: "cuenta_delitos_p", class: "celdaCenter" },
        { title: "# Delitos Conexos", data: "cuenta_delitos_c", class: "celdaCenter" },
        { title: "# Información", data: "cuenta_informacion", class: "celdaCenter" },
        { title: "# Responsables", data: "cuenta_responsable", class: "celdaCenter" },
        { title: "# Documentos", data: "cuenta_documentos", class: "celdaCenter" },
        { title: "# Unidad Responsable", data: "cuenta_unidad_responsable", class: "celdaCenter" },
        { title: "CriminalidadId", data: "criminalidad_id", visible: false }
    ]);
}



// renderDataTable ahora vive en /js/IniciarTabla.js (compartida por todas las grillas del sitio).

$("#btnExcel").on("click", function () {
    let filtro = $(".dataTables_filter input").val() || "";
    let anio = $("#ddlAnioIris").val() || "";
    window.location.href =
        "Reportes/ReporteVerificacion/ExportarExcelReporteVerificacion?filtro=" + encodeURIComponent(filtro) +
        "&V_Anio=" + encodeURIComponent(anio);
});

$("#btnPdf").on("click", function () {
    let filtro = $(".dataTables_filter input").val() || "";
    let anio = $("#ddlAnioIris").val() || "";
    window.open(
        "Reportes/ReporteVerificacion/ExportarPdfReporteVerificacion?filtro=" + encodeURIComponent(filtro) +
        "&V_Anio=" + encodeURIComponent(anio),
        "_blank"
    );
});
