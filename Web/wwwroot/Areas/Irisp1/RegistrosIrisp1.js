let archivoSubido = null; // Guardará el archivo ya cargado

$(document).ready(function () {
  

    if ($.fn.select2) {
        $('#ddlAnioIris').select2();
    }




    // Asocia el evento change
    $('#ddlAnioIris').on('change', function () {
        F_GetInfoGrillas();
    });    


   // F_GetInfoGrillas($('#ddlAnioIris').val());

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

$('#btnCerrarVerRegistro').on('click', function () {
    // 1. Destruir DataTable si existe
    if ($.fn.DataTable.isDataTable('#tbGrillaIntegantes')) {
        $('#tbGrillaIntegantes').DataTable().clear().destroy();
    }

    // 2. Limpiar HTML de la tabla
    $('#tbGrillaIntegantes').empty();

    // 3. Ocultar el panel
    $('#pn_GrillaIntegantes')
        .removeClass('show')
        .addClass('hidden');

    // 4. Limpiar inputs
    $('#Modal_VerRegistro').find('input[type=text], input[type=number], textarea').val('');

    // 5. Resetear selects con Select2
    $('#Modal_VerRegistro').find('select.select2').val(null).trigger('change');

    // 6. Resetear clases de error
    $('#Modal_VerRegistro').find('.form-group').removeClass('has-error');
});


function SubirFoto(input) {

    if (!input.files || input.files.length === 0) {
        Swal.fire('Advertencia', 'Debe seleccionar una imagen.', 'warning');
        return;
    }

    var file = input.files[0];
    var maxSizeMB = 5;
    var allowedTypes = ['image/jpeg', 'image/png'];

    if (!allowedTypes.includes(file.type)) {
        Swal.fire('Error', 'Formato inválido. Solo JPG o PNG.', 'error');
        input.value = '';
        return;
    }

    if (file.size > maxSizeMB * 1024 * 1024) {
        Swal.fire('Error', 'La imagen supera el límite de 5MB.', 'error');
        input.value = '';
        return;
    }

    var CriminalidadId = $("#txtConsecutivoIris").val();

    if (!CriminalidadId) {
        Swal.fire('Error', 'No se encontró ID de Criminalidad.', 'error');
        input.value = '';
        return;
    }

    var formData = new FormData();
    formData.append("foto", file);
    formData.append("idCriminalidad", CriminalidadId);

    $.ajax({
        url: 'Irisp1/RegistrosIrisp1/GuardarFotoConRegistro',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (resp) {

            if (resp.exito) {
                Swal.fire({
                    icon: "success",
                    title: "Fotografía cargada",
                    text: "La imagen se guardó correctamente.",
                    timer: 2000
                });

                input.value = "";

                // 🔥 Recargas tu grilla existente
                F_GetFotosIris(CriminalidadId);

            } else {
                Swal.fire('Error', resp.mensaje || 'No se pudo subir la imagen.', 'error');
            }
        },
        error: function () {
            Swal.fire('Error', 'Ocurrió un error al subir la fotografía.', 'error');
        }
    });
}

function AbrirModalMas() {

    $('#ModalCaracteristicasGenerales').modal("show");

}
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

function ActualizarEstadoIris(CriminalidadId) {
    $('#Modal_UpdEstadoRegistroIris').modal("show");
    $('#txtCriminalidadIdUpd').text(CriminalidadId);
}

function ActualizarExistenciaIris(CriminalidadId) {
    $('#Modal_UpdExistenciaRegistroIris').modal("show");
    $('#txtCriminalidadIdUpd').text(CriminalidadId);
}

function ActualizarIrisp1(CriminalidadId) {
    $('#Modal_UpdRegistroIris').modal("show");
    $('#txtCriminalidadIdUpd').text(CriminalidadId);
 
}

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
   // console.log("✅ año:", $('#ddlAnioIris').val());
    $.ajax({
        type: 'GET',
        url: AppRoutes.RegistroIrisP1.UrlGetInfoGrillas,
        dataType: 'json',
        data: { V_Anio: $('#ddlAnioIris').val() },
        success: function (response) {
            //console.log("✅ Respuesta exitosa:", response);
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
        pageLength: 10,
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
        { title: "Fecha de Creacion", data: "FechaCreacion" ,render: formatDate },
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
        { title: "Fecha de Creacion", data: "FechaCreacion", render: formatDate },
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
        { title: "Fecha de Creacion", data: "FechaCreacion", render: formatDate },
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

            var DetallesIris = `<li style="padding-left: 15px;">
                                    <a style="color: #102717;" href="#"
                                       class="btn-detalle-iris"
                                       data-datos="${DatosFila}">
                                        <i class="fas fa-list"></i>&nbsp; Detalles
                                    </a>
                                </li>`;

            var ActualizarIris = `<li style="padding-left: 15px;">
                                    <a style="color: #102717;" href="javascript:ActualizarIrisp1('${row.CriminalidadId}')">
                                        <i class="fa fa-retweet green"></i>&nbsp;Actualizar Iris
                                    </a>
                                  </li>`;
            var ActualizarEstado = `<li style="padding-left: 15px;">
                                        <a style="color: #102717;" href="javascript:ActualizarEstadoIris('${row.CriminalidadId}')">
                                            <i class="fa fa-retweet green"></i>&nbsp;Actualizar Estado
                                        </a>
                                    </li>`;
            var ActualizarExistencia = `<li style="padding-left: 15px;">
                                            <a style="color: #102717;" href="javascript:ActualizarExistenciaIris('${row.CriminalidadId}')">
                                                <i class="fa fa-retweet green"></i>&nbsp;Actualizar Existencia
                                            </a>
                                        </li>`;
            var Eliminar = `<li style="padding-left: 15px;">
                                <a style="color: #102717;" href="javascript:DellIris('${row.CriminalidadId}')">
                                    <i class="fa fa-trash red"></i>&nbsp;Eliminar
                                </a>
                            </li>`;

            var finBoton = '</ul></div>';
            return inicioBoton + DetallesIris + ActualizarIris + ActualizarEstado + ActualizarExistencia + Eliminar + finBoton;
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




// Insertar integrantes en la modal para crear un nuevo Iris
function InsIntegrantes() {
    const criminalidadId = $("#txtConsecutivoIris").val().trim();
    const alias = $("#txtAlias").val().trim();
    const nombres = $("#txtNombreInteg").val().trim();
    const apellidos = $("#txtApellidosInteg").val().trim();
    const identificacion = $("#txtIdentificacionInteg").val().trim();
    const celular = $("#txtCelularInteg").val().trim();
    const direccion = $("#txtDireccionInteg").val();

    // Validaciones
    if (!criminalidadId || (!nombres && !alias)) {
        Swal.fire({
            icon: 'warning',
            title: 'Campos obligatorios',
            text: 'Debe diligenciar al menos Nombre o Alias.'
        });
        return;
    }

   

    const Obj_Integrante = {
        CRIMINALIDAD_ID: criminalidadId,
        ALIAS: alias,
        NOMBRE: nombres,
        APELLIDO: apellidos,
        CEDULA: parseInt(identificacion),
        ID_TIPO_INFO: 30,
        TIPO_DOCUMENTO: 1,
        CELULAR: celular ? parseInt(celular) : null,
        DIRECCION: direccion
    };

    $.ajax({
        url: AppRoutes.RegistroIrisP1.UrlInsIntegrantesPreliminar,
        type: 'POST',
        data: Obj_Integrante,  // ← Envío normal
        success: function (resp) {
            if (resp.success) {
                $('#pn_GrillaIntegantes').removeClass('hidden').addClass('show');
                F_GetIntegrantesPreliminar();
                limpiarFormularioIntegrantes();
            } else {
                Swal.fire('Error', 'Error al insertar: ' + resp.message, 'error');
            }
        },
        error: function () {
            Swal.fire('Error', 'Fallo en la llamada AJAX.', 'error');
        }
    });

}

function F_GetIntegrantesPreliminar() {

    var V_CriminalidadId = $("#txtConsecutivoIris").val()
    $.ajax({
        type: 'GET',
        url: AppRoutes.RegistroIrisP1.UrlGetIntegrantesPreliminar, 
        async: true,
        data: { V_CriminalidadId: V_CriminalidadId },
        dataType: 'json',
        success: function (response) {
            if (response.success) {
               // $("#pn_GrillaIntegantes").removeClass('hidden');

               
                Grillantegrantes(response.data);
            } else {
                Grillantegrantes([]);
                Swal.fire('Error', response.message, 'error');
            }
        },
        error: function () {
            Grillantegrantes([]);
            Swal.fire('Error', 'No se pudo obtener la lista de integrantes.', 'error');
        }
    });
}

function Grillantegrantes(Datos) {
    if ($.fn.dataTable.isDataTable("#tbGrillaIntegantes")) {
        $("#tbGrillaIntegantes").DataTable().destroy();
    }

    $("#tbGrillaIntegantes").DataTable({
        destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        responsive: true,
        columns: [
            { title: "Alias", data: "ALIAS", className: "celdaCenter" },
            { title: "Nombre", data: "NOMBRE", className: "celdaCenter" },
            { title: "Apellido", data: "APELLIDO", className: "celdaCenter" },
            { title: "Cédula", data: "CEDULA", className: "celdaCenter" },
            { title: "Dirección", data: "DIRECCION", className: "celdaCenter" },
            {
                title: "Fecha Creación", data: "FECHA_CREACION", className: "celdaJust",
               

                  render: function (data) {
                    if (!data) return "";
                    const fecha = moment(data).format('DD/MM/YYYY');
                    const hora = moment(data).format('hh:mm:ss a');
                    return `${fecha} - ${hora}`;

                }
            }
        ],
        lengthMenu: [
            [5, 10, 25, 50, -1],
            ['5 registros', '10 registros', '25 registros', '50 registros', 'Todos']
        ],
        ordering: false,
        pageLength: 10,
        bLengthChange: true,
        searching: true,
        paging: true,
        info: true
    });
}

function F_GetFuncionariosIris(V_Identificacion) {

    let Identificación = Number(V_Identificacion);
    if (Identificación < 1) {
        create('error', 'Debe digitar número de Identificación', '../../img/AlertError.png');
        return;
    }

    $.ajax({
        type: "GET",
        url: AppRoutes.RegistroIrisP1.UrlGetFuncionarios,
        async: true,
        data: { V_Identificacion: $("#txtIdentificacion").val() },
        dataType: 'json',
        cache: false,
        success: function (respuesta) {

            if (respuesta.success) {
             
                $("#txtFuncionario").val(respuesta.data.Funcionario);
              
                $("#txtDependencia2").val(respuesta.data.Estacion);
                $("#txtTelefono").val(respuesta.data.Celular);
                $("#txtDependencia").val(respuesta.data.Dependencia).trigger('change');
               

                $("#txtUnidad").val(respuesta.data.Fisica + " - " + respuesta.data.Dependencia);
             
                $("#txtUndeLabora").val(respuesta.data.IdUndeLaborando);
                $("#txtSiglaUnidad").val(respuesta.data.Fisica);

                
                
                
            } else {
               
                Swal.fire({
                    type: 'error',
                    title: 'Señor(a) Funcionario(a:)',
                    text: "No se Encontro el Funcionario"
                });
            }
        },
        error: function () {
            Swal.fire({
                type: 'error',
                title: 'Señor(a) Funcionario(a:)',
                text: 'No es posible consultar, revise!!'
            });
        }
    });
}

/*Datos Iris P1, campos dinamicos según la clase*/
$('#ddlClase').on('change', function () {
    var claseSeleccionada = $("#ddlClase option:selected").text().trim();

    // Ocultar todos los campos primero
    $('#txtNombreTrafico').closest('.col-md-3').addClass('hidden');
    $('#ddlModExpendio').closest('.col-md-4').addClass('hidden');

    $('#txtNombreEstructura').closest('.col-md-3').addClass('hidden');
    $('#txtCantidadEstructura').closest('.col-md-3').addClass('hidden');

    $('#txtNombreInstalacion').closest('.col-md-3').addClass('hidden');
    $('#txtCantidadInstalacion').closest('.col-md-3').addClass('hidden');

    $('#txtCantidadPersonas').closest('.col-md-3').addClass('hidden');

    $('#txtNombreNarcotrafico').closest('.col-md-3').addClass('hidden'); // Reutilizado en narcotráfico
    $('#ddlClasiNarcotrafico').closest('.col-md-4').addClass('hidden');

    // Mostrar según la clase seleccionada
    switch (claseSeleccionada) {
        case "Trafico De Estupefacientes":
            $('#txtNombreTrafico').closest('.col-md-3').removeClass('hidden');
            $('#ddlModExpendio').closest('.col-md-4').removeClass('hidden');
            break;

        case "Estructura":
            $('#txtNombreEstructura').closest('.col-md-3').removeClass('hidden');
            $('#txtCantidadEstructura').closest('.col-md-3').removeClass('hidden');
            break;

        case "Instalación Fija":
            $('#txtNombreInstalacion').closest('.col-md-3').removeClass('hidden'); // primer campo
            $('#txtCantidadInstalacion').closest('.col-md-3').removeClass('hidden');
            break;

        case "Persona":
            $('#txtCantidadPersonas').closest('.col-md-3').removeClass('hidden');
            break;

        case "Narcotrafico (grandes cantidades)":
            $('#txtNombreNarcotrafico').closest('.col-md-3').removeClass('hidden'); // segundo campo reutilizado
            $('#ddlClasiNarcotrafico').closest('.col-md-4').removeClass('hidden');
            break;
    }
});


$('#ddlClaseModal').on('change', function () {
    var claseSeleccionada = $("#ddlClaseModal option:selected").text().trim();

    // Ocultar todos los campos primero
    $('#txtNombreTraficoModal').closest('.col-md-3').addClass('hidden');
    $('#ddlModExpendioModal').closest('.col-md-4').addClass('hidden');

    $('#txtNombreEstructuraModal').closest('.col-md-3').addClass('hidden');
    $('#txtCantidadEstructuraModal').closest('.col-md-3').addClass('hidden');

    $('#txtNombreInstalacionModal').closest('.col-md-3').addClass('hidden');
    $('#txtCantidadInstalacionModal').closest('.col-md-3').addClass('hidden');

    $('#txtCantidadPersonasModal').closest('.col-md-3').addClass('hidden');

    $('#txtNombreNarcotraficoModal').closest('.col-md-3').addClass('hidden'); // Reutilizado en narcotráfico
    $('#ddlClasiNarcotraficoModal').closest('.col-md-4').addClass('hidden');

    // Mostrar según la clase seleccionada
    switch (claseSeleccionada) {
        case "Trafico De Estupefacientes":
            $('#txtNombreTraficoModal').closest('.col-md-3').removeClass('hidden');
            $('#ddlModExpendioModal').closest('.col-md-4').removeClass('hidden');
            break;

        case "Estructura":
            $('#txtNombreEstructuraModal').closest('.col-md-3').removeClass('hidden');
            $('#txtCantidadEstructuraModal').closest('.col-md-3').removeClass('hidden');
            break;

        case "Instalación Fija":
            $('#txtNombreInstalacionModal').closest('.col-md-3').removeClass('hidden'); // primer campo
            $('#txtCantidadInstalacionModal').closest('.col-md-3').removeClass('hidden');
            break;

        case "Persona":
            $('#txtCantidadPersonasModal').closest('.col-md-3').removeClass('hidden');
            break;

        case "Narcotrafico (grandes cantidades)":
            $('#txtNombreNarcotraficoModal').closest('.col-md-3').removeClass('hidden'); // segundo campo reutilizado
            $('#ddlClasiNarcotraficoModal').closest('.col-md-4').removeClass('hidden');
            break;
    }
});

$('#txtDependencia').change(function () {
    const valor1 = $(this).val();
    const valor2 = $('#txtDependencia2').val();

    handleDropdownChange('Irisp1/RegistrosIrisp1/F_GetCuadrantes', {
        V_unidadLabora: valor1,
        V_unidadLabora2: valor2
    }, '#ddlCuadrante');
});

function handleDropdownChange(url, params, dropdownSelector, callback) {
    if (params && params.V_unidadLabora) {
        $.getJSON(url, params, function (data) {
            const dropdown = $(dropdownSelector);
            dropdown.empty().append('<option value="">Seleccione</option>');

            if (Array.isArray(data) && data.length > 0) {
                $.each(data, function (index, item) {
                    if (item && item.descripcion) {
                        dropdown.append(`<option value="${item.codigo}">${item.descripcion}</option>`);
                    }
                });

                // Volver a inicializar Select2 si es necesario
                dropdown.trigger('change');

                if (callback && typeof callback === "function") {
                    callback();
                }
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            console.error(`Error al cargar datos desde ${url}:`, textStatus, errorThrown);
        });
    }
}

function consultarConsecutivoIris() {
    $.ajax({
        url: AppRoutes.RegistroIrisP1.UrlGetConsecutivoIris
,
        type: 'POST',
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                $("#txtConsecutivoIris").val(response.data);
               
            } else {
                $("#txtConsecutivoIris").val('');
                // alert(response.message || "Error al obtener consecutivo.");
                Swal.fire({
                    type: 'info',
                    title: 'Señor(a) Funcionario(a:)',
                    text: "Error al obtener consecutivo."
                });
            }
        },
        error: function () {
            $("#txtConsecutivoIris").val('');
            //  alert("Error de comunicación con el servidor.");
            Swal.fire({
                type: 'error',
                title: 'Señor(a) Funcionario(a:)',
                text: 'Error de comunicación con el servidor.'
            });
        }
    });
}

//function P_InsRegistroIrisP1() {

//    var formData = new FormData();

//    var IdClase = $("#ddlClase").val();

//    let NombreClaseI = '';
//    let Id_modalidadI = 0;
//    let Cantidad = 0;
//    let Id_ClasiNarcotrafico = 0;

//    switch (IdClase) {
//        case '74':
//            NombreClaseI = $("#txtNombreTrafico").val();
//            Id_modalidadI = $("#ddlModExpendio").val();
//            break;

//        case '13':
//            NombreClaseI = $("#txtNombreEstructura").val();
//            Cantidad = $("#txtCantidadEstructura").val();
//            break;

//        case '14':
//            NombreClaseI = $("#txtNombreInstalacion").val();
//            Cantidad = $("#txtCantidadInstalacion").val();
//            break;

//        case '15':
//            Cantidad = $("#txtCantidadPersonas").val();
//            break;

//        case '153':
//            NombreClaseI = $("#txtNombreNarcotrafico").val();
//            Id_ClasiNarcotrafico = $("#ddlClasiNarcotrafico").val();
//            break;

//        case '154':
//            break;

//        default:
//            NombreClaseI = '';
//            Id_modalidadI = 0;
//            Cantidad = 0;
//            Id_ClasiNarcotrafico = 0;
//    }

//    var Obj_DelitosSecundarios = obtenerDelitosSecundariosSeleccionados();

//    const Obj_NuevoIrisP1 = {
//        CriminalidadId: $("#txtConsecutivoIris").val(),
//        IdUnidad: $("#txtUndeLabora").val(),
//        IdZona: $("#ddlZona").val(),
//        IdentificacionInforma: $("#txtIdentificacion").val(),
//        Celular: $("#txtTelefono").val(),
//        IdTipoServicio: $("#ddlTipoServicio").val(),
//        IdCuadrante: $("#ddlCuadrante").val(),
//        IdClase: IdClase,
//        NombreClase: NombreClaseI,
//        Id_modalidad: Id_modalidadI,
//        CantidadIntegrantes: Cantidad,
//        CaracteristicasGenerales: $("#txtCaractGenerales").val(),
//        Vigente: 1,
//        SiglaUnidad: $("#txtSiglaUnidad").val(),
//        IdEstado: 2,
//        IdFuente: $("#ddlFuente").val(),
//        EntornoAfectado: $("#ddlEntono").val(),
//        IdtiempoDelito: $("#ddlActividad").val(),
//        Clasificacion: Id_ClasiNarcotrafico,
//        Modalidadexpendio: Id_modalidadI,
//        Origen: 'WEB',
//        NombreEntornoAfectado: $("#txtNombreEntornoAfectado").val(),
//        EspecialidadAporta: $("#ddlEspecialidad").val(),
//        IdDelitoPrincipal: $("#ddlDelitoPrincipal").val(),
//        IdDelitoSecundario: Obj_DelitosSecundarios,
//        IdTipoInfo: 30,

//        Latitud: $("#LATITUD_CASO").val(),
//        Longitud: $("#LONGITUD_CASO").val(),
//        Barrio: $("#txtBarrio").val(),
//        Cuadrante: $("#txtCuadrante").val(),
//        Direccion: $("#txtDireccion").val(),
//        CuadranteRural: $("#txtCuadrante").val(),
//        CodigoDane: $("#txtCodDane").val(),
//        CodigoEstacion: $("#txtCodEstacion").val(),
//        CodigoSiedcoCuadrante: $("#txtCodSiedcoCuadrante").val(),
//        MunicipioUbica: $("#txtMunicipio").val(),
//        RadioAccion: $("#txtRadioAccion").val()

//    };

//    // --- Validación de campos obligatorios ---
//    for (let key in Obj_NuevoIrisP1) {
//        if (key !== 'IdDelitoSecundario' && key !== 'IdCuadrante' && key !== 'RadioAccion' &&// puede venir vacío
//            (Obj_NuevoIrisP1[key] === null || Obj_NuevoIrisP1[key] === '' || Obj_NuevoIrisP1[key] === undefined || (typeof Obj_NuevoIrisP1[key] === 'number' && isNaN(Obj_NuevoIrisP1[key]))))
//        {
//            /*Swal.fire('Advertencia', `El campo "${key}" es obligatorio y no puede estar vacío.`, 'warning');*/
           
//            Swal.fire({
//                type: 'warning',
//                title: 'Señor(a) Funcionario(a:)',
//                text: "Valide todos los campos para completar el presente registro"
//            });
//            return; // detener ejecución
//        }
//    }


//    formData.append("datosJson", JSON.stringify(Obj_NuevoIrisP1));


//    var file = $("#fileAnexoFotografico")[0].files[0];
//    if (file) {
//        formData.append("foto", file);
//    }


//    // --- Enviar solicitud ---
//    $.ajax({
//        url: AppRoutes.RegistroIrisP1.UrlInsRegistroIrisP1
//,
//        type: 'POST',
//        data: Obj_NuevoIrisP1,
//        success: function (resp) {
//            if (resp.success) {
//                Swal.fire({
//                    icon: 'success',
//                    title: 'Señor(a) Funcionario(a):',
//                    text: resp.message
//                })
                  
//                // Cerrar la modal
//                $('#Modal_VerRegistro').modal('hide');

//                // Obtener todas las opciones y convertir a número
//                var opciones = $('#ddlAnioIris option').map(function () {
//                    return parseInt($(this).val(), 10);
//                }).get();

//                // Filtrar solo los valores numéricos válidos
//                var opcionesValidas = opciones.filter(function (v) {
//                    return !isNaN(v);
//                });

//                // Calcular el máximo año
//                var maxAnio = Math.max.apply(null, opcionesValidas);

//                // Asignar el máximo año como valor seleccionado
//                $('#ddlAnioIris').val(maxAnio).trigger('change');

//                // Refrescar la grilla
//                F_GetInfoGrillas();
//                limpiarFormularioIrisP1();


                
//            } else {
//                Swal.fire('Error', 'Error al insertar: ' + resp.message, 'error');
//            }
//        },
//        error: function () {
//            Swal.fire('Error', 'Fallo en la llamada AJAX.', 'error');
//        }
//    });
//}




function P_InsRegistroIrisP1() {

    // ------------------------------
    // 1. VALIDACIONES
    // ------------------------------

    var IdClase = $("#ddlClase").val();
    let NombreClaseI = '';
    let Id_modalidadI = 0;
    let Cantidad = 0;
    let Id_ClasiNarcotrafico = 0;

    switch (IdClase) {
        case '74':
            NombreClaseI = $("#txtNombreTrafico").val();
            Id_modalidadI = $("#ddlModExpendio").val();
            break;
        case '13':
            NombreClaseI = $("#txtNombreEstructura").val();
            Cantidad = $("#txtCantidadEstructura").val();
            break;
        case '14':
            NombreClaseI = $("#txtNombreInstalacion").val();
            Cantidad = $("#txtCantidadInstalacion").val();
            break;
        case '15':
            Cantidad = $("#txtCantidadPersonas").val();
            break;
        case '153':
            NombreClaseI = $("#txtNombreNarcotrafico").val();
            Id_ClasiNarcotrafico = $("#ddlClasiNarcotrafico").val();
            break;
        case '154':
            break;
        default:
            NombreClaseI = '';
            Id_modalidadI = 0;
            Cantidad = 0;
            Id_ClasiNarcotrafico = 0;
    }

    var Obj_DelitosSecundarios = obtenerDelitosSecundariosSeleccionados()
        .filter(x => x !== null && x !== "" && !isNaN(x))
        .map(Number);


    const Obj_NuevoIrisP1 = {
        CriminalidadId: $("#txtConsecutivoIris").val(),
        IdUnidad: $("#txtUndeLabora").val(),
        IdZona: $("#ddlZona").val(),
        IdentificacionInforma: $("#txtIdentificacion").val(),
        Celular: $("#txtTelefono").val(),
        IdTipoServicio: $("#ddlTipoServicio").val(),
        IdCuadrante: $("#ddlCuadrante").val(),
        IdClase: IdClase,
        NombreClase: NombreClaseI,
        Id_modalidad: Id_modalidadI,
        CantidadIntegrantes: Cantidad,
        CaracteristicasGenerales: $("#txtCaractGenerales").val(),
        Vigente: 1,
        SiglaUnidad: $("#txtSiglaUnidad").val(),
        IdEstado: 2,
        IdFuente: $("#ddlFuente").val(),
        EntornoAfectado: $("#ddlEntono").val(),
        IdtiempoDelito: $("#ddlActividad").val(),
        Clasificacion: Id_ClasiNarcotrafico,
        Modalidadexpendio: Id_modalidadI,
        Origen: 'WEB',
        NombreEntornoAfectado: $("#txtNombreEntornoAfectado").val(),
        EspecialidadAporta: $("#ddlEspecialidad").val(),
        IdDelitoPrincipal: $("#ddlDelitoPrincipal").val(),
        IdDelitoSecundario: Obj_DelitosSecundarios,
        IdTipoInfo: 30,
        Latitud: $("#LATITUD_CASO").val(),
        Longitud: $("#LONGITUD_CASO").val(),
        Barrio: $("#txtBarrio").val(),
        Cuadrante: $("#txtCuadrante").val(),
        Direccion: $("#txtDireccion").val(),
        CuadranteRural: $("#txtCuadrante").val(),
        CodigoDane: $("#txtCodDane").val(),
        CodigoEstacion: $("#txtCodEstacion").val(),
        CodigoSiedcoCuadrante: $("#txtCodSiedcoCuadrante").val(),
        MunicipioUbica: $("#txtMunicipio").val(),
        RadioAccion: $("#txtRadioAccion").val()
    };

    // ------------------------------
    // 2. VALIDACIONES OBLIGATORIAS
    // ------------------------------



    for (let key in Obj_NuevoIrisP1) {
        if (key !== 'IdDelitoSecundario' && key !== 'IdCuadrante' && key !== 'RadioAccion' && key !== 'NombreClase' && key !== 'Id_modalidad'
            && key !== 'Clasificacion' && key !== 'Modalidadexpendio') {
            if (!Obj_NuevoIrisP1[key]) {
                console.warn(`El campo "${key}" está vacío o nulo.`); // 👈 mensaje en consola
                Swal.fire({
                    icon: 'warning', // se recomienda usar 'icon' en lugar de 'type'
                    title: 'Señor(a) Funcionario(a)',
                    text: `Valide el campo "${key}" para completar el registro.`
                });
                return;
            }
        }
    }

    // ------------------------------
    // 3. FOTO — VALIDACIONES ORIGINALES
    // ------------------------------

    var fileInput = $("#fileAnexoFotografico")[0];
    var file = fileInput.files[0];

    // Nota: si el checkbox de "Registro fotográfico" está marcado
    if ($("#chkRegFoto").prop("checked")) {

        if (!file) {
            Swal.fire('Advertencia', 'Debe seleccionar una imagen.', 'warning');
            return;
        }

        var maxSizeMB = 5;
        var allowedTypes = ['image/jpeg', 'image/png'];

        if (!allowedTypes.includes(file.type)) {
            Swal.fire('Error', 'Formato no permitido. Solo JPG o PNG.', 'error');
            return;
        }

        if (file.size > maxSizeMB * 1024 * 1024) {
            Swal.fire('Error', 'La imagen supera el tamaño máximo permitido de 5MB.', 'error');
            return;
        }
    }

    // ------------------------------
    // 4. ARMAR FORM-DATA (AQUÍ SE INTEGRA TODO)
    // ------------------------------

    var formData = new FormData();
    formData.append("datosJson", JSON.stringify(Obj_NuevoIrisP1));

    if (file) {
        formData.append("foto", file);
    }

    // ------------------------------
    // 5. ENVÍO A UN SOLO ENDPOINT
    // ------------------------------

 
    $.ajax({
        url: AppRoutes.RegistroIrisP1.UrlInsRegistroIrisP1,
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (resp) {
            if (resp.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Señor(a) Funcionario(a):',
                    text: resp.message
                })

                // Cerrar la modal
                $('#Modal_VerRegistro').modal('hide');

                // Obtener todas las opciones y convertir a número
                var opciones = $('#ddlAnioIris option').map(function () {
                    return parseInt($(this).val(), 10);
                }).get();

                // Filtrar solo los valores numéricos válidos
                var opcionesValidas = opciones.filter(function (v) {
                    return !isNaN(v);
                });

                // Calcular el máximo año
                var maxAnio = Math.max.apply(null, opcionesValidas);

                // Asignar el máximo año como valor seleccionado
                $('#ddlAnioIris').val(maxAnio).trigger('change');

                // Refrescar la grilla
                F_GetInfoGrillas();
                limpiarFormularioIrisP1();



            } else {
                Swal.fire('Error', 'Error al insertar: ' + resp.message, 'error');
            }
        },
        error: function () {
            Swal.fire('Error', 'Fallo en la llamada AJAX.', 'error');
        }
    });
}


function limpiarFormularioIrisP1() {


    /* Cerrar modal si está visible*/
    const modal = bootstrap.Modal.getInstance(document.getElementById('Modal_VerRegistro'));
    if (modal) {
        modal.hide();
    }

     /*Campos de texto*/
    $("#txtUndeLabora").val('');
    $("#txtCodDane").val('');
    $("#txtCodSiedcoEstacion").val('');
    $("#txtCodSiedcoCuadrante").val('');
    $("#txtCodEstacion").val('');
    $("#txtCodIdCuadrante").val('');
    $("#txtSiglaUnidad").val('');
    $("#txtConsecutivoIris").val('');
    $("#txtConsecutivoIntegrante").val('');
    $("#txtIdentificacion").val('');
    $("#txtFuncionario").val('');
    $("#txtUnidad").val('');
    $("#txtDependencia").val('');
    $("#txtTelefono").val('');
    $("#txtNombreTrafico").val('');
    $("#txtNombreEstructura").val('');
    $("#txtCantidadEstructura").val('');
    $("#txtNombreInstalacion").val('');
    $("#txtCantidadInstalacion").val('');
    $("#txtCantidadPersonas").val('');
    $("#txtNombreNarcotrafico").val('');
    $("#txtNombreEntornoAfectado").val('');
    $("#txtCaractGenerales").val('');
    $("#txtIdentificacionInteg").val('');
    $("#txtApellidosInteg").val('');
    $("#txtNombreInteg").val('');
    $("#txtAlias").val('');
    $("#txtCelularInteg").val('');
    $("#txtDirecciónInteg").val('');
    $("#txtMunicipio").val('');
    $("#txtBarrio").val('');
    $("#txtCuadrante").val('');
    $("#txtDireccion").val('');
    $("#LONGITUD_CASO").val('');
    $("#LATITUD_CASO").val('');
    $("#txtRadioAccion").val('');

    // Combos con select2 o similares
    $("#ddlCuadrante").val('').trigger('change');
    $("#ddlEspecialidad").val('').trigger('change');
    $("#ddlTipoServicio").val('').trigger('change');
    $("#ddlClase").val('').trigger('change');
    $("#ddlModExpendio").val('').trigger('change');
    $("#ddlClasiNarcotrafico").val('').trigger('change');
    $("#ddlActividad").val('').trigger('change');
    $("#ddlFuente").val('').trigger('change');
    $("#ddlEntono").val('').trigger('change');
    $("#ddlZona").val('').trigger('change');
    $("#ddlDelitoPrincipal").val('').trigger('change');
    $("#ddlDelitoSecundario").val('').trigger('change');

    // Limpiar grilla
    $('#tbGrillaIntegantes').empty();
    $('#pn_GrillaIntegantes').addClass('hidden').removeClass('show');
    $('#pn_GrillaIntegantes').collapse('hide');
}
function limpiarFormularioIntegrantes() {
    $("#txtConsecutivoIntegrante").val('');
    $("#txtAlias").val('');
    $("#txtNombreInteg").val('');
    $("#txtApellidosInteg").val('');
    $("#txtIdentificacionInteg").val('');
    $("#txtCelularInteg").val('');
    $("#txtDirecciónInteg").val('');
    $("#txtApellidosIntegModal").val('');
    $("#txtIdentificacionIntegModal").val('');
    $("#txtNombreIntegModal").val('');
    $("#txtAliasModal").val('');
    $("#txtCelularIntegModal").val('');
    $("#txtDirecciónIntegModal").val('');
}

function limpiarFormularioDelitos() {
    $("#ddlDelitoPrincipalModal").val('').trigger('change');
    $("#ddlDelitoSecundarioModal").val('').trigger('change');

}
function mostrarNombreArchivo(input) {
  const archivo = input.files[0];
    if (archivo) {
        document.getElementById('nombreArchivo').value = archivo.name;
 } else {
        document.getElementById('nombreArchivo').value = "";
    }
}

$('#btnObtener').on('click', function () {
    const seleccionados = obtenerDelitosSecundariosSeleccionados();
});
function obtenerDelitosSecundariosSeleccionados() {
    const delitos = [];
    $('#ddlDelitoSecundario option:selected').each(function () {
        delitos.push($(this).val());
    });
    console.log("Delitos secundarios seleccionados: ", delitos);
    return delitos;
}
function obtenerDelitosSecundariosSeleccionadosModal() {
    const delitos = [];
    $('#ddlDelitoSecundarioModal option:selected').each(function () {
        delitos.push($(this).val());
    });
    console.log("Delitos secundarios seleccionados: ", delitos);
    return delitos;
}
function F_GetDetalleIris(registro) {
   // console.log("✅ Registro recibido:", registro);

    $("#txtCriminalidadIdModal").val(registro.CriminalidadId);
    $("#txtConsecutivoIris").val(registro.CriminalidadId);

    var FechaInicio = registro.FechaInicioExistencia ? moment(registro.FechaInicioExistencia).format('DD/MM/YYYY hh:mm:ss a') : '';
    var FechaCreacion = registro.FechaCreacion ? moment(registro.FechaCreacion).format('DD/MM/YYYY hh:mm:ss a') : '';

    $("#txtCodigoIrispi").text(registro.Codigo);
    $("#txtClaseHeader").text(registro.Clase);
    $("#txtClaseDetalle").text(registro.Clase);
    $("#txtNombreClaseHeader").text(registro.NombreClase);
    $("#txtNombreClaseDetalle").text(registro.NombreClase);
    $("#txtCantidad").text(registro.CantidadIntegrantes);
    $("#txtCantidadDetalle").text(registro.CantidadIntegrantes);
    $("#txtFuente").text(registro.Fuente);
    $("#txtFechaInicio").text(FechaInicio);
    $("#txtCaracteristicas").text(registro.CaracteristicasGenerales);
    $("#txtCelularIris").text(registro.Celular);
    $("#txtCodigoCuadrante").text(registro.Cuadrante);
    $("#txtEstacion1").text(registro.DependCuadrante);
    $("#txtEstacion2").text(registro.Estacioncuadrante);
    $("#txtComando").text(registro.Nivel1cuadrante);
    $("#txtCelularCuadrante").text(registro.CelularCuadrante);
    $("#txtFechaCreacion").text(FechaCreacion);

    var IdenInforma = registro.IdentificacionInforma;

    $.ajax({
        type: "GET",
        url: AppRoutes.RegistroIrisP1.UrlGetFuncionarios,
        async: true,
        data: { V_Identificacion: IdenInforma },
        dataType: 'json',
        cache: false,
        success: function (respuesta) {
            if (respuesta.success) {
                $("#txtFuncionarioDetalle").text(respuesta.data.Funcionario);
                $("#txtUnidadDetalle").text(respuesta.data.Fisica + " - " + respuesta.data.Dependencia);
                $("#txtUnidadDetalle2").text(respuesta.data.Fisica + " - " + respuesta.data.Dependencia);
                $("#txtCorreo").text(respuesta.data.Correo);
                $("#txtCelularSiath").text(respuesta.data.Celular);

                $('#Modal_DetalleIris').modal("show");

                F_GetIntegrantesIris(registro.CriminalidadId);
                F_GetUbicacionIris(registro.CriminalidadId);
                F_GetDelitosIris(registro.CriminalidadId);
                F_GetInfoAdiconalIris(registro.CriminalidadId);
                F_GetDocumentosIris(registro.CriminalidadId);
                F_GetFotosIris(registro.CriminalidadId);
            } else {
                Swal.fire({
                    type: 'error',
                    title: 'Señor(a) Funcionario(a:)',
                    text: "No se Encontro el Funcionario"
                });
            }
        },
        error: function () {
            Swal.fire({
                type: 'error',
                title: 'Señor(a) Funcionario(a:)',
                text: 'No es posible consultar, revise!!'
            });
        }
    });
}

function F_GetRelacionIris() {
    $.ajax({
        type: 'GET',
        url: AppRoutes.RegistroIrisP1.UrlGetInfoGrillas
, // URL del endpoint que devuelve los datos
        dataType: 'json',
        data: { V_Anio: $('#ddlAnioIris').val() },
        success: function (response) {

            // Inicializar la grilla con los datos filtrados o vacíos
            GetGrillaRelacionIris(response.data);
         
        },
        error: function () {
            // En caso de error, inicializar la grilla con datos vacíos
            GetGrillaRelacionIris([]);
         
        }
    });
}
function GetGrillaRelacionIris(Datos) {
    if ($.fn.dataTable.isDataTable("#tbGrillaRelacionIrisP1")) {
        $("#tbGrillaRelacionIrisP1").DataTable().destroy();
    }

    $("#tbGrillaRelacionIrisP1").empty();
    $("#pn_GrillaRelacionIrisP1").removeClass('hidden');

    //$("#tbGrillaRelacionIrisP1").DataTable({
    //    destroy: true,
    //    data: Datos,
    //    language: glOpcionesIdioma,
    //    responsive: true,
    //    "columns": [
    //        {
    //            data: null, className: "celdaCenter celda3", "render": function (data, type, row) {
    //                var inicioBoton = '<div class="dropdown dropend"><button class="btn btn-success" type="button" id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false"><span class="fas fa-list"></span></button><ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';
    //                var Eliminar = `<li style="padding-left: 17px;"><a style="color: #102717;" href="javascript:Dell_Roles(${row.IdUserRol})"><i class="fa fa-trash red"></i>&nbsp;Eliminar</a></li>`;
    //                var finBoton = '</ul></div>';
    //                return inicioBoton + Eliminar + finBoton;
    //            }
    //        },
    //        { "title": "Roles Asignados", "data": "Descripcion", "name": "Descripcion", className: "celdaCenter celda5" },
    //        { "title": "Fecha de Asignación", "data": "FechaCreacion", "name": "FechaCreacion", className: "celdaCenter celda7" },
    //        { "title": "Funcionario que Asignó", "data": "FuncionarioCreacion", "name": "FuncionarioCreacion", className: "celdaJust celda17" },
    //        { "title": "Fecha Caducidad", "data": "FechaFin", "name": "FechaFin", className: "celdaCenter celda7" },
    //        { "title": "Observaciones", "data": "Justificacion", "name": "Justificacion", className: "celdaJust" }
    //    ],
    //    lengthMenu: [
    //        [5, 10, 25, 50, -1],
    //        ['5 registros', '10 registros', '25 registros', '50 registros', 'Todos']
    //    ],
    //    ordering: false,
    //    pageLength: 10,
    //    bLengthChange: true,
    //    searching: true,
    //    paging: true,
    //    info: true
    //});
}
function F_GetIntegrantesIris(CriminalidadId) {
   

    $.ajax({
        type: 'GET',
        url: AppRoutes.RegistroIrisP1.UrlGetIntegrantes,
        async: true,
        data: { V_CriminalidadId: CriminalidadId },
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                
                GetGrillaIntegrantesIris(response.data);
            } else {
                GetGrillaIntegrantesIris([]);
               // Swal.fire('Error', response.message, 'error');
            }
        },
        error: function () {
            GetGrillaIntegrantesIris([]);
            Swal.fire('Error', 'No se pudo obtener la lista de integrantes.', 'error');
        }
    });
}
function GetGrillaIntegrantesIris(Datos) {
    if ($.fn.dataTable.isDataTable("#tbGrillaIntegrantesIrisP1")) {
        $("#tbGrillaIntegrantesIrisP1").DataTable().destroy();
    }

    $("#tbGrillaIntegrantesIrisP1").empty();
    $("#pn_GrillaIntegrantesIrisP1").removeClass('hidden');

    $("#tbGrillaIntegrantesIrisP1").DataTable({
        destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        responsive: true,
        "columns": [
            {
                data: null, className: "celdaCenter celda3", "render": function (data, type, row) {
                    var inicioBoton = '<div class="dropdown dropend"><button class="btn btn-success" type="button" id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false"><span class="fas fa-list"></span></button><ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';
                    var Eliminar = `<li style="padding-left: 17px;">
                                        <a style="color: #102717; cursor:pointer;" onclick="P_DelIntegranteIris('${row.INTEGRANTE_ID}')">
                                        <i class="fa fa-trash red"></i>&nbsp;Eliminar
                                        </a>
                                    </li>`;

                    var finBoton = '</ul></div>';
                    return inicioBoton + Eliminar + finBoton;
                }
            },
            { "title": "Alias", "data": "ALIAS", "name": "ALIAS", className: "celdaCenter celda5" },
            { "title": "Nombre", "data": "NOMBRE", "name": "NOMBRE", className: "celdaCenter celda7" },
            { "title": "Apellido", "data": "APELLIDO", "name": "APELLIDO", className: "celdaCenter celda17" },
            { "title": "Cédula", "data": "CEDULA", "name": "CEDULA", className: "celdaCenter celda7" },
            { "title": "Dirección", "data": "DIRECCION", "name": "DIRECCION", className: "celdaCenter" },
            {
                  title: "Fecha Creación",
                  data: "FECHA_CREACION",
                  name: "FECHA_CREACION",
                  render: function (data) {
                    if (!data) return "";
                    const fecha = moment(data).format('DD/MM/YYYY');
                    const hora = moment(data).format('hh:mm:ss a');
                    return `${fecha} - ${hora}`;

                  }
            }

        ],
        lengthMenu: [
            [5, 10, 25, 50, -1],
            ['5 registros', '10 registros', '25 registros', '50 registros', 'Todos']
        ],
        lengthChange: false,
        searching: false,
        ordering: false,
        pageLength: 10,
        paging: false,
        info: false
    });
}


