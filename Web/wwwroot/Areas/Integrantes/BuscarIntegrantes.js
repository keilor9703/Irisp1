
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

// renderDataTable ahora vive en /js/IniciarTabla.js (compartida por todas las grillas del sitio).

// Muestra/oculta el distintivo de "reincidente" (watchlist) junto al buscador. Correlaciona
// IRISP_REINCIDENTE con la persona consultada, cruce que antes no existía en el sistema.
function MostrarBadgeReincidente(esReincidente, tipoReincidencia) {
    var $badge = $("#badgeReincidente");
    if ($badge.length === 0) return;

    if (Number(esReincidente) === 1) {
        var txt = "REINCIDENTE" + (tipoReincidencia ? " · " + tipoReincidencia : "");
        $badge.text(txt).removeClass("d-none");
    } else {
        $badge.addClass("d-none").text("");
    }
}

function F_GetIntegrantesPorId(V_Identificacion) {

    $.ajax({
        type: "GET",
        url: AppRoutes.BuscarIntegrantes.UrlGetintegrantesPorId,
        data: { V_Identificacion: V_Identificacion },
        dataType: 'json',
        cache: false,
        success: function (resp) {

            // Una persona puede ser integrante de VARIOS IRISP1, por lo que la consulta puede
            // devolver más de una fila. Antes se exigía "length === 1" y cualquier cédula con 2+
            // apariciones caía al else mostrando el falso "No se encontró información". Se usa
            // "length > 0" y se toma la primera fila como datos consolidados de la persona.
            if (resp.success && Array.isArray(resp.data) && resp.data.length > 0) {

                let item = resp.data[0];
                $("#txtAlias").val(item.alias);
                $("#txtNombres").val(item.nombre);
                $("#txtApellidos").val(item.apellido);
                $("#txtObservacion").val(item.observacion);

                // Correlación con la lista de vigilancia (watchlist): resalta si el sujeto
                // consultado está registrado como reincidente.
                MostrarBadgeReincidente(item.esreincidente, item.tiporeincidencia);

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
                MostrarBadgeReincidente(0, null);
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
           
            //console.log("✅Respuesta exitosa:", respuesta);

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

// Estados()/EstadosExistencia(): el mapeo de colores vive en /js/IniciarTabla.js (columnaEstadoGrilla),
// compartido por todos los módulos que muestran estas dos columnas.
// Nota: este endpoint devuelve los campos en camelCase ("estadoDescripcion"), a diferencia de
// Irisp1/Seguimiento/Verificación que usan PascalCase ("EstadoDescripcion") — se respeta tal cual
// viene del backend, no se cambia aquí para no romper el binding con la respuesta real.
function Estados() {
    return columnaEstadoGrilla("Estado", "estadoDescripcion");
}

function EstadosExistencia() {
    return columnaEstadoGrilla("Estado Existencia", "estadoExistenciaDescripcion");
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
    MostrarBadgeReincidente(0, null);
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

                var urlArcGis = `Ubicacion/GraficarCoordenada?latitud=${data.latitud}&longitud=${data.longitud}`;
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
   // console.log("🗺️ Abriendo modal con coordenadas:", latitud, longitud);

    // Abre el modal
    $('#myModal').modal("show");

    // Evento al mostrarse completamente
    $('#myModal').off('shown.bs.modal').on('shown.bs.modal', function () {
        //console.log("✅ Modal visible, inicializando mapa...");

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
        //console.log("🧹 Modal cerrado, destruyendo mapa...");

        // Si existe un mapa, destrúyelo correctamente
        if (typeof map !== "undefined" && map) {
            try {
                map.destroy();   // Libera memoria del objeto ArcGIS
               // console.log("🧭 Mapa destruido correctamente");
            } catch (err) {
                //console.warn("⚠️ Error al destruir mapa:", err);
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

   // console.log("🔵 Coordenadas recopiladas:", ubicaciones);

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
        "Integrantes/BuscarInteg/ExportarExcelListaIris?V_Identificacion=" +
        encodeURIComponent(identificacion);
});


// ------------------------
// EXPORTAR LISTA IRIS (PDF)
// ------------------------
$("#btnPdf").on("click", function () {
    let identificacion = $("#txtIdentificacion").val();

    window.open(
        "Integrantes/BuscarInteg/ExportarPdfListaIris?V_Identificacion=" +
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
        "Integrantes/BuscarInteg/ExportarExcelUbicaciones?V_Identificacion=" +
        encodeURIComponent(identificacion);
});

// ------------------------
// EXPORTAR UBICACIONES (PDF)
// ------------------------
$("#btnPdf2").on("click", function () {
    let identificacion = $("#txtIdentificacion").val();
    window.open(
        "Integrantes/BuscarInteg/ExportarPdfUbicaciones?V_Identificacion=" +
        encodeURIComponent(identificacion),
        "_blank"
    );
});
