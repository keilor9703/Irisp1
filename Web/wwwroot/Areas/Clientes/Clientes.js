
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
//$("#ddlUnidad").change(function () {
$("#ddlUnidad").on('change.select2', function () {

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


// funcion de Consultar

function F_GetKardex(V_Identificacion) {

    let Identificación = Number(V_Identificacion);
    if (Identificación < 1) {
        create('error', 'Debe digitar número de Identificación', '../../img/AlertError.png');
        return;
    }

    $.ajax({
        type: "POST",
        url: UrlF_GetDatosCliente,
        async: true,
        data: { V_Identificacion: $("#txtIdentificacion").val() },
        dataType: 'json',
        cache: false,
        success: function (respuesta) {
            if (respuesta.success) {
                //$("#IdDto").empty();
                $("#txtApellidos").val(respuesta.data[0].Apellidos);
                $("#txtNombres").val(respuesta.data[0].Nombres);
                $("#txtFechaNace").val(respuesta.data[0].FechaNace);


                $("#IdDto").val(respuesta.data[0].Dto);
                $("#IdDto").trigger('change.select2');
                $("#IdDto").trigger("chosen:updated");

                $("#IdLugar").val(respuesta.data[0].Lugar);
                $("#IdLugar").trigger('change.select2');
                $("#IdLugar").trigger("chosen:updated");

                /*$("#IdDto").on('change.select2', function () {
                    // Lógica para realizar la otra consulta
                    var dtoSeleccionado = $(this).val();
                    // Aquí puedes hacer la llamada a la otra consulta usando dtoSeleccionado
                    realizarConsulta(dtoSeleccionado);
                });*/
                /*
                $("#IdLugar").empty();
                $("#IdLugar").append($("<option></option>").text(respuesta.data[0].Lugar).val(respuesta.data[0].Lugar));
                $("#IdLugar").trigger('change.select2');
                $("#IdLugar").trigger("chosen:updated");*/
                //$("#IdLugar").attr("disabled", true); //deshabilitar elemento

                $("#IdGenero").val(respuesta.data[0].Genero);
                $("#IdGenero").trigger('change.select2');
                $("#IdGenero").trigger("chosen:updated");

                $("#txtDireccion").val(respuesta.data[0].Direccion);

                $("#ddlUnidad").val(respuesta.data[0].Unidad);
                $("#ddlUnidad").trigger('change.select2');
                $("#ddlUnidad").trigger("chosen:updated");

                create('success', "Si se Encontraron Programas en este año", '../../img/AlertSucess.png');

                $("#ddlDependencia").val(respuesta.data[0].Dependencia);
                $("#ddlDependencia").trigger('change.select2');
                $("#ddlDependencia").trigger("chosen:updated");

                $("#txtObservaciones").val(respuesta.data[0].Observaciones);

               /* F_GetUserRoles(V_Identificacion);*/
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
                Limpiar();
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

//function F_GetKardexGrilla(P_Identificacion) {
//    $.ajax({
//        type: "POST",
//        url: UrlGetKardexGrilla,
//        async: true,
//        data: { V_Identificacion: P_Identificacion },
//        dataType: 'json',
//        cache: false,
//        success: function (respuesta) {
//            if (respuesta.success) {
//                $("#pn_Grilla").removeClass('hidden');
//                $("#pn_Roles").removeClass('hidden');

//                $("#txtIdUsuario").val(respuesta.data[0].IdUsuario);

//                GrillaUserRoles(respuesta.data);

//                var uno = document.getElementById('btnGrabar');
//                uno.innerHTML = '<span class="fa ico_grabar faa-wrench animated"></span>Actualizar';

//                var _Bloqueado = respuesta.data[0].Bloqueado;
//                var Activo = 0;
//                if (_Bloqueado == 0) {
//                    Activo = 1;
//                }
//                else {
//                    Activo = 0;
//                }

//                $('#chkActivo').prop('checked', Activo).trigger('change');


//            } else {
//                $("#pn_Grilla").addClass('hidden');
//                $("#pn_Roles").addClass('hidden');

//                var uno = document.getElementById('btnGrabar');
//                uno.innerHTML = '<span class="fa ico_grabar faa-wrench animated"></span>Guardar';

//                Swal.fire({
//                    type: 'info',
//                    title: 'Señor(a) Funcionario(a:)',
//                    text: "El usuario no ha sido creado en el sistema, proceda activarlo"
//                });
//            }
//        },
//        error: function () {
//            Swal.fire({
//                type: 'error',
//                title: 'Señor(a) Funcionario(a:)',
//                text: 'No es posible consultar, revise!!'
//            });
//        }
//    });
//}



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

                //ddl.trigger('change.select2');
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
//$("#IdDto").change(function () {
$("#IdDto").on('change.select2', function () {
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
    $("#ddlUnidad").val("");
    $("#ddlDependencia").val("");
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

function GrillaDatosCliente(Datos) {
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
            { "title": "No Identificacion", "data": "Descripcion", "name": "Descripcion", className: "celdaCenter celda15" },
            { "title": "Apellidos", "data": "FechaCreacion", "name": "FechaCreacion", className: "celdaCenter celda7" },
            { "title": "Nombres", "data": "FuncionarioCreacion", "name": "FuncionarioCreacion", className: "celdaJust celda17" },
            { "title": "Fecha de Nacimiento", "data": "FechaFin", "name": "FechaFin", className: "celdaCenter celda7" },
            { "title": "Departamento", "data": "Justificacion", "name": "Justificacion", className: "celdaJust" },
            { "title": "Ciudad", "data": "Justificacion", "name": "Justificacion", className: "celdaJust" },
            { "title": "Genero", "data": "Justificacion", "name": "Justificacion", className: "celdaJust" },
            { "title": "Direccion de residencia", "data": "Justificacion", "name": "Justificacion", className: "celdaJust" },
            { "title": "Unidad Policial", "data": "Justificacion", "name": "Justificacion", className: "celdaJust" },
            { "title": "Dependencia", "data": "Justificacion", "name": "Justificacion", className: "celdaJust" },
            { "title": "Observaciones", "data": "Justificacion", "name": "Justificacion", className: "celdaJust" },
  
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