function P_InsUbicacionModal() {

    const Obj_Ubicacion = {

        CriminalidadId: $("#txtCriminalidadIdModal").val(),
        Latitud: $("#LATITUD_CASO").val(),
        Longitud: $("#LONGITUD_CASO").val(),
        MunicipioUbica: $("#txtMunicipio").val(),
        Barrio: $("#txtBarrio").val(),
        CuadranteUbica: $("#txtCuadrante").val(),
        RadioAccion: $("#txtRadioAccion").val(),
        Direccion: $("#txtDireccion").val(),
        CodigoDane: $("#txtCodDane").val(),
        CodigoEstacion: $("#txtCodEstacion").val(),
        CodigoSiedcoCuadrante: $("#txtCodSiedcoCuadrante").val(),
      
       
    }


    if (!Obj_Ubicacion.CriminalidadId || !Obj_Ubicacion.Latitud || !Obj_Ubicacion.Longitud ) {
        Swal.fire({
            icon: 'warning',
            title: 'Campos obligatorios',
            text: 'Valide la información ingresada.'
        });
        return; // Detener ejecución si faltan campos
    }


    $.ajax({
        url: AppRoutes.RegistroIrisP1.UrlInsUbicacion
,
        type: 'POST',
        data: Obj_Ubicacion,
        success: function (resp) {
            if (resp.success) {

                F_GetUbicacionIris($("#txtCriminalidadIdModal").val());
                $('#myModal2').modal('hide');
              
            } else {

                Swal.fire({
                    type: 'error',
                    title: 'Señor(a) Funcionario(a:)',
                    text: 'Error al insertar: ' + resp.message
                });
            }
        },
        error: function () {
            Swal.fire('Error', 'Fallo en la llamada AJAX.', 'error');
        }
    });

}


