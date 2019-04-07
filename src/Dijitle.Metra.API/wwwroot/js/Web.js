var map;
var myView;

$(document).ready(function () {
    $('[data-toggle="popover"]').popover();
});

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";Secure;" + expires + ";path=/";
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function start() {
    loadURL()
    loadRoutes();    
    startTime();
    setupMap();
    getPositions();
    showTrain();
    getAlerts();
}

function loadURL() {
    if (window.location.href.indexOf('?') === -1) {

        var start = getCookie("stopsFrom");
        var end = getCookie("stopsTo");
        var express = getCookie("express");

        if (start === "" || end === "" || express === "") {
            window.history.pushState("", "", '?start=ROUTE59&dest=CUS&expressOnly=false');
        }
        else {
            window.location.href = '?start=' + start + '&dest=' + end + '&expressOnly=' + express
        }
    }
}

function startTime() {
    var today = new Date();
    var h = today.getHours();
    var m = today.getMinutes();
    var s = today.getSeconds();
    h = checkTime(h);
    m = checkTime(m);
    s = checkTime(s);
    document.getElementById('clock').innerHTML =
        h + ":" + m + ":" + s;
    var items = document.getElementsByName('timeUntil')

    var now = today.getTime();

    items.forEach(function (i) {

        var time = new Date(i.getAttribute('time')); 
        
        var diff = time - now;
        
        var diffHours = Math.floor(diff  / (1000 * 60 * 60));
        diff -= diffHours * (1000 * 60 * 60);

        var diffMinutes = Math.floor(diff / (1000 * 60));
        diff -= diffMinutes * (1000 * 60);

        var diffSeconds = Math.floor(diff / (1000));
        diff -= diffSeconds * (1000);
        
        if (diffHours < 0) {
            i.innerHTML = "";
            i.setAttribute("class", "")
        }
        else if(diffHours < 1) {
            i.innerHTML = diffMinutes + ":" + checkTime(diffSeconds)

            if (diffMinutes < 1) {
                i.setAttribute("class", "text-danger")
            }
            else if (diffMinutes < 10) {
                i.setAttribute("class", "text-warning")
            }
            else {
                i.setAttribute("class", "")
            }
        }
        else {
            i.innerHTML = diffHours + ":" + checkTime(diffMinutes) + ":" + checkTime(diffSeconds)
            i.setAttribute("class", "")
        }
    });

    items = document.getElementsByName('timeUntilStop')
    
    items.forEach(function (i) {
        var arrive = new Date(i.getAttribute('arrive_time'));

        var diff = arrive - now;
        
        var diffHours = Math.floor(diff / (1000 * 60 * 60));
        diff -= diffHours * (1000 * 60 * 60);

        var diffMinutes = Math.floor(diff / (1000 * 60));
        diff -= diffMinutes * (1000 * 60);

        var diffSeconds = Math.floor(diff / (1000));
        diff -= diffSeconds * (1000);

        if (diffHours < 0) {
            i.innerHTML = "-";
            i.setAttribute("class", "text-success text-monospaced")
        }
        else if (diffHours < 1) {
            i.innerHTML = diffMinutes + ":" + checkTime(diffSeconds)

            if (diffMinutes < 1) {
                i.setAttribute("class", "text-danger text-monospaced")
            }
            else if (diffMinutes < 10) {
                i.setAttribute("class", "text-warning text-monospaced")
            }
            else {
                i.setAttribute("class", "text-monospaced")
            }
        }
        else {
            i.innerHTML = diffHours + ":" + checkTime(diffMinutes) + ":" + checkTime(diffSeconds)
            i.setAttribute("class", "text-monospaced")
        }
    });

    items = document.getElementsByName('stopdot')

    items.forEach(function (i) {
        var arrive = new Date(i.getAttribute('arrive_time'));

        var diff = arrive - now;

        var diffHours = Math.floor(diff / (1000 * 60 * 60));
        diff -= diffHours * (1000 * 60 * 60);

        var diffMinutes = Math.floor(diff / (1000 * 60));
        diff -= diffMinutes * (1000 * 60);

        var diffSeconds = Math.floor(diff / (1000));
        diff -= diffSeconds * (1000);

        if (diffHours < 0) {
            i.setAttribute("class", "fas fa-circle font-weight-bold")
        }        
        else {
            i.setAttribute("class", "fas fa-circle font-weight-lighter")
        }
    });

    var t = setTimeout(startTime, 500);
}

function checkTime(i) {
    if (i < 10) { i = "0" + i };  // add zero in front of numbers < 10
    return i;
}

function getTimes() {
    var from = $('#stopsFrom option:selected').val();
    var to = $('#stopsTo option:selected').val();
    var express = $('#expressOnly').is(':checked')

    save();
 
    window.location.href = '?start=' + from +'&dest=' + to + "&expressOnly=" + express;
}

function loadRoutes() {
    var routeComboBox = $('#routes');
    var expressCheck = $('#expressOnly');

    var cookieRoutes = getCookie("routes")

    expressCheck.prop("checked", window.location.href.indexOf("expressOnly=true") > -1);

    routeComboBox.empty().append(new Option("All Routes", "-1", true, false));

    $.get("api/metra/routes", function (data) {
        data.forEach(function (d) {
            routeComboBox.append(new Option(d.longName + " (" + d.shortName + ")", d.id, false, cookieRoutes === d.id));
        })

        loadStops();
    });
}

