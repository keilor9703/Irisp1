$(document).ready(function () {

    F_GetSlaConfig();

    $("#btnNuevaSla").on("click", function () {
        LimpiarFormSla();
        $("#ModalSlaLabel").text("Nueva configuración de SLA");
        $("#ModalSla").modal("show");
    });

    $("#btnGuardarSla").on("click", function (e) {
        e.preventDefault();
        P_InsUpdSlaConfig();
    });
});

function LimpiarFormSla() {
    $("#SlaConfigId").val("");
    $("#ddlTipoTarea").val("");
    $("#txtTiempoAlertaHoras").val("");
    $("#txtTiempoMaximoHoras").val("");
    $("#txtJustificacionSla").val("");
}

function F_GetSlaConfig() {
    $.ajax({
        type: "GET",
        url: UrlGetSlaConfig,
        dataType: "json",
        success: function (response) {
            renderTablaSla((response && response.data) ? response.data : []);
        },
        error: function () {
            renderTablaSla([]);
        }
    });
}

function renderTablaSla(data) {
    var filas = data.map(function (s) {
        var acciones =
            '<button type="button" class="btn btn-sm btn-primary" onclick="EditarSla(\'' + s.SlaConfigId + '\')"><i class="fas fa-edit"></i></button> ' +
            '<button type="button" class="btn btn-sm btn-danger" onclick="EliminarSla(\'' + s.SlaConfigId + '\')"><i class="fas fa-trash"></i></button>';

        return [s.DescTipoTarea || "", s.TiempoAlertaHoras, s.TiempoMaximoHoras, s.Justificacion || "", acciones];
    });

    window.slaConfigData = data;

    if ($.fn.dataTable.isDataTable("#tbSlaConfig")) {
        var tabla = $("#tbSlaConfig").DataTable();
        tabla.clear();
        tabla.rows.add(filas);
        tabla.draw();
        return;
    }

    $("#tbSlaConfig").DataTable({
        data: filas,
        language: glOpcionesIdioma,
        columns: [
            { title: "Tipo de tarea" },
            { title: "Alerta (horas)" },
            { title: "Máximo (horas)" },
            { title: "Justificación" },
            { title: "Acciones", orderable: false }
        ]
    });
}

function EditarSla(slaConfigId) {
    var item = (window.slaConfigData || []).find(function (s) { return s.SlaConfigId === slaConfigId; });
    if (!item) return;

    $("#SlaConfigId").val(item.SlaConfigId);
    $("#ddlTipoTarea").val(item.IdListaTareas);
    $("#txtTiempoAlertaHoras").val(item.TiempoAlertaHoras);
    $("#txtTiempoMaximoHoras").val(item.TiempoMaximoHoras);
    $("#txtJustificacionSla").val(item.Justificacion);

    $("#ModalSlaLabel").text("Editar configuración de SLA");
    $("#ModalSla").modal("show");
}

function P_InsUpdSlaConfig() {
    var data = {
        IdListaTareas: $("#ddlTipoTarea").val(),
        TiempoAlertaHoras: $("#txtTiempoAlertaHoras").val(),
        TiempoMaximoHoras: $("#txtTiempoMaximoHoras").val(),
        Justificacion: $("#txtJustificacionSla").val()
    };

    $.ajax({
        type: "POST",
        url: UrlInsUpdSlaConfig,
        data: data,
        dataType: "json",
        success: function (response) {
            if (response && response.success) {
                $("#ModalSla").modal("hide");
                F_GetSlaConfig();
            }
            alert(response.message);
        },
        error: function () {
            alert("Error: no fue posible guardar, intente nuevamente.");
        }
    });
}

function EliminarSla(slaConfigId) {
    if (!confirm("¿Desea eliminar esta configuración de SLA?")) return;

    $.ajax({
        type: "POST",
        url: UrlDelSlaConfig,
        data: { slaConfigId: slaConfigId },
        dataType: "json",
        success: function (response) {
            if (response && response.success) {
                F_GetSlaConfig();
            }
            alert(response.message);
        },
        error: function () {
            alert("Error: no fue posible eliminar, intente nuevamente.");
        }
    });
}
