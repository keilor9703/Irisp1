let archivoSubido = null; // Guardará el archivo ya cargado

$(document).ready(function () {


    //if ($.fn.select2) {
    //    $('#ddlAnioIris').select2();
    //}

    // Asocia el evento change
    //$('#ddlAnioIris').on('change', function () {
    //    F_GetInfoGrillas();
    //});

    
    $('.select2').select2({
        placeholder: "Seleccione",
        allowClear: true
    });



});


function F_GetInfoGrillas() {
    $.ajax({
        type: 'GET',
        url: UrlGetInfoGrillas,
        dataType: 'json',
        data: { V_Anio: $('#ddlAnioIris').val() },
        success: function (response) {
            console.log("✅ Respuesta exitosa:", response);
            let data = response.data || [];
            GetGrillaVerificacion(data);
            GetGrillaInvestigacion(data);
            GetGrillaFinalizacion(data);
        },
        error: function (xhr, status, error) {
            console.error("❌ Error Ajax:", status, error);
            console.error("Respuesta cruda:", xhr.responseText);
            GetGrillaVerificacion([]);
            GetGrillaInvestigacion([]);
            GetGrillaFinalizacion([]);
        }
    });
}

/**** funcion para consultar listado de iris por año ******/
function ConsultarIrisAnio() {
    const txtAnio = $("#ddlAnioIris").val();
    let listaMensajes = "";

    if (!txtAnio || txtAnio.trim() === "") {
        listaMensajes += "<li>El campo año es obligatorio</li>";
    }

    if (listaMensajes !== "") {
        ModalError("Fallo", `<ul>${listaMensajes}</ul>`);
        return;
    }

    $.ajax({
        type: "POST",
        url: UrlConsultarAnioSeguimiento,
        data: { _anioSeguimiento: txtAnio },
        success: function (datos) {
            if (datos && datos.length > 0) {
                $("#pnSeguimiento").removeClass('hidden');
                $("#pnSeguimientoInv").removeClass('hidden');
                $("#pnSeguimientoFin").removeClass('hidden');
                $("#pnGrillaSeguimiento").removeClass('hidden');
                $("#pnGrillaSeguimientoInv").removeClass('hidden');
                $("#pnGrillaSeguimientoFin").removeClass('hidden');
                CargarTbSeguimiento(datos);
                GetGrillaInvestigacion(datos);
                GetGrillaFinalizacion(datos);
            } else {

                $("#pnGrillaSeguimiento").addClass('hidden');
                ModalInfo("Sin resultados", "No se encontraron hechos para el año seleccionado.");
            }
        },
        error: function (ex) {
            $("#pnSeguimiento").addClass('hidden');
            Swal.fire({
                type: 'info',
                title: 'Señor(a) Funcionario(a:)',
                text: "El año no tiene Iris registrados, ¿desea hacerlo?"
            });

        }

    });
}