function F_GetUbicacionIris(CrininalidadId) {


    $.ajax({
        type: 'GET',
        url: AppRoutes.RegistroIrisP1.UrlGetUbicacion
,
        async: true,
        data: { V_CriminalidadId: CrininalidadId },
        dataType: 'json',
        success: function (response) {
            if (response.success) {

                GetGrillaUbicacionIris(response.data);
            } else {
                GetGrillaUbicacionIris([]);
               // Swal.fire('Error', response.message, 'error');
            }
        },
        error: function () {
            GetGrillaUbicacionIris([]);
            Swal.fire('Error', 'No se pudo obtener la lista de ubicaciones.', 'error');
        }
    });
}


function GetGrillaUbicacionIris(Datos) {
    if ($.fn.dataTable.isDataTable("#tbGrillaUbicacionIrisP1")) {
        $("#tbGrillaUbicacionIrisP1").DataTable().destroy();
    }

    $("#tbGrillaUbicacionIrisP1").empty();
    $("#pn_GrillaUbicacionIrisP1").removeClass('hidden');

    $("#tbGrillaUbicacionIrisP1").DataTable({
        destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        responsive: true,
        "columns": [
            {
                data: null, className: "celdaCenter celda3", "render": function (data, type, row) {
                    var inicioBoton = '<div class="dropdown dropend"><button class="btn btn-success" type="button" id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false"><span class="fas fa-list"></span></button><ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';
                    var Eliminar = `<li style="padding-left: 17px;"><a style="color: #102717;" href="javascript:P_DelUbicacionIris('${row.UbicacionId}')"><i class="fa fa-trash red"></i>&nbsp;Eliminar</a></li>`;
                    var finBoton = '</ul></div>';
                    return inicioBoton + Eliminar + finBoton;
                }
            },
            { "title": "Longitud", "data": "Longitud", "name": "Longitud", className: "celdaCenter celda4" },
            { "title": "Latitud", "data": "Latitud", "name": "Latitud", className: "celdaCenter celda4" },
            { "title": "Radio", "data": "RadioAccion", "name": "RadioAccion", className: "celdaCenter celda3" },
            { "title": "Municipio", "data": "MunicipioUbica", "name": "MunicipioUbica", className: "celdaCenter celda5" },
            { "title": "Cuadrante", "data": "Cuadrante", "name": "Cuadrante", className: "celdaCenter celda6" },
            { "title": "Dirección", "data": "Direccion", "name": "Direccion", className: "celdaCenter" }
        ],
       
        lengthChange: false,
        searching: false,
        ordering: false,
        pageLength: 10,
        paging: false,
        info: false
    });
}

