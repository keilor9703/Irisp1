
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


    F_GetReincidentes();
});

$("#txtIdentificacionReinc").keyup(function (event) {
    if (event.keyCode === 13) {
        $("#btnConsultarReincidente").click();
    }


});


$("#btnAddReincidente").on("click", function (e) {
    e.preventDefault();
    P_InsOrUpdReincidente();
});


$("#btnConsultarReincidente").on("click", function (e) {
    e.preventDefault();

    F_GetReincidentesPorId($('#txtIdentificacionReinc').val());
});


$("#btnNuevoReincidente").on("click", function (e) {
    e.preventDefault();

    $('#Modal_InsReincidente').modal("show");
});


$("#btnLimpiarInteg").on("click", function (e) {
    e.preventDefault();

    Limpiar();
});


function F_GetReincidentes() {

    $.ajax({
        url: AppRoutes.RegistroReincidentes.UrlGetInfoGrila,
        type: "POST",
        success: function (respuesta) {
           /* console.log(respuesta.data);*/
            console.log("✅Respuesta exitosa:", respuesta);

            let data = respuesta.data || [];
            GetGrillaReincidentes(data)

        },
        error: function (err) {
            console.error("ERROR:", err);
            GetGrillaReincidentes([]);
        }
    });
}

function GetGrillaReincidentes(Datos) {


   if ($.fn.dataTable.isDataTable('#tbGrillaReincidentes')) {
        const table = $('#tbGrillaReincidentes').DataTable();
        table.clear();
        table.rows.add(Datos);
        table.draw(false);
        return;
    }


    $("#pn_GrillaReincidentes").removeClass('hidden');
    $("#tbGrillaReincidentes").DataTable({
        // destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        scrollX: true,          // ✅ Activa scroll horizontal
        scrollCollapse: true,   // ✅ Permite colapsar si hay menos columnas
        responsive: false,      // ✅ Desactiva comportamiento que oculta columnas
        autoWidth: false,       // ✅ Evita cálculos automáticos de ancho que rompen el scroll
        "columns": [
            {
                data: null, className: "celdaCenter celda3", "render": function (data, type, row) {
                    var inicioBoton = '<div class="dropdown dropend"><button class="btn btn-success" type="button" id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false"><span class="fas fa-list"></span></button><ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';
                    var Actualizar = `<li style="padding-left: 17px;"><a style="color: #102717;" href="javascript:ModalActualizar(${row.IdUserRol})"><i class="fa fa-rotate-right green"></i>&nbsp;Actualizar</a></li>`;
                    var Eliminar = `<li style="padding-left: 17px;"><a style="color: #102717;" href="javascript:Dell_Roles(${row.IdUserRol})"><i class="fa fa-trash red"></i>&nbsp;Eliminar</a></li>`;
                    
                    var finBoton = '</ul></div>';
                    return inicioBoton + Actualizar + Eliminar + finBoton;
                }
            },

            { title: "Tipo reincidencia", data: "tipoId", class: " celda7" },
            { title: "Nombre", data: "nombre", class: " celda10" },
            { title: "Apellido", data: "apellido"   ,class: " celda10" },
            { title: "Identificación", data: "identificacion", class: "celdaCenter celda6" },
            { title: "Alias", data: "alias", class: " celda10" },
            { title: "Observación", data: "observacion" },

            { title: "ReincidenteId", data: "reincidenteId", visible: false}
        ],
        lengthMenu: [
            [15, 25, 50, -1],
            ['15 registros', '25 registros', '50 registros', 'Todos']
        ],
        ordering: false,
        pageLength: 15,
        bLengthChange: true,
        searching: true,
        paging: true,
        info: true,

    });
}