/**** grilla verificacion  ******/
function CargarTbSeguimiento(datos) {
    $("#tbSeguimiento").DataTable({
        destroy: true,
        language: glOpcionesIdioma,
        responsive: true,
        paging: true,
        data: datos,
        initComplete: function () {
            $("#pnSeguimiento").removeClass('hidden');
            $("#pnGrillaverificacion").removeClass('hidden');
        },
        columns: [
            {
                data: null,
                className: "text-center",
                render: function (data, type, row) {
                    return `
                        <div class="dropdown dropend">
                            <button class="btn btn-azul btn-sm" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                <i class="fas fa-ellipsis-v"></i>
                            </button>
                            <ul class="dropdown-menu">
                                <li><a class="dropdown-item" href="#" onclick="EditarSeguimiento(${row.idSeguimiento}); return false;"><i class="fa fa-edit text-success"></i> Asignar</a></li>
                                <li><a class="dropdown-item" href="#" onclick="EliminarSeguimiento(${row.idSeguimiento}); return false;"><i class="fa fa-trash text-danger"></i> Finalizar</a></li>
                            </ul>
                        </div>
                    `;
                }
            },
            { title: "Estado", data: "estadoDescripcion", className: "d-none" },
            { title: "Estado Existencia", data: "estadoExistenciaDescripcion", className: "text-justify" },
            { title: "Codigo", data: "codigo", className: "text-justify" },
            { title: "Dependencia", data: "dependencia", className: "text-justify" },
            { title: "Municipio", data: "municipio", className: "text-center" },
            { title: "Fecha Inicio Actividad", data: "fechaInicioExistencia", className: "text-justify" },
            { title: "Clase", data: "clase", className: "text-justify" },
            { title: "Razón Social", data: "nombreClase", className: "text-center" },
            { title: "Cantidad", data: "cantidadIntegrantes", className: "text-center" },
            { title: "Caracteristicas Generales", data: "caracteristicasGenerales", className: "text-center" },
            { title: "Descripción Trámite", data: "descripcionTramite", className: "text-justify" },
            { title: "Zona", data: "zona", className: "text-justify" },
            { title: "Tipo Servicio", data: "tipoServicio", className: "text-justify" },
            { title: "Fuente", data: "fuente", className: "text-justify" },
            { title: "Fecha Creación", data: "fechaCreacion", className: "text-justify" },
            { title: "Unidad Verificación Existencia", data: "dependencia", className: "text-justify" },
            { title: "Fecha Asignación Verificación Existencia", data: "fechaAsignaciónVerificaciónExistencia", className: "text-justify" }, 
            { title: "Fecha Respuesta Verificación Existencia", data: "fechaRespuestaVerificaciónExistencia", className: "text-justify" }, 
            { title: "Contador Verificación Existencia", data: "vigente", className: "text-justify",
                render: function (data) {
                    if (!data) return "";
                    let fechaCreacion = new Date(data);
                    let hoy = new Date();

                    // Diferencia en milisegundos
                    let diff = hoy - fechaCreacion;

                    // Cálculo de días y horas
                    let dias = Math.floor(diff / (10000 * 60 * 60 * 24));
                    let horas = Math.floor((diff % (10000 * 60 * 60 * 24)) / (10000 * 60 * 60));

                    // Condición: más de 365 días -> convertir a años    // Convertir a años si pasa de 365 días
                    
                    if (dias >= 365) {
                        let años = Math.floor(dias / 365);
                        let diasRestantes = dias % 365;
                        return años + " años " + diasRestantes + " días " + horas + " horas";
                    } else {
                        return dias + " días " + horas + " horas";
                    }
                } },
            { title: "Unidad Proceso Investigativo", data: "unidadResponsable", className: "text-justify" },                        
            { title: "Fecha Asignación Proceso Investigativo", data: "fechaAsignaciónProcesoInvestigativo", className: "text-justify" },
            { title: "Fecha Respuesta Proceso Investigativo", data: "fecha Respuesta Proceso Investigativo", className: "text-justify" },
            { title: "Contador Proceso Investigativo", data: "contadorProcesoInvestigativo", className: "text-justify" },
            { title: "Resultados", data: "numeroResultado", className: "text-justify" }
        ],
        lengthMenu: [
            [10, 25, 50, -1],
            ['10 registros', '25 registros', '50 registros', 'Todos']
        ],
        ordering: false,
        pageLength: 10,
        searching: true,
        info: true
    });

}

 //Grilla investigacion  ////////////////////////////////////////////////////////////////
