
$(document).ready(function () {


    var fechaMinima = new Date();
    fechaMinima.setHours(0, 0, 0, 0); // Reinicia hora, minutos, segundos y milisegundos

    $(".CalendarioHora").kendoDateTimePicker({
        culture: "es-CO",
        format: "dd/MM/yyyy HH:mm",
        timeFormat: "HH:mm",
        interval: 15,
        min: fechaMinima, // <-- Fecha mínima: hoy a las 00:00
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



    $('#ddlAnioIris').on('change', function () {
        F_GetInfoGrillas();
    });

    F_GetInfoGrillas($('#ddlAnioIris').val());


    $("#btnVisualizarFuncionario").on("click", function (e) {
        e.preventDefault();
        OpenUbicacionModal($("#txtLatitud").text(), $("#txtLongitud").text());
    });
    $("#btnNuevoIntegrante").on("click", function (e) {
        e.preventDefault();
        $('#Modal_InsIntegrantesExendios').modal("show");
    });
    $("#btnNuevoDelito").on("click", function (e) {
        e.preventDefault();
        $('#Modal_InsDelitosExpendios').modal("show");
    });
    $("#btnNuevoBitacora").on("click", function (e) {
        e.preventDefault();
        $('#Modal_InsInfoBitacora').modal("show");
    });
    $("#btnNuevoResultado").on("click", function (e) {
        e.preventDefault();
        $('#Modal_InsResultadosExendios').modal("show");
    });

});

$('#ddlTipoResultado').change(function () {
    const valor = $(this).val();

    if (valor && !isNaN(valor)) {
        handleDropdownChange(
            '/Expendios/Registros/F_GetDominiosIris',
            { V_id: valor },
            '#ddlSubTipoResultado'
        );
    } else {
        console.warn("Valor inválido o vacío:", valor);
    }
});

function handleDropdownChange(url, params, dropdownSelector, callback) {
    if (params && params.V_id) {
        $.getJSON(url, params, function (data) {
            const dropdown = $(dropdownSelector);
            dropdown.empty().append('<option value="">Seleccione</option>');

            if (Array.isArray(data) && data.length > 0) {
                $.each(data, function (index, item) {
                    if (item && item.Descripcion) {
                        dropdown.append(`<option value="${item.IdDominio}">${item.Descripcion}</option>`);
                    }
                });
            }

            // Si usas Select2, reinit aquí
            if ($.fn.select2) {
                dropdown.select2();
            }

            if (callback && typeof callback === "function") {
                callback();
            }

        }).fail(function (jqXHR, textStatus, errorThrown) {
            console.error(`Error al cargar datos desde ${url}:`, textStatus, errorThrown);
        });
    } else {
        // Limpiar el segundo dropdown si no hay valor seleccionado
        $(dropdownSelector).empty().append('<option value="">Seleccione</option>');
    }
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



function F_GetInfoGrillas() {
    $.ajax({
        type: 'GET',
        url: AppRoutes.RegistroExpendio.UrlGetInfoGrillas,
        dataType: 'json',
        data: { V_Anio: $('#ddlAnioIris').val() },
        success: function (response) {
            console.log("✅ Respuesta exitosa:", response);
            let data = response.data || [];
            GetGrillaVerificacion(data);
           
        },
        error: function (xhr, status, error) {
            console.error("❌ Error Ajax:", status, error);
            console.error("Respuesta cruda:", xhr.responseText);
            GetGrillaVerificacion([]);
           
        }
    });
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

// 🔧 Función utilitaria para inicializar o refrescar tablas
function renderDataTable(selector, datosFiltrados, columnas) {
    if ($.fn.dataTable.isDataTable(selector)) {
        const table = $(selector).DataTable();
        table.clear();
        table.rows.add(datosFiltrados);
        table.draw(false);
        return;
    }

    $(selector).DataTable({
        data: datosFiltrados,
        language: glOpcionesIdioma,
        scrollX: true,
        scrollY: 800,
        scroller: true,
        deferRender: true,
        autoWidth: false,   // importante para scroll + columnas fijas
        responsive: false,  // desactiva responsive, interfiere con fixedColumns

        columnDefs: [
            { targets: '_all', className: 'dt-head-center dt-body-center' },
            { targets: 2, className: 'no-wrap', width: '90px' } // Código
        ],
        columns: columnas,

        //fixedColumns: {
        //    leftColumns: 1,   // 🔒 mantiene fija la primera columna (acciones)
        //    // rightColumns: 0 // puedes usar esto si quieres fijar columnas al final
        //},

        lengthMenu: [
            [10, 25, 50, 100],
            ['10 registros', '25 registros', '50 registros', '100 registros']
        ],
        pageLength: 25,
        ordering: true,
        searching: true,
        paging: true,
        info: true
    });
}



function GetGrillaVerificacion(datosFiltrados) {
    //const datosFiltrados = Datos.filter(item => [2, 3, 4].includes(item.IdEstado));
    $("#pn_GrillaExpendios").removeClass('hidden');
    
    renderDataTable("#tbGrillaExpendios", datosFiltrados, [
        columnaAcciones(datosFiltrados),
        Estados(),
        
        { title: "Codigo", data: "Codigo" },
        
        { title: "Unidad Informa", data: "UnidadInformaDescripcion" },
        { title: "Sigla", data: "SiglaUnidadInforma" },
        { title: "Region", data: "RegionDescripcion" },
       
        { title: "Unidad Hecho", data: "Unidad" },
        { title: "Sigla", data: "Sigla" },
        { title: "Zona", data: "Zona" },
        { title: "Clase", data: "Clase" },
        { title: "Tipo Expendio", data: "Expendio" },
        { title: "Fuente", data: "Fuente" },
        { title: "Fecha Inicio Existencia", data: "FechaInicioExistencia", render: formatDate },
        columnaCaracteristicasGenerales(),
        { title: "Categoría", data: "Categoria" },
        { title: "Otra Categoría", data: "OtraCategoria" },
        { title: "Código Operación", data: "CodigoMored" },
        { title: "Nombre Operación", data: "NombreMored" },
        { title: "NUNC", data: "Nunc" },
        { title: "SIEDCO", data: "Siedco" },
        { title: "Erradicado ?", data: "Erradicado" },
        { title: "Barrio", data: "Barrio" },
        { title: "Direccion", data: "Direccion" },
        { title: "Latitud", data: "Latitud" },
        { title: "Longitud", data: "Longitud" },
        { title: "Cuadrante", data: "Cuadrante" },
        { title: "Municipio", data: "Municipio" },
        { title: "Fecha Creacion", data: "FechaCreacion", render: formatDate },
        { title: "CriminalidadDirecId", data: "CriminalidadDirecId", visible: false }
    ]);
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

            var Detalle = `<li style="padding-left: 15px;">
                                    <a style="color: #102717;" href="#"
                                       class="btn-detalle-expendio"
                                       data-datos="${DatosFila}">
                                        <i class="fas fa-list"></i>&nbsp; Detalle
                                    </a>
                                </li>`;

            var ActualizarEstado = `<li style="padding-left: 15px;">
                                    <a style="color: #102717;" href="javascript:Finalizar('${row.CriminalidadId}')">
                                        <i class="fa fa-retweet green"></i>&nbsp;Actualizar Estado
                                    </a>
                                  </li>`;


            var finBoton = '</ul></div>';
            return inicioBoton + Detalle + ActualizarEstado + finBoton;
        }
    }
}


// Delegación de eventos para los botones de detalle
$(document).on("click", ".btn-detalle-expendio", function (e) {
    e.preventDefault();

    // Recuperamos el JSON guardado en data-datos
    var datosAttr = $(this).attr("data-datos").replace(/&quot;/g, '"');

    try {
        var registro = JSON.parse(datosAttr);
        F_GetDetalleExpendio(registro);
    } catch (err) {
        console.error("❌ Error parseando data-datos:", err, datosAttr);
        Swal.fire('Error', 'No se pudo procesar el detalle del registro', 'error');
    }
});


function Estados() {
    return {
        title: "Estado",
        data: "Estado",
        name: "Estado",
        autoWidth: true,
        render: function (data, type, row) {
            // Si el estado viene vacío o nulo
            if (!data) {
                return `<span style="background-color: #808080; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 120px;">Por establecer</span>`;
            }

            const estado = data.toLowerCase();
            let color = '';

            switch (estado) {
                case 'descartado':
                    color = '#c53a1d'; // rojo
                    break;
                case 'investigación':
                    color = '#2127f5'; // azul
                    break;
                case 'finalizado':
                    color = '#032b57'; // azul obscuro
                    break;
                case 'verificación':
                    color = '#236305'; // verde
                    break;
              
                default:
                    color = '#386ca0'; // gris oscuro
            }

            return `<span style="background-color: ${color}; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 120px;">${data}</span>`;
        }
    };
}

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


function AbrirModalVisualizarTexto(Texto) {

    // Mostrar la modal
    $('#Modal_VisualizarTexto').modal("show");
    $('#txtDescripcion').val(Texto);
}


function F_GetDetalleExpendio(registro) {   // 👈 ahora recibe directamente el objeto
    console.log("✅ Registro recibido:", registro);

    // Ya no se hace JSON.parse de nuevo
    $("#txtCriminalidadIdModal").val(registro.CriminalidadDirecId);
   // $("#txtConsecutivoIris").val(registro.CriminalidadId);

    var FechaInicio = moment(registro.FechaInicioExistencia).format('DD/MM/YYYY hh:mm:ss a');
    var FechaCreacion = moment(registro.FechaCreacion).format('DD/MM/YYYY hh:mm:ss a');

    $("#txtClaseHeader").text(registro.Clase);
    $("#txtCodigoHeader").text(registro.Codigo);


    $("#txtUnidadDetalle").text(registro.Unidad);
    $("#txtFechaCreacion").text(FechaInicio);

    $("#txtCodigoExpendio").text(registro.Codigo);
    $("#txtUnidadInforma").text(registro.UnidadInformaDescripcion);
    $("#txtSiglaInforma").text(registro.SiglaUnidadInforma);
    $("#txtZona").text(registro.Zona);
    $("#txtClase").text(registro.Clase);
    $("#txtFuente").text(registro.Fuente);
    $("#txtCategoria").text(registro.Categoria);
    $("#txtOtraCategoria").text(registro.OtraCategoria);


    $("#txtMunicipio").text(registro.Municipio);
    $("#txtBarrio").text(registro.Barrio);
    $("#txtCuadrante").text(registro.Cuadrante);
    $("#txtLatitud").text(registro.Latitud);
    $("#txtLongitud").text(registro.Longitud);
    $("#txtDireccion").text(registro.Direccion);


   
    $("#txtCodigoOperacion").text(registro.CodigoMored);
    $("#txtNombreOperacion").text(registro.NombreMored);
    $("#txtNUNC").text(registro.Nunc);
    $("#txtSIEDCO").text(registro.Siedco);
  

    // ✅ Renderiza el badge con color (no solo texto)
    $("#txtEstado").html(RenderEstadoBadge(registro.Estado));

    // ✅ Lo mismo para Estado Existencia
    $("#txtErradicado").html(RenderErradicadoModal(registro.Erradicado));



    var Estado = $("#txtEstado").text().trim();
    if  (Estado !== "Finalizado") 
    
    {

        $("#btnNuevoIntegrante").removeClass("hidden");
        $("#btnNuevoDelito").removeClass("hidden");
        $("#btnNuevoBitacora").removeClass("hidden");
        $("#btnNuevoResultado").removeClass("hidden");
       
    }



    $('#Modal_DetalleExpendio').modal("show");

    F_GetIntegrantesIris(registro.CriminalidadDirecId);
    F_GetDelitosExpendios(registro.CriminalidadDirecId);
    F_GetBitacora(registro.CriminalidadDirecId);
    F_GetResultados(registro.CriminalidadDirecId);



}

// 1️⃣ Función utilitaria para renderizar el estado (extraída de Estados().render)
function RenderEstadoBadge(estadoTexto) {
    if (!estadoTexto) {
        return `<span style="background-color: #808080; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 120px;">Por establecer</span>`;

    }

    const estado = estadoTexto.toLowerCase();
    let color = '';

    switch (estado) {
        case 'descartado':
            color = '#c53a1d'; // rojo
            break;
        case 'investigación':
            color = '#2127f5'; // azul
            break;
        case 'finalizado':
            color = '#032b57'; // azul obscuro
            break;
        case 'verificación':
            color = '#236305'; // verde
            break;

        default:
            color = '#386ca0'; // gris oscuro
    }

    return `<span style="background-color: ${color}; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 120px;">${estadoTexto}</span>`;
}

function RenderErradicadoModal(Valor) {

    let color = '';
    if (!Valor) {
        return `<span style="background-color: #c53a1d; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 30px;">NO</span>`;

    } else {
        color = '#032b57';
        return `<span style="background-color: ${color}; color: white; padding: 3px 8px; border-radius: 5px; display: inline-block; min-width: 30px;">SI</span>`;
    }

    
}

function OpenUbicacionModal(latitud, longitud) {
    console.log("🗺️ Abriendo modal con coordenadas:", latitud, longitud);

    // Abre el modal
    $('#myModal').modal("show");

    // Evento al mostrarse completamente
    $('#myModal').off('shown.bs.modal').on('shown.bs.modal', function () {
        console.log("✅ Modal visible, inicializando mapa...");

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
        console.log("🧹 Modal cerrado, destruyendo mapa...");

        // Si existe un mapa, destrúyelo correctamente
        if (typeof map !== "undefined" && map) {
            try {
                map.destroy();   // Libera memoria del objeto ArcGIS
                console.log("🧭 Mapa destruido correctamente");
            } catch (err) {
                console.warn("⚠️ Error al destruir mapa:", err);
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
        url: AppRoutes.RegistroExpendio.UrlGetIntegrantes,
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
                                        <a style="color: #102717; cursor:pointer;" onclick="F_GetAntecedentes('${row.INTEGRANTE_DIREC_ID}')">
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
            { "title": "Cédula", "data": "CEDULA", class: "celdaCenter" }
            
        ],
        lengthMenu: [
            [5, 10, 25, 50, -1],
            ['5 registros', '10 registros', '25 registros', '50 registros', 'Todos']
        ],
        lengthChange: false,
        searching: false,
        ordering: false,
        pageLength: 10,
        paging: false,
        info: false
    });
}

function F_GetDelitosExpendios(CriminalidadId) {
    $.ajax({
        type: 'GET',
        url: AppRoutes.RegistroExpendio.UrlGetDelitosIris, // URL del endpoint que devuelve los datos
        dataType: 'json',
        data: { V_CriminalidadId: CriminalidadId },
        success: function (response) {

            // Inicializar la grilla con los datos filtrados o vacíos
            GetGrillaDelitosExpendios(response.data);

        },
        error: function () {
            // En caso de error, inicializar la grilla con datos vacíos
            GetGrillaDelitosExpendios([]);

        }
    });
}

function GetGrillaDelitosExpendios(Datos) {
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
            { "title": "Delito", "data": "DelitoDesc", class: "celdaCenter" },
            //{ "title": "Tipo", "data": "DescTipo" },
            //{ "title": "Tipo Informacón", "data": "DescTipoInfo" }

        ],

        lengthChange: false,
        searching: false,
        ordering: false,
        pageLength: 10,
        paging: false,
        info: false
    });
}


