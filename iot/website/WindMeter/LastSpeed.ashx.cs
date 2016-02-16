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
                ? new Random().Next(1, 16)
                : Global.LastReceivedWindMeasurement.Speed);
        }

        public bool IsReusable => false;
    }
}