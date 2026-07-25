var mapaLeaflet = null;
var capaPuntos = null;
var capaCluster = null;
var capaCalor = null;
var modoMapaActual = "puntos";
var puntosCargados = [];

$(document).ready(function () {

    $(".Calendario").kendoDatePicker({
        culture: "es-CO",
        interval: 1
    });

    $("#ddlRegion").select2({ placeholder: "Todas las regiones", allowClear: true, width: "100%" });
    $("#ddlSiglaUnidad").select2({ placeholder: "Todas las unidades", allowClear: true, width: "100%" });

    inicializarMapa();

    var hoy = new Date();
    var haceUnAnio = new Date();
    haceUnAnio.setFullYear(hoy.getFullYear() - 1);

    $("#txtFechaInicioMapa").data("kendoDatePicker").value(haceUnAnio);
    $("#txtFechaFinMapa").data("kendoDatePicker").value(hoy);

    // Cascada Región -> Sigla de unidad, igual que en el Tablero.
    $("#ddlRegion").on("change", function () {
        RecargarSiglasPorRegion();
    });

    $(".btn-modo-mapa").on("click", function () {
        $(".btn-modo-mapa").removeClass("active");
        $(this).addClass("active");
        modoMapaActual = $(this).data("modo");
        renderizarCapaSegunModo();
    });

    $("#btnAplicarFiltroMapa").on("click", function () {
        F_GetMapaIrisp1();
    });

    F_GetMapaIrisp1();
});

function inicializarMapa() {
    // Centro aproximado de Colombia
    mapaLeaflet = L.map("mapaIrisp1").setView([4.5709, -74.2973], 6);

    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
        maxZoom: 18,
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(mapaLeaflet);

    capaPuntos = L.layerGroup();
    capaCluster = L.markerClusterGroup();
}

function parsearFechaKendo(id) {
    var picker = $(id).data("kendoDatePicker");
    return picker ? picker.value() : null;
}

function formatearFechaIso(fecha) {
    if (!fecha) return null;
    var yyyy = fecha.getFullYear();
    var mm = String(fecha.getMonth() + 1).padStart(2, "0");
    var dd = String(fecha.getDate()).padStart(2, "0");
    return yyyy + "-" + mm + "-" + dd;
}

function RecargarSiglasPorRegion() {
    var regionCodigo = $("#ddlRegion").val();

    $.ajax({
        type: "GET",
        url: UrlGetSiglasUnidad,
        data: { regionCodigo: regionCodigo || "" },
        dataType: "json",
        success: function (response) {
            var siglas = (response && response.success === true) ? (response.data || []) : [];
            var $ddl = $("#ddlSiglaUnidad");

            $ddl.empty();
            siglas.forEach(function (s) {
                $ddl.append($("<option>", { value: s.SiglaUnidad, text: s.SiglaUnidad }));
            });
            $ddl.val(null).trigger("change");
        }
    });
}

function F_GetMapaIrisp1() {
    var fechaInicio = parsearFechaKendo("#txtFechaInicioMapa");
    var fechaFin = parsearFechaKendo("#txtFechaFinMapa");
    var regionCodigo = $("#ddlRegion").val();
    var siglaUnidad = $("#ddlSiglaUnidad").val();

    $.ajax({
        type: "GET",
        url: UrlGetMapaIrisp1,
        data: {
            V_FechaInicio: formatearFechaIso(fechaInicio),
            V_FechaFin: formatearFechaIso(fechaFin),
            V_RegionCodigo: regionCodigo || "",
            V_SiglaUnidad: siglaUnidad || ""
        },
        dataType: "json",
        success: function (response) {
            puntosCargados = (response && response.success === true) ? (response.data || []) : [];
            mostrarAvisoSinDatos(response && response.success === false ? response.message : null);
            renderizarPuntos();
        },
        error: function () {
            puntosCargados = [];
            mostrarAvisoSinDatos("No fue posible consultar el mapa. Verifique la conexión o que el paquete Oracle PK_CONTROL_GESTION_IRIS esté actualizado.");
            renderizarPuntos();
        }
    });
}

