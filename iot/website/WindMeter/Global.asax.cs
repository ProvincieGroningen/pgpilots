using System;
using System.Collections.Generic;
using System.Linq;

namespace WindMeter
{
    public class Global : System.Web.HttpApplication
    {
        private const string MqttBrokerHostname = "croft.thethings.girovito.nl";

        public static readonly Dictionary<string, string> Nodes = new Dictionary<string, string>
        {
            {"02017401", "Brug 1"},
            {"02017402", "Brug 2"},
        };

        private static MqttWorker _mqttWorker;
        public static readonly Dictionary<string, WindMeasurement> LastReceivedWindMeasurements = new Dictionary<string, WindMeasurement>();
        public static DateTime LastReceived { get; set; }
        public static string Debug { get; private set; }

        protected void Application_Start(object sender, EventArgs e)
        {
            try
            {
                _mqttWorker = new MqttWorker(MqttBrokerHostname, Nodes.Keys.ToArray());
                _mqttWorker.WindReadEvent += _mqttWorker_WindReadEvent;
                _mqttWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Debug = ex.Message;
            }
        }

        private void _mqttWorker_WindReadEvent(object sender, WindReadEventArgs e)
        {
            var measurement = e.WindMeasurement;
            measurement.ReceivedAt = DateTime.Now;
            measurement.NodeDescription = Nodes[measurement.NodeEui];
            if (LastReceivedWindMeasurements.ContainsKey(measurement.NodeEui))
            {
                LastReceivedWindMeasurements[measurement.NodeEui] = measurement;
            }
            else
            {
                LastReceivedWindMeasurements.Add(measurement.NodeEui, measurement);
            }
        }

        protected void Application_End(object sender, EventArgs e)
        {
            _mqttWorker.CancelAsync();
        }
    }
}