
$(document).ready(function () {


    var fechaMinima = new Date();
    fechaMinima.setHours(0, 0, 0, 0); // Reinicia hora, minutos, segundos y milisegundos

    $(".CalendarioHora").kendoDateTimePicker({
        culture: "es-CO",
        format: "dd/MM/yyyy HH:mm",
        timeFormat: "HH:mm",
        interval: 15,
        // min: fechaMinima, // <-- Fecha mínima: hoy a las 00:00
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

    $('.select2').select2({
        placeholder: "Seleccione",
        allowClear: true
    });

    // Manejo genérico para cualquier modal secundaria
    $(document).on('hidden.bs.modal', '.modal', function () {
        // Verifica si todavía hay alguna modal abierta
        if ($('.modal.show').length > 0) {
            $('body').addClass('modal-open');
        }
    });


    $("#btnConsultarIntegrante").on("click", function (e) {
        e.preventDefault();

        F_GetIntegrantesPorId($("#txtIdentificacion").val());
    });

    $("#btnLimpiarInteg").on("click", function (e) {
        e.preventDefault();

        Limpiar();
    });


    
});

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

function F_GetIntegrantesPorId(V_Identificacion) {

    $.ajax({
        type: "GET",
        url: AppRoutes.BuscarIntegrantes.UrlGetintegrantesPorId,
        data: { V_Identificacion: V_Identificacion },
        dataType: 'json',
        cache: false,
        success: function (resp) {

            if (resp.success && Array.isArray(resp.data) && resp.data.length === 1) {

                let item = resp.data[0];
                $("#txtAlias").val(item.alias);
                $("#txtNombres").val(item.nombre);
                $("#txtApellidos").val(item.apellido);
                $("#txtObservacion").val(item.observacion);


                F_GetListaIris(V_Identificacion);
                F_GetLogPorIdentificacion(V_Identificacion);

            } else {

                Swal.fire({
                    icon: 'info',
                    title: 'Señor(a) Funcionario(a)',
                    text: "No se encontró información con la identificación suministrada."
                });

                $("#txtAlias").val("");
                $("#txtNombres").val("");
                $("#txtApellidos").val("");
                $("#txtObservacion").val("");
            }

        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: "No es posible consultar la información."
            });
        }
    });

}

function F_GetListaIris(V_Indentificacion) {

    $.ajax({
        url: AppRoutes.BuscarIntegrantes.UrlGetListaIris,
        type: "GET",
        data: { V_Identificacion: V_Indentificacion },
        success: function (respuesta) {
           
            console.log("✅Respuesta exitosa:", respuesta);

            let data = respuesta.data || [];
            GetGrillaListaIris(data)

        },
        error: function (err) {
            console.error("ERROR:", err);
            GetGrillaListaIris([]);
        }
    });


}

function GetGrillaListaIris(Datos) {
  
    $("#pn_GrillaListaIris").removeClass('hidden');

    renderDataTable("#tbGrillaListaIris", Datos, [

        Estados(),
        EstadosExistencia(),
        { title: "Codigo", data: "codigo" },
        { title: "Unidad", data: "dependencia" },
        { title: "Municipio", data: "municipio" },
        { title: "Zona", data: "zona" },
        { title: "Clase", data: "clase" },
        { title: "Fuente", data: "fuente" },
        { title: "Nombre", data: "nombreClase" },
        { title: "Fecha Inicio Existencia", data: "fechaInicioExistencia", render: formatDate },
        { title: "Cantidad Integrante", data: "cantidadIntegrantes" },
        columnaCaracteristicasGenerales(),
        { title: "Tipo Servicio", data: "tipoServicio" },
        { title: "Unidad Verificación Existencia", data: "unidadResponsable" },
        { title: "Fecha Asignación Verificación Existencia", data: "fechaVerificacionExistencia", render: formatDate },
        { title: "Fecha Respuesta Verificación Existencia", data: "fechaRespuestaVerificacion", render: formatDate },
      
        Contador1(),
        { title: "Unidad Proceso Investigativo", data: "unidadProcesoInvestigativo" },
        { title: "Fecha Asignación Proceso Investigativo", data: "fechaProcesoInvestigativo", render: formatDate },
        { title: "Fecha Respuesta Proceso Investigativo", data: "fechaRespuestaInvestigativo", render: formatDate },
        Contador2(),
       
        Resultados()


    ]);
}

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

