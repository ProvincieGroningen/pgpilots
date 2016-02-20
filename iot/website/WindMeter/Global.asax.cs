using System;

namespace WindMeter
{
    public class Global : System.Web.HttpApplication
    {
        private const string MqttBrokerHostname = "croft.thethings.girovito.nl";
        //private static readonly string[] Nodes = {"nodes/02A29723/packets"};
        private static readonly string[] Nodes = {"nodes/02017401/packets"};

        private static MqttWorker _mqttWorker;
        public static WindMeasurement LastReceivedWindMeasurement { get; set; }
        public static DateTime LastReceived { get; set; }

        protected void Application_Start(object sender, EventArgs e)
        {
            _mqttWorker = new MqttWorker(MqttBrokerHostname, Nodes);
            _mqttWorker.WindReadEvent += _mqttWorker_WindReadEvent;
            _mqttWorker.RunWorkerAsync();
        }

        private void _mqttWorker_WindReadEvent(object sender, WindReadEventArgs e)
        {
            LastReceivedWindMeasurement = e.WindMeasurement;
            LastReceived = DateTime.Now;
        }

        protected void Application_End(object sender, EventArgs e)
        {
            _mqttWorker.CancelAsync();
        }
    }
}