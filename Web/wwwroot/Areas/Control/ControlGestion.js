$(document).ready(function () {

    $('#ddlAnioIris').select2({ placeholder: "Seleccione", allowClear: true });

    $("#ddlAnioIris").on("change", function () {
        CargarTablero();
    });

    CargarTablero();
});

var chartControlGestion = null;

function CargarTablero() {
    F_GetTareasControlGestion();
    F_GetKpisTiempoGestion();
    RenderRangoFechas();
}

// El tablero filtra siempre por año calendario completo (01/01 - 31/12), o hasta hoy si es el año
// en curso — este es exactamente el rango que aplica el filtro P_Anio en Oracle, así que se calcula
// aquí mismo en vez de pedirlo al backend.
function RenderRangoFechas() {
    var anio = parseInt($("#ddlAnioIris").val(), 10);
    if (!anio) {
        $("#rangoFechasTablero").text("");
        return;
    }

    var hoy = new Date();
    var inicio = new Date(anio, 0, 1);
    var fin = (anio === hoy.getFullYear()) ? hoy : new Date(anio, 11, 31);

    $("#rangoFechasTablero").text("Periodo del reporte: " + formatearFecha(inicio) + " — " + formatearFecha(fin));
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

function F_GetTareasControlGestion() {
    var anio = $("#ddlAnioIris").val();

    $.ajax({
        type: "GET",
        url: UrlGetTareasControlGestion,
        data: { V_Anio: anio },
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
    var anio = $("#ddlAnioIris").val();

    $.ajax({
        type: "GET",
        url: UrlGetKpisTiempoGestion,
        data: { V_Anio: anio },
        dataType: "json",
        success: function (response) {
            renderKpisTiempoGestion(response && response.success === true ? response.kpis : null);
        },
        error: function () {
            renderKpisTiempoGestion(null);
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

    if (chartControlGestion) {
        chartControlGestion.data.labels = etiquetas;
        chartControlGestion.data.datasets[0].data = valores;
        chartControlGestion.update();
        return;
    }

    chartControlGestion = new Chart(ctx, {
        type: "bar",
        data: {
            labels: etiquetas,
            datasets: [{
                label: "Promedio de horas",
                data: valores,
                backgroundColor: "#0d6efd"
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            legend: { display: false },
            scales: {
                yAxes: [{
                    ticks: { beginAtZero: true },
                    scaleLabel: { display: true, labelString: "Horas" }
                }],
                xAxes: [{
                    scaleLabel: { display: true, labelString: "Sigla de unidad" }
                }]
            }
        }
    });
}
