
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
        "esri/geometry/webMercatorUtils",
        "dojo/domReady!",
        "esri/renderers/SimpleRenderer",
        "esri/layers/LabelClass"

    ], function (
        Map, Point, SimpleMarkerSymbol, Graphic, Color, Extent, FeatureLayer, SimpleLineSymbol, Query, SimpleFillSymbol,
        Polyline, TextSymbol, Font, webMercatorUtils
    ) {

        // El basemap OSM está en Web Mercator (metros); las coordenadas de la BD son geográficas
        // (grados lat/long). Sin convertir, un punto (-74, 4) se interpreta como metros y cae en el
        // océano (~0,0). Este helper devuelve el punto ya proyectado al SR del mapa.
        function aPuntoMapa(lng, lat) {
            return webMercatorUtils.geographicToWebMercator(new Point(lng, lat));
        }



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
                    let punto = aPuntoMapa(parseFloat(coord.longitud), parseFloat(coord.latitud));
                    let g = new Graphic(punto, symbol);

                    map.graphics.add(g);
                    extentBuilder.push(punto);
                } catch (err) {
                    console.warn("Coordenada inválida:", coord);
                }
            });

            if (extentBuilder.length > 0) {
                try {
                    if (extentBuilder.length === 1) {
                        map.centerAndZoom(extentBuilder[0], 14);
                    } else {
                        let xs = extentBuilder.map(p => p.x), ys = extentBuilder.map(p => p.y);
                        map.setExtent(new Extent(
                            Math.min(...xs), Math.min(...ys), Math.max(...xs), Math.max(...ys),
                            map.spatialReference).expand(1.4));
                    }
                } catch (e) { console.warn("No se pudo ajustar el extent:", e); }
            }

            console.log("🟢 Se pintaron", extentBuilder.length, "ubicaciones");
        };




        // -------- Helpers de dibujo del recorrido (nodo numerado, cluster y despliegue) --------
        var C_INICIO = "#0a9242", C_FIN = "#c53a1d", C_MEDIO = "#08a6cb", C_CLUSTER = "#6f42c1";

        function etiquetaNumero(texto) {
            var t = new TextSymbol(texto);
            t.setColor(new Color("#ffffff"));
            t.setFont(new Font("12pt").setWeight(Font.WEIGHT_BOLD));
            if (t.setHaloColor) { t.setHaloColor(new Color([0, 0, 0, 0.65])); }
            if (t.setHaloSize) { t.setHaloSize(1); }
            if (t.setVerticalAlignment) { t.setVerticalAlignment("middle"); }
            if (t.setHorizontalAlignment) { t.setHorizontalAlignment("middle"); }
            return t;
        }

        // Clusters del recorrido actual (para desplegar/replegar por proximidad al clic del mapa).
        var _clusters = [];

        // Dibuja un nodo (círculo + número). Inicio verde y fin rojo, ambos más grandes.
        // Si se pasa "col", los gráficos creados se agregan a ese arreglo (para poder quitarlos).
        function dibujarNodo(mapPoint, orden, total, col) {
            var color, tam;
            if (orden === 1) { color = new Color(C_INICIO); tam = 34; }
            else if (orden === total) { color = new Color(C_FIN); tam = 34; }
            else { color = new Color(C_MEDIO); tam = 26; }

            var marker = new SimpleMarkerSymbol(SimpleMarkerSymbol.STYLE_CIRCLE, tam,
                new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([255, 255, 255]), 2), color);
            var gMarker = new Graphic(mapPoint, marker);
            var gLabel = new Graphic(mapPoint, etiquetaNumero(orden.toString()));
            map.graphics.add(gMarker);
            map.graphics.add(gLabel);
            if (col) { col.push(gMarker, gLabel); }
        }

        // Dibuja un marcador de agrupación (×N) y registra el cluster para el toggle por proximidad.
        function dibujarCluster(centerMapPoint, miembros) {
            var n = miembros.length;
            var tam = Math.min(48, 28 + n * 2);
            var marker = new SimpleMarkerSymbol(SimpleMarkerSymbol.STYLE_CIRCLE, tam,
                new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([255, 255, 255]), 2), new Color(C_CLUSTER));
            map.graphics.add(new Graphic(centerMapPoint, marker));
            map.graphics.add(new Graphic(centerMapPoint, etiquetaNumero(n.toString())));
            _clusters.push({ cx: centerMapPoint.x, cy: centerMapPoint.y, miembros: miembros, expandido: false, graficos: [] });
        }

        // Despliega en abanico los nodos de un cluster alrededor de su ubicación real, con una
        // línea punteada del centro a cada nodo. El radio es proporcional al zoom actual.
        function expandirCluster(cl) {
            var R;
            try { R = map.extent.getWidth() * 0.02; } catch (e) { R = 60; }
            var ordenados = cl.miembros.slice().sort(function (a, b) { return a.orden - b.orden; });
            ordenados.forEach(function (m, j) {
                var base = aPuntoMapa(m.lng, m.lat);
                var ang = (2 * Math.PI * j) / ordenados.length - Math.PI / 2;
                var off = new Point(base.x + R * Math.cos(ang), base.y + R * Math.sin(ang), map.spatialReference);
                try {
                    var linea = new Polyline(map.spatialReference);
                    linea.addPath([base, off]);
                    var gLinea = new Graphic(linea,
                        new SimpleLineSymbol(SimpleLineSymbol.STYLE_DOT, new Color([111, 66, 193, 0.9]), 1));
                    map.graphics.add(gLinea);
                    cl.graficos.push(gLinea);
                } catch (e) { /* conector opcional */ }
                dibujarNodo(off, m.orden, m.total, cl.graficos);
            });
            cl.expandido = true;
        }

        // Repliega un cluster: quita del mapa los gráficos agregados al desplegarlo.
        function colapsarCluster(cl) {
            cl.graficos.forEach(function (g) { try { map.graphics.remove(g); } catch (e) { } });
            cl.graficos = [];
            cl.expandido = false;
        }

        function toggleCluster(cl) {
            if (cl.expandido) { colapsarCluster(cl); } else { expandirCluster(cl); }
        }

        // ===========================================================
        // 🚶 RECORRIDO CRONOLÓGICO: dibuja la línea de desplazamiento del sujeto
        // conectando las consultas de antecedentes en orden temporal, con marca de
        // inicio (verde) y fin (rojo). Los puntos co-localizados se agrupan en un
        // marcador ×N que se despliega en abanico al hacer clic. Permite ver por
        // dónde se ha movido y anticipar su siguiente ubicación probable.
        // ===========================================================
        window.pintarRecorrido = function (lista) {
            if (!lista || lista.length === 0) { console.warn("Sin recorrido para pintar"); return; }

            map.graphics.clear();
            _clusters = [];

            var puntosValidos = [];
            lista.forEach(function (c) {
                var lat = parseFloat(c.latitud), lng = parseFloat(c.longitud);
                if (isNaN(lat) || isNaN(lng) || lat === 0 || lng === 0) return;
                puntosValidos.push({ lat: lat, lng: lng, fechaStr: c.fechaStr, tipo: c.tipo });
            });
            if (puntosValidos.length === 0) { console.warn("Recorrido sin coordenadas válidas"); return; }

            // Convertir todo a Web Mercator (SR del basemap OSM) antes de dibujar.
            var puntosMapa = puntosValidos.map(function (p) { return aPuntoMapa(p.lng, p.lat); });

            // 1) Línea del recorrido (si hay 2+ puntos)
            if (puntosMapa.length > 1) {
                try {
                    var polyline = new Polyline(map.spatialReference);
                    polyline.addPath(puntosMapa);
                    var lineSymbol = new SimpleLineSymbol(
                        SimpleLineSymbol.STYLE_DASH, new Color([8, 102, 203, 0.9]), 3);
                    map.graphics.add(new Graphic(polyline, lineSymbol));
                } catch (err) { console.warn("No se pudo trazar la línea de recorrido:", err); }
            }

            // 2) Extent del recorrido (coordenadas ya en Web Mercator)
            var xmin = Infinity, ymin = Infinity, xmax = -Infinity, ymax = -Infinity;
            puntosMapa.forEach(function (mp) {
                if (mp.x < xmin) xmin = mp.x; if (mp.x > xmax) xmax = mp.x;
                if (mp.y < ymin) ymin = mp.y; if (mp.y > ymax) ymax = mp.y;
            });

            // 3) Agrupar puntos co-localizados (misma celda ~11 m). Los que caen en el mismo
            //    sitio se colapsan en un marcador ×N (se despliega en abanico al hacer clic);
            //    los únicos se dibujan como nodo numerado normal. Así los números dejan de
            //    amontonarse cuando hay muchas consultas en la misma ubicación.
            var total = puntosValidos.length;
            var GRID = 0.0004; // ~11 m
            var grupos = {};
            puntosValidos.forEach(function (p, idx) {
                var key = Math.round(p.lat / GRID) + "_" + Math.round(p.lng / GRID);
                (grupos[key] = grupos[key] || []).push({ lng: p.lng, lat: p.lat, orden: idx + 1, total: total });
            });
            Object.keys(grupos).forEach(function (key) {
                var g = grupos[key];
                var centro = aPuntoMapa(g[0].lng, g[0].lat);
                if (g.length === 1) { dibujarNodo(centro, g[0].orden, total); }
                else { dibujarCluster(centro, g); }
            });

            // 4) Ajustar el zoom para incluir todo el recorrido (coordenadas ya en Web Mercator)
            try {
                if (xmax > xmin && ymax > ymin) {
                    map.setExtent(new Extent(xmin, ymin, xmax, ymax, map.spatialReference).expand(1.4));
                } else {
                    map.centerAndZoom(puntosMapa[0], 13);
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

            var punto = aPuntoMapa(parseFloat(longitud), parseFloat(latitud));
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

        // Clic en el mapa: si cae sobre un marcador de agrupación (×N), lo despliega o repliega.
        // Se usa prueba de proximidad contra el centro del cluster (independiente del enrutamiento
        // de clics sobre gráficos, que las capas de features suelen interceptar).
        try {
            map.on("click", function (evt) {
                try {
                    if (!_clusters.length || !evt || !evt.mapPoint) return;
                    var upp = (map.extent.getWidth() / map.width) || 1; // unidades de mapa por pixel
                    var umbral = 26 * upp;                               // ~26 px de tolerancia
                    var mejor = null, mejorD = Infinity;
                    _clusters.forEach(function (c) {
                        var d = Math.sqrt(Math.pow(evt.mapPoint.x - c.cx, 2) + Math.pow(evt.mapPoint.y - c.cy, 2));
                        if (d <= umbral && d < mejorD) { mejorD = d; mejor = c; }
                    });
                    if (mejor) { toggleCluster(mejor); }
                } catch (e) { console.warn("No se pudo procesar el clic del recorrido:", e); }
            });
        } catch (e) { console.warn("No se pudo enlazar el clic del mapa:", e); }





    })
};