function loadStops() {
    var fromComboBox = $("#stopsFrom");
    var toComboBox = $("#stopsTo");
    var route = $("#routes option:selected").val();

    fromComboBox.empty().append(new Option("Choose a origin stop...", "ROUTE59", true, false));
    toComboBox.empty().append(new Option("Choose a destination stop...", "CUS", true, false));

    var cookieStopsFrom = getCookie("stopsFrom")
    var cookieStopsTo = getCookie("stopsTo")

    if (route === '-1') {
        $.get("api/metra/AllStops", function (data) {

            data.forEach(function (d) {
                fromComboBox.append(new Option(d.name, d.id, false, cookieStopsFrom === d.id));
                toComboBox.append(new Option(d.name, d.id, false, cookieStopsTo === d.id));
            });
        });
    }
    else {
        $.get("api/metra/StopsByRoute?route=" + route + "&sortAsc=false", function (data) {
            data.forEach(function (d) {
                fromComboBox.append(new Option(d.name, d.id, false, cookieStopsFrom === d.id));
            });
        });

        $.get("api/metra/StopsByRoute?route=" + route + "&sortAsc=true", function (data) {
            data.forEach(function (d) {
                toComboBox.append(new Option(d.name, d.id, false, cookieStopsTo === d.id));
            });
        });
    }
}

function save() {
    saveCombo("routes");
    saveCombo("stopsFrom");
    saveCombo("stopsTo");

    saveExpress();
}

function saveCombo(item) {
    setCookie(item, $('#' + item + ' option:selected').val(), 365); 
}

function saveExpress() {
    setCookie("express", $('#expressOnly').is(':checked'), 365);
}

function getAlerts() {
    $("[name^='alert']").each(function (i) {
        this.innerHTML = '';
    });

    $.get("api/gtfs/alerts", function (data) {
        data.forEach(function (d) {
            $("[name='alertIcon']").each(function (i) {
                if (d.affectedIds.indexOf(this.attributes.tripId.value) > -1) {
                    this.innerHTML = '<i class="fas fa-exclamation-triangle"</i>';
                }
            });

            $("[name='alertBody']").each(function (i) {
                if (d.affectedIds.indexOf(this.attributes.tripId.value) > -1) {
                    this.innerHTML = ' <h4>' + d.header + '</h4>' + d.description + '<br/><a target="_blank" href="' + d.url + '">Click here for more info</a><br/>';                    
                }
            });
        });
    });

    setTimeout(getAlerts, 60000);
}

function getPositions() {

    $.get("api/gtfs/positions", function (data) {
        data.forEach(function (d) {
            $("[name='map']").each(function (i) {
                if (d.tripId === this.attributes.tripId.value) {
                    this.setAttribute("gpsTrainLat", d.latitude);
                    this.setAttribute("gpsTrainLon", d.longitude);
                }
            });

            $("[name='distanceFromStation']").each(function (i) {
                if (d.tripId === this.attributes.tripId.value) {
                    var distStopToDest = getDistance(this.attributes.stopLat.value, this.attributes.stopLon.value, this.attributes.destLat.value, this.attributes.destLon.value);
                    var distTrainToStop = getDistance(this.attributes.stopLat.value, this.attributes.stopLon.value, d.latitude, d.longitude);
                    var distTrainToDest = getDistance(this.attributes.destLat.value, this.attributes.destLon.value, d.latitude, d.longitude);

                    if (distStopToDest < distTrainToDest) {
                        this.innerHTML = distTrainToStop + " miles";
                    }
                    else {
                        this.innerHTML = "";
                    }

                }
            });

        });
    });

    setTimeout(getPositions, 30000);
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

    var trainFeature = new ol.Feature({
        geometry: new ol.geom.Point(ol.proj.transform([Number(lon), Number(lat)], 'EPSG:4326', 'EPSG:3857'))
    });
    
    var iconStyle = new ol.style.Style({
        image: new ol.style.Icon(({
            anchor: [0.5, 1],
            src: "https://cdn.mapmarker.io/api/v1/pin?icon=fa-train&size=75&background=0000ff"
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

function getDistance(lat1, lon1, lat2, lon2) {
    var EARTH_RADIUS = 3959;

    var startLatRadians = getRadians(lat1);
    var destLatRadians = getRadians(lat2);
    var deltaLatRadians = getRadians(lat2 - lat1);
    var detlaLonRadians = getRadians(lon2 - lon1);

    var a = Math.sin(deltaLatRadians / 2) *
        Math.sin(deltaLatRadians / 2) +
        Math.cos(startLatRadians) *
        Math.cos(destLatRadians) *
        Math.sin(detlaLonRadians / 2) *
        Math.sin(detlaLonRadians / 2);

    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    return Math.round(EARTH_RADIUS * c * 100) / 100;
}

function getRadians(degrees) {
    return degrees * (Math.PI / 180)
}

function getLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(showPosition);
    } 
}

function showPosition(position) {
   alert( "Latitude: " + position.coords.latitude +
        "<br/>Longitude: " + position.coords.longitude);
}

function switchStops() {

    if (window.location.href.indexOf('?') > - 1) {
        var params = window.location.href.split('?')[1];

        var oldStart = params.split('&')[0].split('=')[1]
        var oldDest = params.split('&')[1].split('=')[1]
        var oldExpress = params.split('&')[2].split('=')[1]

        window.location.href = "?start=" + oldDest + "&dest=" + oldStart + "&expressOnly=" + oldExpress;
    }
    else {
        window.location.href = window.location.href + "?start=CUS&dest=ROUTE59&expressOnly=false";
    }

}

function changeExpress() {

    if (window.location.href.indexOf('?') === -1) {
        window.history.pushState("", "", '?start=ROUTE59&dest=CUS&expressOnly=false');
    }

    saveExpress();

    if (express = $('#expressOnly').is(':checked')) {
        
        window.location.href = window.location.href.replace('expressOnly=false', 'expressOnly=true');
    }
    else {
        window.location.href = window.location.href.replace('expressOnly=true', 'expressOnly=false');
    }
}