function GetGrillaInvestigacion(datos) {
    $("#tbSeguimientoInv").DataTable({
        destroy: true,
        language: glOpcionesIdioma,
        responsive: true,
        paging: true,
        data: datos,
        initComplete: function () {
            $("#pnSeguimientoInv").removeClass('hidden');
            $("#pnGrillaInvestigacion").removeClass('hidden');
        },
        columns: [
            {
                data: null,
                className: "text-center",
                render: function (data, type, row) {
                    return `
                        <div class="dropdown dropend">
                            <button class="btn btn-azul btn-sm" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                <i class="fas fa-ellipsis-v"></i>
                            </button>
                            <ul class="dropdown-menu">
                                <li><a class="dropdown-item" href="#" onclick="EditarSeguimiento(${row.idSeguimiento}); return false;"><i class="fa fa-edit text-success"></i> Asignar</a></li>
                                <li><a class="dropdown-item" href="#" onclick="EliminarSeguimiento(${row.idSeguimiento}); return false;"><i class="fa fa-trash text-danger"></i> Finalizar</a></li>
                            </ul>
                        </div>
                    `;
                }
            },
            { title: "Estado", data: "estadoDescripcion", className: "d-none" },
            { title: "Estado Existencia", data: "estadoExistenciaDescripcion", className: "text-justify" },
            { title: "Codigo", data: "codigo", className: "text-justify" },
            { title: "Dependencia", data: "dependencia", className: "text-justify" },
            { title: "Municipio", data: "municipio", className: "text-center" },
            { title: "Fecha Inicio Actividad", data: "fechaInicioExistencia", className: "text-justify" },
            { title: "Clase", data: "clase", className: "text-justify" },
            { title: "Razón Social", data: "nombreClase", className: "text-center" },
            { title: "Cantidad", data: "cantidadIntegrantes", className: "text-center" },
            { title: "Caracteristicas Generales", data: "caracteristicasGenerales", className: "text-center" },
            { title: "Descripción Trámite", data: "descripcionTramite", className: "text-justify" },
            { title: "Zona", data: "zona", className: "text-justify" },
            { title: "Tipo Servicio", data: "tipoServicio", className: "text-justify" },
            { title: "Fuente", data: "fuente", className: "text-justify" },
            { title: "Fecha Creación", data: "fechaCreacion", className: "text-justify" },
            { title: "Unidad Verificación Existencia", data: "dependencia", className: "text-justify" },
            { title: "Fecha Asignación Verificación Existencia", data: "fechaAsignaciónVerificaciónExistencia", className: "text-justify" }, 
            { title: "Fecha Respuesta Verificación Existencia", data: "fechaRespuestaVerificaciónExistencia", className: "text-justify" }, 
            {
                title: "Contador Verificación Existencia", data: "vigente", className: "text-justify",
                render: function (data) {
                    if (!data) return "";
                    let fechaCreacion = new Date(data);
                    let hoy = new Date();

                    // Diferencia en milisegundos
                    let diff = hoy - fechaCreacion;

                    // Cálculo de días y horas
                    let dias = Math.floor(diff / (10000 * 60 * 60 * 24));
                    let horas = Math.floor((diff % (10000 * 60 * 60 * 24)) / (10000 * 60 * 60));

                    // Condición: más de 365 días -> convertir a años    // Convertir a años si pasa de 365 días

                    if (dias >= 365) {
                        let años = Math.floor(dias / 365);
                        let diasRestantes = dias % 365;
                        return años + " años " + diasRestantes + " días " + horas + " horas";
                    } else {
                        return dias + " días " + horas + " horas";
                    }
                }
            },
            { title: "Unidad Proceso Investigativo", data: "unidadResponsable", className: "text-justify" },
            { title: "Fecha Asignación Proceso Investigativo", data: "fechaAsignaciónProcesoInvestigativo", className: "text-justify" },
            { title: "Fecha Respuesta Proceso Investigativo", data: "fecha Respuesta Proceso Investigativo", className: "text-justify" },
            { title: "Contador Proceso Investigativo", data: "contadorProcesoInvestigativo", className: "text-justify" },
            { title: "Resultados", data: "estadoResultados", className: "text-justify" }
        ],
        lengthMenu: [
            [10, 25, 50, -1],
            ['10 registros', '25 registros', '50 registros', 'Todos']
        ],
        ordering: false,
        pageLength: 10,
        searching: true,
        info: true
    });

}


 //Grilla finalizacion  ////////////////////////////////////////////////////////////////
function GetGrillaFinalizacion(datos) {
    $("#tbSeguimientoFin").DataTable({
        destroy: true,
        language: glOpcionesIdioma,
        responsive: true,
        paging: true,
        data: datos,
        initComplete: function () {
            $("#pnSeguimientoFin").removeClass('hidden');
            $("#pnGrillaFinalizacion").removeClass('hidden');
        },
        columns: [
            {
                data: null,
                className: "text-center",
                render: function (data, type, row) {
                    return `
                        <div class="dropdown dropend">
                            <button class="btn btn-azul btn-sm" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                <i class="fas fa-ellipsis-v"></i>
                            </button>
                            <ul class="dropdown-menu">
                                <li><a class="dropdown-item" href="#" onclick="EditarSeguimiento(${row.idSeguimiento}); return false;"><i class="fa fa-edit text-success"></i> Asignar</a></li>
                                <li><a class="dropdown-item" href="#" onclick="EliminarSeguimiento(${row.idSeguimiento}); return false;"><i class="fa fa-trash text-danger"></i> Finalizar</a></li>
                            </ul>
                        </div>
                    `;
                }
            },
            { title: "Estado", data: "estadoDescripcion", className: "d-none" },
            { title: "Estado Existencia", data: "estadoExistenciaDescripcion", className: "text-justify" },
            { title: "Codigo", data: "codigo", className: "text-justify" },
            { title: "Dependencia", data: "dependencia", className: "text-justify" },
            { title: "Municipio", data: "municipio", className: "text-center" },
            { title: "Fecha Inicio Actividad", data: "fechaInicioExistencia", className: "text-justify" },
            { title: "Clase", data: "clase", className: "text-justify" },
            { title: "Razón Social", data: "nombreClase", className: "text-center" },
            { title: "Cantidad", data: "cantidadIntegrantes", className: "text-center" },
            { title: "Caracteristicas Generales", data: "caracteristicasGenerales", className: "text-center" },
            { title: "Descripción Trámite", data: "descripcionTramite", className: "text-justify" },
            { title: "Zona", data: "zona", className: "text-justify" },
            { title: "Tipo Servicio", data: "tipoServicio", className: "text-justify" },
            { title: "Fuente", data: "fuente", className: "text-justify" },
            { title: "Fecha Creación", data: "fechaCreacion", className: "text-justify" },
            { title: "Unidad Verificación Existencia", data: "dependencia", className: "text-justify" },
            { title: "Fecha Asignación Verificación Existencia", data: "fechaAsignaciónVerificaciónExistencia", className: "text-justify" }, 
            { title: "Fecha Respuesta Verificación Existencia", data: "fechaRespuestaVerificaciónExistencia", className: "text-justify" }, 
            {
                title: "Contador Verificación Existencia", data: "vigente", className: "text-justify",
                render: function (data) {
                    if (!data) return "";
                    let fechaCreacion = new Date(data);
                    let hoy = new Date();

                    // Diferencia en milisegundos
                    let diff = hoy - fechaCreacion;

                    // Cálculo de días y horas
                    let dias = Math.floor(diff / (10000 * 60 * 60 * 24));
                    let horas = Math.floor((diff % (10000 * 60 * 60 * 24)) / (10000 * 60 * 60));

                    // Condición: más de 365 días -> convertir a años    // Convertir a años si pasa de 365 días

                    if (dias >= 365) {
                        let años = Math.floor(dias / 365);
                        let diasRestantes = dias % 365;
                        return años + " años " + diasRestantes + " días " + horas + " horas";
                    } else {
                        return dias + " días " + horas + " horas";
                    }
                }
            },
            { title: "Unidad Proceso Investigativo", data: "unidadResponsable", className: "text-justify" },
            { title: "Fecha Asignación Proceso Investigativo", data: "fechaAsignaciónProcesoInvestigativo", className: "text-justify" },
            { title: "Fecha Respuesta Proceso Investigativo", data: "fecha Respuesta Proceso Investigativo", className: "text-justify" },
            { title: "Contador Proceso Investigativo", data: "contadorProcesoInvestigativo", className: "text-justify" },
            { title: "Resultados", data: "numeroResultado", className: "text-justify" }
        ],
        lengthMenu: [
            [10, 25, 50, -1],
            ['10 registros', '25 registros', '50 registros', 'Todos']
        ],
        ordering: false,
        pageLength: 10,
        searching: true,
        info: true
    });

}


