var speedData;
var speedLimitData;
var timeData;
var speedLimit = 6;

function init() {
    speedData = new Array();
    timeData = new Array();
    speedLimitData = new Array();

    initChartData();
    initSpeed();
    ReadWindMeasurement();
}

function initChartData() {
    for (var i = 0; i < 60; i++) {
        speedData[i] = 0;
        timeData[i] = 0;
        speedLimitData[i] = speedLimit;
    }
}

function ReadWindMeasurement() {
    if (chart == null) drawChart();

    readDirection();
    readSpeed();
    readReceived();
    updateChart();
    setTimeout(ReadWindMeasurement, 1000);
}

function readDirection() {
    var rq = new XMLHttpRequest();
    rq.addEventListener("load", function() {
        var direction = parseFloat(this.responseText);

        $("#directionName").text(directionName(direction));
        $("#directionValue").text(direction + ' graden');
        $("#compass").rotate({ animateTo: -direction });
    });
    rq.open("GET", "LastDirection.ashx");
    rq.send();
}

function readSpeed() {
    var rq = new XMLHttpRequest();
    rq.addEventListener("load", function() {
        var speed = this.responseText;
        $("#speedValue").text(speed + 'm/s');
        setSpeed(parseFloat(speed));
    });
    rq.open("GET", "LastSpeed.ashx");
    rq.send();
}

function readReceived() {
    var rq = new XMLHttpRequest();
    rq.addEventListener("load", function() {
        var received = this.responseText;
        $("#receivedValue").text(received + 's');
    });
    rq.open("GET", "LastReceived.ashx");
    rq.send();
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
    setSpeed(0);
}

function setSpeed(speed) {
    var i = 0;
    while (i < numberOfSpeedNotches) {
        var active = (i <= speed * speedPerNotch);
        var high = (i > lastLowSpeedNotch);

        var style = (high ? 'high' : 'low')
            + (active ? 'Active' : 'Passive');
        $("#speed" + i).attr('class', "speed " + style);
        i++;
    }
    speedData.push(speed);
    var d = new Date();
    if (d.getSeconds() % 5 === 0)
        timeData.push(addZero(d.getHours()) + ":" + addZero(d.getMinutes()) + ":" + addZero(d.getSeconds()));
    else {
        timeData.push("");
    }
}

function directionName(direction) {
    if (direction < 11.25) return "Noord";
    if (direction < 33.75) return "NoordNoordOost";
    if (direction < 56.25) return "NoordOost";
    if (direction < 78.75) return "OostNoordOost";
    if (direction < 101.25) return "Oost";
    if (direction < 123.75) return "OostZuidOost";
    if (direction < 146.25) return "ZuidOost";
    if (direction < 168.75) return "ZuidZuidOost";
    if (direction < 191.25) return "Zuid";
    if (direction < 213.75) return "ZuidZuidWest";
    if (direction < 236.25) return "ZuidWest";
    if (direction < 258.75) return "WestZuidWest";
    if (direction < 281.25) return "West";
    if (direction < 303.75) return "WestNoordWest";
    if (direction < 326.25) return "NoordWest";
    if (direction < 348.75) return "NoordNoordWest";
    return "Noord";
}

var chart;

function drawChart() {
    var data = {
        labels: timeData.slice(-60),
        datasets: [
            {
                label: "Windsnelheid",
                fillColor: "rgba(220,220,220,0.2)",
                strokeColor: "rgba(0,0,255,1)",
                pointColor: "rgba(220,220,220,1)",
                pointStrokeColor: "#fff",
                pointHighlightFill: "#fff",
                pointHighlightStroke: "rgba(220,220,220,1)",
                data: speedData.slice(-60),
            },
            {
                label: "WindsnelheidLimiet",
                strokeColor: "rgba(255,0,0,1)",
                pointColor: "rgba(220,220,220,1)",
                pointStrokeColor: "#fff",
                pointHighlightFill: "#fff",
                pointHighlightStroke: "rgba(220,220,220,1)",
                data: speedLimitData.slice(-60),
            }
        ]
    };
    var ctx = document.getElementById("chart").getContext("2d");
    chart = new Chart(ctx).Line(data, {
        pointDot: false,
        bezierCurve: false,
        datasetFill: false,
        animation: false,
        scaleLineColor: "rgba(0,0,0,1)",
        scaleFontColor: "#000",
    });
}

function updateChart() {
    speedData = speedData.slice(-60);
    timeData = timeData.slice(-60);
    chart.removeData();
    chart.addData([speedData[59], speedLimit], timeData[59]);
    chart.update();
}

function addZero(i) {
    if (i < 10) {
        i = "0" + i;
    }
    return i;
}