function F_GetDelitosIris(CriminalidadId) {
    $.ajax({
        type: 'GET',
        url: AppRoutes.RegistroIrisP1.UrlGetDelitosIris
, // URL del endpoint que devuelve los datos
        dataType: 'json',
        data: { V_CriminalidadId: CriminalidadId },
        success: function (response) {

            // Inicializar la grilla con los datos filtrados o vacíos
            GetGrillaDelitosIris(response.data);

        },
        error: function () {
            // En caso de error, inicializar la grilla con datos vacíos
            GetGrillaDelitosIris([]);

        }
    });
}

function GetGrillaDelitosIris(Datos) {
    if ($.fn.dataTable.isDataTable("#tbGrillaDelitosIrisP1")) {
        $("#tbGrillaDelitosIrisP1").DataTable().destroy();
    }

    $("#tbGrillaDelitosIrisP1").empty();
    $("#pn_GrillaDelitosIrisP1").removeClass('hidden');

    $("#tbGrillaDelitosIrisP1").DataTable({
        destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        responsive: true,
        "columns": [
            {
                data: null, className: "celdaCenter celda3", "render": function (data, type, row) {
                    var inicioBoton = '<div class="dropdown dropend"><button class="btn btn-success" type="button" id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false"><span class="fas fa-list"></span></button><ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';
                    var Eliminar = `<li style="padding-left: 17px;"><a style="color: #102717;" href="javascript:P_DelDelitosIris('${row.DelitoId}')"><i class="fa fa-trash red"></i>&nbsp;Eliminar</a></li>`;
                    var finBoton = '</ul></div>';
                    return inicioBoton + Eliminar + finBoton;
                }
            },
            { "title": "Delito", "data": "DelitoDesc", "name": "DelitoDesc", className: "celdaCenter celda40" },
            { "title": "Tipo", "data": "DescTipo", "name": "DescTipo", className: "celdaCenter celda5" },
            { "title": "Tipo Informacón", "data": "DescTipoInfo", "name": "DescTipoInfo", className: "celdaCenter " }
          
        ],
       
        lengthChange: false,
        searching: false,
        ordering: false,
        pageLength: 10,
        paging: false,
        info: false
    });
}


