let archivoSubido = null; // Guardará el archivo ya cargado
let EstadoExistencia = null; // Guardará el archivo ya cargado

$(document).ready(function () {

    // Inicializa select2 generales
    if ($.fn.select2) {
        $('#ddlAnioIris').select2();
    }

    $('.select2').select2({
        placeholder: "Seleccione",
        allowClear: true
    });

    $('#ddlTipoUnidad').select2();

 
    $('#ddlAnioIris').on('change', function () {
        F_GetInfoGrillas();
    });

    F_GetInfoGrillas($('#ddlAnioIris').val());


    $('#chkRegFoto').change(function () {
        if ($(this).is(':checked')) {
            $('#fotografia').closest('.col-md-4').removeClass('hidden');
        } else {
            $('#fotografia').closest('.col-md-4').addClass('hidden');
        }
    });



    //Onclicks
    
    //$("#btnNuevo").on("click", function (e) {
    //    e.preventDefault();
    //    AbrirModalNuevoIris();
    //});
    $("#btnNuevoResultado").on("click", function (e) {
        e.preventDefault();
        OpenInsResponsableValModal();
    });
    $("#btnGuardarResponsable").on("click", function (e) {
        e.preventDefault();
        P_InsResponsabeValModal();
    });
    $("#btnActualizarResponsable").on("click", function (e) {
        e.preventDefault();
        P_UpdUnidadResponsable();
    });
    $("#btnEvalTarea").on("click", function (e) {
        e.preventDefault();
        P_EvalTarea();
    });
    $("#btnReasignarTarea").on("click", function (e) {
        e.preventDefault();
        P_ReasignarTarea();
    });





});

// Manejo genérico para cualquier modal secundaria
$(document).on('hidden.bs.modal', '.modal', function () {
    // Verifica si todavía hay alguna modal abierta
    if ($('.modal.show').length > 0) {
        $('body').addClass('modal-open');
    }
});


function AbrirModalVisualizarTexto(Texto) {

    // Mostrar la modal
    $('#Modal_VisualizarTexto').modal("show");
    $('#txtDescripcion').val(Texto);
}

//función para dar formato de fecha a las columnas que de las grillas
function formatDate(dateStr) {
    if (!dateStr) return ""; // si viene null o vacío
    const fecha = new Date(dateStr);
    if (isNaN(fecha)) return dateStr; // si no es fecha válida, devuelvo lo que llegó
    return fecha.toLocaleString("es-CO", {
        year: "numeric",
        month: "2-digit",
        day: "2-digit",
        hour: "2-digit",
        minute: "2-digit",
        second: "2-digit",
        hour12: true // 👈 esto activa AM/PM
    });
}




/// Evento al hacer clic en el botón de descarga
$('#btnDescargarExcel').on('click', function () {
    Swal.fire({
        title: 'Confirmación de descarga',
        text: 'Este archivo contiene información confidencial. Su descarga será registrada.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Aceptar y descargar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {

      console.log(result);
        // ✅ Asegúrate de que el bloque esté bien indentado y contenido
        if (result.isConfirmed) {


            try {
                const tablas = [
                    { id: '#tbGrilla', nombre: 'Verificación' },
                    { id: '#tbGrillaInvestigacion', nombre: 'Investigación' },
                    { id: '#tbGrillaFinalizacion', nombre: 'Finalización' }
                ];

                const wb = XLSX.utils.book_new();
                let hayDatos = false;

                tablas.forEach(t => {
                    const table = $(t.id).DataTable();
                    if (!table) return;

                    const datosFiltrados = table.rows({ search: 'applied' }).data().toArray();
                    const columnasVisibles = table.columns().indexes().filter(idx => table.column(idx).visible());

                    if (datosFiltrados.length > 0 && columnasVisibles.length > 0) {
                        hayDatos = true;

                        const datosVisibles = datosFiltrados.map(row => {
                            const fila = {};
                            columnasVisibles.each(idx => {
                                const nombreColumna = table.column(idx).header().textContent.trim();
                                const propiedad = table.column(idx).dataSrc();
                                let valor = row[propiedad];

                                if (typeof valor === 'string' && /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}/.test(valor)) {
                                    const fecha = new Date(valor);
                                    valor = fecha.toLocaleString('es-CO', {
                                        day: '2-digit',
                                        month: '2-digit',
                                        year: 'numeric',
                                        hour: '2-digit',
                                        minute: '2-digit',
                                        second: '2-digit',
                                        hour12: true
                                    });
                                }

                                fila[nombreColumna] = valor;
                            });
                            return fila;
                        });

                        const hoja = XLSX.utils.json_to_sheet(datosVisibles);
                        XLSX.utils.book_append_sheet(wb, hoja, t.nombre);
                    }
                });

                if (!hayDatos) {
                    Swal.fire('Sin datos', 'No hay registros filtrados para exportar.', 'warning');
                    return;
                }

                const anio = $('#ddlAnioIris').val() || new Date().getFullYear();
                const nombreArchivo = `Reporte_IrisP1_Seguimiento_${anio}_Filtrado.xlsx`;
                XLSX.writeFile(wb, nombreArchivo);

                Swal.fire({
                    icon: 'success',
                    title: 'Descarga completa',
                    text: 'El archivo Excel se ha generado exitosamente.',
                    timer: 2000,
                    showConfirmButton: false
                });
            } catch (e) {
                console.error(e);
                Swal.fire({
                    icon: 'error',
                    title: 'Error al generar Excel',
                    text: 'Hubo un problema al generar el archivo.'
                });
            }
        }
    });
});

