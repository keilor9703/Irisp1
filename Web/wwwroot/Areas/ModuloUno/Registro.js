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

// Control para el Tabs
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

//  botones de la vista

$(document).ready(function () {
    initializeIntegranteManagement();
});

function initializeIntegranteManagement() {
    setupAddIntegranteButton();
    setupRemoveIntegranteButtons();
}

function setupAddIntegranteButton() {
    $('#btnAddIntegrante').click(function () {
        addNewIntegrante();
    });
}

function addNewIntegrante() {
    const newIntegranteHTML = getIntegranteTemplate();
    if (newIntegranteHTML) {
        appendIntegranteToContainer(newIntegranteHTML);
    } else {
        logTemplateError();
    }
}

function getIntegranteTemplate() {
    return $('#integranteTemplate').html();
}

function appendIntegranteToContainer(htmlContent) {
    $('#integrantesContainer').append(htmlContent);
}

function logTemplateError() {
    console.error("El elemento con ID 'integranteTemplate' no existe o está vacío.");
}

function setupRemoveIntegranteButtons() {
    $('#integrantesContainer').on('click', '.btnRemoveIntegrante', function () {
        removeIntegrante(this);
    });
}

function removeIntegrante(buttonElement) {
    $(buttonElement).closest('.integrante-row').remove();
}


