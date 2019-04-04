var map;
var myView;

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
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
    loadRoutes();
    loadStops();
    startTime();
    setupMap();
    getPositions();
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

        var depart = new Date(i.getAttribute('depart_time')); 
        var arrive = new Date(i.getAttribute('arrive_time'));
        
        var diffDepart = depart - now;
        var diffArrive = arrive - now;

        var diff;
        var preText;

        if (diffDepart > 0) {
            diff = diffDepart;
            preText = "Departing: ";
        }
        else {
            diff = diffArrive;
            preText = "Arriving: ";
        }
        
        var diffHours = Math.floor(diff  / (1000 * 60 * 60));
        diff -= diffHours * (1000 * 60 * 60);

        var diffMinutes = Math.floor(diff / (1000 * 60));
        diff -= diffMinutes * (1000 * 60);

        var diffSeconds = Math.floor(diff / (1000));
        diff -= diffSeconds * (1000);
        
        if (diffHours < 0) {
            i.innerHTML = "Complete";
            i.setAttribute("class", "text-success")
        }
        else if(diffHours < 1) {
            i.innerHTML = preText + diffMinutes + ":" + checkTime(diffSeconds)

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
            i.innerHTML = preText + diffHours + ":" + checkTime(diffMinutes) + ":" + checkTime(diffSeconds)
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
 
    window.location.href = '?start=' + from +'&dest=' + to + "&expressOnly=" + express;
}

function loadRoutes() {
    var routeComboBox = document.getElementById('routes');
    var expressCheck = document.getElementById('expressOnly');

    expressCheck.checked = window.location.href.indexOf("expressOnly=true") > -1;

    routeComboBox.innerHTML = "<option selected value='-1'>All Routes</option>";

    $.get("api/metra/routes", function (data) {

        data.forEach(function (d) {
            routeComboBox.innerHTML += "<option value=" + d.id + ">" + d.longName + " (" + d.shortName + ")</option>";
        })
    });
}

function loadStops() {
    var fromComboBox = document.getElementById('stopsFrom');
    var toComboBox = document.getElementById('stopsTo');
    var route = document.getElementById('routes').selectedOptions[0].value;

    fromComboBox.innerHTML = "<option selected value='ROUTE59' hidden>Choose a origin stop...</option>";
    toComboBox.innerHTML = "<option selected value='CUS' hidden>Choose a destination stop...</option>";

    var cookieStopsFrom = getCookie("stopsFrom")
    var cookieStopsTo = getCookie("stopsTo")

    if (route === '-1') {
        $.get("api/metra/AllStops", function (data) {

            data.forEach(function (d) {

                var fromOption = document.createElement("OPTION");                
                if (cookieStopsFrom == d.id) {
                    fromOption.selected = true;
                }
                fromOption.setAttribute("value", d.id)
                fromOption.innerText = d.name;
                fromComboBox.appendChild(fromOption);


                var toOption = document.createElement("OPTION");
                if (cookieStopsTo == d.id) {
                    toOption.selected = true;
                }
                toOption.setAttribute("value", d.id)
                toOption.innerText = d.name;
                toComboBox.appendChild(toOption);
            })
        });
    }
    else {
        $.get("api/metra/StopsByRoute?route=" + route + "&sortAsc=false", function (data) {

            data.forEach(function (d) {
                var fromOption = document.createElement("OPTION");
                if (cookieStopsFrom == d.id) {
                    fromOption.selected = true;
                }
                fromOption.setAttribute("value", d.id)
                fromOption.innerText = d.name;
                fromComboBox.appendChild(fromOption);
            })
        });

        $.get("api/metra/StopsByRoute?route=" + route + "&sortAsc=true", function (data) {

            data.forEach(function (d) {
                var toOption = document.createElement("OPTION");
                if (cookieStopsTo == d.id) {
                    toOption.selected = true;
                }
                toOption.setAttribute("value", d.id)
                toOption.innerText = d.name;
                toComboBox.appendChild(toOption);
            })
        });
    }
}


function save(item) {
    setCookie(item, document.getElementById(item).selectedOptions[0].value, 365); 
}

function getPositions() {

    $.get("api/gtfs/positions", function (data) {
        data.forEach(function (d) {
            maps.forEach(function (i) {
                if (d.tripId === i.attributes.trip_id.value) {
                    var distTotal = getDistance(i.attributes.latStart.value, i.attributes.lonStart.value, i.attributes.latDest.value, i.attributes.lonDest.value);
                    var distTraveled = getDistance(i.attributes.latStart.value, i.attributes.lonStart.value, d.latitude, d.longitude);
                                        
                    i.innerHTML =  (distTraveled / distTotal * 100) + "%;";
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

        var removeLayer;
        map.getLayers().forEach(function (l) {
            if (l.get('name') != undefined && l.get('name') === "route") {

            removeLayer = l;
            }
        });

        map.removeLayer(removeLayer);
        map.addLayer(routeLayer);

        myView.fit(transCoords, { padding: [20, 20, 20, 20], constrainResolution: false });
    });
}

function showPoint(lat, lon) {

    var stopFeature = new ol.Feature({
        geometry: new ol.geom.Point(ol.proj.transform([Number(lon), Number(lat)], 'EPSG:4326', 'EPSG:3857'))
    });

    var stroke = new ol.style.Stroke({ color: 'white', width: 3 });
    var goldFill = new ol.style.Fill({ color: 'red' });

    var squareStyle = new ol.style.Style({
        image: new ol.style.RegularShape({
            fill: goldFill,
            stroke: stroke,
            points: 8,
            radius: 10,
            angle: Math.PI / 8
        })
    });

    stopFeature.setStyle(squareStyle);
    
    var stopLayerSource = new ol.source.Vector({
        features: [stopFeature]
    });
    
    var stopLayer = new ol.layer.Vector({
        name: "stop",
        source: stopLayerSource
    });

    var removeLayer;
    map.getLayers().forEach(function (l) {
        if (l.get('name') != undefined && l.get('name') === "stop") {

            removeLayer = l;
        }
    });

    map.removeLayer(removeLayer);
    map.addLayer(stopLayer);
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
        "<br>Longitude: " + position.coords.longitude);
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

    if (express = $('#expressOnly').is(':checked')) {
        
        window.location.href = window.location.href.replace('expressOnly=false', 'expressOnly=true');
    }
    else {
        window.location.href = window.location.href.replace('expressOnly=true', 'expressOnly=false');
    }

    


}