function F_GetInfoAdiconalIris(CriminalidadId) {
    $.ajax({
        type: 'GET',
        url: AppRoutes.RegistroIrisP1.UrlGetInfoAdicional
, // URL del endpoint que devuelve los datos
        dataType: 'json',
        data: { V_CriminalidadId: CriminalidadId },
        success: function (response) {

            // Inicializar la grilla con los datos filtrados o vacíos
            GetGrillaInfoAdicionalIris(response.data);

        },
        error: function () {
            // En caso de error, inicializar la grilla con datos vacíos
            GetGrillaInfoAdicionalIris([]);

        }
    });
}

function GetGrillaInfoAdicionalIris(Datos) {
    if ($.fn.dataTable.isDataTable("#tbGrillaInforAdicionalIrisP1")) {
        $("#tbGrillaInforAdicionalIrisP1").DataTable().destroy();
    }

    $("#tbGrillaInforAdicionalIrisP1").empty();
    $("#pn_GrillaInfoAdicionalIrisP1").removeClass('hidden');

    $("#tbGrillaInforAdicionalIrisP1").DataTable({
        destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        responsive: true,
        "columns": [
            {
                data: null, className: "celdaCenter celda3", "render": function (data, type, row) {
                    var inicioBoton = '<div class="dropdown dropend"><button class="btn btn-success" type="button" id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false"><span class="fas fa-list"></span></button><ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';
                    var Eliminar = `<li style="padding-left: 17px;"><a style="color: #102717;" href="javascript:P_DelDelInfoAdicionalIris('${row.InfoId}')"><i class="fa fa-trash red"></i>&nbsp;Eliminar</a></li>`;
                    var finBoton = '</ul></div>';
                    return inicioBoton + Eliminar + finBoton;
                }
            },
            columnaInforAdicionalDetalleIris(),
            { "title": "Tipo Información", "data": "DescTipoInfo", "name": "DescTipoInfo", className: "celdaCenter" }
            
        ],
        
        lengthChange: false,
        searching: false,
        ordering: false,
        pageLength: 10,
        paging: false,
        info: false
    });
}

