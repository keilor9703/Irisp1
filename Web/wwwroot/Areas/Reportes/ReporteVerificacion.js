$(document).ready(function () {


    F_GetReporteVerificacion();


});



function F_GetReporteVerificacion() {
    $.ajax({
        type: 'GET',
        url: AppRoutes.ReportesGeneral.UrlGetReporteVerificacion,
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



// 🔧 Función utilitaria para inicializar o refrescar tablas
function renderDataTable(selector, datosFiltrados, columnas) {
    if ($.fn.dataTable.isDataTable(selector)) {
        // actualizar data en vez de recrear
        const table = $(selector).DataTable();
        table.clear();
        table.rows.add(datosFiltrados);
        table.draw();
        return;
    }

    $(selector).DataTable({
        data: datosFiltrados,
        language: glOpcionesIdioma,
        scrollX: true,
        // scrollY: 400,          // alto fijo para habilitar virtualización
        scroller: true,        // solo renderiza las filas visibles
        deferRender: true,     // retrasa render hasta que sean visibles
        autoWidth: false,
        responsive: false,

        columnDefs: [
            { targets: '_all', className: 'dt-head-center dt-body-center' },
            { targets: 3, width: '1%', className: 'no-wrap' }
        ],
        columns: columnas,

        lengthMenu: [
            [10, 25, 50, 100],
            ['10 registros', '25 registros', '50 registros', '100 registros']
        ],
        pageLength: 10,
        ordering: true,
        searching: true,
        paging: true,
        info: true
    });
}

$("#btnExcel").on("click", function () {
    let filtro = $(".dataTables_filter input").val() || "";
    window.location.href =
        "/Reportes/ReporteVerificacion/ExportarExcelReporteVerificacion?filtro=" + encodeURIComponent(filtro);
});

$("#btnPdf").on("click", function () {
    let filtro = $(".dataTables_filter input").val() || "";
    window.open(
        "/Reportes/ReporteVerificacion/ExportarPdfReporteVerificacion?filtro=" + encodeURIComponent(filtro),
        "_blank"
    );
});
