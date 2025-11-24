
function inicializarMapa(idMapa) {
    require([
        "esri/map",
        "esri/geometry/Point",
        "esri/symbols/SimpleMarkerSymbol",
        "esri/graphic",
        "esri/Color",
        "esri/geometry/Extent",
        "esri/layers/FeatureLayer",
        "esri/symbols/SimpleLineSymbol",
        "esri/tasks/query",
        "esri/symbols/SimpleFillSymbol",
        "dojo/domReady!",
        "esri/renderers/SimpleRenderer",
        "esri/layers/LabelClass",
        "esri/symbols/TextSymbol",
        "esri/symbols/Font"

    ], function (
        Map, Point, SimpleMarkerSymbol, Graphic, Color, Extent, FeatureLayer, SimpleLineSymbol, Query, SimpleFillSymbol
    ) {



        var map = new Map(idMapa, {
            basemap: "osm",

            // Mejor centrado sobre Colombia
            extent: new Extent({
                xmin: -82, ymin: -5,
                xmax: -66, ymax: 13,
                spatialReference: { wkid: 4326 }
            })
        });



        // Capa de municipios
        var mpioLayer = new FeatureLayer("https://services3.arcgis.com/8cBoM4o6pnuUb1z1/ArcGIS/rest/services/SIDENCO_SinMalla/FeatureServer/3", {
            mode: FeatureLayer.MODE_ONDEMAND,
            outFields: ["*"]
        });

        // --- NUEVO: Capa de cuadrantes (líneas rojas) ---
        var cuadrantesLayer = new FeatureLayer(
            "https://services3.arcgis.com/8cBoM4o6pnuUb1z1/ArcGIS/rest/services/SIDENCO_SinMalla/FeatureServer/11", {
            mode: FeatureLayer.MODE_ONDEMAND,
            outFields: ["*"]
        }
        );

        var capaEstaciones = new FeatureLayer(
            "https://services3.arcgis.com/8cBoM4o6pnuUb1z1/ArcGIS/rest/services/SIDENCO_SinMalla/FeatureServer/9", {
            mode: FeatureLayer.MODE_ONDEMAND,
            outFields: ["*"]
        }
        );
         var capaBarrios = new FeatureLayer(
            "https://services3.arcgis.com/8cBoM4o6pnuUb1z1/ArcGIS/rest/services/SIDENCO_SinMalla/FeatureServer/4", {
            mode: FeatureLayer.MODE_ONDEMAND,
            outFields: ["*"]
        }
        );
         var capaRurales = new FeatureLayer(
            "https://services3.arcgis.com/8cBoM4o6pnuUb1z1/ArcGIS/rest/services/SIDENCO_SinMalla/FeatureServer/12", {
            mode: FeatureLayer.MODE_ONDEMAND,
            outFields: ["*"]
        }
        );



        // Símbolo de línea roja para los cuadrantes
        var cuadranteLineSymbol = new SimpleLineSymbol(
            SimpleLineSymbol.STYLE_SOLID,
            new Color([255, 0, 0]), // Rojo
            2 // Grosor de la línea
        );

        // Aplica el símbolo a la capa de cuadrantes
        cuadrantesLayer.setRenderer(new esri.renderer.SimpleRenderer(cuadranteLineSymbol));

        var polygonHighlightSymbol = new SimpleFillSymbol(
            SimpleFillSymbol.STYLE_SOLID,
            new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([255, 0, 0]), 1),
            new Color([125, 125, 125, 0.35])
        );

        // --- función para crear el muñequito verde ---
        function crearMunecoVerde() {
            var markPath = "M21.021,16.349c-0.611-1.104-1.359-1.998-2.109-2.623c-0.875,0.641-1.941,1.031-3.103,1.031c-1.164,0-2.231-0.391-3.105-1.031c-0.75,0.625-1.498,1.519-2.111,2.623c-1.422,2.563-1.578,5.192-0.35,5.874c0.55,0.307,1.127,0.078,1.723-0.496c-0.105,0.582-0.166,1.213-0.166,1.873c0,2.932,1.139,5.307,2.543,5.307c0.846,0,1.265-0.865,1.466-2.189c0.201,1.324,0.62,2.189,1.463,2.189c1.406,0,2.545-2.375,2.545-5.307c0-0.66-0.061-1.291-0.168-1.873c0.598,0.574,1.174,0.803,1.725,0.496C22.602,21.541,22.443,18.912,21.021,16.349zM15.808,13.757c2.362,0,4.278-1.916,4.278-4.279s-1.916-4.279-4.278-4.279c-2.363,0-4.28,1.916-4.28,4.279S13.445,13.757,15.808,13.757z";
            var symbol = new SimpleMarkerSymbol();
            symbol.setPath(markPath);
            symbol.setColor(new Color("#0a9242")); // Verde
            symbol.setSize(23);
            symbol.setOutline(null);
            return symbol;
        }

        

        // ===========================================================
        // 🔥 FUNCIÓN GLOBAL: PINTAR MÚLTIPLES UBICACIONES EN EL MAPA
        // ===========================================================
        window.pintarMultiplesUbicaciones = function (lista) {

            if (!lista || lista.length === 0) {
                console.warn("No hay coordenadas para pintar");
                return;
            }

            map.graphics.clear();

            let symbol = crearMunecoVerde();
            let extentBuilder = [];

            lista.forEach(coord => {
                try {
                    let punto = new Point(coord.longitud, coord.latitud);
                    let g = new Graphic(punto, symbol);

                    map.graphics.add(g);
                    extentBuilder.push(punto);
                } catch (err) {
                    console.warn("Coordenada inválida:", coord);
                }
            });

            if (extentBuilder.length > 0) {
                // Ajusta el zoom para incluir todos los puntos
                //let xmin = Math.min(...extentBuilder.map(p => p.x));
                //let xmax = Math.max(...extentBuilder.map(p => p.x));
                //let ymin = Math.min(...extentBuilder.map(p => p.y));
                //let ymax = Math.max(...extentBuilder.map(p => p.y));

                //let newExtent = new Extent(xmin, ymin, xmax, ymax, map.spatialReference);
                //map.setExtent(newExtent.expand(1.5));
            }

            console.log("🟢 Se pintaron", extentBuilder.length, "ubicaciones");
        };




        window.ubicarLlamadaEnMapa = function (latitud, longitud) {
            agregarPuntoLlamada(latitud, longitud);
        };


        function agregarPuntoLlamada(latitud, longitud) {
            if (!latitud || !longitud || latitud == "0" || longitud == "0") {
                console.log("Coordenadas de llamada no válidas:", latitud, longitud);
                return;
            }

            console.log("Ubicando llamada en el mapa - Lat:", latitud, "Lng:", longitud);

            var punto = new Point(parseFloat(longitud), parseFloat(latitud));
            var symbolLlamada = crearMunecoVerde();

            var pointGraphic = new Graphic(punto, symbolLlamada);

            map.graphics.clear();
            map.graphics.add(pointGraphic);

            map.centerAndZoom(punto, 15);

           // obtenerDireccionPorCoordenadas(latitud, longitud);

            console.log("Punto de llamada agregado exitosamente al mapa");
        }

        $("#direccionmapa").keypress(function (e) {
            if (e.which == 13) {
                buscarDireccion();
            }
        });

       

        // --- Agrega las capas al mapa ---
        map.addLayer(mpioLayer);
        map.addLayer(cuadrantesLayer); // <--- ¡Aquí agregas la capa de cuadrantes!
        map.addLayer(capaEstaciones);
        map.addLayer(capaBarrios);
        map.addLayer(capaRurales);





    })
};