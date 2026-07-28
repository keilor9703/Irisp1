$(document).ready(function () {

    $(".Calendario").kendoDatePicker({
        culture: "es-CO",
        interval: 1
    });

    $("#ddlRegion").select2({ placeholder: "Todas las regiones", allowClear: true, width: "100%" });
    $("#ddlSiglaUnidad").select2({ placeholder: "Todas las unidades", allowClear: true, width: "100%" });
    $("#ddlClase").select2({ placeholder: "Todas las clases", allowClear: true, width: "100%" });

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

    // Exporta el informe del tablero (con los filtros vigentes) a PDF con marca de agua
    // del usuario institucional. Se hace por POST porque además de los filtros se envían las
    // imágenes de las gráficas capturadas del canvas de Chart.js (no caben en la URL). El POST
    // se envía a un iframe oculto para que el navegador descargue el PDF sin dejar pestañas.
    $("#btnExportarPdfTablero").on("click", function () {
        ExportarTableroPdf();
    });

    CargarTablero();
    InicializarDrillDown();
});

// Captura un canvas de Chart.js como PNG (data URL). Devuelve "" si el canvas no existe
// o si la gráfica aún no se ha renderizado (no hay datos), para que el PDF la omita.
function capturarGrafica(idCanvas) {
    try {
        var canvas = document.getElementById(idCanvas);
        if (!canvas || !canvas.getContext || !canvas.width || !canvas.height) return "";

        // El canvas de Chart.js es transparente; se compone sobre fondo blanco para que en
        // el PDF la gráfica se vea limpia (sin que la marca de agua se trasluzca por debajo).
        var temp = document.createElement("canvas");
        temp.width = canvas.width;
        temp.height = canvas.height;
        var ctx = temp.getContext("2d");
        ctx.fillStyle = "#ffffff";
        ctx.fillRect(0, 0, temp.width, temp.height);
        ctx.drawImage(canvas, 0, 0);
        return temp.toDataURL("image/png");
    } catch (e) {
        return "";
    }
}

function ExportarTableroPdf() {
    var filtros = obtenerFiltrosTablero();

    var iframe = document.createElement("iframe");
    iframe.name = "pdfTableroFrame_" + Date.now();
    iframe.style.display = "none";
    document.body.appendChild(iframe);

    var form = document.createElement("form");
    form.method = "POST";
    form.action = UrlExportarPdfTablero;
    form.target = iframe.name;

    function agregarCampo(nombre, valor) {
        var input = document.createElement("input");
        input.type = "hidden";
        input.name = nombre;
        input.value = (valor === null || valor === undefined) ? "" : valor;
        form.appendChild(input);
    }

    // Filtros vigentes
    Object.keys(filtros).forEach(function (k) { agregarCampo(k, filtros[k]); });

    // Token anti-CSRF (lo renderiza _Layout con @Html.AntiForgeryToken())
    var token = $('input[name="__RequestVerificationToken"]').first().val();
    agregarCampo("__RequestVerificationToken", token);

    // Imágenes de las gráficas
    agregarCampo("GraficoPorUnidad", capturarGrafica("graficoPromedioUnidad"));
    agregarCampo("GraficoExistencia", capturarGrafica("graficoResultadoExistencia"));
    agregarCampo("GraficoTopMas", capturarGrafica("graficoTopMasCasos"));
    agregarCampo("GraficoTopMenos", capturarGrafica("graficoTopMenosCasos"));
    agregarCampo("GraficoPorEstado", capturarGrafica("graficoPorEstado"));
    agregarCampo("GraficoVolumenVerif", capturarGrafica("graficoVolumenVerificacion"));
    agregarCampo("GraficoTopInformantes", capturarGrafica("graficoTopInformantes"));

    document.body.appendChild(form);
    form.submit();

    // Limpieza diferida (tras dar tiempo a que la descarga inicie)
    setTimeout(function () {
        if (form.parentNode) document.body.removeChild(form);
        if (iframe.parentNode) document.body.removeChild(iframe);
    }, 60000);
}

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
        V_SiglaUnidad: $("#ddlSiglaUnidad").val() || "",
        V_IdClase: $("#ddlClase").val() || ""
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
    if ($("#ddlClase").val()) texto += " | Clase: " + $("#ddlClase option:selected").text();

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

