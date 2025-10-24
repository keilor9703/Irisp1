
$(document).ready(function () {



    $('.select2').select2({
        placeholder: "Seleccione",
        allowClear: true
    });



    $('#ddlAnioIris').on('change', function () {
        F_GetInfoGrillas();
    });

    //F_GetInfoGrillas($('#ddlAnioIris').val());


});

function AbrirModalNuevoIris() {

    const modalElement = document.getElementById('Modal_VerRegistro');

    const modalInstance = new bootstrap.Modal(modalElement, {

        backdrop: 'static',

        keyboard: false,

        focus: false  // Desactiva focus automático de Bootstrap

    });

    modalInstance.show();
    consultarConsecutivoIris();
}



function F_GetInfoGrillas() {
    $.ajax({
        type: 'GET',
        url: AppRoutes.RegistroExpendio.UrlGetInfoGrillas,
        dataType: 'json',
        data: { V_Anio: $('#ddlAnioIris').val() },
        success: function (response) {
            console.log("✅ Respuesta exitosa:", response);
            let data = response.data || [];
            GetGrillaVerificacion(data);
           
        },
        error: function (xhr, status, error) {
            console.error("❌ Error Ajax:", status, error);
            console.error("Respuesta cruda:", xhr.responseText);
            GetGrillaVerificacion([]);
           
        }
    });
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

function renderDataTable(selector, datosFiltrados, columnas) {
    if ($.fn.dataTable.isDataTable(selector)) {
        const table = $(selector).DataTable();
        table.clear();
        table.rows.add(datosFiltrados);
        table.draw(false);
        return;
    }

    $(selector).DataTable({
        data: datosFiltrados,
        language: glOpcionesIdioma,
        scrollX: true,
        scrollY: 400,      // altura fija con scroll
        scroller: true,    // virtualización (solo renderiza lo visible)
        deferRender: true, // retrasar render hasta que se vea
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
        ordering: false,
        searching: true,
        paging: true,
        info: true
    });
}
function GetGrillaVerificacion(datosFiltrados) {
    //const datosFiltrados = Datos.filter(item => [2, 3, 4].includes(item.IdEstado));
    $("#pn_GrillaExpendios").removeClass('hidden');

    renderDataTable("#tbGrillaExpendios", datosFiltrados, [
        columnaAcciones(datosFiltrados),
        Estados(),
        //EstadosExistencia(),
        { title: "Codigo", data: "Codigo" },
        { title: "Sigla", data: "SiglaUnidadInforma" },
        { title: "Unidad Informa", data: "UnidadInformaDescripcion" },
        { title: "Region", data: "RegionDescripcion" },
        { title: "Sigla Unidad Hecho", data: "Sigla" },
        { title: "Unidad Hecho", data: "Unidad" },
        { title: "Zona", data: "Zona" },
        { title: "Clase", data: "Clase" },
        { title: "Tipo Expendio", data: "Expendio" },
        { title: "Fuente", data: "Fuente" },
        { title: "Fecha Inicio Existencia", data: "FechaInicioExistencia", render: formatDate },
        columnaCaracteristicasGenerales(),
        { title: "Categoría", data: "Categoria" },
        { title: "Otra Categoría", data: "OtraCategoria" },
        { title: "Código Operación", data: "CodigoMored" },
        { title: "Nombre Operación", data: "NombreMored" },
        { title: "NUNC", data: "Nunc" },
        { title: "SIEDCO", data: "Siedco" },
        { title: "Erradicado ?", data: "Erradicado" },
        { title: "Barrio", data: "Barrio" },
        { title: "Direccion", data: "Direccion" },
        { title: "Latitud", data: "Latitud" },
        { title: "Longitud", data: "Longitud" },
        { title: "Cuadrante", data: "Cuadrante" },
        { title: "Municipio", data: "Municipio" },
        { title: "Fecha Creacion", data: "FechaCreacion", render: formatDate },
        { title: "CriminalidadDirecId", data: "CriminalidadDirecId", visible: false }
    ]);
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

            var Detalle = `<li style="padding-left: 15px;">
                                    <a style="color: #102717;" href="#"
                                       class="btn-detalle-expendio"
                                       data-datos="${DatosFila}">
                                        <i class="fas fa-list"></i>&nbsp; Detalle
                                    </a>
                                </li>`;

            var ActualizarEstado = `<li style="padding-left: 15px;">
                                    <a style="color: #102717;" href="javascript:Finalizar('${row.CriminalidadId}')">
                                        <i class="fa fa-retweet green"></i>&nbsp;Actualizar Estado
                                    </a>
                                  </li>`;


            var finBoton = '</ul></div>';
            return inicioBoton + Detalle + ActualizarEstado + finBoton;
        }
    }
}


// Delegación de eventos para los botones de detalle
$(document).on("click", ".btn-detalle-expendio", function (e) {
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
                case 'descartado':
                    color = '#c53a1d'; // rojo
                    break;
                case 'investigación':
                    color = '#236305'; // azul
                    break;
                case 'finalizado':
                    color = '#799137'; // verde
                    break;
                case 'verificación':
                    color = '#2127f5'; // amarillo
                    break;
                //case 'avance investigación':
                //    color = '#40a8c7'; // naranja
                //    break;
                //case 'finalizado':
                //    color = '#032b57'; // verde
                //    break;
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


function AbrirModalVisualizarTexto(Texto) {

    // Mostrar la modal
    $('#Modal_VisualizarTexto').modal("show");
    $('#txtDescripcion').val(Texto);
}
