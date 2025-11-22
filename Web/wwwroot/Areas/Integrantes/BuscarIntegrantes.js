
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

    // Manejo genérico para cualquier modal secundaria
    $(document).on('hidden.bs.modal', '.modal', function () {
        // Verifica si todavía hay alguna modal abierta
        if ($('.modal.show').length > 0) {
            $('body').addClass('modal-open');
        }
    });


    $("#btnConsultarIntegrante").on("click", function (e) {
        e.preventDefault();

        F_GetIntegrantesPorId($("#txtIdentificacion").val());
    });

    $("#btnLimpiarInteg").on("click", function (e) {
        e.preventDefault();

        Limpiar();
    });


    
});

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

function F_GetIntegrantesPorId(V_Identificacion) {

    $.ajax({
        type: "GET",
        url: AppRoutes.BuscarIntegrantes.UrlGetintegrantesPorId,
        data: { V_Identificacion: V_Identificacion },
        dataType: 'json',
        cache: false,
        success: function (resp) {

            if (resp.success && Array.isArray(resp.data) && resp.data.length === 1) {

                let item = resp.data[0];

                $("#txtAlias").val(item.alias);
                $("#txtNombres").val(item.nombre);
                $("#txtApellidos").val(item.apellido);
                $("#txtObservacion").val(item.observacion);

            } else {

                Swal.fire({
                    icon: 'info',
                    title: 'Señor(a) Funcionario(a)',
                    text: "No se encontró información con la identificación."
                });

                $("#txtAlias").val("");
                $("#txtNombres").val("");
                $("#txtApellidos").val("");
                $("#txtObservacion").val("");
            }

        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: "No es posible consultar la información."
            });
        }
    });

}

function F_GetListaIris(V_Indentificacion) {

    $.ajax({
        url: AppRoutes.RegistroReincidentes.UrlGetListaIris,
        type: "GET",
        data: V_Indentificacion,
        success: function (respuesta) {
           
            console.log("✅Respuesta exitosa:", respuesta);

            let data = respuesta.data || [];
            GetGrillaListaIris(data)

        },
        error: function (err) {
            console.error("ERROR:", err);
            GetGrillaListaIris([]);
        }
    });


}

function GetGrillaListaIris(Datos) {
  
    $("#pn_GrillaListaIris").removeClass('hidden');

    renderDataTable("#tbGrillaListaIris", Datos, [
        {
            data: null, className: "celdaCenter celda3", "render": function (data, type, row) {
                var DatosFila = JSON.stringify(row).replace(/"/g, '&quot;');
                var inicioBoton = '<div class="dropdown dropend"><button class="btn btn-success" type="button" id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false"><span class="fas fa-list"></span></button><ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1" style="line-height:23px;">';
                var Actualizar = `<li style="padding-left: 17px;"><a style="color: #102717;" href="javascript:OpenModalActualizar(${DatosFila})"><i class="fa fa-rotate-right green"></i>&nbsp;Actualizar</a></li>`;
                var Eliminar = `<li style="padding-left: 17px;"><a style="color: #102717;" href="javascript:P_DellReincidente(${DatosFila})"><i class="fa fa-trash red"></i>&nbsp;Eliminar</a></li>`;

                var finBoton = '</ul></div>';
                return inicioBoton + Actualizar + Eliminar + finBoton;
            }
        },
        { title: "Estado", data: "Codigo" },
        { title: "Unidad Verificación", data: "UnidadResponsable" },
        { title: "Dependencia", data: "Dependencia" },
        { title: "Municipio", data: "Municipio" },
        { title: "Fecha Inicio Actividad", data: "FechaInicioExistencia", render: formatDate },
        { title: "Clase", data: "Clase" },
        { title: "Nombre", data: "NombreClase" },
        { title: "Cantidad", data: "CantidadIntegrantes" },
      
        { title: "Zona", data: "Zona" },
        { title: "Tipo Servicio", data: "TipoServicio" },
        { title: "Fuente", data: "Fuente" },
        { title: "Fecha de Creacion", data: "FechaCreacion", render: formatDate },
       
        { title: "CriminalidadId", data: "CriminalidadId", visible: false }
    ]);
}



function Limpiar() {

    $("#txtIdentificacion").val("");
    $("#txtAlias").val("");
    $("#txtNombres").val("");
    $("#txtApellidos").val("");
    $("#txtObservacion").val("");
   

    
}
