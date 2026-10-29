// ============================================================================================
//  Dashboard de Uso y Auditoría (Administración)
//  Consume Admin/AuditoriaUso/GetDashboard y pinta KPIs, 4 gráficas (Chart.js v2) y la tabla de
//  depuración de usuarios. Datos tomados de CTR_AUDITORIA_IRIS vía PK_AUDITORIA_USO.
// ============================================================================================

var chFecha = null, chUnidad = null, chModulo = null, chTopUsuarios = null;
var dtDepuracion = null;
var unidadSelInicializado = false;

var PALETA_AUD = ["#0d6efd", "#08a6cb", "#0ea472", "#f59e0b", "#e11d48", "#6f42c1",
    "#14b8a6", "#a855f7", "#ef4444", "#64748b", "#3b82f6", "#16a34a"];

$(function () {
    cargarDashboard();
    $("#btnAudConsultar").on("click", function () { cargarDashboard(); });
});

function cargarDashboard() {
    var params = {
        fechaIni: $("#fAudIni").val(),
        fechaFin: $("#fAudFin").val(),
        sigla: $("#fAudUnidad").val() || "",
        usuario: $("#fAudUsuario").val() || ""
    };

    $.ajax({
        url: (typeof UrlGetDashboardUso !== "undefined") ? UrlGetDashboardUso : "GetDashboard",
        type: "GET",
        data: params,
        success: function (resp) {
            if (!resp || !resp.success) {
                Swal.fire("Aviso", (resp && resp.message) || "No fue posible cargar el dashboard.", "info");
                return;
            }
            renderKpis(resp.kpis);
            poblarUnidades(resp.porUnidad);
            renderGraficoFecha(resp.porFecha);
            renderGraficoUnidad(resp.porUnidad);
            renderGraficoModulo(resp.porModulo);
            renderGraficoTopUsuarios(resp.topUsuarios);
            renderTablaDepuracion(resp.depuracion);
        },
        error: function () {
            Swal.fire("Error", "Error al consultar el dashboard de uso y auditoría.", "error");
        }
    });
}

function renderKpis(k) {
    k = k || {};
    $("#kpiCreados").text(fmt(k.usuariosCreados));
    $("#kpiActivos").text(fmt(k.usuariosActivos));
    $("#kpiNunca").text(fmt(k.nuncaIngresaron));
    $("#kpiAdopcion").text((k.adopcionPct != null ? k.adopcionPct : 0) + "%");
    $("#kpiIngresos").text(fmt(k.totalIngresos));
    $("#kpiModulos").text(fmt(k.modulosDistintos));
}

function fmt(n) { return (n == null ? 0 : n).toLocaleString("es-CO"); }

// Combo de unidad: se llena una sola vez con las siglas que tienen actividad (sin filtro).
function poblarUnidades(porUnidad) {
    if (unidadSelInicializado) return;
    var $sel = $("#fAudUnidad");
    (porUnidad || []).forEach(function (u) {
        if (u.sigla && u.sigla !== "-") {
            $sel.append('<option value="' + u.sigla + '">' + u.sigla + (u.unidad ? " - " + u.unidad : "") + "</option>");
        }
    });
    unidadSelInicializado = true;
}

function destruir(ch) { if (ch) { try { ch.destroy(); } catch (e) { } } }

function renderGraficoFecha(data) {
    var el = document.getElementById("graficoPorFecha"); if (!el) return;
    var labels = (data || []).map(function (d) { return formatearDia(d.dia); });
    var eventos = (data || []).map(function (d) { return d.totalEventos; });
    var ingresos = (data || []).map(function (d) { return d.totalIngresos; });
    destruir(chFecha);
    chFecha = new Chart(el, {
        type: "line",
        data: {
            labels: labels,
            datasets: [
                { label: "Eventos", data: eventos, borderColor: "#08a6cb", backgroundColor: "rgba(8,166,203,.15)", fill: true, pointRadius: 2, borderWidth: 2 },
                { label: "Ingresos (logins)", data: ingresos, borderColor: "#0d6efd", backgroundColor: "rgba(13,110,253,.10)", fill: true, pointRadius: 2, borderWidth: 2 }
            ]
        },
        options: {
            responsive: true, maintainAspectRatio: false,
            legend: { position: "bottom" },
            scales: { yAxes: [{ ticks: { beginAtZero: true, precision: 0 } }] }
        }
    });
}

