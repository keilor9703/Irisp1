let archivoSubido = null; // Guardará el archivo ya cargado

$(document).ready(function () {
    // Inicializa Select2 si no está inicializado
    if ($.fn.select2) {
        $('#ddlAnioIris').select2();       
    }

    // Asocia el evento change
    $('#ddlAnioIris').on('change', function () {
        F_GetInfoGrillas();
    });    

    $('#chkRegFoto').change(function () {
        if ($(this).is(':checked')) {
            $('#fotografia').closest('.col-md-4').removeClass('hidden');
        } else {
            $('#fotografia').closest('.col-md-4').addClass('hidden');
        }
    });


   
});
function SubirFoto() {

        var fileInput = $('#fileAnexoFotografico')[0];

        if (fileInput.files.length === 0) {
            Swal.fire('Advertencia', 'Debe seleccionar una imagen.', 'warning');
            return;
        }

        var file = fileInput.files[0];
        var maxSizeMB = 5;
        var allowedTypes = ['image/jpeg', 'image/png'];

        if (!allowedTypes.includes(file.type)) {
            Swal.fire('Error', 'Formato no permitido. Solo se aceptan imágenes JPG o PNG.', 'error');
            return;
        }

        if (file.size > maxSizeMB * 1024 * 1024) {
            Swal.fire('Error', 'La imagen supera el tamaño máximo permitido de 5MB.', 'error');
            return;
        }

        // Evitar reenvío del mismo archivo
        if (archivoSubido && archivoSubido.name === file.name && archivoSubido.size === file.size) {
            Swal.fire('Información', 'Esta imagen ya fue subida.', 'info');
            return;
        }

        // Captura de los datos requeridos por el procedimiento

        var idCriminalidad = $("#txtConsecutivoIris").val();


        if (!idCriminalidad) {
            Swal.fire('Error', 'Faltan datos requeridos para guardar la imagen.', 'error');
            return;
        }

        var formData = new FormData();
        formData.append('foto', file);
        formData.append('idCriminalidad', idCriminalidad);


        $.ajax({
            url: '/Irisp1/RegistrosIrisp1/GuardarFotoConRegistro',
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (resp) {
                if (resp.exito) {
                    archivoSubido = file;

                    $('#estadoFoto').show().text('✅ Imagen guardada exitosamente.');
                    $('#fileAnexoFotografico').val('');

                    setTimeout(() => $('#estadoFoto').fadeOut(), 4000);
                } else {
                    Swal.fire('Error', resp.mensaje || 'No se pudo subir la imagen.', 'error');
                }
            },
            error: function () {
                Swal.fire({
                    icon: 'error',
                    title: 'Señor(a) Funcionario(a):',
                    text: 'Ocurrió un error al subir la imagen.'
                });
            }
        });
    
}
function AbrirModalMas() {

    $('#ModalCaracteristicasGenerales').modal("show");

}
function AbrirModalNuevoIris() {

    const modalElement = document.getElementById('Modal_VerRegistro');

    const modalInstance = new bootstrap.Modal(modalElement, {

        backdrop: 'static',

        keyboard: false,

        focus: false  // Desactiva focus automático de Bootstrap

    });

    modalInstance.show();


   
    consultarConsecutivoIris();
   

}
function ActualizarIris() {

    $('#ModalCaracteristicasGenerales').modal("show");

}
function ActualizarEstadoIris() {

    $('#Modal_ActualizarEstado').modal("show");
    F_GetEstadosIrisP1();


}
function ActualizarIrisp1() {

    $('#Modal_ActualizarIrisp1').modal("show");
    F_GetEstadosIrisP1();


}
function F_GetEstadosIrisP1() {
   $.ajax({
        url: UrlGetEstadosIrisP1,
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            if (response && Array.isArray(response.data)) {
                const ddlDto = $('#ddlEstadosIrisP1');
                ddlDto.empty().append('<option value="0">Seleccione</option>');
                const opciones = response.data.map(item => `<option value="${item.CodigoDominio}">${item.DescripcionEstado}</option>`).join('');
                ddlDto.append(opciones).trigger('change');
            }
        },
    });

}
function AbrirModalDatosCaracteristicas(CaracteristicasGenerales) {

    // Mostrar la modal
    $('#Modal_Caracteristicas').modal("show");
    $('#txtCaracteristicasGenerales').val(CaracteristicasGenerales);
}
function F_GetInfoGrillas() {
    $.ajax({
        type: 'GET',
        url: UrlGetInfoGrillas, // URL del endpoint que devuelve los datos
        dataType: 'json',
        data: { V_Anio: $('#ddlAnioIris').val() },
        success: function (response) {

            // Inicializar la grilla con los datos filtrados o vacíos
            GetGrillaVerificacion(response.data);
            GetGrillaInvestigacion(response.data);
            GetGrillaFinalizacion(response.data);
        },
        error: function () {
            // En caso de error, inicializar la grilla con datos vacíos
            GetGrillaVerificacion([]);
            GetGrillaInvestigacion([]);
            GetGrillaFinalizacion([]);
        }
    });
}


