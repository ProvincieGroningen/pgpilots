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

        public static WindMeasurement FromJson(string collectedData)
        {
            var serializer = new DataContractJsonSerializer(typeof (WindMeasurement));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(collectedData)))
            {
                return (WindMeasurement) serializer.ReadObject(ms);
            }
        }
    }
}