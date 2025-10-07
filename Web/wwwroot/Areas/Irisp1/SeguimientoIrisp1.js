let archivoSubido = null; // Guardará el archivo ya cargado

$(document).ready(function () {


    if ($.fn.select2) {
        $('#ddlAnioIris').select2();
    }

    // Asocia el evento change
    $('#ddlAnioIris').on('change', function () {
        F_GetInfoGrillas();
    });

    $('#chkRegFoto').change(function () {
        if ($(this).is(':checked')) {
            $('#fotografia').closest('.col-md-4').removeClass('hidden');
        } else {
            $('#fotografia').closest('.col-md-4').addClass('hidden');
        }
    });


    $('.select2').select2({
        placeholder: "Seleccione",
        allowClear: true
    });



});

// Manejo genérico para cualquier modal secundaria
$(document).on('hidden.bs.modal', '.modal', function () {
    // Verifica si todavía hay alguna modal abierta
    if ($('.modal.show').length > 0) {
        $('body').addClass('modal-open');
    }
});


function AbrirModalVisualizarTexto(Texto) {

    // Mostrar la modal
    $('#Modal_VisualizarTexto').modal("show");
    $('#txtDescripcion').val(Texto);
}

//función para dar formato de fecha a las columnas que de las grillas
function formatDate(dateStr) {
    if (!dateStr) return ""; // si viene null o vacío
    const fecha = new Date(dateStr);
    if (isNaN(fecha)) return dateStr; // si no es fecha válida, devuelvo lo que llegó
    return fecha.toLocaleString("es-CO", {
        year: "numeric",
        month: "2-digit",
        day: "2-digit",
        hour: "2-digit",
        minute: "2-digit",
        second: "2-digit",
        hour12: true // 👈 esto activa AM/PM
    });
}

function F_GetInfoGrillas() {
    $.ajax({
        type: 'GET',
        url: UrlGetInfoGrillas,
        dataType: 'json',
        data: { V_Anio: $('#ddlAnioIris').val() },
        success: function (response) {
            console.log("✅ Respuesta exitosa:", response);
            let data = response.data || [];
            GetGrillaVerificacion(data);
            GetGrillaInvestigacion(data);
            GetGrillaFinalizacion(data);
        },
        error: function (xhr, status, error) {
            console.error("❌ Error Ajax:", status, error);
            console.error("Respuesta cruda:", xhr.responseText);
            GetGrillaVerificacion([]);
            GetGrillaInvestigacion([]);
            GetGrillaFinalizacion([]);
        }
    });
}

// 🔧 función utilitaria para crear o actualizar tablas
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
        pageLength: 25,
        ordering: true,
        searching: true,
        paging: true,
        info: true
    });
}

function GetGrillaVerificacion(Datos) {
    const datosFiltrados = Datos.filter(item => [2, 3, 4].includes(item.IdEstado));
    $("#pn_GrillaVerificacion").removeClass('hidden');

    renderDataTable("#tbGrilla", datosFiltrados, [
        columnaAcciones(datosFiltrados),
        Estados(),
        EstadosExistencia(),
        { title: "Codigo", data: "Codigo" },
        { title: "Dependencia", data: "Dependencia" },
        { title: "Municipio", data: "Municipio" },
        { title: "Fecha Inicio Actividad", data: "FechaInicioExistencia", render: formatDate },
        { title: "Clase", data: "Clase" },
        { title: "Nombre", data: "NombreClase" },
        { title: "Cantidad", data: "CantidadIntegrantes" },
        columnaCaracteristicasGenerales(),
        columnaDescripcionTramite(),
        { title: "Zona", data: "Zona" },
        { title: "Tipo Servicio", data: "TipoServicio" },
        { title: "Fuente", data: "Fuente" },
        { title: "Fecha de Creacion", data: "FechaCreacion" },
        { title: "Unidad Verificación Existencia", data: "UnidadResponsable" },
        { title: "Fecha Asignación Verificación", data: "FechaVerificacionExistencia", render: formatDate },
        { title: "Fecha Respuesta Verificación", data: "FechaRespuestaVerificacion", render: formatDate },
        //{ title: "Contador Verificación", data: "ContadorVerificacionExistencia" },
        Contador1(),
        { title: "Unidad Proceso Investigativo", data: "UnidadProcesoInvestigativo" },
        { title: "Fecha Asignación Investigativo", data: "FechaProcesoInvestigativo", render: formatDate },
        { title: "Fecha Respuesta Investigativo", data: "FechaRespuestaInvestigativo", render: formatDate },
        Contador2(),
        Resultados(),
        { title: "CriminalidadId", data: "CriminalidadId", visible: false }
    ]);
}

