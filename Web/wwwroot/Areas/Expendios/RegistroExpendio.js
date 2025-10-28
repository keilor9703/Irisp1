
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

    $('#ddlTipoEstado').select2();

    $('#ddlAnioIris').on('change', function () {
        F_GetInfoGrillas();
    });


   
    //F_GetInfoGrillas($('#ddlAnioIris').val());


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
    $("#btnConsultarIntegrante").on("click", function (e) {
        e.preventDefault();
      
        F_GetIntegranteAll($('#txtIdentificacion').val());
    });

    $("#btnConsultarIntegranteUpd").on("click", function (e) {
        e.preventDefault();

        F_GetIntegranteAll($('#txtIdentificacionUpd').val());
    });

    $("#btnAddIntegranteExpendio").on("click", function (e) {
        e.preventDefault();

        P_InsIntegranteExpendio();
    });

    $("#btnLimpiarIntegExpendio").on("click", function (e) {
        e.preventDefault();

        Limpiar();
    });

    $("#btnLimpiarIntegExpendioUpd").on("click", function (e) {
        e.preventDefault();

        Limpiar();
    });

    $("#btnAddDelitosExpendios").on("click", function (e) {
        e.preventDefault();

        P_InsInsDelitoExpendio();
    });

    $("#btnAddInfoBitacora").on("click", function (e) {
        e.preventDefault();

        P_InsInsBitacora();
    });

    $("#btnAddResultadoExpendio").on("click", function (e) {
        e.preventDefault();

        P_InsInsResultadosExpendios();
    });

    $("#btnUpdExpendio").on("click", function (e) {
        e.preventDefault();

        P_UpdExpendio();
    });

    $("#btnNuevoExpendio").on("click", function (e) {
        e.preventDefault();

        AbrirModalNuevoExpendio();
    });

    $("#btnUpdIntegranteExpendio").on("click", function (e) {
        e.preventDefault();

       

        Swal.fire({
            title: '¿Está seguro?',
            text: 'Esta acción actualizará el registro?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                P_UpdIntegrante($('#txtIntegranteIdModal').val());
            }
        });



    });

    $("#btnAbrirMapa").on("click", function (e) {
        e.preventDefault();

        $('#myModal').modal("show");
        inicializarMapa('mapaDiv');
    });

});


$("#txtIdentificacion" ).keyup(function (event) {
    if (event.keyCode === 13) {
        $("#btnConsultarIntegrante").click();
    }
}); $("#txtIdentificacionUpd" ).keyup(function (event) {
    if (event.keyCode === 13) {
        $("#btnConsultarIntegranteUpd").click();
    }
});


$('#ddlTipoEstado').change(function () {
    var EstadoSeleccionado = $("#ddlTipoEstado option:selected").text().trim();
    console.log(EstadoSeleccionado);

    // Ocultar todos los campos primero
    $('#txtNUNC2').closest('.col-md-6').addClass('hidden');
    $('#txtSIEDCO2').closest('.col-md-6').addClass('hidden');
    $('#txtCodigoOperacion2').closest('.col-md-6').addClass('hidden');
    $('#txtNombreOperacion2').closest('.col-md-6').addClass('hidden');
    $('#ddlErradicado2').closest('.col-md-6').addClass('hidden');
    $('#txtObservaciones2').closest('.col-md-12').addClass('hidden');

    // Mostrar según el estado seleccionado
    switch (EstadoSeleccionado) {
        case "Investigación":
            $('#txtNUNC2').closest('.col-md-6').removeClass('hidden');
            $('#txtCodigoOperacion2').closest('.col-md-6').removeClass('hidden');
            $('#txtNombreOperacion2').closest('.col-md-6').removeClass('hidden');
            break;

        case "Finalizado":
            $('#txtSIEDCO2').closest('.col-md-6').removeClass('hidden');
            $('#ddlErradicado2').closest('.col-md-6').removeClass('hidden');
            break;

        case "Descartado":
            $('#txtObservaciones2').closest('.col-md-12').removeClass('hidden');
            break;
    }
});