function F_GetResponsableIris() {
    $.ajax({
        type: 'GET',
        url: UrlGetResponsable, // URL del endpoint que devuelve los datos
        dataType: 'json',
        data: { V_Anio: $('#ddlAnioIris').val() },
        success: function (response) {

            // Inicializar la grilla con los datos filtrados o vacíos
            GetGrillaResponsableIris(response.data);

        },
        error: function () {
            // En caso de error, inicializar la grilla con datos vacíos
            GetGrillaResponsableIris([]);

        }
    });
}

function GetGrillaResponsableIris(Datos) {
    if ($.fn.dataTable.isDataTable("#tbGrillaResponsableIrisP1")) {
        $("#tbGrillaResponsableIrisP1").DataTable().destroy();
    }

    $("#tbGrillaResponsableIrisP1").empty();
    $("#pn_GrillaResponsableIrisP1").removeClass('hidden');

    //$("#pn_GrillaResponsableIrisP1").DataTable({
    //    destroy: true,
    //    data: Datos,
    //    language: glOpcionesIdioma,
    //    responsive: true,
    //    "columns": [
    //        {
    //            data: null, className: "celdaCenter celda3", "render": function (data, type, row) {
    //                var inicioBoton = '<div class="dropdown dropend"><button class="btn btn-success" type="button" id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false"><span class="fas fa-list"></span></button><ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';
    //                var Eliminar = `<li style="padding-left: 17px;"><a style="color: #102717;" href="javascript:Dell_Roles(${row.IdUserRol})"><i class="fa fa-trash red"></i>&nbsp;Eliminar</a></li>`;
    //                var finBoton = '</ul></div>';
    //                return inicioBoton + Eliminar + finBoton;
    //            }
    //        },
    //        { "title": "Roles Asignados", "data": "Descripcion", "name": "Descripcion", className: "celdaCenter celda5" },
    //        { "title": "Fecha de Asignación", "data": "FechaCreacion", "name": "FechaCreacion", className: "celdaCenter celda7" },
    //        { "title": "Funcionario que Asignó", "data": "FuncionarioCreacion", "name": "FuncionarioCreacion", className: "celdaJust celda17" },
    //        { "title": "Fecha Caducidad", "data": "FechaFin", "name": "FechaFin", className: "celdaCenter celda7" },
    //        { "title": "Observaciones", "data": "Justificacion", "name": "Justificacion", className: "celdaJust" }
    //    ],
    //    lengthMenu: [
    //        [5, 10, 25, 50, -1],
    //        ['5 registros', '10 registros', '25 registros', '50 registros', 'Todos']
    //    ],
    //    ordering: false,
    //    pageLength: 10,
    //    bLengthChange: true,
    //    searching: true,
    //    paging: true,
    //    info: true
    //});
}

function F_GetDocumentosIris(CriminalidadId) {
    $.ajax({
        type: 'GET',
        url: AppRoutes.RegistroIrisP1.UrlGetDocIris
, // URL del endpoint que devuelve los datos
        dataType: 'json',
        data: { V_CriminalidadId: CriminalidadId },
        success: function (response) {

            // Inicializar la grilla con los datos filtrados o vacíos
            GetGrillaDocumentosIris(response.data);

        },
        error: function () {
            // En caso de error, inicializar la grilla con datos vacíos
            GetGrillaDocumentosIris([]);

        }
    });
}

function GetGrillaDocumentosIris(Datos) {
    if ($.fn.dataTable.isDataTable("#tbGrillaDocumentosIrisP1")) {
        $("#tbGrillaDocumentosIrisP1").DataTable().destroy();
    }

    $("#tbGrillaDocumentosIrisP1").empty();
    $("#pn_GrillaDocumentosIrisP1").removeClass('hidden');

    $("#tbGrillaDocumentosIrisP1").DataTable({
        destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        responsive: true,
        "columns": [
            {
                data: null, className: "celdaCenter celda3", "render": function (data, type, row) {
                    var inicioBoton = '<div class="dropdown dropend"><button class="btn btn-success" type="button" id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false"><span class="fas fa-list"></span></button><ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';
                    var Eliminar = `<li style="padding-left: 17px;"><a style="color: #102717;" href="javascript:P_DelDocumentoIris('${row.documentoId}')"><i class="fa fa-trash red"></i>&nbsp;Eliminar</a></li>`;
                    var finBoton = '</ul></div>';
                    return inicioBoton + Eliminar + finBoton;
                }
            },
            { "title": "Nombre", "data": "nombre", "name": "nombre", className: "celdaCenter celda5" },
            {
                "title": "Enlace",
                "data": "ruta",
                "name": "ruta",
                className: "celdaCenter celda7",
                "render": function (data, type, row) {
                    if (!data || data.trim() === "") {
                        return '';
                    }


                    // Opción 1: enlace azul visible sobre fondo blanco
                  /*  return `<a href="${data}" target="_blank" style="color: #007bff; font-weight: bold; text-decoration: underline;">Descargar</a>`;*/
                    return `<a href="Irisp1/RegistroIrisp1/descargar?ruta=${encodeURIComponent(data)}" target="_blank" style="background-color: #236305; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px; text-decoration: none;">Descargar</a>`;
                }
            },
            { "title": "Fecha Creación", "data": "fechaCreacion", "name": "fechaCreacion", className: "celdaJust celda17", render: formatDate }
        ],
        lengthChange: false,
        searching: false,
        ordering: false,
        pageLength: 10,
        paging: false,
        info: false
    });
}


function F_GetFotosIris(CriminalidadId) {
    $.ajax({
        type: 'GET',
        url: AppRoutes.RegistroIrisP1.UrlGetFotosIris
, // Endpoint del backend
        dataType: 'json',
        data: { V_CriminalidadId: CriminalidadId },
        success: function (response) {
           
            if (response.exito && response.data.length > 0) {
                RenderGaleriaFotos(response.data);
                $("#pn_GrillaFotografiasIrisP1").removeClass('hidden');
            } else {
                $("#contenedorFotosIrisP1").html('<p class="text-center text-muted">No hay fotos disponibles</p>');
            }
        },

        error: function () {
            $("#contenedorFotosIrisP1").html('<p class="text-center text-danger">Error al cargar las fotos</p>');
        }
    });
}


function RenderGaleriaFotos(fotos) {
    let html = '<div class="row">';
    


    fotos.forEach(function (foto) {
        console.log("Ruta Foto:", foto.ruta);

        const urlFoto = `Irisp1/RegistrosIrisp1/Fotos?V_Ruta=${encodeURIComponent(foto.ruta)}`;


        html += `
            <div class="col-md-3 col-sm-4 col-6 mb-3">
                <div class="card shadow-sm border-light">
                    <img src="${urlFoto}" class="card-img-top img-thumbnail" 
                         style="height: 180px; object-fit: cover; cursor: pointer;"
                         alt="${foto.nombreArchivo}" 
                         onclick="VerFotoGrande('${urlFoto}')">
                </div>
            </div>
        `;
    });

    html += '</div>';
    $("#contenedorFotosIrisP1").html(html);
}

// Modal para ver la imagen grande
function VerFotoGrande(ruta) {
    const modalHtml = `
        <div class="modal fade" id="modalFotoGrande" tabindex="-1">
          <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content">
              <div class="modal-body text-center">
                <img src="${ruta}" class="img-fluid rounded" alt="Foto" />
              </div>
            </div>
          </div>
        </div>
    `;
    $("body").append(modalHtml);
    $("#modalFotoGrande").modal("show");

    $("#modalFotoGrande").on("hidden.bs.modal", function () {
        $(this).remove();

        // 🔹 Forzar que la modal principal siga activa y con scroll
        if ($('.modal.show').length) {
            $('body').addClass('modal-open');
        }
    });
}

