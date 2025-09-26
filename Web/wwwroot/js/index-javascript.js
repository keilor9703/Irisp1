/*
 *Evento redes sociales  
 */
//Evento Icon facebook 
var contenedor_icon = $('.contenedor-icon');
var contenedor2 = $('.centrar');
contenedor_icon.mouseenter(function () {
    var parrafo = $('<p>').attr('class', 'texto-parrafo').text('Bienvenido a Facebook').css({'textAlign':'center', 'margin':'20px'});
    contenedor_icon.append(parrafo);
    contenedor2.css({ 'float': 'right', 'width': '80px' });

});

contenedor_icon.mouseleave(function () {
    var parrafo = $('.texto-parrafo').remove();
    contenedor2.css({ 'width': '100%' });
});

//Evento Icon twitter

var contenedor_icon_2 = $('.contenedor-icon-2');
var contenedor3 = $('.centrar-2');

contenedor_icon_2.mouseenter(function () {
    var parrafo = $('<p>').attr('class', 'texto-parrafo-2').text('Bienvenido a Twitter').css({ 'textAlign': 'center', 'margin': '20px', "fontSize": "x-small"});
    contenedor_icon_2.append(parrafo);
    contenedor3.css({ 'float': 'right', 'width': '80px' });

});
contenedor_icon_2.mouseleave(function () {
    var parrafo = $('.texto-parrafo-2').remove();
    contenedor3.css({ 'width': '100%' });
});

//Evento Icon Instagram

var contenedor_icon_3 = $('.contenedor-icon-3');
var contenedor4 = $('.centrar-3');

contenedor_icon_3.mouseenter(function () {
    var parrafo = $('<p>').attr('class', 'texto-parrafo-3').text('Bienvenido a Instagram').css({ 'textAlign': 'center', 'margin': '20px', "fontSize": "x-small" });
    contenedor_icon_3.append(parrafo);
    contenedor4.css({ 'float': 'right', 'width': '80px', });
});
contenedor_icon_3.mouseleave(function () {
    var parrafo = $('.texto-parrafo-3').remove();
    contenedor4.css({ 'width': '100%' });
});

//Evento Icon Youtube
var contenedor_icon_4 = $('.contenedor-icon-4');
var contenedor5 = $('.centrar-4');

contenedor_icon_4.mouseenter(function () {
    var parrafo = $('<p>').attr('class', 'texto-parrafo-4').text('Bienvenido a Youtube').css({ 'textAlign': 'center', 'margin': '20px' });
    contenedor_icon_4.append(parrafo);
    contenedor5.css({ 'float': 'right', 'width': '80px' });
});
contenedor_icon_4.mouseleave(function () {
    var parrafo = $('.texto-parrafo-4').remove();
    contenedor5.css({ 'width': '100%' });
});

//Evento Icon Flickr

var contenedor_icon_5 = $('.contenedor-icon-5');
var contenedor6 = $('.centrar-5');

contenedor_icon_5.mouseenter(function () {
    var parrafo = $('<p>').attr('class', 'texto-parrafo-5').text('Bienvenido a Flickr').css({ 'textAlign': 'center', 'margin': '20px', "fontSize": "x-small" });
    contenedor_icon_5.append(parrafo);
    contenedor6.css({ 'float': 'right', 'width': '80px' });
});
contenedor_icon_5.mouseleave(function () {
    var parrafo = $('.texto-parrafo-5').remove();
    contenedor6.css({ 'width': '100%' });
});

/*
 *Función Modal uno 
 */
function ActivarModal1() {
    var texto = $("#Texto1").html();
    $("#TextoModal1").html(texto);
    $("#primary-header-modal").modal("show");
}
//Función Modal dos
function ActivarModal2() {
    var texto = $("#Texto2").html();
    $("#TextoModal2").html(texto);
    $("#success-header-modal").modal("show");
}
//Función Modal tres
function ActivarModal3() {
    var texto = $("#Texto3").html();
    $("#TextoModal3").html(texto);
    $("#info-header-modal").modal("show");
}
/*
 *Calificación estrellas evento
 */

var puntuacion = $(".puntuacion");
puntuacion.click(function () {
    for (var i = 0; i <= puntuacion.lenght; i++) {
        puntuacion[i].css({ "Color": "blue"});
    } 
});






