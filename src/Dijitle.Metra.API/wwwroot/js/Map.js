var map;
var myView;

$(function () {
    $('div[onload]').trigger('onload');
});

function mapStart() {
    setupMap();
    loadAllRoutes();
    setTimeout(getAllPositions, 4815);
    setTimeout(getAllEstimatedPositions, 2342)
}

function setupMap() {


    var streetmapLayer = new ol.layer.Tile({
        source: new ol.source.OSM()
    });

    myView = new ol.View({
        center: ol.proj.transform([-88, 41.888], 'EPSG:4326', 'EPSG:3857'),
        zoom: 10
    });

    map = new ol.Map({
        layers: [streetmapLayer],
        view: myView
    });
}

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

async function loadAllRoutes() {
    $("#map").innerHTML = '<div class="spinner-grow text-primary"></div>';
    await sleep(1000);

    $("#map").innerHTML = "";

    map.setTarget("map");
    
    $.get("api/metra/routes", function (route) {
        route.forEach(function (r) {

            $("#routeLegend").append("<span class='my-3 mx-5 text-nowrap' style='width:50px'><i class='fas fa-route' style='font-size:24px;color:#" + r.routeColor + "'></i>" + r.shortName + "</span>");

            $.get("api/metra/shapesbyroute?route=" + r.id, function (data) {
                               
                data.forEach(function (s) {

                    var routeCoords = [];
                    var routeColor = s.color; 

                    s.points.forEach(function (p) {
                        routeCoords.push([p.lon, p.lat]);
                    });                

                    var transCoords = new ol.geom.LineString(routeCoords)

                    transCoords.transform('EPSG:4326', 'EPSG:3857');

                    var routeFeature = new ol.Feature({
                        geometry: transCoords
                    });

                    var routeStyle = new ol.style.Style({
                        stroke: new ol.style.Stroke({
                            color: '#' + routeColor,
                            width: 7
                        })
                    });

                    routeFeature.setStyle(routeStyle);

                    var routeLayerSource = new ol.source.Vector({
                        features: [routeFeature]
                    });

                    var routeLayer = new ol.layer.Vector({
                        name: "route" + r.id,
                        source: routeLayerSource
                    });

                    map.addLayer(routeLayer);
                });
            });
        });
    });
}

function getAllPositions() {

    $.get("api/gtfs/positions", function (data) {
        data.forEach(function (d) {


            var trainFeature = new ol.Feature({
                geometry: new ol.geom.Point(ol.proj.transform([Number(d.longitude), Number(d.latitude)], 'EPSG:4326', 'EPSG:3857'))
            });

            var color = "2772ea";

            if (d.direction) {
                color = "1df946";
            }

            var iconStyle = new ol.style.Style({
                image: new ol.style.Icon(({
                    anchor: [0.5, 1],
                    src: "https://cdn.mapmarker.io/api/v1/pin?icon=fa-train&size=75&background=" + color
                }))
            });

            trainFeature.setStyle(iconStyle);

            var trainLayerSource = new ol.source.Vector({
                features: [trainFeature]
            });

            var trainLayer = new ol.layer.Vector({
                name: "train" + d.id,
                source: trainLayerSource
            });

            removeLayer("train" + d.id);

            map.addLayer(trainLayer);
        });
    });
    
    setTimeout(getAllPositions, 23428);
}

function getAllEstimatedPositions() {
    $.get("api/metra/EstimatedPositions/all", function (data) {
        data.forEach(function (d) {


            var trainFeature = new ol.Feature({
                geometry: new ol.geom.Point(ol.proj.transform([Number(d.longitude), Number(d.latitude)], 'EPSG:4326', 'EPSG:3857'))
            });

            var color = "aaaaaa";

            if (d.direction) {
                color = "111111";
            }

            var iconStyle = new ol.style.Style({
                image: new ol.style.Icon(({
                    anchor: [0.5, 1],
                    src: "https://cdn.mapmarker.io/api/v1/pin?icon=fa-train&size=75&background=" + color
                }))
            });

            trainFeature.setStyle(iconStyle);

            var trainLayerSource = new ol.source.Vector({
                features: [trainFeature]
            });

            var trainLayer = new ol.layer.Vector({
                name: "train" + d.id,
                source: trainLayerSource
            });

            removeLayer("train" + d.id);

            map.addLayer(trainLayer);
        });
    });

    setTimeout(getAllEstimatedPositions, 15168);
}

