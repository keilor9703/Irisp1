
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

    _casosData = Datos || [];
    TryRenderAnalitica();

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

// ============================================================================================
//  PANEL DE ANÁLISIS COMPORTAMENTAL Y CRIMINAL
//  Se calcula 100% en el cliente a partir de los dos datasets que ya trae la pantalla:
//    - _logData   : consultas de antecedentes por PDA (fecha, hora, lat/long, tipo) -> comportamiento
//    - _casosData : casos IRISP1 donde el sujeto es integrante (municipio, clase, existencia) -> criminal
//  El objetivo es pasar de "información plana" a inteligencia: por dónde se desplaza, cuándo opera,
//  patrones horarios/semanales y perfil delictivo — insumo para anticipar movimientos e intervención.
// ============================================================================================
var _casosData = null;
var _logData = null;
var _analytCharts = {};

var DIAS_SEMANA = ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"];
var MESES_CORTO = ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"];
var PALETA = ["#08a6cb", "#0d6efd", "#236305", "#c53a1d", "#f18f08", "#6f42c1", "#20c997", "#e83e8c", "#795548", "#607d8b"];

function crearGrafico(idCanvas, config) {
    var el = document.getElementById(idCanvas);
    if (!el) return;
    if (_analytCharts[idCanvas]) { _analytCharts[idCanvas].destroy(); }
    _analytCharts[idCanvas] = new Chart(el.getContext("2d"), config);
}

// Cuenta ocurrencias de una clave y devuelve {labels, valores} ordenado desc, top N opcional.
function contarPor(lista, fnClave, topN) {
    var mapa = {};
    (lista || []).forEach(function (x) {
        var k = fnClave(x);
        if (k === null || k === undefined || k === "") k = "Sin dato";
        mapa[k] = (mapa[k] || 0) + 1;
    });
    var arr = Object.keys(mapa).map(function (k) { return { k: k, v: mapa[k] }; });
    arr.sort(function (a, b) { return b.v - a.v; });
    if (topN) arr = arr.slice(0, topN);
    return { labels: arr.map(function (o) { return o.k; }), valores: arr.map(function (o) { return o.v; }) };
}

function parseFechaLog(item) {
    // El log expone fecha_creacion (ISO) y fecha_creacion_str (dd/MM/yyyy HH:mm)
    var f = item.fecha_creacion ? new Date(item.fecha_creacion) : null;
    if (f && !isNaN(f)) return f;
    return null;
}

function TryRenderAnalitica() {
    // Se renderiza cuando ya llegó al menos uno de los dos datasets.
    if (_casosData === null && _logData === null) return;
    $("#pn_Analitica").removeClass("d-none");

    RenderKpis();
    RenderComportamiento();
    RenderCriminal();
}

function RenderKpis() {
    var log = _logData || [];
    var casos = _casosData || [];

    // Lugares distintos: coordenadas redondeadas a 3 decimales (~110 m) para agrupar consultas cercanas.
    var lugares = {};
    var fechas = [];
    log.forEach(function (x) {
        if (x.latitud && x.longitud) {
            lugares[Number(x.latitud).toFixed(3) + "," + Number(x.longitud).toFixed(3)] = true;
        }
        var f = parseFechaLog(x);
        if (f) fechas.push(f);
    });

    var rango = "-";
    if (fechas.length > 0) {
        fechas.sort(function (a, b) { return a - b; });
        rango = fmtCorta(fechas[0]) + " → " + fmtCorta(fechas[fechas.length - 1]);
    }

    var conResultado = casos.filter(function (c) {
        return (c.resultados || "").toLowerCase().indexOf("tiene resultados (") >= 0;
    }).length;

    $("#kpiConsultas").text(log.length);
    $("#kpiLugares").text(Object.keys(lugares).length);
    $("#kpiRango").text(rango);
    $("#kpiCasos").text(casos.length);
    $("#kpiConResultado").text(conResultado);
}

function fmtCorta(d) {
    return ("0" + d.getDate()).slice(-2) + "/" + MESES_CORTO[d.getMonth()] + "/" + d.getFullYear();
}

// ---- COMPORTAMIENTO (a partir de las consultas de antecedentes por PDA) ----
function RenderComportamiento() {
    var log = _logData || [];

    // Actividad por mes (línea temporal)
    var porMes = {};
    log.forEach(function (x) {
        var f = parseFechaLog(x);
        if (!f) return;
        var k = f.getFullYear() + "-" + ("0" + (f.getMonth() + 1)).slice(-2);
        porMes[k] = (porMes[k] || 0) + 1;
    });
    var mesesOrden = Object.keys(porMes).sort();
    crearGrafico("graficoActividadMes", {
        type: "line",
        data: {
            labels: mesesOrden.map(function (k) { var p = k.split("-"); return MESES_CORTO[parseInt(p[1], 10) - 1] + " " + p[0].slice(2); }),
            datasets: [{
                label: "Consultas", data: mesesOrden.map(function (k) { return porMes[k]; }),
                borderColor: "#08a6cb", backgroundColor: "rgba(8,166,203,.15)", fill: true, tension: 0.3, pointRadius: 3
            }]
        },
        options: opcionesBarra(false)
    });

    // Por hora del día (0-23)
    var horas = new Array(24).fill(0);
    log.forEach(function (x) { var f = parseFechaLog(x); if (f) horas[f.getHours()]++; });
    crearGrafico("graficoHoraConsulta", {
        type: "bar",
        data: {
            labels: horas.map(function (_, i) { return ("0" + i).slice(-2) + "h"; }),
            datasets: [{ label: "Consultas", data: horas, backgroundColor: "#0d6efd" }]
        },
        options: opcionesBarra(false)
    });

    // Por día de la semana
    var dias = new Array(7).fill(0);
    log.forEach(function (x) { var f = parseFechaLog(x); if (f) dias[f.getDay()]++; });
    crearGrafico("graficoDiaSemana", {
        type: "bar",
        data: {
            labels: DIAS_SEMANA.map(function (d) { return d.slice(0, 3); }),
            datasets: [{ label: "Consultas", data: dias, backgroundColor: "#f18f08" }]
        },
        options: opcionesBarra(false)
    });

    // Tipo de consulta (clase)
    var tipos = contarPor(log, function (x) { return x.clase; });
    crearGrafico("graficoClaseConsulta", {
        type: "doughnut",
        data: { labels: tipos.labels, datasets: [{ data: tipos.valores, backgroundColor: PALETA }] },
        options: opcionesDoughnut()
    });
}