//Funciones de Inserción y Actualización  CLIENTES JOHN BERMUDEZ 
function InsDatosIris() {

    Obj = {
        IdentificacionInforma: $("#txtIdentificacion").val(),
        IdUnidad: $("#ddlUnidad").val(),
        Dependencia: $("#ddlDependencia").val(),
        IdCuadrante: $("#ddlCuadrante").val(),
        Celular: $("#txtCelular").val(),
        IdTipoServicio: $("#ddlTServicio").val(),
        Especialidad: $("#ddlEspecialidad").val(),

        IdClase: $("#ddlClase").val(),
        NombreClase: $("#ddlNomClase").val(),
        TiempoActividad: $("#ddlTActividad").val(),
        Fuente: $("#ddlFuente").val(),
        Entorno: $("#ddlEntono").val(),
        EntornoAfectado: $("txtEntAfectado").val(),
        IdZona: $("#ddlZona").val(),
        DescripcionGeneral: $("#txtDescripcionGral").val(),

        /*DescripcionGeneral: $("#txtRegistroFoto").val(),*/

    }

    $.ajax({
        type: 'POST',
        url: UrlInsDatosIris,
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









//Grillas
function agregarIntegrante2(Datos) {
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
            { "title": "Identificacion", "data": "Descripcion", "name": "Descripcion", className: "celdaCenter celda15" },
            { "title": "Apellidos", "data": "FechaCreacion", "name": "FechaCreacion", className: "celdaCenter celda7" },
            { "title": "Nombres", "data": "FuncionarioCreacion", "name": "FuncionarioCreacion", className: "celdaJust celda17" },
            { "title": "Alias", "data": "FechaFin", "name": "FechaFin", className: "celdaCenter celda7" }

            ,
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




    // Función cerrada para agregar el integrante
    function agregarIntegrante() {
        const identificacion = document.getElementById('txtIdentificacionInt').value;
        const apellidos = document.getElementById('txtApellidosInte').value;
        const nombres = document.getElementById('txtNombreInte').value;
        const alias = document.getElementById('txtAliasInte').value;

        // Validar que los campos no estén vacíos
        if (identificacion && apellidos && nombres) {
            const tbody = document.getElementById('tbodyIntegrantes');
            const nuevaFila = document.createElement('tr');
            nuevaFila.innerHTML = `
                <td>${identificacion}</td>
                <td>${apellidos}</td>
                <td>${nombres}</td>
                <td>${alias}</td>
                <td><button class="btn btn-danger btnEliminar" onclick="eliminarIntegrante(this)">Eliminar</button></td>
            `;
            tbody.appendChild(nuevaFila);

            // Limpiar los campos
            document.getElementById('txtIdentificacionInt').value = '';
            document.getElementById('txtApellidosInte').value = '';
            document.getElementById('txtNombreInte').value = '';
            document.getElementById('txtAliasInte').value = '';
        } else {
            alert('Por favor, complete todos los campos requeridos.');
        }
    }

    // Evento para el botón Añadir Integrante
    document.getElementById('btnAddIntegrante').addEventListener('click', agregarIntegrante);

    // Función para eliminar un integrante de la grilla
    function eliminarIntegrante(btn) {
        const row = btn.parentNode.parentNode;
        row.parentNode.removeChild(row);
    }


// Grilla de Etapa de verificacion
function GetGrilla() {
    if ($.fn.dataTable.isDataTable("#tbGrilla")) {
        $("#tbGrilla").DataTable().destroy();
    }
    $("#tbGrilla").DataTable({
        "ajax": {
            type: "POST",
            url: UrlGetDatos,
            async: true,
            data: { V_Identificacion: $("#txtCodigoReporte").val() },
            datatype: "json",
            cache: false
        },
        "initComplete": function (settings, json) {
            if (json.success) {
                $("#pn_Grilla").removeClass('hidden');
            }
            else {
                $("#pn_Grilla").addClass('hidden');
            }
        },
        language: glOpcionesIdioma,
        responsive: true,
        "columns": [
            {
                data: null, className: "celdaCenter celda3", "render": function (data, type, row) {
                    var inicioBoton = '<div class="dropdown dropend"><button class="btn btn-success" type="button" id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false"><span class="fas fa-list"></span></button><ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';
                    var Eliminar = `<li style="padding-left: 17px;"><a style="color: #102717;" href="javascript:Dell_Roles(${row.IdUserRol})"><i class="fa fa-trash red"></i>&nbsp;Eliminar</a></li>`;
                    var Editar = `<li style="padding-left: 17px;"><a style="color: #102717;" href="javascript:ModalBitacora(${row.IdUserRol})"><i class="fa fa-edit green"></i>&nbsp;Editar</a></li>`;
                    var Ver =      `<li style="padding-left: 17px;"><a style="color: #102717;" href="javascript:F_GetBibliaDetalle(${row.IdUserRol})"><i class="fa fa-eye green"></i>&nbsp;Ver Detalle</a></li>`;
                    var finBoton = '</ul></div>';
                    return inicioBoton + Eliminar + Editar + Ver + finBoton;
                }
            },
            { "title": "Estado", "data": "Consecutivo", "name": "Consecutivo", className: "celdaJust celda26" },
            { "title": "Estado Existencia", "data": "Consecutivo", "name": "Consecutivo", className: "celdaJust celda26" },
            { "title": "Codigo", "data": "Consecutivo", "name": "Consecutivo", className: "celdaJust celda26" },
            { "title": "Dependencia", "data": "Consecutivo", "name": "Consecutivo", className: "celdaJust celda26" },
            { "title": "Municipio", "data": "Filename", "name": "Filename", className: "celdaJust celda26" },
            { "title": "Fecha Inicio Actividad", "data": "Filename", "name": "Filename", className: "celdaJust celda26" },
            { "title": "Clase", "data": "Ruta", "name": "Ruta", className: "celdaJust celda20" },
            { "title": "Nombre", "data": "Consecutivo", "name": "Consecutivo", className: "celdaJust celda26" },
            { "title": "Cantidad", "data": "Filename", "name": "Filename", className: "celdaJust celda26" },
            { "title": "Caracteristicas Generales", "data": "Filename", "name": "Filename", className: "celdaJust celda26" },
            { "title": "Descripcion del tramite", "data": "Filename", "name": "Filename", className: "celdaJust celda26" },
            { "title": "Zona", "data": "Filename", "name": "Filename", className: "celdaJust celda26" },
            { "title": "Tipo Servicio", "data": "Filename", "name": "Filename", className: "celdaJust celda26" },
            { "title": "Fuente", "data": "Ruta", "name": "Ruta", className: "celdaJust celda20" },
            
        ],
        lengthMenu: [
            [10, 25, 50, -1],
            ['10 registros', '25 registros', '50 registros', 'Todos']
        ],
        ordering: false,
        pageLength: 10,
        bLengthChange: false,
        searching: true,
        paging: true,
        info: true
    });
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