async function moveMap(divId, shapeId) {

    if (map.getTarget() === divId) {
        return;
    }

    document.getElementById(divId).innerHTML = '<div class="spinner-grow text-primary"></div>';

    await sleep(1000);

    document.getElementById(divId).innerHTML = "";

    map.setTarget(divId);

    $.get("api/metra/shapesbyid?id=" + shapeId, function (data) {

        var routeColor = data.color;

        var routeCoords = [];

        data.points.forEach(function (p) {
            routeCoords.push([p.lon, p.lat]);
        });

        var transCoords = new ol.geom.LineString(routeCoords)

        transCoords.transform('EPSG:4326', 'EPSG:3857');

        var routeFeature = new ol.Feature({
            geometry: transCoords
        });

        var routeStyle = new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: '#' + routeColor,
                width: 7
            })
        });

        routeFeature.setStyle(routeStyle);

        var routeLayerSource = new ol.source.Vector({
            features: [routeFeature]
        });

        var routeLayer = new ol.layer.Vector({
            name: "route",
            source: routeLayerSource
        });

        removeLayer("train");
        removeLayer("stop");
        removeLayer("route");

        map.addLayer(routeLayer);

        myView.fit(transCoords, { padding: [20, 20, 20, 20], constrainResolution: false });
    });
}

function showStop(lat, lon) {

    var stopFeature = new ol.Feature({
        geometry: new ol.geom.Point(ol.proj.transform([Number(lon), Number(lat)], 'EPSG:4326', 'EPSG:3857'))
    });

    var iconStyle = new ol.style.Style({
        image: new ol.style.Icon(({
            anchor: [0.5, 1],
            src: "https://cdn.mapmarker.io/api/v1/pin?icon=fa-flag&size=75"
        }))
    });

    stopFeature.setStyle(iconStyle);

    var stopLayerSource = new ol.source.Vector({
        features: [stopFeature]
    });

    var stopLayer = new ol.layer.Vector({
        name: "stop",
        source: stopLayerSource
    });

    removeLayer("stop");

    map.addLayer(stopLayer);
}

function showTrain() {

    var mapDiv = $("#" + map.getTarget())[0];

    if (typeof mapDiv === 'undefined') {
        setTimeout(showTrain, 5000);
        return;
    }

    var lat = mapDiv.getAttribute("gpsTrainLat")
    var lon = mapDiv.getAttribute("gpsTrainLon")
    var dir = mapDiv.getAttribute("gpsTrainDir")

    var trainFeature = new ol.Feature({
        geometry: new ol.geom.Point(ol.proj.transform([Number(lon), Number(lat)], 'EPSG:4326', 'EPSG:3857'))
    });

    var color = "2772ea";

    if (dir) {
        color = "1df946";
    }

    var iconStyle = new ol.style.Style({
        image: new ol.style.Icon(({
            anchor: [0.5, 1],
            src: "https://cdn.mapmarker.io/api/v1/pin?icon=fa-train&size=75&background=" + color
        }))
    });

    trainFeature.setStyle(iconStyle);

    var trainLayerSource = new ol.source.Vector({
        features: [trainFeature]
    });

    var trainLayer = new ol.layer.Vector({
        name: "train",
        source: trainLayerSource
    });

    removeLayer("train");

    map.addLayer(trainLayer);

    setTimeout(showTrain, 5000);
}

function removeLayer(layer) {
    var removeLayer;
    map.getLayers().forEach(function (l) {
        if (l.get('name') != undefined && l.get('name') === layer) {

            removeLayer = l;
        }
    });

    map.removeLayer(removeLayer);
}