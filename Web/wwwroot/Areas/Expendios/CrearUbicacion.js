
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
            center: [-74.0721, 4.7110], // Bogotá
            zoom: 0,
            basemap: "osm" // OpenStreetMap
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

        function obtenerDireccionPorCoordenadas(latitud, longitud) {
            var url = `https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer/reverseGeocode?location=${longitud},${latitud}&f=json&outSR=4326`;

            fetch(url)
                .then(response => response.json())
                .then(data => {
                    if (data.address) {
                        var direccion = data.address.Match_addr || data.address.LongLabel || "Dirección no disponible";
                        var barrio = data.address.Neighborhood || data.address.District || "Barrio no disponible";
                        var ciudad = data.address.City || data.address.Municipality || "Ciudad no disponible";

                        // Actualiza las cajas de texto
                        $("#txtDireccionN").val(direccion);
                        $("#txtBarrioN").val(barrio);
                        $("#txtMunicipioN").val(ciudad);

                        console.log("Dirección:", direccion, "Barrio:", barrio, "Ciudad:", ciudad);
                    } else {
                        $("#txtDireccionN").val("No se pudo obtener la dirección");
                        $("#txtBarrioN").val("No disponible");
                        $("#txtMunicipioN").val("No disponible");
                    }
                })
                .catch(error => {
                    console.error("Error en geocodificación inversa:", error);
                    $("#txtDireccionN").val("Error al obtener dirección");

                    $("#txtBarrioN").val("Error");
                    $("#txtMunicipioN").val("Error");
                });
        }

        function actualizarMapa(extent) {
            if (extent) {
                map.setExtent(new Extent(extent));
            }
        }

        function municipiosExtent(code) {
            if (!code) {
                console.warn("Código de municipio inválido:", code);
                return;
            }

            console.log("Buscando municipio con código:", code);
            map.graphics.clear();
            var query = new Query();
            query.where = "CODIGO='" + code + "'";
            query.returnGeometry = true;

            mpioLayer.queryFeatures(query, function (featureSet) {
                if (!featureSet || featureSet.features.length === 0) {
                    console.warn("No se encontraron municipios con el código:", code);
                    return;
                }

                var geom = featureSet.features[0].geometry;
                var gra = new Graphic(geom, polygonHighlightSymbol);

                try {
                    var extent = geom.getExtent();
                    if (!extent) {
                        throw new Error("La geometría no tiene extensión válida.");
                    }

                    map.graphics.add(gra);
                    map.setExtent(extent.expand(1.5));
                } catch (error) {
                    console.error("Error al manejar la geometría:", error);
                }
            }, function (error) {
                console.error("Error en la consulta de municipios:", error);
            });
        }

        function mapReady() {
            if (typeof codigoDaneInicial !== "undefined" && codigoDaneInicial) {
                console.log("Código DANE inicial:", codigoDaneInicial);
                municipiosExtent(codigoDaneInicial);
            } else {
                console.warn("codigoDaneInicial no está definido o es inválido.");
            }
        }

        // Función para centrar el mapa en un municipio
        window.CentrarMapaPorMunicipio = function (_codDane) {
            if (!_codDane) {
                console.warn("Código DANE inválido en CentrarMapaPorMunicipio.");
                return;
            }
            console.log("Centrando mapa en municipio con código:", _codDane);
            municipiosExtent(_codDane);
        };

        map.on("load", mapReady);

        // --- Cambiado: Click en el mapa ahora pone el muñequito verde ---
        map.on("click", function (event) {
            var latitud = event.mapPoint.getLatitude();
            var longitud = event.mapPoint.getLongitude();

            $("#LATITUD_CASO").val(latitud).trigger("change");
            $("#LONGITUD_CASO").val(longitud).trigger("change");

            obtenerDireccionPorCoordenadas(latitud, longitud);

            var symbol = crearMunecoVerde();
            var pointGraphic = new Graphic(new Point(event.mapPoint.x, event.mapPoint.y, map.spatialReference), symbol);

            map.graphics.clear();
            map.graphics.add(pointGraphic);

            // Consulta espacial para obtener el cuadrante
            var query = new Query();
            query.geometry = event.mapPoint;

            obtenerDaneDesdeEstaciones(event.mapPoint);
            query.spatialRelationship = Query.SPATIAL_REL_INTERSECTS;
            query.returnGeometry = false;
            query.outFields = ["*"]; // ✅ Obtener todos los campos

            cuadrantesLayer.queryFeatures(query, function (featureSet) {
                if (featureSet.features.length > 0) {
                    var cuadrante = featureSet.features[0].attributes;

                    // ✅ Mostrar todos los campos disponibles
                    console.log("🔎 TODOS LOS CAMPOS DEL CUADRANTE:");
                    console.table(cuadrante);

                    // Usar algunos campos clave para mostrar en inputs
                    var nroCuadrante = cuadrante.NRO_CUADRANTE;
                    var CodSiedcoCuadrante = cuadrante.CODIGO_SIEDCO;
                    var CodIdCuadrante = cuadrante.CUAD_ID;

                    $("#txtCuadranteN").val(nroCuadrante);
                    $("#txtCodSiedcoCuadrante").val(CodSiedcoCuadrante);
                    $("#txtCodIdCuadrante").val(CodIdCuadrante);

                } else {
                    console.warn("⚠️ No se encontró cuadrante en el punto seleccionado.");
                    $("#txtCuadranteN").val("No encontrado");
                }
            }, function (error) {
                console.error("❌ Error al consultar cuadrante:", error);
            });
        });



        function obtenerDaneDesdeEstaciones(punto) {
            var query = new Query();
            query.geometry = punto;
            query.spatialRelationship = Query.SPATIAL_REL_INTERSECTS;
            query.returnGeometry = false;
            query.outFields = ["*"]; // ✅ Obtener todos los campos

            capaEstaciones.queryFeatures(query, function (featureSet) {
                if (featureSet.features.length > 0) {
                    var atributos = featureSet.features[0].attributes;


                    // console.log("🔎 TODOS LOS CAMPOS DE ESTACION:");
                    //console.table(atributos);
                    var CodDane = atributos.DANE;
                    var CodSiedcoEstacion = atributos.CODIGO_SIEDCO;
                    var CodEstacion = atributos.SIATH;

                    $("#txtCodDane").val(CodDane);
                    $("#txtCodSiedcoEstacion").val(CodSiedcoEstacion);
                    $("#txtCodEstacion").val(CodEstacion);

                } else {
                    console.warn("No se encontró municipio en el punto seleccionado.");
                    $("#txtCodDane").val("");
                }
            }, function (error) {
                console.error("Error al consultar municipio para DANE:", error);
            });
        }


        $("#direccionmapa").keypress(function (e) {
            if (e.which == 13) {
                buscarDireccion();
            }
        });

        // --- Cambiado: Punto de llamada es muñequito verde ---
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

            obtenerDireccionPorCoordenadas(latitud, longitud);

            console.log("Punto de llamada agregado exitosamente al mapa");
        }

        window.ubicarLlamadaEnMapa = function (latitud, longitud) {
            agregarPuntoLlamada(latitud, longitud);
        };

        // --- Cambiado: Punto manual es muñequito verde ---
        function agregarPuntoMapa(latitud, longitud) {
            if (!latitud || !longitud) return;

            var punto = new Point(longitud, latitud);
            var symbol = crearMunecoVerde();

            var pointGraphic = new Graphic(punto, symbol);
            map.graphics.clear();
            map.graphics.add(pointGraphic);
            map.centerAt(punto);

            obtenerDireccionPorCoordenadas(latitud, longitud);
        }

        function detectarCambioCoordenadas() {
            var latitud = parseFloat($("#LATITUD_CASO").val());
            var longitud = parseFloat($("#LONGITUD_CASO").val());
            if (!isNaN(latitud) && !isNaN(longitud)) {
                agregarPuntoMapa(latitud, longitud);
            }
        }

        $("#LATITUD_CASO, #LONGITUD_CASO").on("change", detectarCambioCoordenadas);

        function buscarDireccion() {
            var direccion = document.getElementById("direccionmapa").value;
            if (!direccion) return;

            var url = `https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer/findAddressCandidates?SingleLine=${encodeURIComponent(direccion)}&f=json&outFields=Match_addr,Addr_type,StAddr,City`;

            fetch(url)
                .then(response => response.json())
                .then(data => {
                    if (data.candidates.length > 0) {
                        var mejorCoincidencia = data.candidates[0];
                        var latitud = mejorCoincidencia.location.y;
                        var longitud = mejorCoincidencia.location.x;
                        $("#LATITUD_CASO").val(latitud).trigger("change");
                        $("#LONGITUD_CASO").val(longitud).trigger("change");
                        agregarPuntoMapa(latitud, longitud);
                    } else {
                        alert("No se encontró la dirección.");
                    }
                })
                .catch(error => console.error("Error en la búsqueda de dirección:", error));
        }

        // --- Agrega las capas al mapa ---
        map.addLayer(mpioLayer);
        map.addLayer(cuadrantesLayer); // <--- ¡Aquí agregas la capa de cuadrantes!
        map.addLayer(capaEstaciones);
        map.addLayer(capaBarrios);
        map.addLayer(capaRurales);





    })
};