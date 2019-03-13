
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

    fromComboBox.innerHTML = "<option selected hidden>Choose a origin stop...</option>";
    toComboBox.innerHTML = "<option selected hidden>Choose a destination stop...</option>";

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

                    var distToOrigin = 0.0;
                    var distToDestination = 0.0;

                    $.ajax({
                        url: "api/metra/distance?" +
                             "lat1=" + i.attributes.origin_lat.value +
                             "&lon1=" + i.attributes.origin_lon.value +
                             "&lat2=" + d.latitude +
                             "&lon2=" + d.longitude,
                        async: false,
                        success: function (dist) {
                            distToOrigin = dist;
                        }
                     });

                    $.ajax({
                        url : "api/metra/distance?" +
                              "lat1=" + i.attributes.destination_lat.value +
                              "&lon1=" + i.attributes.destination_lon.value +
                              "&lat2=" + d.latitude +
                              "&lon2=" + d.longitude,
                        async: false,
                        success: function (dist) {
                            distToDestination = dist;
                        }
                    });

                    i.setAttribute("data-original-title", d.label + "<br/>" + i.attributes.origin_name.value + ": " + distToOrigin + " miles<br/>" +
                        i.attributes.destination_name.value + ": " + distToDestination + " miles");
                }
            });
        });
    });

    var t = setTimeout(getPositions, 60000);
}