$('#ddlTipoResultado').change(function () {
    const valor = $(this).val();

    if (valor && !isNaN(valor)) {
        handleDropdownChange('/Expendios/Registros/F_GetDominiosIris', { V_id: valor }, '#ddlSubTipoResultado');
    } else {
        console.warn("Valor inválido o vacío:", valor);
    }
});


function handleDropdownChange(url, params, dropdownSelector, callback) {
    if (params && params.V_id) {
        $.getJSON(url, params, function (data) {
            const dropdown = $(dropdownSelector);
            dropdown.empty().append('<option value="">Seleccione</option>');

            if (data.success && Array.isArray(data.data) && data.data.length > 0) {
                $.each(data.data, function (index, item) {
                    if (item && item.Descripcion) {
                        dropdown.append(`<option value="${item.IdDominio}">${item.Descripcion}</option>`);
                    }
                });
            } else {
                console.warn("No hay datos válidos o success = false", data);
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

$('#ddlUnidadExpendio').change(function () {
    const valor = $(this).val();

    if (valor) {
        PoblarListaDesplegable('/Expendios/Registros/F_GetEstaciones', { V_Sigla: valor }, '#ddlEstacionExpendio');
        PoblarListaDesplegable('/Expendios/Registros/F_GetEspecialidad', { V_Sigla: valor }, '#ddlunidadInformaExpendio');
    } else {
        console.warn("Valor inválido o vacío:", valor);
    }
});

$('#ddlCategoria').change(function () {
    const valor = $(this).val();

    if (valor == 105) {

        $('#txtOtraCategoriaExpendio').closest('.col-md-3').removeClass('hidden');
    } else {

        $('#txtOtraCategoriaExpendio').closest('.col-md-3').addClass('hidden');
    }



});
function PoblarListaDesplegable(url, params, dropdownSelector, callback) {
    if (params && params.V_Sigla) {
        $.getJSON(url, params, function (data) {
            const dropdown = $(dropdownSelector);
            dropdown.empty().append('<option value="">Seleccione</option>');

            if (data.success && Array.isArray(data.data) && data.data.length > 0) {
                $.each(data.data, function (index, item) {
                    if (item && item.Descripcion) {
                        dropdown.append(`<option value="${item.CONSECUTIVO}">${item.Descripcion}</option>`);
                    }
                });
            } else {
                console.warn("No hay datos válidos o success = false", data);
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
            GetGrillaExpendios(data);
           
        },
        error: function (xhr, status, error) {
            console.error("❌ Error Ajax:", status, error);
            console.error("Respuesta cruda:", xhr.responseText);
            GetGrillaExpendios([]);
           
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


function GetGrillaExpendios(Datos) {
    

    //if ($.fn.dataTable.isDataTable("#tbGrillaExpendios")) {
    //    $("#tbGrillaExpendios").DataTable().destroy();
    //}

    if ($.fn.dataTable.isDataTable('#tbGrillaExpendios')) {
        const table = $('#tbGrillaExpendios').DataTable();
        table.clear();
        table.rows.add(Datos);
        table.draw(false);
        return;
    }


    $("#pn_GrillaExpendios").removeClass('hidden');
    $("#tbGrillaExpendios").DataTable({
       // destroy: true,
        data: Datos,
        language: glOpcionesIdioma,
        scrollX: true,          // ✅ Activa scroll horizontal
        scrollCollapse: true,   // ✅ Permite colapsar si hay menos columnas
        responsive: false,      // ✅ Desactiva comportamiento que oculta columnas
        autoWidth: false,       // ✅ Evita cálculos automáticos de ancho que rompen el scroll
        "columns": [
            columnaAcciones(data),
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
            { title: "Erradicado ?", data: "Erradicado", render: RenderErradicadoModal },
            { title: "Barrio", data: "Barrio" },
            { title: "Direccion", data: "Direccion" },
            { title: "Latitud", data: "Latitud" },
            { title: "Longitud", data: "Longitud" },
            { title: "Cuadrante", data: "Cuadrante" },
            { title: "Municipio", data: "Municipio" },
            { title: "Fecha Creacion", data: "FechaCreacion", render: formatDate },
            { title: "CriminalidadDirecId", data: "CriminalidadDirecId", visible: false }
        ],
        lengthMenu: [
            [15, 25, 50, -1],
            ['15 registros', '25 registros', '50 registros', 'Todos']
        ],
        ordering: false,
        pageLength: 15,
        bLengthChange: true,
        searching: true,
        paging: true,
        info: true,
         
    });
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
                                    <a style="color: #102717;" href="javascript:F_AbrirMdodalActualizar('${row.CriminalidadDirecId}')">
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


function F_AbrirMdodalActualizar(CriminalidadId){

    $("#txtCriminalidadIdModal").val(CriminalidadId),

    $('#Modal_UpdEstadoExendio').modal("show");
}

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
    if (!Valor || Valor === 0) {
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

                    var DatosFila = JSON.stringify(row).replace(/"/g, '&quot;');

                    var ActualizarDatos = `<li style="padding-left: 17px;">
                                        <a style="color: #102717; href="#"
                                       class="btn-actualizar-expendio"
                                       data-datos="${DatosFila}">
                                        <i class="fas fa-retweet green"></i>&nbsp; Actualizar Datos
                                        </a>
                                    </li>`;

                    var finBoton = '</ul></div>';
                    return inicioBoton + ActualizarDatos + finBoton;
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


// Delegación de eventos para los botones de detalle
$(document).on("click", ".btn-actualizar-expendio", function (e) {
    e.preventDefault();

    // Recuperamos el JSON guardado en data-datos
    var datosAttr = $(this).attr("data-datos").replace(/&quot;/g, '"');

    try {
        var registro = JSON.parse(datosAttr);
        F_AbrirMdodalActualizarIntegrante(registro);
    } catch (err) {
        console.error("❌ Error parseando data-datos:", err, datosAttr);
        Swal.fire('Error', 'No se pudo procesar el detalle del registro', 'error');
    }
});


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
          
            { "title": "Delito", "data": "DelitoDesc", class: "celdaCenter" },
           
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

function F_GetIntegranteAll(P_Identificacion) {
    $.ajax({
        type: 'GET',
        url: AppRoutes.RegistroExpendio.UrlGetIntegrantesAll, // Endpoint que devuelve los datos
        dataType: 'json',
        data: { V_Identificacion: P_Identificacion },
        success: function (response) {
            if (response.success) {
                let data = response.data || [];

               // $("#txtFuncionario").val(respuesta.data[0].Funcionario);
                $("#txtAliasModal").val(data[0].ALIAS);
                $("#txtNombreIntegModal").val(data[0].NOMBRE);
                $("#txtApellidosIntegModal").val(data[0].APELLIDO);
                $("#txtAliasModal").val(data[0].ALIAS);

                $("#txtAliasModal")
                    .addClass("readonly")
                    .prop("readonly", true);
              
                $("#txtNombreIntegModal")
                    .addClass("readonly")
                    .prop("readonly", true);

                $("#txtApellidosIntegModal")
                    .addClass("readonly")
                    .prop("readonly", true);


                $("#txtAliasModalUpd").val(data[0].ALIAS);
                $("#txtNombreIntegModalUpd").val(data[0].NOMBRE);
                $("#txtApellidosIntegModalUpd").val(data[0].APELLIDO);
                $("#txtAliasModal").val(data[0].ALIAS);

               
               
            } else {


                $("#txtAliasModal").val('');
                $("#txtNombreIntegModal").val('');
                $("#txtApellidosIntegModal").val('');

                $("#txtAliasModalUpd").val('');
                $("#txtNombreIntegModalUpd").val('');
                $("#txtApellidosIntegModalUpd").val('');



                $("#txtAliasModal")
                    .removeClass("readonly")
                    .prop("readonly", false);

                $("#txtNombreIntegModal")
                    .removeClass("readonly")
                    .prop("readonly", false);


                $("#txtApellidosIntegModal")
                    .removeClass("readonly")
                    .prop("readonly", false);

               

                
                Swal.fire({
                    icon: 'error',
                    title: 'Señor(a) Funcionario(a):',
                    text: (response.message ? response.message + ' - ' : '') + 'La identificación suministrada no se encuentra relacionada en algún IRISP1 !!!'
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

function P_InsIntegranteExpendio() {

    const Obj_Integrante = {

        CRIMINALIDAD_DIREC_ID: $("#txtCriminalidadIdModal").val(),
        CEDULA: $("#txtIdentificacion").val(),
        ALIAS: $("#txtAliasModal").val(),
        NOMBRE: $("#txtNombreIntegModal").val(),
        APELLIDO: $("#txtApellidosIntegModal").val(),
    }

    // 🔹 Validar campos obligatorios (excepto Observacion)
    for (let key in Obj_Integrante) {
        if (key !== 'ALIAS' || key !== 'CEDULA' || key !== 'APELLIDO') {
            const val = Obj_Integrante[key];
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
        url: AppRoutes.RegistroExpendio.UrlInsIntgrante,
        type: 'POST',
        data: Obj_Integrante,
        success: function (resp) {
            if (resp.success) {

                Swal.fire({
                    type: 'success',
                    title: 'Señor(a) Funcionario(a:)',
                    text: resp.message
                });
                $('#Modal_InsIntegrantesExendios').modal('hide');

                    $("#txtIdentificacion").val('');
                    $("#txtAliasModal").val('');
                    $("#txtNombreIntegModal").val('');
                    $("#txtApellidosIntegModal").val('');

                F_GetIntegrantesIris($("#txtCriminalidadIdModal").val());
               

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
function P_InsInsDelitoExpendio() {

    const Obj_Delito = {

        CRIMINALIDAD_DIREC_ID: $("#txtCriminalidadIdModal").val(),
        IdDelito: $("#ddlDelitoModal").val(),
       
    }


    // 🔹 Validar campos obligatorios (excepto Observacion)
    for (let key in Obj_Delito) {
        
            const val = Obj_Delito[key];
            if (!val || val === '' || val === undefined || (typeof val === 'number' && isNaN(val))) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Señor(a) Funcionario(a):',
                    text: 'Valide todos los campos para completar el registro.'
                });
                return;
            }
    }

    $.ajax({
        url: AppRoutes.RegistroExpendio.UrlInsDelito,
        type: 'POST',
        data: Obj_Delito,
        success: function (resp) {
            if (resp.success) {

                Swal.fire({
                    type: 'success',
                    title: 'Señor(a) Funcionario(a:)',
                    text: resp.message
                });
                $('#Modal_InsDelitosExpendios').modal('hide');

                $("#ddlDelitoModal").val('').trigger('change');
               
                F_GetDelitosExpendios($("#txtCriminalidadIdModal").val());


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


function P_InsInsBitacora() {

    const Obj_Bitacora = {

        CRIMINALIDAD_DIREC_ID: $("#txtCriminalidadIdModal").val(),
        Descripcion: $("#txtInfoBitacoraModal").val(),

    }


    // 🔹 Validar campos obligatorios (excepto Observacion)
    for (let key in Obj_Bitacora) {

        const val = Obj_Bitacora[key];
        if (!val || val === '' || val === undefined || (typeof val === 'number' && isNaN(val))) {
            Swal.fire({
                icon: 'warning',
                title: 'Señor(a) Funcionario(a):',
                text: 'Valide todos los campos para completar el registro.'
            });
            return;
        }
    }

    $.ajax({
        url: AppRoutes.RegistroExpendio.UrlInsBitacora,
        type: 'POST',
        data: Obj_Bitacora,
        success: function (resp) {
            if (resp.success) {

                Swal.fire({
                    type: 'success',
                    title: 'Señor(a) Funcionario(a:)',
                    text: resp.message
                });
                $('#Modal_InsInfoBitacora').modal('hide');

                $("#txtInfoBitacoraModal").val('');

                F_GetBitacora($("#txtCriminalidadIdModal").val());


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


function P_InsInsResultadosExpendios() {

    const Obj_Resultados = {

        CRIMINALIDAD_DIREC_ID: $("#txtCriminalidadIdModal").val(),
        ID_TIPO: $("#ddlTipoResultado").val(),
        ID_SUBTIPO: $("#ddlSubTipoResultado").val(),
        CANTIDAD: $("#txtCantidadModal").val(),
        FECHA: $("#txtFecha").val(),

    }


    // 🔹 Validar campos obligatorios (excepto Observacion)
    for (let key in Obj_Resultados) {
        if (key !== 'ID_SUBTIPO') {
            const val = Obj_Resultados[key];
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
        url: AppRoutes.RegistroExpendio.UrlInsResultados,
        type: 'POST',
        data: Obj_Resultados,
        success: function (resp) {
            if (resp.success) {

                Swal.fire({
                    type: 'success',
                    title: 'Señor(a) Funcionario(a:)',
                    text: resp.message
                });
                $('#Modal_InsResultadosExendios').modal('hide');

              
                $("#ddlTipoResultado").val('').trigger('change');
                $("#ddlSubTipoResultado").val('').trigger('change');
                $("#txtCantidadModal").val('');
                $("#txtFecha").val('');

                F_GetResultados($("#txtCriminalidadIdModal").val());


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

function P_UpdExpendio() {
    var TipoEstado = $("#ddlTipoEstado").val();
    let Obj_UpdExpendio = {};

    // Construcción del objeto según el estado
    switch (parseInt(TipoEstado)) {
        case 107: // Investigación
            Obj_UpdExpendio = {
                CriminalidadDirecId: $("#txtCriminalidadIdModal").val(),
                IdEstado: TipoEstado,
                Nunc: $("#txtNUNC2").val(),
                CodigoMored: $("#txtCodigoOperacion2").val(),
                NombreMored: $("#txtNombreOperacion2").val()
            };
            break;

        case 108: // Erradicado
            Obj_UpdExpendio = {
                CriminalidadDirecId: $("#txtCriminalidadIdModal").val(),
                IdEstado: TipoEstado,
                Siedco: $("#txtSIEDCO2").val(),
                Erradicado: $("#ddlErradicado2").val()
            };
            break;

        case 109: // Observación
            Obj_UpdExpendio = {
                CriminalidadDirecId: $("#txtCriminalidadIdModal").val(),
                IdEstado: TipoEstado,
                Observacion: $("#txtObservaciones2").val()
            };
            break;

        default:
            Swal.fire('Advertencia', 'Debe seleccionar un tipo de estado válido.', 'warning');
            return;
    }

    // Validación general de campos vacíos
    for (let key in Obj_UpdExpendio) {
        const val = Obj_UpdExpendio[key];
        if (!val || val === '' || val === undefined || (typeof val === 'number' && isNaN(val))) {
            Swal.fire({
                icon: 'warning',
                title: 'Señor(a) Funcionario(a):',
                text: 'Valide todos los campos para completar el registro.'
            });
            return;
        }
    }

    // Envío AJAX
    $.ajax({
        url: AppRoutes.RegistroExpendio.UrlUpdExpendio,
        type: 'POST',
        data: Obj_UpdExpendio,
        success: function (resp) {
            if (resp.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Señor(a) Funcionario(a):',
                    text: resp.message
                });

                $('#Modal_UpdEstadoExendio').modal('hide');

                // Limpieza de campos
                $("#ddlTipoEstado").val('').trigger('change');
                $("#ddlErradicado2").val('').trigger('change');
                $("#txtNUNC2, #txtSIEDCO2, #txtCodigoOperacion2, #txtNombreOperacion2, #txtObservaciones2").val('');

                
                F_GetInfoGrillas();

            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Error al actualizar: ' + resp.message
                });
            }
        },
        error: function () {
            Swal.fire('Error', 'Fallo en la llamada AJAX.', 'error');
        }
    });
}


function P_UpdIntegrante(INTEGRANTE_ID) {



    const Obj_Integrante = {

        CRIMINALIDAD_DIREC_ID: $("#txtCriminalidadIdModal").val(),
        INTEGRANTE_DIREC_ID: INTEGRANTE_ID,
       // CEDULA: $("#txtIdentificacion").val(),
        ALIAS: $("#txtAliasModalUpd").val(),
        NOMBRE: $("#txtNombreIntegModalUpd").val(),
        APELLIDO: $("#txtApellidosIntegModalUpd").val(),
    }

    const camposExcluidos = ['ALIAS', 'CEDULA', 'APELLIDO'];

    for (let key in Obj_Integrante) {
        if (!camposExcluidos.includes(key)) {
            const val = Obj_Integrante[key];
            if (!val || val === '' || val === undefined || (typeof val === 'number' && isNaN(val))) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Señor(a) Funcionario(a):',
                    text: `Valide el campo ${key} para completar el registro.`
                });
                return;
            }
        }
    }


    $.ajax({
        url: AppRoutes.RegistroExpendio.UrlUpdIntegrante,
        type: 'POST',
        data: Obj_Integrante,
        success: function (resp) {
            if (resp.success) {

                Swal.fire({
                    type: 'success',
                    title: 'Señor(a) Funcionario(a:)',
                    text: resp.message
                });
                $('#Modal_UpdIntegrantesExendios').modal('hide');

                $("#txtIdentificacionUpd").val('');
                $("#txtAliasModalUpd").val('');
                $("#txtNombreIntegModalUpd").val('');
                $("#txtApellidosIntegModalUpd").val('');

                F_GetIntegrantesIris($("#txtCriminalidadIdModal").val());


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


function Limpiar() {
   
    $("#txtIdentificacionUpd").val("");
    $("#txtAliasModalUpd").val("");
    $("#txtNombreIntegModalUpd").val("");
    $("#txtApellidosIntegModalUpd").val("");
    $("#txtIdentificacion").val("");
    $("#txtAliasModal").val("");
    $("#txtNombreIntegModal").val("");
    $("#txtApellidosIntegModal").val("");
    
}
function F_AbrirMdodalActualizarIntegrante(DatosInegrante) {

    $("#txtIntegranteIdModal").val(DatosInegrante.INTEGRANTE_DIREC_ID);
    $("#txtAliasModalUpd").val(DatosInegrante.ALIAS);
    $("#txtNombreIntegModalUpd").val(DatosInegrante.NOMBRE);
    $("#txtApellidosIntegModalUpd").val(DatosInegrante.APELLIDO);
    $("#txtIdentificacionUpd").val(DatosInegrante.CEDULA);


    $('#Modal_UpdIntegrantesExendios').modal("show");


}


function AbrirModalNuevoExpendio() {

    $('#Modal_RegistroEpendio').modal("show");

}

function obtenerDelitosSeleccionados() {
    const delitos = [];
    $('#ddlDelitosRelacionados option:selected').each(function () {
        delitos.push($(this).val());
    });
    console.log("Delitos seleccionados: ", delitos);
    return delitos;
}




