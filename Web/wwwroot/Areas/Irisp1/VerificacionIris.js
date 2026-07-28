$(document).ready(function () {

    if ($.fn.select2) {
        $('#ddlAnioIris').select2();
    }

    // Asocia el evento change
    $('#ddlAnioIris').on('change', function () {
        F_GetInfoGrillas();
    });

    F_GetInfoGrillas($('#ddlAnioIris').val());

    $('.select2').select2({
        placeholder: "Seleccione",
        allowClear: true
    });


    $(".CalendarioHora").kendoDateTimePicker({
        culture: "es-CO",
        format: "dd/MM/yyyy HH:mm",
        timeFormat: "HH:mm",
        interval: 15,
      
        animation: {
            close: {
                effects: "fadeOut zoom:out",
                duration: 300
            },
            open: {
                effects: "fadeIn zoom:in",
                duration: 300
            }
        }
    });
 

});


$(function () {
    $("#btnMapa").click(function () {
        $('#myModal').modal("show");
    });
});

$('#myModal2').on('shown.bs.modal', function () {
    inicializarMapa('mapaDiv2');
});

// Manejo genérico para cualquier modal secundaria
$(document).on('hidden.bs.modal', '.modal', function () {
    // Verifica si todavía hay alguna modal abierta
    if ($('.modal.show').length > 0) {
        $('body').addClass('modal-open');
    }
});


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
        url: AppRoutes.Verificacion.UrlGetInfoGrillas,
        dataType: 'json',
        data: { V_Anio: $('#ddlAnioIris').val() },
        success: function (response) {
           // console.log("✅ Respuesta exitosa:", response);
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

// renderDataTable ahora vive en /js/IniciarTabla.js (compartida por todas las grillas del sitio).

function GetGrillaVerificacion(Datos) {
    const datosFiltrados = Datos.filter(item => [2, 3, 4].includes(item.IdEstado));
    $("#pn_GrillaVerificacion").removeClass('hidden');

    renderDataTable("#tbGrilla", datosFiltrados, [
        columnaAcciones(datosFiltrados),
        Estados(),
        EstadosExistencia(),
        { title: "Codigo", data: "Codigo" },
        //{ title: "Unidad Verificación", data: "UnidadResponsable" },
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
        Resultados(),
        { title: "CriminalidadId", data: "CriminalidadId", visible: false }
    ], { preserveDraw: true });
}

function GetGrillaInvestigacion(Datos) {
    const datosFiltrados = Datos.filter(item => [72, 73].includes(item.IdEstado));
    $("#pn_GrillaInvestigacion").removeClass('hidden');

    renderDataTable("#tbGrillaInvestigacion", datosFiltrados, [
        columnaAcciones(datosFiltrados),
        Estados(),
        EstadosExistencia(),
        { title: "Codigo", data: "Codigo" },
       // { title: "Unidad Verificación", data: "UnidadResponsable" },
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
        Resultados(),
        { title: "CriminalidadId", data: "CriminalidadId", visible: false }
    ], { preserveDraw: true });
}

function GetGrillaFinalizacion(Datos) {
    const datosFiltrados = Datos.filter(item => [5].includes(item.IdEstado));
    $("#pn_GrillaFinalizacion").removeClass('hidden');

    renderDataTable("#tbGrillaFinalizacion", datosFiltrados, [
        columnaAcciones(datosFiltrados),
        Estados(),
        EstadosExistencia(),
        { title: "Codigo", data: "Codigo" },
       // { title: "Unidad Verificación", data: "UnidadResponsable" },
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
        Resultados(),
        { title: "CriminalidadId", data: "CriminalidadId", visible: false }
    ], { preserveDraw: true });
}

// Estados()/EstadosExistencia(): el mapeo de colores vive en /js/IniciarTabla.js (columnaEstadoGrilla),
// compartido por todos los módulos que muestran estas dos columnas.
function Estados() {
    return columnaEstadoGrilla("Estado", "EstadoDescripcion");
}

function EstadosExistencia() {
    return columnaEstadoGrilla("Estado Existencia", "EstadoExistenciaDescripcion");
}
function EstadoTareas() {
    return {
        title: "Estado Tareas",
        data: "EstadoTareasGrilla",
        name: "EstadoTareasGrilla",
        
        autoWidth: true,
        render: function (data, type, row) {
            // Si el estado viene vacío o nulo
            if (!data) {
                return `<span style="background-color: #808080; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px;">Por establecer</span>`;
            }

            const estado = data.toLowerCase();
            let color = '';

            if (estado.includes('aceptada')) {
                color = '#236305'; // verde
            } else {
                color = '#c53a1d'; // Rojo
            }

            return `<span style="background-color: ${color}; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px;">${data}</span>`;
        }
    };
}
function Resultados() {
    return {
        title: "Resultados",
        data: "EstadoResultados",
        name: "EstadoResultados",
        className: "celdaJust",
        autoWidth: true,
        render: function (data, type, row) {
            // Si el estado viene vacío o nulo
            if (!data) {
                return `<span style="background-color: #808080; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px;">Por establecer</span>`;
            }

            const estado = data.toLowerCase();
            let color = '';

            if (estado.includes('tiene resultados (siedco:')) {
                color = '#236305'; // verde
            } else {
                color = '#c53a1d'; // gris oscuro por defecto
            }

            return `<span style="background-color: ${color}; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px;">${data}</span>`;
        }
    };
}




// Acciones de la tabla.
// data: null es el patrón correcto para una columna que solo pinta botones vía render()
// (igual que el resto de grillas del archivo). Antes usaba "data: datosFiltrados" (el arreglo
// completo), lo cual es inválido como origen de datos de columna y podía hacer que la celda de
// acciones se renderizara de forma inconsistente.
function columnaAcciones(datosFiltrados) {
    return {
        data: null,
        autoWidth: true,
        render: function (data, type, row) {
            var DatosFila = JSON.stringify(row).replace(/"/g, '&quot;');

            var inicioBoton = `
                <div class="dropdown dropend">
                    <button class="btn btn-success" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                        <span class="fas fa-list"></span>
                    </button>
                    <ul class="dropdown-menu" style="line-height:23px;">`;

            var DetallesIris = `
                <li><a class="dropdown-item btn-detalle-iris" href="#" data-datos="${DatosFila}">
                    <i class="fas fa-list"></i> Detalles
                </a></li>`;

            var VerTareas = `
                <li><a class="dropdown-item" href="javascript:F_GetTareas('${row.CriminalidadId}','${row.IdResponsable}')">
                    <i class="fa fa-tasks"></i> Ver Tareas
                </a></li>`;

           

            var finBoton = `</ul></div>`;
            return inicioBoton + DetallesIris + VerTareas + finBoton;
        }
    }
}


// Delegación de eventos
$(document).on("click", ".btn-detalle-iris", function (e) {
    e.preventDefault();

    var datosAttr = $(this).attr("data-datos").replace(/&quot;/g, '"');

    try {
        var registro = JSON.parse(datosAttr); // ya parseado
        F_GetDetalleIris(registro);           // lo pasamos como objeto
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
function AbrirModalVisualizarTexto(Texto) {

    // Mostrar la modal
    $('#Modal_VisualizarTexto').modal("show");
    $('#txtDescripcion').val(Texto);
}
// Función detalle
function F_GetDetalleIris(registro) {   // 👈 ahora recibe directamente el objeto
    //console.log("✅ Registro recibido:", registro);

    // Ya no se hace JSON.parse de nuevo
    $("#txtCriminalidadIdModal").val(registro.CriminalidadId);
    $("#txtConsecutivoIris").val(registro.CriminalidadId);

    var FechaInicio = moment(registro.FechaInicioExistencia).format('DD/MM/YYYY');
    var FechaCreacion = moment(registro.FechaCreacion).format('DD/MM/YYYY hh:mm:ss a');

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
        url: AppRoutes.Verificacion.UrlGetFuncionarios,
        async: true,
        data: { V_Identificacion: IdenInforma },
        dataType: 'json',
        cache: false,
        success: function (respuesta) {

            if (respuesta.success) {

                $("#txtFuncionarioDetalle").text(respuesta.data.Funcionario);
                $("#txtUnidadDetalle").text(respuesta.data.Fisica + " - " + respuesta.data.Dependencia);
                $("#txtCorreo").text(respuesta.data.Correo);
                $("#txtCelularSiath").text(respuesta.data.Celular);

                $('#Modal_DetalleIris').modal("show");
               // F_GetRelacionIris();
                F_GetIntegrantesIris(registro.CriminalidadId);
                F_GetUbicacionIris(registro.CriminalidadId);
                F_GetDelitosIris(registro.CriminalidadId);
                F_GetInfoAdiconalIris(registro.CriminalidadId);
                //F_GetResponsableIris();
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
        url: AppRoutes.Verificacion.UrlGetInfoGrillas
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

 
}
function F_GetIntegrantesIris(CriminalidadId) {


    $.ajax({
        type: 'GET',
        url: AppRoutes.Verificacion.UrlGetIntegrantes,
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
function F_GetUbicacionIris(CrininalidadId) {


    $.ajax({
        type: 'GET',
        url: AppRoutes.Verificacion.UrlGetUbicacion,
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


function F_GetDelitosIris(CriminalidadId) {
    $.ajax({
        type: 'GET',
        url: AppRoutes.Verificacion.UrlGetDelitosIris, // URL del endpoint que devuelve los datos
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
        url: AppRoutes.Verificacion.UrlGetInfoAdicional, // URL del endpoint que devuelve los datos
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
function F_GetResponsableIris() {
    $.ajax({
        type: 'GET',
        url: UrlGetInfoGrillas, // URL del endpoint que devuelve los datos
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

    
}
function F_GetDocumentosIris(CriminalidadId) {
    $.ajax({
        type: 'GET',
        url: AppRoutes.Verificacion.UrlGetDocIris, // URL del endpoint que devuelve los datos
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
                   
                    return `<a href="Irisp1/Verificacion/descargar?ruta=${encodeURIComponent(data)}" target="_blank" style="background-color: #236305; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px; text-decoration: none;">Descargar</a>`;
                }
            },
            { "title": "Fecha Creación", "data": "fechaCreacion", "name": "fechaCreacion", render: formatDate , className: "celdaCenter celda17"  }
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
        url: AppRoutes.Verificacion.UrlGetFotosIris, // Endpoint del backend
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

function OpenInsRespuestaTareaModal(TareaID) {
    // limpiar campos antes de abrir
    $("#txtTareaId").val(TareaID);
    $("#ddlTipoExiste").val(null).trigger("change");
    $("#txtFechaVerificacion").val("");
    $("#txtInfoAdicionalModal").val("");

    $('#Modal_InsRtaTarea').modal("show");
}


function OpenInsResultadosModal() {

    $('#Modal_AgregarResultado').modal("show");
}


function OpenInsUbicacionModal() {
    $('#myModal2').modal("show");

    $('#myModal2').on('shown.bs.modal', function () {
        if (typeof map !== "undefined") {
            map.resize();
            map.reposition();
        }
    });
}




function OpenInsDelitosModal() {

    $('#Modal_InsDelitos').modal("show");
}

function OpenInsInfoadiconalModal() {

    $('#Modal_InsInfoAdicional').modal("show");
}



function InsIntegrantesModal() {
    // Obtener valores de los campos y limpiar espacios
    const identificacion = $("#txtIdentificacionIntegModal").val().trim();
    const apellidos = $("#txtApellidosIntegModal").val().trim();
    const nombres = $("#txtNombreIntegModal").val().trim();
    const celular = $("#txtCelularIntegModal").val().trim();
    const direccion = $("#txtDirecciónIntegModal").val().trim();
    const alias = $("#txtAliasModal").val().trim();

    // Validar campos obligatorios
    if (!identificacion || !alias) {
        Swal.fire({
            icon: 'warning',
            title: 'Campos obligatorios',
            text: 'Por favor complete todos los campos antes de guardar.'
        });
        return; // Detener la ejecución si faltan campos
    }

    $.ajax({
        url: AppRoutes.Verificacion.UrlGetConsecutivoIntegrante,
        type: 'POST',
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                $("#txtConsecutivoIntegrante").val(response.data);

                const Obj_Integrante = {
                    INTEGRANTE_ID: response.data,
                    CRIMINALIDAD_ID: $("#txtCriminalidadIdModal").val(),
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
                    CELULAR: parseInt(celular),
                    DIRECCION: direccion
                };

                $.ajax({
                    url: AppRoutes.Verificacion.UrlInsIntegrantes,
                    type: 'POST',
                    data: Obj_Integrante,
                    success: function (resp) {
                        if (resp.success) {
                            F_GetIntegrantesIris($("#txtCriminalidadIdModal").val());
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

            } else {
                $("#txtConsecutivoIntegrante").val('');
                Swal.fire('Error', "No se pudo obtener el consecutivo.", 'info');
            }
        },
        error: function () {
            $("#txtConsecutivoIntegrante").val('');
            Swal.fire('Error', 'Error de comunicación con el servidor.', 'error');
        }
    });
}


function obtenerDelitosSecundariosSeleccionadosModal() {
    const delitos = [];
    $('#ddlDelitoSecundarioModal option:selected').each(function () {
        delitos.push($(this).val());
    });
    console.log("Delitos secundarios seleccionados: ", delitos);
    return delitos;
}
function P_InsDelitosModal() {

    var Obj_DelitosSecundarios = obtenerDelitosSecundariosSeleccionadosModal();

    const Obj_DelitosIris = {
        CriminalidadId: $("#txtCriminalidadIdModal").val(),
        IdDelitoPrincipal: $("#ddlDelitoPrincipalModal").val(),
        IdDelitoSecundario: Obj_DelitosSecundarios
    };

    // -------------------------------
    // VALIDACIONES
    // -------------------------------

    // Validar CriminalidadId
    if (!Obj_DelitosIris.CriminalidadId || Obj_DelitosIris.CriminalidadId.trim() === "") {
        Swal.fire({
            icon: 'warning',
            title: 'Campos obligatorios',
            text: 'El registro no tiene Criminalidad ID. Refresque e intente nuevamente.'
        });
        return;
    }

    // Validar que exista al menos UN delito: principal o secundario
    const noHayPrincipal = (!Obj_DelitosIris.IdDelitoPrincipal || Obj_DelitosIris.IdDelitoPrincipal === "");
    const noHaySecundarios = (Obj_DelitosIris.IdDelitoSecundario.length === 0);

    if (noHayPrincipal && noHaySecundarios) {
        Swal.fire({
            icon: 'warning',
            title: 'Campos obligatorios',
            text: 'Debe seleccionar al menos un Delito Principal o un Delito Secundario.'
        });
        return;
    }

    // -------------------------------
    // SI TODO ES VÁLIDO -> Enviar al servidor
    // -------------------------------
    $.ajax({
        url: AppRoutes.Verificacion.UrlInsDelitos,
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


function InsRespuestaTareaModal() {


    const Obj_RespuestaTarea = {

        EstadoExistencia: $("#ddlTipoExiste").val(),
        Justificacion: $("#txtInfoJustificacionModal").val(),
        CriminalidadId: $("#txtCriminalidadIdModal").val(),
        TareaId: $("#txtTareaId").val(),
        FechaVerifica: $("#txtFechaVerificaTarea").val(),

    }


    // Validar campos obligatorios
    if (!Obj_RespuestaTarea.CriminalidadId || !Obj_RespuestaTarea.EstadoExistencia || !Obj_RespuestaTarea.Justificacion || !Obj_RespuestaTarea.FechaVerifica || !Obj_RespuestaTarea.TareaId) {
        Swal.fire({
            icon: 'warning',
            title: 'Campos obligatorios',
            text: 'Por favor complete todos los campos antes de guardar.'
        });
        return; // Detener la ejecución si faltan campos
    }

    $.ajax({
        url: AppRoutes.Verificacion.UrlInsRespuestaTarea,
        type: 'POST',
        data: Obj_RespuestaTarea,
        success: function (resp) {
            if (resp.success) {

               // F_GetDelitosIris($("#txtCriminalidadIdModal").val());
                $('#Modal_InsRtaTarea').modal('hide');
                limpiarFormularioRespuestaTarea();

                Swal.fire({
                    type: 'success',
                    title: 'Señor(a) Funcionario(a:)',
                    text: resp.message
                });


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

function limpiarFormularioRespuestaTarea() {
    $("#ddlTipoExiste").val('').trigger('change');
    $("#txtInfoJustificacionModal").val('');
    $("#txtFechaVerificaTarea").val('');

  
}

function P_InsResultadoTareaModal() {



    const Obj_Resultado = {
        IdTipo: $("#ddlTipoResultado").val(),
        Numero: $("#txtNumeroResultado").val(),
        Fecha: $("#txtFechaResultado").val(),
        Observacion: $("#txtObservacionesResultado").val().trim(),
        CriminalidadId: $("#txtCriminalidadIdModal").val()
    };


    // Validar campos obligatorios
    if (!Obj_Resultado.IdTipo || !Obj_Resultado.Numero || !Obj_Resultado.Fecha || !Obj_Resultado.Observacion || !Obj_Resultado.CriminalidadId) {
        Swal.fire({
            icon: 'warning',
            title: 'Campos obligatorios',
            text: 'Por favor complete todos los campos antes de guardar.'
        });
        return; // Detener la ejecución si faltan campos
    }

    $.ajax({
        url: AppRoutes.Verificacion.UrlInsResultadoTareas, // Ojo: endpoint correcto
        type: 'POST',
        data: Obj_Resultado,
        success: function (resp) {
            if (resp.success) {
                F_GetResultados($("#txtCriminalidadIdModal").val(), $("#txtResponsableIdModal").val() );
                $('#Modal_AgregarResultado').modal('hide');
                limpiarFormularioResultado();

                Swal.fire({
                    type: 'success',
                    title: 'Señor(a) Funcionario(a:)',
                    text: resp.message
                });

            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Señor(a) Funcionario(a)',
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
        url: AppRoutes.Verificacion.UrlInsInfoAdicional,
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

    var CriminalidadId = $("#txtCriminalidadIdModal").val();

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



function subirDocumentoSeleccionado(input) {

    if (!input.files || input.files.length === 0) {
        Swal.fire('Error', 'Debe seleccionar un documento válido.', 'error');
        return;
    }

    let file = input.files[0];
    let idCriminalidad = $("#txtCriminalidadIdModal").val();

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
                    url: AppRoutes.Verificacion.UrlDelIris,
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
        text: "Esta acción eliminará el integrante seleccionado.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {

        if (result.value) {

            $.ajax({
                type: 'POST',
                url: AppRoutes.Verificacion.UrlDelIntegrante,
                async: true,
                dataType: 'json',
                data: { IntegranteId: IntegranteId },

                success: function (result) {
                    if (result.success) {
                        var ID = $("#txtCriminalidadIdModal").val();
                        F_GetIntegrantesIris(ID);

                        Swal.fire({
                            icon: 'success',
                            title: 'Señor(a) Funcionario(a)',
                            text: result.message
                        });

                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Señor(a) Funcionario(a)',
                            text: result.message
                        });
                    }
                },

                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'Señor(a) Funcionario(a)',
                        text: "No es posible eliminar el integrante. Por favor revise."
                    });
                }
            });
        }

    });
}

function P_DelDelitosIris(DelitoId) {

    Swal.fire({
        title: '¿Está seguro?',
        text: "Esta acción eliminará el delito registrado.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {

        if (result.value) {

            $.ajax({
                type: 'POST',
                url: AppRoutes.Verificacion.UrlDelDelitos,
                async: true,
                dataType: 'json',
                data: { DelitoId: DelitoId },

                success: function (result) {
                    if (result.success) {

                        F_GetDelitosIris($("#txtCriminalidadIdModal").val());

                        Swal.fire({
                            icon: 'success',
                            title: 'Señor(a) Funcionario(a)',
                            text: result.message
                        });

                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Señor(a) Funcionario(a)',
                            text: result.message
                        });
                    }
                },

                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'Señor(a) Funcionario(a)',
                        text: "No es posible eliminar el delito. Por favor revise."
                    });
                }
            });
        }

    });
}

function P_DelDelInfoAdicionalIris(InfoId) {

    Swal.fire({
        title: '¿Está seguro?',
        text: "Esta acción eliminará la información adicional.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {

        if (result.value) {

            $.ajax({
                type: 'POST',
                url: AppRoutes.Verificacion.UrlDelInfoAdicionalIris,
                async: true,
                dataType: 'json',
                data: { InfoId: InfoId },

                success: function (result) {
                    if (result.success) {

                        F_GetInfoAdiconalIris($("#txtCriminalidadIdModal").val());

                        Swal.fire({
                            icon: 'success',
                            title: 'Señor(a) Funcionario(a)',
                            text: result.message
                        });

                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Señor(a) Funcionario(a)',
                            text: result.message
                        });
                    }
                },

                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'Señor(a) Funcionario(a)',
                        text: "No es posible eliminar la información adicional. Por favor revise."
                    });
                }
            });
        }

    });
}

function P_DelDocumentoIris(DocumentoId) {

    Swal.fire({
        title: '¿Está seguro?',
        text: "Esta acción eliminará el documento seleccionado.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {

        if (result.value) {
            // Usuario confirmó → Ejecutar AJAX
            $.ajax({
                type: 'POST',
                url: AppRoutes.Verificacion.UrlDelDocumentoIris,
                async: true,
                dataType: 'json',
                data: { DocumentoId: DocumentoId },

                success: function (result) {
                    if (result.success) {

                        // Recargar lista de documentos
                        F_GetDocumentosIris($("#txtCriminalidadIdModal").val());

                        Swal.fire({
                            icon: 'success',
                            title: 'Señor(a) Funcionario(a)',
                            text: result.message
                        });

                    } else {

                        Swal.fire({
                            icon: 'error',
                            title: 'Señor(a) Funcionario(a)',
                            text: result.message
                        });
                    }
                },

                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'Señor(a) Funcionario(a)',
                        text: "No es posible eliminar el documento, por favor revise."
                    });
                }
            });
        }

    });
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


function limpiarFormularioResultado() {
    $("#ddlTipoResultado").val('').trigger('change');
   
    $("#txtNumeroResultado").val('');
    $("#txtFechaResultado").val('');
    $("#txtObservacionesResultado").val('');

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
        CodigoSiedcoCuadrante: $("#txtCodSiedcoCuadrante").val()
    };

    // ---------------------------
    // VALIDACIONES
    // ---------------------------

    // Validar CriminalidadId
    if (!Obj_Ubicacion.CriminalidadId || Obj_Ubicacion.CriminalidadId.trim() === "") {
        Swal.fire({
            icon: 'warning',
            title: 'Campos obligatorios',
            text: 'No fue posible encontrar el identificador del caso. Cierre la ventana y vuelva a abrir el registro.'
        });
        return;
    }

    // Validar Latitud y Longitud
    if (!Obj_Ubicacion.Latitud || !Obj_Ubicacion.Longitud) {
        Swal.fire({
            icon: 'warning',
            title: 'Campos obligatorios',
            text: 'Debe seleccionar la ubicación del caso en el mapa (Latitud y Longitud).'
        });
        return;
    }





    // ---------------------------
    // SI PASA LAS VALIDACIONES → ENVIAR AL SERVIDOR
    // ---------------------------

    $.ajax({
        url: AppRoutes.Verificacion.UrlInsUbicacion,
        type: 'POST',
        data: Obj_Ubicacion,
        success: function (resp) {

            if (resp.success) {

                F_GetUbicacionIris($("#txtCriminalidadIdModal").val());
                $('#myModal2').modal('hide');

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

function F_GetTareas(IdCriminalidad, IdResponsable) {

    $("#txtResponsableIdModal").val(IdResponsable);
    $("#txtCriminalidadIdModal").val(IdCriminalidad);

    $.ajax({
        type: "GET",
        url: AppRoutes.Verificacion.UrlGetTareasIris,
        data: { V_Criminalidad: IdCriminalidad }, // ✔ CORRECTO
        dataType: 'json',
        cache: false,

        success: function (respuesta) {

            const tieneDatos = respuesta?.success === true &&
                Array.isArray(respuesta.data) &&
                respuesta.data.length > 0;

            if (tieneDatos) {

                $('#Modal_TareasIris').modal("show");

                GetGrillaTareasIris(respuesta.data);

                F_GetResultados(IdCriminalidad, IdResponsable);

                // Obtener responsables
                $.ajax({
                    type: "GET",
                    url: AppRoutes.Verificacion.UrlGetResponsablesTareasIris,
                    data: { V_Criminalidad: IdCriminalidad }, // ✔ este SÍ recibe criminalidad
                    dataType: 'json',
                    cache: false,

                    success: function (resp) {
                        if (resp?.success === true && Array.isArray(resp.data)) {
                            GetGrillaResponsablesTareas(resp.data);
                        }
                    }
                });

            } else {
                Swal.fire({
                    icon: 'info',
                    title: 'Señor(a) Funcionario(a)',
                    text: "No hay tareas asignadas."
                });
            }
        },

        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Señor(a) Funcionario(a)',
                text: 'No es posible consultar las tareas. Por favor, revise la conexión o contacte al administrador.'
            });
        }
    });
}



function F_GetResultados(IdCriminalidad) {//, IdResponsable) {
    $.ajax({
        type: "GET",
        url: AppRoutes.Verificacion.UrlGetResultadosIris,
        data: { V_Criminalidad: IdCriminalidad },//, V_ResponsableId: IdResponsable },
        dataType: 'json',
        cache: false,
        success: function (respuesta) {
            if (respuesta?.success && Array.isArray(respuesta.data) && respuesta.data.length > 0) {
                // Mostrar el panel de resultados
                $('#pn_GrillaResultados').removeClass('hidden');
                GetGrillaResultados(respuesta.data);
            } else {

                $('#pn_GrillaResultados').removeClass('hidden');
                // Ocultar la grilla si está visible
                //$('#pn_GrillaResultados').addClass('hidden');

                //Swal.fire({
                //    icon: 'info',
                //    title: 'Señor(a) Funcionario(a)',
                //    text: "No hay resultados asociados a esta criminalidad."
                //});
            }

        },
        error: function (xhr, status, error) {
            console.error("Error en F_GetTareas:", {
                status: xhr.status,
                response: xhr.responseText,
                error: error
            });

            Swal.fire({
                icon: 'error',
                title: 'Señor(a) Funcionario(a)',
                text: 'No es posible consultar resultados. Por favor, revise la conexión o contacte al administrador.'
            });
        }
    });
}
function GetGrillaTareasIris(Datos){
    if ($.fn.dataTable.isDataTable("#tbGrillaRelacionTareas")) {
        $("#tbGrillaRelacionTareas").DataTable().destroy();
    }

    $("#tbGrillaRelacionTareas").empty();
    $("#pn_GrillaRelacionTareas").removeClass('hidden');

    $("#tbGrillaRelacionTareas").DataTable({
        destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        responsive: true,
        "columns": [
            {
                data: null, className: "celdaCenter celda3", "render": function (data, type, row) {
                    var inicioBoton = '<div class="dropdown dropend"><button class="btn btn-success" type="button" id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false"><span class="fas fa-list"></span></button><ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';
                    var AnexarDoc = `<li style="padding-left: 17px;"><a style="color: #102717;" href="javascript:subirDocumentoTareas('${row.TareaId}')"><i class="fa fa-file"></i>  &nbsp;Anexar Documento </a></li>`;
                    var Responder = `
                <li><a class="dropdown-item" href="javascript:OpenInsRespuestaTareaModal('${row.TareaId}')">
                    <i class="fa fa-reply"></i> Responder Tarea
                </a></li>`;
                    var finBoton = '</ul></div>';
                    return inicioBoton + AnexarDoc + Responder+ finBoton;
                }
            },
           
            EstadoTareas(),
            columnaObservacion(),
            
            {
                title: "Fecha respuesta",
                data: "FechaVerifica",
                name: "FechaVerifica",
                className: "celdaCenter celda12",
                render: function (data) {
                    if (!data) return "";
                    const fecha = moment(data).format('DD/MM/YYYY');
                    const hora = moment(data).format('hh:mm:ss a');
                    return `${fecha} - ${hora}`;

                }
            },

            columnaJustificacion(),
          
            EstadoEvidencias(),
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

function subirDocumentoTareas(tareaId) {
    var fileInput = document.createElement("input");
    fileInput.type = "file";
    fileInput.accept = ".pdf,.jpg,.png,.doc,.docx";
    fileInput.onchange = function (e) {
        var file = e.target.files[0];
        if (!file) return;

      //  var idCriminalidad = $("#txtCriminalidadIdModal").val();

        var formData = new FormData();
        formData.append('documento', file);
     //   formData.append('idCriminalidad', idCriminalidad);
        formData.append('tareaId', tareaId);  // 🔹 ahora se envía el id de la tarea

        $.ajax({
            url: 'Irisp1/Verificacion/GuardarDocumentoTareaConRegistro',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    Swal.fire('Éxito', response.message, 'success');
                  
                    F_GetTareas($("#txtCriminalidadIdModal").val(), $("#txtResponsableIdModal").val())
                } else {
                    Swal.fire('Error', response.message, 'error');
                }
            },
            error: function () {
                Swal.fire('Error', 'Error al subir documento', 'error');
            }
        });
    };
    fileInput.click();
}

function GetGrillaResultados(Datos) {
    if ($.fn.dataTable.isDataTable("#tbGrillaResultados")) {
        $("#tbGrillaResultados").DataTable().destroy();
    }

    $("#tbGrillaResultados").empty();
    $("#pn_GrillaResultados").removeClass('hidden');

    $("#tbGrillaResultados").DataTable({
        destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        responsive: true,
        "columns": [
           

            { "title": "Tipo", "data": "DescTipoResultado", "name": "DescTipoResultado", className: "celdaCenter celda2" },
            { "title": "Número", "data": "NroSpoaSiedco", "name": "NroSpoaSiedco", className: "celdaCenter celda3" },
            {
                title: "Fecha SIEDCO - SPOA",
                data: "FechaResultado",
                name: "FechaResultado",
                className: "celdaCenter celda5",
                render: function (data) {
                    if (!data) return "";
                    const fecha = moment(data).format('DD/MM/YYYY');
                    const hora = moment(data).format('hh:mm:ss a');
                    return `${fecha} - ${hora}`;

                }
            },
            columnaObservacionResultado(),
            {
                title: "Fecha Creación",
                data: "FechaCreaResultado",
                name: "FechaCreaResultado",
                className: "celdaCenter celda5",
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
function columnaObservacion() {
    return {
        title: "Observación",
        data: "Observacion",
        name: "Observacion",
        "autoWidth": false,
        className: "celdaCenter celda7",
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
function columnaObservacionResultado() {
    return {
        title: "Observación",
        data: "ObservacionResultado",
        name: "ObservacionResultado",
        "autoWidth": false,
        className: "celdaCenter celda7",
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
function columnaJustificacion() {
    return {
        title: "Justificación",
        data: "Justificacion",
        name: "Justificacion",
        "autoWidth": false,
        className: "celdaCenter celda7" ,
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
function EstadoEvidencias() {
    return {
        title: "Evidencia",
        data: "Evidencia",
        name: "Evidencia",
        autoWidth: false,
        className: "celdaJust",
        render: function (data, type, row) {
            // Si el estado viene vacío o nulo
            if (!data) {
                return `<span style="background-color: #808080; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px;">No Tiene</span>`;
            }

            const estado = data.toLowerCase();

            // Si no hay evidencia
            if (estado === 'no tiene') {
                return `<span style="background-color: #c53a1d; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px;">${data}</span>`;
            }

            // Si existe una URL válida, crear enlace de descarga
            /* return `<a href="${data}" download style="background-color: #236305; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px; text-decoration: none;">Descargar</a>`;*/
            return `<a href="/Irisp1/Verificacion/descargarTarea?ruta=${encodeURIComponent(data)}" target="_blank" style="background-color: #236305; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px; text-decoration: none;">Descargar</a>`;


        }
    };
}
function GetGrillaResponsablesTareas(Datos) {
    if ($.fn.dataTable.isDataTable("#tbGrillaResponsablesTareas")) {
        $("#tbGrillaResponsablesTareas").DataTable().destroy();
    }

    $("#tbGrillaResponsablesTareas").empty();
    $("#pn_GrillaResponsablesTareas").removeClass('hidden');

    $("#tbGrillaResponsablesTareas").DataTable({
        destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        responsive: true,
        columns: [
            { title: "Unidad", data: "UnidadCompleta", className: "celdaJust" },
            

             {
                data: "Seguimiento",
                title: "Seguimiento Tareas",
                className: "text-start align-top",
                render: function (data, type, row) {
                    if (type === 'display' && data) {
                        return decodeHtmlEntities(data);
                    }
                    return data;
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


function decodeHtmlEntities(text) {
    if (!text) return "";
    var textarea = document.createElement("textarea");
    textarea.innerHTML = text;
    return textarea.value;
}
