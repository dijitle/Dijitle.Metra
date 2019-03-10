
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
 
    window.location.href = 'times?start=' + from +'&dest=' + to;
}

function loadRoutes() {
    var routeComboBox = document.getElementById('routes')

    routeComboBox.innerHTML = "<option selected value='-1'>All Routes</option>"

    $.get("api/metra/routes", function (data) {

        data.forEach(function (i) {
            routeComboBox.innerHTML += "<option value=" + i.id + ">" + i.longName + " (" + i.shortName + ")</option>";
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

            data.forEach(function (i) {
                fromComboBox.innerHTML += "<option value=" + i.id + ">" + i.name + "</option>";
                toComboBox.innerHTML += "<option value=" + i.id + ">" + i.name + "</option>";
            })
        });
    }
    else {
        $.get("api/metra/StopsByRoute?route=" + route + "&sortAsc=false", function (data) {

            data.forEach(function (i) {
                fromComboBox.innerHTML += "<option value=" + i.id + ">" + i.name + "</option>";
            })
        });

        $.get("api/metra/StopsByRoute?route=" + route + "&sortAsc=true", function (data) {

            data.forEach(function (i) {
                toComboBox.innerHTML += "<option value=" + i.id + ">" + i.name + "</option>";
            })
        });

    }

    
}