// Grillas /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function GetGrillaVerificacion(datos) {
    inicializarGrilla("#tbGrilla", "#pn_GrillaVerificacion", datos, [2, 3, 4], columnasBase);
}

function GetGrillaInvestigacion(datos) {
    inicializarGrilla("#tbGrillaInvestigacion", "#pn_GrillaInvestigacion", datos, [72, 73], columnasBase);
}

function GetGrillaFinalizacion(datos) {
    inicializarGrilla("#tbGrillaFinalizacion", "#pn_GrillaFinalizacion", datos, [5], columnasBase);
}

// Definición de columnas base (puedes extraer las columnas comunes a una variable)
const columnasBase = [

    columnaAcciones(1),
    Estados(),
    { title: "Estado Existencia", data: "EstadoExistenciaDescripcion", name: "EstadoExistenciaDescripcion", className: "celdaCenter" },
    { title: "Codigo ", data: "Codigo", name: "Codigo", className: "celdaCenter" },
    { title: "Dependencia", data: "SiglaUnidad", name: "SiglaUnidad", className: "celdaCenter" },
    { title: "Municipio", data: "Municipio", name: "Municipio", className: "celdaCenter" },
    { title: "Fecha Inicio Actividad", data: "FechaInicioExistencia", name: "FechaInicioExistencia", className: "celdaCenter" },
    { title: "Fuente", data: "Clase", name: "Fuente", className: "celdaCenter" },
    { title: "Nombre", data: "NombreClase", name: "NombreClase", className: "celdaCenter" },
    /*{ title: "Nombre", data: "CaracteristicasGenerales", name: "CaracteristicasGenerales", className: "celdaCenter", visible: false },*/
    columnaCaracteristicasGenerales(),
    columnaDescripcionTramite(),
    // ...agrega aquí el resto de columnas comunes...

    { title: "Codigo Zona", data: "IdZona", name: "IdZona", className: "celdaCenter", visible: false },
    { title: "Zona", data: "Zona", name: "Zona", className: "celdaCenter" },
    { title: "Tipo Servicio", data: "TipoServicio", name: "TipoServicio", className: "celdaCenter" },
    { title: "Codigo Fuente", data: "IdFuente", name: "IdFuente", className: "celdaCenter", visible: false },
    { title: "Fuente", data: "Fuente", name: "Fuente", className: "celdaCenter" },
    {
        title: "Fecha de Creacion",
        data: "FechaCreacion",
        name: "FechaCreacion",
        className: "celdaCenter celda7",
        render: function (data) {
            const fecha = moment(data).format('DD/MM/YYYY');
            const hora = moment(data).format('hh:mm:ss a');
            return `${fecha}<br>${hora}`;
        }
    },
    { title: "Unidad Verificación Existencia", data: "UnidadVerificacionExiostencia", name: "UnidadVerificacionExiostencia", className: "celdaCenter" },
    { title: "Fecha Asiganación Verificación Existencia", data: "FechaVerificacionExistencia", name: "FechaVerificacionExistencia", className: "celdaCenter" },
    { title: "Fecha RespuestaVerificación Existencia", data: "FechaRespuestaVerificacion", name: "FechaRespuestaVerificacion", className: "celdaCenter" },
    { title: "Contador Verificación Existencia", data: "ContadorVerificacionExistencia", name: "ContadorVerificacionExistencia", className: "celdaCenter" },
    { title: "Unidad Proceso Investigativo", data: "UnidadProcesoInvestigativo", name: "UnidadProcesoInvestigativo", className: "celdaCenter" },
    { title: "Fecha Asignación Proceso Investigativo", data: "FechaProcesoInvestigativo", name: "FechaProcesoInvestigativo", className: "celdaCenter" },
    { title: "Fecha Respuesta Proceso Investigativo", data: "FechaRespuestaInvestigativo", name: "FechaRespuestaInvestigativo", className: "celdaCenter" },
    { title: "Contador Proceso Investigativo", data: "ContadorProcesoInvestigativo", name: "ContadorProcesoInvestigativo", className: "celdaCenter" },
    { title: "Resultados", data: "Resultados", name: "Resultados", className: "celdaCenter" },


    { title: "Criminalidad", data: "CriminalidadId", name: "Identificación", className: "celdaCenter", visible: false },
    { title: "Codigo Unidad", data: "IdUnidad", name: "IdUnidad", className: "celdaCenter", visible: false },
    { title: "Identificación Informa", data: "IdentificacionInforma", name: "IdentificacionInforma", className: "celdaCenter", visible: false },
    { title: "Codigo Unidad", data: "Celular", name: "Celular", className: "celdaCenter", visible: false },
    { title: "Codigo Tipo de servcio", data: "IdTipoServicio", name: "IdTipoServicio", className: "celdaCenter", visible: false },
    { title: "Codigo Cuadrante", data: "IdCuadrante", name: "IdCuadrante", className: "celdaCenter", visible: false },
    { title: "Codigo Clase", data: "IdClase", name: "IdClase", className: "celdaCenter", visible: false },
    { title: "Cantidad de Integrantes", data: "CantidadIntegrantes", name: "CantidadIntegrantes", className: "celdaCenter", visible: false },
    { title: "Vigente", data: "Vigente", name: "Vigente", className: "celdaCenter", visible: false },
    { title: "Fecha de Creacion", data: "FechaCreacion", name: "FechaCreacion", className: "celdaCenter", visible: false },
    { title: "Identificacion Crea", data: "IdentificacionCrea", name: "IdentificacionCrea", className: "celdaCenter", visible: false },
    { title: "Codigo Unidad", data: "MaquinaCrea", name: "MaquinaCrea", className: "celdaCenter", visible: false },
    { title: "Fecha de Modificacion", data: "FechaModifica", name: "FechaModifica", className: "celdaCenter", visible: false },
    { title: "Identificacion Modifica", data: "IdentificacionModifica", name: "IdentificacionModifica", className: "celdaCenter", visible: false },
    { title: "Maquina Modifica", data: "MaquinaModifica", name: "MaquinaModifica", className: "celdaCenter", visible: false },
    { title: "Codigo", data: "IdUnidad", name: "IdUnidad", className: "celdaCenter", visible: false },
    { title: "Consecutivo del Codigo", data: "ConsecutivoCodigo", name: "ConsecutivoCodigo", className: "celdaCenter", visible: false },
    { title: "Icodigo de Estado", data: "IdEstado", name: "IdEstado", className: "celdaCenter", visible: false },

];

