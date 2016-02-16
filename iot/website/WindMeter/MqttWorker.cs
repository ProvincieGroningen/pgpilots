using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace WindMeter
{
    public class WindReadEventArgs : EventArgs
    {
        public WindMeasurement WindMeasurement;
    }

    public class MqttWorker : BackgroundWorker
    {
        private readonly MqttClient client;

        public delegate void WindReadEventHandler(object sender, WindReadEventArgs e);

        public event WindReadEventHandler WindReadEvent;

        public MqttWorker(string brokerHostname, string[] nodes)
        {
            client = new MqttClient(brokerHostname);
            client.Connect(Guid.NewGuid().ToString());
            client.Subscribe(nodes, new[] {MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE});
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            base.OnDoWork(e);
            while (!CancellationPending)
            {
                Thread.Sleep(1000);
                Debug.Print($"Mqtt thread running {Thread.CurrentThread.ManagedThreadId}");
            }
        }

        protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            base.OnRunWorkerCompleted(e);
            client.Disconnect();
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var rawMessage = Encoding.UTF8.GetString(e.Message);
            var thingsMessage = ThingsMessage.FromJsonSingle(rawMessage);
            WindReadEvent?.Invoke(this,
                new WindReadEventArgs
                {
                    WindMeasurement = WindMeasurement.FromJson(thingsMessage.DataPlain),
                });
        }
    }
}