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
