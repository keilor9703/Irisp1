$(document).ready(function () {

    $("#ddlAnioIris").on("change", function () {
        F_GetTareasControlGestion();
    });

    F_GetTareasControlGestion();
});

var chartControlGestion = null;

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
            t.Unidad || "",
            t.DescListaTarea || "",
            t.DescEstadoTarea || "",
            (t.HorasTranscurridas != null ? t.HorasTranscurridas : "-"),
            badgeEstadoSla(t.EstadoSla)
        ];
    });

    if ($.fn.dataTable.isDataTable("#tbTareasControlGestion")) {
        var tabla = $("#tbTareasControlGestion").DataTable();
        tabla.clear();
        tabla.rows.add(filas);
        tabla.draw();
        return;
    }

    $("#tbTareasControlGestion").DataTable({
        data: filas,
        language: glOpcionesIdioma,
        columns: [
            { title: "Código IRISP1" },
            { title: "Unidad" },
            { title: "Tarea" },
            { title: "Estado tarea" },
            { title: "Horas transcurridas" },
            { title: "SLA" }
        ]
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

function renderGraficoPorUnidad(promedioPorUnidad) {
    var ctx = document.getElementById("graficoPromedioUnidad");
    if (!ctx) return;

    var etiquetas = (promedioPorUnidad || []).map(function (u) { return u.unidad; });
    var valores = (promedioPorUnidad || []).map(function (u) { return u.promedioHoras; });

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
                label: "Promedio de horas por unidad",
                data: valores,
                backgroundColor: "#0d6efd"
            }]
        },
        options: {
            responsive: true,
            legend: { display: false }
        }
    });
}