function P_InsOrUpdReincidente() {

    const Obj_Reincidente = {
        Identificacion: $("#txtIdentificacionReinc").val(),
        alias: $("#txtAliasReinc").val(),
        Nombre: $("#txtNombreReinc").val(),
        Apellido: $("#txtApellidosReinc").val(),
        observacion: $("#txtObservacionesReinc").val(),
        idTipo: $("#ddlTipoReincidencia").val()
    }

    if (!Obj_Reincidente.idTipo || !Obj_Reincidente.Identificacion || (!Obj_Reincidente.alias && !Obj_Reincidente.Nombre)) {
        Swal.fire({
            icon: 'warning',
            title: 'Campos obligatorios',
            text: 'Debe diligenciar Tipo Reincidencia y al menos Nombre o Alias.'
        });
        return; // Detener ejecución si faltan campos
    }

    $.ajax({
        url: AppRoutes.RegistroReincidentes.UrlInsOrUpdReincidente,
        type: 'POST',
        data: Obj_Reincidente,
        success: function (resp) {
            if (resp.success) {

                Swal.fire({
                    type: 'success',
                    title: 'Señor(a) Funcionario(a:)',
                    text: resp.message
                });
                $('#Modal_InsReincidente').modal('hide');

                $("#txtReincidenteID").val('');
                $("#txtIdentificacionReinc").val('');
                $("#txtAliasReinc").val('');
                $("#txtNombreReinc").val('');
                $("#txtApellidosReinc").val('');
                $("#txtObservacionesReinc").val('');

                F_GetReincidentes();

            } else {

                Swal.fire({
                    type: 'error',
                    title: 'Señor(a) Funcionario(a:)',
                    text: 'Error al insertar: ' + resp.message
                });
            }
        },
        error: function () {
            Swal.fire('Error', 'Fallo en la llamada AJAX.', 'error');
        }
    });

}


function F_GetReincidentesPorId(P_Identificacion) {
    $.ajax({
        type: 'GET',
        url: AppRoutes.RegistroReincidentes.UrlGetReincidentes, // Endpoint que devuelve los datos
        dataType: 'json',
        data: { V_Identificacion: P_Identificacion },
        success: function (response) {
            if (response.success) {
                let data = response.data || [];

                
                $("#txtAliasReinc").val(data[0].alias);
                $("#txtNombreReinc").val(data[0].nombre);
                $("#txtApellidosReinc").val(data[0].apellido);
                $("#txtReincidenteID").val(data[0].reincidenteId);
                

                $("#txtAliasReinc")
                    .addClass("readonly")
                    .prop("readonly", true);

                $("#txtNombreReinc")
                    .addClass("readonly")
                    .prop("readonly", true);

                $("#txtApellidosReinc")
                    .addClass("readonly")
                    .prop("readonly", true);


            } else {


                $("#txtAliasReinc").val('');
                $("#txtNombreReinc").val('');
                $("#txtApellidosReinc").val('');
                $("#txtReincidenteID").val('');
                    
                
                $("#txtAliasReinc")
                    .removeClass("readonly")
                    .prop("readonly", false);

                $("#txtNombreReinc")
                    .removeClass("readonly")
                    .prop("readonly", false);


                $("#txtApellidosReinc")
                    .removeClass("readonly")
                    .prop("readonly", false);


                Swal.fire({
                    icon: 'error',
                    title: 'Señor(a) Funcionario(a):',
                    text: (response.message ? response.message + ' - ' : '') + 'La identificación suministrada no se encuentra relacionada en Base de Datos !!!'
                });

            }
        },
        error: function (xhr, status, error) {
            console.error("Error en la solicitud AJAX:", status, error);

            Swal.fire({
                icon: 'error',
                title: 'Error de conexión',
                text: 'Ocurrió un error al intentar obtener los datos del integrante.'
            });
        }
    });
}


function Limpiar() {

    $("#txtIdentificacionReinc").val("");
    $("#txtAliasReinc").val("");
    $("#txtNombreReinc").val("");
    $("#txtApellidosReinc").val("");
    $("#txtObservacionesReinc").val("");
    $("#txtReincidenteID").val("");
    

    $("#txtAliasReinc")
        .removeClass("readonly")
        .prop("readonly", false);

    $("#txtNombreReinc")
        .removeClass("readonly")
        .prop("readonly", false);


    $("#txtApellidosReinc")
        .removeClass("readonly")
        .prop("readonly", false);


}


