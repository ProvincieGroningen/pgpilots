using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace WindMeter
{
    [DataContract]
    public class ThingsMessage
    {
        //  base64-encoded decrypted Data (if encrypted with standard key)
        [DataMember(Name = "data")] public string Data;
        [DataMember(Name = "dataRate")] public string DataRate;
        [DataMember(Name = "frequency")] public decimal Frequency;
        [DataMember(Name = "gatewayEui")] public string GatewayEui;
        [DataMember(Name = "nodeEui")] public string NodeEui;
        [DataMember(Name = "rawData")] public string RawData; // the unencrypted payload
        [DataMember(Name = "rssi")] public int Rssi;
        [DataMember(Name = "snr")] public decimal Snr;
        [DataMember(Name = "time")] public string Time;

        public DateTime TimeParsed => Convert.ToDateTime(Time);

        public static bool IsAscii(string value)
        {
            // ASCII encoding replaces non-ascii with question marks, so we use UTF8 to see if multi-byte sequences are there
            return Encoding.UTF8.GetByteCount(value) == value.Length;
        }

        /// ascii version of decrypted Data (if Data decode-able into ascii)
        public string DataPlain
        {
            get
            {
                var s = Encoding.ASCII.GetString(Convert.FromBase64String(Data ?? ""));
                return IsAscii(s) ? s.Replace("\0", string.Empty) : string.Empty;
            }
        }

        public static ThingsMessage FromJsonSingle(string json)
        {
            var serializer = new DataContractJsonSerializer(typeof (ThingsMessage));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return (ThingsMessage) serializer.ReadObject(ms);
            }
        }

        public static ThingsMessage[] FromJsonMulti(string json)
        {
            var serializer = new DataContractJsonSerializer(typeof (ThingsMessage[]));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return (ThingsMessage[]) serializer.ReadObject(ms);
            }
        }
    }
}