function mostrarAvisoSinDatos(mensajeError, fueraDeRango) {
    var $aviso = $("#mapaAvisoSinDatos");

    if (mensajeError) {
        $("#mapaAvisoSinDatosTexto").text(mensajeError);
        $aviso.removeClass("alert-info").addClass("alert-warning").show();
    } else if (puntosCargados.length === 0) {
        $("#mapaAvisoSinDatosTexto").text("No se encontraron casos georreferenciados con los filtros seleccionados.");
        $aviso.removeClass("alert-warning").addClass("alert-info").show();
    } else if (fueraDeRango > 0) {
        $("#mapaAvisoSinDatosTexto").text(
            fueraDeRango + " caso(s) no se muestran en el mapa por tener coordenadas fuera del territorio " +
            "colombiano (posible error de digitación en latitud/longitud) — revise esos registros en el sistema."
        );
        $aviso.removeClass("alert-info").addClass("alert-warning").show();
    } else {
        $aviso.hide();
    }
}

function renderizarPuntos() {
    renderKpisMapa(puntosCargados);
    renderGraficosComportamiento(puntosCargados);
    renderizarCapaSegunModo();
}

function limpiarCapasMapa() {
    if (capaPuntos) mapaLeaflet.removeLayer(capaPuntos);
    if (capaCluster) mapaLeaflet.removeLayer(capaCluster);
    if (capaCalor) mapaLeaflet.removeLayer(capaCalor);
}

// Rango geográfico real del territorio colombiano (con margen, incluye San Andrés y Providencia
// al occidente). El REGEXP de Oracle solo valida que latitud/longitud sean números — no que sean
// una coordenada real del país — así que un dato mal digitado (ej. punto decimal corrido) puede
// pasar como "válido" y caer en el Caribe/Centroamérica. Sin este filtro esos puntos arrastran el
// fitBounds() del mapa a un zoom absurdamente alejado, y hacen parecer mal ubicados incluso los
// puntos que sí están correctos (como el caso reportado de un punto de Barranquilla que se veía
// "fuera del continente" porque el mapa estaba zoomeado para abarcar otros puntos con mala data).
var LIMITES_COLOMBIA = { latMin: -5, latMax: 15, lngMin: -82, lngMax: -66 };
var puntosFueraDeRango = 0;

function renderizarCapaSegunModo() {
    if (!mapaLeaflet) return;

    limpiarCapasMapa();
    puntosFueraDeRango = 0;

    var puntosValidos = puntosCargados
        .map(function (p) {
            var lat = parseFloat(p.Latitud);
            var lng = parseFloat(p.Longitud);
            if (isNaN(lat) || isNaN(lng)) return null;

            if (lat < LIMITES_COLOMBIA.latMin || lat > LIMITES_COLOMBIA.latMax ||
                lng < LIMITES_COLOMBIA.lngMin || lng > LIMITES_COLOMBIA.lngMax) {
                puntosFueraDeRango++;
                return null;
            }

            return { lat: lat, lng: lng, data: p };
        })
        .filter(function (p) { return p !== null; });

    if (puntosFueraDeRango > 0) {
        mostrarAvisoSinDatos(null, puntosFueraDeRango);
    }

    if (puntosValidos.length === 0) return;

    if (modoMapaActual === "calor") {
        var puntosCalor = puntosValidos.map(function (p) { return [p.lat, p.lng, 0.5]; });
        capaCalor = L.heatLayer(puntosCalor, { radius: 22, blur: 18, maxZoom: 15 });
        capaCalor.addTo(mapaLeaflet);
    } else if (modoMapaActual === "cluster") {
        capaCluster = L.markerClusterGroup();
        puntosValidos.forEach(function (p) {
            capaCluster.addLayer(L.marker([p.lat, p.lng]).bindPopup(popupPunto(p.data)));
        });
        capaCluster.addTo(mapaLeaflet);
    } else {
        capaPuntos = L.layerGroup();
        puntosValidos.forEach(function (p) {
            capaPuntos.addLayer(L.circleMarker([p.lat, p.lng], {
                radius: 6,
                color: "#08a6cb",
                fillColor: "#0d6efd",
                fillOpacity: 0.7,
                weight: 1
            }).bindPopup(popupPunto(p.data)));
        });
        capaPuntos.addTo(mapaLeaflet);
    }

    var bounds = L.latLngBounds(puntosValidos.map(function (p) { return [p.lat, p.lng]; }));
    mapaLeaflet.fitBounds(bounds, { maxZoom: 12, padding: [20, 20] });
}

