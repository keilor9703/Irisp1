$(document).ready(function () {

    $(".Calendario").kendoDatePicker({
        culture: "es-CO",
        interval: 1
    });

    $("#ddlRegion").select2({ placeholder: "Todas las regiones", allowClear: true, width: "100%" });
    $("#ddlSiglaUnidad").select2({ placeholder: "Todas las unidades", allowClear: true, width: "100%" });

    var hoy = new Date();
    var haceUnAnio = new Date();
    haceUnAnio.setFullYear(hoy.getFullYear() - 1);

    $("#txtFechaInicioTablero").data("kendoDatePicker").value(haceUnAnio);
    $("#txtFechaFinTablero").data("kendoDatePicker").value(hoy);

    // Cascada Región -> Sigla de unidad: al cambiar de región se vuelve a pedir el catálogo de
    // siglas acotado a esa región (la sigla seleccionada previamente puede ya no pertenecer a ella).
    $("#ddlRegion").on("change", function () {
        RecargarSiglasPorRegion();
    });

    $("#btnAplicarFiltroTablero").on("click", function () {
        CargarTablero();
    });

    CargarTablero();
});

var chartControlGestion = null;

function CargarTablero() {
    F_GetTareasControlGestion();
    F_GetKpisTiempoGestion();
    F_GetResultadosCasos();
    RenderRangoFechas();
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

function obtenerFechaKendo(id) {
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

function obtenerFiltrosTablero() {
    return {
        V_FechaInicio: formatearFechaIso(obtenerFechaKendo("#txtFechaInicioTablero")),
        V_FechaFin: formatearFechaIso(obtenerFechaKendo("#txtFechaFinTablero")),
        V_RegionCodigo: $("#ddlRegion").val() || "",
        V_SiglaUnidad: $("#ddlSiglaUnidad").val() || ""
    };
}

function RenderRangoFechas() {
    var inicio = obtenerFechaKendo("#txtFechaInicioTablero");
    var fin = obtenerFechaKendo("#txtFechaFinTablero");
    if (!inicio || !fin) {
        $("#rangoFechasTablero").text("");
        return;
    }

    var texto = "Periodo del reporte: " + formatearFecha(inicio) + " — " + formatearFecha(fin);
    var region = $("#ddlRegion option:selected").text();
    if ($("#ddlRegion").val()) texto += " | Región: " + region;
    var sigla = $("#ddlSiglaUnidad").val();
    if (sigla) texto += " | Unidad: " + sigla;

    $("#rangoFechasTablero").text(texto);
}

function formatearFecha(fecha) {
    var dd = String(fecha.getDate()).padStart(2, "0");
    var mm = String(fecha.getMonth() + 1).padStart(2, "0");
    return dd + "/" + mm + "/" + fecha.getFullYear();
}

// Convierte horas decimales en un texto legible: "3.5 h" si es menos de un día,
// "2 d 5.3 h" si supera las 24 horas (antes siempre mostraba horas, incluso para duraciones de varios días).
function formatearDuracion(horas) {
    if (horas === null || horas === undefined) return "-";

    horas = Number(horas);
    if (isNaN(horas)) return "-";

    if (horas < 24) {
        return horas.toFixed(1).replace(/\.0$/, "") + " h";
    }

    var dias = Math.floor(horas / 24);
    var restoHoras = Math.round((horas - dias * 24) * 10) / 10;
    return dias + " d " + restoHoras.toFixed(1).replace(/\.0$/, "") + " h";
}

// Plugin genérico de Chart.js v2 (sin dependencias externas) que dibuja el valor de cada barra
// justo encima (barras verticales) o al final (horizontalBar) de la misma, para no obligar al
// usuario a pasar el mouse para saber cuánto vale cada barra.
function crearPluginValoresBarra(formateador) {
    return {
        afterDatasetsDraw: function (chart) {
            var ctx = chart.ctx;
            var esHorizontal = chart.config.type === "horizontalBar";

            ctx.save();
            ctx.font = "bold 11px Arial";
            ctx.fillStyle = "#333";

            chart.data.datasets.forEach(function (dataset, i) {
                var meta = chart.getDatasetMeta(i);
                if (meta.hidden) return;

                meta.data.forEach(function (bar, index) {
                    var valor = dataset.data[index];
                    if (valor === null || valor === undefined) return;

                    var texto = formateador ? formateador(valor) : String(valor);
                    var pos = bar.tooltipPosition();

                    if (esHorizontal) {
                        ctx.textAlign = "left";
                        ctx.textBaseline = "middle";
                        ctx.fillText(texto, pos.x + 6, pos.y);
                    } else {
                        ctx.textAlign = "center";
                        ctx.textBaseline = "bottom";
                        ctx.fillText(texto, pos.x, pos.y - 4);
                    }
                });
            });

            ctx.restore();
        }
    };
}

function F_GetTareasControlGestion() {
    $.ajax({
        type: "GET",
        url: UrlGetTareasControlGestion,
        data: obtenerFiltrosTablero(),
        dataType: "json",
        success: function (response) {
            if (response && response.success === true) {
                renderTablaTareasControlGestion(response.data || []);
                renderKpisControlGestion(response.kpis);
            } else {
                renderTablaTareasControlGestion([]);
                renderKpisControlGestion(null);
            }
        },
        error: function () {
            renderTablaTareasControlGestion([]);
            renderKpisControlGestion(null);
        }
    });
}

function F_GetKpisTiempoGestion() {
    $.ajax({
        type: "GET",
        url: UrlGetKpisTiempoGestion,
        data: obtenerFiltrosTablero(),
        dataType: "json",
        success: function (response) {
            renderKpisTiempoGestion(response && response.success === true ? response.kpis : null);
        },
        error: function () {
            renderKpisTiempoGestion(null);
        }
    });
}

function F_GetResultadosCasos() {
    $.ajax({
        type: "GET",
        url: UrlGetResultadosCasos,
        data: obtenerFiltrosTablero(),
        dataType: "json",
        success: function (response) {
            renderResultadosCasos(response && response.success === true ? response.kpis : null);
        },
        error: function () {
            renderResultadosCasos(null);
        }
    });
}

function badgeEstadoSla(estado) {
    var clase = "bg-secondary";
    if (estado === "VENCIDO") clase = "bg-danger";
    else if (estado === "EN RIESGO") clase = "bg-warning text-dark";
    else if (estado === "A TIEMPO") clase = "bg-success";

    return '<span class="badge ' + clase + '">' + (estado || "SIN SLA DEFINIDO") + '</span>';
}

function renderTablaTareasControlGestion(data) {
    var filas = data.map(function (t) {
        return [
            t.Codigo || "",
            t.UnidadSigla || t.Unidad || "",
            t.DescListaTarea || "",
            t.DescEstadoTarea || "",
            formatearDuracion(t.HorasTranscurridas),
            badgeEstadoSla(t.EstadoSla)
        ];
    });

    renderDataTable("#tbTareasControlGestion", filas, [
        { title: "Código IRISP1" },
        { title: "Unidad" },
        { title: "Tarea" },
        { title: "Estado tarea" },
        { title: "Tiempo transcurrido" },
        { title: "SLA" }
    ], {
        columnDefs: [{ targets: '_all', className: 'dt-head-center dt-body-center' }],
        preserveDraw: true
    });
}

function renderKpisControlGestion(kpis) {
    var conteos = { "A TIEMPO": 0, "EN RIESGO": 0, "VENCIDO": 0, "SIN SLA DEFINIDO": 0 };

    if (kpis && kpis.porEstado) {
        kpis.porEstado.forEach(function (e) {
            conteos[e.estado] = e.cantidad;
        });
    }

    $("#kpiTotal").text(kpis ? kpis.total : 0);
    $("#kpiATiempo").text(conteos["A TIEMPO"]);
    $("#kpiEnRiesgo").text(conteos["EN RIESGO"]);
    $("#kpiVencido").text(conteos["VENCIDO"]);
    $("#kpiSinSla").text(conteos["SIN SLA DEFINIDO"]);

    renderGraficoPorUnidad(kpis ? kpis.promedioPorUnidad : []);
}

function renderKpisTiempoGestion(kpis) {
    if (!kpis) {
        $("#kpiTiempoTotalCaso").text("-");
        $("#kpiTiempoTotalCasoSub").text("");
        $("#kpiTiempoVerificacion").text("-");
        $("#kpiTiempoVerificacionSub").text("");
        $("#kpiTiempoInvestigacion").text("-");
        $("#kpiTiempoInvestigacionSub").text("");
        return;
    }

    $("#kpiTiempoTotalCaso").text(kpis.promedioTotalHoras != null ? formatearDuracion(kpis.promedioTotalHoras) : "Sin datos");
    $("#kpiTiempoTotalCasoSub").text(kpis.casosFinalizados + " de " + kpis.totalCasos + " casos finalizados");

    $("#kpiTiempoVerificacion").text(kpis.promedioVerificacionHoras != null ? formatearDuracion(kpis.promedioVerificacionHoras) : "Sin datos");
    $("#kpiTiempoVerificacionSub").text(kpis.casosVerificacion + " casos con etapa completada");

    $("#kpiTiempoInvestigacion").text(kpis.promedioInvestigacionHoras != null ? formatearDuracion(kpis.promedioInvestigacionHoras) : "Sin datos");
    $("#kpiTiempoInvestigacionSub").text(kpis.casosInvestigacion + " casos con etapa completada");
}

function renderGraficoPorUnidad(promedioPorUnidad) {
    var ctx = document.getElementById("graficoPromedioUnidad");
    if (!ctx) return;

    // Se ordena de mayor a menor y se limita a 15 unidades para que el eje X siga siendo legible
    // cuando hay muchas unidades con datos.
    var datos = (promedioPorUnidad || []).slice().sort(function (a, b) { return b.promedioHoras - a.promedioHoras; }).slice(0, 15);

    var etiquetas = datos.map(function (u) { return u.unidad; });
    var valores = datos.map(function (u) { return u.promedioHoras; });
    // Un poco de aire arriba de la barra más alta para que la etiqueta de valor no se corte.
    var maxSugerido = valores.length ? Math.max.apply(null, valores) * 1.15 : undefined;

    if (chartControlGestion) {
        chartControlGestion.data.labels = etiquetas;
        chartControlGestion.data.datasets[0].data = valores;
        chartControlGestion.options.scales.yAxes[0].ticks.suggestedMax = maxSugerido;
        chartControlGestion.update();
        return;
    }

    chartControlGestion = new Chart(ctx, {
        type: "bar",
        data: {
            labels: etiquetas,
            datasets: [{
                label: "Promedio de tiempo",
                data: valores,
                backgroundColor: "#0d6efd"
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            legend: { display: false },
            tooltips: {
                callbacks: {
                    label: function (item) { return "Promedio: " + formatearDuracion(item.yLabel); }
                }
            },
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true,
                        suggestedMax: maxSugerido,
                        callback: function (value) { return formatearDuracion(value); }
                    },
                    scaleLabel: { display: true, labelString: "Tiempo promedio" }
                }],
                xAxes: [{
                    scaleLabel: { display: true, labelString: "Sigla de unidad" }
                }]
            }
        },
        plugins: [crearPluginValoresBarra(formatearDuracion)]
    });
}

