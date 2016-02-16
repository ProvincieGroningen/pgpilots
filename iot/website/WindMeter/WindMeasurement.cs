using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace WindMeter
{
    [DataContract(Name = "s")]
    public enum WindDirection
    {
        [EnumMember(Value = "1")] Noord,
        [EnumMember(Value = "2")] NoordNoordWest,
        [EnumMember(Value = "3")] NoordWest,
        [EnumMember(Value = "4")] WestNoordWest,
        [EnumMember(Value = "5")] West,
        [EnumMember(Value = "6")] WestZuidWest,
        [EnumMember(Value = "7")] ZuidWest,
        [EnumMember(Value = "8")] ZuidZuidWest,
        [EnumMember(Value = "9")] Zuid,
        [EnumMember(Value = "10")] ZuidZuidOost,
        [EnumMember(Value = "11")] ZuidOost,
        [EnumMember(Value = "12")] OostZuidOost,
        [EnumMember(Value = "13")] Oost,
        [EnumMember(Value = "14")] OostNoordOost,
        [EnumMember(Value = "15")] NoordOost,
        [EnumMember(Value = "16")] NoordNoordOost,
    }

    [DataContract]
    public class WindMeasurement
    {
        [DataMember(Name = "I")] public int Instance;
        [DataMember(Name = "S")] public decimal Speed;
        [DataMember(Name = "R")] public WindDirection Direction;

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