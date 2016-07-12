using System.Web;
using System.Web.Script.Serialization;

namespace WindMeter
{
    public class CurrentData : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(Global.LastReceivedWindMeasurements == null
                ? DefaultValue
                : LastReceivedWindMeasurements());
        }

        string LastReceivedWindMeasurements()
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(Global.LastReceivedWindMeasurements);
        }

        private static string DefaultValue => "";

        public bool IsReusable => false;
    }
}