function Estados() {
    return {
        title: "Estado",
        data: "EstadoDescripcion",
        name: "EstadoDescripcion",
        className: "celdaCenter",
        render: function (data, type, row) {
            if (!data) return '';

            const estado = data.toLowerCase();
            let color = '';

            switch (estado) {
                case 'sin asignar':
                    color = '#c53a1d'; // rojo
                    break;
                case 'asignado':
                    color = '#236305'; // azul
                    break;
                case 'avance verificación':
                    color = '#799137'; // verde
                    break;
                case 'investigación':
                    color = '#2127f5'; // amarillo
                    break;
                case 'avance investigación':
                    color = '#40a8c7'; // naranja
                    break;
                case 'finalizado':
                    color = '#032b57'; // verde
                    break;
                default:
                    color = '#386ca0'; // gris oscuro
            }

            return `<span style="background-color: ${color}; color: white ; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 120px;">${data}</span>`;
        }
    };
}
function columnaAcciones() {

    
        return {
            data: null,
            className: "celdaCenter celda3",
            render: function (data, type, row) {
                var inicioBoton = '<div class="dropdown dropend"><button class="btn btn-success" type="button" id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false"><span class="fas fa-list"></span></button><ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';
                var DetallesIris = `<li style="padding-left: 15px;"><a style="color: #102717;" href="javascript:ActualizarEstadoIris()"><i class="fas fa-list"></i>&nbsp; Detalles </a></li>`;
                var ActualizarIris = `<li style="padding-left: 15px;"><a style="color: #102717;" href="javascript:ActualizarIrisp1()"><i class="fa fa-retweet green"></i>&nbsp;Actualizar Iris</a></li>`;
                var ActualizarEstado = `<li style="padding-left: 15px;"><a style="color: #102717;" href="javascript:ActualizarEstadoIris()"><i class="fa fa-retweet green"></i>&nbsp;Actualizar Estado</a></li>`;
                var ActualizarExistencia = `<li style="padding-left: 15px;"><a style="color: #102717;" href="javascript:F_GetBibliaDetalle()"><i class="fa fa-retweet green"></i>&nbsp;Actualizar Existencia</a></li>`;
                var Eliminar = `<li style="padding-left: 15px;"><a style="color: #102717;" href="javascript:Dell_Roles()"><i class="fa fa-trash red"></i>&nbsp;Eliminar</a></li>`;
                var finBoton = '</ul></div>';
                return inicioBoton + DetallesIris + ActualizarIris + ActualizarEstado + ActualizarExistencia + Eliminar + finBoton;
            }
        }
  

    
}

