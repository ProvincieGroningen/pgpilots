<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="WindMeter._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="style/main.css" rel="stylesheet"/>
    <link href='https://fonts.googleapis.com/css?family=Source+Code+Pro' rel="stylesheet" type="text/css"/>

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.2.0/jquery.min.js"></script>
    <script src="script/jqueryrotate.js"></script>
    <script>
        ReadWindMeasurement();

        function ReadWindMeasurement() {
            readDirection();
            readSpeed();
            readReceived();
            setTimeout(ReadWindMeasurement, 1000);
        }

        function readDirection() {
            var rq = new XMLHttpRequest();
            rq.addEventListener("load", function() {
                var direction = parseInt(this.responseText);
                var rotation = (direction - 1) * 22.5;

                $("#directionName").text(directionName(direction));
                $("#directionValue").text(rotation + ' graden');
                $("#compass").rotate({ animateTo: rotation });
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
        }

        function directionName(direction) {
            if (direction === 1) return "Noord";
            if (direction === 2) return "NoordNoordWest";
            if (direction === 3) return "NoordWest";
            if (direction === 4) return "WestNoordWest";
            if (direction === 5) return "West";
            if (direction === 6) return "WestZuidWest";
            if (direction === 7) return "ZuidWest";
            if (direction === 8) return "ZuidZuidWest";
            if (direction === 9) return "Zuid";
            if (direction === 10) return "ZuidZuidOost";
            if (direction === 11) return "ZuidOost";
            if (direction === 12) return "OostZuidOost";
            if (direction === 13) return "Oost";
            if (direction === 14) return "OostNoordOost";
            if (direction === 15) return "NoordOost";
            if (direction === 16) return "NoordNoordOost";
            return "onbekend;";
        }
    </script>
</head>
<body>
<form id="form1" runat="server">
    <div id="">
        <div>
            <span id="receivedValue"></span>
        </div>
        <div>
            <span id="speedValue"></span>
        </div>
        <div>
            <span id="directionName"></span>
        </div>
        <div>
            <span id="directionValue"></span>
        </div>
        <div id="speedContainer">
        </div>
    </div>
    <div>
        <span id="compassSpan">
                <img id="compass" class="rotate0" src="image/compass.png" alt="kompas"/></span>
    </div>
    <script>initSpeed();</script>
</form>
</body>
</html>