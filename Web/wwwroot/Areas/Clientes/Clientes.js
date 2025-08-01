
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
});
//Control para el Tabs
$('.fancyTabs .tab').on('click', function (e) {
    $(this).siblings().removeClass('active');
    $(this).addClass('active');
});
$('#fancyTabWidget .nexttab').on('click', function () {
    $('#fancyTabWidget .tab.active').removeClass('active').next().tab('show');
});
$('#fancyTabWidget .prevtab').on('click', function () {
    $('#fancyTabWidget .tab.active').removeClass('active').prev().tab('show');
});
//Llamar dependencias según Unidad -- ALDANA
$("#ddlUnidad").change(function () {

    $("#ddlDependencia").empty();
    $.ajax({
        type: 'POST',
        url: UrlGetDependencias,
        dataType: 'json',
        data: { V_SiglaPapa: $("#ddlUnidad").val() },
        success: function (result) {
            $("#ddlDependencia").append('<option value="0">Seleccione dependencia</option>');
            $.each(result.datos, function (i, resultado) {
                $("#ddlDependencia").append('<option value="' + resultado.value + '">' + resultado.text + '</option>');
            });
            $("#ddlDependencia").trigger("chosen:updated");
        },
        error: function (ex) {
            Swal.fire({
                type: 'error',
                title: 'Señor(a) Funcionario(a:)',
                text: "No es posible cargar la lista de selección, revise"
            });
        }
    });
});

//Funciones de Inserción y Actualización  CLIENTES JOHN BERMUDEZ 
function InsUsuarios() {

    Obj = {
        Identificacion: $("#txtIdentificacion").val(),
        Apellidos: $("#txtApellidos").val(),
        Nombres: $("#txtNombres").val(),
        FechaNace: $("#txtFechaNace").val(),
        IdDto: $("#IdDto").val(),
        IdLugar: $("#IdLugar").val(),
        IdGenero: $("#IdGenero").val(),
        Direccion: $("#txtDireccion").val(),
        Unidad: $("#ddlUnidad").val(),
        Dependencia: $("#ddlDependencia").val(),
        Observaciones: $("#txtObservaciones").val(),
    }
    
    $.ajax({
        type: 'POST',
        url: UrlInsUsuarios,
        async: true,
        dataType: 'json',
        data: { Obj: Obj },   
        success: function (result) {
            if (result.success) {
                //F_GetUserRoles($("#txtIdentificacion").val());
                //LimpiarRoles();
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
                text: "No es posible grabar datos, revise" + ex
            });
        }
    });
}

function F_GetUserRoles(P_Identificacion) {
    $.ajax({
        type: "POST",
        url: UrlGetUserRoles,
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



// FUNCION PARA FILTRAR MUNICIPIOS POR DEPARTAMENTO   prueba

function obtenerMunicipio(lugarGeograficoId, selectorDropdown) {
    if (!lugarGeograficoId || lugarGeograficoId <= 0) {
        create('error', 'Debe seleccionar un lugar válido.', '/img/AlertError.png');
        resetDropDownList("ddlTab41Barrio");
        return;
    }

    const url = `${UrlGetMunicipios}`

    // Limpiar el dropdown y agregar una opción por defecto
    $(selectorDropdown).empty().append('<option value="">Cargando...</option>');

    $.ajax({
        url: url,  // Ruta del controlador
        type: 'POST',
        data: { V_Id: lugarGeograficoId },  // Parámetro que se envía
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                var ddl = $(selectorDropdown);
                ddl.empty();  // Limpiar opciones anteriores
                ddl.append('<option value="">No Seleccionado</option>');  // Opción por defecto

                // Recorrer los datos y agregar las opciones al dropdown
                $.each(response.datos, function (index, item) {
                    ddl.append('<option value="' + item.value + '">' + item.text + '</option>');
                });

                // Actualizar el dropdown si estás usando 'select2' o algún plugin similar
                ddl.trigger("chosen:updated");
            } else {
                create('error', response.message, '/img/AlertError.png');
            }
        },
        error: function (ex) {
            console.error('Error al cargar los barrios:', ex);
            create('error', 'No se pudieron cargar los barrios, intente de nuevo.', '/img/AlertError.png');
        }
    });
}

// Evento para el cambio en el selector de lugar de residencia
$("#IdDto").change(function () {
    var lugarGeograficoId = $(this).val();

    // Llamada a la función para obtener barrios basados en el lugar de residencia
    obtenerMunicipio(lugarGeograficoId, "#IdLugar");
});


///// FIN DE LA FUNCION DE CARGAR MUNICIPIO POR DEPARTAMENTO

function Limpiar() {
    
    $("#txtIdentificacion").val("");
    $("#txtApellidos").val("");
    $("#txtNombres").val("");
    $("#txtFechaNace").val("");
    $("#IdDto").val("");
    $("#IdLugar").val("");
    $("#IdGenero").val("");
    $("#txtDireccion").val("");
    $("#txtObservaciones").val("");

    $('#chkActivo').prop('checked', 0).trigger('change');

}






function OpenModal() {
    $('#Modal_VerRegistro').modal("show");
}

function OpenModalError() {
    ModalError("Señor(a) Funcionario(a:)", "Mensaje");
}

function SweetAlertError() {
    Swal.fire({
        type: 'error',
        title: 'Señor(a) Funcionario(a:)',
        text: 'El paciente presenta inasistencia a citas, debe solicitar asistencia pedagógica para aplicar comparendo y habilitar citas'
    });
}

function SweetAlertExitoso() {
    Swal.fire({
        type: 'success',
        title: 'Señor(a) Funcionario(a:)',
        text: 'Grabado e'
    });
}

function OpenModalExit() {
    ModalExitoso("Señor(a) Funcionario(a:)", "Mensaje");
}
function Alertas() {
    create('error', 'Se deben digitar minimo dos campos para realizar la busqueda', '/img/AlertError.png');
    return;
}