function inicializarGrilla(selectorTabla, selectorPanel, datos, estados, columnas) {
    const datosFiltrados = datos.filter(item => estados.includes(item.IdEstado));
    if ($.fn.dataTable.isDataTable(selectorTabla)) {
        $(selectorTabla).DataTable().destroy();
    }

    $(selectorTabla).empty();
    $(selectorPanel).removeClass('hidden');

    $(selectorTabla).DataTable({
        destroy: true,
        data: datosFiltrados,
        language: glOpcionesIdioma,
        responsive: true,
        columns: columnas,
        lengthMenu: [
            [10, 25, 50, -1],
            ['10 registros', '25 registros', '50 registros', 'Todos']
        ],
    });
}

function columnaCaracteristicasGenerales() {
    return {
        title: "Características Generales",
        data: "CaracteristicasGenerales",
        name: "CaracteristicasGenerales",
        className: "celdaCenter",
        render: function (data, type, row) {
            if (!data || data.trim() === "") {
                return '';
            }

            const dataEncoded = encodeURIComponent(data);

            return `
                <div style="display: flex; align-items: center; max-width: 300px; gap: 10px;">
                    <button class="btn btn-success btn-sm" type="button"
                        onclick="AbrirModalDatosCaracteristicas(decodeURIComponent('${dataEncoded}'))">
                        <span class="fa fa-eye white"></span>
                    </button>
                    <div style="white-space: nowrap; overflow: hidden; text-overflow: ellipsis; flex-grow: 1;" title="${data}">
                        ${data}
                    </div>
                </div>
            `;
        }
    };
}

function columnaDescripcionTramite() {
    return {
        title: "Descripcion del Tramite",
        data: "DescripcionTramite",
        name: "DescripcionTramite",
        className: "celdaCenter",
        render: function (data, type, row) {
            if (!data || data.trim() === "") {
                return '';
            }

            const dataEncoded = encodeURIComponent(data);

            return `
                <div style="display: flex; align-items: center; max-width: 300px; gap: 10px;">
                    <button class="btn btn-success btn-sm" type="button"
                        onclick="AbrirModalDatosCaracteristicas(decodeURIComponent('${dataEncoded}'))">
                        <span class="fa fa-eye white"></span>
                    </button>
                    <div style="white-space: nowrap; overflow: hidden; text-overflow: ellipsis; flex-grow: 1;" title="${data}">
                        ${data}
                    </div>
                </div>
            `;
        }
    };
}

