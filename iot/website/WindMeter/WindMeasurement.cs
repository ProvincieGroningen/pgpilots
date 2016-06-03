using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace WindMeter
{
    [DataContract]
    public class WindMeasurement
    {
        [DataMember(Name = "I")] public int Instance;
        [DataMember(Name = "S")] public decimal Speed;
        [DataMember(Name = "R")] public decimal Direction;
        [DataMember(Name = "NodeEui")] public string NodeEui;
        [DataMember(Name = "NodeDescription")] public string NodeDescription;
        [DataMember(Name = "ReceivedAt")] public DateTime ReceivedAt;

        public static WindMeasurement FromJson(string collectedData, string nodeEui)
        {
            var serializer = new DataContractJsonSerializer(typeof (WindMeasurement));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(collectedData)))
            {
                var measurement = (WindMeasurement) serializer.ReadObject(ms);
                measurement.NodeEui = nodeEui;
                return measurement;
            }
        }
    }
}