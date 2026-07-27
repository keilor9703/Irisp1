
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
        "esri/geometry/Polyline",
        "esri/symbols/TextSymbol",
        "esri/symbols/Font",
        "dojo/domReady!",
        "esri/renderers/SimpleRenderer",
        "esri/layers/LabelClass"

    ], function (
        Map, Point, SimpleMarkerSymbol, Graphic, Color, Extent, FeatureLayer, SimpleLineSymbol, Query, SimpleFillSymbol,
        Polyline, TextSymbol, Font
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




        // ===========================================================
        // 🚶 RECORRIDO CRONOLÓGICO: dibuja la línea de desplazamiento del sujeto
        // conectando las consultas de antecedentes en orden temporal, con marca de
        // inicio (verde) y fin (rojo). Permite ver por dónde se ha movido y anticipar
        // su siguiente ubicación probable.
        // ===========================================================
        window.pintarRecorrido = function (lista) {
            if (!lista || lista.length === 0) { console.warn("Sin recorrido para pintar"); return; }

            map.graphics.clear();

            var puntosValidos = [];
            lista.forEach(function (c) {
                var lat = parseFloat(c.latitud), lng = parseFloat(c.longitud);
                if (isNaN(lat) || isNaN(lng) || lat === 0 || lng === 0) return;
                puntosValidos.push({ lat: lat, lng: lng, fechaStr: c.fechaStr, tipo: c.tipo });
            });
            if (puntosValidos.length === 0) { console.warn("Recorrido sin coordenadas válidas"); return; }

            // 1) Línea del recorrido (si hay 2+ puntos)
            if (puntosValidos.length > 1) {
                try {
                    var polyline = new Polyline(map.spatialReference);
                    polyline.addPath(puntosValidos.map(function (p) { return new Point(p.lng, p.lat); }));
                    var lineSymbol = new SimpleLineSymbol(
                        SimpleLineSymbol.STYLE_DASH, new Color([8, 102, 203, 0.9]), 3);
                    map.graphics.add(new Graphic(polyline, lineSymbol));
                } catch (err) { console.warn("No se pudo trazar la línea de recorrido:", err); }
            }

            // 2) Marcadores por punto + número de orden
            var xmin = 999, ymin = 999, xmax = -999, ymax = -999;
            puntosValidos.forEach(function (p, idx) {
                try {
                    var punto = new Point(p.lng, p.lat);
                    var color;
                    if (idx === 0) color = new Color("#0a9242");                    // inicio
                    else if (idx === puntosValidos.length - 1) color = new Color("#c53a1d"); // fin
                    else color = new Color("#08a6cb");                              // intermedios

                    var marker = new SimpleMarkerSymbol(SimpleMarkerSymbol.STYLE_CIRCLE, 14,
                        new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([255, 255, 255]), 1), color);
                    map.graphics.add(new Graphic(punto, marker));

                    // Número de orden encima del punto
                    try {
                        var etiqueta = new TextSymbol((idx + 1).toString());
                        etiqueta.setColor(new Color("#ffffff"));
                        etiqueta.setFont(new Font("9pt").setWeight(Font.WEIGHT_BOLD));
                        map.graphics.add(new Graphic(punto, etiqueta));
                    } catch (e) { /* etiqueta opcional */ }

                    if (p.lng < xmin) xmin = p.lng; if (p.lng > xmax) xmax = p.lng;
                    if (p.lat < ymin) ymin = p.lat; if (p.lat > ymax) ymax = p.lat;
                } catch (err) { console.warn("Punto de recorrido inválido:", p); }
            });

            // 3) Ajustar el zoom para incluir todo el recorrido
            try {
                if (xmax > xmin && ymax > ymin) {
                    map.setExtent(new Extent(xmin, ymin, xmax, ymax, map.spatialReference).expand(1.4));
                } else {
                    map.centerAndZoom(new Point(puntosValidos[0].lng, puntosValidos[0].lat), 13);
                }
            } catch (err) { console.warn("No se pudo ajustar el extent:", err); }

            console.log("🚶 Recorrido pintado con", puntosValidos.length, "puntos");
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