function CambiarEstado() {

    $("#notificacion1").empty();

    var DtoIrispCriminalidad = {
        CriminalidadId: $("#IdCriminalidad1").val(),
        IdEstado: $("#ID_ESTADO").val()
    };

    $.ajax({
        type: 'POST',
        url: urlEstado,
        dataType: 'json',
        data: DtoIrispCriminalidad,
        success: function (response) {
            if (response.ok == true) {
                Swal.fire({
                    title: 'Guardar',
                    text: response.mensaje,
                    type: 'success',
                    showCancelButton: false,
                    confirmButtonColor: '#0a1934',
                    cancelButtonColor: '#d33',
                    confirmButtonText: 'Aceptar'
                }).then((result) => {
                    if (result.value) {
                        window.location.reload();
                    }
                });
            } else {
                sweetAlert("Atención", response.mensaje, "warning");
            }
        },
        error: function (ex) {
            sweetAlert("Error", "No se pudo guardar el registro, intente nuevamente", "error");
        }
    });
};


// FIN Grillas /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

///////////////////////////nuevo//////////////////////////////////////////


//Eventos
$("#txtIdentificacion").keyup(function (event) {
    if (event.keyCode === 13) {
        $("#btnConsultarEmpl").click();
    }
});

//Fin Eventos


$(function () {
    $("#btnMapa").click(function () {
        $('#myModal').modal("show");
    });
});

function InsIntegrantes() {

    $.ajax({
        url: UrlGetConsecutivoIntegrante,
        type: 'POST',
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                $("#txtConsecutivoIntegrante").val(response.data);

                const Obj_Integrante = {
                    INTEGRANTE_ID: response.data,
                    CRIMINALIDAD_ID: $("#txtConsecutivoIris").val(),
                    ALIAS: $("#txtAlias").val(),
                    NOMBRE: $("#txtNombreInteg").val(),
                    APELLIDO: $("#txtApellidosInteg").val(),
                    CEDULA: parseInt($("#txtIdentificacionInteg").val()),
                    ID_TIPO_INFO: 30,
                    VIGENTE: 1,
                    FECHA_MODIFICA: null,
                    IDENTIFICACION_MODIFICA: null,
                    MAQUINA_MODIFICA: null,
                    TIPO_DOCUMENTO: 1,
                    CELULAR: parseInt($("#txtCelularInteg").val()),
                    DIRECCION: $("#txtDirecciónInteg").val()
                };

                $.ajax({
                    url: UrlInsIntegrantes,
                    type: 'POST',
                    data: Obj_Integrante,
                    success: function (resp) {
                        if (resp.success) {
                            $('#pn_GrillaIntegantes').removeClass('hidden').addClass('show');
                            F_GetIntegrantes();
                            limpiarFormularioIntegrantes();
                        } else {
                            Swal.fire('Error', 'Error al insertar: ' + resp.message, 'error');
                        }
                    },
                    error: function () {
                        Swal.fire('Error', 'Fallo en la llamada AJAX.', 'error');
                    }
                });

            } else {
                $("#txtConsecutivoIntegrante").val('');
                Swal.fire('Error', "No se pudo obtener el consecutivo.", 'info');
            }
        },
        error: function () {
            $("#txtConsecutivoIntegrante").val('');
            Swal.fire('Error', 'Error de comunicación con el servidor.', 'error');
        }
    });
}

function limpiarFormularioIntegrantes() {
    $("#txtConsecutivoIntegrante").val('');
    $("#txtAlias").val('');
    $("#txtNombreInteg").val('');
    $("#txtApellidosInteg").val('');
    $("#txtIdentificacionInteg").val('');
    $("#txtCelularInteg").val('');
    $("#txtDirecciónInteg").val('');
}

function F_GetIntegrantes() {

    var V_CriminalidadId = $("#txtConsecutivoIris").val()
    $.ajax({
        type: 'GET',
        url: UrlGetIntegrantes, 
        async: true,
        data: { V_CriminalidadId: V_CriminalidadId },
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                $("#pn_GrillaIntegantes").removeClass('hidden');
                Grillantegrantes(response.data);
            } else {
                Grillantegrantes([]);
                Swal.fire('Error', response.message, 'error');
            }
        },
        error: function () {
            Grillantegrantes([]);
            Swal.fire('Error', 'No se pudo obtener la lista de integrantes.', 'error');
        }
    });
}