// ---- CRIMINAL (a partir de los casos IRISP1 donde el sujeto es integrante) ----
function RenderCriminal() {
    var casos = _casosData || [];

    var muni = contarPor(casos, function (x) { return x.municipio; }, 10);
    crearGrafico("graficoCasosMunicipio", {
        type: "horizontalBar",
        data: { labels: muni.labels, datasets: [{ label: "Casos", data: muni.valores, backgroundColor: "#c53a1d" }] },
        options: opcionesBarra(true)
    });

    var clase = contarPor(casos, function (x) { return x.clase; });
    crearGrafico("graficoCasosClase", {
        type: "doughnut",
        data: { labels: clase.labels, datasets: [{ data: clase.valores, backgroundColor: PALETA }] },
        options: opcionesDoughnut()
    });

    var exist = contarPor(casos, function (x) { return x.estadoExistenciaDescripcion; });
    crearGrafico("graficoExistenciaCasos", {
        type: "doughnut",
        data: { labels: exist.labels, datasets: [{ data: exist.valores, backgroundColor: PALETA }] },
        options: opcionesDoughnut()
    });
}

function opcionesBarra(horizontal) {
    return {
        responsive: true, maintainAspectRatio: false,
        legend: { display: false },
        scales: {
            xAxes: [{ ticks: { beginAtZero: true, precision: 0 } }],
            yAxes: [{ ticks: { beginAtZero: true, precision: 0 } }]
        }
    };
}

function opcionesDoughnut() {
    return {
        responsive: true, maintainAspectRatio: false,
        legend: { position: "right", labels: { boxWidth: 12, fontSize: 10 } }
    };
}

function LimpiarAnalitica() {
    Object.keys(_analytCharts).forEach(function (k) { try { _analytCharts[k].destroy(); } catch (e) { } });
    _analytCharts = {};
    _casosData = null;
    _logData = null;
    $("#pn_Analitica").addClass("d-none");
}

function Limpiar() {

    $("#txtIdentificacion").val("");
    $("#txtAlias").val("");
    $("#txtNombres").val("");
    $("#txtApellidos").val("");
    $("#txtObservacion").val("");
    MostrarBadgeReincidente(0, null);
    LimpiarAnalitica();
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

    _logData = Datos || [];
    TryRenderAnalitica();

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

    // Extraer coordenadas válidas ORDENADAS CRONOLÓGICAMENTE para reconstruir el recorrido
    // del sujeto (por dónde se ha desplazado, en qué orden temporal).
    let ubicaciones = datos
        .filter(x => x.latitud && x.longitud)
        .map(x => ({
            latitud: parseFloat(x.latitud),
            longitud: parseFloat(x.longitud),
            fecha: x.fecha_creacion ? new Date(x.fecha_creacion) : null,
            fechaStr: x.fecha_creacion_str || "",
            tipo: x.clase || ""
        }))
        .sort((a, b) => (a.fecha && b.fecha) ? (a.fecha - b.fecha) : 0);

    // Casos IRIS P1 vinculados al integrante que tienen coordenadas (para el punto rojo).
    let casosIris = (_casosData || []).filter(function (c) { return c.latitud && c.longitud; });

    if (ubicaciones.length === 0 && casosIris.length === 0) {
        Swal.fire("Aviso", "No hay ubicaciones para mostrar.", "info");
        return;
    }

    // Abrir modal y enviar al mapa el recorrido (antecedentes) + ubicación de los IRIS P1.
    OpenModalTodasUbicaciones(ubicaciones, casosIris);
});



function OpenModalTodasUbicaciones(listaCoordenadas, casosIris) {

    casosIris = casosIris || [];

    // Abre el modal
    $("#myModal").modal("show");

    // Evento cuando el modal ya se visualizó
    $("#myModal").off('shown.bs.modal').on('shown.bs.modal', function () {

        // iniciar mapa
        inicializarMapa('mapaDiv');

        // esperar a que cargue ArcGIS correctamente
        setTimeout(() => {
            // Preferir el recorrido cronológico (con línea de desplazamiento); si no está
            // disponible, caer al pintado simple de puntos.
            if (typeof window.pintarRecorrido === "function") {
                window.pintarRecorrido(listaCoordenadas);
            } else if (typeof window.pintarMultiplesUbicaciones === "function") {
                window.pintarMultiplesUbicaciones(listaCoordenadas);
            }

            // Punto(s) rojo(s): ubicación de los IRIS P1 vinculados. Se dibuja sobre el recorrido.
            // Si no hubo recorrido de antecedentes, el zoom se ajusta solo a estos puntos.
            if (typeof window.pintarPuntosIrisp1 === "function") {
                window.pintarPuntosIrisp1(casosIris, (listaCoordenadas || []).length === 0);
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
