"use strict"

function poblarTabla(tabla) {

    $('#' + tabla).DataTable({
        "language": glOpcionesIdioma
    })
}

function poblarTablaCk(tabla) {

    $('#' + tabla).DataTable({
        "language": glOpcionesIdioma,
        'columnDefs': [
            {
                'targets': 0,
                'checkboxes': {
                    'selectRow': true
                }
            }
        ],
        'select': {
            'style': 'multi'
        },
        'order': [[1, 'asc']]
    })
}

function AplicarPluginTablaAjaxServerSidePro(tablaId, ruta, columnas) {
    $("#" + tablaId).DataTable({
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": true, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once 
        "stateSave": true, //Guardar el ordenado realizado por el usuario
        "ajax": {
            "url": ruta,
            "type": "POST",
            "datatype": "json"
        },
        "columns": columnas,
        "language": glOpcionesIdioma,
    });
}

function AplicarPluginTablaAjax(tablaId, ruta, columnas) {
    $("#" + tablaId).DataTable({        
        "ajax": {
            "url": ruta,
            "type": "POST",
            "datatype": "json"
        },
        "columns": columnas,
        "language": glOpcionesIdioma,
    });
}

function AplicarPluginTablaBotonExportacion(tablaId, ruta, columnas) {
    if ($.fn.dataTable.isDataTable("#" + tablaId)) {
        $("#" + tablaId).DataTable().destroy();
    }
    $("#" + tablaId).DataTable({
        ajax: {
            "url": ruta,
            "type": "POST",
            "datatype": "json",
            cache: false
        },
        columns: columnas,
        language: glOpcionesIdioma,        
        dom: 'Bfrtip',
        lengthMenu: [
            [10, 25, 50, -1],
            ['10 registros', '25 registros', '50 registros', 'Todos']
        ],
        buttons: [
            'pageLength', 'copy', 'csv', 'excel', 'pdf', 'print'
        ],

        error: function (ex) {
            alert('Seleccione una unidad de la lista !!!');
        }

    } );
}

function AplicarPluginTablas(tablaId, ruta, columnas) {

    $("#" + tablaId).DataTable({
        ruta,
        columns: columnas,
        language: glOpcionesIdioma,
        dom: 'Bfrtip',
        lengthMenu: [
            [10, 25, 50, -1],
            ['10 registros', '25 registros', '50 registros', 'Todos']
        ],
        buttons: [
            'pageLength', 'copy', 'csv', 'excel', 'pdf', 'print'
        ]
    });
}

