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

        var then = i.getAttribute('current-time');

        var then = today.setHours(then.split(':')[0], then.split(':')[1], 0, 0);
        
        var diff = then - now;
        
        var diffHours = Math.floor(diff / (1000 * 60 * 60));
        diff -= diffHours * (1000 * 60 * 60);

        var diffMinutes = Math.floor(diff / (1000 * 60));
        diff -= diffMinutes * (1000 * 60);

        var diffSeconds = Math.floor(diff / (1000));
        diff -= diffSeconds * (1000);
        
        if (diffHours < 0) {
            i.innerHTML = "";
        }
        else if(diffHours < 1) {
            i.innerHTML = diffMinutes + ":" + checkTime(diffSeconds)
        }
        else {
            i.innerHTML = diffHours + ":" + checkTime(diffMinutes) + ":" + checkTime(diffSeconds)
        }
    });

    var t = setTimeout(startTime, 500);
}
function checkTime(i) {
    if (i < 10) { i = "0" + i };  // add zero in front of numbers < 10
    return i;
}

function getTimes() {
    var from = document.getElementById('stopsFrom').selectedOptions[0].value;
    var to = document.getElementById('stopsTo').selectedOptions[0].value;
    var express = document.getElementById('expressOnly').checked
 
    window.location.href = 'times?start=' + from +'&dest=' + to + "&expressOnly=" + express;
}

function loadRoutes() {
    var routeComboBox = document.getElementById('routes')

    routeComboBox.innerHTML = "<option selected value='-1'>All Routes</option>"

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

    if (route === '-1') {
        $.get("api/metra/AllStops", function (data) {

            data.forEach(function (d) {
                fromComboBox.innerHTML += "<option value=" + d.id + ">" + d.name + "</option>";
                toComboBox.innerHTML += "<option value=" + d.id + ">" + d.name + "</option>";
            })
        });
    }
    else {
        $.get("api/metra/StopsByRoute?route=" + route + "&sortAsc=false", function (data) {

            data.forEach(function (d) {
                fromComboBox.innerHTML += "<option value=" + d.id + ">" + d.name + "</option>";
            })
        });

        $.get("api/metra/StopsByRoute?route=" + route + "&sortAsc=true", function (data) {

            data.forEach(function (d) {
                toComboBox.innerHTML += "<option value=" + d.id + ">" + d.name + "</option>";
            })
        });
    }
}

function getPositions() {

    var items = document.getElementsByName('gpsPositions');

    $.get("api/gtfs/positions", function (data) {
        data.forEach(function (d) {
            items.forEach(function (i) {
                if (d.tripId === i.attributes.trip_id.value) {
                    i.innerHTML = '<i class="fas fa-satellite-dish"></i>';

                    var dist = getDistance(i.attributes.lat.value, i.attributes.lon.value, d.latitude, d.longitude);

                    i.innerHTML += dist + " miles";
                }
            });
        });
    });

    var t = setTimeout(getPositions, 60000);
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
