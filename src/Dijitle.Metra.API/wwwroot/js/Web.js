$(document).ready(function() {
    $('[data-toggle="popover"]').popover();
    loadURL();
    loadRoutes();
    startTime();
    setupMap();
    getPositions();
    showTrain();
    getAlerts();
});

var allStops;

function setCookie(cname, cvalue, exdays) {
  var d = new Date();
  d.setTime(d.getTime() + exdays * 24 * 60 * 60 * 1000);
  var expires = "expires=" + d.toUTCString();
  document.cookie = cname + "=" + cvalue + ";Secure;" + expires + ";path=/";
}

function getCookie(cname) {
  var name = cname + "=";
  var ca = document.cookie.split(";");
  for (var i = 0; i < ca.length; i++) {
    var c = ca[i];
    while (c.charAt(0) == " ") {
      c = c.substring(1);
    }
    if (c.indexOf(name) == 0) {
      return c.substring(name.length, c.length);
    }
  }
  return "";
}

function loadURL() {
  if (window.location.href.indexOf("?") === -1) {
    var start = getCookie("stopsFrom");
    var end = getCookie("stopsTo");
    var express = getCookie("express");

    if (start === "" || end === "" || express === "") {
      window.history.pushState(
        "",
        "",
        "/Trips?start=ROUTE59&dest=CUS&expressOnly=false"
      );
    } else {
      window.location.href =
        "/Trips?start=" + start + "&dest=" + end + "&expressOnly=" + express;
    }
  }
}

function datePicked() {
    Console.log($('#selectDate').value);
}

function startTime() {
  var today = new Date();
  var h = today.getHours();
  var m = today.getMinutes();
  var s = today.getSeconds();
  h = checkTime(h);
  m = checkTime(m);
  s = checkTime(s);
  document.getElementById("clock").innerHTML = h + ":" + m + ":" + s;
  var items = document.getElementsByName("timeUntil");

  var now = today.getTime();

  items.forEach(function(i) {
    var time = new Date(i.getAttribute("time"));
    var timeDuration = i.getAttribute("timeDuration");

    var diff = time - now;

    var diffHours = Math.floor(diff / (1000 * 60 * 60));
    diff -= diffHours * (1000 * 60 * 60);

    var diffMinutes = Math.floor(diff / (1000 * 60));
    diff -= diffMinutes * (1000 * 60);

    var diffSeconds = Math.floor(diff / 1000);
    diff -= diffSeconds * 1000;

    if (diffHours < 0) {
      i.innerHTML = "";
      i.setAttribute("class", "");
    } else if (timeDuration !== "-1") {
      i.innerHTML = timeDuration + " min";
      i.setAttribute("class", "");
    } else if (diffHours < 1) {
      i.innerHTML = diffMinutes + ":" + checkTime(diffSeconds);

      if (diffMinutes < 1) {
        i.setAttribute("class", "text-danger");
      } else if (diffMinutes < 10) {
        i.setAttribute("class", "text-warning");
      } else {
        i.setAttribute("class", "");
      }
    } else {
      i.innerHTML =
        diffHours + ":" + checkTime(diffMinutes) + ":" + checkTime(diffSeconds);
      i.setAttribute("class", "");
    }
  });

  items = document.getElementsByName("timeUntilStop");

  items.forEach(function(i) {
    var arrive = new Date(i.getAttribute("arrive_time"));

    var diff = arrive - now;

    var diffHours = Math.floor(diff / (1000 * 60 * 60));
    diff -= diffHours * (1000 * 60 * 60);

    var diffMinutes = Math.floor(diff / (1000 * 60));
    diff -= diffMinutes * (1000 * 60);

    var diffSeconds = Math.floor(diff / 1000);
    diff -= diffSeconds * 1000;

    if (diffHours < 0) {
      i.innerHTML = "-";
      i.setAttribute("class", "text-success text-monospaced");
    } else if (diffHours < 1) {
      i.innerHTML = diffMinutes + ":" + checkTime(diffSeconds);

      if (diffMinutes < 1) {
        i.setAttribute("class", "text-danger text-monospaced");
      } else if (diffMinutes < 10) {
        i.setAttribute("class", "text-warning text-monospaced");
      } else {
        i.setAttribute("class", "text-monospaced");
      }
    } else {
      i.innerHTML =
        diffHours + ":" + checkTime(diffMinutes) + ":" + checkTime(diffSeconds);
      i.setAttribute("class", "text-monospaced");
    }
  });

  items = document.getElementsByName("stopdot");

  items.forEach(function(i) {
    var arrive = new Date(i.getAttribute("arrive_time"));

    var diff = arrive - now;

    var diffHours = Math.floor(diff / (1000 * 60 * 60));
    diff -= diffHours * (1000 * 60 * 60);

    var diffMinutes = Math.floor(diff / (1000 * 60));
    diff -= diffMinutes * (1000 * 60);

    var diffSeconds = Math.floor(diff / 1000);
    diff -= diffSeconds * 1000;

    if (diffHours < 0) {
      i.setAttribute("class", "fas fa-circle font-weight-bold");
    } else {
      i.setAttribute("class", "fas fa-circle font-weight-lighter");
    }
  });

  var t = setTimeout(startTime, 500);
}