function F_GetInfoGrillas() {
    $.ajax({
        type: 'GET',
        url: AppRoutes.Seguimiento.UrlGetInfoGrillas,
        dataType: 'json',
        data: { V_Anio: $('#ddlAnioIris').val() },
        success: function (response) {
            //console.log("✅ Respuesta exitosa:", response);
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

// 🔧 Función utilitaria para inicializar o refrescar tablas
function renderDataTable(selector, datosFiltrados, columnas) {
    if ($.fn.dataTable.isDataTable(selector)) {
        // actualizar data en vez de recrear
        const table = $(selector).DataTable();
        table.clear();
        table.rows.add(datosFiltrados);
        table.draw();
        return;
    }

    $(selector).DataTable({
        data: datosFiltrados,
        language: glOpcionesIdioma,
        scrollX: true,
        // scrollY: 400,          // alto fijo para habilitar virtualización
        scroller: true,        // solo renderiza las filas visibles
        deferRender: true,     // retrasa render hasta que sean visibles
        autoWidth: false,
        responsive: false,

        columnDefs: [
            { targets: '_all', className: 'dt-head-center dt-body-center' },
            { targets: 3, width: '1%', className: 'no-wrap' }
        ],
        columns: columnas,

        lengthMenu: [
            [10, 25, 50, 100],
            ['10 registros', '25 registros', '50 registros', '100 registros']
        ],
        pageLength: 10,
        ordering: true,
        searching: true,
        paging: true,
        info: true
    });
}

function GetGrillaVerificacion(Datos) {
    const datosFiltrados = Datos.filter(item => [2, 3, 4].includes(item.IdEstado));
    $("#pn_GrillaVerificacion").removeClass('hidden');

    renderDataTable("#tbGrilla", datosFiltrados, [
        columnaAcciones(datosFiltrados),
        Estados(),
        EstadosExistencia(),
        { title: "Codigo", data: "Codigo" },
        { title: "Unidad Verificación", data: "UnidadResponsable" },
        { title: "Dependencia", data: "Dependencia" },
        { title: "Municipio", data: "Municipio" },
        { title: "Fecha Inicio Actividad", data: "FechaInicioExistencia", render: formatDate},
        { title: "Clase", data: "Clase" },
        { title: "Nombre", data: "NombreClase" },
        { title: "Cantidad", data: "CantidadIntegrantes" },
        columnaCaracteristicasGenerales(),
        columnaDescripcionTramite(),
        { title: "Zona", data: "Zona" },
        { title: "Tipo Servicio", data: "TipoServicio" },
        { title: "Fuente", data: "Fuente" },
        { title: "Fecha de Creacion", data: "FechaCreacion",render: formatDate },
        Resultados(),
        { title: "CriminalidadId", data: "CriminalidadId", visible: false }
    ]);
}

function GetGrillaInvestigacion(Datos) {
    const datosFiltrados = Datos.filter(item => [72, 73].includes(item.IdEstado));
    $("#pn_GrillaInvestigacion").removeClass('hidden');

    renderDataTable("#tbGrillaInvestigacion", datosFiltrados, [
        columnaAcciones(datosFiltrados),
        Estados(),
        EstadosExistencia(),
        { title: "Codigo", data: "Codigo" },
        { title: "Unidad Verificación", data: "UnidadResponsable" },
        { title: "Dependencia", data: "Dependencia" },
        { title: "Municipio", data: "Municipio" },
        { title: "Fecha Inicio Actividad", data: "FechaInicioExistencia", render: formatDate },
        { title: "Clase", data: "Clase" },
        { title: "Nombre", data: "NombreClase" },
        { title: "Cantidad", data: "CantidadIntegrantes" },
        columnaCaracteristicasGenerales(),
        columnaDescripcionTramite(),
        { title: "Zona", data: "Zona" },
        { title: "Tipo Servicio", data: "TipoServicio" },
        { title: "Fuente", data: "Fuente" },
        { title: "Fecha de Creacion", data: "FechaCreacion", render: formatDate },
        Resultados(),
        { title: "CriminalidadId", data: "CriminalidadId", visible: false }
    ]);
}

function GetGrillaFinalizacion(Datos) {
    const datosFiltrados = Datos.filter(item => [5].includes(item.IdEstado));
    $("#pn_GrillaFinalizacion").removeClass('hidden');

    renderDataTable("#tbGrillaFinalizacion", datosFiltrados, [
        columnaAcciones(datosFiltrados),
        Estados(),
        EstadosExistencia(),
        { title: "Codigo", data: "Codigo" },
        { title: "Unidad Verificación", data: "UnidadResponsable" },
        { title: "Dependencia", data: "Dependencia" },
        { title: "Municipio", data: "Municipio" },
        { title: "Fecha Inicio Actividad", data: "FechaInicioExistencia", render: formatDate },
        { title: "Clase", data: "Clase" },
        { title: "Nombre", data: "NombreClase" },
        { title: "Cantidad", data: "CantidadIntegrantes" },
        columnaCaracteristicasGenerales(),
        columnaDescripcionTramite(),
        { title: "Zona", data: "Zona" },
        { title: "Tipo Servicio", data: "TipoServicio" },
        { title: "Fuente", data: "Fuente" },
        { title: "Fecha de Creacion", data: "FechaCreacion", render: formatDate },
        Resultados(),
        { title: "CriminalidadId", data: "CriminalidadId", visible: false }
    ]);
}





function Estados() {
    return {
        title: "Estado",
        data: "EstadoDescripcion",
        name: "EstadoDescripcion",
        autoWidth: true,
        render: function (data, type, row) {
            // Si el estado viene vacío o nulo
            if (!data) {
                return `<span style="background-color: #808080; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 120px;">Por establecer</span>`;
            }

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

            return `<span style="background-color: ${color}; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 120px;">${data}</span>`;
        }
    };
}

function EstadosExistencia() {
    return {
        title: "Estado Existencia",
        data: "EstadoExistenciaDescripcion",
        name: "EstadoExistenciaDescripcion",
        autoWidth: true,
        render: function (data, type, row) {
            // Si el estado viene vacío o nulo
            if (!data) {
                return `<span style="background-color: #808080; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 120px;">Por establecer</span>`;
            }

            const estado = data.toLowerCase();
            let color = '';

            switch (estado) {
                case 'no existe':
                    color = '#c53a1d'; // rojo
                    break;

                case 'si existe':
                    color = '#236305'; // verde
                    break;
                default:
                    color = '#386ca0'; // gris oscuro
            }

            return `<span style="background-color: ${color}; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 120px;">${data}</span>`;
        }
    };
}

function columnaAcciones(datosFiltrados) {
    return {
        data: datosFiltrados,
        "autoWidth": true,
        render: function (data, type, row) {

            // Guardamos el objeto en un atributo data de forma segura
            // Reemplazamos comillas dobles por &quot; para no romper el HTML
            var DatosFila = JSON.stringify(row).replace(/"/g, '&quot;');

            var inicioBoton = '<div class="dropdown dropend">' +
                '<button class="btn btn-success" type="button" id="dropdownMenuButton1" ' +
                'data-bs-toggle="dropdown" aria-expanded="false">' +
                '<span class="fas fa-list"></span></button>' +
                '<ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';

            var Asignar = `<li style="padding-left: 15px;">
                                    <a style="color: #102717;" href="#"
                                       class="btn-asignar-iris"
                                       data-datos="${DatosFila}">
                                        <i class="fas fa-list"></i>&nbsp; Asignar
                                    </a>
                                </li>`;

            var Finalizar = `<li style="padding-left: 15px;">
                    <a style="color: #102717;" href="javascript:FinalizarIris('${row.CriminalidadId}')">
                        <i class="fa fa-times-circle" style="color:red;"></i>&nbsp;Finalizar
                    </a>
                 </li>`;
         

            var finBoton = '</ul></div>';
            return inicioBoton + Asignar + Finalizar + finBoton;
        }
    }
}


// Delegación de eventos para los botones de detalle
$(document).on("click", ".btn-asignar-iris", function (e) {
    e.preventDefault();

    // Recuperamos el JSON guardado en data-datos
    var datosAttr = $(this).attr("data-datos").replace(/&quot;/g, '"');

    try {
        var registro = JSON.parse(datosAttr);
        F_GetDetalleIris(registro);
    } catch (err) {
        console.error("❌ Error parseando data-datos:", err, datosAttr);
        Swal.fire('Error', 'No se pudo procesar el detalle del registro', 'error');
    }
});


function columnaCaracteristicasGenerales() {
    return {
        title: "Características Generales",
        data: "CaracteristicasGenerales",
        name: "CaracteristicasGenerales",
        "autoWidth": true,
        render: function (data, type, row) {
            if (!data || data.trim() === "") {
                return '';
            }

            const dataEncoded = encodeURIComponent(data);

            return `
                <div style="display: flex; align-items: center; max-width: 100px; gap: 10px;">
                    <button class="btn btn-success btn-sm" type="button"
                        onclick="AbrirModalVisualizarTexto(decodeURIComponent('${dataEncoded}'))">
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
        "autoWidth": true,
        render: function (data, type, row) {
            if (!data || data.trim() === "") {
                return '';
            }

            const dataEncoded = encodeURIComponent(data);

            return `
                <div style="display: flex; align-items: center; max-width: 100px; gap: 10px;">
                    <button class="btn btn-success btn-sm" type="button"
                        onclick="AbrirModalVisualizarTexto(decodeURIComponent('${dataEncoded}'))">
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


function columnaInforAdicionalDetalleIris() {
    return {
        title: "Descripción",
        data: "Descripcion",
        name: "Descripcion",
        className: "celdaCenter celda50",
        render: function (data, type, row) {
            if (!data || data.trim() === "") {
                return '';
            }

            const dataEncoded = encodeURIComponent(data);

            return `
                <div style="
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    gap: 10px;
                    max-width: 900px;
                    margin: 0 auto;
                    text-align: center;
                ">
                    <button class="btn btn-success btn-sm" type="button"
                        onclick="AbrirModalVisualizarTexto(decodeURIComponent('${dataEncoded}'))"
                        style="flex-shrink: 0;">
                        <span class="fa fa-eye white"></span>
                    </button>
                    <div style="
                        white-space: nowrap;
                        overflow: hidden;
                        text-overflow: ellipsis;
                        max-width: 700px;
                    " title="${data}">
                        ${data}
                    </div>
                </div>
            `;
        }
    };
}

function Contador1() {
    return {
        title: "Contador Verificación",
        data: "ContadorVerificacionExistencia",
        name: "ContadorVerificacionExistencia",

        autoWidth: true,
        render: function (data, type, row) {
            // Si el estado viene vacío o nulo
            if (!data) {
                return `<span style="background-color: #808080; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px;">Por establecer</span>`;
            }

            const estado = data.toLowerCase();
            let color = '';

            if (estado.includes('no asignado')) {
                color = '#c53a1d'; //  Rojo
            } else {
                color = '#236305'; // verde
            }

            return `<span style="background-color: ${color}; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px;">${data}</span>`;
        }
    };
}

function Contador2() {
    return {
        title: "Contador Investigativo",
        data: "ContadorProcesoInvestigativo",
        name: "ContadorProcesoInvestigativo",

        autoWidth: true,
        render: function (data, type, row) {
            // Si el estado viene vacío o nulo
            if (!data) {
                return `<span style="background-color: #808080; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px;">Por establecer</span>`;
            }

            const estado = data.toLowerCase();
            let color = '';

            if (estado.includes('no asignado')) {
                color = '#c53a1d'; //  Rojo
            } else {
                color = '#236305'; // verde
            }

            return `<span style="background-color: ${color}; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px;">${data}</span>`;
        }
    };
}
function Resultados() {
    return {
        title: "Resultados",
        data: "Resultados",
        name: "Resultados",
        className: "celdaJust",
        autoWidth: true,
        render: function (data, type, row) {
            // Si el estado viene vacío o nulo
            if (!data) {
                return `<span style="background-color: #808080; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px;">Por establecer</span>`;
            }

            const estado = data.toLowerCase();
            let color = '';

            if (estado.includes('tiene resultados (')) {
                color = '#236305'; // verde
            } else {
                color = '#c53a1d'; // gris oscuro por defecto
            }

            return `<span style="background-color: ${color}; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px;">${data}</span>`;
        }
    };
}

function F_GetDetalleIris(registro) {
    //console.log("✅ Registro recibido:", registro);

    $("#txtCriminalidadIdModal").val(registro.CriminalidadId);
   

    var FechaInicio = registro.FechaInicioExistencia ? moment(registro.FechaInicioExistencia).format('DD/MM/YYYY hh:mm:ss a') : '';
    var FechaCreacion = registro.FechaCreacion ? moment(registro.FechaCreacion).format('DD/MM/YYYY hh:mm:ss a') : '';

    $("#txtCodigoIrispi").text(registro.Codigo);
    $("#txtClaseHeader").text(registro.Clase);
    $("#txtClaseDetalle").text(registro.Clase);
    $("#txtNombreClaseHeader").text(registro.NombreClase);
    $("#txtNombreClaseDetalle").text(registro.NombreClase);
    $("#txtCantidad").text(registro.CantidadIntegrantes);
    $("#txtCantidadDetalle").text(registro.CantidadIntegrantes);
    $("#txtFuente").text(registro.Fuente);
    $("#txtFechaInicio").text(FechaInicio);
    $("#txtCaracteristicas").text(registro.CaracteristicasGenerales);
    $("#txtCelularIris").text(registro.Celular);
    $("#txtCodigoCuadrante").text(registro.Cuadrante);
    $("#txtEstacion1").text(registro.DependCuadrante);
    $("#txtEstacion2").text(registro.Estacioncuadrante);
    $("#txtComando").text(registro.Nivel1cuadrante);
    $("#txtCelularCuadrante").text(registro.CelularCuadrante);
    $("#txtFechaCreacion").text(FechaCreacion);


    // ✅ Renderiza el badge con color (no solo texto)
    $("#txtEstado").html(RenderEstadoBadge(registro.EstadoDescripcion));

    // ✅ Lo mismo para Estado Existencia
    $("#txtEstadoExistencia").html(RenderEstadoBadge(registro.EstadoExistenciaDescripcion));
   
    if (!registro.EstadoExistenciaDescripcion) {
        EstadoExistencia = 'Por establecer';
    } else {
        EstadoExistencia = registro.EstadoExistenciaDescripcion;
    }


    

    var IdenInforma = registro.IdentificacionInforma;

    $.ajax({
        type: "GET",
        url: AppRoutes.Seguimiento.UrlGetFuncionarios,
        async: true,
        data: { V_Identificacion: IdenInforma },
        dataType: 'json',
        cache: false,
        success: function (respuesta) {
            if (respuesta.success) {
                $("#txtFuncionarioDetalle").text(respuesta.data.Funcionario);
                $("#txtUnidadDetalle").text(respuesta.data.Fisica + " - " + respuesta.data.Dependencia);
                $("#txtUnidadDetalle2").text(respuesta.data.Fisica + " - " + respuesta.data.Dependencia);
                $("#txtCorreo").text(respuesta.data.Correo);
                $("#txtCelularSiath").text(respuesta.data.Celular);

                $('#Modal_AsignarIris').modal("show");

               
                F_GetUbicacionIris(registro.CriminalidadId);
                F_GetIntegrantesIris(registro.CriminalidadId);
                F_GetDelitosIris(registro.CriminalidadId);
                F_GetDocumentosIris(registro.CriminalidadId);
                F_GetInfoAdiconalIris(registro.CriminalidadId);
              
               
                F_GetTareas(registro.CriminalidadId)
                F_GetResultados(registro.CriminalidadId);
               F_GetResponsableIris(registro.CriminalidadId)
            } else {
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


// 1️⃣ Función utilitaria para renderizar el estado (extraída de Estados().render)
function RenderEstadoBadge(estadoTexto) {
    if (!estadoTexto) {
        return `<span style="background-color: #808080; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 120px;">Por establecer</span>`;
        
    }

    
    const estado = estadoTexto.toLowerCase();
    let color = '';

    switch (estado) {
        case 'sin asignar':
            color = '#c53a1d'; // rojo
            break;
        case 'asignado':
            color = '#236305'; // verde oscuro
            break;
        case 'si existe':
            color = '#236305'; // verde oscuro
            break;
        case 'no existe':
            color = '#c53a1d'; // verde oscuro
            break;
        case 'avance verificación':
            color = '#799137'; // verde oliva
            break;
        case 'investigación':
            color = '#2127f5'; // azul fuerte
            break;
        case 'avance investigación':
            color = '#40a8c7'; // celeste
            break;
        case 'finalizado':
            color = '#032b57'; // azul oscuro
            break;
        default:
            color = '#386ca0'; // gris
    }

    return `<span style="background-color: ${color}; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 120px;">${estadoTexto}</span>`;
}

// 2️⃣ Función principal de detalle

function F_GetUbicacionIris(CrininalidadId) {


    $.ajax({
        type: 'GET',
        url: AppRoutes.Seguimiento.UrlGetUbicacion,
        async: true,
        data: { V_CriminalidadId: CrininalidadId },
        dataType: 'json',
        success: function (response) {
            if (response.success) {

                GetGrillaUbicacionIris(response.data);
            } else {
                GetGrillaUbicacionIris([]);
                // Swal.fire('Error', response.message, 'error');
            }
        },
        error: function () {
            GetGrillaUbicacionIris([]);
            Swal.fire('Error', 'No se pudo obtener la lista de ubicaciones.', 'error');
        }
    });
}

function GetGrillaUbicacionIris(Datos) {
    if ($.fn.dataTable.isDataTable("#tbGrillaUbicacion")) {
        $("#tbGrillaUbicacion").DataTable().destroy();
    }

    $("#tbGrillaUbicacion").empty();

    $("#tbGrillaUbicacion").DataTable({
        destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        responsive: true,
        columns: [
            {
                title: "Mapa",
                data: null,
                className: "celdaCenter celda3",
                render: function (data, type, row) {
                    return `
                        <button class="btn btn-success btn-sm btn-mapa" 
                                title="Ver en mapa" 
                                onclick="OpenInsUbicacionModal('${row.Latitud}', '${row.Longitud}')">
                            <i class="fas fa-map-marker-alt"></i>
                        </button>`;
                }
            },
            { title: "Longitud", data: "Longitud", class: "celdaCenter" },
            { title: "Latitud", data: "Latitud", class: "celdaCenter" },
            { title: "Radio", data: "RadioAccion", class: "celdaCenter" },
            { title: "Municipio", data: "MunicipioUbica", class: "celdaCenter" },
            { title: "Cuadrante", data: "Cuadrante", class: "celdaCenter" },
            { title: "Dirección", data: "Direccion", class: "celdaCenter" }
        ],
        lengthChange: false,
        searching: false,
        ordering: false,
        pageLength: 10,
        paging: false,
        info: false
    });
}

function OpenInsUbicacionModal(latitud, longitud) {
    console.log("🗺️ Abriendo modal con coordenadas:", latitud, longitud);

    // Abre el modal
    $('#myModal').modal("show");

    // Evento al mostrarse completamente
    $('#myModal').off('shown.bs.modal').on('shown.bs.modal', function () {
        //console.log("✅ Modal visible, inicializando mapa...");

        // Inicializa el mapa
        inicializarMapa('mapaDiv');

        // Espera un poco a que cargue ArcGIS y luego centra el muñeco verde
        setTimeout(() => {
            if (typeof window.ubicarLlamadaEnMapa === "function") {
                window.ubicarLlamadaEnMapa(latitud, longitud);
            }
        }, 800);
    });

    // 🧹 Evento al cerrar completamente el modal: destruir mapa
    $('#myModal').off('hidden.bs.modal').on('hidden.bs.modal', function () {
       // console.log("🧹 Modal cerrado, destruyendo mapa...");

        // Si existe un mapa, destrúyelo correctamente
        if (typeof map !== "undefined" && map) {
            try {
                map.destroy();   // Libera memoria del objeto ArcGIS
               // console.log("🧭 Mapa destruido correctamente");
            } catch (err) {
               // console.warn("⚠️ Error al destruir mapa:", err);
            }
        }

        // Limpia el contenedor HTML del mapa (para crear uno nuevo luego)
        $("#mapaDiv").empty();

        // Elimina la variable global 'map' para asegurar una nueva instancia
        window.map = undefined;
    });
}

function F_GetIntegrantesIris(CriminalidadId) {


    $.ajax({
        type: 'GET',
        url: AppRoutes.Seguimiento.UrlGetIntegrantes,
        async: true,
        data: { V_CriminalidadId: CriminalidadId },
        dataType: 'json',
        success: function (response) {
            if (response.success) {

                GetGrillaIntegrantesIris(response.data);
            } else {
                GetGrillaIntegrantesIris([]);
                // Swal.fire('Error', response.message, 'error');
            }
        },
        error: function () {
            GetGrillaIntegrantesIris([]);
            Swal.fire('Error', 'No se pudo obtener la lista de integrantes.', 'error');
        }
    });
}

function GetGrillaIntegrantesIris(Datos) {
    if ($.fn.dataTable.isDataTable("#tbGrillaIntegrantes")) {
        $("#tbGrillaIntegrantes").DataTable().destroy();
    }

    $("#tbGrillaIntegrantes").empty();
    

    $("#tbGrillaIntegrantes").DataTable({
        destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        responsive: true,
        "columns": [
            {
                data: null, className: "celdaCenter celda3", "render": function (data, type, row) {
                    var inicioBoton = '<div class="dropdown dropend"><button class="btn btn-success" type="button" id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false"><span class="fas fa-list"></span></button><ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';
                    var Antecedentes = `<li style="padding-left: 17px;">
                                        <a style="color: #102717; cursor:pointer;" onclick="F_GetAntecedentes('${row.INTEGRANTE_ID}')">
                                        <i class="fa fa-trash red"></i>&nbsp;Antecedentes
                                        </a>
                                    </li>`;

                    var finBoton = '</ul></div>';
                    return inicioBoton + Antecedentes + finBoton;
                }
            },
            { "title": "Alias", "data": "ALIAS", class: "celdaCenter" },
            { "title": "Nombre", "data": "NOMBRE", class: "celdaCenter" },
            { "title": "Apellido", "data": "APELLIDO", class: "celdaCenter" },
            { "title": "Cédula", "data": "CEDULA", class: "celdaCenter" },
            { "title": "Dirección", "data": "DIRECCION", class: "celdaCenter" },
            { "title": "Fecha Creación", "data": "FECHA_CREACION", render: formatDate, class: "celdaCenter" }
           
        ],
        lengthMenu: [
            [5, 10, 25, 50, -1],
            ['5 registros', '10 registros', '25 registros', '50 registros', 'Todos']
        ],
        lengthChange: true,
        searching: true,
        ordering: true,
        pageLength: 10,
        paging: true,
        info: true
    });
}

function F_GetDelitosIris(CriminalidadId) {
    $.ajax({
        type: 'GET',
        url: AppRoutes.Seguimiento.UrlGetDelitosIris, // URL del endpoint que devuelve los datos
        dataType: 'json',
        data: { V_CriminalidadId: CriminalidadId },
        success: function (response) {

            // Inicializar la grilla con los datos filtrados o vacíos
            GetGrillaDelitosIris(response.data);

        },
        error: function () {
            // En caso de error, inicializar la grilla con datos vacíos
            GetGrillaDelitosIris([]);

        }
    });
}

function GetGrillaDelitosIris(Datos) {
    if ($.fn.dataTable.isDataTable("#tbGrillaDelitos")) {
        $("#tbGrillaDelitos").DataTable().destroy();
    }

    $("#tbGrillaDelitos").empty();
   

    $("#tbGrillaDelitos").DataTable({
        destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        responsive: true,
        "columns": [
            //{
            //    data: null, className: "celdaCenter celda3", "render": function (data, type, row) {
            //        var inicioBoton = '<div class="dropdown dropend"><button class="btn btn-success" type="button" id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false"><span class="fas fa-list"></span></button><ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';
            //        var Eliminar = `<li style="padding-left: 17px;"><a style="color: #102717;" href="javascript:P_DelDelitosIris('${row.DelitoId}')"><i class="fa fa-trash red"></i>&nbsp;Eliminar</a></li>`;
            //        var finBoton = '</ul></div>';
            //        return inicioBoton + Eliminar + finBoton;
            //    }
            //},
            { "title": "Delito", "data": "DelitoDesc" },
            { "title": "Tipo", "data": "DescTipo" },
            { "title": "Tipo Informacón", "data": "DescTipoInfo" }

        ],

        lengthChange: true,
        searching: true,
        ordering: true,
        pageLength: 10,
        paging: true,
        info: true
    });
}




function F_GetDocumentosIris(CriminalidadId) {
    $.ajax({
        type: 'GET',
        url: AppRoutes.Seguimiento.UrlGetDocIris, // URL del endpoint que devuelve los datos
        dataType: 'json',
        data: { V_CriminalidadId: CriminalidadId },
        success: function (response) {

            // Inicializar la grilla con los datos filtrados o vacíos
            GetGrillaDocumentosIris(response.data);

        },
        error: function () {
            // En caso de error, inicializar la grilla con datos vacíos
            GetGrillaDocumentosIris([]);

        }
    });
}

function GetGrillaDocumentosIris(Datos) {
    if ($.fn.dataTable.isDataTable("#tbGrillaDocumento")) {
        $("#tbGrillaDocumento").DataTable().destroy();
    }

    $("#tbGrillaDocumento").empty();
   
    
    $("#tbGrillaDocumento").DataTable({
        destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        responsive: true,
        "columns": [
            //{
            //    data: null, className: "celdaCenter celda3", "render": function (data, type, row) {
            //        var inicioBoton = '<div class="dropdown dropend"><button class="btn btn-success" type="button" id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false"><span class="fas fa-list"></span></button><ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';
            //        var Eliminar = `<li style="padding-left: 17px;"><a style="color: #102717;" href="javascript:P_DelDocumentoIris('${row.DocumentoId}')"><i class="fa fa-trash red"></i>&nbsp;Eliminar</a></li>`;
            //        var finBoton = '</ul></div>';
            //        return inicioBoton + Eliminar + finBoton;
            //    }
            //},
            { "title": "Nombre", "data": "nombre", "name": "nombre", className: "celdaCenter celda5" },
            {
                "title": "Enlace",
                "data": "ruta",
                "name": "ruta",
                className: "celdaCenter celda7",
                "render": function (data, type, row) {
                    if (!data || data.trim() === "") {
                        return '';
                    }


                    // Opción 1: enlace azul visible sobre fondo blanco
                    /*  return `<a href="${data}" target="_blank" style="color: #007bff; font-weight: bold; text-decoration: underline;">Descargar</a>`;*/
                    return `<a href="Irisp1/RegistroIrisp1/descargar?ruta=${encodeURIComponent(data)}" target="_blank" style="background-color: #236305; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 200px; text-decoration: none;">Descargar</a>`;
                }
            },
            { "title": "Fecha Creación", "data": "fechaCreacion", "name": "fechaCreacion", className: "celdaCenter celda17", render: formatDate }
        ],
        lengthChange: false,
        searching: false,
        ordering: false,
        pageLength: 10,
        paging: false,
        info: false
    });
}


function F_GetInfoAdiconalIris(CriminalidadId) {
    $.ajax({
        type: 'GET',
        url: AppRoutes.Seguimiento.UrlGetInfoAdicional, // URL del endpoint que devuelve los datos
        dataType: 'json',
        data: { V_CriminalidadId: CriminalidadId },
        success: function (response) {

            // Inicializar la grilla con los datos filtrados o vacíos
            GetGrillaInfoAdicionalIris(response.data);

        },
        error: function () {
            // En caso de error, inicializar la grilla con datos vacíos
            GetGrillaInfoAdicionalIris([]);

        }
    });
}

function GetGrillaInfoAdicionalIris(Datos) {
    if ($.fn.dataTable.isDataTable("#tbGrillaInfoAdicional")) {
        $("#tbGrillaInfoAdicional").DataTable().destroy();
    }

    $("#tbGrillaInfoAdicional").empty();
    

    $("#tbGrillaInfoAdicional").DataTable({
        destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        responsive: true,
        "columns": [
            //{
            //    data: null, className: "celdaCenter celda3", "render": function (data, type, row) {
            //        var inicioBoton = '<div class="dropdown dropend"><button class="btn btn-success" type="button" id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false"><span class="fas fa-list"></span></button><ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';
            //        var Eliminar = `<li style="padding-left: 17px;"><a style="color: #102717;" href="javascript:P_DelDelInfoAdicionalIris('${row.InfoId}')"><i class="fa fa-trash red"></i>&nbsp;Eliminar</a></li>`;
            //        var finBoton = '</ul></div>';
            //        return inicioBoton + Eliminar + finBoton;
            //    }
            //},
            columnaInforAdicionalDetalleIris(),
            { "title": "Tipo Información", "data": "DescTipoInfo", "name": "DescTipoInfo", className: "celdaCenter" }

        ],

        lengthChange: false,
        searching: false,
        ordering: false,
        pageLength: 10,
        paging: false,
        info: false
    });
}



function F_GetResponsableIris(CriminalidadId) {

    $("#Process").hide();
    $.ajax({
        type: 'GET',
        url: AppRoutes.Seguimiento.UrlGetResponsable, // URL del endpoint que devuelve los datos
        dataType: 'json',
        data: { V_CriminalidadId: CriminalidadId },
        success: function (response) {

            // Inicializar la grilla con los datos filtrados o vacíos
            GetGrillaResponsableIris(response.data);

        },
        error: function () {
            // En caso de error, inicializar la grilla con datos vacíos
            GetGrillaResponsableIris([]);

        }
    });
}

function GetGrillaResponsableIris(Datos) {
    if ($.fn.dataTable.isDataTable("#tbGrillaResponsables")) {
        $("#tbGrillaResponsables").DataTable().destroy();
    }

    $("#tbGrillaResponsables").empty();
   

    $("#tbGrillaResponsables").DataTable({
        destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        responsive: true,
        "columns": [
           
            { "title": "Responsable", "data": "FuncionarioResponsable" },
            { "title": "Unidad", "data": "UnidadFuncionarioResponsable"},
           // { "title": "Funcionario que Asignó", "data": "FuncionarioCreacion" },
          
        ],
        lengthMenu: [
            [5, 10, 25, 50, -1],
            ['5 registros', '10 registros', '25 registros', '50 registros', 'Todos']
        ],
        ordering: true,
        pageLength: 10,
        bLengthChange: true,
        searching: true,
        paging: true,
        info: true
    });
}

var EstadoGlobalAceptada = "";
function F_GetTareas(IdCriminalidad) {
   
    $("#txtCriminalidadIdModal").val(IdCriminalidad);

    // 🔹 NUEVO: Obtener responsables
    $.ajax({
        type: "GET",
        url: AppRoutes.Seguimiento.UrlGetResponsablesTareasIris, // crea este endpoint en backend
        data: { V_Criminalidad: IdCriminalidad },
        dataType: 'json',
        cache: false,
        success: function (resp) {
            if (resp?.success && Array.isArray(resp.data)) {


                // ✅ Recorre todos los registros para revisar el campo Aceptada
                const tieneAceptada = resp.data.some(
                    (item) => item.Aceptada && item.Aceptada.toUpperCase() === "SI"
                );

                // ✅ Guarda en la variable global el estado
                EstadoGlobalAceptada = tieneAceptada ? "Aceptada" : "No Aceptada";



                GetGrillaResponsablesTareas(resp.data);

            } else {


                GetGrillaResponsablesTareas([]);
                //Swal.fire({
                //    icon: 'info',
                //    title: 'Señor(a) Funcionario(a)',
                //    text: "No hay tareas asignadas."
                //});
            }
        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Señor(a) Funcionario(a)',
                text: 'No es posible consultar las tareas. Por favor, revise la conexión o contacte al administrador.'
            });
        }
    });
}




function decodeHtmlEntities(text) {
    if (!text) return "";
    var textarea = document.createElement("textarea");
    textarea.innerHTML = text;
    return textarea.value;
}

function GetGrillaResponsablesTareas(Datos) {
    if ($.fn.dataTable.isDataTable("#tbGrillaResponsablesVerificacion")) {
        $("#tbGrillaResponsablesVerificacion").DataTable().destroy();
    }

    $("#tbGrillaResponsablesVerificacion").empty();
    $("#pn_GrillaResponsablesVerificacion").removeClass('hidden');

    $("#tbGrillaResponsablesVerificacion").DataTable({
        destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        responsive: true,
        columns: [
            {
                data: null,
                className: "celdaCenter celda3",
                render: function (data, type, row) {
                    var inicioBoton = '<div class="dropdown dropend">' +
                        '<button class="btn btn-success" type="button" id="dropdownMenuButton1" ' +
                        'data-bs-toggle="dropdown" aria-expanded="false">' +
                        '<span class="fas fa-list"></span></button>' +
                        '<ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';

                    var CambiarUnidad =
                        `<li style="padding-left: 17px;">` +
                        `<a style="color: #102717;" href="javascript:OpenUpdResponsableValModal('${row.ResponValidacionId}')">` +
                        `<i class="fas fa-exchange-alt"></i>&nbsp;Cambiar Unidad</a></li>`;

                    var EliminarUnidad =
                        `<li style="padding-left: 17px;">` +
                        `<a style="color: #102717;" href="javascript:P_DelUnidadResponsable('${row.ResponValidacionId}','${row.IdUnidadResponsable}')">` +
                        `<i class="fa fa-trash red"></i>&nbsp;Eliminar Unidad</a></li>`;

                    var finBoton = '</ul></div>';
                    return inicioBoton + CambiarUnidad + EliminarUnidad + finBoton;
                }
            },
            {
                title: "Unidad",
                data: "UnidadCompleta",
                name: "UnidadCompleta",
                className: "celda30"
            },
            {
                data: "Seguimiento",
                title: "Seguimiento Tareas",
                className: "text-start align-top",
                render: function (data, type, row) {
                    if (type === 'display' && data) {
                        return decodeHtmlEntities(data);
                    }
                    return data;
                }
            }
        ],
        lengthMenu: [
            [5, 10, 25, 50, -1],
            ['5 registros', '10 registros', '25 registros', '50 registros', 'Todos']
        ],
        ordering: true,
        pageLength: 10,
        bLengthChange: true,
        searching: true,
        paging: true,
        info: true
    });
}





function F_GetResultados(IdCriminalidad) {//, IdResponsable) {
    $.ajax({
        type: "GET",
        url: AppRoutes.Seguimiento.UrlGetResultadosIris,
        data: { V_Criminalidad: IdCriminalidad },//, V_ResponsableId: IdResponsable },
        dataType: 'json',
        cache: false,
        success: function (respuesta) {
            if (respuesta?.success && Array.isArray(respuesta.data) && respuesta.data.length > 0) {
                // Mostrar el panel de resultados
                $('#pn_GrillaResultados').removeClass('hidden');
                GetGrillaResultados(respuesta.data);
            } else {

              
                // Ocultar la grilla si está visible
               $('#pn_GrillaResultados').addClass('hidden');

                //Swal.fire({
                //    icon: 'info',
                //    title: 'Señor(a) Funcionario(a)',
                //    text: "No hay resultados asociados a esta criminalidad."
                //});
            }

        },
        error: function (xhr, status, error) {
            console.error("Error en F_GetTareas:", {
                status: xhr.status,
                response: xhr.responseText,
                error: error
            });

            Swal.fire({
                icon: 'error',
                title: 'Señor(a) Funcionario(a)',
                text: 'No es posible consultar las tareas. Por favor, revise la conexión o contacte al administrador.'
            });
        }
    });
}

function GetGrillaResultados(Datos) {
    if ($.fn.dataTable.isDataTable("#tbGrillaResultados")) {
        $("#tbGrillaResultados").DataTable().destroy();
    }

    $("#tbGrillaResultados").empty();
    $("#pn_GrillaResultados").removeClass('hidden');

    $("#tbGrillaResultados").DataTable({
        destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        responsive: true,
        "columns": [
            //{
            //    data: null, className: "celdaCenter celda3", "render": function (data, type, row) {
            //        var inicioBoton = '<div class="dropdown dropend"><button class="btn btn-success" type="button" id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false"><span class="fas fa-list"></span></button><ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';
            //        var Eliminar = `<li style="padding-left: 17px;"><a style="color: #102717;" href="javascript:Dell_Roles(${row.IdUserRol})"><i class="fa fa-trash red"></i>&nbsp;Eliminar</a></li>`;
            //        var finBoton = '</ul></div>';
            //        return inicioBoton + Eliminar + finBoton;
            //    }
            //},

            { "title": "Tipo", "data": "DescTipoResultado", "name": "DescTipoResultado", className: "celdaCenter celda2" },
            { "title": "Número", "data": "NroSpoaSiedco", "name": "NroSpoaSiedco", className: "celdaCenter celda3" },
            {
                title: "Fecha SIEDCO - SPOA",
                data: "FechaResultado",
                name: "FechaResultado",
                className: "celdaCenter celda5",
                render: function (data) {
                    if (!data) return "";
                    const fecha = moment(data).format('DD/MM/YYYY');
                    const hora = moment(data).format('hh:mm:ss a');
                    return `${fecha} - ${hora}`;

                }
            },
            columnaObservacionResultado(),
            {
                title: "Fecha Creación",
                data: "FechaCreaResultado",
                name: "FechaCreaResultado",
                className: "celdaCenter celda5",
                render: function (data) {
                    if (!data) return "";
                    const fecha = moment(data).format('DD/MM/YYYY');
                    const hora = moment(data).format('hh:mm:ss a');
                    return `${fecha} - ${hora}`;

                }
            }


        ],
        lengthMenu: [
            [5, 10, 25, 50, -1],
            ['5 registros', '10 registros', '25 registros', '50 registros', 'Todos']
        ],
        ordering: true,
        pageLength: 10,
        bLengthChange: true,
        searching: true,
        paging: true,
        info: true
    });
}


function columnaObservacionResultado() {
    return {
        title: "Observación",
        data: "ObservacionResultado",
        name: "ObservacionResultado",
        "autoWidth": false,
        className: "celdaCenter celda7",
        render: function (data, type, row) {
            if (!data || data.trim() === "") {
                return '';
            }

            const dataEncoded = encodeURIComponent(data);

            return `
                <div style="display: flex; align-items: center; max-width: 100px; gap: 10px;">
                    <button class="btn btn-success btn-sm" type="button"
                        onclick="AbrirModalVisualizarTexto(decodeURIComponent('${dataEncoded}'))">
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



function OpenInsResponsableValModal() {
    const modalElement = document.getElementById('Modal_InsResponable');
    const modalInstance = new bootstrap.Modal(modalElement, {
        backdrop: 'static',
        keyboard: true,
        focus: true // 👈 clave: permite escribir en el buscador de Select2
    });
    modalInstance.show();

    // ✅ Re-inicializa los Select2 dentro de la modal con el parent correcto
    $('#ddlTipoUnidad, #ddlTipoDependencia, #ddlTipoTarea').select2({
        dropdownParent: $('#Modal_InsResponable'),
        placeholder: "Seleccione",
        allowClear: true,
        width: '100%'
    });
}



$(document).on('change', '#ddlTipoUnidad', function () {
    handleDropdownChange('Irisp1/Seguimiento/F_GetUnidadesPorSigla', 'V_Sigla', $(this).val(), '#ddlTipoDependencia');
   
});

$(document).on('change', '#ddlTipoUnidad2', function () {
  
    handleDropdownChange('Irisp1/Seguimiento/F_GetUnidadesPorSigla', 'V_Sigla', $(this).val(), '#ddlTipoDependencia2');
});


function handleDropdownChange(url, paramName, paramValue, dropdownSelector, callback) {
    if (paramValue) {
        $.getJSON(url, { [paramName]: paramValue }, function (data) {
            const $dropdown = $(dropdownSelector);
            $dropdown.empty().append('<option value="">Seleccione</option>');

            if (Array.isArray(data) && data.length > 0) {
                $.each(data, function (index, item) {
                    if (item && item.descripcion) {
                        /*dropdown.append(`<option value="${item.id || item.consecutivo || item.codigo}">${item.descripcion}</option>`);*/
                        $dropdown.append(`<option value="${item.codigo}"data-sigla="${item.sigla || ''}"data-descripcion="${item.descripcion}"> ${item.descripcion}</option>`);
                    }
                });

                // Ejecutar callback si existe
                if (typeof callback === "function") callback();

                $dropdown.trigger('change');
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            console.error(`Error al cargar datos desde ${url}:`, textStatus, errorThrown);
        });
    }
}

$('#ddlTipoDependencia').on('change', function () {
    const $opt = $(this).find(':selected');
    const id = $opt.val();
    const descripcion = $opt.data('descripcion');
    const sigla = $opt.data('sigla');

    // Guardar en el propio select
    $(this).data('idSeleccionado', id);
    $(this).data('descripcionSeleccionada', descripcion);
    $(this).data('siglaSeleccionada', sigla);

    console.log('Guardado en el select:', $(this).data());
});


$('#ddlTipoDependencia2').on('change', function () {
    const $opt = $(this).find(':selected');
    const id = $opt.val();
    const descripcion = $opt.data('descripcion');
    const sigla = $opt.data('sigla');

    // Guardar en el propio select
    $(this).data('idSeleccionado', id);
    $(this).data('descripcionSeleccionada', descripcion);
    $(this).data('siglaSeleccionada', sigla);

    console.log('Guardado en el select:', $(this).data());
});

function P_InsResponsabeValModal() {
    const tipoTarea = parseInt($("#ddlTipoTarea").val());
    const estadoExistencia = (typeof EstadoExistencia !== "undefined" ? EstadoExistencia.toLowerCase() : "");
    const estadoAceptada = (typeof EstadoGlobalAceptada !== "undefined" ? EstadoGlobalAceptada.toLowerCase() : "");


    // 🔹 Objeto a enviar
    const Obj_Responsable = {
        CriminalidadId: $("#txtCriminalidadIdModal").val(),
        IdUnidad: $('#ddlTipoDependencia').data('idSeleccionado'),
        IdTareai: tipoTarea,
        Observacion: $("#txtObservaciones").val()
    };

    // 🔹 Validar campos obligatorios (excepto Observacion)
    for (let key in Obj_Responsable) {
        if (key !== 'Observacion') {
            const val = Obj_Responsable[key];
            if (!val || val === '' || val === undefined || (typeof val === 'number' && isNaN(val))) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Señor(a) Funcionario(a):',
                    text: 'Valide todos los campos para completar el registro.'
                });
                return;
            }
        }
    }


    // 🔹 Validación: asignar proceso investigativo (71)
    if (tipoTarea === 71) {
        if (estadoExistencia === 'si existe') {
            // OK, continúa
        } else {
            Swal.fire({
                icon: 'warning',
                title: 'Señor(a) Funcionario(a):',
                text: 'Para asignar el proceso investigativo, el IRISP debe estar en estado existencia "Si Existe".'
            });
            return;
        }
    }

    // 🔹 Validación: verificación de existencia (52)
    if (tipoTarea === 52) {
        if (estadoAceptada === 'no aceptada') {
            // OK, continúa
        } else {
            Swal.fire({
                icon: 'warning',
                title: 'Señor(a) Funcionario(a):',
                text: 'Ya se registró una unidad responsable de la verificación de la existencia o existen tareas de verificación no rechazadas.'
            });
            return;
        }
    }

   
    // 🔹 Enviar solicitud AJAX
    $.ajax({
        url: AppRoutes.Seguimiento.UrlInsResponsable,
        type: 'POST',
        data: Obj_Responsable, 
        success: function (resp) {
            if (resp.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Señor(a) Funcionario(a):',
                    text: resp.message
                }).then(() => {
                    // Limpieza del formulario
                    $('#ddlTipoUnidad, #ddlTipoDependencia, #ddlTipoTarea').val('').trigger('change');
                    $("#txtObservaciones").val('');
                    $("#Modal_InsResponable").modal('hide');

                    // Refrescar la grilla
                    F_GetTareas($("#txtCriminalidadIdModal").val());
                });
            } else {
                Swal.fire('Error', 'Error al insertar: ' + resp.message, 'error');
            }
        },
        error: function (xhr, status, error) {
            Swal.fire('Error', 'Fallo en la llamada AJAX: ' + error, 'error');
        }
    });
}



function OpenUpdResponsableValModal(V_ResponsableValidacionId) {


    $("#txtResponsableId").val(V_ResponsableValidacionId);
    const modalElement = document.getElementById('Modal_UpdResponable');
    const modalInstance = new bootstrap.Modal(modalElement, {
        backdrop: 'static',
        keyboard: true,
        focus: true // 
    });
    modalInstance.show();

    // ✅ Re-inicializa los Select2 dentro de la modal con el parent correcto
    $('#ddlTipoUnidad2, #ddlTipoDependencia2').select2({
        dropdownParent: $('#Modal_UpdResponable'),
        placeholder: "Seleccione",
        allowClear: true,
        width: '100%'
    });
}

function P_UpdUnidadResponsable() {
   
    const obj_responsableUpd = {
        IdResponsable: $("#txtResponsableId").val(),
        IdUnidad: $('#ddlTipoDependencia2').data('idSeleccionado'),
        //IdUnidad: $('#ddlTipoUnidad2').data('idSeleccionado')
    }


    // 🔹 Validar campos obligatorios (excepto Observacion)
    for (let key in obj_responsableUpd) {
        //if (key !== 'Observacion') {
        const val = obj_responsableUpd[key];
        if (!val || val === '' || val === undefined || (typeof val === 'number' && isNaN(val))) {
            Swal.fire({
                icon: 'warning',
                title: 'Señor(a) Funcionario(a):',
                text: 'Valide todos los campos para completar la actualización.'
            });
            return;
        }
        //}
    }

       
    $.ajax({
        url: AppRoutes.Seguimiento.UrlUpdResponsable,
        type: 'POST',
        data: obj_responsableUpd, 
        success: function (resp) {
            if (resp.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Señor(a) Funcionario(a):',
                    text: resp.message
                }).then(() => {
                    // Limpieza del formulario
                    $('#ddlTipoUnidad2, #ddlTipoDependencia2').val('').trigger('change');
                   
                    $("#Modal_UpdResponable").modal('hide');

                    // Refrescar la grilla
                    F_GetTareas($("#txtCriminalidadIdModal").val());
                });
            } else {
                Swal.fire('Error', 'Error al insertar: ' + resp.message, 'error');
            }
        },
        error: function (xhr, status, error) {
            Swal.fire('Error', 'Fallo en la llamada AJAX: ' + error, 'error');
        }
    });


}


function P_DelUnidadResponsable(V_ResposablId, V_IdUnidadResponsable) {

    const obj_DelResponsable = {
        IdResponsable: V_ResposablId,
        IdUnidad: V_IdUnidadResponsable
    };

    Swal.fire({
        title: '¿Está seguro?',
        text: 'Esta acción eliminará la unidad responsable seleccionada.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {


        if (result.isConfirmed) {
            $.ajax({
                url: AppRoutes.Seguimiento.UrlDelResponsable,
                type: 'POST',
                data: obj_DelResponsable,
                success: function (resp) {
                    if (resp.success) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Señor(a) Funcionario(a):',
                            text: resp.message
                        }).then(() => {
                            F_GetTareas($("#txtCriminalidadIdModal").val());
                        });
                    } else {
                        Swal.fire('Error', 'Error al eliminar: ' + resp.message, 'error');
                    }
                },
                error: function (xhr, status, error) {
                    Swal.fire('Error', 'Fallo en la llamada AJAX: ' + error, 'error');
                }
            });
        }
    });
}


function AbrirModalEvaluarTarea(V_IdResponsable) {
  
    $("#txtTareaId").val(V_IdResponsable);

    $("#Modal_EvaluarTarea").modal("show");


}

function AbrirModalReasignarTarea(V_IdResponsable) {

    $("#txtTareaId").val(V_IdResponsable);

    $("#Modal_ReasignarTarea").modal("show");


}

function P_EvalTarea() {

    const obj_EvalTarea = {
        IdTarea: $("#txtTareaId").val(),
        IdEstado: $('#ddlTipoEvalTarea').val()
    }



    // 🔹 Validar campos obligatorios (excepto Observacion)
    for (let key in obj_EvalTarea) {
        //if (key !== 'Observacion') {
        const val = obj_EvalTarea[key];
            if (!val || val === '' || val === undefined || (typeof val === 'number' && isNaN(val))) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Señor(a) Funcionario(a):',
                    text: 'Valide todos los campos para completar el registro.'
                });
                return;
            }
        //}
    }



    $.ajax({
        url: AppRoutes.Seguimiento.UrlEvalTarea,
        type: 'POST',
        data: obj_EvalTarea,
        success: function (resp) {
            if (resp.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Señor(a) Funcionario(a):',
                    text: resp.message
                }).then(() => {
                    // Limpieza del formulario
                    $('#ddlTipoEvalTarea').val('').trigger('change');

                    $("#Modal_EvaluarTarea").modal('hide');

                    // Refrescar la grilla
                    F_GetTareas($("#txtCriminalidadIdModal").val());
                });
            } else {
                Swal.fire('Error', 'Error al insertar: ' + resp.message, 'error');
            }
        },
        error: function (xhr, status, error) {
            Swal.fire('Error', 'Fallo en la llamada AJAX: ' + error, 'error');
        }
    });


}

function P_ReasignarTarea() {

    const obj_ReasignarTarea = {
        ResponValidacionId: $("#txtTareaId").val(),
        CriminalidadId: $("#txtCriminalidadIdModal").val(),
        Observacion: $('#txtObservacionesReasig').val()
    }


    // 🔹 Validar campos obligatorios (excepto Observacion)
    for (let key in obj_ReasignarTarea) {
        if (key !== 'Observacion') {
            const val = obj_ReasignarTarea[key];
            if (!val || val === '' || val === undefined || (typeof val === 'number' && isNaN(val))) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Señor(a) Funcionario(a):',
                    text: 'Valide todos los campos para completar el registro.'
                });
                return;
            }
        }
    }


    $.ajax({
        url: AppRoutes.Seguimiento.UrlReasignarTarea,
        type: 'POST',
        data: obj_ReasignarTarea,
        success: function (resp) {
            if (resp.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Señor(a) Funcionario(a):',
                    text: resp.message
                }).then(() => {
                    // Limpieza del formulario
                    $('#txtObservacionesReasig').val("");

                    $("#Modal_ReasignarTarea").modal('hide');

                    // Refrescar la grilla
                    F_GetTareas($("#txtCriminalidadIdModal").val());
                });
            } else {
                Swal.fire('Error', 'Error al insertar: ' + resp.message, 'error');
            }
        },
        error: function (xhr, status, error) {
            Swal.fire('Error', 'Fallo en la llamada AJAX: ' + error, 'error');
        }
    });


}


function DellIris(CriminalidadId) {

    bootbox.confirm({
        message: "¿Está seguro de finalizar el registro seleccionado?",
        buttons: {
            confirm: {
                label: '<i class="fa fa-check"></i> Sí',
                className: 'btn-success'
            },
            cancel: {
                label: '<i class="fa fa-times"></i> No',
                className: 'btn-danger'
            }
        },
        callback: function (result) {
            if (result) {
                // Llamar directamente a la función de eliminación sin pedir motivo

                $.ajax({
                    type: 'POST',
                    url: AppRoutes.RegistroIrisP1.UrlDelIris
                    ,
                    async: true,
                    dataType: 'json',
                    data: { CriminalidadId: CriminalidadId },
                    success: function (result) {
                        if (result.success) {

                            var fecha = $('#ddlAnioIris').val(); // obtengo valor actual
                            $('#ddlAnioIris').val(fecha).trigger('change'); // lo reasigno para refrescar
                            F_GetInfoGrillas();
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
        }
    });
}



function FinalizarIris(CriminalidadId) {

    bootbox.confirm({
        message: "Está seguro de finalizar el registro seleccionado?",
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
                    title: "Digite el No. Comunicación Oficial o No. Mored con el cual se le realizo el tramite",
                    inputType: 'text',
                    placeholder: "Si no hubo resultado, Justifique las razones de su finalización",
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
                                message: "Debe Justificar las razones de su finalización",
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
                            P_FinalizarIris(CriminalidadId, resu);
                        }
                    }
                });
            }
        }
    });
}



function P_FinalizarIris(P_CriminalidadId, P_Justificacion) {

    var DtoFinalizar = {
        CriminalidadId: P_CriminalidadId,
        Justificacion: P_Justificacion
    }


    // Validación de campos obligatorios
    if (!DtoFinalizar.CriminalidadId ||
        !DtoFinalizar.Justificacion) {

        Swal.fire("Atención", "Todos los campos son obligatorios. Por favor complete la información.", "warning");
        return; // Detener ejecución
    }

    $.ajax({
        type: 'POST',
        url: AppRoutes.Seguimiento.UrlFinalizarIris,
        async: true,
        dataType: 'json',
        data: { obj: DtoFinalizar },
        success: function (result) {
            if (result.success) {
               
                Swal.fire({
                    type: 'success',
                    title: 'Señor(a) Funcionario(a:)',
                    text: result.message
                });

                F_GetInfoGrillas();

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


function actualizarEstadoCriminalidad() {

    var data = {
        CriminalidadId: $("#txtCriminalidadIdUpd").text(),
        IdEstado: $("#ddlEstadosIrisP1").val(),

    };

    // Validación de campos obligatorios
    if (!data.CriminalidadId ||
        !data.IdEstado) {

        Swal.fire("Atención", "Todos los campos son obligatorios. Por favor complete la información.", "warning");
        return; // Detener ejecución
    }

    $.ajax({
        url: AppRoutes.RegistroIrisP1.UrlUpdEstadoCriminalidad
        ,
        type: 'POST',
        data: data,
        success: function (response) {
            if (response.success) {
                Swal.fire("Éxito", response.message, "success");
                $("#Modal_UpdEstadoRegistroIris").modal("hide");
                $("#ddlEstadosIrisP1").val('').trigger('change');

                var fecha = $('#ddlAnioIris').val(); // obtengo valor actual
                $('#ddlAnioIris').val(fecha).trigger('change'); // lo reasigno para refrescar
                F_GetInfoGrillas();


            } else {
                Swal.fire("Atención", response.message, "warning");
            }
        },
        error: function (xhr, status, error) {
            Swal.fire("Error", "Ocurrió un error al intentar actualizar", "error");
        }
    });
}