function Estados() {
    return {
        title: "Estado",
        data: "estadoDescripcion",
        name: "estadoDescripcion",
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
        data: "estadoExistenciaDescripcion",
        name: "estadoExistenciaDescripcion",
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
        data: "caracteristicasGenerales",
        name: "caracteristicasGenerales",
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


function Contador1() {
    return {
        title: "Contador Verificación Existencia",
        data: "contadorVerificacionExistencia",
        name: "contadorVerificacionExistencia",

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
        title: "Contador Proceso Investigativo",
        data: "contadorProcesoInvestigativo",
        name: "contadorProcesoInvestigativo",

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
        data: "resultados",
        name: "resultados",
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

function Limpiar() {

    $("#txtIdentificacion").val("");
    $("#txtAlias").val("");
    $("#txtNombres").val("");
    $("#txtApellidos").val("");
    $("#txtObservacion").val("");
   

    
}



function F_GetLogPorIdentificacion(V_Identificacion) {

    $.ajax({
        url: AppRoutes.BuscarIntegrantes.UrlGetLogPorIdentificacion,
        type: "GET",
        data: { V_Identificacion: V_Identificacion },
        success: function (respuesta) {

            let data = respuesta.data || [];
            GetGrillaLog(data);
        },
        error: function () {
            Swal.fire("Error", "No es posible consultar la información.", "error");
        }
    });

}






function GetGrillaLog(Datos) {

    $("#pn_GrillaUbicaciones").removeClass('hidden');

    renderDataTable("#tbGrillaUbicaciones", Datos, [


        {
            data: null, className: "celdaCenter celda3", render: function (data) {

                var urlArcGis = `/Ubicacion/GraficarCoordenada?latitud=${data.latitud}&longitud=${data.longitud}`;
                var urlGoogle = `https://www.google.com/maps/place/${data.latitud},${data.longitud}`;

                var inicioBoton = `
                    <div class="dropdown dropend">
                        <button class="btn btn-success" type="button" id="dropdownMenuButton1"
                            data-bs-toggle="dropdown" aria-expanded="false">
                            <span class="fas fa-list"></span>
                        </button>
                        <ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">
                `;

                var VermapaArcGIS = `
                    <li style="padding-left: 17px;">
                        <a style="color: #102717;" href="javascript:OpenInsUbicacionModal('${data.latitud}', '${data.longitud}')">
                            <i class="fa fa-map green"></i>&nbsp;Ver mapa ArcGIS
                        </a>
                    </li>
                `;

                var VermapaGoogle = `
                    <li style="padding-left: 17px;">
                        <a style="color: #102717;"
                           href="${urlGoogle}"
                           target="_blank"
                           rel="noopener noreferrer">
                           <i class="fa fa-map red"></i>&nbsp;Ver mapa Google
                        </a>
                    </li>
                `;

                var finBoton = '</ul></div>';

                return inicioBoton + VermapaArcGIS + VermapaGoogle + finBoton;
            }
        },

        { title: "Tipo", data: "clase", class: "celdaCenter " },
        { title: "Nombres", data: "nombres", class: "celdaCenter " },
        { title: "Apellidos", data: "apellidos", class: "celdaCenter " },
        { title: "Identificación", data: "identificacion", class: "celdaCenter " },
        { title: "Latitud", data: "latitud", class: "celdaCenter " },
        { title: "Longitud", data: "longitud", class: "celdaCenter " },
        { title: "Fecha Consulta", data: "fecha_creacion_str" }


    ]);
}



function OpenInsUbicacionModal(latitud, longitud) {
    console.log("🗺️ Abriendo modal con coordenadas:", latitud, longitud);

    // Abre el modal
    $('#myModal').modal("show");

    // Evento al mostrarse completamente
    $('#myModal').off('shown.bs.modal').on('shown.bs.modal', function () {
        console.log("✅ Modal visible, inicializando mapa...");

        // Inicializa el mapa
        inicializarMapa('mapaDiv');

        // Espera un poco a que cargue ArcGIS y luego centra el muñeco verde
        setTimeout(() => {
            if (typeof window.ubicarLlamadaEnMapa === "function") {
                window.ubicarLlamadaEnMapa(latitud, longitud);
            }
        }, 800);
    });

    // 🧹 Evento al cerrar completamente el modal: destruir mapa
    $('#myModal').off('hidden.bs.modal').on('hidden.bs.modal', function () {
        console.log("🧹 Modal cerrado, destruyendo mapa...");

        // Si existe un mapa, destrúyelo correctamente
        if (typeof map !== "undefined" && map) {
            try {
                map.destroy();   // Libera memoria del objeto ArcGIS
                console.log("🧭 Mapa destruido correctamente");
            } catch (err) {
                console.warn("⚠️ Error al destruir mapa:", err);
            }
        }

        // Limpia el contenedor HTML del mapa (para crear uno nuevo luego)
        $("#mapaDiv").empty();

        // Elimina la variable global 'map' para asegurar una nueva instancia
        window.map = undefined;
    });
}


$("#btnAbrirMapaUbicaciones").on("click", function () {
    let table = $("#tbGrillaUbicaciones").DataTable();

    // 🔥 Obtener TODAS las filas cargadas (no solo las visibles)
    let datos = table.rows().data().toArray();

    // Extraer latitud y longitud de cada registro válido
    let ubicaciones = datos
        .filter(x => x.latitud && x.longitud)
        .map(x => ({
            latitud: parseFloat(x.latitud),
            longitud: parseFloat(x.longitud)
        }));

    if (ubicaciones.length === 0) {
        Swal.fire("Aviso", "No hay ubicaciones para mostrar.", "info");
        return;
    }

    console.log("🔵 Coordenadas recopiladas:", ubicaciones);

    // Abrir modal y enviar lista al mapa
    OpenModalTodasUbicaciones(ubicaciones);
});



function OpenModalTodasUbicaciones(listaCoordenadas) {

    // Abre el modal
    $("#myModal").modal("show");

    // Evento cuando el modal ya se visualizó
    $("#myModal").off('shown.bs.modal').on('shown.bs.modal', function () {

        // iniciar mapa
        inicializarMapa('mapaDiv');

        // esperar a que cargue ArcGIS correctamente
        setTimeout(() => {
            if (typeof window.pintarMultiplesUbicaciones === "function") {
                window.pintarMultiplesUbicaciones(listaCoordenadas);
            }
        }, 700);
    });

    // limpieza al cerrar modal
    $("#myModal").off('hidden.bs.modal').on('hidden.bs.modal', function () {
        if (typeof map !== "undefined") {
            try { map.destroy(); } catch { }
        }
        $("#mapaDiv").empty();
        window.map = undefined;
    });
}


// ------------------------
// EXPORTAR LISTA IRIS (Excel)
// ------------------------
$("#btnExcel").on("click", function () {
    let identificacion = $("#txtIdentificacion").val();

    window.location.href =
        "/Integrantes/BuscarInteg/ExportarExcelListaIris?V_Identificacion=" +
        encodeURIComponent(identificacion);
});


// ------------------------
// EXPORTAR LISTA IRIS (PDF)
// ------------------------
$("#btnPdf").on("click", function () {
    let identificacion = $("#txtIdentificacion").val();

    window.open(
        "/Integrantes/BuscarInteg/ExportarPdfListaIris?V_Identificacion=" +
        encodeURIComponent(identificacion),
        "_blank"
    );
});


// ------------------------
// EXPORTAR UBICACIONES (Excel)
// ------------------------
$("#btnExcel2").on("click", function () {
    let identificacion = $("#txtIdentificacion").val();

    window.location.href =
        "/Integrantes/BuscarInteg/ExportarExcelUbicaciones?V_Identificacion=" +
        encodeURIComponent(identificacion);
});

// ------------------------
// EXPORTAR UBICACIONES (PDF)
// ------------------------
$("#btnPdf2").on("click", function () {
    let identificacion = $("#txtIdentificacion").val();
    window.open(
        "/Integrantes/BuscarInteg/ExportarPdfUbicaciones?V_Identificacion=" +
        encodeURIComponent(identificacion),
        "_blank"
    );
});
