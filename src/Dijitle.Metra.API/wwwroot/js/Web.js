$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
});

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

        if (i.getAttribute('next_day') === 'next_day') {
            then += 1000 * 60 * 60 * 24
        }
        
        var diff = then - now;
        
        var diffHours = Math.floor(diff / (1000 * 60 * 60));
        diff -= diffHours * (1000 * 60 * 60);

        var diffMinutes = Math.floor(diff / (1000 * 60));
        diff -= diffMinutes * (1000 * 60);

        var diffSeconds = Math.floor(diff / (1000));
        diff -= diffSeconds * (1000);
        
        if (diffHours < 0) {
            i.innerHTML = "";
            i.setAttribute("style", "")
        }
        else if(diffHours < 1) {
            i.innerHTML = diffMinutes + ":" + checkTime(diffSeconds)

            if (diffMinutes < 1) {
                i.setAttribute("style", "font-color:red;")
            }
            else if (diffMinutes < 10) {
                i.setAttribute("style", "font-color:orange;")
            }
            else {
                i.setAttribute("style", "")
            }
        }
        else {
            i.innerHTML = diffHours + ":" + checkTime(diffMinutes) + ":" + checkTime(diffSeconds)
            i.setAttribute("style", "")
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

    var items = document.getElementsByName('progressBar');

    var today = new Date();
    var now = today.getTime();

    items.forEach(function (i) {

        var departTime = i.getAttribute('route_StartTime');
        departTime = today.setHours(departTime.split(':')[0], departTime.split(':')[1], 0, 0);

        if (i.getAttribute('route_StartNextDay') === 'route_StartNextDay') {
            departTime += 1000 * 60 * 60 * 24
        }

        var arriveTime = i.getAttribute('route_DestTime');
        arriveTime = today.setHours(arriveTime.split(':')[0], arriveTime.split(':')[1], 0, 0);

        if (i.getAttribute('route_DestNextDay') === 'route_DestNextDay') {
            arriveTime += 1000 * 60 * 60 * 24
        }

        if (departTime < now) {

            i.setAttribute("aria-valuenow", 0);
            i.setAttribute("style", "width: 0%;");
        }
        else if (now > arriveTime) {
            i.setAttribute("aria-valuenow", 100);
            i.setAttribute("style", "width: 100%;");
        }
    });

    $.get("api/gtfs/positions", function (data) {
        data.forEach(function (d) {
            items.forEach(function (i) {
                if (d.tripId === i.attributes.trip_id.value) {
                    var distTotal = getDistance(i.attributes.latStart.value, i.attributes.lonStart.value, i.attributes.latDest.value, i.attributes.lonDest.value);
                    var distTraveled = getDistance(i.attributes.latStart.value, i.attributes.lonStart.value, d.latitude, d.longitude);

                    i.setAttribute("aria-valuenow", (distTraveled / distTotal) * 100);
                    i.setAttribute("style", "width: " + (distTraveled / distTotal * 100) + "%;");
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

    if (document.getElementById('expressOnly').checked) {

        window.location.href = window.location.href.replace('expressOnly=false', 'expressOnly=true');
    }
    else {
        window.location.href = window.location.href.replace('expressOnly=true', 'expressOnly=false');
    }

    


}