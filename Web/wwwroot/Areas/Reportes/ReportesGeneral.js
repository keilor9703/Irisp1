$(document).ready(function () {



    $('#ddlAnioIris').on('change', function () {
        F_GetInfoGrillas();
    });

    F_GetInfoGrillas($('#ddlAnioIris').val());



});


function F_GetInfoGrillas() {
    $.ajax({
        type: 'GET',
        url: AppRoutes.ReportesGeneral.UrlGetInfoGrillas,
        dataType: 'json',
        data: { anio: $('#ddlAnioIris').val() },
        success: function (response) {
            console.log("📩 Respuesta recibida:", response);

            // Validar si la respuesta es exitosa
            if (response && response.success === true) {
                let data = response.data || [];
                console.log("✅ Datos cargados:", data);
                GetGrillaLista(data);
            } else {
                console.warn("⚠️ Respuesta no exitosa o success=false");
                GetGrillaLista([]); // devolvemos grilla vacía
            }
        },
        error: function (xhr, status, error) {
            console.error("❌ Error Ajax:", status, error);
            console.error("Respuesta cruda:", xhr.responseText);
            GetGrillaLista([]); // devolvemos grilla vacía
        }
    });
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


function GetGrillaLista(Datos) {

    $("#pn_GrillaGeneral").removeClass('hidden');

    renderDataTable("#tbGrillaGeneral", Datos, [

        Estados(), // usa: estado_descripcion
        EstadosExistencia(), // usa: estado_existencia_descripcion

        { title: "Código", data: "codigo" },
        { title: "Delito Principal", data: "delito_principal" },
        { title: "Región", data: "region_p" },
        { title: "Unidad", data: "sigla_unidad" },
        { title: "Dependencia", data: "dependencia" },
        { title: "Cuadrante", data: "nro_cuadrante" },
        { title: "Municipio", data: "municipio" },
        { title: "Zona", data: "zona" },
        { title: "Clase", data: "clase" },
        { title: "Fuente", data: "fuente" },
        { title: "Tipo Servicio", data: "tipo_servicio" },
        { title: "Nombre Clase", data: "nombre_clase" },

        { title: "Fecha Inicio Actividad", data: "fecha_inicio_existencia_str" },

        { title: "Cantidad Integrantes", data: "cantidad_integrante" },

        columnaCaracteristicasGenerales(), // usa: caracteristicas_generales

        { title: "Fecha creación", data: "fecha_creacion_irisp1_str" },

        { title: "Funcionario Informa", data: "funcionario_informa" },
        { title: "Unidad Funcionario Informa", data: "unidad_funcionario_informa" },
        { title: "Identificación Funcionario", data: "identificacion_informa" },
        
        columnaDescripcionTramite(),

        { title: "Unidad Verificación", data: "unidad_verifica" },
        { title: "Fecha Asignación Verificación", data: "fecha_asig_tarea_verifica_str" },
        { title: "Fecha Respuesta Verificación", data: "fecha_resp_tarea_verifica_str" },

        { title: "Unidad Proceso Investigativo", data: "unidad_asig_inves" },
        { title: "Fecha Asignación Investigativa", data: "fecha_asig_tarea_inves_str" },
        { title: "Fecha Respuesta Investigativa", data: "fecha_resp_tarea_inves_str" },

        { title: "Longitud", data: "longitud" },
        { title: "Latitud", data: "latitud" },

        { title: "Municipio 2", data: "municipio_2" },
        { title: "Barrio", data: "barrio" },
        { title: "Dirección", data: "direccion" },

        { title: "Cantidad SPOA", data: "cantidad_spoa" },
        { title: "NUNC", data: "nunc" },
        { title: "Cantidad SIEDCO", data: "cantidad_siedco" },

        { title: "CriminalidadId", data: "criminalidad_id", visible: false }
    ]);
}



function Estados() {
    return {
        title: "Estado",
        data: "estado_descripcion",
        name: "estado_descripcion",
        autoWidth: true,
        render: function (data, type, row) {
            // Si el estado viene vacío o nulo
            if (!data) {
                return `<span style="background-color: #808080; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 120px;">Por establecer</span>`;
            }

            const estado = data.toLowerCase();
            let color = '';

            switch (estado) {
                case 'sin asignar':
                    color = '#c53a1d'; // rojo
                    break;
                case 'asignado':
                    color = '#236305'; // azul
                    break;
                case 'avance verificación':
                    color = '#799137'; // verde
                    break;
                case 'investigación':
                    color = '#2127f5'; // amarillo
                    break;
                case 'avance investigación':
                    color = '#40a8c7'; // naranja
                    break;
                case 'finalizado':
                    color = '#032b57'; // verde
                    break;
                default:
                    color = '#386ca0'; // gris oscuro
            }

            return `<span style="background-color: ${color}; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 120px;">${data}</span>`;
        }
    };
}

function EstadosExistencia() {
    return {
        title: "Estado Existencia",
        data: "estado_existencia_descripcion",
        name: "estado_existencia_descripcion",
        autoWidth: true,
        render: function (data, type, row) {
            // Si el estado viene vacío o nulo
            if (!data) {
                return `<span style="background-color: #808080; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 120px;">Por establecer</span>`;
            }

            const estado = data.toLowerCase();
            let color = '';

            switch (estado) {
                case 'no existe':
                    color = '#c53a1d'; // rojo
                    break;

                case 'si existe':
                    color = '#236305'; // verde
                    break;
                default:
                    color = '#386ca0'; // gris oscuro
            }

            return `<span style="background-color: ${color}; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 120px;">${data}</span>`;
        }
    };
}

function columnaCaracteristicasGenerales() {
    return {
        title: "Características Generales",
        data: "caracteristicas_generales",
        name: "caracteristicas_generales",
        "autoWidth": true,
        render: function (data, type, row) {
            if (!data || data.trim() === "") {
                return '';
            }

            const dataEncoded = encodeURIComponent(data);

            return `
                <div style="display: flex; align-items: center; max-width: 100px; gap: 10px;">
                    <button class="btn btn-success btn-sm" type="button"
                        onclick="AbrirModalVisualizarTexto(decodeURIComponent('${dataEncoded}'))">
                        <span class="fa fa-eye white"></span>
                    </button>
                    <div style="white-space: nowrap; overflow: hidden; text-overflow: ellipsis; flex-grow: 1;" title="${data}">
                        ${data}
                    </div>
                </div>
            `;
        }
    };
}

function columnaDescripcionTramite() {
    return {
        title: "Descripcion del Tramite",
        data: "descripcion_tramite",
        name: "descripcion_tramite",
        "autoWidth": true,
        render: function (data, type, row) {
            if (!data || data.trim() === "") {
                return '';
            }

            const dataEncoded = encodeURIComponent(data);

            return `
                <div style="display: flex; align-items: center; max-width: 100px; gap: 10px;">
                    <button class="btn btn-success btn-sm" type="button"
                        onclick="AbrirModalVisualizarTexto(decodeURIComponent('${dataEncoded}'))">
                        <span class="fa fa-eye white"></span>
                    </button>
                    <div style="white-space: nowrap; overflow: hidden; text-overflow: ellipsis; flex-grow: 1;" title="${data}">
                        ${data}
                    </div>
                </div>
            `;
        }
    };
}

function AbrirModalVisualizarTexto(Texto) {

    // Mostrar la modal
    $('#Modal_VisualizarTexto').modal("show");
    $('#txtDescripcion').val(Texto);
}


// ------------------------
// EXPORTAR REPORTE GENERAL (Excel)
// ------------------------
$("#btnExcel").on("click", function () {
    let anio = $("#ddlAnioIris").val();

    window.location.href =
        "/Reportes/ReporteGeneral/ExportarExcelReporteGeneral?anio=" +
        encodeURIComponent(anio);
});

// ------------------------
// EXPORTAR REPORTE GENERAL (PDF)
// ------------------------
$("#btnPdf").on("click", function () {
    let anio = $("#ddlAnioIris").val();

    window.open(
        "/Reportes/ReporteGeneral/ExportarPdfReporteGeneral?anio=" +
        encodeURIComponent(anio),
        "_blank"
    );
});
