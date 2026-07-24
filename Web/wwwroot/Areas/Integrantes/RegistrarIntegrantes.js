
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



$("#btnNuevoReincidente").on("click", function (e) {
    e.preventDefault();

    $('#Modal_InsReincidente').modal("show");
});




$("#btnAddReincidente").on("click", function (e) {
    e.preventDefault();
    P_InsOrUpdReincidente();
  
});

$("#btnUpdReincidente").on("click", function (e) {

    Swal.fire({
        title: '¿Está seguro?',
        text: 'Esta acción actualizará el registro?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Sí, Actualizar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            P_UpdReincidente();
        }
    });

  
});


$("#btnConsultarReincidente").on("click", function (e) {
    e.preventDefault();

    F_GetReincidentesPorId($("#txtIdentificacionReinc").val());
});


$("#btnLimpiarInteg").on("click", function (e) {
    e.preventDefault();

    Limpiar();
});




function F_GetReincidentes() {

    $.ajax({
        url: AppRoutes.RegistroReincidentes.UrlGetInfoGrila,
        type: "GET",
        success: function (respuesta) {
           /* console.log(respuesta.data);*/
            //console.log("✅Respuesta exitosa:", respuesta);

            let data = respuesta.data || [];
            GetGrillaReincidentes(data)

        },
        error: function (err) {
            console.error("ERROR:", err);
            GetGrillaReincidentes([]);
        }
    });
}



// renderDataTable ahora vive en /js/IniciarTabla.js (compartida por todas las grillas del sitio).

function GetGrillaReincidentes(Datos) {
   // const datosFiltrados = Datos.filter(item => [2, 3, 4].includes(item.IdEstado));
    $("#pn_GrillaReincidentes").removeClass('hidden');

    renderDataTable("#tbGrillaReincidentes", Datos, [
        {
            data: null, className: "celdaCenter celda3", "render": function (data, type, row) {
                var DatosFila = JSON.stringify(row).replace(/"/g, '&quot;');
                var inicioBoton = '<div class="dropdown dropend"><button class="btn btn-success" type="button" id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false"><span class="fas fa-list"></span></button><ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';
                var Actualizar = `<li style="padding-left: 17px;"><a style="color: #102717;" href="javascript:OpenModalActualizar(${DatosFila})"><i class="fa fa-rotate-right green"></i>&nbsp;Actualizar</a></li>`;
                var Eliminar = `<li style="padding-left: 17px;"><a style="color: #102717;" href="javascript:P_DellReincidente('${row.reincidenteId}')"><i class="fa fa-trash red"></i>&nbsp;Eliminar</a></li>`;

                var finBoton = '</ul></div>';
                return inicioBoton + Actualizar + Eliminar + finBoton;
            }
        },

        { title: "Tipo reincidencia", data: "tipoId", class: " celda7" },
        { title: "Nombre", data: "nombre", class: " celda10" },
        { title: "Apellido", data: "apellido", class: " celda10" },
        { title: "Identificación", data: "identificacion", class: "celdaCenter celda6" },
        { title: "Alias", data: "alias", class: " celda10" },
        { title: "Observación", data: "observacion" },

        { title: "ReincidenteId", data: "reincidenteId", visible: false }
    ]);
}



$("#btnExcel").on("click", function () {
    let filtro = $(".dataTables_filter input").val() || "";
    window.location.href = "Integrantes/RegistrarInteg/ExportarExcel?filtro=" + encodeURIComponent(filtro);
});

$("#btnPdf").on("click", function () {
    let filtro = $(".dataTables_filter input").val() || "";
    window.open("Integrantes/RegistrarInteg/ExportarPdf?filtro=" + encodeURIComponent(filtro), "_blank");
});



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

                $("#txtReincidenteID").val("");
                $("#txtIdentificacionReinc").val("");
                $("#txtAliasReinc").val("");
                $("#txtNombreReinc").val("");
                $("#txtApellidosReinc").val("");
                $("#txtObservacionesReinc").val("");

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


function OpenModalActualizar(Reincidente) {

    $("#txtReincidenteIDUpd").val(Reincidente.reincidenteId);
    $("#txtAliasReincUpd").val(Reincidente.alias);
    $("#txtObservacionesReincUpd").val(Reincidente.observacion);
    $('#Modal_UpdReincidente').modal("show");
}

function P_UpdReincidente() {

    const Obj_Reincidente = {
        
        reincidenteId: $("#txtReincidenteIDUpd").val(),
        alias: $("#txtAliasReincUpd").val(),
        observacion: $("#txtObservacionesReincUpd").val(),
        idTipo: $("#ddlTipoReincidencia2").val()

    }

    if (!Obj_Reincidente.reincidenteId || !Obj_Reincidente.alias || !Obj_Reincidente.idTipo ) {
        Swal.fire({
            icon: 'warning',
            title: 'Campos obligatorios',
            text: 'Debe diligenciar Tipo Reincidencia y al menos Alias.'
        });
        return; // Detener ejecución si faltan campos
    }

    $.ajax({
        url: AppRoutes.RegistroReincidentes.UrlUpdReincidente,
        type: 'POST',
        data: Obj_Reincidente,
        success: function (resp) {
            if (resp.success) {

                Swal.fire({
                    type: 'success',
                    title: 'Señor(a) Funcionario(a:)',
                    text: resp.message
                });
                $('#Modal_UpdReincidente').modal('hide');

                $("#txtReincidenteIDUpd").val("");
                $("#txtAliasReincUpd").val("");
                $("#txtObservacionesReincUpd").val("");
               
                F_GetReincidentes();

            } else {

                Swal.fire({
                    type: 'error',
                    title: 'Señor(a) Funcionario(a:)',
                    text: 'Error al Actualizar: ' + resp.message
                });
            }
        },
        error: function () {
            Swal.fire('Error', 'Fallo en la llamada AJAX.', 'error');
        }
    });

}



function P_DellReincidente(V_ReincidenteId) {

    const Obj_Reincidente = {

        reincidenteId: V_ReincidenteId

    }


    Swal.fire({
        title: '¿Está seguro?',
        text: 'Esta acción eliminará el reincidente seleccionado.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {


        if (result.isConfirmed) {




            $.ajax({
                url: AppRoutes.RegistroReincidentes.UrlDellReincidente,
                type: 'POST',
                data: Obj_Reincidente,
                success: function (resp) {
                    if (resp.success) {

                        Swal.fire({
                            type: 'success',
                            title: 'Señor(a) Funcionario(a:)',
                            text: resp.message
                        });
               
                        F_GetReincidentes();

                    } else {

                        Swal.fire({
                            type: 'error',
                            title: 'Señor(a) Funcionario(a:)',
                            text: 'Error al Eliminar: ' + resp.message
                        });
                    }
                },
                error: function () {
                    Swal.fire('Error', 'Fallo en la llamada AJAX.', 'error');
                }
            });
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
                    text: (response.message ? response.message + ' - ' : '') + 'La identificación suministrada no se encuentra como Reincidente !!!'
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





//$("#btnPrint").on("click", function () {
//    let filtro = $(".dataTables_filter input").val() || "";
//    window.open("Integrantes/RegistrarInteg/Imprimir?filtro=" + encodeURIComponent(filtro), "_blank");
//});