/**** funcion para cargar los hechos ******/
//function CargarTbIris(datos) {
//    $("#tbSeguimiento").DataTable({
//        destroy: true,
//        language: glOpcionesIdioma,
//        responsive: true,
//        paging: true,
//        data: datos,
//        initComplete: function () {
//            $("#pnSeguimiento").removeClass('hidden');
//            $("#pnGrillaSeguimiento").removeClass('hidden');
//        },
//        columns: [
//            {
//                data: null,
//                className: "text-center",
//                render: function (data, type, row) {
//                    return `
//                        <div class="dropdown dropend">
//                            <button class="btn btn-success btn-sm" type="button" data-bs-toggle="dropdown" aria-expanded="false">
//                                <i class="fas fa-ellipsis-v"></i>
//                            </button>
//                            <ul class="dropdown-menu">
//                                <li><a class="dropdown-item" href="#" onclick="EditarHecho(${row.idSeguimiento}); return false;"><i class="fa fa-edit text-success"></i> Editar</a></li>
//                                <li><a class="dropdown-item" href="#" onclick="mostrarModalActuaciones(${row.idHecho}); return false;"><i class="fa fa-edit text-success"></i> Agregar Activación al hecho</a></li>
//                                <li><a class="dropdown-item" href="#" onclick="EliminarHecho(${row.idHecho}); return false;"><i class="fa fa-trash text-danger"></i> Eliminar</a></li>
//                            </ul>
//                        </div>
//                    `;
//                }
//            },
//            { title: "ID", data: "idHecho", className: "d-none" },
//            { title: "Tipo Hecho", data: "tipoHecho", className: "text-justify" },
//            { title: "Medio Agresor", data: "medioAgresor", className: "text-justify" },
//            { title: "Presunto Agresor", data: "presuntoAgresor", className: "text-justify" },
//            { title: "Fecha Hechos", data: "fechaHechos", className: "text-center" },
//            { title: "Departamento", data: "departamento", className: "text-justify" },
//            { title: "Municipio", data: "municipio", className: "text-justify" },
//            { title: "Latitud", data: "latitud", className: "text-center" },
//            { title: "Longitud", data: "longitud", className: "text-center" },
//            { title: "Descripción Hechos", data: "descripcionHechos", className: "text-justify" }
//        ],
//        lengthMenu: [
//            [10, 25, 50, -1],
//            ['10 registros', '25 registros', '50 registros', 'Todos']
//        ],
//        ordering: false,
//        pageLength: 10,
//        searching: true,
//        info: true
//    });
//}