function checkTime(i) {
  if (i < 10) {
    i = "0" + i;
  } // add zero in front of numbers < 10
  return i;
}

function getTimes() {
  var from = $("#stopsFrom option:selected").val();
  var to = $("#stopsTo option:selected").val();
  var express = $("#expressOnly").is(":checked");

  save();

  window.location.href =
    "?start=" + from + "&dest=" + to + "&expressOnly=" + express;
}

function loadRoutes() {
  var routeComboBox = $("#routes");
  var expressCheck = $("#expressOnly");

  var cookieRoutes = getCookie("routes");

  expressCheck.prop(
    "checked",
    window.location.href.indexOf("expressOnly=true") > -1
  );

  routeComboBox.empty().append(new Option("All Routes", "-1", true, false));

  $.get("api/metra/AllStops", function(data) {
    allStops = data;

    $.get("api/metra/routes", function(data) {
      data.forEach(function(d) {
        routeComboBox.append(
          new Option(
            d.shortName + " [" + d.longName + "]",
            d.id,
            false,
            cookieRoutes === d.id
          )
        );
      });

      loadStops();
    });

    loadHistory();
  });
}

function loadStops() {
  var fromComboBox = $("#stopsFrom");
  var toComboBox = $("#stopsTo");
  var route = $("#routes option:selected").val();

  fromComboBox
    .empty()
    .append(new Option("Choose a origin stop...", "ROUTE59", true, false));
  toComboBox
    .empty()
    .append(new Option("Choose a destination stop...", "CUS", true, false));

  var cookieStopsFrom = getCookie("stopsFrom");
  var cookieStopsTo = getCookie("stopsTo");

  if (route === "-1") {
    allStops.forEach(function(d) {
      fromComboBox.append(
        new Option(d.name, d.id, false, cookieStopsFrom === d.id)
      );
      toComboBox.append(
        new Option(d.name, d.id, false, cookieStopsTo === d.id)
      );
    });
  } else {
    var data = [];

    allStops.forEach(function(s) {
      if (s.routes.includes(route)) {
        data.push(s);
      }
    });

    data.sort((a, b) => (a.distanceAway > b.distanceAway ? 1 : -1));

    data.reverse().forEach(function(d) {
      fromComboBox.append(
        new Option(d.name, d.id, false, cookieStopsFrom === d.id)
      );
    });

    data.reverse().forEach(function(d) {
      toComboBox.append(
        new Option(d.name, d.id, false, cookieStopsTo === d.id)
      );
    });
  }
}

function filterStops() {}

function loadHistory() {
  var hist = getCookie("history");
  var historyComboBox = $("#history");

  hist.split(",").forEach(function(h) {
    if (h.length > 0) {
      var from = allStops.find(s => s.id === h.split("-")[0]);
      var to = allStops.find(s => s.id === h.split("-")[1]);
      var express = h.endsWith("-X") ? " Express" : "";

      historyComboBox.append(
        new Option(from.name + " to " + to.name + express, h, false, false)
      );
    }
  });
}

function goToHistory() {
  var hist = $("#history option:selected").val();

  var from = hist.split("-")[0];
  var to = hist.split("-")[1];
  var express = hist.endsWith("-X");

  window.location.href =
    "?start=" + from + "&dest=" + to + "&expressOnly=" + express;
}

function save() {
  saveCombo("routes");
  saveCombo("stopsFrom");
  saveCombo("stopsTo");

  saveExpress();
  saveHistory();
}

function saveCombo(item) {
  setCookie(item, $("#" + item + " option:selected").val(), 365);
}

function saveExpress() {
  setCookie("express", $("#expressOnly").is(":checked"), 365);
}

function saveHistory() {
  var hist = getCookie("history");
  var item =
    $("#stopsFrom option:selected").val() +
    "-" +
    $("#stopsTo option:selected").val() +
    ($("#expressOnly").is(":checked") ? "-X" : "");

  if (!hist.includes(item)) {
    hist = item + "," + hist;
  }

  setCookie("history", hist, 365);
}

