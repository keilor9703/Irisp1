
var map;
var strAddress;
var locator;
var latDelito = 0;
var lonDelito = 0;
var numCuadrante, numCuadrante1, ent_administrativa, barrio, cod_estacion, cod_cai, cua_rural, nom_municipio;
var scaleBar;
var strInformacion;
var mapPoint;
var address;
var infoWindow;
var markColor = "0a9242";


require([
    "esri/map",
    "esri/arcgis/utils",
    "esri/dijit/BasemapGallery",
    "esri/dijit/LayerList",
    "esri/dijit/Scalebar",
    "esri/dijit/Search",
    "esri/dijit/LocateButton",
    "esri/dijit/Geocoder",
    "esri/geometry",
    "esri/geometry/webMercatorUtils",
    "esri/tasks/locator",
    "esri/InfoTemplate",
    "esri/tasks/IdentifyTask",
    "esri/tasks/IdentifyParameters",
    "esri/graphic",
    "esri/dijit/InfoWindow",
    "esri/geometry/Point",
    "dojo/parser",
    "dojo/dom-construct",
    "dojo/query",
    "dojo/on",
    "dojo/dom-attr",
    "dojo/_base/array",
    "dojo/dom",
    "dojo/promise/all",
    "dijit/layout/BorderContainer",
    "dijit/layout/ContentPane",
    "dijit/TitlePane",
    "dojo/domReady!"
], function (

    Map,
    arcgisUtils,
    BasemapGallery,
    LayerList,
    Scalebar,
    Search,
    LocateButton,
    Geocoder,
    Geometry,
    webMercatorUtils,
    Locator,
    InfoTemplate,
    IdentifyTask,
    IdentifyParameters,
    Graphic,
    Point,
    InfoWindow,
    parser,
    domConstruct,
    query,
    on,
    domAttr,
    array,
    dom,
    all


) {
        parser.parse();

        /**
         * Crea un objeto mapa a partir de un objeto Web Map de ArcGIS Online.
         * Todas las capas (operacionales y mapas base), simbologia, escalas de visualizacion son tomadas del Web Map.
         * Si el Mapa no es publico, la aplicacion solicitara credeniales.
         */

        //Si se quiere acceder a un webmap de Portal se debe quitar el comentario siguiente y  reemplazar el id del webmap segun se requiera.
        //arcgisUtils.arcgisUrl = "https://srvportalgis.policia.gov.co/arcgis/sharing/rest/content/items";
        arcgisUtils.createMap("da89bd5e69024aca92bc36e5908b3233", "map").then(function (response) {
            //Actualiza el titulo de la pagina web de acuerdo a titulo del webmap
            document.title = "Creacion IRISP1";

            map = response.map;
            window.map = response.map;

            /**
             * Agrega  el widget Galeria de Mapas Base a la ventana de Mapa
             * @showArcGISBasemaps {Boolean}          Determina si se cargan o no los mapas Base de ArcGIS y Bing a la Galeria.
             * @map: Objeto Mapa creado previamente.
             */

            var basemapGallery = new BasemapGallery({
                showArcGISBasemaps: true,
                map: response.map
            }, "basemapGallery");
            basemapGallery.startup();

            basemapGallery.on("error", function (msg) {
                console.log("Ha ocurrido un error cargando la Galeria de Mapas Base:  ", msg);
            });

            /**
             * Agrega  el widget Scalebar a la ventana de Mapa que despliega una barra de escala en la parte inferior izquierda de la pantalla.
             * @scalebarUnit {String}. Los valores permitidos son  "english" o "metric" o "dual", si se selecciona dual, la barra de escala mostrara unidades en el sistema metrico y en el sistema ingles.
             * @map: Objeto Mapa creado previamente.
             */

            scaleBar = new Scalebar({
                map: response.map,
                scalebarUnit: "metric"
            });

            /**
             * Agrega  el widget LayerList a la ventana de Mapa el cual permite administrar la visibilidad y transparencia de las capas operacionales agregadas al mapa. Este widget respeta los niveles de
             * visualizacion de las capas configurados en el webmap
             * @map: Objeto Mapa creado previamente.
             * @showLegend: {Boolean} indica si se despliega o no la leyenda de las capas operacionales
             * @showSubLayers: {Boolean} indica si se despliegan o no las subcapas asociadas a las capas operacionales
             * @showOpacitySlider:  {Boolean} Indica si se debe desplegar el slider de transparencia
             * @layers: {array} lista de capas que seran cargadas en el control, por defecto se cargaran todas las capas operacionales del webmap indicado previamente.
             */

            var listLayers = new LayerList({
                map: response.map,
                showLegend: true,
                showSubLayers: true,
                showOpacitySlider: true,
                layers: arcgisUtils.getLayerList(response)
            }, "layerList");
            listLayers.startup();

            /**
             * Agrega  el widget Search a la ventana de Mapa el cual permite realizar la busqueda por direccion
             * @map: Objeto Mapa creado previamente.
             * @countryCode: {String} Restingue las busquedas al pais indicado en a traves del Codigo corto, 'COL' o 'CO' sirven para Colombia
             */

            var search = new Search({
                map: response.map,
                countryCode: 'COL,CO'

            }, "search");
            search.startup(executeIdentifyTask);

            /**
             * Agrega  el widget LocateButton a la ventana de Mapa el cual permite localizar y hacer zoom a la posicion (localizacion) del usuario.
             * Solo se activa si la pagina se carga como https (de forma segura).
             * @map: Objeto Mapa creado previamente.
             */

            geoLocate = new LocateButton({
                map: response.map
            }, "LocateButton");
            geoLocate.startup();

            /**
             * Agrega  el widget personalizado enviarInformacionLink el cual reescribe el comportamiento de la ventana infoWindow del mapa y agrega un tag html de tipo "a" a la seccion actionList de la misma.
             * @map: Objeto Mapa creado previamente.
             */

            enviarInformacionLink = domConstruct.create("a", {
                "class": "action",
                "id": "infoLink",
                "innerHTML": "Capturar coordenadas",
                "href": "javascript:void(0);"
            }, query(".actionList", window.map.infoWindow.domNode)[0]);

            //on(enviarInformacionLink, "click", procesoInformacion);

            map.on("load", mapReady);
            // Registra los eventos click y mouse-move en el mapa y les agrega funciones especificas a cada uno
            map.on("click", executeIdentifyTask);
            map.on("mouse-move", showCoordinates);

            //Registra la URL del servicio de geocodificacion a usar para buscar por direcciones.
            locator = new esri.tasks.Locator("https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer");

            //Agrega un comportamiento adicional al widget Buscar, una vez el usuario selecciona una de las direcciones sugeridas el control se encarga de realizar la consulta de informacion de localizacion que se asociara al
            //delito (Codigo SIEDCO, Cod DANE, Direccion, Cuadrante, etc.)

            //Genera busqueda sobre la opcion seleccionada 
            on(search, 'select-result', function (response) {
                //map.infoWindow.hide();
                map.centerAndZoom(response.result.feature.geometry, 15);
                /*var scrPt = response.result.feature.geometry;
                var mpPt = response.result.feature.geometry;
                markColor = 'cc33ff';
                map.emit("click", {
                    bubbles: true,
                    cancelable: true,
                    screenPoint: scrPt,
                    mapPoint: mpPt
                });*/
            });
            map.centerAndZoom([-86.560476, 3.902196], 6);
        });



        function mapReady(map) {

            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(centerMap, locationError);
            } else {
                map.centerAndZoom([-86.560476, 3.902196], 6);
            }
        }

        function executeIdentifyTask(evt) {

            //document.getElementById("LATITUD_COOR").value = "";
            //document.getElementById("LONGITUD_COOR").value = "";
            //document.getElementById("MUNICIPIO_COOR").value = "";
            //document.getElementById("CODIGO_DANE_COOR").value = "";
            //document.getElementById("CODIGO_ESTACION_COOR").value = "";
            //document.getElementById("CUADRANTE_RURAL_COOR").value = "";
            //document.getElementById("BARRIO_COOR").value = "";
            //document.getElementById("DIRECCION_COOR").value = "";
            //document.getElementById("CUADRANTE_COOR").value = "";

            mapPoint = evt.mapPoint;
            latDelito = evt.mapPoint.getLatitude();
            lonDelito = evt.mapPoint.getLongitude();

            //Direccion
            locator.locationToAddress(mapPoint, 10, direccionObtenida, errorDireccionObtenida);

            //Cuadrantes AGOL
            var qtCuadrantes = new esri.tasks.QueryTask("https://services3.arcgis.com/8cBoM4o6pnuUb1z1/ArcGIS/rest/services/SIDENCO_SinMalla/FeatureServer/11");
            var qCuadrantes = new esri.tasks.Query();

            /* Definimos las tareas de consulta para cada una de las capas dell servicio en AGOL */

            //Layer Municipios
            var qtMunicipios = new esri.tasks.QueryTask("https://services3.arcgis.com/8cBoM4o6pnuUb1z1/ArcGIS/rest/services/SIDENCO_SinMalla/FeatureServer/3");
            var qMunicipios = new esri.tasks.Query();

            //Layer Jurisdiccion_Estaciones
            var qtJEstaciones = new esri.tasks.QueryTask("https://services3.arcgis.com/8cBoM4o6pnuUb1z1/ArcGIS/rest/services/SIDENCO_SinMalla/FeatureServer/9");
            var qJEstaciones = new esri.tasks.Query();

            //Layer Barrios
            var qtBarrios = new esri.tasks.QueryTask("https://services3.arcgis.com/8cBoM4o6pnuUb1z1/ArcGIS/rest/services/SIDENCO_SinMalla/FeatureServer/4");
            var qBarrios = new esri.tasks.Query();

            //Layer Cuadrantes Rurales
            var qtCRurales = new esri.tasks.QueryTask("https://services3.arcgis.com/8cBoM4o6pnuUb1z1/ArcGIS/rest/services/SIDENCO_SinMalla/FeatureServer/12");
            var qCRurales = new esri.tasks.Query();

            //qEstaciones.returnGeometry = false;
            qCuadrantes.returnGeometry = false;
            qMunicipios.returnGeometry = false;
            qJEstaciones.returnGeometry = false;
            qBarrios.returnGeometry = false;
            qCRurales.returnGeometry = false;


            qCuadrantes.outFields = ['CODIGO_SIEDCO', 'NRO_CUADRANTE']; //['CODIGO_SIEDCO'];
            qJEstaciones.outFields = ['CODIGO_SIEDCO'];
            qMunicipios.outFields = ['CODIGO', 'NOMBRE'];
            //qJEstaciones.outFields = ['CODIGO_SIEDCO'];
            qBarrios.outFields = ['NOMBRE'];
            qCRurales.outFields = ['SIEDCO'];

            var qGeom = new esri.geometry.Point({
                longitude: lonDelito,
                latitude: latDelito
            });

            qMunicipios.geometry = qGeom;
            qJEstaciones.geometry = qGeom;
            qBarrios.geometry = qGeom;
            qCRurales.geometry = qGeom;
            qCuadrantes.geometry = qGeom;
            //qEstaciones.geometry = qGeom;

            var municipio_ = qtMunicipios.execute(qMunicipios);
            var estacion_ = qtJEstaciones.execute(qJEstaciones);
            var barrio_ = qtBarrios.execute(qBarrios);
            var crural_ = qtCRurales.execute(qCRurales);
            var cuadrante_ = qtCuadrantes.execute(qCuadrantes);

            all([estacion_, cuadrante_, municipio_, barrio_, crural_]).then(function (results) {
                makeMarker(lonDelito, latDelito);
                //display a message so user knows something is happening
                //domAttr.set(dom.byId("enviarInformacionLink"), "innerHTML", "Buscando informaci\xf3n...");
                var codigoSiedco = "";
                if (results[0].hasOwnProperty("features")) {
                    if (results[0].features.length > 0) {
                        codigoSiedco = "Cod SIEDCO Estacion: " + results[0].features[0].attributes.CODIGO_SIEDCO;
                    } else if (results[1].hasOwnProperty("features")) {
                        if (results[1].features.length > 0) {
                            codigoSiedco = "Cod SIEDCO Cuadrante: " + results[1].features[0].attributes.CODIGO_SIEDCO;
                        }
                    }
                }
                //estacion_
                if (results[0].hasOwnProperty("features")) {
                    if (results[0].features.length > 0) {
                        cod_estacion = results[0].features[0].attributes.CODIGO_SIEDCO;
                    }
                }
                //cuadrante_
                if (results[1].hasOwnProperty("features")) {
                    if (results[1].features.length > 0) {
                        numCuadrante = results[1].features[0].attributes.CODIGO_SIEDCO;
                        numCuadrante1 = results[1].features[0].attributes.NRO_CUADRANTE;
                    }
                }

                //municipio_		
                if (results[2].hasOwnProperty("features")) {
                    if (results[2].features.length > 0) {
                        ent_administrativa = results[2].features[0].attributes.CODIGO;
                        nom_municipio = results[2].features[0].attributes.NOMBRE;
                    }
                }

                //barrio_		
                if (results[3].hasOwnProperty("features")) {
                    if (results[3].features.length > 0) {
                        barrio = results[3].features[0].attributes.NOMBRE;
                    }
                }

                //crural_
                if (results[4].hasOwnProperty("features")) {
                    if (results[4].features.length > 0) {
                        cua_rural = results[4].features[0].attributes.SIEDCO;
                    }
                }

                //Declaracion de la plantilla que usara dentro del popup para desplegar la informacion relacionada con los datos encontrados

                templateContent =
                    '<table class="tablePopup">' +
                    '<tr><td>Cuadrante</td><td>' + formatearTexto(numCuadrante1) + '</td></tr>' +
                    '<tr><td>Latitud</td><td>' + numberWithCommas(latDelito) + '</td></tr>' +
                    '<tr><td>Longitud</td><td>' + numberWithCommas(lonDelito) + '</td></tr>' +
                    '<tr><td>Nombre Municipio</td><td>' + formatearTexto(nom_municipio) + '</td></tr>' +
                    '<tr><td>Barrio</td><td>' + formatearTexto(barrio) + '</td></tr>' +
                    '<tr><td>Direccion</td><td>' + formatearTexto(strAddress) + '</td></tr>' +
                    '</table><br>' +
                    '<div class="row">' +
                    '<div class="col-md-12 text text-center">' +
                    '<button class="btn btn-success" href="#" onclick="CerrarModal()" style="text-align:center">Capturar</button>' +
                    '</div>' +
                    '</div>';

                window.map.infoWindow.clearFeatures();
                window.map.infoWindow.setTitle("Coordenadas");
                window.map.infoWindow.setContent(templateContent);
                window.map.infoWindow.show(evt.mapPoint, map.getInfoWindowAnchor(evt.screenPoint));

                if (latDelito != undefined || latDelito != null) {
                    document.getElementById("LATITUD_COOR").value = latDelito;
                }

                if (lonDelito != undefined || lonDelito != null) {
                    document.getElementById("LONGITUD_COOR").value = lonDelito;
                }

                if (nom_municipio != undefined || nom_municipio != null) {
                    document.getElementById("MUNICIPIO_COOR").value = nom_municipio;
                }

                if (ent_administrativa != undefined || ent_administrativa != null) {
                    document.getElementById("CODIGO_DANE_COOR").value = ent_administrativa;
                }

                if (cod_estacion != undefined || cod_estacion != null) {
                    document.getElementById("CODIGO_ESTACION_COOR").value = cod_estacion;
                }

                if (cua_rural != undefined || cua_rural != null) {
                    document.getElementById("CUADRANTE_RURAL_COOR").value = cua_rural;
                }

                if (barrio != undefined || barrio != null) {
                    document.getElementById("BARRIO_COOR").value = barrio;
                }

                if (strAddress != undefined || strAddress != null) {
                    document.getElementById("DIRECCION_COOR").value = strAddress;
                }

                if (numCuadrante1 != undefined || numCuadrante1 != null) {
                    document.getElementById("CUADRANTE_COOR").value = numCuadrante1;
                }

                console.log("Envio informacion exitosamente");

                numCuadrante = numCuadrante1 = ent_administrativa = barrio = cod_estacion = cod_cai = cua_rural = nom_municipio = strAddress = latDelito = lonDelito = null;
            });


        }
        //Function to show coordinates on map
        function showCoordinates(event) {
            mapCoor = esri.geometry.webMercatorToGeographic(event.mapPoint);
            $("#spnCoordinates").html('<h6 style="background-color:#064665; padding:5px" class="text-center text-white">' + "Latitud: " + mapCoor.y + " , Longitud: " + mapCoor.x + '</h6>');
        }


        //Function to convert decimal to degree
        function converCoor(coor) {
            var gg, mm, ss;
            if (coor < 0) {
                var absCoor = Math.abs(coor);
                gg = parseInt(absCoor, 10);
                mm = parseInt(((absCoor - gg) * 60), 10);
                ss = ((((absCoor - gg) * 60) - mm) * 60).toFixed(2);
                gg *= -1;
            } else {
                gg = parseInt(coor, 10);
                mm = parseInt(((coor - gg) * 60), 10);
                ss = ((((coor - gg) * 60) - mm) * 60).toFixed(2);
            }
            return (gg.toString() + '\xBA ' + mm.toString() + "\' " + ss.toString() + "\"");
        }

        function direccionObtenida(evt) {
            if (evt.address) {
                strAddress = evt.address.Address;
            }
            return strAddress;
        }

        function errorDireccionObtenida(evt) {
            //onButtonOKClick();
            return strAddress;
        }

        function numberWithCommas(x) {
            return x.toFixed(6);
        }

        //Function to obtain the coordenates from points
        function makeMarker(lon, lat) {

            //code to disegner 
            var markPath = "M21.021,16.349c-0.611-1.104-1.359-1.998-2.109-2.623c-0.875,0.641-1.941,1.031-3.103,1.031c-1.164,0-2.231-" +
                "0.391-3.105-1.031c-0.75,0.625-1.498,1.519-2.111,2.623c-1.422,2.563-1.578,5.192-0.35,5.874c0.55,0.307,1.127,0.078,1.723-" +
                "0.496c-0.105,0.582-0.166,1.213-0.166,1.873c0,2.932,1.139,5.307,2.543,5.307c0.846,0,1.265-" +
                "0.865,1.466-2.189c0.201,1.324,0.62,2.189,1.463,2.189c1.406,0,2.545-2.375,2.545-5.307c0-0.66-0.061-1.291-0.168-" +
                "1.873c0.598,0.574,1.174,0.803,1.725,0.496C22.602,21.541,22.443,18.912,21.021,16.349zM15.808,13.757c2.362,0,4.278-1.916,4.278-" +
                "4.279s-1.916-4.279-4.278-4.279c-2.363,0-4.28,1.916-4.28,4.279S13.445,13.757,15.808,13.757z";

            var point = new esri.geometry.Point({
                longitude: lon,
                latitude: lat
            });
            var mark = new esri.Graphic(point, createSymbol(markPath, markColor));
            map.graphics.add(mark);
            markColor = "0a9242";
        }

        //Function to create symbol on click
        function createSymbol(path, color) {
            map.graphics.clear();
            var markerSymbol = new esri.symbol.SimpleMarkerSymbol();
            markerSymbol.setPath(path);
            markerSymbol.setColor(new dojo.Color(color));
            markerSymbol.setSize(23);
            markerSymbol.setOutline(null);
            return markerSymbol;
        }

        function formatearTexto(t) {
            if (t == undefined || t == null) {
                t = 'Sin informacion';
            }
            return t;
        }

    });