// Inicialización/actualización estándar de una grilla DataTables alimentada por AJAX manual
// (no por el "ajax" nativo de DataTables). Reemplaza las ~7 copias casi idénticas de esta misma
// función que había en RegistrosIrisp1.js, SeguimientoIrisp1.js, VerificacionIris.js,
// BuscarIntegrantes.js, RegistrarIntegrantes.js, ReportesGeneral.js y ReporteVerificacion.js.
//
// opciones (todas opcionales):
//   - columnDefs: sobrescribe el columnDefs por defecto
//   - preserveDraw: si es true, al refrescar mantiene la página/orden actual (table.draw(false))
//     en vez de volver a la página 1 (table.draw())
function renderDataTable(selector, datosFiltrados, columnas, opciones) {
    opciones = opciones || {};

    if ($.fn.dataTable.isDataTable(selector)) {
        const table = $(selector).DataTable();
        table.clear();
        table.rows.add(datosFiltrados);
        table.draw(opciones.preserveDraw === true ? false : undefined);
        return table;
    }

    return $(selector).DataTable({
        data: datosFiltrados,
        language: glOpcionesIdioma,
        scrollX: true,
        scroller: true,     // solo renderiza las filas visibles
        deferRender: true,  // retrasa render hasta que sean visibles
        autoWidth: false,
        responsive: false,

        columnDefs: opciones.columnDefs || [
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

// ============================================================================
// Badges de estado para las columnas "Estado"/"Estado Existencia" de las grillas.
// Antes cada módulo (RegistrosIrisp1, Seguimiento, Verificacion, BuscarIntegrantes,
// ReportesGeneral) tenía su propia copia de esta lógica con colores hex hardcodeados
// (algunos con contraste insuficiente, ej. #40a8c7 con texto blanco) y comentarios que
// describían mal el color (ej. "// azul" sobre un verde). Ahora es una sola tabla de
// clases Bootstrap semánticas, reutilizada por todos los módulos.
// ============================================================================
var COLORES_ESTADO_GRILLA = {
    'sin asignar': 'bg-danger',
    'no existe': 'bg-danger',
    'rechazada': 'bg-danger',
    'descartado': 'bg-danger',
    'asignado': 'bg-success',
    'si existe': 'bg-success',
    'aceptada': 'bg-success',
    'verificación': 'bg-success',
    'avance verificación': 'bg-info text-dark',
    'avance investigación': 'bg-info text-dark',
    'investigación': 'bg-primary',
    'respondida': 'bg-primary',
    'finalizado': 'bg-dark'
};

function badgeEstadoGrilla(valor, minWidth) {
    minWidth = minWidth || 120;

    if (!valor) {
        return '<span class="badge bg-secondary" style="min-width:' + minWidth + 'px;">Por establecer</span>';
    }

    var clase = COLORES_ESTADO_GRILLA[valor.toLowerCase()] || 'bg-secondary';
    return '<span class="badge ' + clase + '" style="min-width:' + minWidth + 'px;">' + valor + '</span>';
}

// Devuelve una definición de columna de DataTables lista para usar en el arreglo "columns".
function columnaEstadoGrilla(title, dataField) {
    return {
        title: title,
        data: dataField,
        name: dataField,
        autoWidth: true,
        render: function (data) { return badgeEstadoGrilla(data); }
    };
}

// ============================================================================
// Indicador de carga global: se muestra automáticamente mientras hay peticiones
// AJAX en curso (jQuery ajaxStart/ajaxStop), para que el usuario tenga feedback
// visual en vez de ver la pantalla en blanco durante los varios segundos que
// puede tardar la carga de una grilla.
// ============================================================================
(function () {
    var $overlay;

    function crearOverlay() {
        if ($overlay) return $overlay;

        $("<style>")
            .prop("type", "text/css")
            .html(
                "#GrillaLoadingOverlay{position:fixed;top:0;left:0;right:0;z-index:99999;" +
                "display:none;background:#0d6efd;height:3px;overflow:hidden;}" +
                "#GrillaLoadingOverlay .bar{position:absolute;top:0;left:0;height:100%;width:30%;" +
                "background:#66a3ff;animation:GrillaLoadingBar 1s linear infinite;}" +
                "@keyframes GrillaLoadingBar{0%{left:-30%;}100%{left:100%;}}"
            )
            .appendTo("head");

        $overlay = $('<div id="GrillaLoadingOverlay"><div class="bar"></div></div>').appendTo("body");
        return $overlay;
    }

    $(function () {
        var $ov = crearOverlay();

        // jQuery ya agrupa las peticiones concurrentes: ajaxStart dispara una sola vez cuando
        // empieza la primera petición activa, ajaxStop una sola vez cuando termina la última.
        $(document).on("ajaxStart", function () {
            $ov.stop(true, true).show();
        });

        $(document).on("ajaxStop", function () {
            $ov.hide();
        });
    });
})();

//Configuración del idioma
var glOpcionesIdioma = {
    search: '<span>Buscar:</span> _INPUT_',
    lengthMenu: '<span>Mostrar:</span> _MENU_',
    paginate: { 'first': 'Primero', 'last': 'Último', 'next': '→', 'previous': '←' },
    info: "Mostrando _START_ a _END_ de _TOTAL_ registros.",
    infoEmpty: "Mostrando _START_ a _END_ de _TOTAL_ registros.",
    loadingRecords: "Cargando registros...",
    zeroRecords: "No se han encontrado registros",
    processing: "Procesando...",
    infoFiltered: "(Filtrados de _MAX_ registros.)",
    oPaginate: {
        "sFirst": "Primero",
        "sLast": "Último",
        "sNext": "Siguiente",
        "sPrevious": "Anterior"
    }
};