// Se conservan en memoria los mismos datasets crudos que ya trae cada respuesta, para poder
// filtrarlos en el cliente cuando el usuario hace doble click sobre una tarjeta/gráfica/fila
// (drill-down tipo tabla dinámica de Excel) sin tener que volver a consultar Oracle.
var datosTareasActuales = [];
var datosCasosActuales = [];
var datosResultadosActuales = [];

function F_GetTareasControlGestion() {
    $.ajax({
        type: "GET",
        url: UrlGetTareasControlGestion,
        data: obtenerFiltrosTablero(),
        dataType: "json",
        success: function (response) {
            if (response && response.success === true) {
                datosTareasActuales = response.data || [];
                renderTablaTareasControlGestion(datosTareasActuales);
                renderKpisControlGestion(response.kpis);
            } else {
                datosTareasActuales = [];
                renderTablaTareasControlGestion([]);
                renderKpisControlGestion(null);
            }
        },
        error: function () {
            datosTareasActuales = [];
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
            datosCasosActuales = (response && response.success === true) ? (response.data || []) : [];
            renderKpisTiempoGestion(response && response.success === true ? response.kpis : null);
        },
        error: function () {
            datosCasosActuales = [];
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
            datosResultadosActuales = (response && response.success === true) ? (response.data || []) : [];
            renderResultadosCasos(response && response.success === true ? response.kpis : null);
        },
        error: function () {
            datosResultadosActuales = [];
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
var chartVolumenVerificacion = null;
var chartTopInformantes = null;
var chartPorClase = null;

function renderResultadosCasos(kpis) {
    if (!kpis) {
        $("#kpiCasosFinalizados, #kpiCasosExiste, #kpiCasosNoExiste, #kpiCasosInconclusos, #kpiCasosAbiertos").text("0");
        renderGraficoResultadoExistencia([]);
        renderGraficoPorEstado([]);
        renderTablaEfectividadUnidad([]);
        renderGraficosTopCasosPorUnidad([]);
        renderTablaEfectividadVerificacion([]);
        renderGraficoVolumenVerificacion([]);
        renderTablaFuncionariosInforma([]);
        renderGraficoTopInformantes([]);
        renderGraficoPorClase([]);
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
    renderTablaEfectividadVerificacion(kpis.rankingUnidadesVerificacion || []);
    renderGraficoVolumenVerificacion(kpis.rankingUnidadesVerificacion || []);
    renderTablaFuncionariosInforma(kpis.rankingFuncionariosInforma || []);
    renderGraficoTopInformantes(kpis.rankingFuncionariosInforma || []);
    renderGraficoPorClase(kpis.porClase || []);
}

// --- Funcionarios que INFORMARON (IDENTIFICACION_INFORMA) ---
// Reconoce a los funcionarios cuya información se convirtió en casos exitosos. La tabla ordena por
// casos exitosos; el gráfico muestra el Top 10 de informantes con más casos exitosos.
function renderTablaFuncionariosInforma(ranking) {
    var filas = (ranking || []).map(function (u) {
        return [u.funcionario || String(u.identificacion || ""), u.identificacion || "",
                u.totalInformados, u.exitosos, u.conResultados, u.noExiste, badgeEfectividad(u.efectividadPct)];
    });

    renderDataTable("#tbFuncionariosInforma", filas, [
        { title: "Funcionario que informó" },
        { title: "Identificación" },
        { title: "Registros informados" },
        { title: "Existe confirmado" },
        { title: "Con resultados (SPOA/SIEDCO)" },
        { title: "No existe" },
        { title: "% Efectividad" }
    ], {
        columnDefs: [{ targets: '_all', className: 'dt-head-center dt-body-center' }],
        order: [[3, 'desc']],
        preserveDraw: true
    });
}

function renderGraficoTopInformantes(ranking) {
    // Top 10 informantes por casos EXITOSOS (el objetivo del reporte es reconocer a los que más
    // aportan resultados). renderBarraTopUnidad espera { unidad, total }.
    var top10 = (ranking || []).slice()
        .sort(function (a, b) { return b.exitosos - a.exitosos; })
        .slice(0, 10)
        .map(function (u) {
            var nombre = u.funcionario || String(u.identificacion || "");
            var etiqueta = u.sigla ? (u.sigla + " - " + nombre) : nombre;
            return { unidad: etiqueta, total: u.exitosos, identificacion: u.identificacion };
        });

    chartTopInformantes = renderBarraTopUnidad("graficoTopInformantes", chartTopInformantes, top10, "#20c997");
    // Guarda la identificación por barra para el drill-down (el label es solo el nombre).
    if (chartTopInformantes) chartTopInformantes.datosOriginales = top10;
}

// --- Análisis de unidades de VERIFICACIÓN (unidades a las que se asigna la investigación) ---
// Mide su efectividad (registros exitosos = existencia confirmada) y su volumen (registros
// asignados). Ambos salen del mismo kpis.rankingUnidadesVerificacion; no hay consulta extra.
function renderTablaEfectividadVerificacion(ranking) {
    var filas = (ranking || []).map(function (u) {
        return [u.unidad || "", u.total, u.finalizados, u.existeConfirmado, u.noExiste, u.abiertos, badgeEfectividad(u.efectividadPct)];
    });

    renderDataTable("#tbEfectividadVerificacion", filas, [
        { title: "Unidad de verificación" },
        { title: "Registros asignados" },
        { title: "Finalizados" },
        { title: "Existe confirmado" },
        { title: "No existe" },
        { title: "Abiertos" },
        { title: "% Efectividad" }
    ], {
        columnDefs: [{ targets: '_all', className: 'dt-head-center dt-body-center' }],
        order: [[1, 'desc']],
        preserveDraw: true
    });
}

function renderGraficoVolumenVerificacion(ranking) {
    var top10 = (ranking || []).slice().sort(function (a, b) { return b.total - a.total; }).slice(0, 10);
    chartVolumenVerificacion = renderBarraTopUnidad("graficoVolumenVerificacion", chartVolumenVerificacion, top10, "#6f42c1");
}

// Plugin que dibuja el total en el centro de la dona — evita el hueco vacío en medio del
// gráfico y da de entrada el dato más importante (cuántos casos hay en total).
var pluginTotalCentroDona = {
    afterDraw: function (chart) {
        if (chart.config.type !== "doughnut") return;

        var total = chart.data.datasets[0].data.reduce(function (a, b) { return a + (b || 0); }, 0);
        var ctx = chart.ctx;
        var area = chart.chartArea;
        var cx = (area.left + area.right) / 2;
        var cy = (area.top + area.bottom) / 2;

        ctx.save();
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.font = "bold 26px Arial";
        ctx.fillStyle = "#002a66";
        ctx.fillText(total, cx, cy - 10);
        ctx.font = "11px Arial";
        ctx.fillStyle = "#6c757d";
        ctx.fillText("casos", cx, cy + 14);
        ctx.restore();
    }
};

function renderGraficoResultadoExistencia(porExistencia) {
    var ctx = document.getElementById("graficoResultadoExistencia");
    if (!ctx) return;

    var etiquetas = porExistencia.map(function (e) { return e.resultado; });
    var valores = porExistencia.map(function (e) { return e.cantidad; });
    var colores = ["#28a745", "#dc3545", "#6c757d"];

    renderLeyendaResultadoExistencia(porExistencia, colores);

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
            datasets: [{ data: valores, backgroundColor: colores, borderWidth: 2, borderColor: "#fff" }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            cutoutPercentage: 62,
            legend: { display: false },
            tooltips: {
                callbacks: {
                    label: function (item, data) {
                        var valor = data.datasets[0].data[item.index];
                        var total = data.datasets[0].data.reduce(function (a, b) { return a + b; }, 0);
                        var pct = total > 0 ? Math.round(valor * 1000 / total) / 10 : 0;
                        return data.labels[item.index] + ": " + valor + " (" + pct + "%)";
                    }
                }
            }
        },
        plugins: [pluginTotalCentroDona]
    });
}

// Leyenda propia en HTML (en vez de la leyenda por defecto de Chart.js) para poder mostrar
// cantidad y porcentaje de cada resultado, no solo el color — la leyenda original no dejaba
// analizar nada, solo mostraba el nombre.
function renderLeyendaResultadoExistencia(porExistencia, colores) {
    var $leyenda = $("#leyendaResultadoExistencia");
    if (!$leyenda.length) return;

    var total = porExistencia.reduce(function (acc, e) { return acc + (e.cantidad || 0); }, 0);

    var html = porExistencia.map(function (e, i) {
        var pct = total > 0 ? Math.round(e.cantidad * 1000 / total) / 10 : 0;
        return '<div class="d-flex justify-content-between align-items-center" style="font-size:.85rem; padding:4px 2px; cursor:pointer;" data-existencia="' + e.resultado + '">' +
            '<span><span style="display:inline-block;width:10px;height:10px;border-radius:2px;background:' + colores[i] + ';margin-right:6px;"></span>' + e.resultado + '</span>' +
            '<strong>' + e.cantidad + ' <span class="text-muted" style="font-weight:normal;">(' + pct + '%)</span></strong>' +
            '</div>';
    }).join("");

    $leyenda.html(html);
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

// Cantidad de casos IRISP1 por CLASE (kpis.porClase: [{clase, cantidad}]).
function renderGraficoPorClase(porClase) {
    var ctx = document.getElementById("graficoPorClase");
    if (!ctx) return;

    var etiquetas = porClase.map(function (e) { return e.clase; });
    var valores = porClase.map(function (e) { return e.cantidad; });
    var maxSugerido = valores.length ? Math.max.apply(null, valores) * 1.15 : undefined;

    if (chartPorClase) {
        chartPorClase.data.labels = etiquetas;
        chartPorClase.data.datasets[0].data = valores;
        chartPorClase.options.scales.xAxes[0].ticks.suggestedMax = maxSugerido;
        chartPorClase.update();
        return;
    }

    chartPorClase = new Chart(ctx, {
        type: "horizontalBar",
        data: {
            labels: etiquetas,
            datasets: [{ label: "Casos", data: valores, backgroundColor: "#6f42c1" }]
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

// ============================================================================
// Drill-down: doble click sobre una tarjeta KPI, una barra/porción de gráfica o una fila de
// "Efectividad por unidad" abre un modal con el detalle de los registros que arman ese número
// — igual que al hacer doble click sobre un valor de una tabla dinámica en Excel. Los datasets
// crudos (datosTareasActuales/datosCasosActuales/datosResultadosActuales) ya están en memoria
// porque cada respuesta de F_Get* los trae junto con los KPIs agregados, así que el filtrado es
// 100% en el cliente, sin volver a consultar Oracle.
// ============================================================================

const ID_ESTADO_FINALIZADO = 5;

function esCasoConExistencia(c) {
    return !!(c.DescEstadoExistencia && c.DescEstadoExistencia.trim().toUpperCase().indexOf("SI EXISTE") !== -1);
}

function esCasoSinExistencia(c) {
    return !!(c.DescEstadoExistencia && c.DescEstadoExistencia.trim().toUpperCase().indexOf("NO EXISTE") !== -1);
}

function esCasoFinalizado(c) {
    return c.IdEstado === ID_ESTADO_FINALIZADO;
}

function formatearEstadoCaso(idEstado) {
    return idEstado === ID_ESTADO_FINALIZADO ? "Finalizado" : "En trámite";
}

var columnasTareasDetalle = [
    { title: "Código IRISP1" }, { title: "Unidad" }, { title: "Tarea" },
    { title: "Estado tarea" }, { title: "Tiempo transcurrido" }, { title: "SLA" }
];
function filasTareasDetalle(lista) {
    return lista.map(function (t) {
        return [t.Codigo || "", t.UnidadSigla || t.Unidad || "", t.DescListaTarea || "",
                t.DescEstadoTarea || "", formatearDuracion(t.HorasTranscurridas), badgeEstadoSla(t.EstadoSla)];
    });
}

var columnasCasosDetalle = [
    { title: "Código IRISP1" }, { title: "Unidad" }, { title: "Fecha creación" },
    { title: "Tiempo total" }, { title: "Tiempo Verificación" }, { title: "Tiempo Investigación" }
];
function filasCasosDetalle(lista) {
    return lista.map(function (c) {
        return [c.Codigo || "", c.UnidadSigla || c.Unidad || "",
                c.FechaCreacion ? new Date(c.FechaCreacion).toLocaleDateString("es-CO") : "-",
                formatearDuracion(c.HorasTotalCaso), formatearDuracion(c.HorasVerificacion), formatearDuracion(c.HorasInvestigacion)];
    });
}

var columnasResultadosDetalle = [
    { title: "Código IRISP1" }, { title: "Unidad" }, { title: "Dependencia (estación)" },
    { title: "Unidad de verificación" }, { title: "Funcionario que informó" }, { title: "Fecha creación" },
    { title: "Estado" }, { title: "Existencia" }, { title: "Con resultados" }
];
function filasResultadosDetalle(lista) {
    return lista.map(function (c) {
        var funcionario = c.FuncionarioInforma || (c.IdentificacionInforma ? String(c.IdentificacionInforma) : "-");
        return [c.Codigo || "", c.UnidadSigla || c.Unidad || "",
                c.Dependencia || "-",
                c.UnidadVerificacion || c.UnidadVerificacionSigla || "-",
                funcionario,
                c.FechaCreacion ? new Date(c.FechaCreacion).toLocaleDateString("es-CO") : "-",
                c.DescEstado || formatearEstadoCaso(c.IdEstado), c.DescEstadoExistencia || "Sin determinar",
                c.TieneResultados === 1 ? "Sí" : "No"];
    });
}

function abrirDetalle(titulo, subtitulo, columnas, filas) {
    $("#ModalDetalleRegistrosLabel").html('<i class="mr-2 fas fa-list-ul"></i> ' + titulo);
    $("#ModalDetalleRegistrosSubtitulo").text(subtitulo || "");

    if ($.fn.dataTable.isDataTable("#tbDetalleRegistros")) {
        $("#tbDetalleRegistros").DataTable().destroy();
        $("#tbDetalleRegistros").empty();
    }

    $("#tbDetalleRegistros").DataTable({
        data: filas,
        columns: columnas,
        language: glOpcionesIdioma,
        scrollX: true,
        pageLength: 10,
        lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
        columnDefs: [{ targets: '_all', className: 'dt-head-center dt-body-center' }]
    });

    $("#ModalDetalleRegistros").modal("show");
}

function abrirDetalleVacio(titulo) {
    abrirDetalle(titulo, "No hay registros que coincidan.", [{ title: "Sin datos" }], []);
}

// Despacha por el identificador data-kpi de cada tarjeta hacia el dataset y filtro correctos.
function abrirDetalleKpi(kpi) {
    switch (kpi) {
        case "tareas-total":
            abrirDetalle("Total tareas", datosTareasActuales.length + " tarea(s)", columnasTareasDetalle, filasTareasDetalle(datosTareasActuales));
            break;
        case "tareas-atiempo":
            var atiempo = datosTareasActuales.filter(function (t) { return t.EstadoSla === "A TIEMPO"; });
            abrirDetalle("Tareas a tiempo", atiempo.length + " tarea(s)", columnasTareasDetalle, filasTareasDetalle(atiempo));
            break;
        case "tareas-enriesgo":
            var enriesgo = datosTareasActuales.filter(function (t) { return t.EstadoSla === "EN RIESGO"; });
            abrirDetalle("Tareas en riesgo", enriesgo.length + " tarea(s)", columnasTareasDetalle, filasTareasDetalle(enriesgo));
            break;
        case "tareas-vencido":
            var vencidas = datosTareasActuales.filter(function (t) { return t.EstadoSla === "VENCIDO"; });
            abrirDetalle("Tareas vencidas", vencidas.length + " tarea(s)", columnasTareasDetalle, filasTareasDetalle(vencidas));
            break;
        case "tareas-sinsla":
            var sinSla = datosTareasActuales.filter(function (t) { return !t.EstadoSla || t.EstadoSla === "SIN SLA DEFINIDO"; });
            abrirDetalle("Tareas sin SLA definido", sinSla.length + " tarea(s)", columnasTareasDetalle, filasTareasDetalle(sinSla));
            break;

        case "casos-tiempototal":
            var conTiempoTotal = datosCasosActuales.filter(function (c) { return c.HorasTotalCaso != null; });
            abrirDetalle("Casos finalizados (tiempo total)", conTiempoTotal.length + " caso(s)", columnasCasosDetalle, filasCasosDetalle(conTiempoTotal));
            break;
        case "casos-verificacion":
            var conVerif = datosCasosActuales.filter(function (c) { return c.HorasVerificacion != null; });
            abrirDetalle("Casos con etapa de Verificación completada", conVerif.length + " caso(s)", columnasCasosDetalle, filasCasosDetalle(conVerif));
            break;
        case "casos-investigacion":
            var conInves = datosCasosActuales.filter(function (c) { return c.HorasInvestigacion != null; });
            abrirDetalle("Casos con etapa de Investigación completada", conInves.length + " caso(s)", columnasCasosDetalle, filasCasosDetalle(conInves));
            break;

        case "resultados-finalizados":
            var finalizados = datosResultadosActuales.filter(esCasoFinalizado);
            abrirDetalle("Casos finalizados", finalizados.length + " caso(s)", columnasResultadosDetalle, filasResultadosDetalle(finalizados));
            break;
        case "resultados-existe":
            var existe = datosResultadosActuales.filter(esCasoConExistencia);
            abrirDetalle("Casos con existencia confirmada", existe.length + " caso(s)", columnasResultadosDetalle, filasResultadosDetalle(existe));
            break;
        case "resultados-noexiste":
            var noExiste = datosResultadosActuales.filter(esCasoSinExistencia);
            abrirDetalle("Casos sin existencia (descartados)", noExiste.length + " caso(s)", columnasResultadosDetalle, filasResultadosDetalle(noExiste));
            break;
        case "resultados-inconclusos":
            var inconclusos = datosResultadosActuales.filter(function (c) {
                return esCasoFinalizado(c) && !esCasoConExistencia(c) && !esCasoSinExistencia(c);
            });
            abrirDetalle("Casos inconclusos", inconclusos.length + " caso(s)", columnasResultadosDetalle, filasResultadosDetalle(inconclusos));
            break;
        case "resultados-abiertos":
            var abiertos = datosResultadosActuales.filter(function (c) { return !esCasoFinalizado(c); });
            abrirDetalle("Casos abiertos (en trámite)", abiertos.length + " caso(s)", columnasResultadosDetalle, filasResultadosDetalle(abiertos));
            break;

        default:
            abrirDetalleVacio("Detalle");
    }
}

// Índice del punto/barra sobre el que se hizo doble click en un canvas de Chart.js v2.
function obtenerIndiceChart(chart, evt) {
    var elementos = chart.getElementsAtEventForMode(evt, "nearest", { intersect: true }, false);
    return elementos.length ? elementos[0]._index : null;
}

function InicializarDrillDown() {
    InicializarDrillDownInformantes();
    // DataTables calcula el ancho de columnas en el momento de inicializarse; si el modal todavía
    // está oculto (display:none) esas columnas quedan en 0px. Se recalculan cuando el modal
    // termina de mostrarse.
    $("#ModalDetalleRegistros").on("shown.bs.modal", function () {
        if ($.fn.dataTable.isDataTable("#tbDetalleRegistros")) {
            $("#tbDetalleRegistros").DataTable().columns.adjust().draw(false);
        }
    });

    // Tarjetas KPI
    $(document).on("dblclick", ".stat-card[data-kpi]", function () {
        abrirDetalleKpi($(this).data("kpi"));
    });

    // Doughnut "Resultado de existencia": también se puede hacer doble click sobre cada fila
    // de la leyenda propia (no solo sobre la porción de la torta).
    document.getElementById("graficoResultadoExistencia").addEventListener("dblclick", function (evt) {
        if (!chartResultadoExistencia) return;
        var idx = obtenerIndiceChart(chartResultadoExistencia, evt);
        if (idx === null) return;
        filtrarPorExistenciaYAbrir(chartResultadoExistencia.data.labels[idx]);
    });
    $(document).on("dblclick", "#leyendaResultadoExistencia [data-existencia]", function () {
        filtrarPorExistenciaYAbrir($(this).data("existencia"));
    });

    // Barras "Promedio de tiempo por unidad" (dataset de tareas, filtra por unidad)
    document.getElementById("graficoPromedioUnidad").addEventListener("dblclick", function (evt) {
        if (!chartControlGestion) return;
        var idx = obtenerIndiceChart(chartControlGestion, evt);
        if (idx === null) return;
        var unidad = chartControlGestion.data.labels[idx];
        var filtradas = datosTareasActuales.filter(function (t) { return (t.UnidadSigla || t.Unidad) === unidad; });
        abrirDetalle("Tareas de la unidad " + unidad, filtradas.length + " tarea(s)", columnasTareasDetalle, filasTareasDetalle(filtradas));
    });

    // Barras "Casos por estado general" (dataset de resultados, filtra por estado)
    document.getElementById("graficoPorEstado").addEventListener("dblclick", function (evt) {
        if (!chartPorEstadoCriminalidad) return;
        var idx = obtenerIndiceChart(chartPorEstadoCriminalidad, evt);
        if (idx === null) return;
        var estado = chartPorEstadoCriminalidad.data.labels[idx];
        var filtrados = datosResultadosActuales.filter(function (c) { return (c.DescEstado || "Sin estado") === estado; });
        abrirDetalle("Casos en estado " + estado, filtrados.length + " caso(s)", columnasResultadosDetalle, filasResultadosDetalle(filtrados));
    });

    // Top 10 con más/menos casos (dataset de resultados, filtra por unidad)
    ["graficoTopMasCasos", "graficoTopMenosCasos"].forEach(function (canvasId) {
        document.getElementById(canvasId).addEventListener("dblclick", function (evt) {
            var chart = canvasId === "graficoTopMasCasos" ? chartTopMasCasos : chartTopMenosCasos;
            if (!chart) return;
            var idx = obtenerIndiceChart(chart, evt);
            if (idx === null) return;
            var unidad = chart.data.labels[idx];
            var filtrados = datosResultadosActuales.filter(function (c) { return (c.UnidadSigla || c.Unidad) === unidad; });
            abrirDetalle("Casos de la unidad " + unidad, filtrados.length + " caso(s)", columnasResultadosDetalle, filasResultadosDetalle(filtrados));
        });
    });

    // Filas de la tabla "Efectividad por unidad" (delegado: la tabla se reconstruye con cada filtro)
    $(document).on("dblclick", "#tbEfectividadUnidad tbody tr", function () {
        var fila = $("#tbEfectividadUnidad").DataTable().row(this).data();
        if (!fila) return;
        var unidad = $("<div>").html(fila[0]).text(); // primera columna = Unidad
        var filtrados = datosResultadosActuales.filter(function (c) { return (c.UnidadSigla || c.Unidad) === unidad; });
        abrirDetalle("Casos de la unidad " + unidad, filtrados.length + " caso(s)", columnasResultadosDetalle, filasResultadosDetalle(filtrados));
    });

    // Filas de la tabla "Efectividad por unidad de verificación": la primera columna es la SIGLA
    // (DIJIN, DIPOL...); se filtra por esa sigla y la modal se titula con la descripción completa.
    $(document).on("dblclick", "#tbEfectividadVerificacion tbody tr", function () {
        var fila = $("#tbEfectividadVerificacion").DataTable().row(this).data();
        if (!fila) return;
        var sigla = $("<div>").html(fila[0]).text();
        abrirDetalleVerificacion(sigla);
    });

    // Gráfico de volumen por unidad de verificación (etiquetas = siglas)
    var canvasVol = document.getElementById("graficoVolumenVerificacion");
    if (canvasVol) {
        canvasVol.addEventListener("dblclick", function (evt) {
            if (!chartVolumenVerificacion) return;
            var idx = obtenerIndiceChart(chartVolumenVerificacion, evt);
            if (idx === null) return;
            abrirDetalleVerificacion(chartVolumenVerificacion.data.labels[idx]);
        });
    }
}

// Abre el detalle de los casos asignados a una unidad de verificación identificada por su SIGLA.
// El título de la modal usa la descripción completa (ej. "SECCIONAL INVESTIGACION CRIMINAL MECAL").
function abrirDetalleVerificacion(sigla) {
    var filtrados = datosResultadosActuales.filter(function (c) { return c.UnidadVerificacionSigla === sigla; });
    var descripcion = filtrados.length ? (filtrados[0].UnidadVerificacion || sigla) : sigla;
    abrirDetalle("Casos asignados a la unidad de verificación " + descripcion,
        filtrados.length + " caso(s)", columnasResultadosDetalle, filasResultadosDetalle(filtrados));
}

// Abre el detalle de los registros informados por un funcionario (por su identificación).
function abrirDetalleInformante(identificacion) {
    if (identificacion === null || identificacion === undefined || identificacion === "") return;
    var idNum = String(identificacion);
    var filtrados = datosResultadosActuales.filter(function (c) { return String(c.IdentificacionInforma) === idNum; });
    var reg = filtrados.length ? filtrados[0] : null;
    var nombre = reg ? (reg.FuncionarioInforma || idNum) : idNum;

    // Subtítulo con la dependencia y el cargo del funcionario que informó (más el conteo).
    var partes = [];
    if (reg && reg.SiglaInforma) partes.push("Unidad: " + reg.SiglaInforma);
    if (reg && reg.DependenciaInforma) partes.push("Dependencia: " + reg.DependenciaInforma);
    if (reg && reg.CargoInforma) partes.push("Cargo: " + reg.CargoInforma);
    partes.push("Identificación: " + idNum);
    partes.push(filtrados.length + " registro(s)");

    abrirDetalle("Registros informados por " + nombre, partes.join(" | "),
        columnasResultadosDetalle, filasResultadosDetalle(filtrados));
}

function InicializarDrillDownInformantes() {
    // Fila de la tabla de funcionarios: la 2a columna (índice 1) es la identificación
    $(document).on("dblclick", "#tbFuncionariosInforma tbody tr", function () {
        var fila = $("#tbFuncionariosInforma").DataTable().row(this).data();
        if (!fila) return;
        abrirDetalleInformante($("<div>").html(fila[1]).text());
    });

    // Gráfico Top informantes: cada barra guarda la identificación en el punto ordenado
    var canvasInf = document.getElementById("graficoTopInformantes");
    if (canvasInf) {
        canvasInf.addEventListener("dblclick", function (evt) {
            if (!chartTopInformantes) return;
            var idx = obtenerIndiceChart(chartTopInformantes, evt);
            if (idx === null) return;
            var ident = (chartTopInformantes.datosOriginales && chartTopInformantes.datosOriginales[idx])
                ? chartTopInformantes.datosOriginales[idx].identificacion : null;
            abrirDetalleInformante(ident);
        });
    }
}

function filtrarPorExistenciaYAbrir(etiqueta) {
    var filtrados;
    var titulo;

    if (etiqueta === "Existe") {
        filtrados = datosResultadosActuales.filter(esCasoConExistencia);
        titulo = "Casos con existencia confirmada";
    } else if (etiqueta === "No existe") {
        filtrados = datosResultadosActuales.filter(esCasoSinExistencia);
        titulo = "Casos sin existencia (descartados)";
    } else {
        filtrados = datosResultadosActuales.filter(function (c) { return !esCasoConExistencia(c) && !esCasoSinExistencia(c); });
        titulo = "Casos sin determinar";
    }

    abrirDetalle(titulo, filtrados.length + " caso(s)", columnasResultadosDetalle, filasResultadosDetalle(filtrados));
}
