$(document).ready(function () {



    $('#ddlAnioIris').on('change', function () {
        F_GetInfoGrillas();
    });

    F_GetInfoGrillas($('#ddlAnioIris').val());



});


function F_GetInfoGrillas() {
    $.ajax({
        type: 'GET',
        url: AppRoutes.ReportesGeneral.UrlGetInfoGrillas,
        dataType: 'json',
        data: { anio: $('#ddlAnioIris').val() },
        success: function (response) {
           // console.log("📩 Respuesta recibida:", response);

            // Validar si la respuesta es exitosa
            if (response && response.success === true) {
                let data = response.data || [];
              //  console.log("✅ Datos cargados:", data);
                GetGrillaLista(data);
            } else {
                console.warn("⚠️ Respuesta no exitosa o success=false");
                GetGrillaLista([]); // devolvemos grilla vacía
            }
        },
        error: function (xhr, status, error) {
            console.error("❌ Error Ajax:", status, error);
            console.error("Respuesta cruda:", xhr.responseText);
            GetGrillaLista([]); // devolvemos grilla vacía
        }
    });
}

// 🔧 Función utilitaria para inicializar o refrescar tablas
// renderDataTable ahora vive en /js/IniciarTabla.js (compartida por todas las grillas del sitio).


function GetGrillaLista(Datos) {

    $("#pn_GrillaGeneral").removeClass('hidden');

    renderDataTable("#tbGrillaGeneral", Datos, [

        Estados(), // usa: estado_descripcion
        EstadosExistencia(), // usa: estado_existencia_descripcion

        { title: "Código", data: "codigo" },

        { title: "Origen", data: "origen" }, // Origen del registro

        { title: "Delito Principal", data: "delito_principal" },
        { title: "Región", data: "region_p" },
        { title: "Unidad", data: "sigla_unidad" },
        { title: "Dependencia", data: "dependencia" },
        { title: "Cuadrante", data: "nro_cuadrante" },
        { title: "Municipio", data: "municipio" },
        { title: "Zona", data: "zona" },
        { title: "Clase", data: "clase" },
        { title: "Fuente", data: "fuente" },
        { title: "Tipo Servicio", data: "tipo_servicio" },
        { title: "Nombre Clase", data: "nombre_clase" },

        { title: "Fecha Inicio Actividad", data: "fecha_inicio_existencia_str" },

        { title: "Cantidad Integrantes", data: "cantidad_integrante" },

        columnaCaracteristicasGenerales(), // usa: caracteristicas_generales

        { title: "Fecha creación", data: "fecha_creacion_irisp1_str" },

        { title: "Funcionario Informa", data: "funcionario_informa" },
        { title: "Unidad Funcionario Informa", data: "unidad_funcionario_informa" },
        { title: "Identificación Funcionario", data: "identificacion_informa" },
        
        columnaDescripcionTramite(),

        { title: "Unidad Verificación", data: "unidad_verifica" },
        { title: "Fecha Asignación Verificación", data: "fecha_asig_tarea_verifica_str" },
        { title: "Fecha Respuesta Verificación", data: "fecha_resp_tarea_verifica_str" },

        { title: "Unidad Proceso Investigativo", data: "unidad_asig_inves" },
        { title: "Fecha Asignación Investigativa", data: "fecha_asig_tarea_inves_str" },
        { title: "Fecha Respuesta Investigativa", data: "fecha_resp_tarea_inves_str" },

        { title: "Longitud", data: "longitud" },
        { title: "Latitud", data: "latitud" },

        { title: "Municipio 2", data: "municipio_2" },
        { title: "Barrio", data: "barrio" },
        { title: "Dirección", data: "direccion" },

        { title: "Cantidad SPOA", data: "cantidad_spoa" },
        { title: "NUNC", data: "nunc" },
        { title: "Cantidad SIEDCO", data: "cantidad_siedco" },

        { title: "CriminalidadId", data: "criminalidad_id", visible: false }
    ]);
}



// Estados()/EstadosExistencia(): el mapeo de colores vive en /js/IniciarTabla.js (columnaEstadoGrilla),
// compartido por todos los módulos que muestran estas dos columnas.
// Nota: este endpoint devuelve los campos en snake_case ("estado_descripcion"), a diferencia de
// Irisp1/Seguimiento/Verificación que usan PascalCase — se respeta tal cual viene del backend.
function Estados() {
    return columnaEstadoGrilla("Estado", "estado_descripcion");
}

function EstadosExistencia() {
    return columnaEstadoGrilla("Estado Existencia", "estado_existencia_descripcion");
}

function columnaCaracteristicasGenerales() {
    return {
        title: "Características Generales",
        data: "caracteristicas_generales",
        name: "caracteristicas_generales",
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
        data: "descripcion_tramite",
        name: "descripcion_tramite",
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


// ------------------------
// EXPORTAR REPORTE GENERAL (Excel)
// ------------------------
$("#btnExcel").on("click", function () {
    let anio = $("#ddlAnioIris").val();

    window.location.href =
        "Reportes/ReporteGeneral/ExportarExcelReporteGeneral?anio=" +
        encodeURIComponent(anio);
});