function GetGrillaInvestigacion(Datos) {
    const datosFiltrados = Datos.filter(item => [72, 73].includes(item.IdEstado));
    $("#pn_GrillaInvestigacion").removeClass('hidden');

    renderDataTable("#tbGrillaInvestigacion", datosFiltrados, [
        columnaAcciones(datosFiltrados),
        Estados(),
        EstadosExistencia(),
        { title: "Codigo", data: "Codigo" },
        { title: "Dependencia", data: "Dependencia" },
        { title: "Municipio", data: "Municipio" },
        { title: "Fecha Inicio Actividad", data: "FechaInicioExistencia", render: formatDate },
        { title: "Clase", data: "Clase" },
        { title: "Nombre", data: "NombreClase" },
        { title: "Cantidad", data: "CantidadIntegrantes" },
        columnaCaracteristicasGenerales(),
        columnaDescripcionTramite(),
        { title: "Zona", data: "Zona" },
        { title: "Tipo Servicio", data: "TipoServicio" },
        { title: "Fuente", data: "Fuente" },
        { title: "Fecha de Creacion", data: "FechaCreacion" },
        { title: "Unidad Verificación Existencia", data: "UnidadResponsable" },
        { title: "Fecha Asignación Verificación", data: "FechaVerificacionExistencia", render: formatDate },
        { title: "Fecha Respuesta Verificación", data: "FechaRespuestaVerificacion", render: formatDate },
        Contador1(),
        { title: "Unidad Proceso Investigativo", data: "UnidadProcesoInvestigativo" },
        { title: "Fecha Asignación Investigativo", data: "FechaProcesoInvestigativo", render: formatDate },
        { title: "Fecha Respuesta Investigativo", data: "FechaRespuestaInvestigativo", render: formatDate },
        Contador2(),
        Resultados(),
        { title: "CriminalidadId", data: "CriminalidadId", visible: false }
    ]);
}

function GetGrillaFinalizacion(Datos) {
    const datosFiltrados = Datos.filter(item => [5].includes(item.IdEstado));
    $("#pn_GrillaFinalizacion").removeClass('hidden');

    renderDataTable("#tbGrillaFinalizacion", datosFiltrados, [
        columnaAcciones(datosFiltrados),
        Estados(),
        EstadosExistencia(),
        { title: "Codigo", data: "Codigo" },
        { title: "Dependencia", data: "Dependencia" },
        { title: "Municipio", data: "Municipio" },
        { title: "Fecha Inicio Actividad", data: "FechaInicioExistencia", render: formatDate },
        { title: "Clase", data: "Clase" },
        { title: "Nombre", data: "NombreClase" },
        { title: "Cantidad", data: "CantidadIntegrantes" },
        columnaCaracteristicasGenerales(),
        columnaDescripcionTramite(),
        { title: "Zona", data: "Zona" },
        { title: "Tipo Servicio", data: "TipoServicio" },
        { title: "Fuente", data: "Fuente" },
        { title: "Fecha de Creacion", data: "FechaCreacion" },
        { title: "Unidad Verificación Existencia", data: "UnidadResponsable" },
        { title: "Fecha Asignación Verificación", data: "FechaVerificacionExistencia", render: formatDate },
        { title: "Fecha Respuesta Verificación", data: "FechaRespuestaVerificacion", render: formatDate },
        Contador1(),
        { title: "Unidad Proceso Investigativo", data: "UnidadProcesoInvestigativo" },
        { title: "Fecha Asignación Investigativo", data: "FechaProcesoInvestigativo", render: formatDate },
        { title: "Fecha Respuesta Investigativo", data: "FechaRespuestaInvestigativo", render: formatDate },
        //  { title: "Contador Investigativo", data: "ContadorProcesoInvestigativo" },
        Contador2(),
        //  { title: "Resultados", data: "Resultados", className: "celdaJust" },
        Resultados(),
        { title: "CriminalidadId", data: "CriminalidadId", visible: false }
    ]);
}

