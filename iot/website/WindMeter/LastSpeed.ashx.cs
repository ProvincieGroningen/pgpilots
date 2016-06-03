using System;
using System.Web;

namespace WindMeter
{
    public class LastSpeed : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(Global.LastReceivedWindMeasurement == null
                ? DefaultValue
                : Global.LastReceivedWindMeasurement.Speed);
        }

        private decimal DefaultValue => 0;

        public bool IsReusable => false;
    }
}