function OpenInsIntegrantesModal() {

    $('#Modal_InsIntegrantes').modal("show");
}

function OpenInsUbicacionModal() {
    $('#myModal2').modal("show");

}

$('#myModal2').on('shown.bs.modal', function () {
    inicializarMapa('mapaDiv2');
});


$(function () {
    $("#btnMapa").click(function () {
        $('#myModal').modal("show");
    });
});


$('#myModal').on('shown.bs.modal', function () {
    inicializarMapa('mapaDiv');
});

function OpenInsDelitosModal() {

    $('#Modal_InsDelitos').modal("show");
}

function OpenInsInfoadiconalModal() {

    $('#Modal_InsInfoAdicional').modal("show");
}


// Insertar integrantes en la modal ver detalle
function InsIntegrantesModal() {

    var Criminalidad_Id = $("#txtCriminalidadIdModal").val();
    // Obtener valores de los campos y limpiar espacios
    const identificacion = $("#txtIdentificacionIntegModal").val().trim();
    const apellidos = $("#txtApellidosIntegModal").val().trim();
    const nombres = $("#txtNombreIntegModal").val().trim();
    const celular = $("#txtCelularIntegModal").val().trim();
    const direccion = $("#txtDirecciónIntegModal").val().trim();
    const alias = $("#txtAliasModal").val().trim();

    // Validación de campos obligatorios:
    // 1. Identificación siempre requerida
    // 2. Al menos uno entre Nombre o Alias
    if (!Criminalidad_Id || (!nombres && !alias)) {
        Swal.fire({
            icon: 'warning',
            title: 'Campos obligatorios',
            text: 'Debe diligenciar al menos Nombre o Alias.'
        });
        return; // Detener ejecución si faltan campos
    }

    // Objeto de integrante (sin consecutivo, lo calcula la BD)
    const Obj_Integrante = {
        CRIMINALIDAD_ID: Criminalidad_Id,
        ALIAS: alias,
        NOMBRE: nombres,
        APELLIDO: apellidos,
        CEDULA: parseInt(identificacion),
        ID_TIPO_INFO: 30,
        VIGENTE: 1,
        FECHA_MODIFICA: null,
        IDENTIFICACION_MODIFICA: null,
        MAQUINA_MODIFICA: null,
        TIPO_DOCUMENTO: 1,
        CELULAR: celular ? parseInt(celular) : null,
        DIRECCION: direccion
    };

    $.ajax({
        url: AppRoutes.RegistroIrisP1.UrlInsIntegrantes,
        type: 'POST',
        data: Obj_Integrante,
        success: function (resp) {
            if (resp.success) {
                F_GetIntegrantesIris(Criminalidad_Id);
                $('#Modal_InsIntegrantes').modal('hide');
                limpiarFormularioIntegrantes();
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Señor(a) Funcionario(a):',
                    text: 'Error al insertar: ' + resp.message
                });
            }
        },
        error: function () {
            Swal.fire('Error', 'Fallo en la llamada AJAX.', 'error');
        }
    });
}


function P_InsDelitosModal() {

    var Obj_DelitosSecundarios = obtenerDelitosSecundariosSeleccionadosModal();

    const Obj_DelitosIris = {
        CriminalidadId: $("#txtCriminalidadIdModal").val(),
        IdDelitoPrincipal: $("#ddlDelitoPrincipalModal").val(),
        IdDelitoSecundario: Obj_DelitosSecundarios
    };

    // VALIDACIÓN CORRECTA
    if (
        !Obj_DelitosIris.CriminalidadId ||
        (
            (!Obj_DelitosIris.IdDelitoPrincipal || Obj_DelitosIris.IdDelitoPrincipal === "") &&
            Obj_DelitosIris.IdDelitoSecundario.length === 0
        )
    ) {
        Swal.fire({
            icon: 'warning',
            title: 'Campos obligatorios',
            text: 'Debe Seleccionar al menos un Delito Principal o Secundario.'
        });
        return;
    }

    $.ajax({
        url: AppRoutes.RegistroIrisP1.UrlInsDelitos,
        type: 'POST',
        data: Obj_DelitosIris,
        success: function (resp) {
            if (resp.success) {
                F_GetDelitosIris($("#txtCriminalidadIdModal").val());
                $('#Modal_InsDelitos').modal('hide');
                limpiarFormularioDelitos();
            } else {

                Swal.fire({
                    icon: 'error',
                    title: 'Señor(a) Funcionario(a:)',
                    text: 'Error al insertar: ' + resp.message
                });
            }
        },
        error: function () {
            Swal.fire('Error', 'Fallo en la llamada AJAX.', 'error');
        }
    });
}


function P_InsInfoAdicionalModal() {

    const criminalidadId = $("#txtCriminalidadIdModal").val().trim();
    const descripcion = $("#txtInfoAdicionalModal").val().trim();

    // Validación de campos vacíos
    if (criminalidadId === "" || descripcion === "") {
        Swal.fire({
            icon: 'warning',
            title: 'Campos obligatorios',
            text: 'Por favor, complete todos los campos antes de continuar.'
        });
        return; // Detener la ejecución si hay campos vacíos
    }

    const Obj_InfoAdicional = {
        CriminalidadId: criminalidadId,
        Descripcion: descripcion,
    };

    $.ajax({
        url: AppRoutes.RegistroIrisP1.UrlInsInfoAdicional
,
        type: 'POST',
        data: Obj_InfoAdicional,
        success: function (resp) {
            if (resp.success) {
                F_GetInfoAdiconalIris(criminalidadId);
                $("#Modal_InsInfoAdicional").modal("hide");
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Señor(a) Funcionario(a):',
                    text: 'Error al insertar: ' + resp.message
                });
            }
        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Señor(a) Funcionario(a):',
                text: "Error en la solicitud"
            });
        }
    });
}

function subirDocumentoSeleccionado(input) {

    if (!input.files || input.files.length === 0) {
        Swal.fire('Error', 'Debe seleccionar un documento válido.', 'error');
        return;
    }

    let file = input.files[0];
    let idCriminalidad = $("#txtConsecutivoIris").val();

    if (!idCriminalidad) {
        Swal.fire('Error', 'Faltan datos requeridos para guardar la imagen.', 'error');
        return;
    }

    let formData = new FormData();
    formData.append('documento', file);
    formData.append('idCriminalidad', idCriminalidad);

    $.ajax({
        url: 'Irisp1/RegistrosIrisp1/GuardarDocumentoConRegistro',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        beforeSend: function () {
            Swal.fire({
                title: 'Cargando...',
                allowOutsideClick: false,
                didOpen: () => Swal.showLoading()
            });
        },
        success: function (response) {

            Swal.close();

            // Muy importante: tu controlador devuelve { exito, mensaje }
            if (response.exito === true) {

                Swal.fire({
                    icon: 'success',
                    title: 'Señor(a) Funcionario(a:)',
                    text: response.mensaje
                });

                // Vuelve a cargar los documentos del IRIS
                F_GetDocumentosIris($("#txtCriminalidadIdModal").val());

            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: response.mensaje || 'No se pudo procesar el documento.'
                });
            }
        },
        error: function (xhr) {
            Swal.close();

            let msg = 'Ocurrió un error al cargar el documento';

            if (xhr.responseJSON && xhr.responseJSON.mensaje) {
                msg = xhr.responseJSON.mensaje;
            }

            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: msg
            });
        }
    });
}


function actualizarCriminalidad() {
    var data = {
        CriminalidadId: $("#txtCriminalidadIdUpd").text(),
        IdClase: $("#ddlClaseModal").val(),
        NombreClase: $("#txtNombreTraficoModal").val() || $("#txtNombreEstructuraModal").val() || $("#txtNombreInstalacionModal").val() || $("#txtNombreNarcotraficoModal").val(),
        CantidadIntegrantes: $("#txtCantidadEstructuraModal").val() || $("#txtCantidadInstalacionModal").val() || $("#txtCantidadPersonasModal").val(),
        CaracteristicasGenerales: $("#txtCaractGeneralesModal").val(),
        IdFuente: $("#ddlFuenteModal").val()
    };

    // Validación de campos obligatorios
    if (data.IdClase === '15') {
        if (!data.CriminalidadId ||
            !data.IdClase ||
            // !data.NombreClase ||  // excluido en este caso
            !data.CantidadIntegrantes ||
            !data.CaracteristicasGenerales ||
            !data.IdFuente) {

            Swal.fire({
                icon: 'warning',
                title: 'Campos obligatorios',
                text: 'Todos los campos son obligatorios. Por favor complete la información.'
            });
            return; // Detener ejecución
        }
    } else {
        if (!data.CriminalidadId ||
            !data.IdClase ||
            !data.NombreClase ||
            !data.CantidadIntegrantes ||
            !data.CaracteristicasGenerales ||
            !data.IdFuente) {

            Swal.fire({
                icon: 'warning',
                title: 'Campos obligatorios',
                text: 'Todos los campos son obligatorios. Por favor complete la información.'
            });
            return; // Detener ejecución
        }
    }

    $.ajax({
        url: AppRoutes.RegistroIrisP1.UrlUpdCriminalidad
,
        type: 'POST',
        data: data,
        success: function (response) {
            if (response.success) {
                Swal.fire("Éxito", response.message, "success");
                $("#Modal_UpdRegistroIris").modal("hide");
                var fecha = $('#ddlAnioIris').val(); // obtengo valor actual
                $('#ddlAnioIris').val(fecha).trigger('change'); // lo reasigno para refrescar
                F_GetInfoGrillas();

                $("#ddlClaseModal").val('').trigger('change');
                $("#txtNombreTraficoModal").val('');
                $("#ddlModExpendioModal").val('').trigger('change');
                $("#txtNombreEstructuraModal").val('');
                $("#txtCantidadEstructuraModal").val('');
                $("#txtNombreInstalacionModal").val('');
                $("#txtCantidadInstalacionModal").val('');
                $("#txtCantidadPersonasModal").val('');
                $("#txtNombreNarcotraficoModal").val('');
                $("#txtCaractGeneralesModal").val('');
                $("#ddlClasiNarcotraficoModal").val('').trigger('change');
                $("#ddlFuenteModal").val('').trigger('change');
            } else {
                Swal.fire("Atención", response.message, "warning");
            }
        },
        error: function (xhr, status, error) {
            Swal.fire("Error", "Ocurrió un error al intentar actualizar", "error");
        }
    });
}