function F_GetBitacora(CriminalidadId) {
    $.ajax({
        type: 'GET',
        url: AppRoutes.RegistroExpendio.UrlGetBitacora, // URL del endpoint que devuelve los datos
        dataType: 'json',
        data: { V_CriminalidadId: CriminalidadId },
        success: function (response) {

            // Inicializar la grilla con los datos filtrados o vacíos
            GetGrillaBitacora(response.data);

        },
        error: function () {
            // En caso de error, inicializar la grilla con datos vacíos
            GetGrillaBitacora([]);

        }
    });
}

function GetGrillaBitacora(Datos) {
    if ($.fn.dataTable.isDataTable("#tbGrillaBitacora")) {
        $("#tbGrillaBitacora").DataTable().destroy();
    }

    $("#tbGrillaBitacora").empty();


    $("#tbGrillaBitacora").DataTable({
        destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        responsive: true,
        "columns": [
          
            { "title": "Decripción", "data": "Descripcion", "name": "Descripcion", className: "celdaCenter celda5" },
           
           
        ],
        lengthChange: false,
        searching: false,
        ordering: false,
        pageLength: 10,
        paging: false,
        info: false
    });
}


function F_GetResultados(CriminalidadId) {
    $.ajax({
        type: 'GET',
        url: AppRoutes.RegistroExpendio.UrlGetResultados, // URL del endpoint que devuelve los datos
        dataType: 'json',
        data: { V_CriminalidadId: CriminalidadId },
        success: function (response) {

            // Inicializar la grilla con los datos filtrados o vacíos
            GetGrillaResultados(response.data);

        },
        error: function () {
            // En caso de error, inicializar la grilla con datos vacíos
            GetGrillaResultados([]);

        }
    });
}

function GetGrillaResultados(Datos) {
   
    if ($.fn.dataTable.isDataTable("#tbGrillaResultdos")) {
        $("#tbGrillaResultdos").DataTable().destroy();
    }

    $("#tbGrillaResultdos").empty();


    $("#tbGrillaResultdos").DataTable({
        destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        responsive: true,
        "columns": [

            { title: "Tipo", data: "DescTipo" , class: "celdaCenter" },
            { title: "Sub-Tipo", data: "DescSubTipo", class: "celdaCenter" },
            { title: "Cantidad", data: "CANTIDAD", class: "celdaCenter" },
            { title: "Fecha", data: "FECHA", class: "celdaCenter",render: formatDate }


        ],
        lengthChange: false,
        searching: false,
        ordering: true,
        pageLength: 10,
        paging: false,
        info: false
    });
}


