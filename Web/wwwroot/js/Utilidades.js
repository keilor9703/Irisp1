
//Funcion Para Extablecer Tamaño Auto de un elemento textarea
function auto_grow(element) {

    let AltoTextArea = (element.scrollHeight) + "px";
    element.style.setProperty('height', AltoTextArea, 'important');
}

(function initializeSelect2Modal(IdCampo, IdModal) {
    $('.selectInputModal').select2({ dropdownParent: $('.selectModal') });

    //$('.select2').each(function () {
    //    $(this).select2({
    //        dropdownParent: $(this).parent()
    //    });
    //})
})();

//en caso de Necesitar una segunda modal con lista desplegable utilizar esta
(function initializeSelect2Modal2(IdCampo, IdModal) {
    $('.selectInputModal2').select2({ dropdownParent: $('.selectModal2') });
})();