function popupPunto(p) {
    var fecha = p.FechaCreacion ? new Date(p.FechaCreacion).toLocaleDateString("es-CO") : "-";
    return "<b>" + (p.Codigo || "Sin código") + "</b><br/>" +
        "Unidad: " + (p.SiglaUnidad || "-") + "<br/>" +
        "Municipio: " + (p.Municipio || "-") + (p.Barrio ? (" - " + p.Barrio) : "") + "<br/>" +
        "Delito: " + (p.DelitoPrincipal || "-") + "<br/>" +
        "Estado: " + (p.Estado || "-") + "<br/>" +
        "Fecha: " + fecha;
}

function renderKpisMapa(puntos) {
    var unidades = {};
    var municipios = {};
    var delitos = {};

    puntos.forEach(function (p) {
        if (p.SiglaUnidad) unidades[p.SiglaUnidad] = true;
        if (p.Municipio) municipios[p.Municipio] = true;
        if (p.DelitoPrincipal) delitos[p.DelitoPrincipal] = (delitos[p.DelitoPrincipal] || 0) + 1;
    });

    var delitoTop = "-";
    var maxCount = 0;
    Object.keys(delitos).forEach(function (d) {
        if (delitos[d] > maxCount) { maxCount = delitos[d]; delitoTop = d; }
    });

    $("#mapaKpiTotal").text(puntos.length);
    $("#mapaKpiUnidades").text(Object.keys(unidades).length);
    $("#mapaKpiMunicipios").text(Object.keys(municipios).length);
    $("#mapaKpiDelito").text(delitoTop);
}

var chartPorDiaSemana = null;
var chartPorHora = null;

function renderGraficosComportamiento(puntos) {
    var diasSemana = ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"];
    var conteoDias = [0, 0, 0, 0, 0, 0, 0];
    var conteoHoras = new Array(24).fill(0);

    puntos.forEach(function (p) {
        if (!p.FechaCreacion) return;
        var fecha = new Date(p.FechaCreacion);
        if (isNaN(fecha.getTime())) return;
        conteoDias[fecha.getDay()]++;
        conteoHoras[fecha.getHours()]++;
    });

    var ctxDia = document.getElementById("graficoPorDiaSemana");
    if (ctxDia) {
        if (chartPorDiaSemana) {
            chartPorDiaSemana.data.datasets[0].data = conteoDias;
            chartPorDiaSemana.update();
        } else {
            chartPorDiaSemana = new Chart(ctxDia, {
                type: "bar",
                data: {
                    labels: diasSemana,
                    datasets: [{ label: "Casos", data: conteoDias, backgroundColor: "#08a6cb" }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    legend: { display: false },
                    scales: { yAxes: [{ ticks: { beginAtZero: true } }] }
                }
            });
        }
    }

    var ctxHora = document.getElementById("graficoPorHora");
    if (ctxHora) {
        var etiquetasHoras = conteoHoras.map(function (_, h) { return h + ":00"; });
        if (chartPorHora) {
            chartPorHora.data.datasets[0].data = conteoHoras;
            chartPorHora.update();
        } else {
            chartPorHora = new Chart(ctxHora, {
                type: "line",
                data: {
                    labels: etiquetasHoras,
                    datasets: [{
                        label: "Casos",
                        data: conteoHoras,
                        borderColor: "#0d6efd",
                        backgroundColor: "rgba(13,110,253,0.15)",
                        fill: true,
                        tension: 0.3
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    legend: { display: false },
                    scales: { yAxes: [{ ticks: { beginAtZero: true } }] }
                }
            });
        }
    }
}
