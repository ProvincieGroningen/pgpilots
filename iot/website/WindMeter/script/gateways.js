var speedData;
var speedLimitData;
var timeData;
var speedLimit = 6;

function init() {
    speedData = new Array();
    timeData = new Array();
    speedLimitData = new Array();
    readMeters();

    initChartData();
    ReadWindMeasurement();
}

function initChartData() {
    for (var i = 0; i < Meters.length; i++) {
        speedData[i] = new Array();
    }
    for (var j = 0; j < 60; j++) {
        timeData[j] = "";
        speedLimitData[j] = speedLimit;
        for (var i = 0; i < Meters.length; i++) {
            speedData[i][j] = 0;
        }
    }
}

function ReadWindMeasurement() {
    if (chart == null) drawChart();

    readData();
    updateChart();
    setTimeout(ReadWindMeasurement, 1000);
}

var Meters;

function readMeters() {
    $.ajax({
        type: "GET",
        url: "meters.ashx",
        dataType: 'json',
        async: false,
        success: function(meters) {
            Meters = meters;
            var container = $("#meters");
            for (var i = 0; i < meters.length; i++) {
                $("<input />",
                    { type: "checkbox", id: "cb" + meters[i].NodeEui, value: meters[i].NodeEui, checked: "true" })
                    .appendTo(container);
                $("<label />",
                    {
                        'for': "cb" + meters[i].NodeEui,
                        'class': "legendColor" + i,
                        html: "&nbsp;&nbsp;"
                    })
                    .appendTo(container);
                $("<label />",
                    {
                        'for': "cb" + meters[i].NodeEui,
                        text: meters[i].NodeDescription
                    })
                    .appendTo(container);
                $("<br />").appendTo(container);
            }
        }
    });
}

function readData() {
    $.getJSON("currentdata.ashx",
        function(data) {
            for (var i = 0; i < Meters.length; i++) {
                if ($("#cb" + Meters[i].NodeEui).is(":checked")) {
                    $("#needle" + i).css('visibility', 'visible');
                } else {
                    $("#needle" + i).css('visibility', 'hidden');
                }
                var measurement = data[Meters[i].NodeEui];
                if (measurement) {
                    $("#needle" + i).rotate({ animateTo: -measurement.Direction });
                    setSpeed(i, measurement.Speed);
                }
            }
            setTime();
        });
}

var numberOfSpeedNotches = 24;
var lastLowSpeedNotch = 12;
var speedPerNotch = 2;

function initSpeed() {
    var i = 0;
    while (i < numberOfSpeedNotches) {
        $("#speedContainer").prepend('<div id="speed' + i + '" class="speed"></div>');
        i++;
    }
}

function setSpeed(meterIndex, speed) {
    var i = 0;
    while (i < numberOfSpeedNotches) {
        var active = (i <= speed * speedPerNotch);
        var high = (i > lastLowSpeedNotch);

        var style = (high ? "high" : "low") + (active ? "Active" : "Passive");
        $("#speed" + i).attr("class", "speed " + style);
        i++;
    }
    speedData[meterIndex].push(speed);
}

function setTime() {
    var d = new Date();
    if (d.getSeconds() % 5 === 0)
        timeData.push(addZero(d.getHours()) + ":" + addZero(d.getMinutes()) + ":" + addZero(d.getSeconds()));
    else {
        timeData.push("");
    }
}

var chart;
var colors = ["red", "green", "blue", "blueviolet"];

function drawChart() {
    var dataSets = new Array();
    for (var i = 0; i < Meters.length; i++) {
        if ($("#cb" + Meters[i].NodeEui).is(":checked")) {
            dataSets.push(
            {
                label: "Windsnelheid " + Meters[i].NodeDescription,
                fillColor: colors[i], //"rgba(220,220,220,0.2)",
                strokeColor: colors[i], //"rgba(0,0,255,1)",
                pointColor: "rgba(220,220,220,1)",
                pointStrokeColor: "#fff",
                pointHighlightFill: "#fff",
                pointHighlightStroke: "rgba(220,220,220,1)",
                data: speedData[i].slice(-60)
            });
        }
    }

    dataSets.push(
    {
        label: "WindsnelheidLimiet",
        strokeColor: "rgba(255,0,0,1)",
        pointColor: "rgba(220,220,220,1)",
        pointStrokeColor: "#fff",
        pointHighlightFill: "#fff",
        pointHighlightStroke: "rgba(220,220,220,1)",
        data: speedLimitData.slice(-60)
    });

    var data = {
        labels: timeData.slice(-60),
        datasets: dataSets
    };
    var ctx = document.getElementById("chart").getContext("2d");
    chart = new Chart(ctx).Line(data,
    {
        pointDot: false,
        bezierCurve: false,
        datasetFill: false,
        animation: false,
        scaleLineColor: "rgba(0,0,0,1)",
        scaleFontColor: "#000"
    });
}

function updateChart() {
    chart.destroy();
    drawChart();
    return;
    var newData = Array();
    for (var i = 0; i < Meters.length; i++) {
        speedData[i] = speedData[i].slice(-60);
        if ($("#cb" + Meters[i].NodeEui).is(":checked")) {
            newData.push(speedData[i][59]);
        }
    }
    newData.push(speedLimit);
    timeData = timeData.slice(-60);
    chart.removeData();
    chart.addData(newData, timeData[59]);
    chart.update();
}

function addZero(i) {
    if (i < 10) {
        i = "0" + i;
    }
    return i;
}