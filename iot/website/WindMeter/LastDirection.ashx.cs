using System;
using System.Web;

namespace WindMeter
{
    public class LastDirection : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(Global.LastReceivedWindMeasurement == null
                ? DefaultValue
                : (int) Global.LastReceivedWindMeasurement.Direction);
        }

        private static int DefaultValue => 0;

        public bool IsReusable => false;
    }
}