function actualizarEstadoCriminalidad() {

    var data = {
        CriminalidadId: $("#txtCriminalidadIdUpd").text(),
        IdEstado: $("#ddlEstadosIrisP1").val(),
     
    };

    // Validación de campos obligatorios
    if (!data.CriminalidadId ||
        !data.IdEstado) {

        Swal.fire("Atención", "Todos los campos son obligatorios. Por favor complete la información.", "warning");
        return; // Detener ejecución
    }

    $.ajax({
        url: AppRoutes.RegistroIrisP1.UrlUpdEstadoCriminalidad
,
        type: 'POST',
        data: data,
        success: function (response) {
            if (response.success) {
                Swal.fire("Éxito", response.message, "success");
                $("#Modal_UpdEstadoRegistroIris").modal("hide");
                $("#ddlEstadosIrisP1").val('').trigger('change');

                var fecha = $('#ddlAnioIris').val(); // obtengo valor actual
                $('#ddlAnioIris').val(fecha).trigger('change'); // lo reasigno para refrescar
                F_GetInfoGrillas();
                
               
            } else {
                Swal.fire("Atención", response.message, "warning");
            }
        },
        error: function (xhr, status, error) {
            Swal.fire("Error", "Ocurrió un error al intentar actualizar", "error");
        }
    });
}

function actualizarExistenciaCriminalidad() {

    var data = {
        CriminalidadId: $("#txtCriminalidadIdUpd").text(),
        IdEstadoExistencia: $("#ddlExistenciaIrisP1").val(),

    };

    // Validación de campos obligatorios
    if (!data.CriminalidadId ||
        !data.IdEstadoExistencia) {

        Swal.fire("Atención", "Todos los campos son obligatorios. Por favor complete la información.", "warning");
        return; // Detener ejecución
    }

    $.ajax({
        url: AppRoutes.RegistroIrisP1.UrlUpdExistenciaCriminalidad
,
        type: 'POST',
        data: data,
        success: function (response) {
            if (response.success) {
                Swal.fire("Éxito", response.message, "success");
                $("#Modal_UpdExistenciaRegistroIris").modal("hide");
                $("#ddlEstadosIrisP1").val('').trigger('change');

                var fecha = $('#ddlAnioIris').val(); // obtengo valor actual
                $('#ddlAnioIris').val(fecha).trigger('change'); // lo reasigno para refrescar
                F_GetInfoGrillas();


            } else {
                Swal.fire("Atención", response.message, "warning");
            }
        },
        error: function (xhr, status, error) {
            Swal.fire("Error", "Ocurrió un error al intentar actualizar", "error");
        }
    });
}

// Funciones de Eliminación
function DellIris(CriminalidadId) {

    bootbox.confirm({
        message: "¿Está seguro de eliminar el Iris-P1 seleccionado?",
        buttons: {
            confirm: {
                label: '<i class="fa fa-check"></i> Sí',
                className: 'btn-success'
            },
            cancel: {
                label: '<i class="fa fa-times"></i> No',
                className: 'btn-danger'
            }
        },
        callback: function (result) {
            if (result) {
                // Llamar directamente a la función de eliminación sin pedir motivo
              
                $.ajax({
                    type: 'POST',
                    url: AppRoutes.RegistroIrisP1.UrlDelIris
,
                    async: true,
                    dataType: 'json',
                    data: { CriminalidadId: CriminalidadId },
                    success: function (result) {
                        if (result.success) {

                            var fecha = $('#ddlAnioIris').val(); // obtengo valor actual
                            $('#ddlAnioIris').val(fecha).trigger('change'); // lo reasigno para refrescar
                            F_GetInfoGrillas();
                            Swal.fire({
                                type: 'success',
                                title: 'Señor(a) Funcionario(a:)',
                                text: result.message
                            });

                        } else {
                            Swal.fire({
                                type: 'error',
                                title: 'Señor(a) Funcionario(a:)',
                                text: result.message
                            });
                        }
                    },
                    error: function (ex) {
                        Swal.fire({
                            type: 'error',
                            title: 'Señor(a) Funcionario(a:)',
                            text: "No es posible grabar, revise"
                        });
                    }
                });
            }
        }
    });
}

function P_DelIntegranteIris(IntegranteId) {

    Swal.fire({
        title: '¿Está seguro?',
        text: "¿Desea eliminar este integrante del registro?",
        type: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {

        if (result.value) { // en versiones viejas no existe "isConfirmed"

            $.ajax({
                type: 'POST',
                url: AppRoutes.RegistroIrisP1.UrlDelIntegrante,
                async: true,
                dataType: 'json',
                data: { IntegranteId: IntegranteId },

                success: function (result) {
                    if (result.success) {

                        var ID = $("#txtCriminalidadIdModal").val();
                        F_GetIntegrantesIris(ID);

                        Swal.fire({
                            type: 'success',
                            title: 'Señor(a) Funcionario(a):',
                            text: result.message
                        });

                    } else {

                        Swal.fire({
                            type: 'error',
                            title: 'Señor(a) Funcionario(a):',
                            text: result.message
                        });

                    }
                },

                error: function () {
                    Swal.fire({
                        type: 'error',
                        title: 'Señor(a) Funcionario(a):',
                        text: "No es posible eliminar el registro, por favor revise."
                    });
                }
            });

        }

    });
}

       

function P_DelDelitosIris(DelitoId) {

    Swal.fire({
        title: '¿Está seguro?',
        text: "¿Desea eliminar este delito del registro?",
        type: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {

        if (result.value) {

            $.ajax({
                type: 'POST',
                url: AppRoutes.RegistroIrisP1.UrlDelDelitos,
                async: true,
                dataType: 'json',
                data: { DelitoId: DelitoId },
                success: function (result) {

                    if (result.success) {

                        F_GetDelitosIris($("#txtCriminalidadIdModal").val());

                        Swal.fire({
                            type: 'success',
                            title: 'Señor(a) Funcionario(a):',
                            text: result.message
                        });

                    } else {

                        Swal.fire({
                            type: 'error',
                            title: 'Señor(a) Funcionario(a):',
                            text: result.message
                        });

                    }
                },
                error: function () {
                    Swal.fire({
                        type: 'error',
                        title: 'Señor(a) Funcionario(a):',
                        text: "No es posible eliminar, revise."
                    });
                }
            });
        }
    });
}



function P_DelDelInfoAdicionalIris(InfoId) {

    Swal.fire({
        title: '¿Está seguro?',
        text: "¿Desea eliminar esta información adicional?",
        type: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {

        if (result.value) {

            $.ajax({
                type: 'POST',
                url: AppRoutes.RegistroIrisP1.UrlDelInfoAdicionalIris,
                async: true,
                dataType: 'json',
                data: { InfoId: InfoId },
                success: function (result) {

                    if (result.success) {

                        F_GetInfoAdiconalIris($("#txtCriminalidadIdModal").val());

                        Swal.fire({
                            type: 'success',
                            title: 'Señor(a) Funcionario(a):',
                            text: result.message
                        });

                    } else {

                        Swal.fire({
                            type: 'error',
                            title: 'Señor(a) Funcionario(a):',
                            text: result.message
                        });

                    }
                },
                error: function () {
                    Swal.fire({
                        type: 'error',
                        title: 'Señor(a) Funcionario(a):',
                        text: "No es posible eliminar, revise."
                    });
                }
            });

        }
    });
}

function P_DelUbicacionIris(UbicacionId) {

    Swal.fire({
        title: '¿Está seguro?',
        text: "¿Desea eliminar esta ubicación?",
        type: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {

        if (result.value) {

            $.ajax({
                type: 'POST',
                url: AppRoutes.RegistroIrisP1.UrlDelUbiacionIris,
                async: true,
                dataType: 'json',
                data: { UbicacionId: UbicacionId },
                success: function (result) {

                    if (result.success) {

                        F_GetUbicacionIris($("#txtCriminalidadIdModal").val());

                        Swal.fire({
                            type: 'success',
                            title: 'Señor(a) Funcionario(a):',
                            text: result.message
                        });

                    } else {

                        Swal.fire({
                            type: 'error',
                            title: 'Señor(a) Funcionario(a):',
                            text: result.message
                        });

                    }
                },
                error: function () {
                    Swal.fire({
                        type: 'error',
                        title: 'Señor(a) Funcionario(a):',
                        text: "No es posible eliminar, revise."
                    });
                }
            });

        }
    });

}


function P_DelDocumentoIris(DocumentoId) {

    Swal.fire({
        title: '¿Está seguro?',
        text: "¿Desea eliminar este documento?",
        type: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {

        if (result.value) {

            $.ajax({
                type: 'POST',
                url: AppRoutes.RegistroIrisP1.UrlDelDocumentoIris,
                async: true,
                dataType: 'json',
                data: { DocumentoId: DocumentoId },
                success: function (result) {

                    if (result.success) {

                        F_GetDocumentosIris($("#txtCriminalidadIdModal").val());

                        Swal.fire({
                            type: 'success',
                            title: 'Señor(a) Funcionario(a):',
                            text: result.message
                        });

                    } else {

                        Swal.fire({
                            type: 'error',
                            title: 'Señor(a) Funcionario(a):',
                            text: result.message
                        });

                    }
                },
                error: function () {
                    Swal.fire({
                        type: 'error',
                        title: 'Señor(a) Funcionario(a):',
                        text: "No es posible eliminar, revise."
                    });
                }
            });

        }
    });
}
