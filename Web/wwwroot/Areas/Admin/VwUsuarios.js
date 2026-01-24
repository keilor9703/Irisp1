$(document).ready(function () {
    //Manejo de fechas con Kendo
    $(".Calendario").kendoDatePicker({
        culture: "es-CO",
        interval: 1,
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
    //Fin Manejo de fechas Kendo



    $("#btnConsultarEmpl").on("click", function (e) {
        e.preventDefault();

        F_GetFuncionarios($('#txtIdentificacion').val());
    });


    $("#BtnLimpiarEmpl").on("click", function (e) {
        e.preventDefault();

        Limpiar();
    });

    $("#btnGrabar").on("click", function (e) {
        e.preventDefault();

        P_InsUdpUsuarios();
    });
    $("#btnListaUsuarios").on("click", function (e) {
        e.preventDefault();

        GetGrillaUsuarios();
    });
    $("#btnLimpiar").on("click", function (e) {
        e.preventDefault();

        Limpiar();
    }); $("#btnGrabarRol").on("click", function (e) {
        e.preventDefault();

        P_InsRoles();
    });



});

//Eventos
$("#txtIdentificacion").keyup(function (event) {
    if (event.keyCode === 13) {
        $("#btnConsultarEmpl").click();
    }
});
$("#txtFuncionario").autocomplete({
    source: function (request, response) {
        $.ajax({
            url: AppRoutes.Administracion.UrlGetEmpleadoIntel,
            type: "GET",
            dataType: "json",
            data: { V_Busqueda: $("#txtFuncionario").val() },
            success: function (respuesta) {
                response($.map(respuesta.data, function (item) {
                    return { label: item.Funcionario, value: item.Identificacion };
                }));
            },
            error: function (respuesta) {
                alert("Error");
            }
        });
    },
    minLength: 10,
    select: function (event, ui) {
        $(document.getElementById("txtFuncionario")).val(ui.item.label);
        $('#txtIdentificacion').val(ui.item.value);
        F_GetFuncionarios(ui.item.value);
        return false;
    }
});
//Fuciones de Consulta
function GetGrillaUsuarios() {
    if ($.fn.dataTable.isDataTable("#tbGrillaUsuarios")) {
        $("#tbGrillaUsuarios").DataTable().destroy();
    }
    $("#tbGrillaUsuarios").DataTable({
        "ajax": {
            type: "POST",
            url: AppRoutes.Administracion.UrlGetListUsuarios,
            async: true,
            datatype: "json",
            cache: false
        },
        "initComplete": function (settings, json) {
            if (json.success) {
                $("#pn_GrillaUsuarios").removeClass('hidden');
            }
            else {
                $("#pn_GrillaUsuarios").addClass('hidden');
            }
        },
        language: glOpcionesIdioma,
        responsive: true,
        "columns": [
            { "title": "Grado", "data": "GradAlfabetico", "name": "GradAlfabetico", className: "celdaCenter celda2" },
            { "title": "Funcionario", "data": "Funcionario", "name": "Funcionario", className: "celda15" },
            { "title": "Identificación", "data": "Identificacion", "name": "Identificacion", className: "celdaCenter celda5" },
            { "title": "Cargo", "data": "Cargo", "name": "Cargo", className: "celdaCenter" },

            // NUEVO
            {
                "title": "Roles",
                "data": "Roles",
                "name": "Roles",
                className: "celdaCenter",
                "render": function (data, type, row) {
                    if (!data) return "SIN ROLES";
                    return data; // viene como "ROL1 | ROL2 | ROL3"
                }
            }
        ],

        ordering: true,
        pageLength: 10,
        bLengthChange: true,
        searching: true,
        paging: true,
        info: true
    });
}

function F_GetFuncionarios(V_IdentificacionB){

    let Identificacion = Number(V_IdentificacionB);
    if (Identificacion < 1) {
        create('error', 'Debe digitar número de Identificación', '../../img/AlertError.png');
        return;
    }

    if (!V_IdentificacionB || V_IdentificacionB.length < 6) {
        $("#imgFoto").attr("src", "/img/Avatar.png");
        return;
    }


    $.ajax({
        type: "GET",
        url: AppRoutes.Administracion.UrlGetFuncionarios,
        async: true,
        data: { V_Identificacion: V_IdentificacionB },
        dataType: 'json',
        cache: false,
        success: function (respuesta) {
            if (respuesta.success) {


                $("#imgFoto").attr(
                    "src",
                    "Cuenta/FotoFuncionario?identificacion=" + V_IdentificacionB + "&t=" + new Date().getTime()
                );

                $("#txtSituacionLab").val(respuesta.data.SituacionLaboral);
                $("#txtFuncionario").val(respuesta.data.Funcionario);
                $("#txtCorreo").val(respuesta.data.Correo);
                $("#txtUserName").val(respuesta.data.Usuario);
                $("#txtCelular").val(respuesta.data.Celular);
                $("#txtDependencia").val(respuesta.data.Fisica + " - " + respuesta.data.Dependencia);
                $("#txtCargo").val(respuesta.data.Cargo);

                F_GetUserRoles(V_IdentificacionB);
            } else {
                Limpiar();
                Swal.fire({
                    type: 'error',
                    title: 'Señor(a) Funcionario(a:)',
                    text: "No se Encontro el Funcionario"
                });
            }
        },
        error: function () {
            Swal.fire({
                type: 'error',
                title: 'Señor(a) Funcionario(a:)',
                text: 'No es posible consultar, revise!!'
            });
        }
    });
}
function F_GetUserRoles(P_Identificacion) {
    $.ajax({
        type: "POST",
        url: AppRoutes.Administracion.UrlGetUserRoles,
        async: true,
        data: { V_Identificacion: P_Identificacion },
        dataType: 'json',
        cache: false,
        success: function (respuesta) {
            if (respuesta.success) {
                $("#pn_Grilla").removeClass('hidden');
                $("#pn_Roles").removeClass('hidden');

                $("#txtIdUsuario").val(respuesta.data[0].IdUsuario);

                GrillaUserRoles(respuesta.data);

                var uno = document.getElementById('btnGrabar');
                uno.innerHTML = '<span class="fa ico_grabar faa-wrench animated"></span>Actualizar';		

                var _Bloqueado = respuesta.data[0].Bloqueado;
                var Activo = 0;
                if (_Bloqueado == 0) {
                    Activo = 1;
                }
                else {
                    Activo = 0;
                }

                $('#chkActivo').prop('checked', Activo).trigger('change');


                F_GetEstadoMfa(P_Identificacion, respuesta.data[0].UsuarioInst );


            } else {
                $("#pn_Grilla").addClass('hidden');
                $("#pn_Roles").addClass('hidden');

                var uno = document.getElementById('btnGrabar');
                uno.innerHTML = '<span class="fa ico_grabar faa-wrench animated"></span>Guardar';

                Swal.fire({
                    type: 'info',
                    title: 'Señor(a) Funcionario(a:)',
                    text: "El usuario no ha sido creado en el sistema, proceda activarlo"
                });
            }
        },
        error: function () {
            Swal.fire({
                type: 'error',
                title: 'Señor(a) Funcionario(a:)',
                text: 'No es posible consultar, revise!!'
            });
        }
    });
}


function F_GetEstadoMfa(P_Identificacion, P_Usuario) {
    $.ajax({
        type: "POST",
        url: AppRoutes.Administracion.UrlGetEstadoMfa,
        async: true,
        data: { V_Identificacion: P_Identificacion, V_Usuario: P_Usuario },
        dataType: 'json',
        cache: false,
        success: function (respuesta) {
            if (respuesta.success) {
               
                var _Habilitado = respuesta.data.EstadoMfa;

                $('#chkMfaActivo').prop('checked', _Habilitado).trigger('change');


            } else {
               
            }
        },
        error: function () {
            Swal.fire({
                type: 'error',
                title: 'Señor(a) Funcionario(a:)',
                text: 'No es posible consultar MFA, revise!!'
            });
        }
    });
}
//Funciones de Inserción y Actualización     AISGNAR ROLES ADMINISTRACION DE USUARIOS
function P_InsRoles() {

    if ($("#txtJustificacion").val() == "") {
        create('error', 'Debe registrar justificación para asignar el rol', '../../img/AlertError.png');
        return;
    }

    var DtoInsUserRoles = {
        IdUsuario: $("#txtIdUsuario").val(),
        IdRol: $("#ddlRol").val(),
        FechaFin: $("#txtFechaFin").val(),
        Justificacion: $("#txtJustificacion").val()
    }

    $.ajax({
        type: 'POST',
        url: AppRoutes.Administracion.UrlInsRoles,
        async: true,
        dataType: 'json',
        data: { obj: DtoInsUserRoles },
        success: function (result) {
            if (result.success) {
                F_GetUserRoles($("#txtIdentificacion").val());
                LimpiarRoles();
                Swal.fire({
                    type: 'success',
                    title: 'Señor(a) Funcionario(a:)',
                    text: result.message
                });

            } else {
                Swal.fire({
                    type: 'error',
                    title: 'Señor(a) Funcionario(a:)',
                    text: result.message
                });
            }
        },
        error: function (ex) {
            Swal.fire({
                type: 'error',
                title: 'Señor(a) Funcionario(a:)',
                text: "No es posible grabar, revise"
            });
        }
    });
}
//function P_InsUdpUsuarios() {

//    var _Bloq = 0;
//    var _2FA = 0;
//    let _chkBlq = chkActivo.checked;
//    if (_chkBlq == true) {
//        _Bloq = 0;
//    } else {
//        _Bloq = 1;
//    }
//    let _chk2FA = chkMfaActivo.checked;
//    if (_chk2FA == true) {
//        _2FA = 1;
//    } else {
//        _2FA = 0;
//    }

//    var DtoUsuario = {
//        Identificacion: $("#txtIdentificacion").val(),
//        Bloqueado: _Bloq,
//        Estado2Fa: _2FA,
//        Usuario: $("#txtUserName").val(),
//    }

//    $.ajax({
//        type: 'POST',
//        url: AppRoutes.Administracion.UrlInsUdpUsuarios,
//        async: true,
//        dataType: 'json',
//        data: { obj: DtoUsuario },
//        success: function (respuesta) {
//            if (respuesta.success) {

//                $("#txtIdUsuario").val(respuesta.data);
//                $("#pn_Roles").removeClass('hidden');

//                var uno = document.getElementById('btnGrabar');
//                uno.innerHTML = '<span class="fa ico_grabar faa-wrench animated"></span>Actualizar';

//                Swal.fire({
//                    type: 'success',
//                    title: 'Señor(a) Funcionario(a:)',
//                    text: respuesta.message + ", ahora revise roles del sistema"
//                });

//            } else {
//                $("#pn_Roles").addClass('hidden');
//                Swal.fire({
//                    type: 'error',
//                    title: 'Señor(a) Funcionario(a:)',
//                    text: respuesta.message
//                });
//            }
//        },
//        error: function (ex) {
//            $("#txtIdUsuario").val(0);
//            Swal.fire({
//                type: 'error',
//                title: 'Señor(a) Funcionario(a:)',
//                text: "No es posible grabar, revise"
//            });
//        }
//    });
//}


function P_InsUdpUsuarios() {

    var _Bloq = 0;
    var _2FA = null; // importante: null = "no cambiar MFA"

    const chkActivoEl = document.getElementById('chkActivo');
    const chkMfaEl = document.getElementById('chkMfaActivo'); // puede ser null si no es rol 1 super usuario

    // Bloqueo (siempre existe)
    _Bloq = (chkActivoEl && chkActivoEl.checked) ? 0 : 1;

    // MFA
    if (chkMfaEl == null) {
        _2FA = 1;
    } else {
        _2FA = chkMfaEl.checked ? 1 : 0;
    }

    var DtoUsuario = {
        Identificacion: $("#txtIdentificacion").val(),
        Bloqueado: _Bloq,
        Estado2Fa: _2FA,  
        Usuario: $("#txtUserName").val(),
    };

    $.ajax({
        type: 'POST',
        url: AppRoutes.Administracion.UrlInsUdpUsuarios,
        async: true,
        dataType: 'json',
        data: { obj: DtoUsuario },
        success: function (respuesta) {
            if (respuesta.success) {

                $("#txtIdUsuario").val(respuesta.data);
                $("#pn_Roles").removeClass('hidden');

                var uno = document.getElementById('btnGrabar');
                uno.innerHTML = '<span class="fa ico_grabar faa-wrench animated"></span>Actualizar';

                Swal.fire({
                    type: 'success',
                    title: 'Señor(a) Funcionario(a:)',
                    text: respuesta.message + ", ahora revise roles del sistema"
                });

            } else {
                $("#pn_Roles").addClass('hidden');
                Swal.fire({
                    type: 'error',
                    title: 'Señor(a) Funcionario(a:)',
                    text: respuesta.message
                });
            }
        },
        error: function (ex) {
            $("#txtIdUsuario").val(0);
            Swal.fire({
                type: 'error',
                title: 'Señor(a) Funcionario(a:)',
                text: "No es posible grabar, revise"
            });
        }
    });
}




//Funciones de Eliminación
function Dell_Roles(P_IdRolUser) {

    bootbox.confirm({
        message: "Está seguro de eliminar el rol seleccionado?",
        buttons: {
            confirm: {
                label: '<i class="fa fa-check"></i> Si',
                className: 'btn-success'
            },
            cancel: {
                label: '<i class="fa fa-times"></i> No',
                className: 'btn-danger'
            }
        },
        callback: function a(result) {
            if (result) {
                var t = result;
                bootbox.prompt({
                    title: "Justifique el motivo por el cual elimina el rol",
                    inputType: 'text',
                    buttons: {
                        confirm: {
                            label: '<i class="fa fa-check"></i> Aceptar',
                            className: "btn-success",
                        },
                        cancel: {
                            label: '<i class="fa fa-times"></i> Cancelar',
                            className: "btn btn-warning",
                        }
                    },
                    callback: function (resulta) {
                        if (resulta == null) {

                        }
                        else if (resulta == "") {
                            bootbox.alert({
                                message: "Debe justificar el motivo por el cual elimina el registro",
                                buttons: {
                                    ok: {
                                        label: '<i class="fa fa-check"></i> Aceptar',
                                        className: 'btn-success',
                                    }
                                },
                                callback: function () { a(t); }
                            });
                        }
                        else {
                            var resu = resulta.replace(/>|<|&|=|#|\?/gi, "");
                            P_DelRoles(P_IdRolUser, resu);
                        }
                    }
                });
            }
        }
    });
}

function P_DelRoles(P_IdUserRol, P_Justificacion) {
    
    var DtoInsUserRoles = {
        IdUserRol: P_IdUserRol,
        Justificacion: P_Justificacion
    }

    $.ajax({
        type: 'POST',
        url: AppRoutes.Administracion.UrlDelRoles,
        async: true,
        dataType: 'json',
        data: { obj: DtoInsUserRoles },
        success: function (result) {
            if (result.success) {
                F_GetUserRoles($("#txtIdentificacion").val());
                Swal.fire({
                    type: 'success',
                    title: 'Señor(a) Funcionario(a:)',
                    text: result.message
                });

            } else {
                Swal.fire({
                    type: 'error',
                    title: 'Señor(a) Funcionario(a:)',
                    text: result.message
                });
            }
        },
        error: function (ex) {
            Swal.fire({
                type: 'error',
                title: 'Señor(a) Funcionario(a:)',
                text: "No es posible grabar, revise"
            });
        }
    });
}


//Grillas
function GrillaUserRoles(Datos) {
    if ($.fn.dataTable.isDataTable("#tbGrilla")) {
        $("#tbGrilla").DataTable().destroy();
    }
    $("#tbGrilla").DataTable({
        destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        responsive: true,
        "columns": [
            {
                data: null, className: "celdaCenter celda3", "render": function (data, type, row) {
                    var inicioBoton = '<div class="dropdown dropend"><button class="btn btn-success" type="button" id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false"><span class="fas fa-list"></span></button><ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';
                    var Eliminar = `<li style="padding-left: 17px;"><a style="color: #102717;" href="javascript:Dell_Roles(${row.IdUserRol})"><i class="fa fa-trash red"></i>&nbsp;Eliminar</a></li>`;
                    var finBoton = '</ul></div>';
                    return inicioBoton + Eliminar + finBoton;
                }
            },
            { "title": "Roles Asignados", "data": "Descripcion", "name": "Descripcion", className: "celdaCenter celda15" },
            { "title": "Fecha de Asignación", "data": "FechaCreacion", "name": "FechaCreacion", className: "celdaCenter celda7" },
            { "title": "Funcionario que Asignó", "data": "FuncionarioCreacion", "name": "FuncionarioCreacion", className: "celdaJust celda17" },
            { "title": "Fecha Caducidad", "data": "FechaFin", "name": "FechaFin", className: "celdaCenter celda7" },
            { "title": "Observaciones", "data": "Justificacion", "name": "Justificacion", className: "celdaJust" }
        ],
        lengthMenu: [
            [5, 10, 25, 50, -1],
            ['5 registros', '10 registros', '25 registros', '50 registros', 'Todos']
        ],
        ordering: false,
        pageLength: 10,
        bLengthChange: true,
        searching: true,
        paging: true,
        info: true
    });
}


//Limpiar Variables
function Limpiar() {
    $("#imgFoto")[0].src = "img/Avatar.png";
    $("#txtSituacionLab").val("");
    $("#txtFuncionario").val("");
    $("#txtCorreo").val("");
    $("#txtUserName").val("");
    $("#txtCelular").val("");
    $("#txtDependencia").val("");
    $("#txtCargo").val("");
    $("#txtIdentificacion").val("");
    $('#chkActivo').prop('checked', 0).trigger('change');

    var uno = document.getElementById('btnGrabar');
    uno.innerHTML = '<span class="fa ico_grabar faa-wrench animated"></span>Guardar';

    $("#pn_Roles").addClass('hidden');

    LimpiarGrilla();
}
function LimpiarGrilla() {
    if ($.fn.dataTable.isDataTable("#tbGrilla")) {
        $("#tbGrilla").DataTable().destroy();
    }
    $("#tbGrilla").empty();
    $("#pn_Grilla").addClass('hidden');
}
function LimpiarRoles() {
    $("#ddlRol").val("");
    $("#ddlRol").trigger('change.select2');
    $("#ddlRol").trigger("chosen:updated");

    $("#txtFechaFin").val("");
    $("#txtJustificacion").val("");
}