function Grillantegrantes(Datos) {
    if ($.fn.dataTable.isDataTable("#tbGrillaIntegantes")) {
        $("#tbGrillaIntegantes").DataTable().destroy();
    }

    $("#tbGrillaIntegantes").DataTable({
        destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        responsive: true,
        columns: [
            { title: "Alias", data: "ALIAS", className: "celdaCenter" },
            { title: "Nombre", data: "NOMBRE", className: "celdaCenter" },
            { title: "Apellido", data: "APELLIDO", className: "celdaCenter" },
            { title: "Cédula", data: "CEDULA", className: "celdaCenter" },
            { title: "Dirección", data: "DIRECCION", className: "celdaJust" },
            {
                title: "Fecha Creación", data: "FECHA_CREACION", className: "celdaCenter",
                render: function (data) {
                    if (!data) return '';
                    let fecha = new Date(data);
                    return fecha.toLocaleDateString();
                }
            }
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

function F_GetFuncionariosIris(V_Identificacion) {

    let Identificación = Number(V_Identificacion);
    if (Identificación < 1) {
        create('error', 'Debe digitar número de Identificación', '../../img/AlertError.png');
        return;
    }

    $.ajax({
        type: "POST",
        url: UrlGetFuncionarios,
        async: true,
        data: { V_Identificacion: $("#txtIdentificacion").val() },
        dataType: 'json',
        cache: false,
        success: function (respuesta) {
            if (respuesta.success) {
               // $("#imgFoto")[0].src = "https://sinac.policia.gov.co:8443/SinacPicture/picture.aspx?DocID=" + respuesta.idEncry + "&Token=Mxl7995Julabdfjughyts1*_58$$";
               // $("#txtSituacionLab").val(respuesta.data[0].SituacionLaboral);
                $("#txtFuncionario").val(respuesta.data[0].Funcionario);
               // $("#txtCorreo").val(respuesta.data[0].Correo);
                //$("#txtUserName").val(respuesta.data[0].Usuario);
                $("#txtTelefono").val(respuesta.data[0].Celular);
                $("#txtDependencia").val(respuesta.data[0].Dependencia).trigger('change');

                $("#txtUnidad").val(respuesta.data[0].Fisica + " - " + respuesta.data[0].Dependencia);
               // $("#txtEspecialidad").val(respuesta.data[0].Direccion);
                $("#txtUndeLabora").val(respuesta.data[0].UndeLaborando);
                $("#txtSiglaUnidad").val(respuesta.data[0].Fisica);

                
                
                //$("#txtCargo").val(respuesta.data[0].CargoActual);
               
               // F_GetUserRoles(V_Identificacion);
            } else {
               // Limpiar();
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


/*Datos Iris P1, campos dinamicos según la clase*/
$('#ddlClase').on('change', function () {
    var claseSeleccionada = $("#ddlClase option:selected").text().trim();

    // Ocultar todos los campos primero
    $('#txtNombreTrafico').closest('.col-md-3').addClass('hidden');
    $('#ddlModExpendio').closest('.col-md-4').addClass('hidden');

    $('#txtNombreEstructura').closest('.col-md-3').addClass('hidden');
    $('#txtCantidadEstructura').closest('.col-md-3').addClass('hidden');

    $('#txtNombreInstalacion').closest('.col-md-3').addClass('hidden');
    $('#txtCantidadInstalacion').closest('.col-md-3').addClass('hidden');

    $('#txtCantidadPersonas').closest('.col-md-3').addClass('hidden');

    $('#txtNombreNarcotrafico').closest('.col-md-3').addClass('hidden'); // Reutilizado en narcotráfico
    $('#ddlClasiNarcotrafico').closest('.col-md-4').addClass('hidden');

    // Mostrar según la clase seleccionada
    switch (claseSeleccionada) {
        case "Trafico De Estupefacientes":
            $('#txtNombreTrafico').closest('.col-md-3').removeClass('hidden');
            $('#ddlModExpendio').closest('.col-md-4').removeClass('hidden');
            break;

        case "Estructura":
            $('#txtNombreEstructura').closest('.col-md-3').removeClass('hidden');
            $('#txtCantidadEstructura').closest('.col-md-3').removeClass('hidden');
            break;

        case "Instalación Fija":
            $('#txtNombreInstalacion').closest('.col-md-3').removeClass('hidden'); // primer campo
            $('#txtCantidadInstalacion').closest('.col-md-3').removeClass('hidden');
            break;

        case "Persona":
            $('#txtCantidadPersonas').closest('.col-md-3').removeClass('hidden');
            break;

        case "Narcotrafico (grandes cantidades)":
            $('#txtNombreNarcotrafico').closest('.col-md-3').removeClass('hidden'); // segundo campo reutilizado
            $('#ddlClasiNarcotrafico').closest('.col-md-4').removeClass('hidden');
            break;
    }
});

$('#txtDependencia').change(function () {
    handleDropdownChange('/Irisp1/RegistrosIrisp1/F_GetCuadrantes', 'V_unidadLabora', $(this).val(), '#ddlCuadrante');
});

function handleDropdownChange(url, paramName, paramValue, dropdownSelector, callback) {

    if (paramValue) {
        $.getJSON(url, { [paramName]: paramValue }, function (data) {
            let dropdown = $(dropdownSelector);
            dropdown.empty().append('<option value="">Seleccione</option>');
            if (Array.isArray(data) && data.length > 0) {
                $.each(data, function (index, item) {
                    if (item && item.descripcion) {
                        dropdown.append(`<option value="${item.id || item.consecutivo || item.codigo}">${item.descripcion}</option>`);
                    }
                });

                // Ejecutar el callback después de llenar el dropdown
                if (callback && typeof callback === "function") {
                    callback();
                }

                dropdown.trigger('change'); // Opcional
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            console.error(`Error al cargar datos desde ${url}:`, textStatus, errorThrown);
        });
    }
}

function consultarConsecutivoIris() {
    $.ajax({
        url: UrlGetConsecutivoIris,
        type: 'POST',
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                $("#txtConsecutivoIris").val(response.data);
               
            } else {
                $("#txtConsecutivoIris").val('');
                // alert(response.message || "Error al obtener consecutivo.");
                Swal.fire({
                    type: 'info',
                    title: 'Señor(a) Funcionario(a:)',
                    text: "Error al obtener consecutivo."
                });
            }
        },
        error: function () {
            $("#txtConsecutivoIris").val('');
            //  alert("Error de comunicación con el servidor.");
            Swal.fire({
                type: 'error',
                title: 'Señor(a) Funcionario(a:)',
                text: 'Error de comunicación con el servidor.'
            });
        }
    });
}


function P_InsRegistroIrisP1() {
    var IdClase = $("#ddlClase").val();

    let NombreClaseI = '';
    let Id_modalidadI = 0;
    let Cantidad = 0;
    let Id_ClasiNarcotrafico = 0;

    switch (IdClase) {
        case '74':
            NombreClaseI = $("#txtNombreTrafico").val();
            Id_modalidadI = $("#ddlModExpendio").val();
            break;

        case '13':
            NombreClaseI = $("#txtNombreEstructura").val();
            Cantidad = $("#txtCantidadEstructura").val();
            break;

        case '14':
            NombreClaseI = $("#txtNombreInstalacion").val();
            Cantidad = $("#txtCantidadInstalacion").val();
            break;

        case '15':
            Cantidad = $("#txtCantidadPersonas").val();
            break;

        case '153':
            NombreClaseI = $("#txtNombreNarcotrafico").val();
            Id_ClasiNarcotrafico = $("#ddlClasiNarcotrafico").val();
            break;

        case '154':
            // Aquí puedes añadir lógica si aplica
            break;

        default:
            NombreClaseI = '';
            Id_modalidadI = 0;
            Cantidad = 0;
            Id_ClasiNarcotrafico = 0;
    }

    const Obj_NuevoIrisP1 = {
        CriminalidadId: $("#txtConsecutivoIris").val(),
        IdUnidad: $("#txtUndeLabora").val(),
        IdZona: $("#ddlZona").val(),
        IdentificacionInforma: $("#txtIdentificacion").val(),
        Celular: $("#txtTelefono").val(),
        IdTipoServicio: $("#ddlTipoServicio").val(),
        IdCuadrante: $("#txtCodIdCuadrante").val(),
        IdClase: IdClase,
        NombreClase: NombreClaseI,
        Id_modalidad: Id_modalidadI,
        CantidadIntegrantes: Cantidad,
        CaracteristicasGenerales: $("#txtCaractGenerales").val(),
        Vigente: 1,
        SiglaUnidad: $("#txtSiglaUnidad").val(),
        IdEstado: 2,
        IdFuente: $("#ddlFuente").val(),
        EntornoAfectado: $("#ddlEntono").val(),
        IdtiempoDelito: $("#ddlActividad").val(),
        Clasificacion: Id_ClasiNarcotrafico,
        Modalidadexpendio: Id_modalidadI,
        Origen: 'WEB',
        NombreEntornoAfectado: $("#txtNombreEntornoAfectado").val(),
        EspecialidadAporta: $("#ddlEspecialidad").val()
    };

    $.ajax({
        url: UrlInsRegistroIrisP1,
        type: 'POST',
        data: Obj_NuevoIrisP1,
        success: function (resp) {
            if (resp.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Señor(a) Funcionario(a):',
                    text: resp.message
                }).then(() => {
                    limpiarFormularioIris();
                });
            } else {
                Swal.fire('Error', 'Error al insertar: ' + resp.message, 'error');
            }
        },
        error: function () {
            Swal.fire('Error', 'Fallo en la llamada AJAX.', 'error');
        }
    });
}


function limpiarFormularioIrisP1() {

    $("#txtUndeLabora").val('');
    $("#txtCodDane").val('');
    $("#txtCodSiedcoEstacion").val('');
    $("#txtCodSiedcoCuadrante").val('');
    $("#txtCodEstacion").val('');
    $("#txtCodIdCuadrante").val('');
    $("#txtSiglaUnidad").val('');
    $("#txtConsecutivoIris").val('');
    $("#txtConsecutivoIntegrante").val('');
    $("#txtIdentificacion").val('');
    $("#txtFuncionario").val('');
    $("#txtUnidad").val('');
    $("#txtDependencia").val('');
    $("#ddlCuadrante").val('');
    $("#txtTelefono").val('');
    $("#ddlEspecialidad").val('');
    $("#ddlTipoServicio").val('');
    $("#ddlClase").val('');
    $("#txtNombreTrafico").val('');
    $("#ddlModExpendio").val('');
    $("#txtNombreEstructura").val('');
    $("#txtCantidadEstructura").val('');
    $("#txtNombreInstalacion").val('');
    $("#txtCantidadInstalacion").val('');
    $("#txtCantidadPersonas").val('');
    $("#txtNombreNarcotrafico").val('');
    $("#ddlClasiNarcotrafico").val('');
    $("#ddlActividad").val('');
    $("#ddlFuente").val('');
    $("#ddlEntono").val('');
    $("#txtNombreEntornoAfectado").val('');
    $("#ddlZona").val('');
    $("#txtCaractGenerales").val('');
    $("#txtIdentificacionInteg").val('');
    $("#txtApellidosInteg").val('');
    $("#txtNombreInteg").val('');
    $("#txtAlias").val('');
    $("#txtCelularInteg").val('');
    $("#txtDirecciónInteg").val('');
    $("#ddlDelitoPrincipal").val('');
    $("#ddlDelitoSecundario").val('');
    $('#tbGrillaIntegantes').empty();
    $('#pn_GrillaIntegantes').addClass('hidden').removeClass('show');
    $('#pn_GrillaIntegantes').collapse('hide');
    $("#txtMunicipio").val('');
    $("#txtBarrio").val('');
    $("#txtCuadrante").val('');
    $("#txtDireccion").val('');
    $("#LONGITUD_CASO").val('');
    $("#LATITUD_CASO").val('');
    $("#txtRadioAccion").val('');


}



    function mostrarNombreArchivo(input) {
                                    const archivo = input.files[0];
    if (archivo) {
        document.getElementById('nombreArchivo').value = archivo.name;
                                    } else {
        document.getElementById('nombreArchivo').value = "";
                                    }
                       }


