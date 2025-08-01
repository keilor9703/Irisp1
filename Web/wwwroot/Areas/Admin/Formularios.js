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


// Modelo para la grilla jave script- >

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
            { "title": "Identificador", "data": "Consecutivo", "name": "Consecutivo", className: "celdaJust celda26" },
            { "title": "Nombres y Apellidos", "data": "Filename", "name": "Filename", className: "celdaJust celda26" },
            { "title": "Dirección", "data": "Ruta", "name": "Ruta", className: "celdaJust celda20" },
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
    ModalExitoso("Señor(a) Funcionario(a:)", "je");
}
function Alertas() {
    create('error', 'Se deben digitar minimo dos campos para realizar la busqueda', '/img/AlertError.png');
    return;
}



/// CAMBIOS EN MODAL IT JOHN BERMUDEZ
$(document).ready(function () {
    $('#ddlNomClase1').change(function () {
        if ($(this).val() === "1") {
            // Mostrar caja de texto si se selecciona "Opción 1"
            $('#textContainer').show();
        } else {
            // Ocultar caja de texto para cualquier otra opción
            $('#textContainer').hide();
        }
    });
});