function renderGraficoUnidad(data) {
    var el = document.getElementById("graficoPorUnidad"); if (!el) return;
    var top = (data || []).slice(0, 10);
    var labels = top.map(function (u) { return u.sigla || "-"; });
    var valores = top.map(function (u) { return u.cantidad; });
    var colores = top.map(function (_, i) { return PALETA_AUD[i % PALETA_AUD.length]; });
    destruir(chUnidad);
    chUnidad = new Chart(el, {
        type: "doughnut",
        data: { labels: labels, datasets: [{ data: valores, backgroundColor: colores, borderWidth: 2, borderColor: "#fff" }] },
        options: {
            responsive: true, maintainAspectRatio: false, cutoutPercentage: 62,
            legend: { position: "right", labels: { fontSize: 11, boxWidth: 12 } }
        }
    });
}

function renderGraficoModulo(data) {
    var el = document.getElementById("graficoPorModulo"); if (!el) return;
    var top = (data || []).slice(0, 12);
    var labels = top.map(function (m) { return m.modulo; });
    var valores = top.map(function (m) { return m.cantidad; });
    destruir(chModulo);
    chModulo = new Chart(el, {
        type: "horizontalBar",
        data: { labels: labels, datasets: [{ label: "Ingresos", data: valores, backgroundColor: "#6f42c1" }] },
        options: {
            responsive: true, maintainAspectRatio: false, legend: { display: false },
            scales: { xAxes: [{ ticks: { beginAtZero: true, precision: 0 } }] }
        }
    });
}

function renderGraficoTopUsuarios(data) {
    var el = document.getElementById("graficoTopUsuarios"); if (!el) return;
    var top = (data || []).slice(0, 12);
    var labels = top.map(function (u) { return String(u.identificacion); });
    var valores = top.map(function (u) { return u.totalEventos; });
    destruir(chTopUsuarios);
    chTopUsuarios = new Chart(el, {
        type: "horizontalBar",
        data: { labels: labels, datasets: [{ label: "Eventos", data: valores, backgroundColor: "#0ea472" }] },
        options: {
            responsive: true, maintainAspectRatio: false, legend: { display: false },
            scales: { xAxes: [{ ticks: { beginAtZero: true, precision: 0 } }] }
        }
    });
}

function renderTablaDepuracion(filas) {
    filas = filas || [];
    if ($.fn.dataTable.isDataTable("#tbDepuracion")) {
        dtDepuracion.clear().rows.add(filas).draw();
        return;
    }
    dtDepuracion = $("#tbDepuracion").DataTable({
        data: filas,
        language: (typeof glOpcionesIdioma !== "undefined") ? glOpcionesIdioma : {},
        pageLength: 10,
        order: [[4, "asc"]],
        columns: [
            { title: "Funcionario", data: "funcionario", defaultContent: "" },
            { title: "Cédula", data: "identificacion" },
            { title: "Usuario", data: "usuario", defaultContent: "" },
            { title: "Unidad", data: "unidad", defaultContent: "" },
            { title: "# Ingresos", data: "totalIngresos", className: "text-center" },
            { title: "Módulos", data: "modulosDistintos", className: "text-center" },
            { title: "Último ingreso", data: "ultimoIngreso", render: function (d) { return formatearFechaHora(d); } },
            { title: "Días sin ingresar", data: "diasSinIngresar", className: "text-center", render: function (d) { return d == null ? "—" : d; } },
            {
                title: "Estado", data: "estado", className: "text-center",
                render: function (d) {
                    var cls = "bg-success";
                    if (d === "Nunca ingresó") cls = "bg-danger";
                    else if (d && d.indexOf("Inactivo") === 0) cls = "bg-warning text-dark";
                    return '<span class="badge ' + cls + '">' + (d || "") + "</span>";
                }
            }
        ]
    });
}

function formatearDia(iso) {
    if (!iso) return "";
    var d = new Date(iso);
    if (isNaN(d)) return iso;
    return ("0" + d.getDate()).slice(-2) + "/" + ("0" + (d.getMonth() + 1)).slice(-2) + "/" + d.getFullYear();
}

function formatearFechaHora(iso) {
    if (!iso) return "—";
    var d = new Date(iso);
    if (isNaN(d)) return "—";
    return ("0" + d.getDate()).slice(-2) + "/" + ("0" + (d.getMonth() + 1)).slice(-2) + "/" + d.getFullYear() +
        " " + ("0" + d.getHours()).slice(-2) + ":" + ("0" + d.getMinutes()).slice(-2);
}