function Estados() {
    return {
        title: "Estado",
        data: "EstadoDescripcion",
        name: "EstadoDescripcion",
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
        data: "EstadoExistenciaDescripcion",
        name: "EstadoExistenciaDescripcion",
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

function columnaAcciones(datosFiltrados) {
    return {
        data: datosFiltrados,
        "autoWidth": true,
        render: function (data, type, row) {

            // Guardamos el objeto en un atributo data de forma segura
            // Reemplazamos comillas dobles por &quot; para no romper el HTML
            var DatosFila = JSON.stringify(row).replace(/"/g, '&quot;');

            var inicioBoton = '<div class="dropdown dropend">' +
                '<button class="btn btn-success" type="button" id="dropdownMenuButton1" ' +
                'data-bs-toggle="dropdown" aria-expanded="false">' +
                '<span class="fas fa-list"></span></button>' +
                '<ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';

            var Asignar = `<li style="padding-left: 15px;">
                                    <a style="color: #102717;" href="#"
                                       class="btn-detalle-iris"
                                       data-datos="${DatosFila}">
                                        <i class="fas fa-list"></i>&nbsp; Detalles
                                    </a>
                                </li>`;

            var Finalizar = `<li style="padding-left: 15px;">
                                    <a style="color: #102717;" href="javascript:ActualizarIrisp1('${row.CriminalidadId}')">
                                        <i class="fa fa-retweet green"></i>&nbsp;Actualizar Iris
                                    </a>
                                  </li>`;
         

            var finBoton = '</ul></div>';
            return inicioBoton + Asignar + Finalizar + finBoton;
        }
    }
}


// Delegación de eventos para los botones de detalle
$(document).on("click", ".btn-detalle-iris", function (e) {
    e.preventDefault();

    // Recuperamos el JSON guardado en data-datos
    var datosAttr = $(this).attr("data-datos").replace(/&quot;/g, '"');

    try {
        var registro = JSON.parse(datosAttr);
        F_GetDetalleIris(registro);
    } catch (err) {
        console.error("❌ Error parseando data-datos:", err, datosAttr);
        Swal.fire('Error', 'No se pudo procesar el detalle del registro', 'error');
    }
});


function columnaCaracteristicasGenerales() {
    return {
        title: "Características Generales",
        data: "CaracteristicasGenerales",
        name: "CaracteristicasGenerales",
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
        data: "DescripcionTramite",
        name: "DescripcionTramite",
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


function columnaInforAdicionalDetalleIris() {
    return {
        title: "Descripción",
        data: "Descripcion",
        name: "Descripcion",
        className: "celdaCenter celda50",
        render: function (data, type, row) {
            if (!data || data.trim() === "") {
                return '';
            }

            const dataEncoded = encodeURIComponent(data);

            return `
                <div style="
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    gap: 10px;
                    max-width: 900px;
                    margin: 0 auto;
                    text-align: center;
                ">
                    <button class="btn btn-success btn-sm" type="button"
                        onclick="AbrirModalVisualizarTexto(decodeURIComponent('${dataEncoded}'))"
                        style="flex-shrink: 0;">
                        <span class="fa fa-eye white"></span>
                    </button>
                    <div style="
                        white-space: nowrap;
                        overflow: hidden;
                        text-overflow: ellipsis;
                        max-width: 700px;
                    " title="${data}">
                        ${data}
                    </div>
                </div>
            `;
        }
    };
}

function Contador1() {
    return {
        title: "Contador Verificación",
        data: "ContadorVerificacionExistencia",
        name: "ContadorVerificacionExistencia",

        autoWidth: true,
        render: function (data, type, row) {
            // Si el estado viene vacío o nulo
            if (!data) {
                return `<span style="background-color: #808080; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px;">Por establecer</span>`;
            }

            const estado = data.toLowerCase();
            let color = '';

            if (estado.includes('no asignado')) {
                color = '#c53a1d'; //  Rojo
            } else {
                color = '#236305'; // verde
            }

            return `<span style="background-color: ${color}; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px;">${data}</span>`;
        }
    };
}

function Contador2() {
    return {
        title: "Contador Investigativo",
        data: "ContadorProcesoInvestigativo",
        name: "ContadorProcesoInvestigativo",

        autoWidth: true,
        render: function (data, type, row) {
            // Si el estado viene vacío o nulo
            if (!data) {
                return `<span style="background-color: #808080; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px;">Por establecer</span>`;
            }

            const estado = data.toLowerCase();
            let color = '';

            if (estado.includes('no asignado')) {
                color = '#c53a1d'; //  Rojo
            } else {
                color = '#236305'; // verde
            }

            return `<span style="background-color: ${color}; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px;">${data}</span>`;
        }
    };
}
function Resultados() {
    return {
        title: "Resultados",
        data: "Resultados",
        name: "Resultados",
        className: "celdaJust",
        autoWidth: true,
        render: function (data, type, row) {
            // Si el estado viene vacío o nulo
            if (!data) {
                return `<span style="background-color: #808080; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px;">Por establecer</span>`;
            }

            const estado = data.toLowerCase();
            let color = '';

            if (estado.includes('tiene resultados (')) {
                color = '#236305'; // verde
            } else {
                color = '#c53a1d'; // gris oscuro por defecto
            }

            return `<span style="background-color: ${color}; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px;">${data}</span>`;
        }
    };
}