function getAlerts() {
  $("[name^='alert']").each(function(i) {
    this.innerHTML = "";
  });

  $.get("api/gtfs/alerts", function(data) {
    data.forEach(function(d) {
      $("[name='alertIcon']").each(function(i) {
        if (d.affectedIds.indexOf(this.attributes.tripId.value) > -1) {
          this.innerHTML = '<i class="fas fa-exclamation-triangle"</i>';
        }
      });

      $("[name='alertBody']").each(function(i) {
        if (d.affectedIds.indexOf(this.attributes.tripId.value) > -1) {
          this.innerHTML =
            " <h4>" +
            d.header +
            "</h4>" +
            d.description +
            '<br/><a target="_blank" href="' +
            d.url +
            '">Click here for more info</a><br/>';
        }
      });
    });
  });

  setTimeout(getAlerts, 60000);
}

function getPositions() {
  $.get("api/metra/positions/all", function(data) {
      data.forEach(function (d) {

          if (d.realTimeCoordinates == undefined) { return; }
          $("[name='map']").each(function() {
            if (d.tripId === this.attributes.tripId.value) {
                this.setAttribute("gpsTrainLat", d.realTimeCoordinates.latitude);
                this.setAttribute("gpsTrainLon", d.realTimeCoordinates.longitude);
                this.setAttribute("gpsTrainDir", d.direction);
            }
          });

          $("[name='distanceFromStation']").each(function() {
            if (d.tripId === this.attributes.tripId.value) {
              var distStopToDest = getDistance(
                  this.attributes.stopLat.value,
                  this.attributes.stopLon.value,
                  this.attributes.destLat.value,
                  this.attributes.destLon.value
              );
              var distTrainToStop = getDistance(
                  this.attributes.stopLat.value,
                  this.attributes.stopLon.value,
                  d.realTimeCoordinates.latitude,
                  d.realTimeCoordinates.longitude
              );
              var distTrainToDest = getDistance(
                  this.attributes.destLat.value,
                  this.attributes.destLon.value,
                  d.realTimeCoordinates.latitude,
                  d.realTimeCoordinates.longitude
              );

              if (distStopToDest < distTrainToDest) {
                if (this.attributes.intable.value === "true") {
                  this.innerHTML =
                    distTrainToStop + "mi";
                } else {
                  this.innerHTML = "<br />" + distTrainToStop + "mi";
                }
              } else {
                this.innerHTML = "";
              }
            }
          });
      });
  });

  setTimeout(getPositions, 30000);
}

function getDistance(lat1, lon1, lat2, lon2) {
  var EARTH_RADIUS = 3959;

  var startLatRadians = getRadians(lat1);
  var destLatRadians = getRadians(lat2);
  var deltaLatRadians = getRadians(lat2 - lat1);
  var detlaLonRadians = getRadians(lon2 - lon1);

  var a =
    Math.sin(deltaLatRadians / 2) * Math.sin(deltaLatRadians / 2) +
    Math.cos(startLatRadians) *
      Math.cos(destLatRadians) *
      Math.sin(detlaLonRadians / 2) *
      Math.sin(detlaLonRadians / 2);

  var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
  return Math.round(EARTH_RADIUS * c * 100) / 100;
}

function getRadians(degrees) {
  return degrees * (Math.PI / 180);
}

function getLocation() {
  if (navigator.geolocation) {
    navigator.geolocation.getCurrentPosition(loadStopsByLocation);
  }
}

function loadStopsByLocation(position) {
  var fromComboBox = $("#stopsFrom");
  var toComboBox = $("#stopsTo");

  fromComboBox.empty();
  toComboBox.empty();

  $.get(
    "api/metra/StopsByDistance?lat=" +
      position.coords.latitude +
      "&lon=" +
      position.coords.longitude,
    function(data) {
      data.forEach(function(d) {
        fromComboBox.append(new Option(d.name, d.id));
      });
    }
  );

  $.get("api/metra/StopsByDistance?distance=1", function(data) {
    data.forEach(function(d) {
      toComboBox.append(new Option(d.name, d.id));
    });
  });
}

function switchStops() {
  if (window.location.href.indexOf("?") > -1) {
    var params = window.location.href.split("?")[1];

    var oldStart = params.split("&")[0].split("=")[1];
    var oldDest = params.split("&")[1].split("=")[1];
    var oldExpress = params.split("&")[2].split("=")[1];

    window.location.href =
      "?start=" + oldDest + "&dest=" + oldStart + "&expressOnly=" + oldExpress;
  } else {
    window.location.href =
      window.location.href + "?start=CUS&dest=ROUTE59&expressOnly=false";
  }
}
