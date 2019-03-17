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

        var depart = i.getAttribute('depart_time');

        depart = today.setHours(depart.split(':')[0], depart.split(':')[1], 0, 0);

        if (i.getAttribute('depart_next_day') === 'depart_next_day') {
            depart += 1000 * 60 * 60 * 24;
        }

        var arrive = i.getAttribute('arrive_time');

        arrive = today.setHours(arrive.split(':')[0], arrive.split(':')[1], 0, 0);

        if (i.getAttribute('arrive_next_day') === 'arrive_next_day') {
            arrive += 1000 * 60 * 60 * 24;
        }
        
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

        if (now < departTime) {

            i.setAttribute("aria-valuenow", 0);
            i.setAttribute("style", "height: 0%;");
        }
        else if (now > arriveTime) {
            i.setAttribute("aria-valuenow", 100);
            i.setAttribute("style", "height: 100%;");
        }
    });

    $.get("api/gtfs/positions", function (data) {
        data.forEach(function (d) {
            items.forEach(function (i) {
                if (d.tripId === i.attributes.trip_id.value) {
                    var distTotal = getDistance(i.attributes.latStart.value, i.attributes.lonStart.value, i.attributes.latDest.value, i.attributes.lonDest.value);
                    var distTraveled = getDistance(i.attributes.latStart.value, i.attributes.lonStart.value, d.latitude, d.longitude);

                    i.setAttribute("aria-valuenow", (distTraveled / distTotal) * 100);
                    i.setAttribute("style", "height: " + (distTraveled / distTotal * 100) + "%;");
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