var chartResultadoExistencia = null;
var chartPorEstadoCriminalidad = null;
var chartTopMasCasos = null;
var chartTopMenosCasos = null;

function renderResultadosCasos(kpis) {
    if (!kpis) {
        $("#kpiCasosFinalizados, #kpiCasosExiste, #kpiCasosNoExiste, #kpiCasosInconclusos, #kpiCasosAbiertos").text("0");
        renderGraficoResultadoExistencia([]);
        renderGraficoPorEstado([]);
        renderTablaEfectividadUnidad([]);
        renderGraficosTopCasosPorUnidad([]);
        return;
    }

    $("#kpiCasosFinalizados").text(kpis.finalizados);
    $("#kpiCasosExiste").text(kpis.existe);
    $("#kpiCasosNoExiste").text(kpis.noExiste);
    $("#kpiCasosInconclusos").text(kpis.inconclusos);
    $("#kpiCasosAbiertos").text(kpis.abiertos);

    renderGraficoResultadoExistencia(kpis.porExistencia || []);
    renderGraficoPorEstado(kpis.porEstado || []);
    renderTablaEfectividadUnidad(kpis.rankingUnidades || []);
    renderGraficosTopCasosPorUnidad(kpis.rankingUnidades || []);
}

function renderGraficoResultadoExistencia(porExistencia) {
    var ctx = document.getElementById("graficoResultadoExistencia");
    if (!ctx) return;

    var etiquetas = porExistencia.map(function (e) { return e.resultado; });
    var valores = porExistencia.map(function (e) { return e.cantidad; });
    var colores = ["#28a745", "#dc3545", "#6c757d"];

    if (chartResultadoExistencia) {
        chartResultadoExistencia.data.labels = etiquetas;
        chartResultadoExistencia.data.datasets[0].data = valores;
        chartResultadoExistencia.update();
        return;
    }

    chartResultadoExistencia = new Chart(ctx, {
        type: "doughnut",
        data: {
            labels: etiquetas,
            datasets: [{ data: valores, backgroundColor: colores }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            legend: { position: "bottom" }
        }
    });
}

function renderGraficoPorEstado(porEstado) {
    var ctx = document.getElementById("graficoPorEstado");
    if (!ctx) return;

    var etiquetas = porEstado.map(function (e) { return e.estado; });
    var valores = porEstado.map(function (e) { return e.cantidad; });
    var maxSugerido = valores.length ? Math.max.apply(null, valores) * 1.15 : undefined;

    if (chartPorEstadoCriminalidad) {
        chartPorEstadoCriminalidad.data.labels = etiquetas;
        chartPorEstadoCriminalidad.data.datasets[0].data = valores;
        chartPorEstadoCriminalidad.options.scales.xAxes[0].ticks.suggestedMax = maxSugerido;
        chartPorEstadoCriminalidad.update();
        return;
    }

    chartPorEstadoCriminalidad = new Chart(ctx, {
        type: "horizontalBar",
        data: {
            labels: etiquetas,
            datasets: [{ label: "Casos", data: valores, backgroundColor: "#08a6cb" }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            legend: { display: false },
            scales: {
                xAxes: [{ ticks: { beginAtZero: true, suggestedMax: maxSugerido } }]
            }
        },
        plugins: [crearPluginValoresBarra(function (v) { return String(v); })]
    });
}

function badgeEfectividad(pct) {
    var clase = "bg-danger";
    if (pct >= 70) clase = "bg-success";
    else if (pct >= 40) clase = "bg-warning text-dark";

    return '<span class="badge ' + clase + '">' + pct.toFixed(1) + '%</span>';
}

function renderTablaEfectividadUnidad(ranking) {
    var filas = ranking.map(function (u) {
        return [u.unidad || "", u.total, u.finalizados, u.existeConfirmado, u.abiertos, badgeEfectividad(u.efectividadPct)];
    });

    renderDataTable("#tbEfectividadUnidad", filas, [
        { title: "Unidad" },
        { title: "Total asignados" },
        { title: "Finalizados" },
        { title: "Existe confirmado" },
        { title: "Abiertos" },
        { title: "% Efectividad" }
    ], {
        columnDefs: [{ targets: '_all', className: 'dt-head-center dt-body-center' }],
        preserveDraw: true
    });
}

// Ambos gráficos se arman a partir del mismo "rankingUnidades" que ya trajo F_GetResultadosCasos
// (kpis.rankingUnidades[].total = cantidad de casos de esa unidad en el periodo filtrado) — no hace
// falta una consulta nueva a Oracle.
function renderGraficosTopCasosPorUnidad(rankingUnidades) {
    var ordenDesc = (rankingUnidades || []).slice().sort(function (a, b) { return b.total - a.total; });
    var top10Mas = ordenDesc.slice(0, 10);
    var top10Menos = ordenDesc.slice(-10).reverse();

    chartTopMasCasos = renderBarraTopUnidad("graficoTopMasCasos", chartTopMasCasos, top10Mas, "#28a745");
    chartTopMenosCasos = renderBarraTopUnidad("graficoTopMenosCasos", chartTopMenosCasos, top10Menos, "#dc3545");
}

function renderBarraTopUnidad(canvasId, chartExistente, datos, color) {
    var ctx = document.getElementById(canvasId);
    if (!ctx) return chartExistente;

    var etiquetas = datos.map(function (u) { return u.unidad; });
    var valores = datos.map(function (u) { return u.total; });
    var maxSugerido = valores.length ? Math.max.apply(null, valores) * 1.2 : undefined;

    if (chartExistente) {
        chartExistente.data.labels = etiquetas;
        chartExistente.data.datasets[0].data = valores;
        chartExistente.options.scales.xAxes[0].ticks.suggestedMax = maxSugerido;
        chartExistente.update();
        return chartExistente;
    }

    return new Chart(ctx, {
        type: "horizontalBar",
        data: {
            labels: etiquetas,
            datasets: [{ label: "Casos registrados", data: valores, backgroundColor: color }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            legend: { display: false },
            scales: {
                xAxes: [{ ticks: { beginAtZero: true, suggestedMax: maxSugerido, precision: 0 } }]
            }
        },
        plugins: [crearPluginValoresBarra(function (v) { return String(v); })]
    });
}
