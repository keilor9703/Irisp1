namespace Web.wwwroot.Areas.Irisp1
{
    public class MapaIrisP1
    {
    }


    $(document).ready(function () {


        if ($.fn.select2) {
            $('#ddlAnioIris').select2();
        }

         Asocia el evento change
        $('#ddlAnioIris').on('change', function () {
            F_GetInfoGrillas();
        });


        $('.select2').select2({
            placeholder: "Seleccione",
            allowClear: true
        });
    });


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
            url: UrlConsultarAnioMapa,
            data: { _anioMapa: txtAnio },
            success: function (datos) {
                if (datos && datos.length > 0) {
                    $("#pnMapa").removeClass('hidden');
                    $("#pnGrillaMapa").removeClass('hidden');
                    CargarTbMapa(datos);
                } else {

                    $("#pnGrillaMapa").addClass('hidden');
                    ModalInfo("Sin resultados", "No se encontraron hechos para el año seleccionado.");
                }
            },
            error: function (ex) {
                $("#pnMapa").addClass('hidden');
                Swal.fire({
                    type: 'info',
                    title: 'Señor(a) Funcionario(a:)',
                    text: "El año no tiene Iris registrados, ¿desea hacerlo?"
                });

            }

        });
    }


}
